using IracingSdkDotNet.Serialization.Internal.Yaml;

namespace IracingSdkDotNet.Serialization.Models.Session.WeekendInfo;

public class WeekendInfoModel
{
    public string TrackName { get; set; } // %s
    public int TrackID { get; set; } // %d

    [SingleUnitYamlConverterFactory(" km")]
    public float TrackLength { get; set; } // %0.2f km

    [SingleUnitYamlConverterFactory(" km")]
    public float TrackLengthOfficial { get; set; } // %0.2f km
    public string TrackDisplayName { get; set; } // %s
    public string TrackDisplayShortName { get; set; } // %s
    public string TrackConfigName { get; set; } // %s
    public string TrackCity { get; set; } // %s
    public string TrackCountry { get; set; } // %s

    [SingleUnitYamlConverterFactory(" m")]
    public float TrackAltitude { get; set; } // %0.2f m

    [SingleUnitYamlConverterFactory(" m")]
    public float TrackLatitude { get; set; } // %0.6f m

    [SingleUnitYamlConverterFactory(" m")]
    public float TrackLongitude { get; set; } // %0.6f m

    [SingleUnitYamlConverterFactory(" rad")]
    public float TrackNorthOffset { get; set; } // %0.4f rad
    public int TrackNumTurns { get; set; } // %d

    [SingleUnitYamlConverterFactory(" kph")]
    public float TrackPitSpeedLimit { get; set; } // %0.2f kph
    public string TrackType { get; set; } // %s
    public string TrackDirection { get; set; } // %s
    public string TrackWeatherType { get; set; } // %s
    public string TrackSkies { get; set; } // %s

    [SingleUnitYamlConverterFactory(" C")]
    public float TrackSurfaceTemp { get; set; } // %0.2f C

    [SingleUnitYamlConverterFactory(" C")]
    public float TrackAirTemp { get; set; } // %0.2f C

    [SingleUnitYamlConverterFactory(" Hg")]
    public float TrackAirPressure { get; set; } // %0.2f Hg

    [SingleUnitYamlConverterFactory(" m/s")]
    public float TrackWindVel { get; set; } // %0.2f m/s

    [SingleUnitYamlConverterFactory(" rad")]
    public float TrackWindDir { get; set; } // %0.2f rad

    [YamlConverter(typeof(PercentageYamlConverter))]
    public float TrackRelativeHumidity { get; set; } // %d %

    [YamlConverter(typeof(PercentageYamlConverter))]
    public float TrackFogLevel { get; set; } // %d %

    [YamlConverter(typeof(PercentageYamlConverter))]
    public float TrackPrecipitation { get; set; } // %d %
    public int TrackCleanup { get; set; } // %d (?boolean)
    public int TrackDynamicTrack { get; set; } // %d (?boolean)
    public string TrackVersion { get; set; } // %s
    public int SeriesID { get; set; } // %d
    public int SeasonID { get; set; } // %d
    public int SessionID { get; set; } // %d
    public int SubSessionID { get; set; } // %d
    public int LeagueID { get; set; } // %d
    public int Official { get; set; } // %d (?boolean)
    public int RaceWeek { get; set; } // %d (?boolean)
    public string EventType { get; set; } // %s
    public string Category { get; set; } // %s
    public string SimMode { get; set; } // %s

    [YamlConverter(typeof(IntegerBooleanYamlConverter))]
    public bool TeamRacing { get; set; } // %d (boolean)
    public int MinDrivers { get; set; } // %d
    public int MaxDrivers { get; set; } // %d
    public string DCRuleSet { get; set; } // %s

    [YamlConverter(typeof(IntegerBooleanYamlConverter))]
    public bool QualifierMustStartRace { get; set; } // %d (boolean)
    public int NumCarClasses { get; set; } // %d
    public int NumCarTypes { get; set; } // %d
    public int HeatRacing { get; set; } // %d (?boolean)
    public string BuildType { get; set; } // %s
    public string BuildTarget { get; set; } // %s
    public string BuildVersion { get; set; } // %s
    public string RaceFarm { get; set; } // %s
    public WeekendOptionsModel WeekendOptions { get; set; }
    public TelemetryOptionsModel TelemetryOptions { get; set; }
}
