using System;
using System.Collections.Concurrent;

namespace IracingSdkDotNet.Serialization.Internal.Yaml;

internal sealed class YamlConverterAttribute 
    : YamlConverterFactoryAttribute
{
    private static readonly ConcurrentDictionary<Type, YamlConverter> Converters = new();

    public Type ConverterType { get; }

    public YamlConverterAttribute(Type converterType)
    {
        if (!typeof(YamlConverter).IsAssignableFrom(converterType))
        {
            throw new InvalidOperationException($"The type '{converterType.FullName}' does not implement '{nameof(YamlConverter)}'.");
        }

        ConverterType = converterType;
    }

    public override YamlConverter? CreateConverter(Type type)
    {
        if (Converters.TryGetValue(type, out YamlConverter? converter))
        {
            return converter;
        }

        converter = (YamlConverter)Activator.CreateInstance(ConverterType)!;
        Converters.TryAdd(type, converter);

        return converter;
    }
}