using IracingSdkDotNet.Serialization.Yaml.Converters;
using System;
using System.Collections.Generic;

namespace IracingSdkDotNet.Serialization.Yaml;

public sealed class YamlSerializerOptions
{
    public static YamlSerializerOptions Default { get; } = new YamlSerializerOptions();

    public List<YamlConverter> Converters { get; } = [StringYamlConverter.Instance, BooleanYamlConverter.Instance, Int32YamlConverter.Instance, SingleYamlConverter.Instance];

    internal YamlConverter? GetConverter(Type type)
    {
        foreach (YamlConverter converter in Converters)
        {
            if (converter.CanConvert(type))
            {
                return converter;
            }
        }

        return null;
    }
}
