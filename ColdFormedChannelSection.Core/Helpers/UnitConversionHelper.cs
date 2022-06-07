using ColdFormedChannelSection.Core.Entities;
using ColdFormedChannelSection.Core.Enums;
using System;
using System.Collections.Generic;

namespace ColdFormedChannelSection.Core.Helpers
{
    public static class UnitConversionHelper
    {

        private static readonly Dictionary<KeyValuePair<UnitSystems, UnitSystems>, double> _lengthUnitFactors = new Dictionary<KeyValuePair<UnitSystems, UnitSystems>, double>()
        {
            {KeyValuePair.Create(UnitSystems.NMM,UnitSystems.NMM),1.0 },
            {KeyValuePair.Create(UnitSystems.TONCM,UnitSystems.NMM),10.0 },
            {KeyValuePair.Create(UnitSystems.KIPINCH,UnitSystems.NMM),25.4 },

            {KeyValuePair.Create(UnitSystems.KIPINCH,UnitSystems.KIPINCH),1.0 },
            {KeyValuePair.Create(UnitSystems.TONCM,UnitSystems.KIPINCH),1/2.54 },
            {KeyValuePair.Create(UnitSystems.NMM,UnitSystems.KIPINCH),1/25.4 },

            {KeyValuePair.Create(UnitSystems.TONCM,UnitSystems.TONCM),1.0 },
            {KeyValuePair.Create(UnitSystems.NMM,UnitSystems.TONCM), 1/10.0},
            {KeyValuePair.Create(UnitSystems.KIPINCH,UnitSystems.TONCM),2.54 },
        };


        private static readonly Dictionary<KeyValuePair<UnitSystems, UnitSystems>, double> _stressUnitFactors = new Dictionary<KeyValuePair<UnitSystems, UnitSystems>, double>()
        {
            {KeyValuePair.Create(UnitSystems.NMM,UnitSystems.NMM),1.0 },
            {KeyValuePair.Create(UnitSystems.TONCM,UnitSystems.NMM),98.07 },
            {KeyValuePair.Create(UnitSystems.KIPINCH,UnitSystems.NMM),6.889 },

            {KeyValuePair.Create(UnitSystems.KIPINCH,UnitSystems.KIPINCH),1.0 },
            {KeyValuePair.Create(UnitSystems.TONCM,UnitSystems.KIPINCH),14.223 },
            {KeyValuePair.Create(UnitSystems.NMM,UnitSystems.KIPINCH),1/6.889 },

            {KeyValuePair.Create(UnitSystems.TONCM,UnitSystems.TONCM),1.0 },
            {KeyValuePair.Create(UnitSystems.NMM,UnitSystems.TONCM), 1/98.07},
            {KeyValuePair.Create(UnitSystems.KIPINCH,UnitSystems.TONCM),1/14.223 },
        };


        private static readonly Dictionary<KeyValuePair<UnitSystems, UnitSystems>, Tuple<double, string>> _momentUnitFactors = new Dictionary<KeyValuePair<UnitSystems, UnitSystems>, Tuple<double, string>>()
        {
            {KeyValuePair.Create(UnitSystems.KIPINCH,UnitSystems.KIPINCH),Tuple.Create( 1.0 ,"Kip.in")},
            {KeyValuePair.Create(UnitSystems.KIPINCH,UnitSystems.NMM), Tuple.Create(112900.0, "N.mm") },
            {KeyValuePair.Create(UnitSystems.KIPINCH,UnitSystems.TONCM), Tuple.Create(1.152124, "t.cm") },

            {KeyValuePair.Create(UnitSystems.TONCM,UnitSystems.TONCM),Tuple.Create( 1.0,"t.cm") },
            {KeyValuePair.Create(UnitSystems.TONCM,UnitSystems.KIPINCH),Tuple.Create( 1/1.152124,"Kip.in")},
            {KeyValuePair.Create(UnitSystems.TONCM,UnitSystems.NMM),Tuple.Create( 98066.5,"N.mm") },

            {KeyValuePair.Create(UnitSystems.NMM,UnitSystems.NMM),Tuple.Create( 1.0,"N.mm") },
            {KeyValuePair.Create(UnitSystems.NMM,UnitSystems.KIPINCH),Tuple.Create( 1/112900.0,"Kip.in")},
            {KeyValuePair.Create(UnitSystems.NMM,UnitSystems.TONCM),Tuple.Create(1/98066.5,"t.cm")},
        };


        private static readonly Dictionary<KeyValuePair<UnitSystems, UnitSystems>, Tuple<double, string>> _forceUnitFactors = new Dictionary<KeyValuePair<UnitSystems, UnitSystems>, Tuple<double, string>>()
        {
            {KeyValuePair.Create(UnitSystems.KIPINCH,UnitSystems.KIPINCH),Tuple.Create( 1.0,"Kip") },
            {KeyValuePair.Create(UnitSystems.KIPINCH,UnitSystems.NMM),Tuple.Create( 4444.44,"N") },
            {KeyValuePair.Create(UnitSystems.KIPINCH,UnitSystems.TONCM),Tuple.Create(1/2.2046,"ton") },

            {KeyValuePair.Create(UnitSystems.TONCM,UnitSystems.TONCM),Tuple.Create( 1.0,"ton") },
            {KeyValuePair.Create(UnitSystems.TONCM,UnitSystems.KIPINCH), Tuple.Create(2.2046, "Kip")},
            {KeyValuePair.Create(UnitSystems.TONCM,UnitSystems.NMM), Tuple.Create(9806.65, "N") },

            {KeyValuePair.Create(UnitSystems.NMM,UnitSystems.NMM), Tuple.Create(1.0, "N") },
            {KeyValuePair.Create(UnitSystems.NMM,UnitSystems.KIPINCH), Tuple.Create(1 / 4444.44, "Kip")},
            {KeyValuePair.Create(UnitSystems.NMM,UnitSystems.TONCM), Tuple.Create(1 / 9806.65, "ton")},
        };

        private static double ConvertLength(this double length, UnitSystems sourceUnit, UnitSystems targetUnit)
        {
            var key = KeyValuePair.Create(sourceUnit, targetUnit);
            var factor = _lengthUnitFactors[key];
            var newLength = length * factor;
            return newLength;
        }


        private static double ConvertStress(this double stress, UnitSystems sourceUnit, UnitSystems targetUnit)
        {
            var key = KeyValuePair.Create(sourceUnit, targetUnit);
            var factor = _stressUnitFactors[key];
            var newStress = stress * factor;
            return newStress;
        }

        public   static Tuple<double, string> ConvertMoment(this double moment, UnitSystems sourceUnit, UnitSystems targetUnits)
        {
            var key = KeyValuePair.Create(sourceUnit, targetUnits);
            (var factor, var unit) = _momentUnitFactors[key];
            var newMoment = moment * factor;
            return Tuple.Create(newMoment, unit);
        }

        public static Tuple<double, string> ConvertForce(this double force, UnitSystems sourceUnit, UnitSystems targetUnits)
        {
            var key = KeyValuePair.Create(sourceUnit, targetUnits);
            (var factor, var unit) = _forceUnitFactors[key];
            var newForce = force * factor;
            return Tuple.Create(newForce, unit);
        }

        

        public static Material Convert(this Material material, UnitSystems sourceUnit, UnitSystems targetUnits)
        {
            var Fy = material.Fy.ConvertStress(sourceUnit, targetUnits);
            var E = material.E.ConvertStress(sourceUnit, targetUnits);
            var newMaterial = new Material(Fy, E, material.V);
            return newMaterial;
        }

        public static LengthBracingConditions Convert(this LengthBracingConditions bracingConditions, UnitSystems sourceUnit, UnitSystems targetUnit)
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


        public static SectionDimension Convert(this SectionDimension section, UnitSystems sourceUnit, UnitSystems targetUnit)
        {
            var H = section.TotalHeightH.ConvertLength(sourceUnit, targetUnit);
            var B = section.TotalFlangeWidthB.ConvertLength(sourceUnit, targetUnit);
            var C = section.TotalFoldWidthC.ConvertLength(sourceUnit, targetUnit);
            var R = section.InternalRadiusR.ConvertLength(sourceUnit, targetUnit);
            var t = section.ThicknessT.ConvertLength(sourceUnit, targetUnit);
            var newSection = new SectionDimension(H, B, R, t, C);
            return newSection;
        }

        public static ResistanceInteractionOutput Convert(this ResistanceInteractionOutput output, UnitSystems sourceUnit, UnitSystems targetUnit)
        {
            (var mu, var mu_unit) = output.Mu.ConvertMoment(sourceUnit, targetUnit);
            (var mn, var mn_unit) = output.Mn.ConvertMoment(sourceUnit, targetUnit);

            (var pu, var pu_unit) = output.Pu.ConvertForce(sourceUnit, targetUnit);
            (var pn, var pn_unit) = output.Pn.ConvertForce(sourceUnit, targetUnit);

            var newOutput = new ResistanceInteractionOutput(pu,pn,mu,mn,output.IE,output.IEValue,mu_unit,pu_unit);
            return newOutput;
        }

        public static MomentResistanceOutput Convert(this MomentResistanceOutput output, UnitSystems sourceUnit, UnitSystems targetUnit)
        {
           ( var nominalResistance , var unit) = output.NominalResistance.ConvertMoment(sourceUnit, targetUnit);
            var phi = output.Phi;
            var failureMode = output.GoverningCase;
            var newOutput = new MomentResistanceOutput(nominalResistance, phi, failureMode ,unit);
            return newOutput;
        }

        public static CompressionResistanceOutput Convert(this CompressionResistanceOutput output, UnitSystems sourceUnit, UnitSystems targetUnit)
        {
            (var nominalResistance , var unit) = output.NominalResistance.ConvertForce(sourceUnit, targetUnit);
            var phi = output.Phi;
            var failureMode = output.GoverningCase;
            var newOutput = new CompressionResistanceOutput(nominalResistance, phi, failureMode ,unit);
            return newOutput;
        }


    }
}
