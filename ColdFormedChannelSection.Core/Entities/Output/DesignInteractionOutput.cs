namespace ColdFormedChannelSection.Core.Entities
{
    public class DesignInteractionOutput:CheckInteractionOutput
    {
        #region Properties

        public string SectionName { get; }

        #endregion

        #region Constructors

        public DesignInteractionOutput(double pu, double pn, double mu, double mn, string iE, double iEValue,string sectionName,string momentUnitName, string forceUnitName,IReport report)
            :base(pu, pn, mu, mn, iE, iEValue,momentUnitName,forceUnitName,report)
        {
           SectionName = sectionName;
        }

        #endregion
    }
}
