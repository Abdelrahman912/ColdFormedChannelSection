using ColdFormedChannelSection.Core.Entities;

namespace ColdFormedChannelSection.Core.Helpers
{
    public static class OutputHelper
    {
        public static CompressionCheckOutput AsCheck(this CompressionResistanceOutput resist , double ultimateLoad)
        {
            return new CompressionCheckOutput(
                ultimateLoad: ultimateLoad,
                nominalResistance: resist.NominalResistance,
                phi: resist.Phi,
                governingCase: resist.GoverningCase,
                unitName: resist.UnitName
                );
        }
        public static MomentCheckOutput AsCheck(this MomentResistanceOutput resist, double ultimateLoad)
        {
            return new MomentCheckOutput(
                ultimateLoad: ultimateLoad,
                nominalResistance: resist.NominalResistance,
                phi: resist.Phi,
                governingCase: resist.GoverningCase,
                unitName: resist.UnitName
                );
        }
    } 
}
