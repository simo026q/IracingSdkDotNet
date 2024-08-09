using IracingSdkDotNet.Core.Internal;
using System.IO.MemoryMappedFiles;

namespace IracingSdkDotNet.Core.Extensions;

internal static class MemoryMappedViewAccessorExtensions
{
    public static string ReadString(this MemoryMappedViewAccessor accessor, int position, int count)
    {
        byte[] bytes = accessor.ReadArray<byte>(position, count);

        return Constants.MemoryMappedFileEncoding
            .GetString(bytes)
            .TrimEnd(Constants.StringTerminator);
    }
    
    public static T[] ReadArray<T>(this MemoryMappedViewAccessor accessor, int position, int count) 
        where T : struct
    {
        var value = new T[count];
        accessor.ReadArray(position, value, 0, count);
        return value;
    }
}