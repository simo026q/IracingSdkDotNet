using System.IO;
using YamlDotNet.Core;
using YamlDotNet.Core.Events;

namespace IracingSdkDotNet.Serialization.Yaml;

public static class YamlSerializer
{
    public static object? Deserialize(Parser parser, YamlSerializationContext serializationContext)
    {
        parser.Consume<StreamStart>();
        parser.Consume<DocumentStart>();

        return serializationContext.GetRootConverter().ReadAsObject(parser);
    }

    public static T? Deserialize<T>(Parser parser, YamlSerializationContext serializationContext)
    {
        return (T?)Deserialize(parser, serializationContext);
    }

    public static T? Deserialize<T>(TextReader reader, YamlSerializationContext serializationContext)
    {
        return Deserialize<T>(new Parser(reader), serializationContext);
    }

    public static T? Deserialize<T>(string yaml, YamlSerializationContext serializationContext)
    {
        return Deserialize<T>(new StringReader(yaml), serializationContext);
    }
}
