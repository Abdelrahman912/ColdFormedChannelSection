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

        #endregion

        #region Constructors

        public LocalEgyptMomentDto(double ae, double be, double ce, double ze, double fy)
        {
            Ae = ae;
            Be = be;
            Ce = ce;
            Ze = ze;
            Fy = fy;
        }

        #endregion
    }
}
