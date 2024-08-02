namespace IracingSdkDotNet.Core;

public enum BroadcastMessageType 
{ 
    CamSwitchPos = 0, 
    CamSwitchNum, 
    CamSetState, 
    ReplaySetPlaySpeed, 
    ReplaySetPlayPosition, 
    ReplaySearch, 
    ReplaySetState, 
    ReloadTextures, 
    ChatCommand, 
    PitCommand, 
    TelemCommand 
}