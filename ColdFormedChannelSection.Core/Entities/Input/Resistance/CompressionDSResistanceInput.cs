namespace ColdFormedChannelSection.Core.Entities.Input.Resistance
{
    public class CompressionDSResistanceInput : ResistanceInput
    {
        public CompressionDSResistanceInput(Section section, Material material, LengthBracingConditions bracingConditions) 
            : base(section, material, bracingConditions)
        {
        }
    }
}
