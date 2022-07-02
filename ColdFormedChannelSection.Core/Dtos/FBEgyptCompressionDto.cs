namespace ColdFormedChannelSection.Core.Dtos
{
    public class FBEgyptCompressionDto:NominalStrengthDto
    {
       
        #region Properties

        public double F { get; }

        public double A { get;}

        #endregion

        #region Constructors

        public FBEgyptCompressionDto(double f, double a,double pn)
            :base(pn,Enums.FailureMode.FLEXURALBUCKLING)
        {
            F = f;
            A = a;
        }

        #endregion

    }
}
