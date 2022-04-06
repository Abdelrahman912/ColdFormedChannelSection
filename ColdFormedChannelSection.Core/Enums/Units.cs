using System.ComponentModel;

namespace ColdFormedChannelSection.Core.Enums
{
    [TypeConverter(typeof(EnumDescriptionTypeConverter))]
    public enum Units
    {
        [Description("ton & cm")]
        TONCM,
        [Description("N & mm")]
        NMM,
        [Description("kip & inch")]
        KIPINCH,
    }
}
