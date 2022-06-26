﻿using ColdFormedChannelSection.Core.Entities;
using ColdFormedChannelSection.Core.Enums;
using ColdFormedChannelSection.Core.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ColdFormedChannelSection.Core.Helpers
{
    public static class EuroHelper
    {

        private const double PHI = 1.0;

        private const string PHI_NAME = "gamma";

        private const string COMP_DESIGN_RESIST = "Pn/gamma";

        private const string MOM_DESIGN_RESIST = "Mn/gamma";

        private static bool IsValid(this LippedSection section)
        {
            var b_over_t = Tuple.Create(section.Properties.BPrime / section.Dimensions.ThicknessT, 60.0);

            var c_over_t = Tuple.Create(section.Properties.CPrime / section.Dimensions.ThicknessT, 50.0);
            var a_over_t = Tuple.Create(section.Properties.APrime / section.Dimensions.ThicknessT, 500.0);
            var c_over_b = section.Properties.CPrime / section.Properties.BPrime;

            var allows = new List<Tuple<double, double>>()
            {
                b_over_t,c_over_t,a_over_t
            };
            return !allows.Any(tuple => tuple.Item1 > tuple.Item2) && c_over_b >= 0.2 && c_over_b <= 0.6;
        }


        private static bool IsValid(this UnStiffenedSection section)
        {
            var b_over_t = Tuple.Create(section.Properties.BPrime / section.Dimensions.ThicknessT, 50.0);

            var a_over_t = Tuple.Create(section.Properties.APrime / section.Dimensions.ThicknessT, 500.0);

            var allows = new List<Tuple<double, double>>()
            {
                b_over_t,a_over_t
            };
            return !allows.Any(tuple => tuple.Item1 > tuple.Item2);

        }


        #region Moment & Compression

        public static ResistanceInteractionOutput AsEuroInteractionResistance(this LippedSection section, Material material, LengthBracingConditions bracingConditions, double pu, double mu)
        {
            var Pn = section.AsEuroCompressionResistance(material, bracingConditions,pu);
            var Mn = section.AsEuroMomentResistance(material, bracingConditions,mu);
            //tex:
            //$$ (\frac{P_u}{ P_n})^{0.8} + (\frac{M_u}{M_n})^{0.8}  $$
            var ie = (pu / Pn.NominalResistance).Power(0.8) + (mu / Mn.NominalResistance).Power(0.8);

            var sections = Pn.Report.Sections.Take(1).Concat(Pn.Report.Sections.Skip(1).Select(sec => sec.AppendToName("Compression")))
                                                .Concat(Mn.Report.Sections.Skip(1).Select(sec => sec.AppendToName("Moment")))
                                                .ToList();

            var report = new Report(UnitSystems.NMM, "Euro Code - Interaction", sections);

            return new ResistanceInteractionOutput(pu, Pn.NominalResistance, mu, Mn.NominalResistance, "(\\frac{P_u}{ P_n})^{0.8} + (\\frac{M_u}{M_n})^{0.8}", ie, "N.mm", "N", report);
        }

        public static ResistanceInteractionOutput AsEuroInteractionResistance(this UnStiffenedSection section, Material material, LengthBracingConditions bracingConditions, double pu, double mu)
        {
            var Pn = section.AsEuroCompressionResistance(material, bracingConditions,pu);
            var Mn = section.AsEuroMomentResistance(material, bracingConditions,mu);
            //tex:
            //$$ (\frac{P_u}{ P_n})^{0.8} + (\frac{M_u}{M_n})^{0.8}  $$
            var ie = (pu / Pn.NominalResistance).Power(0.8) + (mu / Mn.NominalResistance).Power(0.8);

            var sections = Pn.Report.Sections.Take(1).Concat(Pn.Report.Sections.Skip(1).Select(sec => sec.AppendToName("Compression")))
                                                .Concat(Mn.Report.Sections.Skip(1).Select(sec => sec.AppendToName("Moment")))
                                                .ToList();

            var report = new Report(UnitSystems.NMM, "Euro - Interaction", sections);

            return new ResistanceInteractionOutput(pu, Pn.NominalResistance, mu, Mn.NominalResistance, "(\\frac{P_u}{ P_n})^{0.8} + (\\frac{M_u}{M_n})^{0.8}", ie, "N.mm", "N", report);
        }

        #endregion

        #region Compression

        /// <summary>
        /// Get Xd.
        /// </summary>
        /// <param name="section"></param>
        /// <param name="material"></param>
        /// <param name="Kw"></param>
        /// <param name="be2_intial"></param>
        /// <param name="ce_intial"></param>
        /// <returns></returns>
        private static double ReduceLippedSection(this LippedSection section, Material material, double Kw, double be2_intial, double ce_intial)
        {
            var t = section.Dimensions.ThicknessT;
            var b_prime = section.Properties.BPrime;
            var a_prime = section.Properties.APrime;
            var E = material.E;
            var v = material.V;
            var Fy = material.Fy;

            var Xd = 1.0;
            var be2 = be2_intial;
            var ce = ce_intial;

            var As = t * (be2 + ce);
            var b1 = b_prime - ((be2.Power(2) * (t / 2.0)) / (As));
            var k = ((E * t.Power(3)) / (4 * (1 - v.Power(2)))) * ((1) / (b1.Power(2) * a_prime + b1.Power(3) + 0.5 * b1.Power(2) * a_prime * Kw));
            var Is = ((be2 * t.Power(3)) / (12.0)) + ((ce.Power(3) * t) / (12.0)) + (be2 * t * ((ce.Power(2)) / (2 * (be2 + ce))).Power(2)) + (ce * t * ((ce / 2) - ((ce.Power(2)) / (2 * (be2 + ce)))).Power(2));
            var sigma_cr = (2 * Math.Sqrt(k * E * Is)) / (As);
            var lambda_d = Math.Sqrt(Fy / sigma_cr);
            if (lambda_d <= 0.65)
                Xd = 1.0;
            else if (lambda_d >= 1.38)
                Xd = (0.66) / (lambda_d);
            else
                Xd = 1.47 - 0.723 * lambda_d;

            return Xd;
        }

        /// <summary>
        /// Gets ae
        /// </summary>
        /// <param name="section"></param>
        /// <param name="material"></param>
        /// <returns></returns>
        private static double ReduceWebCompression(this Section section, Material material)
        {
            var a_prime = section.Properties.APrime;
            var t = section.Dimensions.ThicknessT;
            var Fy = material.Fy;

            var sai_w = 1.0;
            var kw = 4.0;
            var epslon = Math.Sqrt(235.0 / Fy);
            var lambda_w = (a_prime / t) / (28.4 * epslon * Math.Sqrt(kw));

            var lambda_w_limit = 0.5 + Math.Sqrt(0.085 - 0.055 * sai_w);
            var row_w = 1.0;
            if (lambda_w > lambda_w_limit)
                row_w = (((lambda_w - 0.055 * (3 + sai_w)) / (lambda_w.Power(2)))).IfNegativeReturnOne().TakeMinWithOne();
            var ae = row_w * a_prime;
            return ae;
        }

        private static (double be1, double be2, double ce, double Xd, double Kf, double Kc) GetEuroReducedFlange(this LippedSection section, Material material, double kw)
        {
            var Fy = material.Fy;
            var b_prime = section.Properties.BPrime;
            var t = section.Dimensions.ThicknessT;
            var c_prime = section.Properties.CPrime;

            var isEqual = false;
            //Flange.
            var epslon = Math.Sqrt(235.0 / Fy);
            var sai_f = 1.0;
            var kf = 4.0;
            var lambda_f_initial = (b_prime / t) / (28.4 * epslon * Math.Sqrt(kf));
            var lambda_f = lambda_f_initial;
            //Lip.
            var kc = 0.5;
            if (c_prime / b_prime > 0.35)
                kc = 0.5 + 0.83 * ((c_prime / b_prime) - 0.35).Power(2.0 / 3.0);

            var lambda_c_initial = (c_prime / t) / (28.4 * epslon * Math.Sqrt(kc));
            var lambda_c = lambda_c_initial;
            var be2 = 0.0;
            var ce = 0.0;
            var Xd = 0.0;
            var Xd_new = 0.0;
            var lambda_f_limit = 0.5 + Math.Sqrt(0.085 - 0.055 * sai_f);
            var row_f = 1.0;
            var be1_lst = new List<double>();
            do
            {
                if (lambda_f > lambda_f_limit)
                    row_f = (((lambda_f - 0.055 * (3 + sai_f)) / (lambda_f.Power(2)))).IfNegativeReturnOne().TakeMinWithOne();
                else
                    row_f = 1.0;
                var be = row_f * b_prime;
                be1_lst.Add(0.5 * be);
                be2 = 0.5 * be;

                var row_c = 1.0;
                if (lambda_c > 0.748)
                    row_c = ((lambda_c - 0.188) / (lambda_c.Power(2))).IfNegativeReturnOne().TakeMinWithOne();
                else
                    row_c = 1.0;
                ce = row_c * c_prime;

                Xd_new = section.ReduceLippedSection(material, kw, be2, ce);
                if (Math.Abs(Xd_new - Xd) <= 0.0001)
                    isEqual = true;
                else
                    Xd = Xd_new;
                lambda_f = lambda_f_initial * Math.Sqrt(Xd);
                lambda_c = lambda_c_initial * Math.Sqrt(Xd);

            } while (!isEqual);

            return (be1: be1_lst.First(), be2: be2, ce: ce, Xd: Xd, Kf: kf, Kc: kc);
        }

        private static (double be1, double be2) GetEuroReducedFlange(this UnStiffenedSection section, Material material)
        {
            var Fy = material.Fy;
            var b_prime = section.Properties.BPrime;
            var t = section.Dimensions.ThicknessT;

            var epslon = Math.Sqrt(235.0 / Fy);
            var kf = 0.43;
            var lambda_f = (b_prime / t) / (28.4 * epslon * Math.Sqrt(kf));
            var row_f = 1.0;
            if (lambda_f > 0.748)
                row_f = ((lambda_f - 0.188) / (lambda_f.Power(2))).IfNegativeReturnOne().TakeMinWithOne();

            var be = row_f * b_prime;
            var be1 = 0.5 * be;
            var be2 = 0.5 * be;
            return (be1, be2);
        }

        private static Tuple<double, List<ReportItem>> GetEuroReducedArea(this LippedSection section, Material material)
        {
            var t = section.Dimensions.ThicknessT;
            (var be1, var be2, var ce, var Xd, var Kf, var Kc) = section.GetEuroReducedFlange(material, 1);

            //Web.
            var Kw = 4.0;
            var ae = section.ReduceWebCompression(material);

            var Ae = t * (2 * be1 + ae + 2 * Xd * (be2 + ce));

            var items = new List<ReportItem>()
            {
                new ReportItem("Kw",Kw.ToString("0.###"),Units.NONE),
                new ReportItem("Effective Height (ae)",ae.ToString("0.###"),Units.MM),
                new ReportItem("Kf",Kf.ToString("0.###"),Units.NONE),
                new ReportItem("Effective Flange Width (be)",(be1+be2).ToString("0.###"),Units.MM),
                new ReportItem("Kc",Kc.ToString("0.###"),Units.NONE),
                new ReportItem("Effective Lip (ce)",ce.ToString("0.###"),Units.MM),
                new ReportItem("Effective Area (Ae)",Ae.ToString("0.###"),Units.MM_2),
                new ReportItem("Reduction Factor (Xd)",Xd.ToString("0.###"),Units.NONE),
                new ReportItem("Yield Stress (Fy)",material.Fy.ToString("0.###"),Units.N_MM_2),
                new ReportItem("Nominal Load (Pn)",(material.Fy*Ae).ToString("0.###"),Units.N),
            };

            return Tuple.Create(Ae, items);
        }

        private static Tuple<double, List<ReportItem>> GetEuroReducedArea(this UnStiffenedSection section, Material material)
        {
            var t = section.Dimensions.ThicknessT;
            var Kf = 0.43;
            (var be1, var be2) = section.GetEuroReducedFlange(material);

            //Web.
            var Kw = 4.0;
            var ae = section.ReduceWebCompression(material);

            var Ae = t * (2 * be1 + ae + 2 * be2);
            var items = new List<ReportItem>()
            {
                 new ReportItem("Kw",Kw.ToString("0.###"),Units.NONE),
                new ReportItem("Effective Height (ae)",ae.ToString("0.###"),Units.MM),
                new ReportItem("Kf",Kf.ToString("0.###"),Units.NONE),
                new ReportItem("Effective Flange Width (be)",(be1+be2).ToString("0.###"),Units.MM),
                new ReportItem("Effective Area (Ae)",Ae.ToString("0.###"),Units.MM_2),
                new ReportItem("Yield Stress (Fy)",material.Fy.ToString("0.###"),Units.N_MM_2),
                new ReportItem("Nominal Load (Pn)",(material.Fy*Ae).ToString("0.###"),Units.N),
            };
            return Tuple.Create(Ae, items);
        }

        public static CompressionResistanceOutput AsEuroCompressionResistance(this LippedSection section, Material material, LengthBracingConditions bracingConditions , double pu)
        {
            if (!section.IsValid())
                return new CompressionResistanceOutput(0.0, PHI, PHI_NAME,COMP_DESIGN_RESIST, FailureMode.UNSAFE, "N", null);
            (var Ae, var localItems) = section.GetEuroReducedArea(material);
            var pn1 = Tuple.Create(section.GetEuroCompressionLBResistance(material, Ae), FailureMode.LOCALBUCKLING);
            (var pn_FB, var fbItems) = section.GetEuroCompressionFBResistance(material, bracingConditions, Ae, 0.34,pu);
            (var pn_TB, var tbItems) = section.GetEuroCompressionTBResistance(material, bracingConditions, Ae, 0.34,pu);
            (var pn_TFB, var tfbItems) = section.GetEuroCompressionTFBResistance(material, bracingConditions, Ae, 0.34,pu);
            var pn2 = Tuple.Create(pn_FB, FailureMode.FLEXURALBUCKLING);
            var pn3 = Tuple.Create(pn_TB, FailureMode.TORSIONALBUCKLING);
            var pn4 = Tuple.Create(pn_TFB, FailureMode.FLEXURAL_TORSIONAL_BUCKLING);
            var pns = new List<Tuple<double, FailureMode>>()
            {
                pn1, pn2, pn3,pn4
            };
            var pn = pns.OrderBy(tuple => tuple.Item1).First();
            var designItems = new List<ReportItem>()
            {
                new ReportItem("Governing Case",pn.Item2.GetDescription(),Units.NONE),
                new ReportItem("Nominal Load (Pn)",pn.Item1.ToString("0.###"),Units.N),
                new ReportItem("Gamma",(1.0).ToString("0.###"),Units.N),
                new ReportItem("Design Load (Pn/gamma)",pn.Item1.ToString("0.###"),Units.N),
            };
            var secDimsItems = new List<ReportItem>()
            {
                new ReportItem("H",section.Dimensions.TotalHeightH.ToString("0.###"),Units.MM),
                new ReportItem("B",section.Dimensions.TotalFlangeWidthB.ToString("0.###"),Units.MM),
                new ReportItem("R",section.Dimensions.InternalRadiusR.ToString("0.###"),Units.MM),
                new ReportItem("T",section.Dimensions.ThicknessT.ToString("0.###"),Units.MM),
                new ReportItem("C",section.Dimensions.TotalFoldWidthC.ToString("0.###"),Units.MM)
            };
            var secDimSection = new ListReportSection("Section Dimensions", secDimsItems);
            var localSection = new ListReportSection("Local Buckling", localItems);
            var flexuralSection = new ListReportSection("Flexural Buckling", fbItems);
            var tbSection = new ListReportSection("Torsional Buckling", tbItems);
            var tfbSection = new ListReportSection("Torsional Flexural Buckling", tfbItems);
            var designSection = new ListReportSection("Design Compression Load", designItems);

            var sections = new List<IReportSection>() { secDimSection, localSection, flexuralSection, tbSection, tfbSection, designSection };

            var report = new Report(UnitSystems.NMM, "Euro Code - Compression", sections);

            var result = new CompressionResistanceOutput(pn.Item1, PHI, PHI_NAME,COMP_DESIGN_RESIST, pn.Item2, "N", report);
            return result;
        }

        public static CompressionResistanceOutput AsEuroCompressionResistance(this UnStiffenedSection section, Material material, LengthBracingConditions bracingConditions,double pu)
        {
            if (!section.IsValid())
                return new CompressionResistanceOutput(0.0, PHI, PHI_NAME, COMP_DESIGN_RESIST, FailureMode.UNSAFE, "N", null);
            (var Ae, var localItems) = section.GetEuroReducedArea(material);
            var pn1 = Tuple.Create(section.GetEuroCompressionLBResistance(material, Ae), FailureMode.LOCALBUCKLING);
            (var pn_FB, var fbItems) = section.GetEuroCompressionFBResistance(material, bracingConditions, Ae, 0.49,pu);
            (var pn_TB, var tbItems) = section.GetEuroCompressionTBResistance(material, bracingConditions, Ae, 0.49, pu);
            (var pn_TFB, var tfbItems) = section.GetEuroCompressionTFBResistance(material, bracingConditions, Ae, 0.49, pu);
            var pn2 = Tuple.Create(pn_FB, FailureMode.FLEXURALBUCKLING);
            var pn3 = Tuple.Create(pn_TB, FailureMode.TORSIONALBUCKLING);
            var pn4 = Tuple.Create(pn_TFB, FailureMode.FLEXURAL_TORSIONAL_BUCKLING);
            var pns = new List<Tuple<double, FailureMode>>()
            {
                pn1, pn2, pn3,pn4
            };
            var pn = pns.OrderBy(tuple => tuple.Item1).First();
            var designItems = new List<ReportItem>()
            {
                new ReportItem("Governing Case",pn.Item2.GetDescription(),Units.NONE),
                new ReportItem("Nominal Load (Pn)",pn.Item1.ToString("0.###"),Units.N),
                new ReportItem("Gamma",(1.0).ToString("0.###"),Units.N),
                new ReportItem("Design Load (Pn/gamma)",pn.Item1.ToString("0.###"),Units.N),
            };
            var secDimsItems = new List<ReportItem>()
            {
                new ReportItem("H",section.Dimensions.TotalHeightH.ToString("0.###"),Units.MM),
                new ReportItem("B",section.Dimensions.TotalFlangeWidthB.ToString("0.###"),Units.MM),
                new ReportItem("R",section.Dimensions.InternalRadiusR.ToString("0.###"),Units.MM),
                new ReportItem("T",section.Dimensions.ThicknessT.ToString("0.###"),Units.MM),
            };
            var secDimSection = new ListReportSection("Section Dimensions", secDimsItems);
            var localSection = new ListReportSection("Local Buckling", localItems);
            var fbSection = new ListReportSection("Flexural Buckling", fbItems);
            var tbSection = new ListReportSection("Torsional Buckling", tbItems);
            var tfbSection = new ListReportSection("Torsional Flexural Buckling", tfbItems);
            var designSection = new ListReportSection("Design Compression Load", designItems);
            var sections = new List<IReportSection>() { secDimSection, localSection, fbSection, tbSection, tfbSection, designSection };

            var report = new Report(UnitSystems.NMM, "Euro Code - Compression", sections);

            var result = new CompressionResistanceOutput(pn.Item1, PHI, PHI_NAME, COMP_DESIGN_RESIST, pn.Item2, "N", report);
            return result;
        }

        private static double GetEuroCompressionLBResistance(this Section section, Material material, double Ae) =>
            Ae * material.Fy;

        private static Tuple<double, List<ReportItem>> GetEuroCompressionFBResistance(this Section section, Material material, LengthBracingConditions lengthBracingConditions, double Ae, double alpa_w,double pu)
        {
            var E = material.E;
            var Fy = material.Fy;
            var ky = lengthBracingConditions.Ky;
            var Ly = lengthBracingConditions.Ly;
            var Kx = lengthBracingConditions.Kx;
            var Lx = lengthBracingConditions.Lx;
            var ix = section.Properties.Rx;
            var iy = section.Properties.Ry;
            var A = section.Properties.A;

            var lambda_1 = Math.PI * Math.Sqrt(E / Fy);

            var lambda_y = ((ky * Ly) / iy) * (Math.Sqrt(Ae / A) / lambda_1);
            var lambda_x = ((Kx * Lx) / ix) * (Math.Sqrt(Ae / A) / lambda_1);

            var phi_y = 0.5 * (1 + alpa_w * (lambda_y - 0.2) + lambda_y.Power(2));
            var phi_x = 0.5 * (1 + alpa_w * (lambda_x - 0.2) + lambda_x.Power(2));

            var Xy = Math.Min(1.0, (1 / (phi_y + Math.Sqrt(phi_y.Power(2) - lambda_y.Power(2)))));
            var Xx = Math.Min(1.0, (1.0 / (phi_x + Math.Sqrt(phi_x.Power(2) - lambda_x.Power(2)))));
            var X = Math.Min(Xx, Xy);

            var Ncr_x = (Math.PI.Power(2) * E) / ((Kx * Lx) / ix).Power(2);
            var Ncr_y = (Math.PI.Power(2)*E)/((ky*Ly)/iy).Power(2);

            if(pu==0 && lambda_x*lambda_y <= 0.2)
            {
                X = 1;
            }else if(pu!=0 && (lambda_x*lambda_y <=0.2 || pu/Ncr_x <= 0.04 || pu/Ncr_y <= 0.04))
            {
                X = 1;
            }

            var Pn = X * Ae * Fy;
            var items = new List<ReportItem>()
            {
                new ReportItem("Flexural Stress (X.Fy)",(X*Fy).ToString("0.###"),Units.N_MM_2),
                new ReportItem("Flexural Area (A)",Ae.ToString("0.###"),Units.MM_2),
                new ReportItem("Nominal Load (Pn)",Pn.ToString("0.###"),Units.N)
            };
            return Tuple.Create(Pn, items);
        }


        private static Tuple<double, List<ReportItem>> GetEuroCompressionTBResistance(this Section section, Material material, LengthBracingConditions lengthBracingConditions, double Ae, double alpha_w,double pu)
        {
            var E = material.E;
            var G = material.G;
            var v = material.V;
            var Fy = material.Fy;
            var ix = section.Properties.Rx;
            var iy = section.Properties.Ry;
            var Xo = section.Properties.Xo;
            var J = section.Properties.J;
            var Cw = section.Properties.Cw;
            var Kz = lengthBracingConditions.Kz;
            var Lz = lengthBracingConditions.Lz;

            var io_squared = ix.Power(2) + iy.Power(2) + Xo.Power(2);
            var Ncr = (1 / io_squared) * (G * J + ((Math.PI.Power(2) * Cw * E) / (Kz * Lz).Power(2)));
            var lambda_t = Math.Sqrt((Ae * Fy) / (Ncr));
            var phi_t = 0.5 * (1 + alpha_w * (lambda_t - 0.2) + lambda_t.Power(2));
            var Xt = Math.Min(1.0, (1 / (phi_t + Math.Sqrt(phi_t.Power(2) - lambda_t.Power(2)))));
            if(pu != 0 && pu/Ncr <= 0.04)
            {
                Xt = 1;
            }
            var Pn = Xt * Ae * Fy;
            var items = new List<ReportItem>()
            {
                new ReportItem("Torsional Stress (Xt.Fy)",(Xt*material.Fy).ToString("0.###"),Units.N_MM_2),
                new ReportItem("Torsional Area (Ae)",Ae.ToString("0.###"),Units.MM_2),
                new ReportItem("Nominal Load (Pn)",Pn.ToString("0.###"),Units.N)
            };
            return Tuple.Create(Pn, items);
        }

        private static Tuple<double, List<ReportItem>> GetEuroCompressionTFBResistance(this Section section, Material material, LengthBracingConditions lengthBracingConditions, double Ae, double alpha_w,double pu)
        {
            var E = material.E;
            var G = material.G;
            var v = material.V;
            var Fy = material.Fy;
            var ix = section.Properties.Rx;
            var iy = section.Properties.Ry;
            var Xo = section.Properties.Xo;
            var J = section.Properties.J;
            var Cw = section.Properties.Cw;
            var Kx = lengthBracingConditions.Kx;
            var Lx = lengthBracingConditions.Lx;
            var Kz = lengthBracingConditions.Kz;
            var Lz = lengthBracingConditions.Lz;


            var io_squared = ix.Power(2) + iy.Power(2) + Xo.Power(2);
            var beta = 1 - (Xo.Power(2) / io_squared);
            var Ncr = (1 / io_squared) * (G * J + ((Math.PI.Power(2) * Cw * E) / (Kz * Lz).Power(2)));
            var Ncr_x = (Math.PI.Power(2) * E) / ((Kx * Lx) / ix).Power(2);
            var Ncr_ft = (Ncr_x / (2 * beta)) * (1 + (Ncr / Ncr_x) - Math.Sqrt((1 + (Ncr / Ncr_x)).Power(2) - 4 * beta * (Ncr / Ncr_x)));
            var lambda_ft = Math.Sqrt((Ae * Fy) / (Ncr_ft));
            var phi_ft = 0.5 * (1 + alpha_w * (lambda_ft - 0.2) + lambda_ft.Power(2));
            var X_ft = (1.0) / (phi_ft + Math.Sqrt(phi_ft.Power(2) - lambda_ft.Power(2))).TakeMinWithOne();
            if(pu!=0 && pu / Ncr_ft <=0.04)
            {
                X_ft = 1;
            }
            var Pn = X_ft * Ae * Fy;
            var items = new List<ReportItem>()
            {
                new ReportItem("Torsional Flexural Stress (Xft.Fy)",(X_ft*material.Fy).ToString("0.###"),Units.N_MM_2),
                new ReportItem("Torsional Flexural Area (Ae)",Ae.ToString("0.###"),Units.MM_2),
                new ReportItem("Nominal Load (Pn)",Pn.ToString("0.###"),Units.N)
            };
            return Tuple.Create(Pn, items);
        }


        #endregion


        #region Moment

        public static MomentResistanceOutput AsEuroMomentResistance(this LippedSection section, Material material, LengthBracingConditions bracingConditions,double mu)
        {
            if (!section.IsValid())
                return new MomentResistanceOutput(0.0, PHI, PHI_NAME,MOM_DESIGN_RESIST, FailureMode.UNSAFE, "N.mm", null);
            (var Ze, var localItems) = section.GetZe(material);
            var Mn1 = Tuple.Create(section.GetEuroMomentLBResistance(material, Ze), FailureMode.LOCALBUCKLING);
            (var Mn_LTB, var ltbItems) = section.GetEuroMomentLTBResistance(material, bracingConditions, Ze,mu);
            var Mn2 = Tuple.Create(Mn_LTB, FailureMode.LATERALTORSIONALBUCKLING);
            var Mns = new List<Tuple<double, FailureMode>>()
            {
                Mn1,Mn2
            };
            var Mn = Mns.OrderBy(tuple => tuple.Item1).First();
            var designItems = new List<ReportItem>()
            {
                new ReportItem("Governing Case",Mn.Item2.GetDescription(),Units.NONE),
                new ReportItem("Nominal Moment",Mn.Item1.ToString("0.###"),Units.N_MM),
                new ReportItem("gamma",1.0.ToString(),Units.NONE),
                new ReportItem("Design Moment (Mn/gamma)",Mn.Item1.ToString("0.###"),Units.N_MM)

            };
            var secDimsItems = new List<ReportItem>()
            {
                new ReportItem("H",section.Dimensions.TotalHeightH.ToString("0.###"),Units.MM),
                new ReportItem("B",section.Dimensions.TotalFlangeWidthB.ToString("0.###"),Units.MM),
                new ReportItem("R",section.Dimensions.InternalRadiusR.ToString("0.###"),Units.MM),
                new ReportItem("T",section.Dimensions.ThicknessT.ToString("0.###"),Units.MM),
                new ReportItem("C",section.Dimensions.TotalFoldWidthC.ToString("0.###"),Units.MM)
            };
            var secDimSection = new ListReportSection("Section Dimensions", secDimsItems);
            var localSection = new ListReportSection("Local Buckling", localItems);
            var ltbSection = new ListReportSection("Lateral Torsional Buckling", ltbItems);
            var designSection = new ListReportSection("Design Moment", designItems);
            var sections = new List<IReportSection>() { secDimSection, localSection, ltbSection, designSection };

            var report = new Report(UnitSystems.NMM, "Euro Code - Moment", sections);

            var result = new MomentResistanceOutput(Mn.Item1, PHI, PHI_NAME,MOM_DESIGN_RESIST, Mn.Item2, "N.mm", report);
            return result;
        }

        public static MomentResistanceOutput AsEuroMomentResistance(this UnStiffenedSection section, Material material, LengthBracingConditions bracingConditions,double mu)
        {
            if (!section.IsValid())
                return new MomentResistanceOutput(0.0, PHI, PHI_NAME,MOM_DESIGN_RESIST, FailureMode.UNSAFE, "N.mm", null);
            (var Ze, var localItems) = section.GetZe(material);
            var Mn1 = Tuple.Create(section.GetEuroMomentLBResistance(material, Ze), FailureMode.LOCALBUCKLING);
            (var Mn_LTB, var ltbItems) = section.GetEuroMomentLTBResistance(material, bracingConditions, Ze,mu);
            var Mn2 = Tuple.Create(Mn_LTB, FailureMode.LATERALTORSIONALBUCKLING);
            var Mns = new List<Tuple<double, FailureMode>>()
            {
                Mn1,Mn2
            };
            var Mn = Mns.OrderBy(tuple => tuple.Item1).First();
            var designItems = new List<ReportItem>()
            {
                new ReportItem("Governing Case",Mn.Item2.GetDescription(),Units.NONE),
                new ReportItem("Nominal Moment",Mn.Item1.ToString("0.###"),Units.N_MM),
                new ReportItem("gamma",(1.0).ToString("0.###"),Units.NONE),
                new ReportItem("Design Moment (Mn/gamma)",Mn.Item1.ToString("0.###"),Units.N_MM)

            };
            var secDimsItems = new List<ReportItem>()
            {
                new ReportItem("H",section.Dimensions.TotalHeightH.ToString("0.###"),Units.MM),
                new ReportItem("B",section.Dimensions.TotalFlangeWidthB.ToString("0.###"),Units.MM),
                new ReportItem("R",section.Dimensions.InternalRadiusR.ToString("0.###"),Units.MM),
                new ReportItem("T",section.Dimensions.ThicknessT.ToString("0.###"),Units.MM),
            };
            var secDimSection = new ListReportSection("Section Dimensions", secDimsItems);
            var localSection = new ListReportSection("Local Buckling", localItems);
            var ltbSection = new ListReportSection("Lateral Tosional Buckling", ltbItems);
            var designSection = new ListReportSection("Design Moment", designItems);
            var sections = new List<IReportSection>() { secDimSection, localSection, ltbSection, designSection };
            var report = new Report(UnitSystems.NMM, "Euro Code - Moment", sections);
            var result = new MomentResistanceOutput(Mn.Item1, PHI, PHI_NAME,MOM_DESIGN_RESIST, Mn.Item2, "N.mm", report);
            return result;
        }

        private static double GetEuroMomentLBResistance(this Section section, Material material, double Ze) =>
            Ze * material.Fy;

        private static Tuple<double, List<ReportItem>> GetEuroMomentLTBResistance(this Section section, Material material, LengthBracingConditions bracingConditions, double Ze,double mu)
        {
            var Fy = material.Fy;
            var E = material.E;
            var G = material.G;
            var v = material.V;
            var C1 = bracingConditions.C1;
            var Lu = bracingConditions.Lu;
            var Iy = section.Properties.Iy;
            var J = section.Properties.J;
            var Cw = section.Properties.Cw;

            var Mcr = C1 * ((Math.PI.Power(2) * E * Iy) / (Lu.Power(2))) * ((Cw / Iy) - ((Lu.Power(2) * G * J) / (Math.PI.Power(2) * E * Iy))).Power(0.5);

            var alpha_lt = 0.34;
            var lambda_lt = Math.Sqrt((Ze * Fy) / (Mcr));
            var phi_lt = 0.5 * (1 + alpha_lt * (lambda_lt - 0.2) + lambda_lt.Power(2));
            var x_lt = Math.Min(1.0, (1 / (phi_lt + Math.Sqrt(phi_lt.Power(2) - lambda_lt.Power(2)))));
            if (mu == 0 && lambda_lt <= 0.2)
                x_lt = 1;
            else if (mu != 0 && (lambda_lt <= 0.2 || mu / Mcr <= 0.16))
                x_lt = 1;
            var Mn = x_lt * Ze * Fy;
            var items = new List<ReportItem>()
            {
                new ReportItem("Lateral Torsional Section Modulus (Z)",Ze.ToString("0.###"),Units.MM_3),
                new ReportItem("Lateral Torsional Stress (F)",(x_lt*Fy).ToString("0.###"),Units.N_MM_2),
                new ReportItem("Lateral Torsional Nominal Moment (Mn)",Mn.ToString("0.###"),Units.N_MM),

            };
            return Tuple.Create(Mn, items);
        }



        private static Tuple<double, List<ReportItem>> GetZe(this Section section, Material material, double be1, double be2, double ce, double Xd)
        {
            var c_prime = section.Properties.CPrime;
            var b_prime = section.Properties.BPrime;
            var a_prime = section.Properties.APrime;
            var t = section.Dimensions.ThicknessT;
            var Fy = material.Fy;

            var epslon = Math.Sqrt(235.0 / Fy);

            var hc_numen = (c_prime * (a_prime - (c_prime / 2.0))) + (b_prime * a_prime) + (a_prime.Power(2) / 2.0) + ((ce.Power(2) * Xd) / (2.0));
            var hc_dnumen = c_prime + b_prime + a_prime + be1 + (be2 + ce) * Xd;
            var hc = hc_numen / hc_dnumen;
            var sai_w = (hc - a_prime) / (hc);
            var kw = 0.0;
            if (sai_w > -1 && sai_w < 0)
                kw = 7.81 - 6.29 * sai_w + 9.78 * sai_w.Power(2);
            else if (sai_w == -1)
                kw = 23.9;
            else if (sai_w < -1 && sai_w >= -3)
                kw = 5.98 * (1 - sai_w).Power(2);

            var lambda_w = (a_prime / t) / (28.4 * epslon * Math.Sqrt(kw));

            var lambda_w_limit = 0.5 + Math.Sqrt(0.085 - 0.055 * sai_w);

            var row_w = 1.0;
            if (lambda_w > lambda_w_limit)
                row_w = (((lambda_w - 0.055 * (3 + sai_w)) / (lambda_w.Power(2)))).IfNegativeReturnOne().TakeMinWithOne();

            var ae = row_w * (a_prime / (1 - sai_w));
            var he1 = 0.4 * ae;
            var he2 = 0.6 * ae;
            var h1 = he1;
            var h2 = a_prime - (hc - he2);
            var Ae = t * (c_prime + b_prime + h1 + h2 + be1 + (be2 + ce) * Xd);
            var y_bar = (t / Ae) * ((c_prime * (a_prime - (c_prime / 2))) + (b_prime * a_prime) + (h2 * (a_prime - (h2 / 2))) + (h1.Power(2) / 2) + ((ce.Power(2) * Xd) / (2)));
            var yt = a_prime - y_bar;
            var Ieff = ((h1.Power(3) * t) / 12.0) + ((h2.Power(3) * t) / (12.0)) + ((b_prime * t.Power(3)) / (12.0)) + ((c_prime.Power(3) * t) / (12.0)) +
                ((be1 * t.Power(3)) / (12)) + ((be2 * (Xd * t).Power(3)) / (12.0)) + ((ce.Power(3) * Xd * t) / 12.0)
                 + (c_prime * t * (yt - (c_prime / 2)).Power(2)) + (b_prime * t * yt.Power(2)) + (h2 * t * (yt - (h2 / 2)).Power(2))
                 + (h1 * t * (y_bar - (h1 / 2)).Power(2)) + (be1 * t * y_bar.Power(2)) + (be2 * Xd * t * y_bar.Power(2))
                 + (ce * Xd * t * (y_bar - (ce / 2)).Power(2));

            var Ze = Ieff / y_bar;
            var items = new List<ReportItem>()
            {
                new ReportItem("Effective Heigh (ae)",(h1+h2).ToString("0.###"),Units.MM),
                new ReportItem("Effective Flange Width (be)",(be1+be2).ToString("0.###"),Units.MM)
            };
            return Tuple.Create(Ze, items);

        }

        private static Tuple<double, List<ReportItem>> GetZe(this LippedSection section, Material material)
        {
            var t = section.Dimensions.ThicknessT;
            (var be1, var be2, var ce, var Xd, var Kf, var Kc) = section.GetEuroReducedFlange(material, 0);

            (var Ze, var local_items) = section.GetZe(material, be1, be2, ce, Xd);
            local_items.Add(new ReportItem("Effective Lip (Ce)", ce.ToString("0.###"), Units.MM));
            local_items.Add(new ReportItem("Reduction Factor (Xd)", Xd.ToString("0.###"), Units.NONE));
            local_items.Add(new ReportItem("Effective Section Modulus (Ze)", Ze.ToString("0.###"), Units.MM_3));
            local_items.Add(new ReportItem("Yield Stress (Fy)", material.Fy.ToString("0.###"), Units.N_MM_2));
            local_items.Add(new ReportItem("Local Nominal Moment (Mn)", (Ze * material.Fy).ToString("0.###"), Units.N_MM));

            return Tuple.Create(Ze, local_items);
        }


        private static Tuple<double, List<ReportItem>> GetZe(this UnStiffenedSection section, Material material)
        {
            var t = section.Dimensions.ThicknessT;
            (var be1, var be2) = section.GetEuroReducedFlange(material);

            (var Ze, var local_items) = section.GetZe(material, be1, be2, 0, 1.0);
            local_items.Add(new ReportItem("Effective Section Modulus (Ze)", Ze.ToString("0.###"), Units.MM_3));
            local_items.Add(new ReportItem("Yield Stress (Fy)", material.Fy.ToString("0.###"), Units.N_MM_2));
            local_items.Add(new ReportItem("Local Nominal Moment (Mn)", (Ze * material.Fy).ToString("0.###"), Units.N_MM));

            return Tuple.Create(Ze, local_items);
        }

        #endregion

    }
}
