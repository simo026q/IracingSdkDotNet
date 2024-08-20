using YamlDotNet.Core;
using YamlDotNet.Core.Events;

namespace IracingSdkDotNet.Serialization.Yaml.Converters;

public abstract class ScalarYamlConverter<T> : YamlConverter<T>
{
    public abstract T ReadValue(string value);

    public sealed override T? ReadYaml(Parser parser)
        => ReadValue(parser.Consume<Scalar>().Value);
}
