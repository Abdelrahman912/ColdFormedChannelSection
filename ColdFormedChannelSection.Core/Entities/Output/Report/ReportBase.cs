using System.Collections.Generic;

namespace ColdFormedChannelSection.Core.Entities
{
    public abstract class ReportBase:IReport
    {
       
        #region Properties

        public string Title { get; }

        public string Item1Name { get; }

        public List<ReportItem> Item1List { get; }

        public string Item2Name { get; }

        public List<ReportItem> Item2List { get; }

        public string DesignName { get; }

        public List<ReportItem> DesignList { get; }

        #endregion

        #region Constructors

        protected ReportBase(string title, string item1Name, List<ReportItem> item1List, string item2Name, List<ReportItem> item2List, string designName, List<ReportItem> designList)
        {
            Title = title;
            Item1Name = item1Name;
            Item1List = item1List;
            Item2Name = item2Name;
            Item2List = item2List;
            DesignName = designName;
            DesignList = designList;
        }

        #endregion



    }
}
