namespace IracingSdkDotNet.Serialization.Models.Session.WeekendInfo;

public class WeekendOptionsModel
{
    public int NumStarters { get; set; } // %d
    public string StartingGrid { get; set; } // %s
    public string QualifyScoring { get; set; } // %s
    public string CourseCautions { get; set; } // %s
    public int StandingStart { get; set; } // %d (boolean)
    public int ShortParadeLap { get; set; } // %d (boolean)
    public string Restarts { get; set; } // %s
    public string WeatherType { get; set; } // %s
    public string Skies { get; set; } // %s
    public string WindDirection { get; set; } // %s
    public string WindSpeed { get; set; } // %0.2f km/h
    public string WeatherTemp { get; set; } // %0.2f C
    public string RelativeHumidity { get; set; } // %d %
    public string FogLevel { get; set; } // %d %
    public string TimeOfDay { get; set; } // %s
    public string Date { get; set; } // %s
    public string EarthRotationSpeedupFactor { get; set; } // %d
    public int Unofficial { get; set; } // %d (boolean)
    public string CommercialMode { get; set; } // %s
    public string NightMode { get; set; } // %s
    public int IsFixedSetup { get; set; } // %d (boolean)
    public string StrictLapsChecking { get; set; } // %s
    public int HasOpenRegistration { get; set; } // %d (boolean)
    public int HardcoreLevel { get; set; } // %d
    public int NumJokerLaps { get; set; } // %d
    public string IncidentLimit { get; set; } // %d or "unlimited"
    public string FastRepairsLimit { get; set; } // %d or "unlimited"
    public int GreenWhiteCheckeredLimit { get; set; } // %d
}
