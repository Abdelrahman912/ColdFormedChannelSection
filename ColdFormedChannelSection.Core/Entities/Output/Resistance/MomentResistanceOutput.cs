using ColdFormedChannelSection.Core.Enums;

namespace ColdFormedChannelSection.Core.Entities
{
    public class MomentResistanceOutput : ResistanceOutput
    {
        public MomentResistanceOutput(double nominalResistance, double phi,string phiName , string designResistanceName,FailureMode governingCase, string unitName,IReport report) 
            : base(nominalResistance, phi,governingCase, "Mn", phiName,designResistanceName,unitName,report)
        {
        }
    }
}
