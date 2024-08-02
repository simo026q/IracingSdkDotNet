using System;
using System.IO;
using System.IO.MemoryMappedFiles;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.Win32.SafeHandles;
using IracingSdkDotNet.Extensions;
using IracingSdkDotNet.Reader;
using IracingSdkDotNet.Internal;

namespace IracingSdkDotNet;

public sealed class IracingSdk
    : IDisposable
{
    private readonly Encoding _encoding;
    private readonly ILogger<IracingSdk>? _logger;

    private bool _disposed;
    private CancellationTokenSource? _loopCancellationSource;

    public IracingSdkOptions Options { get; }

    public bool IsStarted => _disposed
        ? throw new ObjectDisposedException(nameof(IracingSdk))
        : _loopCancellationSource != null && !_loopCancellationSource.IsCancellationRequested;

    public IracingDataReader? DataReader { get; private set; }

    public bool IsConnected => _disposed 
        ? throw new ObjectDisposedException(nameof(IracingSdk)) 
        : DataReader != null && (DataReader.Header.Status & 1) > 0;

    public event EventHandler<IracingDataReader>? DataUpdated;
    public event EventHandler? Connected;
    public event EventHandler? Disconnected;

    public IracingSdk(IracingSdkOptions? options = null, ILogger<IracingSdk>? logger = null)
    {
        // Register CP1252 encoding
        Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
        _encoding = Encoding.GetEncoding(1252);

        Options = options ?? IracingSdkOptions.Default;

        _logger = logger;
    }

    public IracingSdk(MemoryMappedViewAccessor accessor)
        : this()
    {
        DataReader = new IracingDataReader(accessor, _encoding);
    }

    public void Start()
    {
        if (_disposed)
            throw new ObjectDisposedException(nameof(IracingSdk));

        if (IsStarted)
            return;

        _loopCancellationSource = new();

        Task.Factory.StartNew(
            () => ConnectionLoop(_loopCancellationSource.Token),
            _loopCancellationSource.Token,
            TaskCreationOptions.LongRunning,
            TaskScheduler.Default);

        _logger?.LogInformation("Started the Sdk.");
    }

    public void Stop()
    {
        if (_disposed)
            throw new ObjectDisposedException(nameof(IracingSdk));

        _loopCancellationSource?.Cancel();
        _loopCancellationSource?.Dispose();
        _loopCancellationSource = null;

        _logger?.LogInformation("Stopped the Sdk.");
    }

    public void Dispose()
    {
        if (_disposed)
            return;

        Stop();

        DataReader?.Dispose();
        DataReader = null;

        _disposed = true;

        GC.SuppressFinalize(this);
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
                        MemoryMappedViewAccessor viewAccessor = memoryMappedFile.CreateViewAccessor(0, 0, MemoryMappedFileAccess.Read);
                        DataReader = new IracingDataReader(viewAccessor, _encoding);

                        _logger?.LogDebug("Created IracingDataReader.");

                        nint eventHandle = OpenEvent(Constants.DesiredAccess, false, Constants.DataValidEventName);
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
                        _logger?.LogWarning(ex, "Could not open iRacing's memory mapped file.");
                    }
                }

                try
                {
                    _logger?.LogInformation("Waiting {CheckConnectionDelay} before retrying to connect to iRacing.", Options.CheckConnectionDelay);
                    await Task.Delay(Options.CheckConnectionDelay, cancellationToken);
                }
                catch (OperationCanceledException)
                {
                    _logger?.LogTrace("The CancellationToken was cancelled while waiting for next connect retry to iRacing.");
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

        _logger?.LogDebug("Exited the connection loop.");
    }

    private async Task DataLoop(AutoResetEvent autoResetEvent, CancellationToken cancellationToken)
    {
        bool wasValid = false;

        while (!cancellationToken.IsCancellationRequested && DataReader != null)
        {
            try
            {
                var valid = await autoResetEvent.WaitOneAsync(cancellationToken);

                if (valid)
                {
                    if (!wasValid)
                    {
                        wasValid = true;
                        Connected?.Invoke(this, EventArgs.Empty);
                        _logger?.LogInformation("Connected to iRacing.");
                    }

                    DataUpdated?.Invoke(this, DataReader);
                    _logger?.LogTrace("Data changed.");
                }
                else if (wasValid)
                {
                    Disconnected?.Invoke(this, EventArgs.Empty);
                    _logger?.LogWarning("Disconnected from iRacing.");
                }
            }
            catch (Exception ex)
            {
                _logger?.LogWarning(ex, "Error while waiting in data loop");
            }
        }

        _logger?.LogDebug("Exited the data loop.");
    }

    private static IntPtr GetBroadcastMessageId()
    {
        return RegisterWindowMessage(Constants.BroadcastMessageName);
    }

    /*private static IntPtr GetPadCarNumId()
    {
        return RegisterWindowMessage(Constants.PadCarNumName);
    }*/

    public static int BroadcastMessage(BroadcastMessageType msg, int var1, int var2, int var3)
    {
        return BroadcastMessage(msg, var1, MakeLong((short)var2, (short)var3));
    }

    public static int BroadcastMessage(BroadcastMessageType msg, int var1, int var2)
    {
        IntPtr msgId = GetBroadcastMessageId();
        IntPtr hwndBroadcast = IntPtr.Add(IntPtr.Zero, 0xffff);
        IntPtr result = IntPtr.Zero;
        if (msgId != IntPtr.Zero)
        {
            result = PostMessage(hwndBroadcast, msgId.ToInt32(), MakeLong((short)msg, (short)var1), var2);
        }
        return result.ToInt32();
    }

    [DllImport("user32.dll")]
    private static extern IntPtr RegisterWindowMessage(string lpProcName);

    //[DllImport("user32.dll")]
    //private static extern IntPtr SendMessage(IntPtr hWnd, int Msg, int wParam, int lParam);

    [DllImport("user32.dll")]
    private static extern IntPtr PostMessage(IntPtr hWnd, int Msg, int wParam, int lParam);

    [DllImport("Kernel32.dll", CharSet = CharSet.Auto)]
    private static extern IntPtr OpenEvent(uint dwDesiredAccess, bool bInheritHandle, string lpName);

    private static int MakeLong(short lowPart, short highPart)
    {
        return (int)(((ushort)lowPart) | (uint)(highPart << 16));
    }
}