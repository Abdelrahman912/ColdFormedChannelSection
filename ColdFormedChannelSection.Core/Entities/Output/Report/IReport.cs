using ColdFormedChannelSection.Core.Enums;
using CSharp.Functional.Constructs;
using iText.Layout;
using System.Collections.Generic;
using Unit = System.ValueTuple;

namespace ColdFormedChannelSection.Core.Entities
{
    public interface IReport
    {
        public Exceptional<Unit> CreatePdf(string fileName);
        public string Name { get; }
        public List<IReportSection> Sections { get; }
        public UnitSystems UnitSystem { get;  }
        IReport Convert( UnitSystems target);
    }
}
