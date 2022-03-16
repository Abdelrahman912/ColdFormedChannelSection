namespace ColdFormedChannelSection.Core.Entities.Output.Resistance
{
    public class MomentResistanceOutput : ResistanceOutput
    {
        public MomentResistanceOutput(double nominalResistance, double phi) 
            : base(nominalResistance, phi, "Mn", "(phi)m")
        {
        }
    }
}
