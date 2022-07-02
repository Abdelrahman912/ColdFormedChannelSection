namespace ColdFormedChannelSection.Core.Dtos
{
    public class TFBEgyptCompressionDto:NominalStrengthDto
    {
        #region Properties

        public double F { get; }

        public double A { get;}

        #endregion

        #region Constructors

        public TFBEgyptCompressionDto(double f, double a,double pn)
            :base(pn,Enums.FailureMode.FLEXURAL_TORSIONAL_BUCKLING)
        {
            F = f;
            A = a;
        }

        #endregion

    }
}
