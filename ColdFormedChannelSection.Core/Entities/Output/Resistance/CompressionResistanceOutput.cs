using ColdFormedChannelSection.Core.Enums;

namespace ColdFormedChannelSection.Core.Entities
{
    public class CompressionResistanceOutput : ResistanceOutput
    {
        public CompressionResistanceOutput(double nominalResistance, double phi,FailureMode governingCase) 
            : base(nominalResistance, phi,governingCase,"Pn","(phi)c")
        {
        }
    }
}
