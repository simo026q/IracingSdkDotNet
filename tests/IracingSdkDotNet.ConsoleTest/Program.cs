using IracingSdkDotNet;
using Microsoft.Extensions.Logging;

using var loggerFactory = LoggerFactory.Create(builder =>
{
    builder.AddConsole().SetMinimumLevel(LogLevel.Debug);
});

var options = new IracingSdkOptions
{
    CheckConnectionDelay = TimeSpan.FromSeconds(5)
};

using IracingSdk sdk = new(options, loggerFactory.CreateLogger<IracingSdk>());

sdk.Start();

Console.ReadKey();