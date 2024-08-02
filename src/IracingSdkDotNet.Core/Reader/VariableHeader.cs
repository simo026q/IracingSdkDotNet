namespace IracingSdkDotNet.Core.Reader;

/// <summary>
/// Represents the header of a variable in the iRacing shared memory.
/// </summary>
/// <param name="type">The type of the variable.</param>
/// <param name="offset">The offset of the variable in the shared memory.</param>
/// <param name="count">The number of elements in the variable.</param>
/// <param name="name">The name of the variable.</param>
/// <param name="description">The description of the variable.</param>
/// <param name="unit">The unit of the variable.</param>
public sealed class VariableHeader(VariableType type, int offset, int count, string name, string description, string unit)
{
    /// <summary>
    /// The size of a variable header.
    /// </summary>
    public const int Size = 144;

    /// <summary>
    /// The type of the variable.
    /// </summary>
    public VariableType Type { get; } = type;

    /// <summary>
    /// The offset of the variable in the shared memory.
    /// </summary>
    public int Offset { get; } = offset;

    /// <summary>
    /// The number of elements in the variable.
    /// </summary>
    public int Count { get; } = count;

    /// <summary>
    /// The name of the variable.
    /// </summary>
    public string Name { get; } = name;

    /// <summary>
    /// The description of the variable.
    /// </summary>
    public string Description { get; } = description;

    /// <summary>
    /// The unit of the variable.
    /// </summary>
    public string Unit { get; } = unit;

    /// <summary>
    /// The number of bytes of the variable.
    /// </summary>
    public int Bytes => Type switch
    {
        VariableType.Char or VariableType.Boolean => 1,
        VariableType.Int32 or VariableType.BitField or VariableType.Single => 4,
        VariableType.Double => 8,
        _ => 0
    };

    /// <summary>
    /// The length of the variable in bytes.
    /// </summary>
    public int Length => Bytes * Count;
}