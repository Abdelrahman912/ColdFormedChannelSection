using System.ComponentModel;

namespace ColdFormedChannelSection.Core.Enums
{
    [TypeConverter(typeof(EnumDescriptionTypeConverter))]
    public enum FailureMode
    {
        [Description("Local Buckling governs")]
        LOCALBUCKLING,
        [Description("Local and Distorsional Buckling")]
        LOCAL_DISTORSIONAL_BUCKLING,
        [Description("Torsional Buckling governs")]
        TORSIONALBUCKLING,
        [Description("Flexural Torsional Buckling governs")]
        FLEXURAL_TORSIONAL_BUCKLING,
        [Description("Flexural Buckling governs")]
        FLEXURALBUCKLING,
        [Description("Lateral Torsional Buckling governs")]
        LATERALTORSIONALBUCKLING,
        [Description("Global Buckling governs")]
        GLOBALBUCKLING,
        [Description("Distorsional Buckling governs")]
        DISTRORSIONALBUCKLING,
        [Description("Unsafe, Try Other Dimensions")]
        UNSAFE
    }
}
