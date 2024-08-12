using System;

namespace IracingSdkDotNet.Serialization.Internal.Yaml;

[AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = false)]
internal sealed class YamlConverterAttribute : Attribute
{
    public Type ConverterType { get; }

    public YamlConverterAttribute(Type converterType)
    {
        if (!converterType.IsAssignableFrom(typeof(IYamlConverter<>)))
        {
            throw new InvalidOperationException($"The type '{converterType}' does not implement '{typeof(IYamlConverter<>)}'.");
        }

        ConverterType = converterType;
    }
}