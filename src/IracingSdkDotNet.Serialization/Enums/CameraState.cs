using System;

namespace IracingSdkDotNet.Serialization.Enums;

[Flags]
public enum CameraState
{
    None                = 0x0000,

    SessionScreen       = 0x0001,
    ScenicCameraActive  = 0x0002,

    CameraToolActive    = 0x0004,
    UserInterfaceHidden = 0x0008,
    AutoShotSelection   = 0x0010,
    TemporaryEdits      = 0x0020,
    KeyAcceleration     = 0x0040,
    Key10xAcceleration  = 0x0080,
    MouseAimMode        = 0x0100
}
