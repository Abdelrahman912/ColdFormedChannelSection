using ColdFormedChannelSection.Core.Entities;
using ColdFormedChannelSection.Core.Enums;

namespace ColdFormedChannelSection.Core.Dtos
{
    public abstract class LocalDSCompressionDto
    {
       
        #region Properties

        public double Pcrl { get; }

        #endregion

        #region Constructors
        protected LocalDSCompressionDto(double pcrl)
        {
            Pcrl = pcrl;
        }
        #endregion

        #region Methods

        #endregion

        public abstract ListReportSection AsReportSection(TypeOfSection sectionType);
    }
}
