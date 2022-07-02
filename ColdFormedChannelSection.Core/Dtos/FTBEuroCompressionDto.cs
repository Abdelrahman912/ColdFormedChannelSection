namespace ColdFormedChannelSection.Core.Dtos
{
    public class FTBEuroCompressionDto:NominalStrengthDto
    {
      

        #region Properties

        public double X { get;  }

        public double Fy { get;  }

        public double A { get;}

        #endregion

        #region Constructors

        public FTBEuroCompressionDto(double x, double fy, double a,double pn)
            :base(pn,Enums.FailureMode.FLEXURAL_TORSIONAL_BUCKLING)
        {
            X = x;
            Fy = fy;
            A = a;
        }

        #endregion
    }
}
