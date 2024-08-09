using IracingSdkDotNet.Core.Extensions;
using System;
using System.Collections.Generic;
using System.IO.MemoryMappedFiles;
using IracingSdkDotNet.Core.Internal;

#if NET8_0_OR_GREATER
using System.Diagnostics.CodeAnalysis;
#endif

namespace IracingSdkDotNet.Core.Reader;

/// <summary>
/// A reader used to read data from the iRacing shared memory.
/// </summary>
public sealed class IracingDataReader : IDisposable
{
    private readonly MemoryMappedViewAccessor _viewAccessor;

    private bool _disposed;
    private IracingDataHeader? _header;
    private Dictionary<string, VariableHeader>? _variableHeaders;

    internal IracingDataReader(MemoryMappedViewAccessor viewAccessor)
    {
        _viewAccessor = viewAccessor;
    }

    /// <summary>
    /// Finalizes an instance of the <see cref="IracingDataReader"/> class.
    /// </summary>
    ~IracingDataReader()
    {
        Dispose(false);
    }

    /// <summary>
    /// The <see cref="IracingDataHeader"/> of the current data.
    /// </summary>
    public IracingDataHeader Header => _disposed
        ? throw new ObjectDisposedException(nameof(IracingDataReader))
        : _header ??= new IracingDataHeader(_viewAccessor);

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
            : _viewAccessor.ReadString(Header.SessionInfoOffset, Header.SessionInfoLength);
    }

    private Dictionary<string, VariableHeader> ReadVariableHeaders()
    {
        var varHeaders = new Dictionary<string, VariableHeader>(Header.VariableCount, StringComparer.InvariantCultureIgnoreCase);

        for (int i = 0; i < Header.VariableCount; i++)
        {
            int positionOffset = Header.VariableHeaderOffset + i * VariableHeader.Size;

            var type = (VariableType)_viewAccessor.ReadInt32(positionOffset);
            int offset = _viewAccessor.ReadInt32(positionOffset + Constants.VarOffsetOffset);
            int count = _viewAccessor.ReadInt32(positionOffset + Constants.VarCountOffset);

            string name = _viewAccessor.ReadString(positionOffset + Constants.VarNameOffset, Constants.MaxString);
            string desc = _viewAccessor.ReadString(positionOffset + Constants.VarDescOffset, Constants.MaxDesc);
            string unit = _viewAccessor.ReadString(positionOffset + Constants.VarUnitOffset, Constants.MaxString);

            varHeaders[name] = new VariableHeader(type, offset, count, name, desc, unit);
        }

        return varHeaders;
    }

#if NET8_0_OR_GREATER
    private bool TryReadHeaderValue<T>(string name, VariableType type, Func<VariableHeader, int, T> unsafeReader, [MaybeNullWhen(false)] out T value)
#else
    private bool TryReadHeaderValue<T>(string name, VariableType type, Func<VariableHeader, int, T> unsafeReader, out T? value)
#endif
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
#if NET8_0_OR_GREATER
    public bool TryReadString(string name, [MaybeNullWhen(false)] out string value)
#else
    public bool TryReadString(string name, out string? value)
#endif
    {
        return TryReadHeaderValue(name, VariableType.Char, ReadString, out value);

        string ReadString(VariableHeader header, int position)
        {
            return _viewAccessor.ReadString(position, header.Count);
        }
    }

    /// <summary>
    /// Tries to read a boolean from the shared memory.
    /// </summary>
    /// <param name="name">The name of the variable to read. Case-insensitive.</param>
    /// <param name="value">The value of the variable if it exists.</param>
    /// <returns><see langword="true"/> if the variable exists and is a boolean; otherwise, <see langword="false"/>.</returns>
#if NET8_0_OR_GREATER
    public bool TryReadBoolean(string name, [MaybeNullWhen(false)] out bool value)
#else
    public bool TryReadBoolean(string name, out bool value)
#endif
    {
        return TryReadHeaderValue(name, VariableType.Boolean, ReadBoolean, out value);

        bool ReadBoolean(VariableHeader _, int position)
        {
            return _viewAccessor.ReadBoolean(position);
        }
    }

    /// <summary>
    /// Tries to read a boolean array from the shared memory.
    /// </summary>
    /// <param name="name">The name of the variable to read. Case-insensitive.</param>
    /// <param name="value">The value of the variable if it exists.</param>
    /// <returns><see langword="true"/> if the variable exists and is a boolean array; otherwise, <see langword="false"/>.</returns>
#if NET8_0_OR_GREATER
    public bool TryReadBooleanArray(string name, [MaybeNullWhen(false)] out bool[] value)
#else
    public bool TryReadBooleanArray(string name, out bool[]? value)
#endif
    {
        return TryReadHeaderValue(name, VariableType.Boolean, ReadBooleanArray, out value);

        bool[] ReadBooleanArray(VariableHeader header, int position)
        {
            return _viewAccessor.ReadArray<bool>(position, header.Count);
        }
    }

    /// <summary>
    /// Tries to read an integer value from the shared memory.
    /// </summary>
    /// <param name="name">The name of the variable to read. Case-insensitive.</param>
    /// <param name="value">The value of the variable if it exists.</param>
    /// <returns><see langword="true"/> if the variable exists and is an integer; otherwise, <see langword="false"/>.</returns>
#if NET8_0_OR_GREATER
    public bool TryReadInt32(string name, [MaybeNullWhen(false)] out int value)
#else
    public bool TryReadInt32(string name, out int value)
#endif
    {
        return TryReadHeaderValue(name, VariableType.Int32, ReadInt32, out value);

        int ReadInt32(VariableHeader _, int position)
        {
            return _viewAccessor.ReadInt32(position);
        }
    }

    /// <summary>
    /// Tries to read an integer array from the shared memory.
    /// </summary>
    /// <param name="name">The name of the variable to read. Case-insensitive.</param>
    /// <param name="value">The value of the variable if it exists.</param>
    /// <returns><see langword="true"/> if the variable exists and is an integer array; otherwise, <see langword="false"/>.</returns>
#if NET8_0_OR_GREATER
    public bool TryReadInt32Array(string name, [MaybeNullWhen(false)] out int[] value)
#else
    public bool TryReadInt32Array(string name, out int[]? value)
#endif
    {
        return TryReadHeaderValue(name, VariableType.Int32, ReadInt32Array, out value);

        int[] ReadInt32Array(VariableHeader header, int position)
        {
            return _viewAccessor.ReadArray<int>(position, header.Count);
        }
    }

    /// <summary>
    /// Tries to read a bit field value from the shared memory.
    /// </summary>
    /// <param name="name">The name of the variable to read. Case-insensitive.</param>
    /// <param name="value">The value of the variable if it exists.</param>
    /// <returns><see langword="true"/> if the variable exists and is a bit field; otherwise, <see langword="false"/>.</returns>
#if NET8_0_OR_GREATER
    public bool TryReadBitField<T>(string name, [MaybeNullWhen(false)] out T value)
#else
    public bool TryReadBitField<T>(string name, out T value)
#endif
        where T : struct, Enum
    {
        return TryReadHeaderValue(name, VariableType.BitField, ReadBitField, out value);

        T ReadBitField(VariableHeader _, int position)
        {
            _viewAccessor.Read(position, out T v);
            return v;
        }
    }

    /// <summary>
    /// Tries to read a bit field array from the shared memory.
    /// </summary>
    /// <param name="name">The name of the variable to read. Case-insensitive.</param>
    /// <param name="value">The value of the variable if it exists.</param>
    /// <returns><see langword="true"/> if the variable exists and is a bit field array; otherwise, <see langword="false"/>.</returns>
#if NET8_0_OR_GREATER
    public bool TryReadBitFieldArray<T>(string name, [MaybeNullWhen(false)] out T[] value)
#else
    public bool TryReadBitFieldArray<T>(string name, out T[]? value)
#endif
        where T : struct, Enum
    {
        return TryReadHeaderValue(name, VariableType.BitField, ReadBitFieldArray, out value);

        T[] ReadBitFieldArray(VariableHeader header, int position)
        {
            return _viewAccessor.ReadArray<T>(position, header.Count);
        }
    }

    /// <summary>
    /// Tries to read a single-precision floating point value from the shared memory.
    /// </summary>
    /// <param name="name">The name of the variable to read. Case-insensitive.</param>
    /// <param name="value">The value of the variable if it exists.</param>
    /// <returns><see langword="true"/> if the variable exists and is a single-precision floating point value; otherwise, <see langword="false"/>.</returns>
#if NET8_0_OR_GREATER
    public bool TryReadSingle(string name, [MaybeNullWhen(false)] out float value)
#else
    public bool TryReadSingle(string name, out float value)
#endif
    {
        return TryReadHeaderValue(name, VariableType.Single, ReadSingle, out value);

        float ReadSingle(VariableHeader _, int position)
        {
            return _viewAccessor.ReadSingle(position);
        }
    }

    /// <summary>
    /// Tries to read a single-precision floating point array from the shared memory.
    /// </summary>
    /// <param name="name">The name of the variable to read. Case-insensitive.</param>
    /// <param name="value">The value of the variable if it exists.</param>
    /// <returns><see langword="true"/> if the variable exists and is a single-precision floating point array; otherwise, <see langword="false"/>.</returns>
#if NET8_0_OR_GREATER
    public bool TryReadSingleArray(string name, [MaybeNullWhen(false)] out float[] value)
#else
    public bool TryReadSingleArray(string name, out float[]? value)
#endif
    {
        return TryReadHeaderValue(name, VariableType.Single, ReadSingleArray, out value);

        float[] ReadSingleArray(VariableHeader header, int position)
        {
            return _viewAccessor.ReadArray<float>(position, header.Count);
        }
    }

    /// <summary>
    /// Tries to read a double-precision floating point value from the shared memory.
    /// </summary>
    /// <param name="name">The name of the variable to read. Case-insensitive.</param>
    /// <param name="value">The value of the variable if it exists.</param>
    /// <returns><see langword="true"/> if the variable exists and is a double-precision floating point value; otherwise, <see langword="false"/>.</returns>
#if NET8_0_OR_GREATER
    public bool TryReadDouble(string name, [MaybeNullWhen(false)] out double value)
#else
    public bool TryReadDouble(string name, out double value)
#endif
    {
        return TryReadHeaderValue(name, VariableType.Double, ReadDouble, out value);

        double ReadDouble(VariableHeader _, int position)
        {
            return _viewAccessor.ReadDouble(position);
        }
    }

    /// <summary>
    /// Tries to read a double-precision floating point array from the shared memory.
    /// </summary>
    /// <param name="name">The name of the variable to read. Case-insensitive.</param>
    /// <param name="value">The value of the variable if it exists.</param>
    /// <returns><see langword="true"/> if the variable exists and is a double-precision floating point array; otherwise, <see langword="false"/>.</returns>
#if NET8_0_OR_GREATER
    public bool TryReadDoubleArray(string name, [MaybeNullWhen(false)] out double[] value)
#else
    public bool TryReadDoubleArray(string name, out double[]? value)
#endif
    {
        return TryReadHeaderValue(name, VariableType.Double, ReadDoubleArray, out value);

        double[] ReadDoubleArray(VariableHeader header, int position)
        {
            return _viewAccessor.ReadArray<double>(position, header.Count);
        }
    }

    /// <summary>
    /// Tries to read a value from the shared memory.
    /// </summary>
    /// <param name="name">The name of the variable to read. Case-insensitive.</param>
    /// <param name="value">The value of the variable if it exists.</param>
    /// <returns><see langword="true"/> if the variable exists; otherwise, <see langword="false"/>.</returns>
#if NET8_0_OR_GREATER
    public bool TryReadValue(string name, [MaybeNullWhen(false)] out object value)
#else
    public bool TryReadValue(string name, out object? value)
#endif
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
                value = _viewAccessor.ReadString(position, varHeader.Count);
                return true;

            case VariableType.Boolean:
                value = varHeader.Count > 1
                    ? _viewAccessor.ReadArray<bool>(position, varHeader.Count)
                    : _viewAccessor.ReadBoolean(position);

                return true;

            case VariableType.Int32:
            case VariableType.BitField:
                value = varHeader.Count > 1
                    ? _viewAccessor.ReadArray<int>(position, varHeader.Count)
                    : _viewAccessor.ReadInt32(position);

                return true;

            case VariableType.Single:
                value = varHeader.Count > 1
                    ? _viewAccessor.ReadArray<float>(position, varHeader.Count)
                    : _viewAccessor.ReadSingle(position);

                return true;

            case VariableType.Double:
                value = varHeader.Count > 1
                    ? _viewAccessor.ReadArray<double>(position, varHeader.Count)
                    : _viewAccessor.ReadDouble(position);

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

        _viewAccessor.Dispose();

        _disposed = true;
    }
}