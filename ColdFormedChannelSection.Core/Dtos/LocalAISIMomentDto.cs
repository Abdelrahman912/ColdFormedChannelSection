using ColdFormedChannelSection.Core.Enums;

namespace ColdFormedChannelSection.Core.Dtos
{
    public class LocalAISIMomentDto:NominalStrengthDto
    {

        #region Properties

        public double Kw { get; }

        public double Ae { get;}

        public double Kf { get; }

        public double Be { get;  }

        public double Kc { get; }

        public double Ce { get;  }

        public double Ze { get;  }

        public double Fy { get; }

        #endregion

        #region Constructors

        public LocalAISIMomentDto(double kw, double ae, double kf, double be, double kc, double ce, double ze,double fy,double mn)
            :base(mn,FailureMode.LOCALBUCKLING)
        {
            Kw = kw;
            Ae = ae;
            Kf = kf;
            Be = be;
            Kc = kc;
            Ce = ce;
            Ze = ze;
            Fy = fy;
        }

        #endregion

    }
}
