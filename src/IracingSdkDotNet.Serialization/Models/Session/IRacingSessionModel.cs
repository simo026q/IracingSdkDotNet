using IracingSdkDotNet.Serialization.Models.Session.CameraInfo;
using IracingSdkDotNet.Serialization.Models.Session.DriverInfo;
using IracingSdkDotNet.Serialization.Models.Session.QualifyResultsInfo;
using IracingSdkDotNet.Serialization.Models.Session.RadioInfo;
using IracingSdkDotNet.Serialization.Models.Session.SessionInfo;
using IracingSdkDotNet.Serialization.Models.Session.SplitTimeInfo;
using IracingSdkDotNet.Serialization.Models.Session.WeekendInfo;
using IracingSdkDotNet.Serialization.Yaml;
using System;

namespace IracingSdkDotNet.Serialization.Models.Session;

public class IracingSessionModel
{
    private static readonly YamlSerializationContext SerializationContext = new(typeof(IracingSessionModel));

    public WeekendInfoModel WeekendInfo { get; set; }
    public SessionInfoModel SessionInfo { get; set; }
    public QualifyResultsInfoModel QualifyResultsInfo { get; set; } // Not validated
    public CameraInfoModel CameraInfo { get; set; }
    public RadioInfoModel RadioInfo { get; set; }
    public DriverInfoModel DriverInfo { get; set; }
    public SplitTimeInfoModel SplitTimeInfo { get; set; }
    // The CarSetup section is too complex to be implemented since it's different for each car.

    public static IracingSessionModel Deserialize(string yaml)
    {
        return YamlSerializer.Deserialize<IracingSessionModel>(yaml, SerializationContext) 
            ?? throw new InvalidOperationException("Deserialization failed.");
    }
}
