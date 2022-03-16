namespace ColdFormedChannelSection.Core.Entities
{
    public class MomentDSResistanceInput : ResistanceInput
    {
        public MomentDSResistanceInput(Section section, Material material, LengthBracingConditions bracingConditions) 
            : base(section, material, bracingConditions)
        {
        }
    }
}
