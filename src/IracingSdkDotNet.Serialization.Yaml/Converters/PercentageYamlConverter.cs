namespace IracingSdkDotNet.Serialization.Yaml.Converters;

public sealed class PercentageYamlConverter : ScalarYamlConverter<float>
{
    private const string Unit = " %";

    public override float ReadValue(string value)
    {
        string valueWithoutUnit = value.Replace(Unit, string.Empty);

        return SingleYamlConverter.Instance.ReadValue(valueWithoutUnit) / 100;
    }
}
