using System.Collections.Generic;

namespace IracingSdkDotNet.Serialization.Internal.Yaml;

public sealed class YamlSerializerOptions
{
    public static YamlSerializerOptions Default { get; } = new YamlSerializerOptions();

    private readonly List<YamlConverter> _converters = [StringYamlConverter.Instance, Int32YamlConverter.Instance, SingleYamlConverter.Instance];

    public IReadOnlyCollection<YamlConverter> Converters => _converters.AsReadOnly();

    public void AddConverter<T, TConverter>(TConverter converter)
        where TConverter : YamlConverter<T>, new()
    {
        _converters.Add(converter);
    }
}
