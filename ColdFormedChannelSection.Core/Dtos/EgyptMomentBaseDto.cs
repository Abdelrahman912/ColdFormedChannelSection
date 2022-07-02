using ColdFormedChannelSection.Core.Entities;

namespace ColdFormedChannelSection.Core.Dtos
{
    public abstract class EgyptMomentBaseDto
    {
        #region Properties

        public NominalStrengthDto GoverningCase { get;}

        #endregion

        #region Constructors

        protected EgyptMomentBaseDto(NominalStrengthDto governingCase)
        {
            GoverningCase = governingCase;
        }

        #endregion

        #region Methods

        public abstract ListReportSection AsReportSection();

        #endregion
    }
}
