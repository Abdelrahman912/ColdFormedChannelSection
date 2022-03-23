namespace ColdFormedChannelSection.Core.Entities
{
    public class CompressionResistanceOutput : ResistanceOutput
    {
        public CompressionResistanceOutput(double nominalResistance, double phi,string governingCase) 
            : base(nominalResistance, phi,governingCase,"Pn","(phi)c")
        {
        }
    }
}
