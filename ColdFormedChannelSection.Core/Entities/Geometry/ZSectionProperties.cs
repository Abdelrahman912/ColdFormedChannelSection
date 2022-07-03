namespace ColdFormedChannelSection.Core.Entities
{
    public class ZSectionProperties : SectionProperties
    {

        #region Properties

        public double IxPrincipal { get; }
        public double IyPrincipal { get; }
        public double Ixy { get; }
        public double RxGeom { get;  }
        public double RyGeom { get;}
        public double ThetaPrime { get;}

        #endregion

        #region Constructors

        public ZSectionProperties(double aPrime, double bPrime, double cPrime, double a, double _Ix, double zg, double _Iy, double rx, double ry, double xo, double j, double cw, double cSmall, double rSmall, double u, double b_small, int alpha, double a_small, double ixPrincipal, double iyPrincipal, double ixy, double rxGeom, double ryGeom, double thetaPrime)
           : base(aPrime, bPrime, cPrime, a, _Ix, zg, _Iy, rx, ry, xo, j, cw, cSmall, rSmall, u, b_small, alpha, a_small)
        {
            IxPrincipal = ixPrincipal;
            IyPrincipal = iyPrincipal;
            Ixy = ixy;
            RxGeom = rxGeom;
            RyGeom = ryGeom;
            ThetaPrime = thetaPrime;
        }

        #endregion


    }
}
