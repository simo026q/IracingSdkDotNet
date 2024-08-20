namespace IracingSdkDotNet.Serialization.Yaml.Converters;

public sealed class BooleanYamlConverter : ScalarYamlConverter<bool>
{
    public static readonly BooleanYamlConverter Instance = new();

    public override bool ReadValue(string value)
    {
        return bool.TryParse(value, out bool result) && result;
    }
}
