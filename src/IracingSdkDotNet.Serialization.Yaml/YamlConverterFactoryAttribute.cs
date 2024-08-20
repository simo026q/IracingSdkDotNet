using IracingSdkDotNet.Serialization.Yaml.Converters;
using System;

namespace IracingSdkDotNet.Serialization.Yaml;

[AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
public abstract class YamlConverterFactoryAttribute
    : Attribute, IYamlConverterFactory
{
    public abstract YamlConverter? CreateConverter(Type type);
}
