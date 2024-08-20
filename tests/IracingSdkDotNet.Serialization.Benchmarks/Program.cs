using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Running;
using IracingSdkDotNet.Serialization.Internal.Yaml;
using IracingSdkDotNet.Serialization.Models.Session;
using irsdkSharp.Serialization.Models.Session;
using YamlDotNet.Core;

BenchmarkRunner.Run<Benchmarks>();

[MemoryDiagnoser(false)]
public class Benchmarks
{
    public string Yaml { get; } = GetYaml();

    [Benchmark]
    public IracingSessionModel? Deserialize()
    {
        return (IracingSessionModel?)YamlSerializer.Deserialize(new Parser(new StringReader(Yaml)), typeof(IracingSessionModel), YamlSerializerOptions.Default);
    }

    [Benchmark]
    public IRacingSessionModel? DeserializeIrsdk()
    {
        return IRacingSessionModel.Serialize(Yaml);
    }

    private static string? GetSolutionDirectory()
    {
        var directory = new DirectoryInfo(AppContext.BaseDirectory);

        while (directory != null && !directory.GetFiles("*.sln").Any())
        {
            directory = directory.Parent;
        }

        return directory?.FullName;
    }

    private static string GetYaml()
    {
        var solutionDirectory = GetSolutionDirectory();
        if (solutionDirectory == null)
        {
            throw new InvalidOperationException("Solution directory not found.");
        }

        var path = Path.Combine(solutionDirectory, "data", "le-mans porsche-963-gtp", "session-info.yaml");
        return File.ReadAllText(path);
    }
}