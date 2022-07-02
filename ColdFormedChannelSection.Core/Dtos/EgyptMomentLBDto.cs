using ColdFormedChannelSection.Core.Entities;
using  ColdFormedChannelSection.Core.Helpers;

namespace ColdFormedChannelSection.Core.Dtos
{
    public class EgyptMomentLBDto:EgyptMomentBaseDto
    {
       
        #region Properties

        public double LambdaF { get;}

        #endregion

        #region Constructors
        public EgyptMomentLBDto(double lambdaF,NominalStrengthDto dto)
            :base(dto)
        {
            LambdaF = lambdaF;
        }



        #endregion

        #region Methods

        public override ListReportSection AsReportSection()
        {
            return ReportHelper.AsReportSection(this);
        }

        #endregion
    }
}
