namespace IracingSdkDotNet.Serialization.Enums;

/// <summary>
/// The spotter's state of the cars alongside us.
/// </summary>
public enum SpotterState // aka irsdk_CarLeftRight
{
    /// <summary>
    /// Spotter is off.
    /// </summary>
    Off,

    /// <summary>
    /// No cars around us.
    /// </summary>
    Clear,

    /// <summary>
    /// There is a car to the left.
    /// </summary>
    Left,

    /// <summary>
    /// There is a car to the right.
    /// </summary>
    Right,

    /// <summary>
    /// There are cars on both sides.
    /// </summary>
    BothSides,

    /// <summary>
    /// There are two cars to the left.
    /// </summary>
    TwoLeft,

    /// <summary>
    /// There are two cars to the right.
    /// </summary>
    TwoRight,
}