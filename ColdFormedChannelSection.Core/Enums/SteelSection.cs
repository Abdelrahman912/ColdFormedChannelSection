using System.ComponentModel;

namespace ColdFormedChannelSection.Core.Enums
{
    [TypeConverter(typeof(EnumDescriptionTypeConverter))]
    public enum SteelSection
    {
        [Description("C - Lipped")]
        C_LIPPED,
        [Description("C - Unstiffened")]
        C_UNSTIFFENED,
        [Description("Z - Lipped")]
        Z_LIPPED,
        [Description("Z - Unstiffened")]
        Z_UNSTIFFENED
    }
}
