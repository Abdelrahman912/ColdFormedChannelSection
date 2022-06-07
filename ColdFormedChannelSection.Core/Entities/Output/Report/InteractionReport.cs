namespace ColdFormedChannelSection.Core.Entities
{
    public class InteractionReport:IReport
    {
       
        #region Properties

        public IReport CompressionReport { get;}

        public IReport MomentReport { get; }

        #endregion

        #region Constructors

        public InteractionReport(IReport compressionReport, IReport momentReport)
        {
            CompressionReport = compressionReport;
            MomentReport = momentReport;
        }

        #endregion
    }
}
