using ColdFormedChannelSection.Core.Entities;
using ColdFormedChannelSection.Core.Enums;
using ColdFormedChannelSection.Core.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using static ColdFormedChannelSection.Core.Comparers.Comparers;

namespace ColdFormedChannelSection.Core.Helpers
{
    public static class EgyptianHelper
    {

        #region Constants

        private const double PHI_C = 0.8;

        private const double PHI_B = 0.85;

        private const string PHI_C_NAME = "(phi)c";

        private const string COMP_DESIGN_RESIST = "(phi)c * Pn";

        private const string PHI_B_NAME = "(phi)b";

        private const string MOM_DESIGN_RESIST = "(phi)b * Mn";

        #endregion

        #region Moment & Compression

        public static ResistanceInteractionOutput AsEgyptInteractionResistance(this LippedCSection section, Material material, LengthBracingConditions bracingConditions, double pu, double mu)
        {
            var Pn = section.AsEgyptCompressionResistance(material, bracingConditions);
            var Mn = section.AsEgyptMomentResistance(material, bracingConditions);
            //tex:
            //Load Ratio =$$ \frac {P_u} {\phi_c P_n} $$
            var loadRatio = pu / (PHI_C * Pn.NominalResistance);
            var ie = 0.0;
            var ieName = "";
            if (loadRatio >= 0.2)
            {
                //tex:
                //$$\frac {P_u} {\phi_c P_n} + \frac {8} {9} \frac {M_u} {\phi_b M_n}$$
                ie = (pu / (PHI_C * Pn.NominalResistance)) + (8.0 / 9.0) * (mu / (PHI_B * Mn.NominalResistance));
                ieName = "\\frac {P_u} {\\phi_c P_n} + \\frac {8} {9} \\frac {M_u} {\\phi_b M_n}";
            }
            else
            {
                //tex:
                //$$\frac {P_u} {2 \phi_c P_n} +  \frac {M_u} {\phi_b M_n}$$
                ie = (pu / (2 * PHI_C * Pn.NominalResistance)) + (mu / (PHI_B * Mn.NominalResistance));
                ieName = "\\frac {P_u} {2 \\phi_c P_n} +  \frac {M_u} {\\phi_b M_n}";
            }
            var sections = Pn.Report.Sections.Take(1).Concat(Pn.Report.Sections.Skip(1).Select(sec => sec.AppendToName("Compression")))
                                                .Concat(Mn.Report.Sections.Skip(1).Select(sec => sec.AppendToName("Moment")))
                                                .ToList();

            var report = new Report(UnitSystems.TONCM, "Egyptian Code - Interaction", sections);
            return new ResistanceInteractionOutput(pu, Pn.NominalResistance, mu, Mn.NominalResistance, ieName, ie, "t.cm", "ton", report);
        }


        public static ResistanceInteractionOutput AsEgyptInteractionResistance(this UnStiffenedCSection section, Material material, LengthBracingConditions bracingConditions, double pu, double mu)
        {
            var Pn = section.AsEgyptCompressionResistance(material, bracingConditions);
            var Mn = section.AsEgyptMomentResistance(material, bracingConditions);
            //tex:
            //Load Ratio =$$ \frac {P_u} {\phi_c P_n} $$
            var loadRatio = pu / (PHI_C * Pn.NominalResistance);
            var ie = 0.0;
            var ieName = "";
            if (loadRatio >= 0.2)
            {
                //tex:
                //$$\frac {P_u} {\phi_c P_n} + \frac {8} {9} \frac {M_u} {\phi_b M_n}$$
                ie = (pu / (PHI_C * Pn.NominalResistance)) + (8.0 / 9.0) * (mu / (PHI_B * Mn.NominalResistance));
                ieName = "\\frac {P_u} {\\phi_c P_n} + \\frac {8} {9} \\frac {M_u} {\\phi_b M_n}";
            }
            else
            {
                //tex:
                //$$\frac {P_u} {2 \phi_c P_n} +  \frac {M_u} {\phi_b M_n}$$
                ie = (pu / (2 * PHI_C * Pn.NominalResistance)) + (mu / (PHI_B * Mn.NominalResistance));
                ieName = "\\frac {P_u} {2 \\phi_c P_n} +  \frac {M_u} {\\phi_b M_n}";
            }

            var sections = Pn.Report.Sections.Take(1).Concat(Pn.Report.Sections.Skip(1).Select(sec => sec.AppendToName("Compression")))
                                                .Concat(Mn.Report.Sections.Skip(1).Select(sec => sec.AppendToName("Moment")))
                                                .ToList();

            var report = new Report(UnitSystems.TONCM, "Egyptian Code - Interaction", sections);

            return new ResistanceInteractionOutput(pu, Pn.NominalResistance, mu, Mn.NominalResistance, ieName, ie, "t.cm", "ton", report);
        }

        #endregion

        #region Moment

        private static bool IsValidMoment(this LippedCSection section)
        {
            var c_over_t = Tuple.Create(section.Properties.CPrime / section.Dimensions.ThicknessT, 40.0);
            var b_over_t = Tuple.Create(section.Properties.BPrime / section.Dimensions.ThicknessT, 60.0);
            var a_over_t = Tuple.Create(section.Properties.APrime / section.Dimensions.ThicknessT, 200.0);

            var allows = new List<Tuple<double, double>>()
            {
                b_over_t,c_over_t,a_over_t
            };
            return !allows.Any(tuple => tuple.Item1 > tuple.Item2);
        }


        private static bool IsValidMoment(this UnStiffenedCSection section)
        {
            var b_over_t = Tuple.Create(section.Properties.BPrime / section.Dimensions.ThicknessT, 40.0);

            var a_over_t = Tuple.Create(section.Properties.APrime / section.Dimensions.ThicknessT, 200.0);

            var allows = new List<Tuple<double, double>>()
            {
                b_over_t,a_over_t
            };
            return !allows.Any(tuple => tuple.Item1 > tuple.Item2);

        }


        public static MomentResistanceOutput AsEgyptMomentResistance(this LippedCSection section, Material material, LengthBracingConditions bracingConditions)
        {
            if (!section.IsValidMoment())
                return new MomentResistanceOutput(0.0, PHI_B, PHI_B_NAME, MOM_DESIGN_RESIST, FailureMode.UNSAFE, "t.cm", null);

            (var Ze, var Z_items) = section.GetEgyptReducedZe(material);

            (var Mn, var failureMode, var items_stress) = section.GetEgyptMomentResistance(material, bracingConditions, Ze);

            var nominalItems = Z_items.Concat(items_stress).ToList();
            var designItems = new List<ReportItem>()
            {
                new ReportItem("Governing Case",failureMode.GetDescription(),Units.NONE),
                new ReportItem("Nominal Moment (Mn)",Mn.ToString("0.###"),Units.TON_CM),
                new ReportItem("phi",PHI_B.ToString("0.###"),Units.NONE),
                new ReportItem("Design Moment",(PHI_B*Mn).ToString("0.###"),Units.TON_CM)
            };
            var secDimsItems = new List<ReportItem>()
            {
                new ReportItem("H",section.Dimensions.TotalHeightH.ToString("0.###"),Units.CM),
                new ReportItem("B",section.Dimensions.TotalFlangeWidthB.ToString("0.###"),Units.CM),
                new ReportItem("R",section.Dimensions.InternalRadiusR.ToString("0.###"),Units.CM),
                new ReportItem("t",section.Dimensions.ThicknessT.ToString("0.###"),Units.CM),
                new ReportItem("C",section.Dimensions.TotalFoldWidthC.ToString("0.###"),Units.CM)
            };
            var secDimSection = new ListReportSection("Section Dimensions", secDimsItems);
            var nominalSection = new ListReportSection("Nominal Moment", nominalItems);
            var designSection = new ListReportSection("Design Moment", designItems);
            var sections = new List<IReportSection>() { secDimSection, nominalSection, designSection };
            var report = new Report(UnitSystems.TONCM, "Egytian Code - Moment", sections);
            var result = new MomentResistanceOutput(Mn, PHI_B, PHI_B_NAME, MOM_DESIGN_RESIST, failureMode, "t.cm", report);
            return result;
        }

        public static MomentResistanceOutput AsEgyptMomentResistance(this UnStiffenedCSection section, Material material, LengthBracingConditions bracingConditions)
        {
            if (!section.IsValidMoment())
                return new MomentResistanceOutput(0.0, PHI_B, PHI_B_NAME, MOM_DESIGN_RESIST, FailureMode.UNSAFE, "t.cm", null);

            (var Ze, var Z_items) = section.GetEgyptReducedZe(material);
            (var Mn, var failureMode, var items_stress) = section.GetEgyptMomentResistance(material, bracingConditions, Ze);

            var nominalItems = Z_items.Concat(items_stress).ToList();

            var designItems = new List<ReportItem>()
            {
                new ReportItem("Governing Case",failureMode.GetDescription(),Units.NONE),
                new ReportItem("Nominal Moment (Mn)",Mn.ToString("0.###"),Units.TON_CM),
                new ReportItem("phi",PHI_B.ToString("0.###"),Units.NONE),
                new ReportItem("Design Moment",(PHI_B*Mn).ToString("0.###"),Units.TON_CM)
            };
            var secDimsItems = new List<ReportItem>()
            {
                new ReportItem("H",section.Dimensions.TotalHeightH.ToString("0.###"),Units.CM),
                new ReportItem("B",section.Dimensions.TotalFlangeWidthB.ToString("0.###"),Units.CM),
                new ReportItem("R",section.Dimensions.InternalRadiusR.ToString("0.###"),Units.CM),
                new ReportItem("t",section.Dimensions.ThicknessT.ToString("0.###"),Units.CM),
            };
            var secDimSection = new ListReportSection("Section Dimensions", secDimsItems);
            var nominalSection = new ListReportSection("Nominal Moment", nominalItems);
            var designSection = new ListReportSection("Design Moment", designItems);
            var sections = new List<IReportSection>() { secDimSection, nominalSection, designSection };

            var report = new Report(UnitSystems.TONCM, "Egyptian Code - Moment", sections);

            var result = new MomentResistanceOutput(Mn, PHI_B, PHI_B_NAME, MOM_DESIGN_RESIST, failureMode, "t.cm", report);
            return result;
        }

        private static Tuple<double, FailureMode, List<ReportItem>> GetEgyptMomentResistance(this Section section, Material material, LengthBracingConditions bracingConditions, double Ze)
        {
            var Fy = material.Fy;
            var b = section.Properties.BSmall;
            var a = section.Properties.ASmall;
            var t = section.Dimensions.ThicknessT;
            var Zg = section.Properties.Zg;
            var H = section.Dimensions.TotalHeightH;
            var Lu = bracingConditions.Lu;
            var lambda_f = (b / t);
            if (lambda_f <= 30)
            {
                var C_star = 817.0;
                var Mn1 = C_star * (Zg / lambda_f.Power(2));
                var It = ((b.Power(3) * t) / 12) + (1.0 / 6.0) * a * t.Power(3);
                var At = b * t + (1.0 / 6.0) * a * t;
                var rt = Math.Sqrt(It / At);
                var Mn2 = Math.Min(Ze * Fy, Zg * Math.Sqrt(((1380 * b * t) / (H * Lu)).Power(2) + ((20700) / (Lu / rt).Power(2)).Power(2)));
                var Ze_report = 0.0;
                var F_report = 0.0;
                if (Mn2 < Ze * Fy)
                {
                    Ze_report = Zg;
                    F_report = Mn2 / Zg;
                }
                else
                {
                    Ze_report = Ze;
                    F_report = Fy;
                }

                var items = new List<ReportItem>()
                {
                    new ReportItem("Local Section Modulus",Zg.ToString("0.###"),Units.CM_3),
                    new ReportItem("Local Stress (F)",(C_star/lambda_f.Power(2)).ToString("0.###"),Units.TON_CM_2),
                    new ReportItem("Local Nominal Moment (Mn)",Mn1.ToString("0.###"),Units.TON_CM),
                    new ReportItem("Lateral Torsional Modulus (Z)",Ze_report.ToString("0.###"),Units.CM_3),
                    new ReportItem("Lateral Torsional Stress (F)",F_report.ToString("0.###"),Units.TON_CM_2),
                    new ReportItem("Lateral Torsional Nominal Moment (Mn)",Mn2.ToString("0.###"),Units.TON_CM)
                };
                if (Mn1 < Mn2)
                    return Tuple.Create(Mn1, FailureMode.LOCALBUCKLING, items);
                else
                    return Tuple.Create(Mn2, FailureMode.LATERALTORSIONALBUCKLING, items);
            }
            else
            {
                var Mn = Ze * Fy;
                var items = new List<ReportItem>()
                {
                    new ReportItem("Flange Slenderness Ratio (lambadaF)",lambda_f.ToString("0.###"),Units.NONE),
                    new ReportItem("Nominal Moment (Mn)",Mn.ToString("0.###"),Units.TON_CM)
                };
                return Tuple.Create(Mn, FailureMode.LOCALBUCKLING, items);
            }

        }

        private static Tuple<double, List<ReportItem>> GetEgyptReducedZe(this UnStiffenedCSection section, Material material)
        {
            var E = material.E;
            var Fy = material.Fy;
            var C = section.Dimensions.TotalFoldWidthC;
            var b = section.Properties.BSmall;
            var t = section.Dimensions.ThicknessT;
            var a_prime = section.Properties.APrime;
            var b_prime = section.Properties.BPrime;
            var c_prime = section.Properties.CPrime;
            var c = section.Properties.CSmall;

            var Kf = 0.43;
            var sai_f = 1.0;
            var lambda_f = ((b_prime / t) / 59) * Math.Sqrt(Fy / Kf);
            var row_f = Math.Min(1, (lambda_f - 0.15 - 0.05 * sai_f) / (lambda_f.Power(2)));
            var be = row_f * b_prime;
            (var Ze, var items) = section.GetEgyptReducedZe(material, be, false,0);
            items.Add(new ReportItem("Effective Section Modulus (Ze)", Ze.ToString("0.###"), Units.CM_3));
            items.Add(new ReportItem("Yield Stress (Fy)", material.Fy.ToString("0.###"), Units.TON_CM_2));
            return Tuple.Create(Ze, items);
        }

        private static Tuple<double, List<ReportItem>> GetEgyptReducedZe(this LippedCSection section, Material material)
        {
            var E = material.E;
            var Fy = material.Fy;
            var C = section.Dimensions.TotalFoldWidthC;
            var b = section.Properties.BSmall;
            var t = section.Dimensions.ThicknessT;
            var a_prime = section.Properties.APrime;
            var b_prime = section.Properties.BPrime;
            var c_prime = section.Properties.CPrime;
            var c = section.Properties.CSmall;
            var H = section.Dimensions.TotalHeightH;
            var r = section.Properties.RSmall;

            var b_over_t = b / t;
            var c_over_b = C / b;
            var s = 1.28 * Math.Sqrt(E / Fy);

            var be = b_prime;
            var Is = (t * c.Power(3)) / 12;
            var Ia = 0.0;
            if (b_over_t <= s / 3 && c_over_b <= 0.25)
            {
                be = b_prime;
            }
            else if (b_over_t < s && b_over_t > s / 3 && c_over_b > 0.25 && c_over_b <= 0.8)
            {
                Ia = 399 * (((b_over_t) / s) - 0.33).Power(3) * t.Power(4);
                var Kf_1 = (4.82 - 5 * (C / b)) * (Is / Ia).Power(0.5) + 0.43;
                var Kf_2 = 5.25 - 5 * (C / b);
                var Kf = Math.Min(Kf_1, Kf_2);
                var lambda_f = ((b_prime / t) / 44) * Math.Sqrt(Fy / Kf);
                var sai_f = 1.0;
                var row_f = Math.Min(1, Math.Abs((lambda_f - 0.15 - 0.05 * sai_f) / (lambda_f.Power(2))));
                be = row_f * b_prime;

            }
            else if (b_over_t < s && b_over_t > s / 3 && c_over_b <= 0.25)
            {
                Ia = 399 * (((b_over_t) / s) - 0.33).Power(3) * t.Power(4);
                var Kf = Math.Min(3.57 * (Is / Ia).Power(0.5) + 0.43, 4);
                var lambda_f = ((b_prime / t) / 44) * Math.Sqrt(Fy / Kf);
                var sai_f = 1.0;
                var row_f = Math.Min(1, Math.Abs((lambda_f - 0.15 - 0.05 * sai_f) / (lambda_f.Power(2))));
                be = row_f * b_prime;

            }
            else if (b_over_t >= s & c_over_b > 0.25 && c_over_b <= 0.8)
            {

                Ia = (((115 * (b / t)) / (s)) + 5) * t.Power(4);
                var Kf_1 = (4.82 - 5 * (C / b)) * (Is / Ia).Power(1 / 3.0) + 0.43;
                var Kf_2 = 5.25 - 5 * (C / b);
                var Kf = Math.Min(Kf_1, Kf_2);
                var lambda_f = ((b_prime / t) / 44) * Math.Sqrt(Fy / Kf);
                var sai_f = 1.0;
                var row_f = Math.Min(1, Math.Abs((lambda_f - 0.15 - 0.05 * sai_f) / (lambda_f.Power(2))));
                be = row_f * b_prime;


            }
            else if (b_over_t >= s && c_over_b <= 0.25)
            {
                Ia = (((115 * (b / t)) / (s)) + 5) * t.Power(4);
                var Kf = Math.Min(3.57 * (Is / Ia).Power(1 / 3.0) + 0.43, 4);
                var lambda_f = ((b_prime / t) / 44) * Math.Sqrt(Fy / Kf);
                var sai_f = 1.0;
                var row_f = Math.Min(1, Math.Abs((lambda_f - 0.15 - 0.05 * sai_f) / (lambda_f.Power(2))));
                be = row_f * b_prime;
            }
            var Ri = Math.Min(1, Is / Ia);



            (var Ze, var items) = section.GetEgyptReducedZe(material, be, true, Ri);
            items.Add(new ReportItem("Effective Section Modulus (Ze)", Ze.ToString("0.###"), Units.CM_3));
            items.Add(new ReportItem("Yield Stress (Fy)", material.Fy.ToString("0.###"), Units.TON_CM_2));
            return Tuple.Create(Ze, items);
        }

        private static Tuple<double, List<ReportItem>> GetEgyptReducedZe(this Section section, Material material, double be, bool isLipped, double Ri)
        {
            var E = material.E;
            var Fy = material.Fy;
            var C = section.Dimensions.TotalFoldWidthC;
            var b = section.Properties.BSmall;
            var t = section.Dimensions.ThicknessT;
            var a_prime = section.Properties.APrime;
            var b_prime = section.Properties.BPrime;
            var c_prime = section.Properties.CPrime;
            var c = section.Properties.CSmall;
            var H = section.Dimensions.TotalHeightH;
            var r = section.Properties.RSmall;
            var y_bar = H / 2;
            var new_y_bar = H / 2;
            var isEqual = false;
            var Ae = 0.0;
            var xd = 1.0;
            var be1 = 0.5 * be;
            var be2 = 0.5 * be;
            var h1 = 0.0;
            var h2 = 0.0;
            var Kc = 0.0;
            var ce = 0.0;

            //start loop
            do
            {
                y_bar = new_y_bar;
                var F1 = Fy * ((y_bar - (t / 2) - r) / (y_bar));
                var F2 = Fy * ((y_bar - C) / (y_bar));
                var F3 = Fy * ((H - y_bar - (t / 2) - r) / y_bar);

                if (isLipped)
                {
                    var sai_c = F2 / F1;
                    Kc = (0.578) / (sai_c + 0.34);
                    var lambda_c = ((c_prime / t) / 59.0) * (Math.Sqrt(Fy / Kc));
                    var row_c = Math.Min(1, Math.Abs((lambda_c - 0.15 - 0.05 * lambda_c) / (lambda_c.Power(2))));
                    ce = row_c * c_prime * Ri;
                }
                var sai_w = -(F3 / F1);
                var kw = 0.0;
                if (Math.Abs(sai_w + 1) <= 0.00002)
                {
                    kw = 23.9;
                }
                else if (sai_w < 0 && sai_w > -1)
                {
                    kw = 7.81 - 6.29 * sai_w + 9.78 * sai_w.Power(2);
                }
                //else if (sai_w < -1 && sai_w > -2)
                //{
                //    kw = 5.98 * (1 - sai_w).Power(2);
                //}
                else
                {
                    //To handle the wrong cases where sai_w < -2
                    kw = 5.98 * (1 - sai_w).Power(2);
                }
                var lambda_w = ((a_prime / t) / 44) * (Math.Sqrt(Fy / kw));
                var row_w = ((1.1 * lambda_w - 0.16 - 0.1 * sai_w) / (lambda_w.Power(2))).IfNegativeReturnOne().TakeMinWithOne();
                var ae = row_w * (a_prime / (1 - sai_w));


                var he1 = 0.4 * ae;
                var he2 = 0.6 * ae;
                h1 = he1;
                var hc_num = c_prime * (a_prime - (c_prime / 2)) + b_prime * a_prime + (a_prime.Power(2) / 2) + (ce.Power(2) * xd / 2);
                var hc_dnum = c_prime + b_prime + a_prime + be1 + (be2 + ce) * xd;
                var hc = hc_num / hc_dnum;
                h2 = a_prime - (hc - he2);
                Ae = t * (c_prime + b_prime + h1 + h2 + be1 + (be2 + ce) * xd);
                new_y_bar = (t / Ae) * ((c_prime * (a_prime - (c_prime / 2))) + (b_prime * a_prime) + (h2 * (a_prime - (h2 / 2))) + (h1.Power(2) / 2) + ((ce.Power(2) * xd) / (2)));
                if (Math.Abs(y_bar - new_y_bar) <= 0.00002)
                    isEqual = true;
            } while (!isEqual);
            var yt = a_prime - y_bar;
            var Ieff = ((h1.Power(3) * t) / 12.0) + ((h2.Power(3) * t) / (12.0)) + ((b_prime * t.Power(3)) / (12.0)) + ((c_prime.Power(3) * t) / (12.0)) +
                 ((be1 * t.Power(3)) / (12)) + ((be2 * (xd * t).Power(3)) / (12.0)) + ((ce.Power(3) * xd * t) / 12.0)
                  + (c_prime * t * (yt - (c_prime / 2)).Power(2)) + (b_prime * t * yt.Power(2)) + (h2 * t * (yt - (h2 / 2)).Power(2))
                  + (h1 * t * (y_bar - (h1 / 2)).Power(2)) + (be1 * t * y_bar.Power(2)) + (be2 * xd * t * y_bar.Power(2))
                  + (ce * xd * t * (y_bar - (ce / 2)).Power(2));

            var Ze = Ieff / y_bar;
            var items = new List<ReportItem>()
            {
                new ReportItem("Effective Height (ae)",(h1+h2).ToString("0.###"),Units.CM),
                new ReportItem("Effective Flange Width (be)",(be).ToString("0.###"),Units.CM),
            };
            if(isLipped)
                items.Add(new ReportItem("Effective Lip (ce)", ce.ToString("0.###"), Units.CM));
            return Tuple.Create(Ze, items);
        }

        #endregion

        #region Compression

        private static bool IsValidCompression(this LippedCSection section)
        {
            var c_over_t = Tuple.Create(section.Properties.CPrime / section.Dimensions.ThicknessT, 40.0);
            var b_over_t = Tuple.Create(section.Properties.BPrime / section.Dimensions.ThicknessT, 60.0);
            var a_over_t = Tuple.Create(section.Properties.APrime / section.Dimensions.ThicknessT, 300.0);

            var allows = new List<Tuple<double, double>>()
            {
                b_over_t,c_over_t,a_over_t
            };
            return !allows.Any(tuple => tuple.Item1 > tuple.Item2);
        }


        private static bool IsValidCompression(this UnStiffenedCSection section)
        {
            var b_over_t = Tuple.Create(section.Properties.BPrime / section.Dimensions.ThicknessT, 40.0);

            var a_over_t = Tuple.Create(section.Properties.APrime / section.Dimensions.ThicknessT, 300.0);

            var allows = new List<Tuple<double, double>>()
            {
                b_over_t,a_over_t
            };
            return !allows.Any(tuple => tuple.Item1 > tuple.Item2);

        }


        private static Tuple<double, List<ReportItem>> GetEgyptReducedArea(this LippedCSection section, Material material)
        {
            var E = material.E;
            var Fy = material.Fy;
            var C = section.Dimensions.TotalFoldWidthC;
            var b = section.Properties.BSmall;
            var t = section.Dimensions.ThicknessT;
            var a_prime = section.Properties.APrime;
            var b_prime = section.Properties.BPrime;
            var c_prime = section.Properties.CPrime;
            var c = section.Properties.CSmall;

            var b_over_t = b / t;
            var c_over_b = C / b;
            var s = 1.28 * Math.Sqrt(E / Fy);

            var be = b_prime;
            var ce = c_prime;
            var Is = (t * c.Power(3)) / 12;

            var Kf = 0.0;
            var Kc = 0.0;

            if (b_over_t <= s / 3 && c_over_b <= 0.25)
            {
                be = b_prime;
                ce = c_prime;
            }
            else if (b_over_t < s && b_over_t > s / 3 && c_over_b > 0.25 && c_over_b <= 0.8)
            {
                var Ia = 399 * (((b_over_t) / s) - 0.33).Power(3) * t.Power(4);
                var Kf_1 = (4.82 - 5 * (C / b)) * (Is / Ia).Power(0.5) + 0.43;
                var Kf_2 = 5.25 - 5 * (C / b);
                Kf = Math.Min(Kf_1, Kf_2);
                var lambda_f = ((b_prime / t) / 44) * Math.Sqrt(Fy / Kf);
                var sai_f = 1.0;
                var row_f = ((lambda_f - 0.15 - 0.05 * sai_f) / (lambda_f.Power(2))).IfNegativeReturnOne().TakeMinWithOne(); ;
                be = row_f * b_prime;
                Kc = 0.43;
                var lambda_c = ((c_prime / t) / 59) * Math.Sqrt(Fy / Kc);
                var sai_c = 1.0;
                var row_c = (((lambda_c - 0.15 - 0.05 * sai_c)) / lambda_c.Power(2)).IfNegativeReturnOne().TakeMinWithOne();
                var Ri = Math.Min(Is / Ia, 1);
                ce = row_c * c_prime * Ri;
            }
            else if (b_over_t < s && b_over_t > s / 3 && c_over_b <= 0.25)
            {
                var Ia = 399 * (((b_over_t) / s) - 0.33).Power(3) * t.Power(4);
                Kf = Math.Min(3.57 * (Is / Ia).Power(0.5) + 0.43, 4);
                var lambda_f = ((b_prime / t) / 44) * Math.Sqrt(Fy / Kf);
                var sai_f = 1.0;
                var row_f = ((lambda_f - 0.15 - 0.05 * sai_f) / (lambda_f.Power(2))).IfNegativeReturnOne().TakeMinWithOne();
                be = row_f * b_prime;
                Kc = 0.43;
                var lambda_c = ((c_prime / t) / 59) * Math.Sqrt(Fy / Kc);
                var sai_c = 1.0;
                var row_c = (((lambda_c - 0.15 - 0.05 * sai_c) / lambda_c.Power(2))).IfNegativeReturnOne().TakeMinWithOne();
                var Ri = Math.Min(Is / Ia, 1);
                ce = row_c * c_prime * Ri;
            }
            else if (b_over_t >= s & c_over_b > 0.25 && c_over_b <= 0.8)
            {

                var Ia = (((115 * (b / t)) / (s)) + 5) * t.Power(4);
                var Kf_1 = (4.82 - 5 * (C / b)) * (Is / Ia).Power(1 / 3.0) + 0.43;
                var Kf_2 = 5.25 - 5 * (C / b);
                Kf = Math.Min(Kf_1, Kf_2);
                var lambda_f = ((b_prime / t) / 44) * Math.Sqrt(Fy / Kf);
                var sai_f = 1.0;
                var row_f = ((lambda_f - 0.15 - 0.05 * sai_f) / (lambda_f.Power(2))).IfNegativeReturnOne().TakeMinWithOne();
                be = row_f * b_prime;
                Kc = 0.43;
                var lambda_c = ((c_prime / t) / 59) * Math.Sqrt(Fy / Kc);
                var sai_c = 1.0;
                var row_c = (((lambda_c - 0.15 - 0.05 * sai_c) / lambda_c.Power(2))).IfNegativeReturnOne().TakeMinWithOne();
                var Ri = Math.Min(Is / Ia, 1);
                ce = row_c * c_prime * Ri;

            }
            else if (b_over_t >= s && c_over_b <= 0.25)
            {
                var Ia = (((115 * (b / t)) / (s)) + 5) * t.Power(4);
                Kf = Math.Min(3.57 * (Is / Ia).Power(1 / 3.0) + 0.43, 4);
                var lambda_f = ((b_prime / t) / 44) * Math.Sqrt(Fy / Kf);
                var sai_f = 1.0;
                var row_f = ((lambda_f - 0.15 - 0.05 * sai_f) / (lambda_f.Power(2))).IfNegativeReturnOne().TakeMinWithOne();
                be = row_f * b_prime;
                Kc = 0.43;
                var lambda_c = ((c_prime / t) / 59) * Math.Sqrt(Fy / Kc);
                var sai_c = 1.0;
                var row_c = (((lambda_c - 0.15 - 0.05 * sai_c) / lambda_c.Power(2))).IfNegativeReturnOne().TakeMinWithOne();
                var Ri = Math.Min(Is / Ia, 1);
                ce = row_c * c_prime * Ri;
            }

            var sai_w = 1.0;
            var Kw = 4.0;
            var lambda_w = ((a_prime / t) / 44) * Math.Sqrt(Fy / Kw);
            var row_w = ((1.1 * lambda_w - 0.16 - 0.1 * sai_w) / lambda_w.Power(2)).IfNegativeReturnOne().TakeMinWithOne();
            var ae = row_w * a_prime;
            var Ae = t * (2 * be + 2 * ce + ae);
            var items = new List<ReportItem>()
            {
                new ReportItem("Kw",Kw.ToString("0.###"),Units.NONE),
                new ReportItem("Effective Height (ae)",ae.ToString("0.###"),Units.CM),
                new ReportItem("Kf",Kf.ToString("0.###"),Units.NONE),
                new ReportItem("Effective Width (be)",be.ToString("0.###"),Units.CM),
                new ReportItem("Kf",Kc.ToString("0.###"),Units.NONE),
                new ReportItem("Effective Lip (ce)",ce.ToString("0.###"),Units.CM),
                new ReportItem("Effective Area (Ae)",Ae.ToString("0.###"),Units.CM_2),
            };
            return Tuple.Create(Ae, items);
        }

        private static Tuple<double, List<ReportItem>> GetEgyptReducedArea(this UnStiffenedCSection section, Material material)
        {
            var E = material.E;
            var Fy = material.Fy;
            var C = section.Dimensions.TotalFoldWidthC;
            var b = section.Properties.BSmall;
            var t = section.Dimensions.ThicknessT;
            var a_prime = section.Properties.APrime;
            var b_prime = section.Properties.BPrime;
            var c_prime = section.Properties.CPrime;
            var c = section.Properties.CSmall;

            var Kf = 0.43;
            var sai_f = 1.0;
            var lambda_f = ((b_prime / t) / 59) * Math.Sqrt(Fy / Kf);
            var row_f = ((lambda_f - 0.15 - 0.05 * sai_f) / (lambda_f.Power(2))).IfNegativeReturnOne().TakeMinWithOne();
            var be = row_f * b_prime;

            var sai_w = 1.0;
            var Kw = 4.0;
            var lambda_w = ((a_prime / t) / 44) * Math.Sqrt(Fy / Kw);
            var row_w = ((1.1 * lambda_w - 0.16 - 0.1 * sai_w) / lambda_w.Power(2)).IfNegativeReturnOne().TakeMinWithOne();
            var ae = row_w * a_prime;
            var Ae = t * (2 * be + ae);
            var items = new List<ReportItem>()
            {
                new ReportItem("Kw",Kw.ToString("0.###"),Units.NONE),
                new ReportItem("Effective Height (ae)",ae.ToString("0.###"),Units.CM),
                new ReportItem("Kf",Kf.ToString("0.###"),Units.NONE),
                new ReportItem("Effective Flange Width (be)",be.ToString("0.###"),Units.CM),
                new ReportItem("Effective Area (Ae)",Ae.ToString("0.###"),Units.CM_2),
            };
            return Tuple.Create(Ae, items);
        }

        private static double GetEgyptReducedAreaEE(this LippedCSection section, Material material)
        {
            var a_prime = section.Properties.APrime;
            var b_prime = section.Properties.BPrime;
            var c_prime = section.Properties.CPrime;
            var t = section.Dimensions.ThicknessT;
            var E = material.E;
            var Fy = material.Fy;

            //Web
            var E_over_Fy_sqrt = Math.Sqrt(E / Fy);
            var a_over_t = a_prime / t;
            var a_ee = (1.92 * t * E_over_Fy_sqrt * (1 - 0.385 * (E_over_Fy_sqrt / a_over_t))).IfNegativeReturn(a_prime).TakeMin(a_prime);

            //Flange 
            var b_over_t = b_prime / t;
            var b_ee = (1.92 * t * E_over_Fy_sqrt * (1 - 0.385 * (E_over_Fy_sqrt / b_over_t))).IfNegativeReturn(b_prime).TakeMin(b_prime);

            //Lip
            var c_over_t = c_prime / t;
            var c_ee = (0.78 * t * E_over_Fy_sqrt * (1 - (0.13 / c_over_t) * (E_over_Fy_sqrt))).IfNegativeReturn(c_prime).TakeMin(c_prime);

            var A_ee = t * (a_ee + 2 * b_ee + 2 * c_ee);
            return A_ee;
        }

        private static double GetEgyptReducedAreaEE(this UnStiffenedCSection section, Material material)
        {
            var a_prime = section.Properties.APrime;
            var b_prime = section.Properties.BPrime;
            var t = section.Dimensions.ThicknessT;
            var E = material.E;
            var Fy = material.Fy;

            //Web
            var E_over_Fy_sqrt = Math.Sqrt(E / Fy);
            var a_over_t = a_prime / t;
            var a_ee = (1.92 * t * E_over_Fy_sqrt * (1 - 0.385 * (E_over_Fy_sqrt / a_over_t))).IfNegativeReturn(a_prime).TakeMin(a_prime);

            //Flange
            var b_ee = (0.78 * t * E_over_Fy_sqrt * (1 - (0.13 / (b_prime / t)) * E_over_Fy_sqrt)).IfNegativeReturn(b_prime).TakeMin(b_prime);


            var A_ee = t * (a_ee + 2 * b_ee);
            return A_ee;
        }

        private static Tuple<double, List<ReportItem>> GetEgyptCompressionLBResistance(this LippedCSection section, Material material)
        {
            (var Ae, var items) = section.GetEgyptReducedArea(material);
            var Pn = Ae * material.Fy;
            items.Add(new ReportItem("Yield Stress (Fy)", material.Fy.ToString("0.###"), Units.TON_CM_2));
            items.Add(new ReportItem("Nominal Load (Pn)", Pn.ToString("0.###"), Units.TON));
            return Tuple.Create(Pn, items);
        }

        private static Tuple<double, List<ReportItem>> GetEgyptCompressionLBResistance(this UnStiffenedCSection section, Material material)
        {
            (var Ae, var items) = section.GetEgyptReducedArea(material);
            var Pn = Ae * material.Fy;
            items.Add(new ReportItem("Yield Stress (Fy)", material.Fy.ToString("0.###"), Units.TON_CM_2));
            items.Add(new ReportItem("Nominal Load (Pn)", Pn.ToString("0.###"), Units.TON));
            return Tuple.Create(Pn, items);
        }

        private static Tuple<double, List<ReportItem>> GetEgyptCompressionFBResistance(this Section section, Material material, LengthBracingConditions bracingConditions, double Aee)
        {
            var A = section.Properties.A;
            var ix = section.Properties.Rx;
            var iy = section.Properties.Ry;
            var E = material.E;
            var Fy = material.Fy;
            var Kx = bracingConditions.Kx;
            var Ky = bracingConditions.Ky;
            var Lx = bracingConditions.Lx;
            var Ly = bracingConditions.Ly;

            var Q = Aee / A;
            var Fex = (Math.PI.Power(2) * E) / ((Kx * Lx) / (ix)).Power(2);
            var Fey = (Math.PI.Power(2) * E) / ((Ky * Ly) / (iy)).Power(2);

            var Fe = Math.Min(Fex, Fey);
            var lambda_c = Math.Sqrt(Fy / Fe);
            var Fcr = 0.648 * (Fy / lambda_c.Power(2));
            if (lambda_c * Math.Sqrt(Q) <= 1.1)
                Fcr = Fy * Q * (1 - 0.384 * Q * lambda_c.Power(2));
            var pn = Fcr * A;
            var items = new List<ReportItem>()
            {
                new ReportItem("Flexural Stress (Fcr)",Fcr.ToString("0.###"),Units.TON_CM_2),
                new ReportItem("Flexural Area (A)",A.ToString("0.###"),Units.CM_2),
                new ReportItem("Nominal Load (Pn)",Fcr.ToString("0.###"),Units.TON),

            };
            return Tuple.Create(pn, items);
        }


        private static Tuple<double, List<ReportItem>> GetEgyptCompressionTFBResistance(this Section section, Material material, LengthBracingConditions bracingConditions, double Aee)
        {
            var Xo = section.Properties.Xo;
            var ix = section.Properties.Rx;
            var iy = section.Properties.Ry;
            var A = section.Properties.A;
            var Cw = section.Properties.Cw;
            var J = section.Properties.J;
            var E = material.E;
            var Fy = material.Fy;
            var G = material.G;
            var Kx = bracingConditions.Kx;
            var Kz = bracingConditions.Kz;
            var Lx = bracingConditions.Lx;
            var Lz = bracingConditions.Lz;

            var ro_squared = Xo.Power(2) + ix.Power(2) + iy.Power(2);
            var beta = 1 - (Xo.Power(2) / ro_squared);
            var Fez = (((Math.PI.Power(2) * E * Cw) / (Kz * Lz).Power(2)) + G * J) * (1 / (A * ro_squared));
            var Fex = (Math.PI.Power(2) * E) / ((Kx * Lx) / (ix)).Power(2);

            var Fe = ((Fex + Fez) / (2 * beta)) * (1 - Math.Sqrt(1 - ((4 * beta * Fex * Fez) / (Fex + Fez).Power(2))));

            var lambda_c = Math.Sqrt(Fy / Fe);
            var Fcr = 0.648 * (Fy / lambda_c.Power(2));
            var Q = Aee / A;
            if (lambda_c * Math.Sqrt(Q) <= 1.1)
                Fcr = Fy * Q * (1 - 0.384 * Q * lambda_c.Power(2));
            var pn = Fcr * A;
            var items = new List<ReportItem>()
            {
                new ReportItem("Torsional Flexural Stress (Fcr)",Fcr.ToString("0.###"),Units.TON_CM_2),
                new ReportItem("Torsional Flexural Area (A)",A.ToString("0.###"),Units.CM_2),
                new ReportItem("Nominal Load (Pn)",pn.ToString("0.###"),Units.TON),
            };
            return Tuple.Create(pn, items);
        }

        private static Tuple<double, List<ReportItem>> GetEgyptCompressionTBResistance(this Section section, Material material, LengthBracingConditions bracingConditions, double Aee)
        {
            var Xo = section.Properties.Xo;
            var ix = section.Properties.Rx;
            var iy = section.Properties.Ry;
            var A = section.Properties.A;
            var Cw = section.Properties.Cw;
            var J = section.Properties.J;
            var E = material.E;
            var Fy = material.Fy;
            var G = material.G;
            var Kx = bracingConditions.Kx;
            var Kz = bracingConditions.Kz;
            var Lx = bracingConditions.Lx;
            var Lz = bracingConditions.Lz;

            var ro_squared = Xo.Power(2) + ix.Power(2) + iy.Power(2);
            var beta = 1 - (Xo.Power(2) / ro_squared);
            var Fez = (((Math.PI.Power(2) * E * Cw) / (Kz * Lz).Power(2)) + G * J) * (1 / (A * ro_squared));
            var Fet = Fez;

            var Q = Aee / A;
            var lambda_ct = Math.Sqrt(Fy / Fet);
            var Fcrt = 0.0;
            if (lambda_ct * Math.Sqrt(Q) <= 1.1)
            {
                Fcrt = Fy * Q * (1 - 0.384 * Q * lambda_ct.Power(2));
            }
            else
            {
                Fcrt = 0.648 * (Fy / lambda_ct.Power(2));
            }
            var pn = Fcrt * A;
            var items = new List<ReportItem>()
            {
                new ReportItem("Torsional Buckling Stress (Fcrt)",Fcrt.ToString("0.###"),Units.TON_CM_2),
                new ReportItem("Torsional  Area (A)",A.ToString("0.###"),Units.CM_2),
                new ReportItem("Nominal Load (Pn)",pn.ToString("0.###"),Units.TON),
            };
            return Tuple.Create(pn, items);
        }

        public static CompressionResistanceOutput AsEgyptCompressionResistance(this LippedCSection section, Material material, LengthBracingConditions bracingConditions)
        {
            if (!section.IsValidCompression())
                return new CompressionResistanceOutput(0.0, PHI_C, PHI_C_NAME, COMP_DESIGN_RESIST, FailureMode.UNSAFE, "ton", null);
            (var pn_local, var localItems) = section.GetEgyptCompressionLBResistance(material);
            var Aee = section.GetEgyptReducedAreaEE(material);
            (var pn_FB, var fbItems) = section.GetEgyptCompressionFBResistance(material, bracingConditions, Aee);
            (var pn_TFB, var tfbItems) = section.GetEgyptCompressionTFBResistance(material, bracingConditions, Aee);
            (var pn_TB, var tbItems) = section.GetEgyptCompressionTBResistance(material, bracingConditions, Aee);
            var pn1 = Tuple.Create(pn_local, FailureMode.LOCALBUCKLING);
            var pn2 = Tuple.Create(pn_FB, FailureMode.FLEXURALBUCKLING);
            var pn3 = Tuple.Create(pn_TFB, FailureMode.FLEXURAL_TORSIONAL_BUCKLING);
            var pn4 = Tuple.Create(pn_TB, FailureMode.TORSIONALBUCKLING);
            var pns = new List<Tuple<double, FailureMode>>()
            {
                pn1, pn2, pn3,pn4
            };
            var pn = pns.Distinct(NominalStrengthEqualComparer).OrderBy(tuple => tuple.Item1).First();
            var designItems = new List<ReportItem>()
            {
                new ReportItem("Governing Case",pn.Item2.GetDescription(),Units.TON),
                new ReportItem("Nomial Load (Pn)",$"{pn.Item1.ToString("0.###")}",Units.TON),
                new ReportItem("phi",$"{PHI_C}",Units.TON),
                new ReportItem("Design Load (phi*Pn)",$"{(PHI_C*pn.Item1).ToString("0.###")}",Units.TON),
            };
            var secDimsItems = new List<ReportItem>()
            {
                new ReportItem("H",section.Dimensions.TotalHeightH.ToString("0.###"),Units.CM),
                new ReportItem("B",section.Dimensions.TotalFlangeWidthB.ToString("0.###"),Units.CM),
                new ReportItem("R",section.Dimensions.InternalRadiusR.ToString("0.###"),Units.CM),
                new ReportItem("t",section.Dimensions.ThicknessT.ToString("0.###"),Units.CM),
                new ReportItem("C",section.Dimensions.TotalFoldWidthC.ToString("0.###"),Units.CM)
            };
            var secDimSection = new ListReportSection("Section Dimensions", secDimsItems);
            var localSection = new ListReportSection("Local Buckling", localItems);
            var fbSection = new ListReportSection("Flexural Buckling", fbItems);
            var tfbSection = new ListReportSection("Torsional Flexural Buckling", tfbItems);
            var tbSection = new ListReportSection("Torsional Buckling", tbItems);
            var designSection = new ListReportSection("Design Compression Load", designItems);
            var sections = new List<IReportSection> { secDimSection, localSection, fbSection, tfbSection, tbSection };
            var report = new Report(UnitSystems.TONCM, "Egyptian Code - Compression", sections);

            var result = new CompressionResistanceOutput(pn.Item1, PHI_C, PHI_C_NAME, COMP_DESIGN_RESIST, pn.Item2, "ton", report);
            return result;
        }

        public static CompressionResistanceOutput AsEgyptCompressionResistance(this UnStiffenedCSection section, Material material, LengthBracingConditions bracingConditions)
        {
            if (!section.IsValidCompression())
                return new CompressionResistanceOutput(0.0, PHI_C, PHI_C_NAME, COMP_DESIGN_RESIST, FailureMode.UNSAFE, "ton", null);
            (var pn_local, var localItems) = section.GetEgyptCompressionLBResistance(material);
            var Aee = section.GetEgyptReducedAreaEE(material);
            (var pn_FB, var fbItems) = section.GetEgyptCompressionFBResistance(material, bracingConditions, Aee);
            (var pn_TFB, var tfbItems) = section.GetEgyptCompressionTFBResistance(material, bracingConditions, Aee);
            (var pn_TB, var tbItems) = section.GetEgyptCompressionTBResistance(material, bracingConditions, Aee); //TODO: Complete Report.
            var pn1 = Tuple.Create(pn_local, FailureMode.LOCALBUCKLING);
            var pn2 = Tuple.Create(pn_FB, FailureMode.FLEXURALBUCKLING);
            var pn3 = Tuple.Create(pn_TFB, FailureMode.FLEXURAL_TORSIONAL_BUCKLING);
            var pn4 = Tuple.Create(pn_TB, FailureMode.TORSIONALBUCKLING);
            var pns = new List<Tuple<double, FailureMode>>()
            {
                pn1, pn2, pn3,pn4
            };
            var pn = pns.Distinct(NominalStrengthEqualComparer).OrderBy(tuple => tuple.Item1).First();
            var designItems = new List<ReportItem>()
            {
                new ReportItem("Governing Case",pn.Item2.GetDescription(),Units.TON),
                new ReportItem("Nomial Load (Pn)",$"{pn.Item1.ToString("0.###")}",Units.TON),
                new ReportItem("phi",$"{PHI_C}",Units.NONE),
                new ReportItem("Design Load (phi*Pn)",$"{(PHI_C*pn.Item1).ToString("0.###")}",Units.TON),
            };
            var secDimsItems = new List<ReportItem>()
            {
                new ReportItem("H",section.Dimensions.TotalHeightH.ToString("0.###"),Units.CM),
                new ReportItem("B",section.Dimensions.TotalFlangeWidthB.ToString("0.###"),Units.CM),
                new ReportItem("R",section.Dimensions.InternalRadiusR.ToString("0.###"),Units.CM),
                new ReportItem("t",section.Dimensions.ThicknessT.ToString("0.###"),Units.CM),
            };
            var secDimSection = new ListReportSection("Section Dimensions", secDimsItems);
            var localSection = new ListReportSection("Local Buckling", localItems);
            var fbSection = new ListReportSection("Flexural Buckling", fbItems);
            var tfbSection = new ListReportSection("Torsional Flexural Buckling", tfbItems);
            var tbSection = new ListReportSection("Torsional Buckling", tbItems);
            var designSection = new ListReportSection("Design Compression Load", designItems);

            var sections = new List<IReportSection>() { secDimSection, localSection, fbSection, tfbSection, tbSection, designSection };

            var report = new Report(UnitSystems.TONCM, "Egyptian Code - Compression", sections);


            var result = new CompressionResistanceOutput(pn.Item1, PHI_C, PHI_C_NAME, COMP_DESIGN_RESIST, pn.Item2, "ton", report);
            return result;
        }

        #endregion

    }
}
