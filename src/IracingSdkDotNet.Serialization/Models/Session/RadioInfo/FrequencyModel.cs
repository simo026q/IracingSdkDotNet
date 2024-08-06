namespace IracingSdkDotNet.Serialization.Models.Session.RadioInfo;

public class FrequencyModel
{
    public int FrequencyNum { get; set; } // %d
    public string FrequencyName { get; set; } // "%s"
    public int Priority { get; set; } // %d
    public int CarIdx { get; set; } // %d
    public int EntryIdx { get; set; } // %d
    public int ClubID { get; set; } //%d
    public int CanScan { get; set; } // %d (boolean)
    public int CanSquawk { get; set; } // %d (boolean)
    public int Muted { get; set; } // %d (boolean)
    public int IsMutable { get; set; } // %d (boolean)
    public int IsDeletable { get; set; } // %d (boolean)
}
