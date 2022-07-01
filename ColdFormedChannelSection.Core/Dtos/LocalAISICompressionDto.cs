using ColdFormedChannelSection.Core.Enums;

namespace ColdFormedChannelSection.Core.Dtos
{
    public class LocalAISICompressionDto:NominalStrengthDto
    {
     

        #region Properties

        public double Kw { get; }

        public double Ae { get; }

        public double Kf { get; }

        public double Be { get; }

        public double Kc { get; }

        public double Ce { get; }

        public double AreaEffective { get; }

        public double Fy { get; }

        #endregion

        #region Constructors

        public LocalAISICompressionDto(double kw, double ae, double kf, double be, double kc, double ce, double areaEffective, double fy,double pn )
            :base(pn,FailureMode.LOCALBUCKLING)
        {
            Kw = kw;
            Ae = ae;
            Kf = kf;
            Be = be;
            Kc = kc;
            Ce = ce;
            AreaEffective = areaEffective;
            Fy = fy;
        }

        #endregion
    }
}
