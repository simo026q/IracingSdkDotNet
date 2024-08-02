using IracingSdkDotNet.Extensions;
using System;
using System.Collections.Generic;
using System.IO.MemoryMappedFiles;
using System.Text;
using IracingSdkDotNet.Internal;

#if NET8_0_OR_GREATER
using System.Diagnostics.CodeAnalysis;
#endif

namespace IracingSdkDotNet.Reader;

/// <summary>
/// A reader used to read data from the iRacing shared memory.
/// </summary>
public sealed class IracingDataReader : IDisposable
{
    private readonly Encoding _encoding;

    private bool _disposed;
    private IracingDataHeader? _header;
    private Dictionary<string, VariableHeader>? _variableHeaders;

    internal IracingDataReader(MemoryMappedViewAccessor viewAccessor, Encoding encoding)
    {
        _encoding = encoding;
        ViewAccessor = viewAccessor;
    }

    internal MemoryMappedViewAccessor ViewAccessor { get; }

    /// <summary>
    /// The <see cref="IracingDataHeader"/> of the current data.
    /// </summary>
    public IracingDataHeader Header => _disposed
        ? throw new ObjectDisposedException(nameof(IracingDataReader))
        : _header ??= new IracingDataHeader(ViewAccessor);

    /// <summary>
    /// The variable headers of the current data.
    /// </summary>
    public Dictionary<string, VariableHeader> VariableHeaders => _disposed
        ? throw new ObjectDisposedException(nameof(IracingDataReader))
        : _variableHeaders ??= ReadVariableHeaders();

    /// <summary>
    /// Reads the raw session info from the shared memory.
    /// </summary>
    /// <returns>Session info string in YAML format.</returns>
    /// <exception cref="ObjectDisposedException">The object has been disposed.</exception>
    public string? ReadRawSessionInfo()
    {
        if (_disposed)
        {
            throw new ObjectDisposedException(nameof(IracingDataReader));
        }

        return Header is null
            ? null
            : ViewAccessor.ReadString(Header.SessionInfoOffset, Header.SessionInfoLength);
    }

    private Dictionary<string, VariableHeader> ReadVariableHeaders()
    {
        var varHeaders = new Dictionary<string, VariableHeader>(Header.VariableCount, StringComparer.InvariantCultureIgnoreCase);

        for (int i = 0; i < Header.VariableCount; i++)
        {
            int positionOffset = Header.VariableHeaderOffset + i * VariableHeader.Size;

            var type = (VariableType)ViewAccessor.ReadInt32(positionOffset);
            int offset = ViewAccessor.ReadInt32(positionOffset + Constants.VarOffsetOffset);
            int count = ViewAccessor.ReadInt32(positionOffset + Constants.VarCountOffset);

            string name = ViewAccessor.ReadString(positionOffset + Constants.VarNameOffset, Constants.MaxString, _encoding);
            string desc = ViewAccessor.ReadString(positionOffset + Constants.VarDescOffset, Constants.MaxDesc, _encoding);
            string unit = ViewAccessor.ReadString(positionOffset + Constants.VarUnitOffset, Constants.MaxString, _encoding);

            varHeaders[name] = new VariableHeader(type, offset, count, name, desc, unit);
        }

        return varHeaders;
    }

    private bool TryReadHeaderValue<T>(string name, VariableType type, Func<VariableHeader, int, T> unsafeReader, out T? value)
    {
        if (!VariableHeaders.TryGetValue(name, out VariableHeader? varHeader) || varHeader.Type != type)
        {
            value = default;
            return false;
        }

        int position = Header.Offset + varHeader.Offset;

        try
        {
            value = unsafeReader(varHeader, position);
            return true;
        }
        catch
        {
            value = default;
            return false;
        }
    }

    /// <summary>
    /// Tries to read a string value from the shared memory.
    /// </summary>
    /// <param name="name">The name of the variable to read. Case-insensitive.</param>
    /// <param name="value">The value of the variable if it exists.</param>
    /// <returns><see langword="true"/> if the variable exists and is a string; otherwise, <see langword="false"/>.</returns>
    public bool TryReadString(string name, out string? value)
    {
        return TryReadHeaderValue(name, VariableType.Char, ReadString, out value);

        string ReadString(VariableHeader header, int position)
        {
            return ViewAccessor.ReadString(position, header.Count, _encoding);
        }
    }

    /// <summary>
    /// Tries to read a string array from the shared memory.
    /// </summary>
    /// <param name="name">The name of the variable to read. Case-insensitive.</param>
    /// <param name="value">The value of the variable if it exists.</param>
    /// <returns><see langword="true"/> if the variable exists and is a string array; otherwise, <see langword="false"/>.</returns>
    public bool TryReadBoolean(string name, out bool value)
    {
        return TryReadHeaderValue(name, VariableType.Boolean, ReadBoolean, out value);

        bool ReadBoolean(VariableHeader _, int position)
        {
            return ViewAccessor.ReadBoolean(position);
        }
    }

    /// <summary>
    /// Tries to read a boolean array from the shared memory.
    /// </summary>
    /// <param name="name">The name of the variable to read. Case-insensitive.</param>
    /// <param name="value">The value of the variable if it exists.</param>
    /// <returns><see langword="true"/> if the variable exists and is a boolean array; otherwise, <see langword="false"/>.</returns>
    public bool TryReadBooleanArray(string name, out bool[]? value)
    {
        return TryReadHeaderValue(name, VariableType.Boolean, ReadBooleanArray, out value);

        bool[] ReadBooleanArray(VariableHeader header, int position)
        {
            return ViewAccessor.ReadArray<bool>(position, header.Count);
        }
    }

    /// <summary>
    /// Tries to read an integer value from the shared memory.
    /// </summary>
    /// <param name="name">The name of the variable to read. Case-insensitive.</param>
    /// <param name="value">The value of the variable if it exists.</param>
    /// <returns><see langword="true"/> if the variable exists and is an integer; otherwise, <see langword="false"/>.</returns>
    public bool TryReadInt32(string name, out int value)
    {
        return TryReadHeaderValue(name, VariableType.Int32, ReadInt32, out value);

        int ReadInt32(VariableHeader _, int position)
        {
            return ViewAccessor.ReadInt32(position);
        }
    }

    /// <summary>
    /// Tries to read an integer array from the shared memory.
    /// </summary>
    /// <param name="name">The name of the variable to read. Case-insensitive.</param>
    /// <param name="value">The value of the variable if it exists.</param>
    /// <returns><see langword="true"/> if the variable exists and is an integer array; otherwise, <see langword="false"/>.</returns>
    public bool TryReadInt32Array(string name, out int[]? value)
    {
        return TryReadHeaderValue(name, VariableType.Int32, ReadInt32Array, out value);

        int[] ReadInt32Array(VariableHeader header, int position)
        {
            return ViewAccessor.ReadArray<int>(position, header.Count);
        }
    }
    
    /// <summary>
    /// Tries to read a bit field value from the shared memory.
    /// </summary>
    /// <param name="name">The name of the variable to read. Case-insensitive.</param>
    /// <param name="value">The value of the variable if it exists.</param>
    /// <returns><see langword="true"/> if the variable exists and is a bit field; otherwise, <see langword="false"/>.</returns>
    public bool TryReadBitField(string name, out int value)
    {
        return TryReadHeaderValue(name, VariableType.BitField, ReadBitField, out value);

        int ReadBitField(VariableHeader _, int position)
        {
            return ViewAccessor.ReadInt32(position);
        }
    }
    
    /// <summary>
    /// Tries to read a bit field array from the shared memory.
    /// </summary>
    /// <param name="name">The name of the variable to read. Case-insensitive.</param>
    /// <param name="value">The value of the variable if it exists.</param>
    /// <returns><see langword="true"/> if the variable exists and is a bit field array; otherwise, <see langword="false"/>.</returns>
    public bool TryReadBitFieldArray(string name, out int[]? value)
    {
        return TryReadHeaderValue(name, VariableType.BitField, ReadBitFieldArray, out value);

        int[] ReadBitFieldArray(VariableHeader header, int position)
        {
            return ViewAccessor.ReadArray<int>(position, header.Count);
        }
    }

    /// <summary>
    /// Tries to read a single-precision floating point value from the shared memory.
    /// </summary>
    /// <param name="name">The name of the variable to read. Case-insensitive.</param>
    /// <param name="value">The value of the variable if it exists.</param>
    /// <returns><see langword="true"/> if the variable exists and is a single-precision floating point value; otherwise, <see langword="false"/>.</returns>
    public bool TryReadSingle(string name, out float value)
    {
        return TryReadHeaderValue(name, VariableType.Single, ReadSingle, out value);

        float ReadSingle(VariableHeader _, int position)
        {
            return ViewAccessor.ReadSingle(position);
        }
    }

    /// <summary>
    /// Tries to read a single-precision floating point array from the shared memory.
    /// </summary>
    /// <param name="name">The name of the variable to read. Case-insensitive.</param>
    /// <param name="value">The value of the variable if it exists.</param>
    /// <returns><see langword="true"/> if the variable exists and is a single-precision floating point array; otherwise, <see langword="false"/>.</returns>
    public bool TryReadSingleArray(string name, out float[]? value)
    {
        return TryReadHeaderValue(name, VariableType.Single, ReadSingleArray, out value);

        float[] ReadSingleArray(VariableHeader header, int position)
        {
            return ViewAccessor.ReadArray<float>(position, header.Count);
        }
    }

    /// <summary>
    /// Tries to read a double-precision floating point value from the shared memory.
    /// </summary>
    /// <param name="name">The name of the variable to read. Case-insensitive.</param>
    /// <param name="value">The value of the variable if it exists.</param>
    /// <returns><see langword="true"/> if the variable exists and is a double-precision floating point value; otherwise, <see langword="false"/>.</returns>
    public bool TryReadDouble(string name, out double value)
    {
        return TryReadHeaderValue(name, VariableType.Double, ReadDouble, out value);

        double ReadDouble(VariableHeader _, int position)
        {
            return ViewAccessor.ReadDouble(position);
        }
    }

    /// <summary>
    /// Tries to read a double-precision floating point array from the shared memory.
    /// </summary>
    /// <param name="name">The name of the variable to read. Case-insensitive.</param>
    /// <param name="value">The value of the variable if it exists.</param>
    /// <returns><see langword="true"/> if the variable exists and is a double-precision floating point array; otherwise, <see langword="false"/>.</returns>
    public bool TryReadDoubleArray(string name, out double[]? value)
    {
        return TryReadHeaderValue(name, VariableType.Double, ReadDoubleArray, out value);

        double[] ReadDoubleArray(VariableHeader header, int position)
        {
            return ViewAccessor.ReadArray<double>(position, header.Count);
        }
    }

    /// <summary>
    /// Tries to read a value from the shared memory.
    /// </summary>
    /// <param name="name">The name of the variable to read. Case-insensitive.</param>
    /// <param name="value">The value of the variable if it exists.</param>
    /// <returns><see langword="true"/> if the variable exists; otherwise, <see langword="false"/>.</returns>
    public bool TryReadValue(string name, out object? value)
    {
        if (!VariableHeaders.TryGetValue(name, out VariableHeader? varHeader))
        {
            value = default;
            return false;
        }

        int position = Header.Offset + varHeader.Offset;

        switch (varHeader.Type)
        {
            case VariableType.Char:
                value = ViewAccessor.ReadString(position, varHeader.Count, _encoding);
                return true;

            case VariableType.Boolean:
                value = varHeader.Count > 1
                    ? ViewAccessor.ReadArray<bool>(position, varHeader.Count)
                    : ViewAccessor.ReadBoolean(position);

                return true;

            case VariableType.Int32:
            case VariableType.BitField:
                value = varHeader.Count > 1
                    ? ViewAccessor.ReadArray<int>(position, varHeader.Count)
                    : ViewAccessor.ReadInt32(position);

                return true;

            case VariableType.Single:
                value = varHeader.Count > 1
                    ? ViewAccessor.ReadArray<float>(position, varHeader.Count)
                    : ViewAccessor.ReadSingle(position);

                return true;

            case VariableType.Double:
                value = varHeader.Count > 1
                    ? ViewAccessor.ReadArray<double>(position, varHeader.Count)
                    : ViewAccessor.ReadDouble(position);

                return true;

            default:
                value = default;
                return false;
        }
    }

    /// <summary>
    /// Disposes the <see cref="IracingDataReader"/>.
    /// </summary>
    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    private void Dispose(bool disposing)
    {
        if (_disposed)
        {
            return;
        }

        if (disposing)
        {
            _variableHeaders?.Clear();
            _variableHeaders = null;
            _header = null;
        }

        ViewAccessor.Dispose();

        _disposed = true;
    }
}