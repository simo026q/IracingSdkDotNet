using System.Collections.Generic;

namespace IracingSdkDotNet.Serialization.Models.Session.SessionInfo;

public class SessionInfoModel
{
    public int NumSessions { get; set; } // Might be deprecated
    public List<SessionModel> Sessions { get; set; }
}
