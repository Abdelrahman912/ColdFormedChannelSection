namespace ColdFormedChannelSection.Core.Entities
{
    public abstract  class Section
    {
        

        #region Properties

        public SectionDimension Dimensions { get;  }

        public SectionProperties Properties { get; }

        #endregion

        #region Constructors

        public Section(SectionDimension dimensions,SectionProperties properties)
        {
            Dimensions = dimensions;
            Properties = properties;
    }

        #endregion

    }
}
