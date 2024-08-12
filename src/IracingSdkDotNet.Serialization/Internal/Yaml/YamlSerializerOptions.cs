using System;
using System.Collections.Generic;

namespace IracingSdkDotNet.Serialization.Internal.Yaml;

public sealed class YamlSerializerOptions
{
    public static YamlSerializerOptions Default { get; } = new YamlSerializerOptions();

    private readonly List<Type> _converters = [typeof(Int32YamlConverter), typeof(SingleYamlConverter)];

    public IReadOnlyCollection<Type> Converters => _converters.AsReadOnly();

    public void AddConverter<T, TConverter>()
        where TConverter : IYamlConverter<T>, new()
    {
        _converters.Add(typeof(TConverter));
    }
}
