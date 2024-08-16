using IracingSdkDotNet.Serialization.Internal.Yaml;
using System.Collections.Generic;

namespace IracingSdkDotNet.Serialization.Models.Session.RadioInfo;

public class RadioModel
{
    public int RadioNum { get; set; } // %d
    public int HopCount { get; set; } // %d
    public int NumFrequencies { get; set; } // %d
    public int TunedToFrequencyNum { get; set; } // %d

    [YamlConverter(typeof(IntegerBooleanYamlConverter))]
    public bool ScanningIsOn { get; set; } // %d (boolean)
    public List<FrequencyModel> Frequencies { get; set; }
}
