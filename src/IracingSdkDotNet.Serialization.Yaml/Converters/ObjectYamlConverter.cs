using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;
using IracingSdkDotNet.Serialization.Yaml.Extensions;
using YamlDotNet.Core;
using YamlDotNet.Core.Events;

namespace IracingSdkDotNet.Serialization.Yaml.Converters;

internal sealed class ObjectYamlConverter(Type type, IYamlConverterFactory converterFactory) : YamlConverter
{
    private sealed class PropertyDescriptor(PropertyInfo property, YamlConverter converter)
    {
        public PropertyInfo Property { get; } = property;
        public YamlConverter Converter { get; } = converter;
    }

    private readonly Type _type = EnsureValidType(type);
    private readonly Func<object> _typeFactory = CreateTypeFactory(type);
    private readonly Dictionary<string, PropertyDescriptor> _properties = GetProperties(type, converterFactory);

    private static Type EnsureValidType(Type type)
    {
        if (!type.IsClass || type.IsAbstract)
        {
            throw new NotSupportedException($"Unsupported type: {type.FullName}");
        }

        return type;
    }

    private static Func<object> CreateTypeFactory(Type type)
    {
        ConstructorInfo? constructor = type.GetConstructor(Type.EmptyTypes);
        if (constructor is null)
        {
            throw new NotSupportedException($"Type '{type.FullName}' does not have a parameterless constructor.");
        }

        return Expression.Lambda<Func<object>>(Expression.New(constructor)).Compile();
    }

    private static Dictionary<string, PropertyDescriptor> GetProperties(Type type, IYamlConverterFactory converterFactory)
    {
        Dictionary<string, PropertyDescriptor> properties = new();

        foreach (PropertyInfo propertyInfo in type.GetProperties(BindingFlags.Public | BindingFlags.Instance))
        {
            if (!propertyInfo.CanWrite || propertyInfo.GetCustomAttribute<YamlIgnoreAttribute>() != null)
            {
                continue;
            }

            string name = propertyInfo.GetCustomAttribute<YamlPropertyNameAttribute>()?.Name ?? propertyInfo.Name;

            YamlConverter converter = propertyInfo.GetCustomAttribute<YamlConverterFactoryAttribute>()?.CreateConverter(propertyInfo.PropertyType)
                ?? converterFactory.CreateConverter(propertyInfo.PropertyType)
                ?? throw new NotSupportedException($"Unsupported type: {propertyInfo.PropertyType.FullName}");

            properties.Add(name, new PropertyDescriptor(propertyInfo, converter));
        }

        return properties;
    }

    public override bool CanConvert(Type type) => type == _type;

    public override object ReadAsObject(Parser parser)
    {
        object instance = _typeFactory();

        parser.Consume<MappingStart>();

        while (!parser.TryConsume<MappingEnd>(out _))
        {
            string key = parser.Consume<Scalar>().Value;

            if (!_properties.TryGetValue(key, out PropertyDescriptor? propertyDescriptor))
            {
                parser.SkipThisAndNestedEvents();
                continue;
            }

            object? value = propertyDescriptor.Converter.ReadAsObject(parser);
            propertyDescriptor.Property.SetValue(instance, value);
        }

        return instance;
    }
}
