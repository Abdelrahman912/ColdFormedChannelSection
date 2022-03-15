using ColdFormedChannelSection.Core.Enums;
using System.ComponentModel;

namespace ColdFormedChannelSection.App.ViewModels.Enums
{
    [TypeConverter(typeof(EnumDescriptionTypeConverter))]
    internal enum Units
    {
        [Description("ton & cm")]
        TONCM,
        [Description("N & mm")]
        NMM,
        [Description("kip & inch")]
        KIPINCH,
    }
}
