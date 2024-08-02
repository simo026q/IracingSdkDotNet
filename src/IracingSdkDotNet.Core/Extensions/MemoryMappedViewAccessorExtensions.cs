using IracingSdkDotNet.Core.Internal;
using System.IO.MemoryMappedFiles;
using System.Text;

namespace IracingSdkDotNet.Core.Extensions;

internal static class MemoryMappedViewAccessorExtensions
{
    public static string ReadString(this MemoryMappedViewAccessor accessor, int offset, int length, int minLength = 0)
    {
        StringBuilder sb = minLength > 0 
            ? new(minLength) 
            : new();

        for (int i = 0; i < length; i++)
        {
            char c = (char)accessor.ReadByte(offset + i);

            if (c != Constants.EndChar)
            {
                sb.Append(c);
            }
        }

        return sb.ToString();
    }

    public static string ReadString(this MemoryMappedViewAccessor accessor, int position, int count, Encoding encoding)
    {
        byte[] data = new byte[count];
        accessor.ReadArray(position, data, 0, count);
        return encoding.GetString(data).TrimEnd(Constants.EndChar);
    }
    
    public static T[] ReadArray<T>(this MemoryMappedViewAccessor accessor, int position, int count) 
        where T : struct
    {
        var value = new T[count];
        accessor.ReadArray(position, value, 0, count);
        return value;
    }
}