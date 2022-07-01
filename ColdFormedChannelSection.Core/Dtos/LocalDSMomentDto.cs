namespace ColdFormedChannelSection.Core.Dtos
{
    public class LocalDSMomentDto
    {

        #region Properties

        public double Mcrl { get; }

        public double Kw { get;}

        public double Kf { get;}

        public double Kc { get;}

        #endregion

        #region Constructors

        public LocalDSMomentDto(double kw, double kf, double kc, double mcrl)
        {
            Kw = kw;
            Kf = kf;
            Kc = kc;
            Mcrl = mcrl;
        }

        #endregion
    }
}
