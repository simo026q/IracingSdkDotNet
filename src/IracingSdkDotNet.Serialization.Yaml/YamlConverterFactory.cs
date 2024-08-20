using IracingSdkDotNet.Serialization.Yaml.Converters;
using System;
using System.Collections.Generic;

namespace IracingSdkDotNet.Serialization.Yaml;

internal sealed class YamlConverterFactory(YamlSerializerOptions serializerOptions) : IYamlConverterFactory
{
    private readonly YamlSerializerOptions _serializerOptions = serializerOptions;

    public YamlConverter? CreateConverter(Type type)
    {
        YamlConverter? converter = _serializerOptions.GetConverter(type);
        if (converter != null)
        {
            return converter;
        }

        if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(List<>))
        {
            Type elementType = type.GetGenericArguments()[0];

            YamlConverter elementConverter = CreateConverter(elementType)
                ?? throw new NotSupportedException($"Unsupported type: {elementType.FullName}");

            return new SequenceYamlConverter(type, elementType, elementConverter);
        }

        return new ObjectYamlConverter(type, this);
    }
}
