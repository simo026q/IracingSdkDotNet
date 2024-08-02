using System;

namespace IracingSdkDotNet;

public sealed class IracingSdkOptions
{
    private static readonly TimeSpan DefaultCheckConnectionDelay = TimeSpan.FromSeconds(5);
 
    public static readonly IracingSdkOptions Default = new();
    
    public TimeSpan CheckConnectionDelay { get; set; } = DefaultCheckConnectionDelay;
}