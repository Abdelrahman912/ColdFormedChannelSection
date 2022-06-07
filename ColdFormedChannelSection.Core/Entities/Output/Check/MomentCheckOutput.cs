using ColdFormedChannelSection.Core.Enums;

namespace ColdFormedChannelSection.Core.Entities
{
    public class MomentCheckOutput : CheckOutput
    {
        public MomentCheckOutput(double ultimateLoad, double nominalResistance, double phi, FailureMode governingCase, string unitName, IReport report) 
            : base(ultimateLoad, "Mu", nominalResistance, phi, governingCase, "Mn", "(phi)b", unitName,report)
        {
        }
    }
}
