namespace ColdFormedChannelSection.Core.Entities
{
    public class CSectionProperties : SectionProperties
    {
        public CSectionProperties(double aPrime, double bPrime, double cPrime, double a, double _Ix, double zg, double _Iy, double rx, double ry, double xo, double j, double cw, double cSmall, double rSmall, double u, double b_small, int alpha, double a_small) 
            : base(aPrime, bPrime, cPrime, a, _Ix, zg, _Iy, rx, ry, xo, j, cw, cSmall, rSmall, u, b_small, alpha, a_small)
        {
        }
    }
}
