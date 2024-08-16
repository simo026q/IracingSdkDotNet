using System;

namespace IracingSdkDotNet.Serialization.Internal.Yaml;

internal interface IYamlConverterFactory
{
    YamlConverter? CreateConverter(Type type);
}