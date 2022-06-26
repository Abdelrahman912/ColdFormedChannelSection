using ColdFormedChannelSection.Core.Enums;

namespace ColdFormedChannelSection.Core.Entities
{
    public class CompressionResistanceOutput : ResistanceOutput
    {

        public CompressionResistanceOutput(double nominalResistance, double phi , string phiName,string designResistanceName,FailureMode governingCase,string unitName,IReport report) 
            : base(nominalResistance, phi,governingCase,"Pn",phiName,designResistanceName,unitName,report)
        {
        }
    }
}
