using System;

namespace IracingSdkDotNet.Serialization.Internal.Yaml;

[AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = false)]
internal sealed class YamlConverterAttribute : Attribute
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
}