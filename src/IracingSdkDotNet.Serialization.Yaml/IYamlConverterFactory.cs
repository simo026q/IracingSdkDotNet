using IracingSdkDotNet.Serialization.Yaml.Converters;
using System;

namespace IracingSdkDotNet.Serialization.Yaml;

public interface IYamlConverterFactory
{
    YamlConverter? CreateConverter(Type type);
}