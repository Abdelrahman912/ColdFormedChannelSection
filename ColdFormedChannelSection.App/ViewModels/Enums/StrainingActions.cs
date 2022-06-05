using ColdFormedChannelSection.Core.Enums;
using System.ComponentModel;

namespace ColdFormedChannelSection.App.ViewModels.Enums
{
    [TypeConverter(typeof(EnumDescriptionTypeConverter))]
    public enum StrainingActions
    {
        [Description("Beam (Moment)")]
        MOMENT,
        [Description("Column (Compression)")]
        COMPRESSION,
        [Description("Beam Column (M & N)")]
        MOMENT_COMPRESSION
    }
}
