namespace ColdFormedChannelSection.Core.Entities
{
    public class SectionProperties
    {
       
        #region Properties

        public double APrime { get;  }

        public double  BPrime { get; }

        public double CPrime { get; }

        public double A { get; }

        public double Ix { get; }

        public double Zg { get; }

        public double Iy { get; }

        /// <summary>
        /// Radius of gyration about x - axis.
        /// </summary>
        public double Rx { get;  }

        /// <summary>
        /// Radius of gyration about y - axis.
        /// </summary>
        public double Ry { get; }

        public double Xo { get; }

        public double J { get; }

        public double Cw { get;  }

        public double CSmall { get; }

        public double ADimension { get;}

        public double RSmall { get; }

        public double U { get; }

        #endregion

        #region Constructors

        public SectionProperties(double aPrime, double bPrime, double cPrime, double a, double _Ix, double zg, double _Iy, double rx, double ry, double xo, double j, double cw, double cSmall,double aDimension,double rSmall,double u)
        {
            APrime = aPrime;
            BPrime = bPrime;
            CPrime = cPrime;
            A = a;
            Ix = _Ix;
            Zg = zg;
            Iy = _Iy;
            Rx = rx;
            Ry = ry;
            Xo = xo;
            J = j;
            Cw = cw;
            CSmall = cSmall;
            ADimension = aDimension;
            RSmall = rSmall;
            U = u;
        }

        #endregion

    }
}
