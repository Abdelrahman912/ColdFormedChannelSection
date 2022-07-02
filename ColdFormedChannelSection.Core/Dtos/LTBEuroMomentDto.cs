using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ColdFormedChannelSection.Core.Dtos
{
    public class LTBEuroMomentDto:NominalStrengthDto
    {
        #region Properties

        public double Z { get;  }

        public double F { get; }

        public double X { get; }

        #endregion

        #region Constructors

        public LTBEuroMomentDto(double z, double f, double x,double mn)
            :base(mn,Enums.FailureMode.LATERALTORSIONALBUCKLING)
        {
            Z = z;
            F = f;
            X = x;
        }

        #endregion
    }
}
