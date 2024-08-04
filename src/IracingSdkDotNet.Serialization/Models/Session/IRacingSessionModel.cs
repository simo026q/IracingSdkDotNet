using IracingSdkDotNet.Serialization.Models.Session.CameraInfo;
using IracingSdkDotNet.Serialization.Models.Session.DriverInfo;
using IracingSdkDotNet.Serialization.Models.Session.QualifyResultsInfo;
using IracingSdkDotNet.Serialization.Models.Session.RadioInfo;
using IracingSdkDotNet.Serialization.Models.Session.SessionInfo;
using IracingSdkDotNet.Serialization.Models.Session.SplitTimeInfo;
using IracingSdkDotNet.Serialization.Models.Session.WeekendInfo;

namespace IracingSdkDotNet.Serialization.Models.Session;

public class IRacingSessionModel
{
    public WeekendInfoModel WeekendInfo { get; set; }
    public SessionInfoModel SessionInfo { get; set; }
    public QualifyResultsInfoModel QualifyResultsInfo { get; set; } // Not validated
    public CameraInfoModel CameraInfo { get; set; }
    public RadioInfoModel RadioInfo { get; set; }
    public DriverInfoModel DriverInfo { get; set; }
    public SplitTimeInfoModel SplitTimeInfo { get; set; }
    // The CarSetup section is too complex to be implemented since it's different for each car.
}
