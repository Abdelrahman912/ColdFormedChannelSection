namespace ColdFormedChannelSection.Core.Dtos
{
    public class LocalEuroMomentDto:NominalStrengthDto
    {


        #region Properties

        public double Kw { get; }

        public double Ae { get;  }

        public double Kf { get; }

        public double Be { get; }

        public double Kc { get; }

        public double Ce { get; }

        public double Ze { get;  }

        public double Fy { get;}

        public double Xd { get; }

        #endregion

        #region Constructors

        public LocalEuroMomentDto(double ze, double fy,double ae, double be, double ce, double mn, double xd , double kw , double kf , double kc)
            : base(mn, Enums.FailureMode.LOCALBUCKLING)
        {
            Ze = ze;
            Fy = fy;
            Ae = ae;
            Be = be;
            Ce = ce;
            Xd = xd;
            Kw = kw;
            Kf = kf;
            Kc = kc;
        }

        #endregion

    }
}
