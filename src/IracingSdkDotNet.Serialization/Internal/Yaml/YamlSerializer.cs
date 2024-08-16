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

        var result = DeserializeObject(parser, new TypeDescriptor(type, null), serializerOptions);

        return result;
    }

    private static object? DeserializeObject(Parser parser, TypeDescriptor typeDescriptor, YamlSerializerOptions serializerOptions)
    {
        Type type = typeDescriptor.Type;
        object? result = Activator.CreateInstance(type);
        if (result is null)
        {
            return null;
        }
        
        parser.Consume<MappingStart>();

        var properties = type.GetProperties(BindingFlags.Public | BindingFlags.Instance)
            .Where(p => p.CanWrite && p.GetCustomAttribute<YamlIgnoreAttribute>() == null);

        while (!parser.TryConsume<MappingEnd>(out _))
        {
            var key = parser.Consume<Scalar>().Value;

            var property = properties.FirstOrDefault(p => p.Name == key || p.GetCustomAttribute<YamlPropertyNameAttribute>()?.Name == key);

            if (property == null)
            {
                // Skip unknown properties
                parser.SkipThisAndNestedEvents();
                continue;
            }

            object? value = DeserializeValue(parser, new TypeDescriptor(property.PropertyType, property.GetCustomAttribute<YamlConverterAttribute>()?.CreateConverter(property.PropertyType)), serializerOptions);
            property.SetValue(result, value);
        }

        return result;
    }

    private static object? DeserializeValue(Parser parser, TypeDescriptor typeDescriptor, YamlSerializerOptions serializerOptions)
    {
        Type type = typeDescriptor.Type;
        YamlConverter? converter = typeDescriptor.Converter ?? serializerOptions.GetConverter(type);

        var result = converter?.ReadAsObject(parser.Consume<Scalar>().Value);
        if (result != null)
        {
            return result;
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

        if (type.IsClass)
        {
            return DeserializeObject(parser, new TypeDescriptor(type, null), serializerOptions);
        }

        throw new NotSupportedException($"Unsupported type: {type.FullName}");
    }
}

public static class ParserExtensions
{
    public static void SkipThisAndNestedEvents(this Parser parser)
    {
        int depth = 0;

        do
        {
            if (parser.Current is MappingStart or SequenceStart)
            {
                depth++;
            }
            else if (parser.Current is MappingEnd or SequenceEnd)
            {
                depth--;
            }

            parser.MoveNext();
        } while (depth > 0 || parser.Current is not (Scalar or SequenceEnd or MappingEnd));
    }
}