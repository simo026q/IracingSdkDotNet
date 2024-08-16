using System;

namespace IracingSdkDotNet.Serialization.Internal.Yaml;

internal sealed class SingleUnitYamlConverterFactoryAttribute(string unit) : YamlConverterFactoryAttribute
{
    private readonly SingleUnitYamlConverter _converter = new(unit);

    public override YamlConverter? CreateConverter(Type type)
    {
        return _converter.CanConvert(type) ? _converter : null;
    }
}