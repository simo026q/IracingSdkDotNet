using System.Collections.Generic;

namespace IracingSdkDotNet.Serialization.Models.Session.DriverInfo;

public class DriverInfoModel
{
    public int DriverCarIdx { get; set; } // %d
    public int DriverUserID { get; set; } // %d
    public int PaceCarIdx { get; set; } // %d
    public float DriverHeadPosX { get; set; } // %.3f
    public float DriverHeadPosY { get; set; } // %.3f
    public float DriverHeadPosZ { get; set; } // %.3f
    public string DriverCarIsElectric { get; set; } // %d (boolean)
    public float DriverCarIdleRPM { get; set; } // %.3f
    public float DriverCarRedLine { get; set; } // %.3f
    public int DriverCarEngCylinderCount { get; set; } // %d
    public float DriverCarFuelKgPerLtr { get; set; } // %.3f
    public float DriverCarFuelMaxLtr { get; set; } // %.3f
    public float DriverCarMaxFuelPct { get; set; } // %.3f
    public int DriverCarGearNumForward { get; set; } // %d
    public int DriverCarGearNeutral { get; set; } // %d
    public int DriverCarGearReverse { get; set; } //%d
    public float DriverCarSLFirstRPM { get; set; } // %.3f
    public float DriverCarSLShiftRPM { get; set; } // %.3f
    public float DriverCarSLLastRPM { get; set; } // %.3f
    public float DriverCarSLBlinkRPM { get; set; } // %.3f
    public string DriverCarVersion { get; set; } // %s
    public float DriverPitTrkPct { get; set; } // %.6f
    public float DriverCarEstLapTime { get; set; } // %.4f
    public string DriverSetupName { get; set; } // %s
    public int DriverSetupIsModified { get; set; } // %d (boolean)
    public string DriverSetupLoadTypeName { get; set; } // %s
    public int DriverSetupPassedTech { get; set; } // %d (boolean)
    public int DriverIncidentCount { get; set; } // %d
    public List<DriverModel> Drivers { get; set; }
}
