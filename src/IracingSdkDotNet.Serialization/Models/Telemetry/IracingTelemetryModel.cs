using IracingSdkDotNet.Core.Reader;
using IracingSdkDotNet.Serialization.Enums;
using System;

namespace IracingSdkDotNet.Serialization.Models.Telemetry;

/// <summary>
/// Represents the telemetry data from the simulation.
/// </summary>
public class IracingTelemetryModel
{
    private readonly IracingDataReader _dataReader;

    /// <summary>
    /// Initializes a new instance of the <see cref="IracingTelemetryModel"/> class.
    /// </summary>
    /// <param name="reader">The <see cref="IracingDataReader"/> to read the data from.</param>
    public IracingTelemetryModel(IracingDataReader reader)
    {
        _dataReader = reader;
    }

    /// <summary>
    /// The amount of seconds since the session started.
    /// </summary>
    public double SessionElapsedTime => _dataReader.TryReadDouble("SessionTime", out double value) 
        ? value 
        : default;

    /// <summary>
    /// The current update number.
    /// </summary>
    public int SessionTick => _dataReader.TryReadInt32("SessionTick", out int value) 
        ? value 
        : default;

    /// <summary>
    /// The session number.
    /// </summary>
    public int SessionNumber => _dataReader.TryReadInt32("SessionNum", out int value) 
        ? value 
        : default;

    /// <summary>
    /// The session state.
    /// </summary>
    public SessionState SessionState => _dataReader.TryReadInt32("SessionState", out int value) 
        ? (SessionState)value 
        : default;

    /// <summary>
    /// The unique session identifier.
    /// </summary>
    public int SessionId => _dataReader.TryReadInt32("SessionUniqueID", out int value) 
        ? value 
        : default;

    /// <summary>
    /// The session flags.
    /// </summary>
    public SessionFlags SessionFlags => _dataReader.TryReadBitField("SessionFlags", out SessionFlags value) 
        ? value 
        : default;

    /// <summary>
    /// The session time remaining.
    /// </summary>
    public double SessionTimeRemaining => _dataReader.TryReadDouble("SessionTimeRemain", out double value) 
        ? value 
        : default;

    /// <summary>
    /// The session laps remaining.
    /// </summary>
    [Obsolete("Use SessionLapsRemaining instead.", error: false)]
    public int SessionLapsRemainingOld => _dataReader.TryReadInt32("SessionLapsRemain", out int value) 
        ? value 
        : default;

    /// <summary>
    /// The session laps remaining.
    /// </summary>
    public int SessionLapsRemaining => _dataReader.TryReadInt32("SessionLapsRemainEx", out int value) 
        ? value 
        : default;

    /// <summary>
    /// The total number of seconds in the session.
    /// </summary>
    public double SessionTimeTotal => _dataReader.TryReadDouble("SessionTimeTotal", out double value) 
        ? value 
        : default;

    /// <summary>
    /// The total number of laps in the session.
    /// </summary>
    public int SessionLapsTotal => _dataReader.TryReadInt32("SessionLapsTotal", out int value) 
        ? value 
        : default;

    /// <summary>
    /// The number of joker laps remaining to be taken.
    /// </summary>
    public int SessionJokerLapsRemaining => _dataReader.TryReadInt32("SessionJokerLapsRemain", out int value) 
        ? value 
        : default;

    /// <summary>
    /// Indicates that the player is currently on a joker lap.
    /// </summary>
    public bool IsOnJokerLap => _dataReader.TryReadBoolean("SessionOnJokerLap", out bool value) && value;
    
    /// <summary>
    /// The session time of day in seconds.
    /// </summary>
    public float SessionTimeOfDay => _dataReader.TryReadSingle("SessionTimeOfDay", out float value) 
        ? value 
        : default;

    /// <summary>
    /// The car index of the current person speaking on the radio. 
    /// </summary>
    public int RadioTransmitCarIndex => _dataReader.TryReadInt32("RadioTransmitCarIdx", out int value) 
        ? value 
        : default;

    /// <summary>
    /// The radio index of the current person speaking on the radio.
    /// </summary>
    public int RadioTransmitRadioIndex => _dataReader.TryReadInt32("RadioTransmitRadioIdx", out int value) 
        ? value 
        : default;

    /// <summary>
    /// The frequency index of the current person speaking on the radio.
    /// </summary>
    public int RadioTransmitFrequencyIndex => _dataReader.TryReadInt32("RadioTransmitFrequencyIdx", out int value) 
        ? value 
        : default;

    /// <summary>
    /// The unit system used for displaying data to the user.
    /// </summary>
    public UnitSystem DisplayUnitSystem => _dataReader.TryReadInt32("DisplayUnits", out int value) 
        ? (UnitSystem)value 
        : default;

    /// <summary>
    /// Driver activated flag.
    /// </summary>
    public bool DriverMarker => _dataReader.TryReadBoolean("DriverMarker", out bool value) && value;

    /// <summary>
    /// The push-to-talk button is being held.
    /// </summary>
    public bool IsPushToTalkActive => _dataReader.TryReadBoolean("PushToTalk", out bool value) && value;

    /// <summary>
    /// The push-to-pass button is being held.
    /// </summary>
    public bool IsPushToPassActive => _dataReader.TryReadBoolean("PushToPass", out bool value) && value;

    /// <summary>
    /// The hybrid manual boost button is being held.
    /// </summary>
    public bool IsManualBoostActive => _dataReader.TryReadBoolean("ManualBoost", out bool value) && value;

    /// <summary>
    /// Hybrid manual no boost state.
    /// </summary>
    // TODO: Check what this actually does and update the documentation and property name.
    public bool ManualNoBoost => _dataReader.TryReadBoolean("ManualNoBoost", out bool value) && value;

    /// <summary>
    /// The player is currently on the track.
    /// </summary>
    public bool IsOnTrack => _dataReader.TryReadBoolean("IsOnTrack", out bool value) && value;

    /// <summary>
    /// The replay is currently playing.
    /// </summary>
    public bool IsReplayPlaying => _dataReader.TryReadBoolean("IsReplayPlaying", out bool value) && value;

    /// <summary>
    /// The replay frame number. (60 per second)
    /// </summary>
    public int ReplayFrame => _dataReader.TryReadInt32("ReplayFrameNum", out int value) 
        ? value 
        : default;

    /// <summary>
    /// The last frame of the replay.
    /// </summary>
    public int ReplayLastFrame => _dataReader.TryReadInt32("ReplayFrameNumEnd", out int value) 
        ? value 
        : default;

    /// <summary>
    /// Whether or not disk based telemetry logging is enabled.
    /// </summary>
    public bool IsDiskLoggingEnabled => _dataReader.TryReadBoolean("IsDiskLoggingEnabled", out bool value) && value;

    /// <summary>
    /// Whether or not disk based telemetry logging is being actively written to.
    /// </summary>
    public bool IsDiskLoggingActive => _dataReader.TryReadBoolean("IsDiskLoggingActive", out bool value) && value;
    
    /// <summary>
    /// The average frame rate of the simulation.
    /// </summary>
    public float FrameRate => _dataReader.TryReadSingle("FrameRate", out float value) 
        ? value 
        : default;

    /// <summary>
    /// Percent of available tim fg thread took with a 1 sec avg.
    /// </summary>
    public float CpuUsageFG => _dataReader.TryReadSingle("CpuUsageFG", out float value) 
        ? value 
        : default;

    /// <summary>
    /// Percent of available tim bg thread took with a 1 sec avg.
    /// </summary>
    public float CpuUsageBG => _dataReader.TryReadSingle("CpuUsageBG", out float value) 
        ? value 
        : default;

    /// <summary>
    /// Percent of available tim gpu took with a 1 sec avg.
    /// </summary>
    public float GpuUsage => _dataReader.TryReadSingle("GpuUsage", out float value) 
        ? value 
        : default;

    /// <summary>
    /// The average latency of communication.
    /// </summary>
    public float AverageCommunicationLatency => _dataReader.TryReadSingle("ChanAvgLatency", out float value) 
        ? value 
        : default;

    /// <summary>
    /// The latency of communication.
    /// </summary>
    public float CommunicationLatency => _dataReader.TryReadSingle("ChanLatency", out float value) 
        ? value 
        : default;

    /// <summary>
    /// The quality of communication.
    /// </summary>
    public float CommunicationQuality => _dataReader.TryReadSingle("ChanQuality", out float value) 
        ? value 
        : default;

    /// <summary>
    /// The quality of the communication partner.
    /// </summary>
    public float CommunicationPartnerQuality => _dataReader.TryReadSingle("ChanPartnerQuality", out float value) 
        ? value 
        : default;

    /// <summary>
    /// The clock skew of the communication server.
    /// </summary>
    public float CommunicationServerClockSkew => _dataReader.TryReadSingle("ChanClockSkew", out float value) 
        ? value 
        : default;

    /// <summary>
    /// The memory page faults per second.
    /// </summary>
    public float MemoryPageFaultsPerSecond => _dataReader.TryReadSingle("MemPageFaultSec", out float value) 
        ? value 
        : default;

    /// <summary>
    /// The memory soft page faults per second.
    /// </summary>
    public float MemorySoftPageFaultsPerSecond => _dataReader.TryReadSingle("MemSoftPageFaultSec", out float value) 
        ? value 
        : default;

    /// <summary>
    /// The player's position.
    /// </summary>
    public int Position => _dataReader.TryReadInt32("PlayerCarPosition", out int value) 
        ? value 
        : default;

    /// <summary>
    /// The player's class position.
    /// </summary>
    public int PlayerCarClassPosition => _dataReader.TryReadInt32("PlayerCarClassPosition", out int value) 
        ? value
        : default;

    /// <summary>
    /// The player's car class ID.
    /// </summary>
    public int PlayerCarClassId => _dataReader.TryReadInt32("PlayerCarClass", out int value) 
        ? value 
        : default;

    /// <summary>
    /// The player's location in the world.
    /// </summary>
    public WorldLocation PlayerWorldLocation => _dataReader.TryReadInt32("PlayerTrackSurface", out int value) 
        ? (WorldLocation)value 
        : WorldLocation.NotInWorld;

    /// <summary>
    /// The track surface material the player is currently on.
    /// </summary>
    public TrackSurface PlayerTrackSurface => _dataReader.TryReadInt32("PlayerTrackSurfaceMaterial", out int value) 
        ? (TrackSurface)value 
        : TrackSurface.NotInWorld;

    /// <summary>
    /// The player's car index.
    /// </summary>
    public int PlayerCarIndex => _dataReader.TryReadInt32("PlayerCarIdx", out int value) 
        ? value 
        : default;

    /// <summary>
    /// The player's team incident count for the session.
    /// </summary>
    public int PlayerCarTeamIncidentCount => _dataReader.TryReadInt32("PlayerCarTeamIncidentCount", out int value) 
        ? value 
        : default;

    /// <summary>
    /// The player's incident count for the session.
    /// </summary>
    public int PlayerCarMyIncidentCount => _dataReader.TryReadInt32("PlayerCarMyIncidentCount", out int value) 
        ? value 
        : default;

    /// <summary>
    /// The player-team's current driver incident count for the session.
    /// </summary>
    public int PlayerCarDriverIncidentCount => _dataReader.TryReadInt32("PlayerCarDriverIncidentCount", out int value) 
        ? value 
        : default;

    /// <summary>
    /// The player's weight penalty in kilograms.
    /// </summary>
    public float PlayerCarWeightPenalty => _dataReader.TryReadSingle("PlayerCarWeightPenalty", out float value) 
        ? value 
        : default;

    /// <summary>
    /// The player's power adjust in percent.
    /// </summary>
    public float PlayerCarPowerAdjust => _dataReader.TryReadSingle("PlayerCarPowerAdjust", out float value) 
        ? value 
        : default;

    /// <summary>
    /// The player's dyre tire set limit.
    /// </summary>
    public int PlayerCarDryTireSetLimit => _dataReader.TryReadInt32("PlayerCarDryTireSetLimit", out int value) 
        ? value 
        : default;

    /// <summary>
    /// The player's tow time in seconds. The car is being towed if the value is greater than 0.
    /// </summary>
    public float PlayerCarTowTime => _dataReader.TryReadSingle("PlayerCarTowTime", out float value) 
        ? value 
        : default;

    /// <summary>
    /// The player's car is in the pit stall.
    /// </summary>
    public bool IsInPitStall => _dataReader.TryReadBoolean("PlayerCarInPitStall", out bool value) && value;
    
    /// <summary>
    /// The player's car pit service status.
    /// </summary>
    public PitServiceStatus PlayerCarPitServiceStatus => _dataReader.TryReadInt32("PlayerCarPitSvStatus", out int value) 
        ? (PitServiceStatus)value 
        : PitServiceStatus.None;

    /// <summary>
    /// The player's current tire compound.
    /// </summary>
    public int PlayerTireCompound => _dataReader.TryReadInt32("PlayerCarTireCompound", out int value) 
        ? value 
        : default;

    /// <summary>
    /// The fast repairs used by the player.
    /// </summary>
    public int PlayerFastRepairsUsed => _dataReader.TryReadInt32("PlayerFastRepairsUsed", out int value) 
        ? value 
        : default;

    /// <summary>
    /// The player's current lap.
    /// </summary>
    public int CarIndexLap => _dataReader.TryReadInt32("CarIdxLap", out int value) 
        ? value 
        : default;

    /// <summary>
    /// The player's laps completed.
    /// </summary>
    public int CarIndexLapCompleted => _dataReader.TryReadInt32("CarIdxLapCompleted", out int value) 
        ? value 
        : default;

    /// <summary>
    /// The player's lap distance in percent.
    /// </summary>
    public float CarIndexLapDistance => _dataReader.TryReadSingle("CarIdxLapDist", out float value) 
        ? value 
        : default;

    /// <summary>
    /// The track surface material the player is currently on.
    /// </summary>
    public WorldLocation CarIndexWorldLocation => _dataReader.TryReadInt32("CarIdxTrackSurface", out int value) 
        ? (WorldLocation)value 
        : WorldLocation.NotInWorld;
}