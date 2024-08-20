using System;
using System.Reflection;
using YamlDotNet.Core;
using YamlDotNet.Core.Events;

namespace IracingSdkDotNet.Serialization.Yaml.Converters;

internal sealed class SequenceYamlConverter(Type type, Type elementType, YamlConverter elementConverter) : YamlConverter
{
    private sealed class SequenceBuilder(Type type)
    {
        private readonly MethodInfo _addMethod = type.GetMethod("Add")!;
        private readonly object _list = Activator.CreateInstance(type)!;

        public void AddItem(object item)
        {
            _addMethod.Invoke(_list, [item]);
        }

        public object Build() => _list;
    }

    private readonly Type _type = type;
    private readonly Type _elementType = elementType;
    private readonly YamlConverter _elementConverter = elementConverter;

    public override bool CanConvert(Type type) => type == _type;

    public override object ReadAsObject(Parser parser)
    {
        SequenceBuilder builder = new(_type);

        parser.Consume<SequenceStart>();

        while (!parser.TryConsume<SequenceEnd>(out _))
        {
            object? item = _elementConverter.ReadAsObject(parser);

            if (item is null)
            {
                parser.SkipThisAndNestedEvents();
                continue;
            }

            builder.AddItem(item);
        }

        return builder.Build();
    }
}
