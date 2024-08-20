using System;
using System.Globalization;
using YamlDotNet.Core;
using YamlDotNet.Core.Events;

namespace IracingSdkDotNet.Serialization.Internal.Yaml;

public abstract class YamlConverter
{
    public abstract bool CanConvert(Type type);
    public abstract object? ReadAsObject(Parser parser);
}

public abstract class YamlConverter<T> : YamlConverter
{
    public abstract T? ReadYaml(Parser parser);

    public override bool CanConvert(Type type) 
        => type == typeof(T);

    public sealed override object? ReadAsObject(Parser parser) 
        => ReadYaml(parser);
}

public abstract class ScalarYamlConverter<T> : YamlConverter<T>
{
    public abstract T ReadValue(string value);

    public sealed override T? ReadYaml(Parser parser) 
        => ReadValue(parser.Consume<Scalar>().Value);
}

internal sealed class StringYamlConverter : ScalarYamlConverter<string>
{
    public static readonly StringYamlConverter Instance = new();

    public override string ReadValue(string value)
    {
        return value;
    }
}

internal sealed class BooleanYamlConverter : ScalarYamlConverter<bool>
{
    public static readonly BooleanYamlConverter Instance = new();

    public override bool ReadValue(string value)
    {
        return bool.TryParse(value, out bool result) && result;
    }
}

internal sealed class Int32YamlConverter : ScalarYamlConverter<int>
{
    public static readonly Int32YamlConverter Instance = new();

    public override int ReadValue(string value)
    {
        return int.TryParse(value, out int result) 
            ? result 
            : default;
    }
}

internal sealed class SingleYamlConverter : ScalarYamlConverter<float>
{
    public static readonly SingleYamlConverter Instance = new();

    public override float ReadValue(string value)
    {
        return float.TryParse(value, NumberStyles.Float | NumberStyles.AllowThousands, CultureInfo.InvariantCulture, out float result)
            ? result
            : default;
    }
}

internal sealed class PercentageYamlConverter : ScalarYamlConverter<float>
{
    private const string Unit = " %";

    public override float ReadValue(string value)
    {
        string valueWithoutUnit = value.Replace(Unit, string.Empty);

        return SingleYamlConverter.Instance.ReadValue(valueWithoutUnit) / 100;
    }
}

internal sealed class IntegerBooleanYamlConverter : ScalarYamlConverter<bool>
{
    public override bool ReadValue(string value)
    {
        return Int32YamlConverter.Instance.ReadValue(value) == 1;
    }
}

internal sealed class SingleUnitYamlConverter(string unit) : ScalarYamlConverter<float>
{
    private readonly string _unit = unit;

    public override float ReadValue(string value)
    {
        string valueWithoutUnit = value.Replace(_unit, string.Empty);

        return SingleYamlConverter.Instance.ReadValue(valueWithoutUnit);
    }
}