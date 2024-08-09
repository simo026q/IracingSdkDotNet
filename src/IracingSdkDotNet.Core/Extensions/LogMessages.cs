using Microsoft.Extensions.Logging;
using System;
using System.IO;

namespace IracingSdkDotNet.Core.Extensions;

internal static partial class LogMessages
{
    [LoggerMessage(1, LogLevel.Information, "Starting the iRacing SDK.")]
    public static partial void LogStarting(this ILogger logger);

    [LoggerMessage(2, LogLevel.Information, "Stopping the iRacing SDK.")]
    public static partial void LogStopping(this ILogger logger);

    [LoggerMessage(3, LogLevel.Information, "Connected to iRacing.")]
    public static partial void LogConnected(this ILogger logger);

    [LoggerMessage(4, LogLevel.Warning, "Disconnected from iRacing.")]
    public static partial void LogDisconnected(this ILogger logger);

    [LoggerMessage(5, LogLevel.Debug, "The loop has stopped")]
    public static partial void LogLoopStopped(this ILogger logger);

    [LoggerMessage(100, LogLevel.Warning, "Could not open iRacing's memory mapped file.")]
    public static partial void LogMemoryMappedFileOpenFailed(this ILogger logger, FileNotFoundException ex);

    [LoggerMessage(101, LogLevel.Debug, "Opened iRacing's memory mapped file.")]
    public static partial void LogMemoryMappedFileOpened(this ILogger logger);

    [LoggerMessage(102, LogLevel.Information, "Waiting {CheckConnectionDelay} before retrying to connect to iRacing.")]
    public static partial void LogWaitingForConnectionRetry(this ILogger logger, TimeSpan checkConnectionDelay);

    [LoggerMessage(103, LogLevel.Debug, "The CancellationToken was cancelled while waiting for next connect retry to iRacing.")]
    public static partial void LogConnectionRetryWaitCancelled(this ILogger logger);

    [LoggerMessage(104, LogLevel.Trace, "The data in iRacing's shared memory has been updated.")]
    public static partial void LogDataUpdated(this ILogger logger);

    [LoggerMessage(105, LogLevel.Warning, "Error while waiting for a event in data loop")]
    public static partial void LogDataLoopWaitError(this ILogger logger, Exception ex);
}
