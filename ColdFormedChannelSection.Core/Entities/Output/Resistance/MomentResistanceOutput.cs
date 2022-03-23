namespace ColdFormedChannelSection.Core.Entities
{
    public class MomentResistanceOutput : ResistanceOutput
    {
        public MomentResistanceOutput(double nominalResistance, double phi,string governingCase) 
            : base(nominalResistance, phi,governingCase, "Mn", "(phi)m")
        {
        }
    }
}
