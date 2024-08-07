﻿using System;

namespace IracingSdkDotNet.Core;

/// <summary>
/// Options for the <see cref="IracingSdkCore"/>.
/// </summary>
public sealed class IracingSdkOptions
{
    private static readonly TimeSpan DefaultCheckConnectionDelay = TimeSpan.FromSeconds(5);
 
    /// <summary>
    /// The default <see cref="IracingSdkOptions"/>.
    /// </summary>
    public static readonly IracingSdkOptions Default = new();
    
    /// <summary>
    /// The delay between checking the connection to iRacing.
    /// </summary>
    public TimeSpan CheckConnectionDelay { get; set; } = DefaultCheckConnectionDelay;
}