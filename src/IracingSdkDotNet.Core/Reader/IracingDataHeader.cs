using System.IO.MemoryMappedFiles;

namespace IracingSdkDotNet.Core.Reader;

/// <summary>
/// Represents the header of the iRacing shared memory.
/// </summary>
/// <param name="viewAccessor">The <see cref="MemoryMappedViewAccessor"/> to read the data from.</param>
public sealed class IracingDataHeader(MemoryMappedViewAccessor viewAccessor)
{
    private readonly MemoryMappedViewAccessor _viewAccessor = viewAccessor;

    /// <summary>
    /// The version of the header. Should be 2.
    /// </summary>
    public int Version => _viewAccessor.ReadInt32(0);

    /// <summary>
    /// The iRacing status.
    /// </summary>
    public int Status => _viewAccessor.ReadInt32(4);

    /// <summary>
    /// The telemetry update tick rate.
    /// </summary>
    public int TickRate => _viewAccessor.ReadInt32(8);

    /// <summary>
    /// The current update of the session info. When this changes, the session info has been updated.
    /// </summary>
    public int SessionInfoUpdate => _viewAccessor.ReadInt32(12);

    /// <summary>
    /// The length of the session info. Used to read the session info.
    /// </summary>
    public int SessionInfoLength => _viewAccessor.ReadInt32(16);

    /// <summary>
    /// The offset of the session info. Used to read the session info.
    /// </summary>
    public int SessionInfoOffset => _viewAccessor.ReadInt32(20);

    /// <summary>
    /// The number of variables in the shared memory.
    /// </summary>
    public int VariableCount => _viewAccessor.ReadInt32(24);

    /// <summary>
    /// The offset of the variable header. Used to read the variable header.
    /// </summary>
    public int VariableHeaderOffset => _viewAccessor.ReadInt32(28);

    /// <summary>
    /// The number of buffers in the shared memory.
    /// </summary>
    public int BufferCount => _viewAccessor.ReadInt32(32);

    /// <summary>
    /// The length of the buffers in the shared memory.
    /// </summary>
    public int BufferLength => _viewAccessor.ReadInt32(36);

    /// <summary>
    /// The offset of the buffers in the shared memory.
    /// </summary>
    public int Offset
    {
        get
        {
            int maxTickCount = _viewAccessor.ReadInt32(48);
            int curOffset = _viewAccessor.ReadInt32(48 + 4);
            for (var i = 1; i < BufferCount; i++)
            {
                var curTick = _viewAccessor.ReadInt32(48 + i * 16);
                if (maxTickCount < curTick)
                {
                    maxTickCount = curTick;
                    curOffset = _viewAccessor.ReadInt32(48 + i * 16 + 4);
                }
            }
            return curOffset;
        }
    }
}