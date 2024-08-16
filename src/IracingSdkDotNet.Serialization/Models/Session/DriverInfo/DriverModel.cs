using IracingSdkDotNet.Serialization.Internal.Yaml;

namespace IracingSdkDotNet.Serialization.Models.Session.DriverInfo;

public class DriverModel
{
    public int CarIdx { get; set; } // %d
    public string UserName { get; set; } // %s
    public string AbbrevName { get; set; } // %s
    public string Initials { get; set; } // %s
    public int UserID { get; set; } // %d
    public int TeamID { get; set; } // %d
    public string TeamName { get; set; } // %s
    public string CarNumber { get; set; } // "%s"
    public int CarNumberRaw { get; set; } // %d
    public string CarPath { get; set; } // %s
    public int CarClassID { get; set; } // %d
    public int CarID { get; set; } // %d

    [YamlConverter(typeof(IntegerBooleanYamlConverter))]
    public bool CarIsPaceCar { get; set; } // %d (boolean)

    [YamlConverter(typeof(IntegerBooleanYamlConverter))]
    public bool CarIsAI { get; set; } // %d (boolean)

    [YamlConverter(typeof(IntegerBooleanYamlConverter))]
    public bool CarIsElectric { get; set; } // %d (boolean)
    public string CarScreenName { get; set; } // %s
    public string CarScreenNameShort { get; set; } // %s
    public string CarClassShortName { get; set; } // %s
    public int CarClassRelSpeed { get; set; } // %d
    public int CarClassLicenseLevel { get; set; } // %d

    [YamlConverter(typeof(PercentageYamlConverter))]
    public float CarClassMaxFuelPct { get; set; } // %.3f %

    [SingleUnitYamlConverterFactory(" kg")]
    public float CarClassWeightPenalty { get; set; } // %.3f kg

    [YamlConverter(typeof(PercentageYamlConverter))]
    public float CarClassPowerAdjust { get; set; } // %.3f %

    [YamlConverter(typeof(PercentageYamlConverter))]
    public float CarClassDryTireSetLimit { get; set; } // %d %
    public string CarClassColor { get; set; } // 0x%02x%02x%02x
    public float CarClassEstLapTime { get; set; } // %.4f
    public int IRating { get; set; } // %d
    public int LicLevel { get; set; } // %d
    public int LicSubLevel { get; set; } // %d
    public string LicString { get; set; } // %s
    public string LicColor { get; set; } // 0x%s

    [YamlConverter(typeof(IntegerBooleanYamlConverter))]
    public bool IsSpectator { get; set; } // %d (boolean)
    public string CarDesignStr { get; set; } // %s
    public string HelmetDesignStr { get; set; } // %s
    public string SuitDesignStr { get; set; } // %s
    public int BodyType { get; set; } // %d
    public int FaceType { get; set; } // %d
    public int HelmetType { get; set; } // %d
    public string CarNumberDesignStr { get; set; } // %s
    public int CarSponsor_1 { get; set; } // %d
    public int CarSponsor_2 { get; set; } // %d
    public string ClubName { get; set; } // %s
    public int ClubID { get; set; } // %d
    public string DivisionName { get; set; } // %s
    public int DivisionID { get; set; } // %d
    public int CurDriverIncidentCount { get; set; } // %d
    public int TeamIncidentCount { get; set; } // %d
}
