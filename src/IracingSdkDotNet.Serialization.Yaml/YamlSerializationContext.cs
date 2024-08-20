using IracingSdkDotNet.Serialization.Yaml.Converters;
using System;

namespace IracingSdkDotNet.Serialization.Yaml;

public sealed class YamlSerializationContext
{
    private readonly YamlConverter _rootConverter;

    public YamlSerializationContext(Type type, YamlSerializerOptions? serializerOptions = null)
    {
        var factory = new YamlConverterFactory(serializerOptions ?? YamlSerializerOptions.Default);

        _rootConverter = factory.CreateConverter(type)
            ?? throw new NotSupportedException($"Unsupported type: {type.FullName}");
    }

    internal YamlConverter GetRootConverter() => _rootConverter;
}
