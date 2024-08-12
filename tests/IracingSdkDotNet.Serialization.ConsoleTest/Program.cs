using IracingSdkDotNet.Serialization.Internal.Yaml;
using IracingSdkDotNet.Serialization.Models.Session;

string path = @"C:\Users\simon\source\repos\simo026q\IracingSdkDotNet\data\le-mans porsche-963-gtp\session-info.yaml";
string yaml = File.ReadAllText(path);

var model = YamlSerializer.Deserialize<IracingSessionModel>(yaml, YamlSerializerOptions.Default);

Console.WriteLine("End of file.");
Console.ReadKey();