using ColdFormedChannelSection.Core.Entities;
using ColdFormedChannelSection.Core.Enums;
using System.Collections.Generic;

namespace ColdFormedChannelSection.Core.Helpers
{
    public static class UnitConversionHelper
    {

        private static readonly Dictionary<KeyValuePair<Units, Units>, double> _lengthUnitFactors = new Dictionary<KeyValuePair<Units, Units>, double>()
        {
            {KeyValuePair.Create(Units.NMM,Units.NMM),1.0 },
            {KeyValuePair.Create(Units.TONCM,Units.NMM),10.0 },
            {KeyValuePair.Create(Units.KIPINCH,Units.NMM),25.4 },

            {KeyValuePair.Create(Units.KIPINCH,Units.KIPINCH),1.0 },
            {KeyValuePair.Create(Units.TONCM,Units.KIPINCH),1/2.54 },
            {KeyValuePair.Create(Units.NMM,Units.KIPINCH),1/25.4 },

            {KeyValuePair.Create(Units.TONCM,Units.TONCM),1.0 },
            {KeyValuePair.Create(Units.NMM,Units.TONCM), 1/10.0},
            {KeyValuePair.Create(Units.KIPINCH,Units.TONCM),2.54 },
        };


        private static readonly Dictionary<KeyValuePair<Units, Units>, double> _stressUnitFactors = new Dictionary<KeyValuePair<Units, Units>, double>()
        {
            {KeyValuePair.Create(Units.NMM,Units.NMM),1.0 },
            {KeyValuePair.Create(Units.TONCM,Units.NMM),98.07 },
            {KeyValuePair.Create(Units.KIPINCH,Units.NMM),6.889 },

            {KeyValuePair.Create(Units.KIPINCH,Units.KIPINCH),1.0 },
            {KeyValuePair.Create(Units.TONCM,Units.KIPINCH),14.223 },
            {KeyValuePair.Create(Units.NMM,Units.KIPINCH),1/6.889 },

            {KeyValuePair.Create(Units.TONCM,Units.TONCM),1.0 },
            {KeyValuePair.Create(Units.NMM,Units.TONCM), 1/98.07},
            {KeyValuePair.Create(Units.KIPINCH,Units.TONCM),1/14.223 },
        };

        private static double ConvertLength(this double length, Units sourceUnit, Units targetUnit)
        {
            var key = KeyValuePair.Create(sourceUnit, targetUnit);
            var factor = _lengthUnitFactors[key];
            var newLength = length * factor;
            return newLength;
        }


        private static double ConvertStress(this double stress, Units sourceUnit, Units targetUnit)
        {
            var key = KeyValuePair.Create(sourceUnit, targetUnit);
            var factor = _stressUnitFactors[key];
            var newStress = stress * factor;
            return newStress;
        }

        public static Material Convert(this Material material, Units sourceUnit, Units targetUnits)
        {
            var Fy = material.Fy.ConvertStress(sourceUnit, targetUnits);
            var E = material.E.ConvertStress(sourceUnit, targetUnits);
            var newMaterial = new Material(Fy, E, material.V);
            return newMaterial;
        }

        public static LengthBracingConditions Convert(this LengthBracingConditions bracingConditions, Units sourceUnit, Units targetUnit)
        {
            var Lx = bracingConditions.Lx.ConvertLength(sourceUnit, targetUnit);
            var Ly = bracingConditions.Ly.ConvertLength(sourceUnit, targetUnit);
            var Lz = bracingConditions.Lz.ConvertLength(sourceUnit, targetUnit);
            var Lu = bracingConditions.Lu.ConvertLength(sourceUnit, targetUnit);
            var Kx = bracingConditions.Kx;
            var Ky = bracingConditions.Ky;
            var Kz = bracingConditions.Kz;
            var C1 = bracingConditions.C1;
            var Cb = bracingConditions.Cb;
            var newBracingConditions = new LengthBracingConditions(Lx, Ly, Lz, Kx, Ky, Kz, Lu, Cb, C1);
            return newBracingConditions;
        }


        public static SectionDimension Convert(this SectionDimension section, Units sourceUnit, Units targetUnit)
        {
            var H = section.TotalHeightH.ConvertLength(sourceUnit, targetUnit);
            var B = section.TotalFlangeWidthB.ConvertLength(sourceUnit, targetUnit);
            var C = section.TotalFoldWidthC.ConvertLength(sourceUnit, targetUnit);
            var R = section.InternalRadiusR.ConvertLength(sourceUnit, targetUnit);
            var t = section.ThicknessT.ConvertLength(sourceUnit,targetUnit);
            var newSection = new SectionDimension(H, B, R, t, C);
            return newSection;
        }

    }
}
