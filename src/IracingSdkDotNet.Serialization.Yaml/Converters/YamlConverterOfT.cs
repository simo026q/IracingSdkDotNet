using System;
using YamlDotNet.Core;

namespace IracingSdkDotNet.Serialization.Yaml.Converters;

public abstract class YamlConverter<T> : YamlConverter
{
    public abstract T? ReadYaml(Parser parser);

    public override bool CanConvert(Type type)
        => type == typeof(T);

    public sealed override object? ReadAsObject(Parser parser)
        => ReadYaml(parser);
}