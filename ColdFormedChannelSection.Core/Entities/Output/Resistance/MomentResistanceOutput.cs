using ColdFormedChannelSection.Core.Enums;

namespace ColdFormedChannelSection.Core.Entities
{
    public class MomentResistanceOutput : ResistanceOutput
    {
        public MomentResistanceOutput(double nominalResistance, double phi,string factorName, FailureMode governingCase, string unitName, IReport report)
           : base(nominalResistance, phi, governingCase, "Mn", factorName, unitName, report)
        {

        }

        public MomentResistanceOutput(double nominalResistance, double phi,FailureMode governingCase, string unitName,IReport report) 
            : base(nominalResistance, phi,governingCase, "Mn", "(phi)b",unitName,report)
        {
        }
    }
}
