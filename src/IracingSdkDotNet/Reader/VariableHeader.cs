namespace IracingSdkDotNet.Reader;

public sealed class VariableHeader(VariableType type, int offset, int count, string name, string description, string unit)
{
    public const int Size = 144;

    public VariableType Type { get; } = type;
    public int Offset { get; } = offset;
    public int Count { get; } = count;

    public string Name { get; } = name;
    public string Description { get; } = description;
    public string Unit { get; } = unit;

    public int Bytes => Type switch
    {
        VariableType.Char or VariableType.Boolean => 1,
        VariableType.Int32 or VariableType.BitField or VariableType.Single => 4,
        VariableType.Double => 8,
        _ => 0
    };

    public int Length => Bytes * Count;
}