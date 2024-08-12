using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace IracingSdkDotNet.Serialization.Internal.Yaml;

public static class YamlSerializer
{
    private sealed class SerializationContext
    {
        private sealed class PropertyContext(string name, bool isArray, Type? converterType, List<PropertyContext>? children)
        {
            public string Name { get; } = name;
            public bool IsArray { get; } = isArray;
            public Type? ConverterType { get; } = converterType;
            public List<PropertyContext> Children { get; } = children ?? [];

            public override string ToString()
            {
                string type;
                if (ConverterType == null)
                {
                    
                }

                return $"{Name}";
            }
        }

        public Type Type { get; }
        private readonly Dictionary<Type, Type> _converters;
        private readonly PropertyContext _rootPropertyContext;

        public SerializationContext(Type type, IEnumerable<Type> converters)
        {
            Type = type;

            _converters = new();
            foreach (Type converter in converters)
            {
                if (!TryGetConvertionType(converter, out Type? convertedType))
                {
                    throw new InvalidOperationException($"The type '{converter}' does not implement '{typeof(IYamlConverter<>)}'.");
                }

                _converters.Add(convertedType!, converter);
            }

            _rootPropertyContext = CreateRootPropertyContext();
        }

        private PropertyContext CreateRootPropertyContext()
        {
            return new PropertyContext(string.Empty, isArray: false, null, CreatePropertyContext(Type));
        }

        private List<PropertyContext> CreatePropertyContext(Type type)
        {
            PropertyInfo[] properties = type.GetProperties(BindingFlags.Instance | BindingFlags.Public);

            var children = new List<PropertyContext>();

            foreach (PropertyInfo property in properties)
            {
                YamlIgnoreAttribute? ignoreAttribute = property.GetCustomAttribute<YamlIgnoreAttribute>();
                if (ignoreAttribute is not null)
                {
                    continue;
                }

                YamlPropertyNameAttribute? nameAttribute = property.GetCustomAttribute<YamlPropertyNameAttribute>();

                string name = nameAttribute?.Name ?? property.Name;
                Type propertyType = property.PropertyType;

                YamlConverterAttribute? converterAttribute = property.GetCustomAttribute<YamlConverterAttribute>();
                Type? converterType = converterAttribute?.ConverterType;

                if (converterType == null && propertyType != typeof(string))
                {
                    if (!_converters.TryGetValue(propertyType, out converterType))
                    {
                        if (TryGetEnumerableType(propertyType, out Type? elementType))
                        {
                            children.Add(new PropertyContext(name, isArray: true, converterType, CreatePropertyContext(elementType!)));
                        }
                        else
                        {
                            children.Add(new PropertyContext(name, isArray: false, converterType, CreatePropertyContext(property.PropertyType)));
                        }
                        
                        continue;
                    }
                }

                if (!IsValidConverter(propertyType, converterType))
                {
                    throw new InvalidOperationException($"The converter '{converterType}' is not valid for type '{propertyType}'.");
                }

                children.Add(new PropertyContext(name, isArray: false, converterType, null));
            }

            return children;

            static bool IsValidConverter(Type type, Type? converterType)
            {
                if (converterType == null)
                {
                    return type == typeof(string);
                }

                return TryGetConvertionType(converterType, out Type? convertedType) && type == convertedType;
            }
        }

        private static bool TryGetConvertionType(Type converterType, out Type? convertedType)
        {
            foreach (Type interfaceType in converterType.GetInterfaces())
            {
                if (interfaceType.IsGenericType && interfaceType.GetGenericTypeDefinition() == typeof(IYamlConverter<>))
                {
                    convertedType = interfaceType.GetGenericArguments()[0];
                    return true;
                }
            }

            convertedType = null;
            return false;
        }

        private static bool TryGetEnumerableType(Type type, out Type? elementType)
        {
            foreach (Type interfaceType in type.GetInterfaces())
            {
                if (interfaceType.IsGenericType && interfaceType.GetGenericTypeDefinition() == typeof(IEnumerable<>))
                {
                    elementType = interfaceType.GetGenericArguments()[0];
                    return true;
                }
            }

            elementType = null;
            return false;
        }
    }

    public static string Serialize<T>(T value)
    {
        throw new NotImplementedException();
    }

    public static T? Deserialize<T>(string value, YamlSerializerOptions serializerOptions)
    {
        var context = new SerializationContext(typeof(T), serializerOptions.Converters);

        return default;
    }
}
