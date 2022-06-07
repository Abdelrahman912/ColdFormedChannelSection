namespace ColdFormedChannelSection.Core.Entities
{
    public abstract class OutputBase
    {
        

        #region Properties

        public IReport Report { get; set; }

        #endregion

        #region Constructors

        protected OutputBase(IReport report)
        {
            Report = report;
        }

        #endregion


    }

}
