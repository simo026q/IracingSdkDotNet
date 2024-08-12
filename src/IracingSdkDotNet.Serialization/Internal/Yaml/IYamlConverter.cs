using System;

namespace IracingSdkDotNet.Serialization.Internal.Yaml;

public interface IYamlConverter<T>
{
    T? ReadYaml(string value, Type type);
    string WriteYaml(T? value, Type type);
}

internal sealed class Int32YamlConverter : IYamlConverter<int>
{
    public int ReadYaml(string value, Type type)
    {
        return int.TryParse(value, out int result) 
            ? result 
            : default;
    }

    public string WriteYaml(int value, Type type)
    {
        return value.ToString();
    }
}

internal sealed class SingleYamlConverter : IYamlConverter<float>
{
    public float ReadYaml(string value, Type type)
    {
        return float.TryParse(value, out float result) 
            ? result 
            : default;
    }

    public string WriteYaml(float value, Type type)
    {
        return value.ToString();
    }
}