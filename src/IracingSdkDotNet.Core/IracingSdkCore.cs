using System;
using System.IO;
using System.IO.MemoryMappedFiles;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.Win32.SafeHandles;
using IracingSdkDotNet.Core.Extensions;
using IracingSdkDotNet.Core.Reader;
using IracingSdkDotNet.Core.Internal;
using Microsoft.Extensions.Logging.Abstractions;

namespace IracingSdkDotNet.Core;

/// <summary>
/// A class to interact with iRacing's shared memory.
/// </summary>
public sealed class IracingSdkCore
    : IDisposable
{
    private readonly Encoding _encoding;
    private readonly ILogger<IracingSdkCore> _logger;

    private bool _disposed;
    private CancellationTokenSource? _loopCancellationSource;

    /// <summary>
    /// The options for the SDK.
    /// </summary>
    public IracingSdkOptions Options { get; }

    /// <summary>
    /// Indicates if the SDK has started listening for iRacing's shared memory.
    /// </summary>
    /// <exception cref="ObjectDisposedException">The object has been disposed.</exception>
    public bool IsStarted => _disposed
        ? throw new ObjectDisposedException(nameof(IracingSdkCore))
        : _loopCancellationSource != null && !_loopCancellationSource.IsCancellationRequested;

    /// <summary>
    /// The <see cref="IracingDataReader"/> instance that is used to read the data from iRacing's shared memory.
    /// </summary>
    public IracingDataReader? DataReader { get; private set; }

    /// <summary>
    /// Indicates if the SDK is connected to iRacing.
    /// </summary>
    /// <exception cref="ObjectDisposedException">The object has been disposed.</exception>
    public bool IsConnected => _disposed 
        ? throw new ObjectDisposedException(nameof(IracingSdkCore)) 
        : DataReader != null && (DataReader.Header.Status & 1) > 0;

    /// <summary>
    /// Occurs when the data from iRacing's shared memory has been updated. Should be updated at the same rate as the <see cref="IracingDataHeader.TickRate"/> in the data header (usually 60 Hz).
    /// </summary>
    public event EventHandler<IracingDataReader>? DataUpdated;

    /// <summary>
    /// Occurs when the SDK has connected to iRacing.
    /// </summary>
    public event EventHandler? Connected;

    /// <summary>
    /// Occurs when the SDK has disconnected from iRacing.
    /// </summary>
    public event EventHandler? Disconnected;

    /// <summary>
    /// Initializes a new instance of the <see cref="IracingSdkCore"/> class.
    /// </summary>
    /// <param name="options">Options for the SDK.</param>
    /// <param name="logger">A logger instace to log messages.</param>
    public IracingSdkCore(IracingSdkOptions options, ILogger<IracingSdkCore> logger)
    {
        // Register CP1252 encoding
        Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
        _encoding = Encoding.GetEncoding(1252);

        Options = options;
        _logger = logger;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="IracingSdkCore"/> class.
    /// </summary>
    /// <param name="options">Options for the SDK.</param>
    public IracingSdkCore(IracingSdkOptions options)
        : this(options, NullLogger<IracingSdkCore>.Instance)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="IracingSdkCore"/> class with default options.
    /// </summary>
    /// <param name="logger">A logger instace to log messages.</param>
    public IracingSdkCore(ILogger<IracingSdkCore> logger)
        : this(IracingSdkOptions.Default, logger)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="IracingSdkCore"/> class with default options.
    /// </summary>
    public IracingSdkCore()
        : this(IracingSdkOptions.Default, NullLogger<IracingSdkCore>.Instance)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="IracingSdkCore"/> class with predefined <see cref="MemoryMappedViewAccessor"/>.
    /// </summary>
    /// <remarks>Should only be used for testing purposes.</remarks>
    /// <param name="accessor">The <see cref="MemoryMappedViewAccessor"/> to use for reading data.</param>
    public IracingSdkCore(MemoryMappedViewAccessor accessor)
        : this()
    {
        DataReader = new IracingDataReader(accessor, _encoding);
    }

    /// <summary>
    /// Finalizes an instance of the <see cref="IracingSdkCore"/> class.
    /// </summary>
    ~IracingSdkCore()
    {
        Dispose(false);
    }

    /// <summary>
    /// Starts the SDK and begins listening for a connection to iRacing by checking the shared memory.
    /// </summary>
    /// <exception cref="ObjectDisposedException">The object has been disposed.</exception>
    public void Start()
    {
        if (_disposed)
            throw new ObjectDisposedException(nameof(IracingSdkCore));

        if (IsStarted)
            return;

        _logger.LogStarting();

        _loopCancellationSource = new();

        Task.Factory.StartNew(
            () => ConnectionLoop(_loopCancellationSource.Token),
            _loopCancellationSource.Token,
            TaskCreationOptions.LongRunning,
            TaskScheduler.Default);
    }

    /// <summary>
    /// Stops the SDK and disconnects from iRacing.
    /// </summary>
    /// <exception cref="ObjectDisposedException">The object has been disposed.</exception>
    public void Stop()
    {
        if (_disposed)
            throw new ObjectDisposedException(nameof(IracingSdkCore));

        _logger.LogStopping();

        _loopCancellationSource?.Cancel();
        _loopCancellationSource?.Dispose();
        _loopCancellationSource = null;
    }

    /// <summary>
    /// Disposes the object and stops the SDK.
    /// </summary>
    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    /// <summary>
    /// Disposes the object and stops the SDK.
    /// </summary>
    /// <param name="disposing">Indicates if the object is being disposed.</param>
    private void Dispose(bool disposing)
    {
        if (_disposed)
        {
            return;
        }

        if (disposing)
        {
            // Dispose managed resources
        }

        Stop();
        DataReader?.Dispose();
        DataReader = null;

        _disposed = true;
    }

    private async Task ConnectionLoop(CancellationToken cancellationToken)
    {
        CancellationTokenSource? dataCts = null;

        while (!cancellationToken.IsCancellationRequested)
        {
            if (!IsConnected)
            {
                if (DataReader == null)
                {
                    try
                    {
                        using var memoryMappedFile = MemoryMappedFile.OpenExisting(Constants.MemMapFileName, MemoryMappedFileRights.Read);
                        MemoryMappedViewAccessor viewAccessor = memoryMappedFile.CreateViewAccessor(0L, 0L, MemoryMappedFileAccess.Read);
                        DataReader = new IracingDataReader(viewAccessor, _encoding);

                        _logger.LogMemoryMappedFileOpened();

                        nint eventHandle = NativeMethods.OpenEvent(Constants.DesiredAccess, false, Constants.DataValidEventName);
                        var safeWaitHandle = new SafeWaitHandle(eventHandle, true);

                        var autoResetEvent = new AutoResetEvent(false) 
                        {
                            SafeWaitHandle = safeWaitHandle
                        };

                        dataCts = new();

                        _ = Task.Factory.StartNew(
                            () => DataLoop(autoResetEvent, dataCts.Token),
                            dataCts.Token,
                            TaskCreationOptions.LongRunning,
                            TaskScheduler.Default);

                        continue;
                    }
                    catch (FileNotFoundException ex)
                    {
                        _logger.LogMemoryMappedFileOpenFailed(ex);
                    }
                }

                try
                {
                    _logger.LogWaitingForConnectionRetry(Options.CheckConnectionDelay);
                    await Task.Delay(Options.CheckConnectionDelay, cancellationToken);
                }
                catch (OperationCanceledException)
                {
                    _logger.LogConnectionRetryWaitCancelled();
                    break;
                }
            }
        }

#if NET8_0_OR_GREATER
        if (dataCts != null)
        {
            await dataCts.CancelAsync();
        }
#else
        dataCts?.Cancel();
#endif
        dataCts = null;

        _logger.LogConnectionLoopExited();
    }

    private async Task DataLoop(AutoResetEvent autoResetEvent, CancellationToken cancellationToken)
    {
        bool wasValid = false;

        while (!cancellationToken.IsCancellationRequested && DataReader != null)
        {
            try
            {
                bool valid = await autoResetEvent.WaitOneAsync(cancellationToken);

                if (valid)
                {
                    if (!wasValid)
                    {
                        wasValid = true;
                        Connected?.Invoke(this, EventArgs.Empty);
                        _logger.LogConnected();
                    }

                    DataUpdated?.Invoke(this, DataReader);
                    _logger.LogDataUpdated();
                }
                else if (wasValid)
                {
                    Disconnected?.Invoke(this, EventArgs.Empty);
                    _logger.LogDisconnected();
                }
            }
            catch (Exception ex)
            {
                _logger.LogDataLoopWaitError(ex);
            }
        }

        _logger.LogDataLoopExited();
    }
}