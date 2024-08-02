namespace IracingSdkDotNet.Core.Reader;

/// <summary>
/// Represents the value type of a <see cref="VariableHeader"/>.
/// </summary>
public enum VariableType
{
    /// <summary>
    /// A character.
    /// </summary>
    Char,

    /// <summary>
    /// A boolean.
    /// </summary>
    Boolean,

    /// <summary>
    /// A 32-bit integer.
    /// </summary>
    Int32,

    /// <summary>
    /// A bit field.
    /// </summary>
    BitField,

    /// <summary>
    /// A single precision floating point number.
    /// </summary>
    Single,

    /// <summary>
    /// A double precision floating point number.
    /// </summary>
    Double
}