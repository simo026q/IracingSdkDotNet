namespace IracingSdkDotNet.Serialization.Yaml.Converters;

public sealed class SingleUnitYamlConverter(string unit) : ScalarYamlConverter<float>
{
    private readonly string _unit = unit;

    public override float ReadValue(string value)
    {
        string valueWithoutUnit = value.Replace(_unit, string.Empty);

        return SingleYamlConverter.Instance.ReadValue(valueWithoutUnit);
    }
}