using ColdFormedChannelSection.Core.Enums;
using ColdFormedChannelSection.Core.Helpers;
using System.Collections.Generic;

namespace ColdFormedChannelSection.Core.Entities
{
    public class CompressionReport:ReportBase
    {

        #region Properties

       

        public string Item3Name { get; }

        public List<ReportItem> Item3List { get; }

       

        #endregion

        #region Constructors

        public CompressionReport(string title, string item1Name, List<ReportItem> item1List, string item2Name, List<ReportItem> item2List, string item3Name, List<ReportItem> item3List, List<ReportItem> designCompressionList,UnitSystems unitSystem)
            :base(title,item1Name,item1List,item2Name,item2List,"Design Compression Load",designCompressionList,unitSystem)
        {
            Item3Name = item3Name;
            Item3List = item3List;
        }



        #endregion

        #region Methods

        public override IReport Convert( UnitSystems target)
        {
            return UnitConversionHelper.Convert(this, UnitSystem, target);
        }

        #endregion

    }
}
