using ColdFormedChannelSection.Core.Enums;
using iText.Layout;

namespace ColdFormedChannelSection.Core.Entities
{
    public interface IReportSection
    {
        public void AddToPdf(Document doc);
        public string SectionName { get; }
        IReportSection Convert(UnitSystems source,UnitSystems target);
        IReportSection AppendToName(string saName);
    }
}
