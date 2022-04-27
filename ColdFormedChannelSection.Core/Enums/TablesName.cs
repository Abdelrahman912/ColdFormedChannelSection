using System.ComponentModel;

namespace ColdFormedChannelSection.Core.Enums
{
    [TypeConverter(typeof(EnumDescriptionTypeConverter))]
    public enum TablesName
    {
        [Description("Euro\\Egypt")]
        EGYPT_EURO,
        [Description("American")]
        AMERICAN
    }
}
