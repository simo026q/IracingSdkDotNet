using System.Globalization;

namespace IracingSdkDotNet.Serialization.Yaml.Converters;

public sealed class SingleYamlConverter : ScalarYamlConverter<float>
{
    public static readonly SingleYamlConverter Instance = new();

    public override float ReadValue(string value)
    {
        return float.TryParse(value, NumberStyles.Float | NumberStyles.AllowThousands, CultureInfo.InvariantCulture, out float result)
            ? result
            : default;
    }
}
