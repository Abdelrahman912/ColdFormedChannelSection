namespace ColdFormedChannelSection.Core.Dtos
{
    public class TBEgyptCompressionDto:NominalStrengthDto
    {
        #region Properties

        public double F { get; }

        public double A { get; }

        #endregion

        #region Constructors

        public TBEgyptCompressionDto(double f, double a,double pn)
            :base(pn,Enums.FailureMode.TORSIONALBUCKLING)
        {
            F = f;
            A = a;
        }

        #endregion

    }
}
