using ColdFormedChannelSection.Core.Entities;
using ColdFormedChannelSection.Core.Enums;

namespace ColdFormedChannelSection.Core.Helpers
{
    public static class OutputHelper
    {
        public static CheckInteractionOutput AsCheck(this ResistanceInteractionOutput resist,Units momentUnit , Units forceUnit)
        {
            return new CheckInteractionOutput(
                pu: resist.Pu,
                pn: resist.Pn,
                mu: resist.Mu,
                mn: resist.Mn,
                iE:resist.IE,
                iEValue:resist.IEValue,
                momentUnit: momentUnit,
                forceUnit: forceUnit,
                resist.Report
                );
        }

        public static DesignInteractionOutput AsDesign(this ResistanceInteractionOutput resist,string sectionName, Units momentUnit, Units forceUnit)
        {
            return new DesignInteractionOutput(
                pu: resist.Pu,
                pn: resist.Pn,
                mu: resist.Mu,
                mn: resist.Mn,
                iE: resist.IE,
                iEValue: resist.IEValue,
                sectionName: sectionName,
                momentUnit: momentUnit,
                forceUnit: forceUnit,
                resist.Report
                );
        }

        public static CompressionCheckOutput AsCheck(this CompressionResistanceOutput resist , double ultimateLoad,Units unit)
        {
            return new CompressionCheckOutput(
                ultimateLoad: ultimateLoad,
                nominalResistance: resist.NominalResistance,
                phi: resist.Phi,
                governingCase: resist.GoverningCase,
                unit: unit,
                report: resist.Report
                );
        }
        public static MomentCheckOutput AsCheck(this MomentResistanceOutput resist, double ultimateLoad, Units unit)
        {
            return new MomentCheckOutput(
                ultimateLoad: ultimateLoad,
                nominalResistance: resist.NominalResistance,
                phi: resist.Phi,
                governingCase: resist.GoverningCase,
                unit: unit,
                report: resist.Report
                );
        }

        public static CompressionDesignOutput AsDesign(this CompressionResistanceOutput resist, double ultimateLoad,string sectionName, Units unit)
        {
            return new CompressionDesignOutput(
                ultimateLoad: ultimateLoad,
                designSection:sectionName,
                nominalResistance: resist.NominalResistance,
                phi: resist.Phi,
                governingCase: resist.GoverningCase,
                unit: unit,
                report:resist.Report
                );
        }
        public static MomentDesignOutput AsDesign(this MomentResistanceOutput resist, double ultimateLoad,string sectionName,Units unit)
        {
            return new MomentDesignOutput(
                ultimateLoad: ultimateLoad,
                designSection:sectionName,
                nominalResistance: resist.NominalResistance,
                phi: resist.Phi,
                governingCase: resist.GoverningCase,
                unit: unit,
                report:resist.Report
                );
        }


    } 
}
