using System;
using System.Globalization;

namespace IracingSdkDotNet.Serialization.Internal.Yaml;

public abstract class YamlConverter
{
    public abstract bool CanConvert(Type type);
    public abstract object? ReadAsObject(string value);
    public abstract string WriteAsObject(object? value);
}

public abstract class YamlConverter<T> : YamlConverter
{
    public abstract T? ReadYaml(string value);
    public abstract string WriteYaml(T? value);

    public override bool CanConvert(Type type)
    {
        return type == typeof(T);
    }

    public sealed override object? ReadAsObject(string value)
    {
        return ReadYaml(value);
    }

    public sealed override string WriteAsObject(object? value)
    {
        return WriteYaml((T?)value);
    }
}

internal sealed class StringYamlConverter : YamlConverter<string>
{
    public static readonly StringYamlConverter Instance = new();

    public override string ReadYaml(string value)
    {
        return value;
    }

    public override string WriteYaml(string value)
    {
        return value;
    }
}

internal sealed class BooleanYamlConverter : YamlConverter<bool>
{
    public static readonly BooleanYamlConverter Instance = new();

    public override bool ReadYaml(string value)
    {
        return bool.TryParse(value, out bool result) && result;
    }

    public override string WriteYaml(bool value)
    {
        return value 
            ? "true" 
            : "false";
    }
}

internal sealed class Int32YamlConverter : YamlConverter<int>
{
    public static readonly Int32YamlConverter Instance = new();

    public override int ReadYaml(string value)
    {
        return int.TryParse(value, out int result) 
            ? result 
            : default;
    }

    public override string WriteYaml(int value)
    {
        return value.ToString();
    }
}

internal sealed class SingleYamlConverter : YamlConverter<float>
{
    public static readonly SingleYamlConverter Instance = new();

    public override float ReadYaml(string value)
    {
        return float.TryParse(value, NumberStyles.Float | NumberStyles.AllowThousands, CultureInfo.InvariantCulture, out float result)
            ? result
            : default;
    }

    public override string WriteYaml(float value)
    {
        return value.ToString();
    }
}

internal sealed class PercentageYamlConverter : YamlConverter<float>
{
    private const string Unit = " %";

    public override float ReadYaml(string value)
    {
        string valueWithoutUnit = value.Replace(Unit, string.Empty);

        return SingleYamlConverter.Instance.ReadYaml(valueWithoutUnit) / 100;
    }

    public override string WriteYaml(float value)
    {
        return string.Concat(SingleYamlConverter.Instance.WriteYaml(value * 100), Unit);
    }
}

internal sealed class IntegerBooleanYamlConverter : YamlConverter<bool>
{
    public override bool ReadYaml(string value)
    {
        return Int32YamlConverter.Instance.ReadYaml(value) == 1;
    }

    public override string WriteYaml(bool value)
    {
        return Int32YamlConverter.Instance.WriteYaml(value ? 1 : 0);
    }
}

internal sealed class SingleUnitYamlConverter(string unit) : YamlConverter<float>
{
    private readonly string _unit = unit;

    public override float ReadYaml(string value)
    {
        string valueWithoutUnit = value.Replace(_unit, string.Empty);

        return SingleYamlConverter.Instance.ReadYaml(valueWithoutUnit);
    }

    public override string WriteYaml(float value)
    {
        return string.Concat(SingleYamlConverter.Instance.WriteYaml(value), _unit);
    }
}