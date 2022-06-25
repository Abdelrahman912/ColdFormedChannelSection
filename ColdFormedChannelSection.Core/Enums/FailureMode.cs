using System.ComponentModel;

namespace ColdFormedChannelSection.Core.Enums
{
    [TypeConverter(typeof(EnumDescriptionTypeConverter))]
    public enum FailureMode
    {
        [Description("Local Buckling governs")]
        LOCALBUCKLING,
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
        [Description("Distortional Buckling governs")]
        DISTRORTIONALBUCKLING,
        [Description("Unsafe, Try Other Dimensions")]
        UNSAFE
    }
}
