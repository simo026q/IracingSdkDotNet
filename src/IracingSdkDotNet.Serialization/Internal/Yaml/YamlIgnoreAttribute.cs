using System;

namespace IracingSdkDotNet.Serialization.Internal.Yaml;

[AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = false)]
internal sealed class YamlIgnoreAttribute : Attribute
{
}
