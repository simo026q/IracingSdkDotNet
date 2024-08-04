using System;

namespace IracingSdkDotNet.Serialization.Enums;

[Flags]
public enum PitServiceFlags
{
    LeftFrontTireChange     = 0x0001,
    RightFrontTireChange    = 0x0002,
    LeftRearTireChange      = 0x0004,
    RightRearTireChange     = 0x0008,

    RefillFuel              = 0x0010,
    WindshieldTearoff       = 0x0020,
    FastRepair              = 0x0040
}