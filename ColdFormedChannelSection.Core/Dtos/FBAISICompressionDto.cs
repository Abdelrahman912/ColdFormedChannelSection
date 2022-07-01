using ColdFormedChannelSection.Core.Enums;

namespace ColdFormedChannelSection.Core.Dtos
{
    public class FBAISICompressionDto:NominalStrengthDto
    {
      
        #region Properties

        public double F { get; }

        public double A { get; }

        #endregion

        #region Constructors

        public FBAISICompressionDto(double f, double a,double pn)
            :base(pn,FailureMode.FLEXURALBUCKLING)
        {
            F = f;
            A = a;
        }

        #endregion

    }
}
