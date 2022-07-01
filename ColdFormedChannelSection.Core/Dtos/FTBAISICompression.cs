using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ColdFormedChannelSection.Core.Enums;

namespace ColdFormedChannelSection.Core.Dtos
{
    public class FTBAISICompression:NominalStrengthDto
    {
       

        #region Properties

        public double F { get; }

        public double A { get; }

        #endregion

        #region Constructors

        public FTBAISICompression(double f, double a,double pn)
            :base(pn,FailureMode.FLEXURAL_TORSIONAL_BUCKLING)
        {
            F = f;
            A = a;
        }

        #endregion

    }
}
