using System;
using YamlDotNet.Core;

namespace IracingSdkDotNet.Serialization.Yaml.Converters;

public abstract class YamlConverter
{
    public abstract bool CanConvert(Type type);
    public abstract object? ReadAsObject(Parser parser);
}
