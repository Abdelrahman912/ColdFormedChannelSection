using ColdFormedChannelSection.Core.Enums;
using ColdFormedChannelSection.Core.Helpers;
using System.Collections.Generic;

namespace ColdFormedChannelSection.Core.Entities
{
    public class ListReportSection : IReportSection
    {

        #region Properties

        public bool IsConvertable { get;  }

        public string SectionName { get; }

        public List<ReportItem> Items { get; set; }

        #endregion

        #region Constructors

        public ListReportSection(string sectionName,List<ReportItem> items, bool isConvertable=true)
        {
            SectionName = sectionName;
            Items = items;
            IsConvertable = isConvertable;
        }

        #endregion

        #region Methods

        public IReportSection Convert(UnitSystems source, UnitSystems target)
        {
            if (!IsConvertable)
                return this;
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
