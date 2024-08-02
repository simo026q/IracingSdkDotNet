using IracingSdkDotNet.Core;
using Microsoft.Extensions.Logging;

using ILoggerFactory loggerFactory = LoggerFactory.Create(builder =>
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

// The IDisposable implementation of IracingSdk will automatically stop the SDK when it is disposed.