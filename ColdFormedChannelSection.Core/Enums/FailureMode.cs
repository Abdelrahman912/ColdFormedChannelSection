using System.ComponentModel;

namespace ColdFormedChannelSection.Core.Enums
{
    [TypeConverter(typeof(EnumDescriptionTypeConverter))]
    public enum FailureMode
    {
        [Description("Local Buckling governs")]
        LOCALBUCKLING,
        [Description("Flexural Torsional Buckling governs")]
        TORSIONALBUCKLING,
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
