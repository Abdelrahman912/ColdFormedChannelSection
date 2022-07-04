namespace ColdFormedChannelSection.Core.Dtos
{
    public class LocalEgyptMomentDto
    {
        #region Properties

        public double Ae { get;  }

        public double Be { get; }

        public double Ce { get; }

        public double Ze { get; }

        public double Fy { get;}

        public double Kw { get; }

        public double Kf { get; }

        public double Kc { get; }
        #endregion

        #region Constructors

        public LocalEgyptMomentDto(double ae, double be, double ce, double ze, double fy, double kw, double kf,double kc)
        {
            Ae = ae;
            Be = be;
            Ce = ce;
            Ze = ze;
            Fy = fy;
            Kw = kw;
            Kf = kf;
            Kc = kc;
        }

        #endregion
    }
}
