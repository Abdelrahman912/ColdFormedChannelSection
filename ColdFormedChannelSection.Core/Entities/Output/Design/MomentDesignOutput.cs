using ColdFormedChannelSection.Core.Enums;

namespace ColdFormedChannelSection.Core.Entities
{
    public class MomentDesignOutput : DesignOutput
    {
        
        public MomentDesignOutput(double ultimateLoad, string designSection, double nominalResistance, double phi,string phiName , string designResistanceName, FailureMode governingCase, Units unit,IReport report) 
            : base(ultimateLoad, "Mu", designSection, nominalResistance, phi, governingCase, "Mn", phiName,designResistanceName, unit,report)
        {
        }
    }
}
