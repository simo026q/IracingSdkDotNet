using System.Collections.Generic;

namespace IracingSdkDotNet.Serialization.Models.Session.RadioInfo;

public class RadioInfoModel
{
    public int SelectedRadioNum { get; set; } // %d
    public List<RadioModel> Radios { get; set; }
}
