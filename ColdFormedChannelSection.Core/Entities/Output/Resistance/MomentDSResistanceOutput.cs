namespace ColdFormedChannelSection.Core.Entities
{
    public class MomentDSResistanceOutput : ResistanceOutput
    {
        public MomentDSResistanceOutput(double nominalResistance, double phi,string governingCase) 
            : base(nominalResistance, phi,governingCase, "Mn", "(phi)m")
        {
        }
    }
}
