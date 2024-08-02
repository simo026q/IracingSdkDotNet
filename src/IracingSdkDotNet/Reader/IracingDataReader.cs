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

public sealed class IracingDataReader(MemoryMappedViewAccessor viewAccessor, Encoding encoding)
    : IDisposable
{
    private readonly Encoding _encoding = encoding;

    private bool _disposed;
    private IracingDataHeader? _header;
    private Dictionary<string, VariableHeader>? _variableHeaders;

    internal MemoryMappedViewAccessor ViewAccessor { get; } = viewAccessor;

    public IracingDataHeader Header => _disposed
        ? throw new ObjectDisposedException(nameof(IracingDataReader))
        : _header ??= new IracingDataHeader(ViewAccessor);

    public Dictionary<string, VariableHeader> VariableHeaders => _disposed
        ? throw new ObjectDisposedException(nameof(IracingDataReader))
        : _variableHeaders ??= ReadVariableHeaders();

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
        var varHeaders = new Dictionary<string, VariableHeader>(Header.VarCount);

        for (int i = 0; i < Header.VarCount; i++)
        {
            int positionOffset = Header.VarHeaderOffset + i * VariableHeader.Size;

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

    public bool TryReadString(string name, out string? value)
    {
        return TryReadHeaderValue(name, VariableType.Char, ReadString, out value);

        string ReadString(VariableHeader header, int position)
        {
            return ViewAccessor.ReadString(position, header.Count, _encoding);
        }
    }

    public bool TryReadBoolean(string name, out bool value)
    {
        return TryReadHeaderValue(name, VariableType.Boolean, ReadBoolean, out value);

        bool ReadBoolean(VariableHeader _, int position)
        {
            return ViewAccessor.ReadBoolean(position);
        }
    }

    public bool TryReadBooleanArray(string name, out bool[]? value)
    {
        return TryReadHeaderValue(name, VariableType.Boolean, ReadBooleanArray, out value);

        bool[] ReadBooleanArray(VariableHeader header, int position)
        {
            return ViewAccessor.ReadArray<bool>(position, header.Count);
        }
    }

    public bool TryReadInt32(string name, out int value)
    {
        return TryReadHeaderValue(name, VariableType.Int32, ReadInt32, out value);

        int ReadInt32(VariableHeader _, int position)
        {
            return ViewAccessor.ReadInt32(position);
        }
    }

    public bool TryReadInt32Array(string name, out int[]? value)
    {
        return TryReadHeaderValue(name, VariableType.Int32, ReadInt32Array, out value);

        int[] ReadInt32Array(VariableHeader header, int position)
        {
            return ViewAccessor.ReadArray<int>(position, header.Count);
        }
    }
    
    public bool TryReadBitField(string name, out int value)
    {
        return TryReadHeaderValue(name, VariableType.BitField, ReadBitField, out value);

        int ReadBitField(VariableHeader _, int position)
        {
            return ViewAccessor.ReadInt32(position);
        }
    }
    
    public bool TryReadBitFieldArray(string name, out int[]? value)
    {
        return TryReadHeaderValue(name, VariableType.BitField, ReadBitFieldArray, out value);

        int[] ReadBitFieldArray(VariableHeader header, int position)
        {
            return ViewAccessor.ReadArray<int>(position, header.Count);
        }
    }

    public bool TryReadSingle(string name, out float value)
    {
        return TryReadHeaderValue(name, VariableType.Single, ReadSingle, out value);

        float ReadSingle(VariableHeader _, int position)
        {
            return ViewAccessor.ReadSingle(position);
        }
    }

    public bool TryReadSingleArray(string name, out float[]? value)
    {
        return TryReadHeaderValue(name, VariableType.Single, ReadSingleArray, out value);

        float[] ReadSingleArray(VariableHeader header, int position)
        {
            return ViewAccessor.ReadArray<float>(position, header.Count);
        }
    }

    public bool TryReadDouble(string name, out double value)
    {
        return TryReadHeaderValue(name, VariableType.Double, ReadDouble, out value);

        double ReadDouble(VariableHeader _, int position)
        {
            return ViewAccessor.ReadDouble(position);
        }
    }

    public bool TryReadDoubleArray(string name, out double[]? value)
    {
        return TryReadHeaderValue(name, VariableType.Double, ReadDoubleArray, out value);

        double[] ReadDoubleArray(VariableHeader header, int position)
        {
            return ViewAccessor.ReadArray<double>(position, header.Count);
        }
    }

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