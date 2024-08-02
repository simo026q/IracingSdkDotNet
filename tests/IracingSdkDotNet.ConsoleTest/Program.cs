using IracingSdkDotNet.Core;
using IracingSdkDotNet.Core.Reader;
using Microsoft.Extensions.Logging;
using System.Diagnostics;
using System.Text.Json;
using System.Text.Json.Serialization;

using ILoggerFactory loggerFactory = LoggerFactory.Create(builder =>
{
    builder.AddConsole().SetMinimumLevel(LogLevel.Debug);
});

var options = new IracingSdkOptions
{
    CheckConnectionDelay = TimeSpan.FromSeconds(5)
};

using IracingSdkCore sdk = new(options, loggerFactory.CreateLogger<IracingSdkCore>());

sdk.DataUpdated += OnDataUpdated;

sdk.Start();

Console.ReadKey();

// The IDisposable implementation of IracingSdk will automatically stop the SDK when it is disposed.

void OnDataUpdated(object? sender, IracingDataReader reader)
{
    sdk.DataUpdated -= OnDataUpdated;

    Debug.Assert(sdk.DataReader != null);

    using FileStream jsonFs = File.Open("var-headers.json", FileMode.OpenOrCreate);

    var jsonOptions = new JsonSerializerOptions { WriteIndented = true };
    jsonOptions.Converters.Add(new JsonStringEnumConverter());

    JsonSerializer.Serialize(jsonFs, reader.VariableHeaders, jsonOptions);

    using FileStream yamlFs = File.Open("session-info.yaml", FileMode.OpenOrCreate);
    string? sessionInfo = reader.ReadRawSessionInfo();
    if (sessionInfo != null)
    {
        using StreamWriter writer = new(yamlFs);
        writer.Write(sessionInfo);
    }

    sdk.Stop();
}