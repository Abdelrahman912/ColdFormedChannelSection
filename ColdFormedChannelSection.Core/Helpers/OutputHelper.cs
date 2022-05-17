﻿using ColdFormedChannelSection.Core.Entities;
using ColdFormedChannelSection.Core.Enums;

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

        public static CompressionDesignOutput AsDesign(this CompressionResistanceOutput resist, double ultimateLoad,string sectionName)
        {
            return new CompressionDesignOutput(
                ultimateLoad: ultimateLoad,
                designSection:sectionName,
                nominalResistance: resist.NominalResistance,
                phi: resist.Phi,
                governingCase: resist.GoverningCase,
                unitName: resist.UnitName
                );
        }
        public static MomentDesignOutput AsDesign(this MomentResistanceOutput resist, double ultimateLoad,string sectionName)
        {
            return new MomentDesignOutput(
                ultimateLoad: ultimateLoad,
                designSection:sectionName,
                nominalResistance: resist.NominalResistance,
                phi: resist.Phi,
                governingCase: resist.GoverningCase,
                unitName: resist.UnitName
                );
        }


    } 
}