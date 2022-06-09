using ColdFormedChannelSection.Core.Enums;
using ColdFormedChannelSection.Core.Helpers;

namespace ColdFormedChannelSection.Core.Entities
{
    public class InteractionReport:IReport
    {
       
        #region Properties

        public IReport CompressionReport { get;}

        public IReport MomentReport { get; }

        public UnitSystems UnitSystem { get; }

        #endregion

        #region Constructors

        public InteractionReport(IReport compressionReport, IReport momentReport, UnitSystems unitSystem)
        {
            CompressionReport = compressionReport;
            MomentReport = momentReport;
            UnitSystem = unitSystem;
        }

        public IReport Convert( UnitSystems target)
        {
            return UnitConversionHelper.Convert(this, UnitSystem, target);
        }

        #endregion
    }
}
