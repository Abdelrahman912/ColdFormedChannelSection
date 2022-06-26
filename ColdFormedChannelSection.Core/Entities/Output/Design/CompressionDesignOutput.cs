using ColdFormedChannelSection.Core.Enums;
using ColdFormedChannelSection.Core.Extensions;

namespace ColdFormedChannelSection.Core.Entities
{
    public class CompressionDesignOutput : DesignOutput
    {
       
        public CompressionDesignOutput(double ultimateLoad , string designSection, double nominalResistance, double phi,string phiName , string designResistanceName, FailureMode governingCase, Units unit,IReport report) 
            : base(ultimateLoad, "Pu", designSection, nominalResistance, phi, governingCase, "Pn", phiName,designResistanceName, unit,report)
        {
        }
    }
}
