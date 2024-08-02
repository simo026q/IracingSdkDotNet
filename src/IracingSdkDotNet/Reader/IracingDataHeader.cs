using System.IO.MemoryMappedFiles;

namespace IracingSdkDotNet.Reader;

public sealed class IracingDataHeader(MemoryMappedViewAccessor viewAccessor)
{
    private readonly MemoryMappedViewAccessor _viewAccessor = viewAccessor;

    public int Version => _viewAccessor.ReadInt32(0);
    public int Status => _viewAccessor.ReadInt32(4);
    public int TickRate => _viewAccessor.ReadInt32(8);
    public int SessionInfoUpdate => _viewAccessor.ReadInt32(12);
    public int SessionInfoLength => _viewAccessor.ReadInt32(16);
    public int SessionInfoOffset => _viewAccessor.ReadInt32(20);
    public int VarCount => _viewAccessor.ReadInt32(24);
    public int VarHeaderOffset => _viewAccessor.ReadInt32(28);
    public int BufferCount => _viewAccessor.ReadInt32(32);
    public int BufferLength => _viewAccessor.ReadInt32(36);

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