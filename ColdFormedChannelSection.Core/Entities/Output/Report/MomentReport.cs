using ColdFormedChannelSection.Core.Enums;
using ColdFormedChannelSection.Core.Helpers;
using System.Collections.Generic;

namespace ColdFormedChannelSection.Core.Entities
{
    public class MomentReport : ReportBase
    {

        #region Properties

        #endregion

        #region Constructors

        public MomentReport(string title, string item1Name, List<ReportItem> item1List, string item2Name, List<ReportItem> item2List, List<ReportItem> designList,UnitSystems unitSystem)
           : base(title, item1Name, item1List, item2Name, item2List, "Design Moment", designList,unitSystem)
        {

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
