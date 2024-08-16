using System;

namespace IracingSdkDotNet.Serialization.Internal.Yaml;

internal sealed class YamlConverterAttribute 
    : YamlConvertFactoryAttribute
{
    public Type ConverterType { get; }

    public YamlConverterAttribute(Type converterType)
    {
        if (!typeof(YamlConverter).IsAssignableFrom(converterType))
        {
            throw new InvalidOperationException($"The type '{converterType.FullName}' does not implement '{nameof(YamlConverter)}'.");
        }

        ConverterType = converterType;
    }

    public override YamlConverter? CreateConverter(Type type)
    {
        return (YamlConverter?)Activator.CreateInstance(ConverterType);
    }
}