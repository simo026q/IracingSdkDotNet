using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using YamlDotNet.Core;
using YamlDotNet.Core.Events;

namespace IracingSdkDotNet.Serialization.Internal.Yaml;

public static class YamlSerializer
{
    private sealed class PropertyContext(PropertyInfo? propertyInfo, Type? elementType, string? yamlName, YamlConverter? converter, List<PropertyContext>? children)
    {
        private readonly string? _yamlName = yamlName;

        public string YamlName => _yamlName ?? Property?.Name ?? string.Empty;

        public PropertyInfo? Property { get; } = propertyInfo;
        public Type? ElementType { get; } = elementType;
        public YamlConverter? Converter { get; } = converter;
        public List<PropertyContext> Children { get; } = children ?? [];

        public override string ToString()
        {
            StringBuilder sb = new();

            if (Property is not null)
            {
                sb.Append(Property.PropertyType.Name);
                sb.Append(' ');
                sb.Append(Property.Name);
            }

            return sb.ToString();
        }
    }

    private sealed class SerializationContext
    {
        private readonly Type _type;
        private readonly Dictionary<Type, YamlConverter> _converters;

        public PropertyContext Root { get; }

        public SerializationContext(Type type, IEnumerable<YamlConverter> converters)
        {
            _type = type;

            _converters = converters.ToDictionary(static x => x.Type);

            Root = CreateRootPropertyContext();
        }

        private PropertyContext CreateRootPropertyContext()
        {
            return new PropertyContext(null, null, null, null, CreatePropertyContexts(_type));
        }

        private List<PropertyContext> CreatePropertyContexts(Type type)
        {
            return type.GetProperties(BindingFlags.Instance | BindingFlags.Public)
                .Where(property => property.GetCustomAttribute<YamlIgnoreAttribute>() == null)
                .Select(CreatePropertyContext)
                .ToList();
        }

        private PropertyContext CreatePropertyContext(PropertyInfo property)
        {
            string? yamlName = property.GetCustomAttribute<YamlPropertyNameAttribute>()?.Name;
            Type propertyType = property.PropertyType;

            Type? converterType = property.GetCustomAttribute<YamlConverterAttribute>()?.ConverterType;
            YamlConverter? converter = GetConverter(converterType, propertyType);

            if (converter is null && propertyType != typeof(string))
            {
                if (TryGetEnumerableType(propertyType, out Type? elementType))
                {
                    return new PropertyContext(property, elementType, yamlName, converter: null, CreatePropertyContexts(elementType!));
                }
                else
                {
                    return new PropertyContext(property, elementType: null, yamlName, converter: null, CreatePropertyContexts(propertyType));
                }
            }

            return new PropertyContext(property, elementType: null, yamlName, converter, children: null);

            YamlConverter? GetConverter(Type? converterType, Type propertyType)
            {
                if (converterType is not null)
                {
                    return (YamlConverter)Activator.CreateInstance(converterType)!;
                }

                if (_converters.TryGetValue(propertyType, out YamlConverter? typeConverter))
                {
                    return typeConverter;
                }

                return null;
            }
        }

        private static bool TryGetEnumerableType(Type type, out Type? elementType)
        {
            elementType = type.GetInterfaces()
                .FirstOrDefault(i => i.IsGenericType && i.GetGenericTypeDefinition() == typeof(IEnumerable<>))
                ?.GetGenericArguments()
                .FirstOrDefault();

            return elementType is not null;
        }
    }

    public static T? Deserialize<T>(string value, YamlSerializerOptions serializerOptions)
        where T : class, new()
    {
        var context = new SerializationContext(typeof(T), serializerOptions.Converters);

        var parser = new Parser(new StringReader(value));

        T result = new();

        Stack<PropertyContext> stack = new();
        stack.Push(context.Root);
        int ignoredDepth = -1;

        while (parser.MoveNext())
        {
            ParsingEvent? currentEvent = parser.Current;

            switch (currentEvent)
            {
                case Scalar scalar:
                    if (ignoredDepth >= 0)
                    {
                        break;
                    }

                    if (scalar.IsKey)
                    {
                        PropertyContext current = stack.Peek();
                        PropertyContext? child = current.Children.Find(c => c.YamlName == scalar.Value);

                        if (child is not null)
                        {
                            stack.Push(child);
                        }
                        else
                        {
                            ignoredDepth = 0;
                        }
                    }
                    else
                    {
                        PropertyContext current = stack.Pop();
                        Debug.WriteLine($"{current.YamlName}: {scalar.Value}");
                    }

                    break;
                
                case MappingStart _:
                case SequenceStart _:
                    if (ignoredDepth >= 0)
                    {
                        ignoredDepth++;
                    }

                    break;

                case MappingEnd _:
                    {
                        if (ignoredDepth >= 0)
                        {
                            ignoredDepth--;
                            break;
                        }

                        PropertyContext current = stack.Peek();
                        if (stack.Count > 0 && current.ElementType is null)
                        {
                            stack.Pop();
                        }

                        break;
                    }

                case SequenceEnd _:
                    if (ignoredDepth >= 0)
                    {
                        ignoredDepth--;
                        break;
                    }

                    if (stack.Count > 0)
                    {
                        stack.Pop();
                    }

                    break;
            }
        }

        return result;
    }
}
