namespace ColdFormedChannelSection.Core.Entities
{
    public class ResistanceInteractionOutput:IOutput
    {
        #region Properties

        public string MomentUnitName { get; }

        public string ForceUnitName { get; }


        public double Pu { get; }

        public double Pn { get; }

        public double Mu { get; }

        public double Mn { get; }

        public string IE { get; }

        public double IEValue { get; }

        #endregion

        #region Constructors

        public ResistanceInteractionOutput(double pu, double pn, double mu, double mn, string iE, double iEValue, string momentUnitName, string forceUnitName)
        {
            Pu = pu;
            Pn = pn;
            Mu = mu;
            Mn = mn;
            IE = iE;
            IEValue = iEValue;
            MomentUnitName = momentUnitName;
            ForceUnitName = forceUnitName;
        }

        #endregion
    }
}
