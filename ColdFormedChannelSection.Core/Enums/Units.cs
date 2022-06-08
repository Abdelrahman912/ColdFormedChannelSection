using System.ComponentModel;

namespace ColdFormedChannelSection.Core.Enums
{
    [TypeConverter(typeof(EnumDescriptionTypeConverter))]
    public enum Units
    {
        [Description("cm")]
        CM,
        [Description("in")]
        IN,
        [Description("mm")]
        MM,
        [Description("cm^2")]
        CM_2,
        [Description("in^2")] 
        IN_2,
        [Description("mm^2")] 
        MM_2,
        [Description("cm^3")]
        CM_3,
        [Description("in^3")] 
        IN_3,
        [Description("mm^3")] 
        MM_3,
        [Description("ton")]
        TON,
        [Description("kip")] 
        KIP,
        [Description("N")] 
        N,
        [Description("t.cm")]
        TON_CM,
        [Description("kip.in")]
        KIP_IN,
        [Description("N.mm")] 
        N_MM,
        [Description("t/cm^2")]
        TON_CM_2,
        [Description("Ksi")]
        KSI,
        [Description("N/mm^2")] 
        N_MM_2,
        [Description("-")]
        NONE
    }
}
