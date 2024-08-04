using IracingSdkDotNet.Core.Internal;
using System.IO.MemoryMappedFiles;

namespace IracingSdkDotNet.Core.Extensions;

internal static class MemoryMappedViewAccessorExtensions
{
    public static string ReadString(this MemoryMappedViewAccessor accessor, int position, int count)
    {
        byte[] data = new byte[count];
        accessor.ReadArray(position, data, 0, count);
        return Constants.DataEncoding.GetString(data).TrimEnd(Constants.EndChar);
    }
    
    public static T[] ReadArray<T>(this MemoryMappedViewAccessor accessor, int position, int count) 
        where T : struct
    {
        var value = new T[count];
        accessor.ReadArray(position, value, 0, count);
        return value;
    }
}