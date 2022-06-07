using System.ComponentModel;

namespace ColdFormedChannelSection.Core.Enums
{
    [TypeConverter(typeof(EnumDescriptionTypeConverter))]
    public enum UnitSystems
    {
        [Description("ton & cm")]
        TONCM,
        [Description("N & mm")]
        NMM,
        [Description("kip & inch")]
        KIPINCH,
    }
}
