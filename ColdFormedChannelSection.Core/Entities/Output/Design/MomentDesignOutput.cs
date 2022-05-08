using ColdFormedChannelSection.Core.Enums;

namespace ColdFormedChannelSection.Core.Entities
{
    public class MomentDesignOutput : DesignOutput
    {
        public MomentDesignOutput(double ultimateLoad, string designSection, double nominalResistance, double phi, FailureMode governingCase, string unitName) 
            : base(ultimateLoad, "Mu", designSection, nominalResistance, phi, governingCase, "Mn", "(phi)b", unitName)
        {
        }
    }
}
