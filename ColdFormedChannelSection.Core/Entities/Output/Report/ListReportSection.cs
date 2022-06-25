using ColdFormedChannelSection.Core.Enums;
using ColdFormedChannelSection.Core.Helpers;
using System.Collections.Generic;

namespace ColdFormedChannelSection.Core.Entities
{
    public class ListReportSection : IReportSection
    {
       
        #region Properties

        public string SectionName { get; }

        public List<ReportItem> Items { get; set; }

        #endregion

        #region Constructors

        public ListReportSection(string sectionName,List<ReportItem> items)
        {
            SectionName = sectionName;
            Items = items;
        }

        #endregion

        #region Methods

        public IReportSection Convert(UnitSystems source, UnitSystems target)
        {
            var newItems = Items.Convert(source,target);
            return new ListReportSection(SectionName,newItems);
        }

        public IReportSection AppendToName(string saName)
        {
            return new ListReportSection($"{SectionName} - {saName}",Items);
        }

        #endregion


    }
}
