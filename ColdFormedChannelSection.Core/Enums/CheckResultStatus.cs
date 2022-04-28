using System.ComponentModel;

namespace ColdFormedChannelSection.Core.Enums
{
    [TypeConverter(typeof(EnumDescriptionTypeConverter))]
    public enum CheckResultStatus
    {
        [Description("Safe")]
        SAFE,
        [Description("Unsafe")]
        UNSAFE
    }
}
