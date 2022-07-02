using ColdFormedChannelSection.Core.Entities;
using ColdFormedChannelSection.Core.Helpers;

namespace ColdFormedChannelSection.Core.Dtos
{
    public class EgyptMomentLTBDto:EgyptMomentBaseDto
    {
       
        #region Properties

        public double ZLocal { get;  }

        public double CStar { get; }

        public double LambdaF { get;}

        public double FLocal { get; }

        public double ZLTB { get; }

        public double FLTB { get; }

        public double MnLocal { get; }

        public double MnLTB { get; }

        #endregion

        #region Constructors

        public EgyptMomentLTBDto(double zLocal, double cStar, double lambdaF, double fLocal, double zLTB, double fLTB, double mnLocal, double mnLTB,NominalStrengthDto dto)
            :base(dto)
        {
            ZLocal = zLocal;
            CStar = cStar;
            LambdaF = lambdaF;
            FLocal = fLocal;
            ZLTB = zLTB;
            FLTB = fLTB;
            MnLocal = mnLocal;
            MnLTB = mnLTB;
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
