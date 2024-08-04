using Microsoft.Extensions.Logging;
using System;
using System.IO;

namespace IracingSdkDotNet.Core.Extensions;

internal static partial class LogMessages
{
    [LoggerMessage(LogLevel.Information, "Starting the iRacing SDK.")]
    public static partial void LogStarting(this ILogger logger);

    [LoggerMessage(LogLevel.Information, "Stopping the iRacing SDK.")]
    public static partial void LogStopping(this ILogger logger);

    [LoggerMessage(LogLevel.Warning, "Could not open iRacing's memory mapped file.")]
    public static partial void LogMemoryMappedFileOpenFailed(this ILogger logger, FileNotFoundException ex);

    [LoggerMessage(LogLevel.Debug, "Opened iRacing's memory mapped file.")]
    public static partial void LogMemoryMappedFileOpened(this ILogger logger);

    [LoggerMessage(LogLevel.Information, "Waiting {CheckConnectionDelay} before retrying to connect to iRacing.")]
    public static partial void LogWaitingForConnectionRetry(this ILogger logger, TimeSpan checkConnectionDelay);

    [LoggerMessage(LogLevel.Debug, "The CancellationToken was cancelled while waiting for next connect retry to iRacing.")]
    public static partial void LogConnectionRetryWaitCancelled(this ILogger logger);

    [LoggerMessage(LogLevel.Debug, "Exited the connection loop.")]
    public static partial void LogConnectionLoopExited(this ILogger logger);

    [LoggerMessage(LogLevel.Debug, "Exited the data loop.")]
    public static partial void LogDataLoopExited(this ILogger logger);

    [LoggerMessage(LogLevel.Information, "Connected to iRacing.")]
    public static partial void LogConnected(this ILogger logger);

    [LoggerMessage(LogLevel.Warning, "Disconnected from iRacing.")]
    public static partial void LogDisconnected(this ILogger logger);

    [LoggerMessage(LogLevel.Trace, "The data in iRacing's shared memory has been updated.")]
    public static partial void LogDataUpdated(this ILogger logger);

    [LoggerMessage(LogLevel.Warning, "Error while waiting for a event in data loop")]
    public static partial void LogDataLoopWaitError(this ILogger logger, Exception ex);
}
