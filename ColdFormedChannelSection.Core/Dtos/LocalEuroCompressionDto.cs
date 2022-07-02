namespace ColdFormedChannelSection.Core.Dtos
{
    public class LocalEuroCompressionDto:NominalStrengthDto
    {
        #region Properties

        public double Ae { get; }

        public double Be { get;}

        public double Ce { get; }

        public double Kw { get;  }

        public double Kf { get; }

        public double Kc { get; }

        public double Fy { get;}

        public double Xd { get; }

        public double AreaEffective { get;}

        #endregion

        #region Constructors

        public LocalEuroCompressionDto(double ae, double be, double ce, double kw, double kf, double kc, double fy, double areaEffective,double pn,double xd)
            :base(pn,Enums.FailureMode.LOCALBUCKLING)
        {
            Ae = ae;
            Be = be;
            Ce = ce;
            Kw = kw;
            Kf = kf;
            Kc = kc;
            Fy = fy;
            AreaEffective = areaEffective;
            Xd = xd;
        }

        #endregion

    }
}
