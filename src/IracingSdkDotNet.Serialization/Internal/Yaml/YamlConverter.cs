﻿using System;

namespace IracingSdkDotNet.Serialization.Internal.Yaml;

public abstract class YamlConverter
{
    public abstract Type Type { get; }
    public abstract object? ReadAsObject(string value);
    public abstract string WriteAsObject(object? value);
}

public abstract class YamlConverter<T> : YamlConverter
{
    public abstract T? ReadYaml(string value);
    public abstract string WriteYaml(T? value);

    public sealed override Type Type => typeof(T);

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
        return float.TryParse(value, out float result) 
            ? result 
            : default;
    }

    public override string WriteYaml(float value)
    {
        return value.ToString();
    }
}