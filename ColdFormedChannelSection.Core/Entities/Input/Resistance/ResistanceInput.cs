namespace ColdFormedChannelSection.Core.Entities
{
    public abstract class ResistanceInput
    {
        

        #region Properties

        public Section Section { get; }

        public Material Material { get; }

        public LengthBracingConditions BracingConditions { get; }

        #endregion

        #region Constructors

        protected ResistanceInput(Section section, Material material, LengthBracingConditions bracingConditions)
        {
            Section = section;
            Material = material;
            BracingConditions = bracingConditions;
        }

        #endregion

    }
}
