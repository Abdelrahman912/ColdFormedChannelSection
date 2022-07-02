namespace ColdFormedChannelSection.Core.Dtos
{
    public class TBEuroCompressionDto:NominalStrengthDto
    {
       
        #region Properties

        public double X { get;}

        public double Fy { get;}

        public double A { get; }

        #endregion

        #region Constructors

        public TBEuroCompressionDto(double x, double fy, double a,double pn)
            :base(pn,Enums.FailureMode.TORSIONALBUCKLING)
        {
            X = x;
            Fy = fy;
            A = a;
        }

        #endregion
    }
}
