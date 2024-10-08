﻿using System.Collections.Generic;

namespace IracingSdkDotNet.Serialization.Models.Session.SessionInfo;

public class SessionModel
{
    public int SessionNum { get; set; } // %d
    public string SessionLaps { get; set; } // %d or "unlimited"
    public string SessionTime { get; set; } // %0.4f sec
    public int SessionNumLapsToAvg { get; set; } // %d
    public string SessionType { get; set; } // %s
    public string SessionTrackRubberState { get; set; } // %s
    public string SessionName { get; set; } // %0.2f sec
    public string SessionSubType { get; set; } // %s
    public int SessionSkipped { get; set; } // %d (?boolean)
    public int SessionRunGroupsUsed { get; set; } // %d (?boolean)
    public int SessionEnforceTireCompoundChange { get; set; } // %d (?boolean)
    public List<PositionModel> ResultsPositions { get; set; }
    public List<FastestLapModel> ResultsFastestLap { get; set; }
    public float ResultsAverageLapTime { get; set; } // %.4f
    public int ResultsNumCautionFlags { get; set; } // %d
    public int ResultsNumCautionLaps { get; set; } // %d
    public int ResultsNumLeadChanges { get; set; } // %d
    public int ResultsLapsComplete { get; set; } // %d
    public int ResultsOfficial { get; set; } // %d (?boolean)
}
