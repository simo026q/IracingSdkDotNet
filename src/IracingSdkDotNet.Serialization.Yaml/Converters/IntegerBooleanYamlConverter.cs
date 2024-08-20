namespace IracingSdkDotNet.Serialization.Yaml.Converters;

public sealed class IntegerBooleanYamlConverter : ScalarYamlConverter<bool>
{
    public override bool ReadValue(string value)
    {
        return Int32YamlConverter.Instance.ReadValue(value) == 1;
    }
}
