using System;

namespace IracingSdkDotNet.Serialization.Internal.Yaml;

[AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = false)]
internal sealed class YamlPropertyNameAttribute(string name) : Attribute
{
    public string Name { get; } = name;
}
