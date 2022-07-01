using ColdFormedChannelSection.Core.Enums;

namespace ColdFormedChannelSection.Core.Dtos
{
    public class NominalStrengthDto
    {
       

        #region Properties

        public double NominalStrength { get;}

        public FailureMode FailureMode { get;  }

        #endregion

        #region Constructors

        public NominalStrengthDto(double nominalStrength, FailureMode failureMode)
        {
            NominalStrength = nominalStrength;
            FailureMode = failureMode;
        }

        #endregion
    }
}
