namespace ColdFormedChannelSection.Core.Entities
{
    public class LengthBracingConditions
    {
        
        #region Properties

        public double Lx { get;  }
        public double Ly { get; }
        public double Lz { get; }
        public double Kx { get; }
        public double Ky { get; }
        public double Kz { get; }
        public double Lu { get; }
        public double Cb { get; }
        public double C1 { get; }
        public double Cm { get; }

        #endregion

        #region Constructors

        public LengthBracingConditions(double lx, double ly, double lz, double kx, double ky, double kz, double lu, double cb, double c1, double cm)
        {
            Lx = lx;
            Ly = ly;
            Lz = lz;
            Kx = kx;
            Ky = ky;
            Kz = kz;
            Lu = lu;
            Cb = cb;
            C1 = c1;
            Cm = cm;
        }

        #endregion
    }
}
