using ColdFormedChannelSection.Core.Enums;

namespace ColdFormedChannelSection.Core.Dtos
{
    public class LTBAISIMomentDto:NominalStrengthDto
    {
        
        #region Properties

        public double Zf { get; }

        public double F { get;  }


        #endregion

        #region Constructors

        public LTBAISIMomentDto(double zf, double f, double mn,FailureMode failureMode)
            :base(mn,failureMode)
        {
            Zf = zf;
            F = f;
        }

        #endregion

    }
}
