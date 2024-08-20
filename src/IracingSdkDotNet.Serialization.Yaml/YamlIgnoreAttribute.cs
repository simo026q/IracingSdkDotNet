using System;

namespace IracingSdkDotNet.Serialization.Yaml;

[AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = false)]
public sealed class YamlIgnoreAttribute : Attribute
{
}
