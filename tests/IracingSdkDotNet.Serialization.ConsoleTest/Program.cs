using IracingSdkDotNet.Serialization.Models.Session;

var solutionDirectory = GetSolutionDirectory();
if (solutionDirectory == null)
{
    Console.WriteLine("Solution directory not found.");
    return;
}

var path = Path.Combine(solutionDirectory, "data", "le-mans porsche-963-gtp", "session-info.yaml");
string yaml = File.ReadAllText(path);

var model = IracingSessionModel.Deserialize(yaml);

Console.WriteLine("End of file.");
Console.ReadKey();

static string? GetSolutionDirectory()
{
    var directory = new DirectoryInfo(AppContext.BaseDirectory);

    while (directory != null && !directory.GetFiles("*.sln").Any())
    {
        directory = directory.Parent;
    }

    return directory?.FullName;
}