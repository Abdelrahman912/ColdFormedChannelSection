using System.ComponentModel;

namespace ColdFormedChannelSection.Core.Enums
{
    [TypeConverter(typeof(EnumDescriptionTypeConverter))]
    public enum DesignCode
    {
        [Description("Egyptian")]
        EGYPTIAN,
        [Description("Euro")]
        EURO,
        [Description("AISI")]
        AISI
    }
}
