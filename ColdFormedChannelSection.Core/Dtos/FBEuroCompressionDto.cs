namespace ColdFormedChannelSection.Core.Dtos
{
    public class FBEuroCompressionDto:NominalStrengthDto
    {

        #region Properties

        public double A { get; }

        public double X { get; }

        public double Fy { get;}

        #endregion

        #region Constructors

        public FBEuroCompressionDto(double a, double x, double fy,double pn)
            :base(pn,Enums.FailureMode.FLEXURALBUCKLING)
        {
            A = a;
            X = x;
            Fy = fy;
        }

        #endregion
    }
}
