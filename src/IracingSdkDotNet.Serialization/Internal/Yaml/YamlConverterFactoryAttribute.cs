using System;

namespace IracingSdkDotNet.Serialization.Internal.Yaml;

[AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
internal abstract class YamlConverterFactoryAttribute 
    : Attribute, IYamlConverterFactory
{
    public abstract YamlConverter? CreateConverter(Type type);
}
