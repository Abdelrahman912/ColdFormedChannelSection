using ColdFormedChannelSection.Core.Entities;
using ColdFormedChannelSection.Core.Enums;
using ColdFormedChannelSection.Core.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ColdFormedChannelSection.Core.Helpers
{
    public static class UnitConversionHelper
    {

        private static readonly Dictionary<Units, Tuple<Units, double>> _kipToTonDict = new Dictionary<Units, Tuple<Units, double>>()
        {
            [Units.IN] = Tuple.Create(Units.CM, 2.54),
            [Units.IN_2] = Tuple.Create(Units.CM_2, 2.54.Power(2)),
            [Units.IN_3] = Tuple.Create(Units.CM_3, 2.54.Power(3)),
            [Units.KIP] = Tuple.Create(Units.TON, 1.0 / 2.2046),
            [Units.KIP_IN] = Tuple.Create(Units.TON_CM, 1.152124),
            [Units.KSI] = Tuple.Create(Units.TON_CM_2, 1.0 / 14.223),
            [Units.NONE] = Tuple.Create(Units.NONE, 1.0),
        };

        private static readonly Dictionary<Units, Tuple<Units, double>> _kipToNDict = new Dictionary<Units, Tuple<Units, double>>()
        {
            [Units.IN] = Tuple.Create(Units.MM, 25.4),
            [Units.IN_2] = Tuple.Create(Units.MM_2, 25.4.Power(2)),
            [Units.IN_3] = Tuple.Create(Units.MM_3, 25.4.Power(3)),
            [Units.KIP] = Tuple.Create(Units.N, 4444.44),
            [Units.KIP_IN] = Tuple.Create(Units.N_MM, 112.9e03),
            [Units.KSI] = Tuple.Create(Units.N_MM_2, 6.889),
            [Units.NONE] = Tuple.Create(Units.NONE, 1.0),
        };

        private static readonly Dictionary<Units, Tuple<Units, double>> _tonToKip = new Dictionary<Units, Tuple<Units, double>>()
        {
            [Units.CM] = Tuple.Create(Units.IN, 1.0 / 2.54),
            [Units.CM_2] = Tuple.Create(Units.IN_2, (1.0 / 2.54).Power(2)),
            [Units.CM_3] = Tuple.Create(Units.IN_3, (1.0 / 2.54).Power(3)),
            [Units.TON] = Tuple.Create(Units.KIP, 2.2046),
            [Units.TON_CM] = Tuple.Create(Units.KIP_IN, 1.0 / 1.152124),
            [Units.TON_CM_2] = Tuple.Create(Units.KSI, 14.223),
            [Units.NONE] = Tuple.Create(Units.NONE, 1.0),
        };

        private static readonly Dictionary<Units, Tuple<Units, double>> _tonToN = new Dictionary<Units, Tuple<Units, double>>()
        {
            [Units.CM] = Tuple.Create(Units.MM, 10.0),
            [Units.CM_2] = Tuple.Create(Units.MM_2, (10.0).Power(2)),
            [Units.CM_3] = Tuple.Create(Units.MM_3, (10.0).Power(3)),
            [Units.TON] = Tuple.Create(Units.N, 9806.65),
            [Units.TON_CM] = Tuple.Create(Units.N_MM, 98066.5),
            [Units.TON_CM_2] = Tuple.Create(Units.N_MM_2, 98.07),
            [Units.NONE] = Tuple.Create(Units.NONE, 1.0),
        };

        private static readonly Dictionary<Units, Tuple<Units, double>> _nToKip = new Dictionary<Units, Tuple<Units, double>>()
        {
            [Units.MM] = Tuple.Create(Units.IN, 1.0 / 25.4),
            [Units.MM_2] = Tuple.Create(Units.IN_2, (1.0 / 25.4).Power(2)),
            [Units.MM_3] = Tuple.Create(Units.IN_3, (1.0 / 25.4).Power(3)),
            [Units.N] = Tuple.Create(Units.KIP, 1.0 / 4444.44),
            [Units.N_MM] = Tuple.Create(Units.KIP_IN, 1.0 / 112.9e03),
            [Units.N_MM_2] = Tuple.Create(Units.KSI, 1.0 / 6.889),
            [Units.NONE] = Tuple.Create(Units.NONE, 1.0),
        };

        private static readonly Dictionary<Units, Tuple<Units, double>> _nToTon = new Dictionary<Units, Tuple<Units, double>>()
        {
            [Units.MM] = Tuple.Create(Units.CM, 1.0 / 10.0),
            [Units.MM_2] = Tuple.Create(Units.CM_2, (1.0 / 10.0).Power(2)),
            [Units.MM_3] = Tuple.Create(Units.CM_3, (1.0 / 10.0).Power(3)),
            [Units.N] = Tuple.Create(Units.TON, 1.0 / 9806.65),
            [Units.N_MM] = Tuple.Create(Units.TON_CM, 1.0 / 98066.5),
            [Units.N_MM_2] = Tuple.Create(Units.TON_CM_2, 1.0 / 98.07),
            [Units.NONE] = Tuple.Create(Units.NONE, 1.0),
        };

        private static readonly Dictionary<KeyValuePair<UnitSystems, UnitSystems>, Dictionary<Units, Tuple<Units, double>>> _unitDict = new Dictionary<KeyValuePair<UnitSystems, UnitSystems>, Dictionary<Units, Tuple<Units, double>>>()
        {
            [KeyValuePair.Create(UnitSystems.KIPINCH, UnitSystems.TONCM)] = _kipToTonDict,
            [KeyValuePair.Create(UnitSystems.KIPINCH, UnitSystems.NMM)] = _kipToNDict,
            [KeyValuePair.Create(UnitSystems.TONCM, UnitSystems.KIPINCH)] = _tonToKip,
            [KeyValuePair.Create(UnitSystems.TONCM, UnitSystems.NMM)] = _tonToN,
            [KeyValuePair.Create(UnitSystems.NMM, UnitSystems.KIPINCH)] = _nToKip,
            [KeyValuePair.Create(UnitSystems.NMM, UnitSystems.TONCM)] = _nToTon,
        };

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

        public static Tuple<double, string> ConvertMoment(this double moment, UnitSystems sourceUnit, UnitSystems targetUnits)
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
            var newBracingConditions = new LengthBracingConditions(Lx, Ly, Lz, Kx, Ky, Kz, Lu, Cb, C1, Cm);
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

            var newOutput = new ResistanceInteractionOutput(pu, pn, mu, mn, output.IE, output.IEValue, mu_unit, pu_unit, output.Report);
            return newOutput;
        }

        public static MomentResistanceOutput Convert(this MomentResistanceOutput output, UnitSystems sourceUnit, UnitSystems targetUnit)
        {
            (var nominalResistance, var unit) = output.NominalResistance.ConvertMoment(sourceUnit, targetUnit);
            var phi = output.Phi;
            var failureMode = output.GoverningCase;
            var newOutput = new MomentResistanceOutput(nominalResistance, phi,output.PhiName,output.DesignResistanceName, failureMode, unit, output.Report);
            return newOutput;
        }

        public static CompressionResistanceOutput Convert(this CompressionResistanceOutput output, UnitSystems sourceUnit, UnitSystems targetUnit)
        {
            (var nominalResistance, var unit) = output.NominalResistance.ConvertForce(sourceUnit, targetUnit);
            var phi = output.Phi;
            var failureMode = output.GoverningCase;
            var newOutput = new CompressionResistanceOutput(nominalResistance, phi,output.PhiName,output.DesignResistanceName, failureMode, unit, output.Report);
            return newOutput;
        }


        private static ReportItem Convert(this ReportItem item, UnitSystems source, UnitSystems target)
        {
            if (item.Unit == Units.NONE)
                return item;
            (var newUnit, var factor) = _unitDict[KeyValuePair.Create(source, target)][item.Unit];
            var newValue = Double.Parse(item.Value) * factor;
            return new ReportItem(item.Name, newValue.ToString("0.###"), newUnit);
        }

        public static List<ReportItem> Convert(this List<ReportItem> items , UnitSystems source , UnitSystems target)
        {
            
            return items?.Select(item => item.Convert(source, target)).ToList();
        }

        //public static CompressionReport Convert(this CompressionReport report , UnitSystems source , UnitSystems target)
        //{
        //    if (source == target)
        //        return report;
        //    return new CompressionReport(
        //        title: report.Title,
        //        item1Name:report.Item1Name,
        //        item1List: report.Item1List.Convert(source,target),
        //        item2Name:report.Item2Name,
        //        item2List:report.Item2List.Convert(source,target),
        //        item3Name: report.Item3Name,
        //        item3List:report.Item3List.Convert(source,target),
        //        designCompressionList:report.DesignList.Convert(source,target),
        //        target
        //        );
        //}

        //public static MomentReport Convert(this MomentReport report, UnitSystems source, UnitSystems target)
        //{
        //    if (source == target)
        //        return report;
        //    return new MomentReport(
        //        title: report.Title,
        //        item1Name: report.Item1Name,
        //        item1List: report.Item1List.Convert(source, target),
        //        item2Name: report.Item2Name,
        //        item2List: report.Item2List.Convert(source, target),
        //        designList: report.DesignList.Convert(source, target),
        //        target
        //        );
        //}

        //public static InteractionReport Convert(this InteractionReport report, UnitSystems source, UnitSystems target) =>
        //    new InteractionReport(report.CompressionReport.Convert( target), report.MomentReport.Convert( target),target);
       

    }
}
