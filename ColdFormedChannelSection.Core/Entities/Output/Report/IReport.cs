using ColdFormedChannelSection.Core.Enums;

namespace ColdFormedChannelSection.Core.Entities
{
    public interface IReport
    {
        public UnitSystems UnitSystem { get;  }
        IReport Convert( UnitSystems target);
    }
}
