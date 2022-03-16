namespace ColdFormedChannelSection.Core.Entities.Output.Resistance
{
    public class CompressionResistanceOutput : ResistanceOutput
    {
        public CompressionResistanceOutput(double nominalResistance, double phi) 
            : base(nominalResistance, phi,"Pn","(phi)c")
        {
        }
    }
}
