namespace ColdFormedChannelSection.Core.Entities
{
    public class CompressionDSResistanceOutput : ResistanceOutput
    {
        public CompressionDSResistanceOutput(double nominalResistance, double phi,string governingCase) 
            : base(nominalResistance, phi,governingCase,"Pn","(phi)c")
        {
        }
    }
}
