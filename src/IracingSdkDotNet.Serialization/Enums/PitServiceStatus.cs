namespace IracingSdkDotNet.Serialization.Enums;

public enum PitServiceStatus
{
    // status
    None = 0,
    InProgress,
    Complete,

    // errors
    TooFarLeft = 100,
    TooFarRight,
    TooFarForward,
    TooFarBack,
    BadAngle,
    Unrepairable
}