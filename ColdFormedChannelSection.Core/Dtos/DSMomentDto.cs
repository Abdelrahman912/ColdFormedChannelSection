namespace ColdFormedChannelSection.Core.Dtos
{
    public abstract class DSMomentDto
    {
        #region Properties

        public LocalDSMomentDto LB { get; }

        public double Mcre { get; }

        public double Mnl { get; }

        public double Mne { get; }

        public double Fy { get; }

        public double Zg { get; }

        public double My { get; }
        public NominalStrengthDto GoverningCase { get; }

        #endregion

        #region Constructors

        public DSMomentDto(LocalDSMomentDto lb, double mcre, double mnl, double mne, double fy, double zg, NominalStrengthDto governingCase)
        {
            LB = lb;
            Mcre = mcre;
            Mnl = mnl;
            Mne = mne;

            Fy = fy;
            Zg = zg;
            My = Fy * Zg;
            GoverningCase = governingCase; /*nominalLoads.Distinct(NominalStrengthEqualComparer).OrderBy(tup => tup.NominalStrength).First();*/
        }

        #endregion
    }
}
