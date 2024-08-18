using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using YamlDotNet.Core;
using YamlDotNet.Core.Events;

namespace IracingSdkDotNet.Serialization.Internal.Yaml;

public static class YamlSerializer
{
    private sealed class TypeDescriptor(Type type, YamlConverter? converter)
    {
        public Type Type { get; } = type;
        public YamlConverter? Converter { get; } = converter;
    }

    public static object? Deserialize(Parser parser, Type type, YamlSerializerOptions serializerOptions) 
    {
        parser.Consume<StreamStart>();
        parser.Consume<DocumentStart>();

        return DeserializeObject(parser, new TypeDescriptor(type, null), serializerOptions);
    }

    private static object? DeserializeObject(Parser parser, TypeDescriptor typeDescriptor, YamlSerializerOptions serializerOptions)
    {
        Type type = typeDescriptor.Type;

        // Directly handle types that do not have a parameterless constructor
        if (!type.IsClass || type.IsAbstract)
        {
            throw new NotSupportedException($"Unsupported type: {type.FullName}");
        }

        object? result = Activator.CreateInstance(type);
        if (result is null)
        {
            return null;
        }
        
        parser.Consume<MappingStart>();

        // Cache properties with required conditions to avoid calling reflection repeatedly.
        var properties = type.GetProperties(BindingFlags.Public | BindingFlags.Instance)
            .Where(p => p.CanWrite && p.GetCustomAttribute<YamlIgnoreAttribute>() == null)
            .ToDictionary(p => p.GetCustomAttribute<YamlPropertyNameAttribute>()?.Name ?? p.Name, p => p);

        while (!parser.TryConsume<MappingEnd>(out _))
        {
            var key = parser.Consume<Scalar>().Value;

            if (!properties.TryGetValue(key, out var property))
            {
                // Skip unknown properties
                parser.SkipThisAndNestedEvents();
                continue;
            }

            YamlConverter? converter = property.GetCustomAttribute<YamlConverterFactoryAttribute>()?.CreateConverter(property.PropertyType);
            object? value = DeserializeValue(parser, new TypeDescriptor(property.PropertyType, converter), serializerOptions);
            property.SetValue(result, value);
        }

        return result;
    }

    private static object? DeserializeValue(Parser parser, TypeDescriptor typeDescriptor, YamlSerializerOptions serializerOptions)
    {
        Type type = typeDescriptor.Type;
        YamlConverter? converter = typeDescriptor.Converter ?? serializerOptions.GetConverter(type);

        if (converter != null)
        {
            string scalarValue = parser.Consume<Scalar>().Value;
            return converter.ReadAsObject(scalarValue);
        }

        if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(List<>))
        {
            var list = Activator.CreateInstance(type);
            var itemType = type.GetGenericArguments()[0];
            var addMethod = type.GetMethod("Add");

            parser.Consume<SequenceStart>();
            while (!parser.TryConsume<SequenceEnd>(out _))
            {
                var value = DeserializeValue(parser, new TypeDescriptor(itemType, null), serializerOptions);
                if (value != null)
                {
                    addMethod?.Invoke(list, [value]);
                }
            }

            return list;
        }

        return DeserializeObject(parser, new TypeDescriptor(type, null), serializerOptions);
    }
}
