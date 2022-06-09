using ColdFormedChannelSection.Core.Entities;
using ColdFormedChannelSection.Core.Enums;
using ColdFormedChannelSection.Core.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ColdFormedChannelSection.Core.Helpers
{
    public static class AmericanHelper
    {

        #region Moment & Compression

        private static ResistanceInteractionOutput AsAISIInteractionResistance(this Section section, Material material, LengthBracingConditions bracingconditions, double pu, double mu, CompressionResistanceOutput pn_out, MomentResistanceOutput mn_out, Func<double> getAe,double phi_b)
        {
            var pn = pn_out.NominalResistance;
            var mn = mn_out.NominalResistance;
            var phi_c = 0.85;
            var E = material.E;
            var Fy = material.Fy;
            var Ix = section.Properties.Ix;
            var Kx = bracingconditions.Kx;
            var Lx = bracingconditions.Lx;
            var Cm = bracingconditions.Cm;

            var Pex = (Math.PI.Power(2) * E * Ix) / (Kx * Lx).Power(2);
            var alpha_x = 1 - (pu / Pex);

            var loadRatio = (pu / (phi_c * pn));
            var ieName = "";
            var ie = 0.0;
            if (loadRatio <= 0.15)
            {
                ie = (pu / (phi_c * pn)) + (mu / (phi_b * mn));
                //tex:
                //$$ \frac {P_u}{\phi_c P_n} + \frac {M_u} {\phi_b M_n} $$
                ieName = "\\frac {P_u}{\\phi_c P_n} + \\frac {M_u} {\\phi_b M_n}";
            }
            else
            {
                var ie_1 = (pu / (phi_c * pn)) + ((Cm * mu) / (phi_b * mn * alpha_x));
                var Ae = getAe();
                var Pno = Ae * Fy;
                var ie_2 = (pu / (phi_c * Pno)) + (mu / (phi_b * mn));
                ie = Math.Max(ie_1, ie_2);
                if (ie_1 > ie_2)
                {
                    //tex:
                    //$$ \frac {P_u} {\phi_c P_n} + \frac {C_m M_u} {\phi_b M_n \alpha_x} $$
                    ieName = "\\frac {P_u} {\\phi_c P_n} + \\frac {C_m M_u} {\\phi_b M_n \\alpha_x}";
                }
                else
                {
                    //tex:
                    //$$ \frac {P_u} {\phi_c P_{no}} + \frac {M_u} {\phi_b M_n} $$
                    ieName = "\\frac {P_u} {\\phi_c P_{no}} + \\frac {M_u} {\\phi_b M_n}";
                }
            }
            var report = new InteractionReport(pn_out.Report, mn_out.Report,UnitSystems.KIPINCH);
            return new ResistanceInteractionOutput(pu, pn, mu, mn, ieName, ie, "kip.in", "kip",report);
        }

        public static ResistanceInteractionOutput AsAISIInteractionResistance(this LippedSection section, Material material, LengthBracingConditions bracingConditions, double pu, double mu)
        {
            var Pn = section.AsAISICompressionResistance(material, bracingConditions);
            var Mn = section.AsAISIMomentResistance(material, bracingConditions);

            return section.AsAISIInteractionResistance(material, bracingConditions, pu, mu, Pn, Mn, () => section.GetAISIReducedArea(material).Item1,0.95);
        }

        public static ResistanceInteractionOutput AsAISIInteractionResistance(this UnStiffenedSection section, Material material, LengthBracingConditions bracingConditions, double pu, double mu)
        {
            var Pn = section.AsAISICompressionResistance(material, bracingConditions);
            var Mn = section.AsAISIMomentResistance(material, bracingConditions);
            return section.AsAISIInteractionResistance(material, bracingConditions, pu, mu, Pn, Mn, () => section.GetAISIReducedArea(material).Item1,0.9);
        }

        #endregion

        public static Tuple<double, List<ReportItem>> GetAISIReducedArea(this LippedSection lippedSection, Material material, double F_ = 0)
        {
            var b = lippedSection.Properties.BSmall;
            var t = lippedSection.Dimensions.ThicknessT;
            var C = lippedSection.Dimensions.TotalFoldWidthC;
            var c = lippedSection.Properties.CSmall;
            var a = lippedSection.Properties.ADimension;
            var u = lippedSection.Properties.U;
            var alpha = lippedSection.Properties.Alpha;
            var E = material.E;
            var v = material.V;
            var Fy = F_ == 0 ? material.Fy : F_;
            var s = 1.28 * Math.Sqrt(E / Fy);
            var be = lippedSection.Properties.BSmall;
            var Ce = c;
            var ae = lippedSection.Properties.ADimension;
            var b_over_t = b / t;
            var e_over_v_term = (Math.PI.Power(2) * E) / (12 * (1 - v.Power(2)));
            var Kc = 0.0;
            var Kf = 0.0;
            if (b_over_t > (s / 3))
            {
                var Ia_1 = 399 * t.Power(4) * ((b_over_t / s) - 0.328).Power(3);
                var Ia_2 = t.Power(4) * (115 * (b_over_t / s) + 5);
                var Ia = Math.Min(Ia_1, Ia_2);
                var Is = (c.Power(3) * t) / 12;
                var Ri = Math.Min(Is / Ia, 1);
                var n_1 = (0.582 - (b_over_t / (4 * s)));
                var n_2 = (1.0 / 3.0);
                var n = Math.Max(n_1, n_2);

                //Flange.
                Kf = Math.Min(4.0, (4.82 - ((5 * C) / (b))) * Ri.Power(n) + 0.43);
                var Fcr_f = Kf * e_over_v_term * (t / b).Power(2);
                var lambda_f = Math.Sqrt(Fy / Fcr_f);
                var row_f = Math.Min(1.0, (1 - (0.22 / lambda_f)) / (lambda_f));
                if (lambda_f > 0.673)
                    be = row_f * b;

                //Lip.
                Kc = 0.43;
                var Fcr_c = Kc * e_over_v_term * (t / c).Power(2);
                var lambda_c = Math.Sqrt(Fy / Fcr_c);
                var row_c = Math.Min(1.0, (1 - (0.22 / lambda_c)) / (lambda_c));
                if (lambda_c <= 0.673)
                    Ce = c * Ri;
                else
                    Ce = row_c * Ri * c;
            }
            //Web.
            var kw = 4.0;
            var Fcr_w = kw * e_over_v_term * (t / a).Power(2);
            var lambda_w = Math.Sqrt(Fy / Fcr_w);
            var row_w = Math.Min(1.0, (1 - (0.22 / lambda_w)) / lambda_w);
            if (lambda_w > 0.673)
                ae = row_w * a;
            var Ae = t * (2 * Ce + 2 * be + ae + 2 * u * (1 + alpha));
            var items = new List<ReportItem>()
            {
                new ReportItem("Kw",kw.ToString("0.###"),Units.NONE),
                new ReportItem("Effective Height (ae)",ae.ToString("0.###"),Units.IN),
                new ReportItem("Kf",Kf.ToString("0.###"),Units.NONE),
                new ReportItem("Effective Flange Width (be)",be.ToString("0.###"),Units.IN),
                new ReportItem("Kc",Kc.ToString("0.###"),Units.NONE),
                new ReportItem("Effective Lip (ce)",Ce.ToString("0.###"),Units.IN),
                new ReportItem("Effective Area (Ae)",Ae.ToString("0.###"),Units.IN_2),
                new ReportItem("Yield Stress (Fy)",material.Fy.ToString("0.###"),Units.IN_2),
            };
            return Tuple.Create(Ae, items);
        }

        public static Tuple<double, List<ReportItem>> GetAISIReducedArea(this UnStiffenedSection unstiffenedSection, Material material, double F_ = 0)
        {
            var b = unstiffenedSection.Properties.BSmall;
            var t = unstiffenedSection.Dimensions.ThicknessT;
            var a = unstiffenedSection.Properties.ADimension;
            var u = unstiffenedSection.Properties.U;
            var alpha = unstiffenedSection.Properties.Alpha;
            var E = material.E;
            var v = material.V;
            var Fy = F_ == 0 ? material.Fy : F_;
            var be = b;
            var ae = unstiffenedSection.Properties.ADimension;
            var e_over_v_term = (Math.PI.Power(2) * E) / (12 * (1 - v.Power(2)));

            //Flange.
            var Kf = 0.43;
            var Fcr_f = Kf * e_over_v_term * (t / b).Power(2);
            var lambda_f = Math.Sqrt(Fy / Fcr_f);
            var row_f = Math.Min(1.0, ((1 - (0.22 / lambda_f)) / lambda_f));
            if (lambda_f > 0.673)
                be = row_f * b;

            //Web.
            var kw = 4.0;
            var Fcr_w = kw * e_over_v_term * (t / a).Power(2);
            var lambda_w = Math.Sqrt(Fy / Fcr_w);
            var row_w = Math.Min(1.0, (1 - (0.22 / lambda_w)) / lambda_w);
            if (lambda_w > 0.673)
                ae = row_w * a;
            var Ae = t * (2 * be + ae + 2 * u * (1 + alpha));
            var items = new List<ReportItem>()
            {
                new ReportItem("Kw",kw.ToString("0.###"),Units.NONE),
                new ReportItem("Effective Height (ae)",ae.ToString("0.###"),Units.IN),
                new ReportItem("Kf",Kf.ToString("0.###"),Units.NONE),
                new ReportItem("Effective Flange Width (be)",be.ToString("0.###"),Units.IN),
                new ReportItem("Effective Area (Ae)",Ae.ToString("0.###"),Units.IN_2),
                new ReportItem("Yield Stress (Fy)",material.Fy.ToString("0.###"),Units.IN_2),
            };
            return Tuple.Create(Ae, items);
        }


        private static Tuple<double,List<ReportItem>> GetAISIReducedZe(this UnStiffenedSection unstiffenedSection, Material material, double F_ = 0, bool isOneIter = false)
        {
            var b = unstiffenedSection.Properties.BSmall;
            var t = unstiffenedSection.Dimensions.ThicknessT;
            var a = unstiffenedSection.Properties.ADimension;
            var E = material.E;
            var v = material.V;
            var Fy = F_ == 0 ? material.Fy : F_;
            var be = unstiffenedSection.Properties.BSmall;
            var e_over_v_term = (Math.PI.Power(2) * E) / (12 * (1 - v.Power(2)));

            //Flange.
            var kf = 0.43;
            var Fcr_f = kf * e_over_v_term * (t / b).Power(2);
            var lambda_f = Math.Sqrt(Fy / Fcr_f);
            var row_f = Math.Min(1.0, ((1 - (0.22 / lambda_f)) / lambda_f));
            if (lambda_f > 0.673)
                be = row_f * b;

            //Web.
            (var Ze,var items) = unstiffenedSection.GetAISIReducedZe(material, be, 0, F_, isOneIter, 0);
            items.Add(new ReportItem("Kf", kf.ToString("0.###"), Units.NONE));
            items.Add(new ReportItem("Effective Flange Width (be)", be.ToString("0.###"), Units.IN));
            items.Add(new ReportItem("Effective Section Modulus (Ze)", Ze.ToString("0.###"), Units.IN_3));
            items.Add(new ReportItem("Yield stress (F)", Fy.ToString("0.###"), Units.KSI));
            items.Add(new ReportItem("Local Nominal Moment (Mn)", (Fy * Ze).ToString("0.###"), Units.KIP_IN));

            return Tuple.Create(Ze, items);
        }


        private static Tuple<double,List<ReportItem>> GetAISIReducedZe(this LippedSection lippedSection, Material material, double F_ = 0, bool isOneIter = false)
        {
            var b = lippedSection.Properties.BSmall;
            var t = lippedSection.Dimensions.ThicknessT;
            var C = lippedSection.Dimensions.TotalFoldWidthC;
            var H = lippedSection.Dimensions.TotalHeightH;
            var R = lippedSection.Dimensions.InternalRadiusR;
            var c = lippedSection.Properties.CSmall;
            var a = lippedSection.Properties.ADimension;
            var r = lippedSection.Properties.RSmall;
            var u = lippedSection.Properties.U;
            var E = material.E;
            var v = material.V;
            var Fy = F_ == 0 ? material.Fy : F_;
            var s = 1.28 * Math.Sqrt(E / Fy);
            var be = lippedSection.Properties.BSmall;
            var b_over_t = b / t;
            var e_over_v_term = (Math.PI.Power(2) * E) / (12 * (1 - v.Power(2)));
            var Ia_1 = 399 * t.Power(4) * ((b_over_t / s) - 0.328).Power(3);
            var Ia_2 = t.Power(4) * (115 * (b_over_t / s) + 5);
            var Ia = Math.Min(Ia_1, Ia_2);
            var Is = (c.Power(3) * t) / 12;
            var Ri = Math.Min(Is / Ia, 1);
            var Kf = 0.0;
            if (b_over_t > (s / 3))
            {

                var n_1 = (0.582 - (b_over_t / (4 * s)));
                var n_2 = (1.0 / 3.0);
                var n = Math.Max(n_1, n_2);

                //Flange.
                Kf = Math.Min(4.0, (4.82 - ((5 * C) / (b))) * Ri.Power(n) + 0.43);
                var Fcr_f = Kf * e_over_v_term * (t / b).Power(2);
                var lambda_f = Math.Sqrt(Fy / Fcr_f);
                var row_f = Math.Min(1.0, (1 - (0.22 / lambda_f)) / (lambda_f));
                if (lambda_f > 0.673)
                    be = row_f * b;

            }
            //Lip
            var y_bar = H / 2;
            var kc = 0.43;
            var Fcr_c = kc * e_over_v_term * (t / c).Power(2);
            var F1 = Fy * ((y_bar - (t / 2) - r) / y_bar);
            var lambda_c = Math.Sqrt(F1 / Fcr_c);
            var row_c = Math.Min(1.0, ((1 - (0.22 / lambda_c)) / lambda_c));
            var ce = c * Ri;
            if (lambda_c > 0.673)
                ce = row_c * c * Ri;
            (var Ze,var items) = lippedSection.GetAISIReducedZe(material, be, ce, F_, isOneIter, 1);
            items.Add(new ReportItem("Kf",Kf.ToString("0.###"),Units.NONE));
            items.Add(new ReportItem("Effective Flange Width (be)", be.ToString("0.###"), Units.IN));
            items.Add(new ReportItem("Kc", kc.ToString("0.###"), Units.NONE));
            items.Add(new ReportItem("Effective Lip (ce)", ce.ToString("0.###"), Units.IN));
            items.Add(new ReportItem("Effective Section Modulus (Ze)", Ze.ToString("0.###"), Units.IN_3));
            items.Add(new ReportItem("Yield stress (F)", Fy.ToString("0.###"), Units.KSI));
            items.Add(new ReportItem("Local Nominal Moment (Mn)", (Fy*Ze).ToString("0.###"), Units.KIP_IN));

            return Tuple.Create(Ze,items);
        }

        private static Tuple<double,List<ReportItem>> GetAISIReducedZe(this Section section, Material material, double be, double ce, double F_ = 0, bool isOneIter = false, int lipFactor = 0)
        {

            var b = section.Properties.BSmall;
            var B = section.Dimensions.TotalFlangeWidthB;
            var t = section.Dimensions.ThicknessT;
            var C = section.Dimensions.TotalFoldWidthC;
            var H = section.Dimensions.TotalHeightH;
            var R = section.Dimensions.InternalRadiusR;
            var c = section.Properties.CSmall;
            var a = section.Properties.ASmall;
            var r = section.Properties.RSmall;
            var u = section.Properties.U;
            var E = material.E;
            var v = material.V;
            var Fy = F_ == 0 ? material.Fy : F_;
            var s = 1.28 * Math.Sqrt(E / Fy);
            var ae = a;
            var e_over_v_term = (Math.PI.Power(2) * E) / (12 * (1 - v.Power(2)));

            var y_bar = H / 2;
            var new_y_bar = H / 2;
            var sum_Ix = 0.0;
            var sum_L = 0.0;
            var sum_LY = 0.0;
            var sum_LY2 = 0.0;
            var hneg = 0.0;
            var Kw = 0.0;
            do
            {
                y_bar = new_y_bar;
                var F1 = Fy * ((y_bar - (t / 2) - r) / y_bar);

                //Web
                var F2 = Fy * ((H - y_bar - (t / 2) - r) / (y_bar));
                var sai = Math.Abs(F2 / F1);
                Kw = 4 + 2 * (1 + sai).Power(3) + 2 * (1 + sai);
                var Fcr_w = Kw * e_over_v_term * (t / a).Power(2);
                var lambda_W = Math.Sqrt(F1 / Fcr_w);
                var row_w = Math.Min(1, (1 - (0.22 / lambda_W)) / (lambda_W));
                if (lambda_W > 0.673)
                    ae = row_w * a;
                var he1 = (ae) / (3 + sai);
                var he2 = ae - he1;
                if (sai >= 0.236)
                {
                    if ((H / B) <= 4)
                        he2 = ae / 2;
                    else
                        he2 = (ae / (1 + sai)) - he1;
                }
                hneg = y_bar - (t / 2) - r - (he1 + he2);
                var yneg = (t / 2) + r + he1 + (hneg / 2);
                if (hneg <= 0)
                {
                    hneg = 0;
                    yneg = 0;
                }
                var elements = new List<Tuple<double, double, double, int>>()
                {
                    Tuple.Create(be,t/2,0.0,1), //Top Flange
                    Tuple.Create(b,H-t/2,0.0,1),//Bottom Flange
                    Tuple.Create(a,H/2,(a.Power(3)/12),1), //Web
                    Tuple.Create(-hneg,yneg,0.0,1), //Negative web
                    Tuple.Create(u,((1-0.673)*r+(t/2)),0.149 *r.Power(3),1), //Top inside corner
                    Tuple.Create(u,(H-(1-0.673)*r-(t/2)),0.149 *r.Power(3),1), //Bottom inside corner
                     Tuple.Create(u,((1-0.673)*r+(t/2)),0.149 *r.Power(3),lipFactor), //Top outer corner
                    Tuple.Create(u,(H-(1-0.673)*r-(t/2)),0.149 *r.Power(3),lipFactor), //Bottom outer corner
                    Tuple.Create(ce,(R+t+(ce/2)),ce.Power(3)/12,lipFactor), //Top Lip
                    Tuple.Create(c,H-R-t-(c/2),c.Power(3)/12,lipFactor), //Bottom Lip
                };
                sum_L = elements.Sum(tuple => tuple.Item1 * tuple.Item4);
                sum_LY = elements.Aggregate(0.0, (soFar, current) =>
                {
                    var ly = current.Item1 * current.Item2 * current.Item4;
                    soFar += ly;
                    return soFar;
                });
                sum_LY2 = elements.Aggregate(0.0, (soFar, current) =>
                {
                    var ly2 = current.Item1 * current.Item2.Power(2) * current.Item4;
                    soFar += ly2;
                    return soFar;
                });
                sum_Ix = elements.Sum(tuple => tuple.Item3 * tuple.Item4);
                new_y_bar = sum_LY / sum_L;
            } while (Math.Abs(new_y_bar - y_bar) > 0.00002 && !isOneIter);
            var Ixe = t * (sum_Ix + sum_LY2 - new_y_bar.Power(2) * sum_L);
            var Ze = Ixe / new_y_bar;
            var items = new List<ReportItem>()
            {
                new ReportItem("Kw",Kw.ToString("0.###"),Units.NONE),
                new ReportItem("Effective Height (ae)",(ae-hneg).ToString("0.###"),Units.IN),
            };
            return Tuple.Create(Ze,items);
        }


        #region AISI

        #region Compression

        public static CompressionResistanceOutput AsAISICompressionResistance(this LippedSection lippedSection, Material material, LengthBracingConditions bracingConditions)
        {
            if (!lippedSection.IsValidForCompression())
                return new CompressionResistanceOutput(0.0, 0.85, FailureMode.UNSAFE, "Kip",null);
            (var pn_local, var items_local) = lippedSection.GetAISICompressionLBResistance(material);
            (var pn_FB, var items_FB) = lippedSection.GetAISICompressionFBRessistance(material, bracingConditions);
            (var pn_TFB, var items_TFB) = lippedSection.GetAISICompressionFTBRessistance(material, bracingConditions);
            var pn1 = Tuple.Create(pn_local, FailureMode.LOCALBUCKLING);
            var pn2 = Tuple.Create(pn_FB, FailureMode.FLEXURALBUCKLING);
            var pn3 = Tuple.Create(pn_TFB, FailureMode.TORSIONALBUCKLING);
            var pns = new List<Tuple<double, FailureMode>>()
            {
                pn1, pn2, pn3
            };
            var pn = pns.OrderBy(tuple => tuple.Item1).First();
            var items = new List<ReportItem>()
            {
                new ReportItem("Governing Case",pn.Item2.ToString(),Units.NONE),
                new ReportItem("Nominal Load",pn.Item1.ToString("0.###"),Units.KIP),
                new ReportItem("phi",0.85.ToString("0.###"),Units.NONE),
                new ReportItem("Design Resistance",(0.85*pn.Item1).ToString("0.###"),Units.KIP),

            };
            var report = new CompressionReport(
            "AISI Code - Compression",
            "Local Buckling",
             items_local,
             "Flexural Buckling",
             items_FB,
             "Torsional Flexural Buckling",
             items_TFB,
             items,
             UnitSystems.KIPINCH
             );
            var result = new CompressionResistanceOutput(pn.Item1, 0.85, pn.Item2, "Kip",report);
            return result;
        }

        public static CompressionResistanceOutput AsAISICompressionResistance(this UnStiffenedSection unstiffenedSection, Material material, LengthBracingConditions bracingConditions)
        {
            if (!unstiffenedSection.IsValidForCompression())
                return new CompressionResistanceOutput(0.0, 0.85, FailureMode.UNSAFE, "Kip",null);
            (var pn_local, var items_local) = unstiffenedSection.GetAISICompressionLBResistance(material);
            (var pn_FB, var items_FB) = unstiffenedSection.GetAISICompressionFBRessistance(material, bracingConditions);
            (var pn_TFB, var items_TFB) = unstiffenedSection.GetAISICompressionFTBRessistance(material, bracingConditions);
            var pn1 = Tuple.Create(pn_local, FailureMode.LOCALBUCKLING);
            var pn2 = Tuple.Create(pn_FB, FailureMode.FLEXURALBUCKLING);
            var pn3 = Tuple.Create(pn_TFB, FailureMode.TORSIONALBUCKLING);
            var pns = new List<Tuple<double, FailureMode>>()
            {
                pn1, pn2, pn3
            };
            var pn = pns.OrderBy(tuple => tuple.Item1).First();
            var items = new List<ReportItem>()
            {
                new ReportItem("Governing Case",pn.Item2.ToString(),Units.NONE),
                new ReportItem("Nominal Load",pn.Item1.ToString("0.###"),Units.KIP),
                new ReportItem("phi",0.85.ToString("0.###"),Units.NONE),
                new ReportItem("Design Resistance",(0.85*pn.Item1).ToString("0.###"),Units.KIP),

            };
            var report = new CompressionReport(
           "AISI Code - Compression",
           "Local Buckling",
            items_local,
            "Flexural Buckling",
            items_FB,
            "Torsional Flexural Buckling",
            items_TFB,
            items,
            UnitSystems.KIPINCH
            );
            var result = new CompressionResistanceOutput(pn.Item1, 0.85, pn.Item2, "Kip",report);
            return result;
        }

        private static bool IsValidForCompression(this Section section)
        {
            var b_over_t = Tuple.Create(section.Properties.BSmall / section.Dimensions.ThicknessT, 60.0);

            var c_over_t = Tuple.Create(section.Properties.CSmall / section.Dimensions.ThicknessT, 14.0);
            var a_over_t = Tuple.Create(section.Properties.ADimension / section.Dimensions.ThicknessT, 200.0);
            var C_over_b = Tuple.Create(section.Dimensions.TotalFoldWidthC / section.Properties.BSmall, 0.8);

            var allows = new List<Tuple<double, double>>()
            {
                b_over_t,c_over_t,a_over_t,C_over_b
            };
            return !allows.Any(tuple => tuple.Item1 > tuple.Item2);
        }

        private static Tuple<double,List<ReportItem>> GetAISICompressionLBResistance(this LippedSection section, Material material)
        {
            (var Ae, var items) = section.GetAISIReducedArea(material);
            var pn = Ae * material.Fy;
            items.Add(new ReportItem("Nominal Load", pn.ToString("0.###"), Units.KIP));
            return Tuple.Create(pn, items);
        }

        private static Tuple<double,List<ReportItem>> GetAISICompressionLBResistance(this UnStiffenedSection section, Material material)
        {
            (var Ae , var items) = section.GetAISIReducedArea(material);
            var pn = Ae* material.Fy;
            items.Add(new ReportItem("Pn",pn.ToString("0.###"),Units.KIP));
            return Tuple.Create(pn, items);
        }

        private static double GetAISICompressionFBStress(this Section section, Material material, LengthBracingConditions bracingConditions)
        {
            var kx = bracingConditions.Kx;
            var ky = bracingConditions.Ky;
            var lx = bracingConditions.Lx;
            var ly = bracingConditions.Ly;
            var ix = section.Properties.Rx;
            var iy = section.Properties.Ry;
            var E = material.E;
            var Fy = material.Fy;

            var sigma_ex = (Math.PI.Power(2) * E) / ((kx * lx) / ix).Power(2);
            var sigma_ey = (Math.PI.Power(2) * E) / ((ky * ly) / iy).Power(2);
            var Fe = Math.Min(sigma_ex, sigma_ey);
            var lambda_1 = Math.Sqrt(Fy / Fe);
            if (lambda_1 <= 1.5)
            {
                var lambda_squared = lambda_1.Power(2);
                var F1 = (0.658.Power(lambda_squared)) * Fy;
                return F1;
            }
            else
            {
                var F1 = (0.877 / lambda_1.Power(2)) * Fy;
                return F1;
            }
        }

        private static Tuple<double,List<ReportItem>> GetAISICompressionFBRessistance(this LippedSection lippedSection, Material material, LengthBracingConditions bracingConditions)
        {
            var F1 = lippedSection.GetAISICompressionFBStress(material, bracingConditions);
            (var A1,var _) = lippedSection.GetAISIReducedArea(material, F1);
            var Pn = F1 * A1;
            var items = new List<ReportItem>()
            {
                new ReportItem("Flexural Stress (F1)",F1.ToString("0.###"),Units.KSI),
                new ReportItem("Area (A1)",A1.ToString("0.###"),Units.IN_2),
                new ReportItem("Flexural Nominal Load (Pn)",Pn.ToString("0.###"),Units.KIP),
            };
            return Tuple.Create(Pn,items);
        }

        private static Tuple<double,List<ReportItem>> GetAISICompressionFBRessistance(this UnStiffenedSection unstiffenedSection, Material material, LengthBracingConditions bracingConditions)
        {
            var F1 = unstiffenedSection.GetAISICompressionFBStress(material, bracingConditions);
            (var A1,var _) = unstiffenedSection.GetAISIReducedArea(material, F1);
            var Pn = F1 * A1;
            var items = new List<ReportItem>()
            {
                new ReportItem("Flexural Stress (F1)",F1.ToString("0.###"),Units.KSI),
                new ReportItem("Area (A1)",A1.ToString("0.###"),Units.IN_2),
                new ReportItem("Flexural Nominal Load (Pn)",Pn.ToString("0.###"),Units.KIP),
            };
            return Tuple.Create(Pn, items);
        }


        private static double GetAISICompressionFTBStress(this Section section, Material material, LengthBracingConditions bracingConditions)
        {
            var kx = bracingConditions.Kx;
            var kz = bracingConditions.Kz;
            var lx = bracingConditions.Lx;
            var lz = bracingConditions.Lz;
            var ix = section.Properties.Rx;
            var iy = section.Properties.Ry;
            var A = section.Properties.A;
            var Cw = section.Properties.Cw;
            var xo_squared = section.Properties.Xo.Power(2);
            var J = section.Properties.J;
            var E = material.E;
            var Fy = material.Fy;
            var G = material.G;

            var io_squared = ix.Power(2) + iy.Power(2) + xo_squared;
            var beta = 1 - (xo_squared / io_squared);

            var sigma_ez = (1 / (A * io_squared)) * (((Math.PI.Power(2) * E * Cw) / (kz * lz).Power(2)) + G * J);
            var sigma_ex = (Math.PI.Power(2) * E) / ((kx * lx) / ix).Power(2);
            var Fe2 = (1 / (2.0 * beta)) * ((sigma_ex + sigma_ez) - Math.Sqrt((sigma_ex + sigma_ez).Power(2) - 4 * beta * sigma_ez * sigma_ex));
            var lambda_2 = Math.Sqrt(Fy / Fe2);
            var lambda_squared = lambda_2.Power(2);
            if (lambda_2 <= 1.5)
            {
                var F2 = (0.658.Power(lambda_squared)) * Fy;
                return F2;
            }
            else
            {
                var F2 = (0.877 / lambda_squared) * Fy;
                return F2;
            }
        }

        private static Tuple<double,List<ReportItem>> GetAISICompressionFTBRessistance(this LippedSection lippedSection, Material material, LengthBracingConditions bracingConditions)
        {
            var F2 = lippedSection.GetAISICompressionFTBStress(material, bracingConditions);
            (var A2,var _) = lippedSection.GetAISIReducedArea(material, F2);
            var Pn = F2 * A2;
            var items = new List<ReportItem>()
            {
                new ReportItem("Torsional Flexural Stress (F2)",F2.ToString("0.###"),Units.KSI),
                new ReportItem("Area (A2)",A2.ToString("0.###"),Units.IN_2),
                new ReportItem("Torsional Flexural Nominal Load (Pn)",Pn.ToString("0.###"),Units.KIP),
            };
            return Tuple.Create(Pn, items);
        }

        private static Tuple<double, List<ReportItem>> GetAISICompressionFTBRessistance(this UnStiffenedSection unstiffenedSection, Material material, LengthBracingConditions bracingConditions)
        {
            var F2 = unstiffenedSection.GetAISICompressionFTBStress(material, bracingConditions);
            (var A2,var _) = unstiffenedSection.GetAISIReducedArea(material, F2);
            var Pn = F2 * A2;
            var items = new List<ReportItem>()
            {
                new ReportItem("Flexural Stress (F1)",F2.ToString("0.###"),Units.KSI),
                new ReportItem("Area (A1)",A2.ToString("0.###"),Units.IN_2),
                new ReportItem("Flexural Nominal Load (Pn)",Pn.ToString("0.###"),Units.KIP),
            };
            return Tuple.Create(Pn, items);
        }

        #endregion

        #region Moment

        public static MomentResistanceOutput AsAISIMomentResistance(this LippedSection lippedSection, Material material, LengthBracingConditions bracingConditions)
        {
            if (!lippedSection.IsValidForCompression())
                return new MomentResistanceOutput(0.0, 0.95, FailureMode.UNSAFE, "Kip",null);
            (var Mn_local, var items_local) = lippedSection.GetAISIMomentLBResistance(material);
            var Mn1 = Tuple.Create(Mn_local, FailureMode.LOCALBUCKLING);
            (var Mn_ltb, var items_ltb) = lippedSection.GetAISIMomentLTBRessistance(material, bracingConditions);
            var Mn2 = Tuple.Create(Mn_ltb, FailureMode.LATERALTORSIONALBUCKLING);
            var Mns = new List<Tuple<double, FailureMode>>()
            {
                Mn1,Mn2
            };
            var Mn = Mns.OrderBy(tuple => tuple.Item1).First();
            var items_nominal = new List<ReportItem>()
            {
                new ReportItem("Governing Case",Mn.Item2.ToString(),Units.NONE),
                new ReportItem("Nominal Moment (Mn)",Mn.Item1.ToString("0.###"),Units.KIP_IN),
                new ReportItem("phi",0.95.ToString("0.###"),Units.NONE),
                new ReportItem("Design Moment (phi * Mn)",(0.95*Mn.Item1).ToString("0.###"),Units.KIP_IN)
            };
            var report = new MomentReport(
                "AISI Code - Moment",
                "Local Buckling",
                items_local,
                "Lateral Torsional Buckling",
                items_ltb,
                items_nominal,
                UnitSystems.KIPINCH
                );
            var result = new MomentResistanceOutput(Mn.Item1, 0.95, Mn.Item2, "Kip",report);
            return result;
        }

        public static MomentResistanceOutput AsAISIMomentResistance(this UnStiffenedSection unstiffenedSection, Material material, LengthBracingConditions bracingConditions)
        {
            if (!unstiffenedSection.IsValidForCompression())
                return new MomentResistanceOutput(0.0, 0.9, FailureMode.UNSAFE, "Kip",null);
            (var Mn_local, var items_local) = unstiffenedSection.GetAISIMomentLBResistance(material);
            var Mn1 = Tuple.Create(Mn_local, FailureMode.LOCALBUCKLING);
            (var Mn_ltb, var items_ltb) = unstiffenedSection.GetAISIMomentLTBRessistance(material, bracingConditions);
            var Mn2 = Tuple.Create(Mn_ltb, FailureMode.LATERALTORSIONALBUCKLING);
            var Mns = new List<Tuple<double, FailureMode>>()
            {
                Mn1,Mn2
            };
            var Mn = Mns.OrderBy(tuple => tuple.Item1).First();
            var items_nominal = new List<ReportItem>()
            {
                new ReportItem("Governing Case",Mn.Item2.ToString(),Units.NONE),
                new ReportItem("Nominal Moment (Mn)",Mn.Item1.ToString("0.###"),Units.KIP_IN),
                new ReportItem("phi",0.9.ToString("0.###"),Units.NONE),
                new ReportItem("Design Moment (phi * Mn)",(0.9*Mn.Item1).ToString("0.###"),Units.KIP_IN)
            };
            var report = new MomentReport(
               "AISI Code - Moment",
               "Local Buckling",
               items_local,
               "Lateral Torsional Buckling",
               items_ltb,
               items_nominal,
               UnitSystems.KIPINCH
               );
            var result = new MomentResistanceOutput(Mn.Item1, 0.9, Mn.Item2, "Kip",report);
            return result;
        }

        private static Tuple<double,List<ReportItem>> GetAISIMomentLBResistance(this LippedSection section, Material material)
        {
           (var Ze , var items)= section.GetAISIReducedZe(material);
            var Mn = Ze * material.Fy;
            return Tuple.Create(Mn, items);
        }

        private static Tuple<double, List<ReportItem>> GetAISIMomentLBResistance(this UnStiffenedSection section, Material material)
        {
            (var Ze, var items) = section.GetAISIReducedZe(material);
            var Mn = Ze * material.Fy;
            return Tuple.Create(Mn, items);
        }

        private static double GetAISIMomentLTBStress(this Section section, Material material, LengthBracingConditions bracingConditions)
        {
            var E = material.E;
            var G = material.G;
            var Fy = material.Fy;
            var ky = bracingConditions.Ky;
            var Kz = bracingConditions.Kz;
            var ly = bracingConditions.Ly;
            var lz = bracingConditions.Lz;
            var Cb = bracingConditions.Cb;
            var iy = section.Properties.Ry;
            var ix = section.Properties.Rx;
            var xo = section.Properties.Xo;
            var A = section.Properties.A;
            var J = section.Properties.J;
            var Cw = section.Properties.Cw;
            var Zg = section.Properties.Zg;
            var Fey = (Math.PI.Power(2) * E) / ((ky * ly) / (iy)).Power(2);
            var ro_squared = ix.Power(2) + iy.Power(2) + xo.Power(2);
            var Fez = (1 / (A * ro_squared)) * (((Math.PI.Power(2) * E * Cw) / (Kz * lz).Power(2)) + (G * J));
            var ro = Math.Sqrt(ro_squared);
            var Fe = ((Cb * ro * A) / Zg) * Math.Sqrt(Fey * Fez);
            var Fltb = 0.0;
            if (Fe >= 2.78 * Fy)
            {
                Fltb = Fy;
            }
            else if (Fe <= 0.56 * Fy)
            {
                Fltb = Fe;
            }
            else
            {
                Fltb = (10 / 9) * Fy * (1 - ((10 * Fy) / (36 * Fe)));
            }
            return Fltb;
        }


        private static Tuple<double,List<ReportItem>> GetAISIMomentLTBRessistance(this LippedSection lippedSection, Material material, LengthBracingConditions bracingConditions)
        {
            var fltb = lippedSection.GetAISIMomentLTBStress(material, bracingConditions);
            (var Zf,var _) = lippedSection.GetAISIReducedZe(material, fltb, true);
            var Mn = fltb * Zf;
            var items = new List<ReportItem>()
            {
                new ReportItem("Lateral Torsional Section Modulus (Zf)",Zf.ToString("0.###"),Units.IN_3),
                new ReportItem("Lateral Torsional Stress (Zf)",fltb.ToString("0.###"),Units.KSI),
                new ReportItem("Lateral Torsional Nominal Moment (Zf)",Mn.ToString("0.###"),Units.KIP_IN),
            };
            return Tuple.Create(Mn, items);
        }

        private static Tuple<double,List<ReportItem>> GetAISIMomentLTBRessistance(this UnStiffenedSection unStiffenedSection, Material material, LengthBracingConditions bracingConditions)
        {
            var fltb = unStiffenedSection.GetAISIMomentLTBStress(material, bracingConditions);
            (var Zf,var _) = unStiffenedSection.GetAISIReducedZe(material, fltb, true);
            var Mn = fltb * Zf;
            var items = new List<ReportItem>()
            {
                new ReportItem("Lateral Torsional Section Modulus (Zf)",Zf.ToString("0.###"),Units.IN_3),
                new ReportItem("Lateral Torsional Stress (Zf)",fltb.ToString("0.###"),Units.KSI),
                new ReportItem("Lateral Torsional Nominal Moment (Zf)",Mn.ToString("0.###"),Units.KIP_IN),
            };
            return Tuple.Create(Mn,items);
        }

        #endregion

        #endregion


        //#region AISC


        //#region Compression

        //public static CompressionResistanceOutput AsAISCCompressionResistance(this LippedSection lippedSection, Material material, LengthBracingConditions bracingConditions)
        //{
        //    if (!lippedSection.IsValidForCompression())
        //        return new CompressionResistanceOutput(0.0, 0.9, FailureMode.UNSAFE, "Kip");
        //    var pn1 = Tuple.Create(lippedSection.GetAISCCompressionLBResistance(material), FailureMode.LOCALBUCKLING);
        //    var pn2 = Tuple.Create(lippedSection.GetAISCCompressionFBRessistance(material, bracingConditions), FailureMode.FLEXURALBUCKLING);
        //    var pn3 = Tuple.Create(lippedSection.GetAISCCompressionFTBRessistance(material, bracingConditions), FailureMode.TORSIONALBUCKLING);
        //    var pns = new List<Tuple<double, FailureMode>>()
        //    {
        //        pn1, pn2, pn3
        //    };
        //    var pn = pns.OrderBy(tuple => tuple.Item1).First();
        //    var result = new CompressionResistanceOutput(pn.Item1, 0.9, pn.Item2, "Kip");
        //    return result;
        //}

        //public static CompressionResistanceOutput AsAISCCompressionResistance(this UnStiffenedSection unstiffenedSection, Material material, LengthBracingConditions bracingConditions)
        //{
        //    if (!unstiffenedSection.IsValidForCompression())
        //        return new CompressionResistanceOutput(0.0, 0.9, FailureMode.UNSAFE, "Kip");
        //    var pn1 = Tuple.Create(unstiffenedSection.GetAISCCompressionLBResistance(material), FailureMode.LOCALBUCKLING);
        //    var pn2 = Tuple.Create(unstiffenedSection.GetAISCCompressionFBRessistance(material, bracingConditions), FailureMode.FLEXURALBUCKLING);
        //    var pn3 = Tuple.Create(unstiffenedSection.GetAISCCompressionFTBRessistance(material, bracingConditions), FailureMode.TORSIONALBUCKLING);
        //    var pns = new List<Tuple<double, FailureMode>>()
        //    {
        //        pn1, pn2, pn3
        //    };
        //    var pn = pns.OrderBy(tuple => tuple.Item1).First();
        //    var result = new CompressionResistanceOutput(pn.Item1, 0.9, pn.Item2, "Kip");
        //    return result;
        //}


        //private static double GetAISCCompressionLBResistance(this LippedSection section, Material material) =>
        //    section.GetAISIReducedArea(material) * material.Fy;


        //private static double GetAISCCompressionLBResistance(this UnStiffenedSection section, Material material) =>
        //    section.GetAISIReducedArea(material) * material.Fy;

        //private static double GetAISCCompressionFBStress(this Section section, Material material, LengthBracingConditions bracingConditions)
        //{
        //    var kx = bracingConditions.Kx;
        //    var ky = bracingConditions.Ky;
        //    var lx = bracingConditions.Lx;
        //    var ly = bracingConditions.Ly;
        //    var ix = section.Properties.Rx;
        //    var iy = section.Properties.Ry;
        //    var E = material.E;
        //    var Fy = material.Fy;

        //    var sigma_ex = (Math.PI.Power(2) * E) / ((kx * lx) / ix).Power(2);
        //    var sigma_ey = (Math.PI.Power(2) * E) / ((ky * ly) / iy).Power(2);
        //    var Fe = Math.Min(sigma_ex, sigma_ey);
        //    var Fy_over_Fe = Fy / Fe;
        //    if (Fy_over_Fe <= 2.25)
        //    {
        //        var Fcr1 = (0.658.Power(Fy_over_Fe)) * Fy;
        //        return Fcr1;
        //    }
        //    else
        //    {
        //        var Fcr1 = 0.877 * Fe;
        //        return Fcr1;
        //    }
        //}

        //private static double GetAISCCompressionFBRessistance(this Section lippedSection, Material material, LengthBracingConditions bracingConditions)
        //{
        //    var Fcr1 = lippedSection.GetAISCCompressionFBStress(material, bracingConditions);
        //    var A1 = lippedSection.GetAISCReducedArea(material, Fcr1);
        //    var Pn = Fcr1 * A1;
        //    return Pn;
        //}


        //private static double GetAISCCompressionFTBStress(this Section section, Material material, LengthBracingConditions bracingConditions)
        //{
        //    var kx = bracingConditions.Kx;
        //    var kz = bracingConditions.Kz;
        //    var lx = bracingConditions.Lx;
        //    var lz = bracingConditions.Lz;
        //    var ix = section.Properties.Rx;
        //    var iy = section.Properties.Ry;
        //    var A = section.Properties.A;
        //    var Cw = section.Properties.Cw;
        //    var xo_squared = section.Properties.Xo.Power(2);
        //    var J = section.Properties.J;
        //    var E = material.E;
        //    var Fy = material.Fy;
        //    var G = material.G;

        //    var io_squared = ix.Power(2) + iy.Power(2) + xo_squared;
        //    var beta = 1 - (xo_squared / io_squared);

        //    var sigma_ez = (1 / (A * io_squared)) * (((Math.PI.Power(2) * E * Cw) / (kz * lz).Power(2)) + G * J);
        //    var sigma_ex = (Math.PI.Power(2) * E) / ((kx * lx) / ix).Power(2);

        //    var Fe2 = ((sigma_ex + sigma_ez) / (2 * beta)) * (1 - (Math.Sqrt(1 - ((4 * sigma_ex * sigma_ez * beta) / (sigma_ex + sigma_ez).Power(2)))));
        //    var Fy_over_Fe = Fy / Fe2;
        //    if (Fy_over_Fe <= 2.25)
        //    {
        //        var Fcr2 = (0.658.Power(Fy_over_Fe)) * Fy;
        //        return Fcr2;
        //    }
        //    else
        //    {
        //        var Fcr2 = 0.877 * Fe2;
        //        return Fcr2;
        //    }
        //}

        //private static double GetAISCCompressionFTBRessistance(this Section lippedSection, Material material, LengthBracingConditions bracingConditions)
        //{
        //    var Fcr2 = lippedSection.GetAISCCompressionFTBStress(material, bracingConditions);
        //    var A2 = lippedSection.GetAISCReducedArea(material, Fcr2);
        //    var Pn = Fcr2 * A2;
        //    return Pn;
        //}



        //private static double GetAISCReducedArea(this Section section, Material material, double Fcr)
        //{
        //    var E = material.E;
        //    var Fy = material.Fy;
        //    var b = section.Dimensions.TotalFlangeWidthB;
        //    var t = section.Dimensions.ThicknessT;
        //    var a = section.Properties.ADimension;
        //    var c = section.Properties.CSmall;
        //    //Flange.
        //    var C1_b = 0.22;
        //    var C2_b = (1 - Math.Sqrt(1 - 4 * C1_b)) / (2 * C1_b);
        //    var lambda_r_b = 0.56 * Math.Sqrt(E / Fy);
        //    var lambda_b = b / t;
        //    var Fel_b = (C2_b * (lambda_r_b / lambda_b)).Power(2) * Fy;
        //    var bcr = 0.0;
        //    var flangeLimit = lambda_r_b * Math.Sqrt(Fy / Fcr);
        //    if (lambda_b <= flangeLimit)
        //        bcr = b;
        //    else
        //        bcr = b * (1 - C1_b * Math.Sqrt(Fel_b / Fcr)) * (Math.Sqrt(Fel_b / Fcr));
        //    //Web.
        //    var C1_a = 0.18;
        //    var C2_a = (1 - Math.Sqrt(1 - 4 * C1_a)) / (2 * C1_a);
        //    var lambda_r_a = 1.49 * Math.Sqrt(E / Fy);
        //    var lambda_a = a / t;
        //    var Fel_a = (C2_a * (lambda_r_a / lambda_a)).Power(2) * Fy;
        //    var webLimit = lambda_r_a * Math.Sqrt(Fy / Fcr);
        //    var acr = 0.0;
        //    if (lambda_a <= webLimit)
        //        acr = a;
        //    else
        //        acr = a * (1 - C1_a * Math.Sqrt(Fel_a / Fcr)) * (Math.Sqrt(Fel_a / Fcr));
        //    //Lip.
        //    var C1_c = 0.22;
        //    var C2_c = (1 - Math.Sqrt(1 - 4 * C1_c)) / (2 * C1_c);
        //    var lambda_r_c = 0.56 * Math.Sqrt(E / Fy);
        //    var lambda_L = c / t;
        //    var Fel_c = (C2_c * (lambda_r_c / lambda_L)).Power(2) * Fy;
        //    var lipLimit = lambda_r_c * Math.Sqrt(Fy / Fcr);
        //    var Ccr = 0.0;
        //    if (lambda_L <= lipLimit)
        //        Ccr = c;
        //    else
        //        Ccr = c * (1 - C1_c * Math.Sqrt(Fel_c / Fcr)) * (Math.Sqrt(Fel_c / Fcr));

        //    var Acr = t * (2 * bcr + acr + 2 * Ccr);
        //    return Acr;
        //}

        //#endregion


        //#endregion

    }
}
