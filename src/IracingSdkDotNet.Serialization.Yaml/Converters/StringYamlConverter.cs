namespace IracingSdkDotNet.Serialization.Yaml.Converters;

public sealed class StringYamlConverter : ScalarYamlConverter<string>
{
    public static readonly StringYamlConverter Instance = new();

    public override string ReadValue(string value)
    {
        return value;
    }
}
