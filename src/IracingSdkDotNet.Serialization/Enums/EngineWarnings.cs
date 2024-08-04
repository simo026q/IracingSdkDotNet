using System;

namespace IracingSdkDotNet.Serialization.Enums;

[Flags]
public enum EngineWarnings
{
    WaterTemperature    = 0x01,
    FuelPressure        = 0x02,
    OilPressure         = 0x04,
    Stalled             = 0x08,
    PitSpeedLimiter     = 0x10,
    RevLimiterActive    = 0x20,
    OilTemperature      = 0x40,
}