using ColdFormedChannelSection.Core.Enums;
using System.Collections.Generic;

namespace ColdFormedChannelSection.Core.Entities
{
    public interface IReport
    {
        public List<IReportSection> Sections { get; }
        public UnitSystems UnitSystem { get;  }
        IReport Convert( UnitSystems target);
    }
}
