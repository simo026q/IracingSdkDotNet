namespace IracingSdkDotNet.Serialization.Yaml.Converters;

public sealed class Int32YamlConverter : ScalarYamlConverter<int>
{
    public static readonly Int32YamlConverter Instance = new();

    public override int ReadValue(string value)
    {
        return int.TryParse(value, out int result)
            ? result
            : default;
    }
}
