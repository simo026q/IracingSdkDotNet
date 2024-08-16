using System;

namespace IracingSdkDotNet.Serialization.Internal.Yaml;

[AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
internal abstract class YamlConvertFactoryAttribute 
    : Attribute, IYamlConverterFactory
{
    public abstract YamlConverter? CreateConverter(Type type);
}
