namespace ColdFormedChannelSection.Core.Dtos
{
    public class LocalEgyptCompressionDto:NominalStrengthDto
    {
        

        #region Properties

        public double Ae { get;  }

        public double Be { get; }

        public double Ce { get; }

        public double Kw { get; }

        public double Kc { get; }

        public double Kf { get;}

        public double Fy { get; }

        public double  AreaEffective { get; }

        #endregion

        #region Constructors

        public LocalEgyptCompressionDto(double ae, double be, double ce, double kw, double kc, double kf, double fy, double areaEffective,double pn)
            :base(pn,Enums.FailureMode.LOCALBUCKLING)
        {
            Ae = ae;
            Be = be;
            Ce = ce;
            Kw = kw;
            Kc = kc;
            Kf = kf;
            Fy = fy;
            AreaEffective = areaEffective;
        }

        #endregion
    }
}
