using ColdFormedChannelSection.Core.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace ColdFormedChannelSection.App.ViewModels.Enums
{
    [TypeConverter(typeof(EnumDescriptionTypeConverter))]
    public enum StrainingActions
    {
        [Description("Moment")]
        MOMENT,
        [Description("Compression")]
        COMPRESSION
    }
}
