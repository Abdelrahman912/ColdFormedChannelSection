using ColdFormedChannelSection.Core.Entities;
using ColdFormedChannelSection.Core.Enums;
using System;
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


        private static readonly Dictionary<KeyValuePair<Units, Units>, Tuple<double, string>> _momentUnitFactors = new Dictionary<KeyValuePair<Units, Units>, Tuple<double, string>>()
        {
            {KeyValuePair.Create(Units.KIPINCH,Units.KIPINCH),Tuple.Create( 1.0 ,"Kip.in")},
            {KeyValuePair.Create(Units.KIPINCH,Units.NMM), Tuple.Create(112900.0, "N.mm") },
            {KeyValuePair.Create(Units.KIPINCH,Units.TONCM), Tuple.Create(1.152124, "t.cm") },

            {KeyValuePair.Create(Units.TONCM,Units.TONCM),Tuple.Create( 1.0,"t.cm") },
            {KeyValuePair.Create(Units.TONCM,Units.KIPINCH),Tuple.Create( 1/1.152124,"Kip.in")},
            {KeyValuePair.Create(Units.TONCM,Units.NMM),Tuple.Create( 98066.5,"N.mm") },

            {KeyValuePair.Create(Units.NMM,Units.NMM),Tuple.Create( 1.0,"N.mm") },
            {KeyValuePair.Create(Units.NMM,Units.KIPINCH),Tuple.Create( 1/112900.0,"Kip.in")},
            {KeyValuePair.Create(Units.NMM,Units.TONCM),Tuple.Create(1/98066.5,"t.cm")},
        };


        private static readonly Dictionary<KeyValuePair<Units, Units>, Tuple<double, string>> _forceUnitFactors = new Dictionary<KeyValuePair<Units, Units>, Tuple<double, string>>()
        {
            {KeyValuePair.Create(Units.KIPINCH,Units.KIPINCH),Tuple.Create( 1.0,"Kip") },
            {KeyValuePair.Create(Units.KIPINCH,Units.NMM),Tuple.Create( 4444.44,"N") },
            {KeyValuePair.Create(Units.KIPINCH,Units.TONCM),Tuple.Create(1/2.2046,"ton") },

            {KeyValuePair.Create(Units.TONCM,Units.TONCM),Tuple.Create( 1.0,"ton") },
            {KeyValuePair.Create(Units.TONCM,Units.KIPINCH), Tuple.Create(2.2046, "Kip")},
            {KeyValuePair.Create(Units.TONCM,Units.NMM), Tuple.Create(9806.65, "N") },

            {KeyValuePair.Create(Units.NMM,Units.NMM), Tuple.Create(1.0, "N") },
            {KeyValuePair.Create(Units.NMM,Units.KIPINCH), Tuple.Create(1 / 4444.44, "Kip")},
            {KeyValuePair.Create(Units.NMM,Units.TONCM), Tuple.Create(1 / 9806.65, "ton")},
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

        private static Tuple<double, string> ConvertMoment(this double moment, Units sourceUnit, Units targetUnits)
        {
            var key = KeyValuePair.Create(sourceUnit, targetUnits);
            (var factor, var unit) = _momentUnitFactors[key];
            var newMoment = moment * factor;
            return Tuple.Create(newMoment, unit);
        }

        private static Tuple<double, string> ConvertForce(this double force, Units sourceUnit, Units targetUnits)
        {
            var key = KeyValuePair.Create(sourceUnit, targetUnits);
            (var factor, var unit) = _forceUnitFactors[key];
            var newForce = force * factor;
            return Tuple.Create(newForce, unit);
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
            var Cm = bracingConditions.Cm;
            var newBracingConditions = new LengthBracingConditions(Lx, Ly, Lz, Kx, Ky, Kz, Lu, Cb, C1,Cm);
            return newBracingConditions;
        }


        public static SectionDimension Convert(this SectionDimension section, Units sourceUnit, Units targetUnit)
        {
            var H = section.TotalHeightH.ConvertLength(sourceUnit, targetUnit);
            var B = section.TotalFlangeWidthB.ConvertLength(sourceUnit, targetUnit);
            var C = section.TotalFoldWidthC.ConvertLength(sourceUnit, targetUnit);
            var R = section.InternalRadiusR.ConvertLength(sourceUnit, targetUnit);
            var t = section.ThicknessT.ConvertLength(sourceUnit, targetUnit);
            var newSection = new SectionDimension(H, B, R, t, C);
            return newSection;
        }

        public static MomentResistanceOutput Convert(this MomentResistanceOutput output, Units sourceUnit, Units targetUnit)
        {
           ( var nominalResistance , var unit) = output.NominalResistance.ConvertMoment(sourceUnit, targetUnit);
            var phi = output.Phi;
            var failureMode = output.GoverningCase;
            var newOutput = new MomentResistanceOutput(nominalResistance, phi, failureMode ,unit);
            return newOutput;
        }

        public static CompressionResistanceOutput Convert(this CompressionResistanceOutput output, Units sourceUnit, Units targetUnit)
        {
            (var nominalResistance , var unit) = output.NominalResistance.ConvertForce(sourceUnit, targetUnit);
            var phi = output.Phi;
            var failureMode = output.GoverningCase;
            var newOutput = new CompressionResistanceOutput(nominalResistance, phi, failureMode ,unit);
            return newOutput;
        }


    }
}
