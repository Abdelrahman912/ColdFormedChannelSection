using ColdFormedChannelSection.Core.Enums;

namespace ColdFormedChannelSection.Core.Entities
{
    public interface IReportSection
    {
        public string SectionName { get; }
        IReportSection Convert(UnitSystems source,UnitSystems target);
        IReportSection AppendToName(string saName);
    }
}
