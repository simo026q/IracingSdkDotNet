using YamlDotNet.Core;
using YamlDotNet.Core.Events;

namespace IracingSdkDotNet.Serialization.Yaml.Extensions;

internal static class ParserExtensions
{
    public static void SkipThisAndNestedEvents(this Parser parser)
    {
        int depth = 0;

        do
        {
            if (parser.Current is MappingStart or SequenceStart)
            {
                depth++;
            }
            else if (parser.Current is MappingEnd or SequenceEnd)
            {
                depth--;
            }

            parser.MoveNext();
        } while (depth > 0 || parser.Current is not (Scalar or SequenceEnd or MappingEnd));
    }
}