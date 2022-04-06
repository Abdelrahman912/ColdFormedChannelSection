using ColdFormedChannelSection.Core.Enums;

namespace ColdFormedChannelSection.Core.Entities
{
    public class MomentResistanceOutput : ResistanceOutput
    {
        public MomentResistanceOutput(double nominalResistance, double phi,FailureMode governingCase, string unitName) 
            : base(nominalResistance, phi,governingCase, "Mn", "(phi)b",unitName)
        {
        }
    }
}
