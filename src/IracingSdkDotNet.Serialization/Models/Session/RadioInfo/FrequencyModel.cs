using IracingSdkDotNet.Serialization.Yaml;
using IracingSdkDotNet.Serialization.Yaml.Converters;

namespace IracingSdkDotNet.Serialization.Models.Session.RadioInfo;

public class FrequencyModel
{
    public int FrequencyNum { get; set; } // %d
    public string FrequencyName { get; set; } // "%s"
    public int Priority { get; set; } // %d
    public int CarIdx { get; set; } // %d
    public int EntryIdx { get; set; } // %d
    public int ClubID { get; set; } //%d

    [YamlConverter(typeof(IntegerBooleanYamlConverter))]
    public bool CanScan { get; set; } // %d (boolean)

    [YamlConverter(typeof(IntegerBooleanYamlConverter))]
    public bool CanSquawk { get; set; } // %d (boolean)

    [YamlConverter(typeof(IntegerBooleanYamlConverter))]
    public bool Muted { get; set; } // %d (boolean)

    [YamlConverter(typeof(IntegerBooleanYamlConverter))]
    public bool IsMutable { get; set; } // %d (boolean)

    [YamlConverter(typeof(IntegerBooleanYamlConverter))]
    public bool IsDeletable { get; set; } // %d (boolean)
}
