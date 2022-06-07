using System.Collections.Generic;

namespace ColdFormedChannelSection.Core.Entities
{
    public class MomentReport : ReportBase
    {

        #region Properties

        #endregion

        #region Constructors

        public MomentReport(string title, string item1Name, List<ReportItem> item1List, string item2Name, List<ReportItem> item2List, List<ReportItem> designList)
           : base(title, item1Name, item1List, item2Name, item2List, "Design Moment", designList)
        {

        }

        #endregion

    }
}
