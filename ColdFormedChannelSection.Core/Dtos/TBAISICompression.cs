namespace ColdFormedChannelSection.Core.Dtos
{
    public class TBAISICompression:NominalStrengthDto
    {
      
        #region Properties

        public double F { get; }

        public double A { get;}

        #endregion

        #region Constructors

        public TBAISICompression(double f, double a,double pn)
            :base(pn,Enums.FailureMode.TORSIONALBUCKLING)
        {
            F = f;
            A = a;
        }

        #endregion

    }
}
