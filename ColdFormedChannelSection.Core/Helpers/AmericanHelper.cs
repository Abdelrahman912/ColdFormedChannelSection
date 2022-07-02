using ColdFormedChannelSection.Core.Dtos;
using ColdFormedChannelSection.Core.Entities;
using ColdFormedChannelSection.Core.Enums;
using ColdFormedChannelSection.Core.Extensions;
using CSharp.Functional.Constructs;
using CSharp.Functional.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using static ColdFormedChannelSection.Core.Constants;
using static ColdFormedChannelSection.Core.Errors.Errors;

namespace ColdFormedChannelSection.Core.Helpers
{
    public static class AmericanHelper
    {

        #region Constants



        #endregion


        #region Moment & Compression

        #region Z Sections

        public static Validation<ResistanceInteractionOutput> AsAISIInteractionResistance(this LippedZSection section, Material material, LengthBracingConditions bracingConditions, double pu, double mu)
        {
            var result = from Pn in section.AsAISICompressionResistance(material, bracingConditions)
                         from Mn in section.AsAISIMomentResistance(material, bracingConditions)
                         select section.AsAISIInteractionResistance(material, bracingConditions, pu, mu, Pn, Mn, () => section.GetAISIReducedArea(material).AreaEffective);
            return result;
        }

        public static Validation<ResistanceInteractionOutput> AsAISIInteractionResistance(this UnStiffenedZSection section, Material material, LengthBracingConditions bracingConditions, double pu, double mu)
        {
            var result = from Pn in section.AsAISICompressionResistance(material, bracingConditions)
                         from Mn in section.AsAISIMomentResistance(material, bracingConditions)
                         select section.AsAISIInteractionResistance(material, bracingConditions, pu, mu, Pn, Mn, () => section.GetAISIReducedArea(material).AreaEffective);
            return result;
        }

        #endregion

        private static ResistanceInteractionOutput AsAISIInteractionResistance(this Section section, Material material, LengthBracingConditions bracingconditions, double pu, double mu, CompressionResistanceOutput pn_out, MomentResistanceOutput mn_out, Func<double> getAe)
        {
            var pn = pn_out.NominalResistance;
            var mn = mn_out.NominalResistance;
            var E = material.E;
            var Fy = material.Fy;
            var Ix = section.Properties.Ix;
            var Kx = bracingconditions.Kx;
            var Lx = bracingconditions.Lx;
            var Cm = bracingconditions.Cm;

            var Pex = (Math.PI.Power(2) * E * Ix) / (Kx * Lx).Power(2);
            var alpha_x = 1 - (pu / Pex);

            var loadRatio = (pu / (PHI_C_AISI * pn));
            var ieName = "";
            var ie = 0.0;
            if (loadRatio <= 0.15)
            {
                ie = (pu / (PHI_C_AISI * pn)) + (mu / (PHI_B_AISI * mn));
                //tex:
                //$$ \frac {P_u}{\phi_c P_n} + \frac {M_u} {\phi_b M_n} $$
                ieName = "\\frac {P_u}{\\phi_c P_n} + \\frac {M_u} {\\phi_b M_n}";
            }
            else
            {
                var ie_1 = (pu / (PHI_C_AISI * pn)) + ((Cm * mu) / (PHI_B_AISI * mn * alpha_x));
                var Ae = getAe();
                var Pno = Ae * Fy;
                var ie_2 = (pu / (PHI_C_AISI * Pno)) + (mu / (PHI_B_AISI * mn));
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
            var sections = pn_out.Report.Sections.Take(1).Concat(pn_out.Report.Sections.Skip(1).Select(sec => sec.AppendToName("Compression")))
                                                 .Concat(mn_out.Report.Sections.Skip(1).Select(sec => sec.AppendToName("Moment")))
                                                 .ToList();
            var report = new Report(UnitSystems.KIPINCH, "AISI Code - Interaction", sections);
            return new ResistanceInteractionOutput(pu, pn, mu, mn, ieName, ie, "kip.in", "kip", report);
        }

        public static Validation<ResistanceInteractionOutput> AsAISIInteractionResistance(this LippedCSection section, Material material, LengthBracingConditions bracingConditions, double pu, double mu)
        {
            var result = from Pn in section.AsAISICompressionResistance(material, bracingConditions)
                         from Mn in section.AsAISIMomentResistance(material, bracingConditions)
                         select section.AsAISIInteractionResistance(material, bracingConditions, pu, mu, Pn, Mn, () => section.GetAISIReducedArea(material).AreaEffective);
            return result;
        }

        public static Validation<ResistanceInteractionOutput> AsAISIInteractionResistance(this UnStiffenedCSection section, Material material, LengthBracingConditions bracingConditions, double pu, double mu)
        {
            var result = from Pn in section.AsAISICompressionResistance(material, bracingConditions)
                         from Mn in section.AsAISIMomentResistance(material, bracingConditions)
                         select section.AsAISIInteractionResistance(material, bracingConditions, pu, mu, Pn, Mn, () => section.GetAISIReducedArea(material).AreaEffective);
            return result;
        }

        #endregion

        public static LocalAISICompressionDto GetAISIReducedArea(this LippedSection lippedSection, Material material, double F_ = 0)
        {
            var b = lippedSection.Properties.BSmall;
            var t = lippedSection.Dimensions.ThicknessT;
            var C = lippedSection.Dimensions.TotalFoldWidthC;
            var c = lippedSection.Properties.CSmall;
            var a = lippedSection.Properties.ASmall;
            var u = lippedSection.Properties.U;
            var alpha = lippedSection.Properties.Alpha;
            var E = material.E;
            var v = material.V;
            var Fy = F_ == 0 ? material.Fy : F_;
            var s = 1.28 * Math.Sqrt(E / Fy);
            var be = lippedSection.Properties.BSmall;
            var Ce = c;
            var ae = lippedSection.Properties.ASmall;
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
                var C_over_b = C / b;
                Kf = C_over_b <= 0.25 ? (3.57 * (Ri).Power(n) + 0.43).TakeMin(4.0)
                                   : ((4.82 - ((5 * C) / (b))) * Ri.Power(n) + 0.43).TakeMin(4);

                var Fcr_f = Kf * e_over_v_term * (t / b).Power(2);
                var lambda_f = Math.Sqrt(Fy / Fcr_f);
                var row_f = ((1 - (0.22 / lambda_f)) / (lambda_f)).IfNegativeReturnOne().TakeMinWithOne();
                if (lambda_f > 0.673)
                    be = row_f * b;

                //Lip.
                Kc = 0.43;
                var Fcr_c = Kc * e_over_v_term * (t / c).Power(2);
                var lambda_c = Math.Sqrt(Fy / Fcr_c);
                var row_c = ((1 - (0.22 / lambda_c)) / (lambda_c)).IfNegativeReturnOne().TakeMinWithOne();
                if (lambda_c <= 0.673)
                    Ce = c * Ri;
                else
                    Ce = row_c * Ri * c;
            }
            //Web.
            var kw = 4.0;
            var Fcr_w = kw * e_over_v_term * (t / a).Power(2);
            var lambda_w = Math.Sqrt(Fy / Fcr_w);
            var row_w = ((1 - (0.22 / lambda_w)) / lambda_w).IfNegativeReturnOne().TakeMinWithOne();
            if (lambda_w > 0.673)
                ae = row_w * a;
            var Ae = t * (2 * Ce + 2 * be + ae + 2 * u * (1 + alpha));
            return new LocalAISICompressionDto(
               kw: kw,
               ae: ae,
               kf: Kf,
               be: be,
               kc: Kc,
               ce: Ce,
               areaEffective: Ae,
               fy: Fy,
               pn: Fy * Ae
               );
        }

        public static LocalAISICompressionDto GetAISIReducedArea(this UnStiffenedSection unstiffenedSection, Material material, double F_ = 0)
        {
            var b = unstiffenedSection.Properties.BSmall;
            var t = unstiffenedSection.Dimensions.ThicknessT;
            var a = unstiffenedSection.Properties.ASmall;
            var u = unstiffenedSection.Properties.U;
            var alpha = unstiffenedSection.Properties.Alpha;
            var E = material.E;
            var v = material.V;
            var Fy = F_ == 0 ? material.Fy : F_;
            var be = b;
            var ae = unstiffenedSection.Properties.ASmall;
            var e_over_v_term = (Math.PI.Power(2) * E) / (12 * (1 - v.Power(2)));

            //Flange.
            var Kf = 0.43;
            var Fcr_f = Kf * e_over_v_term * (t / b).Power(2);
            var lambda_f = Math.Sqrt(Fy / Fcr_f);
            var row_f = (((1 - (0.22 / lambda_f)) / lambda_f)).IfNegativeReturnOne().TakeMinWithOne();
            if (lambda_f > 0.673)
                be = row_f * b;

            //Web.
            var kw = 4.0;
            var Fcr_w = kw * e_over_v_term * (t / a).Power(2);
            var lambda_w = Math.Sqrt(Fy / Fcr_w);
            var row_w = ((1 - (0.22 / lambda_w)) / lambda_w).IfNegativeReturnOne().TakeMinWithOne();
            if (lambda_w > 0.673)
                ae = row_w * a;
            var Ae = t * (2 * be + ae + 2 * u * (1 + alpha));
            return new LocalAISICompressionDto(
                kw: kw,
                ae: ae,
                kf: Kf,
                be: be,
                kc: 0,
                ce: 0,
                areaEffective: Ae,
                fy: Fy,
                pn: Fy * Ae
                );
        }


        private static LocalAISIMomentDto GetAISIReducedZe(this UnStiffenedSection unstiffenedSection, Material material, double F_ = 0, bool isOneIter = false)
        {
            var b = unstiffenedSection.Properties.BSmall;
            var t = unstiffenedSection.Dimensions.ThicknessT;
            var a = unstiffenedSection.Properties.ASmall;
            var E = material.E;
            var v = material.V;
            var Fy = F_ == 0 ? material.Fy : F_;
            var be = unstiffenedSection.Properties.BSmall;
            var e_over_v_term = (Math.PI.Power(2) * E) / (12 * (1 - v.Power(2)));

            //Flange.
            var kf = 0.43;
            var Fcr_f = kf * e_over_v_term * (t / b).Power(2);
            var lambda_f = Math.Sqrt(Fy / Fcr_f);
            var row_f = (((1 - (0.22 / lambda_f)) / lambda_f)).IfNegativeReturnOne().TakeMinWithOne();
            if (lambda_f > 0.673)
                be = row_f * b;

            //Web.
            var result = unstiffenedSection.GetAISIReducedZe(material, kf, be, false, F_, isOneIter);
            return result;
        }


        private static LocalAISIMomentDto GetAISIReducedZe(this LippedSection lippedSection, Material material, double F_ = 0, bool isOneIter = false)
        {
            var b = lippedSection.Properties.BSmall;
            var t = lippedSection.Dimensions.ThicknessT;
            var C = lippedSection.Dimensions.TotalFoldWidthC;
            var H = lippedSection.Dimensions.TotalHeightH;
            var R = lippedSection.Dimensions.InternalRadiusR;
            var c = lippedSection.Properties.CSmall;
            var a = lippedSection.Properties.ASmall;
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
                var C_over_b = C / b;
                Kf = C_over_b <= 0.25 ? (3.57 * (Ri).Power(n) + 0.43).TakeMin(4)
                                       : ((4.82 - ((5 * C) / (b))) * Ri.Power(n) + 0.43).TakeMin(4);

                var Fcr_f = Kf * e_over_v_term * (t / b).Power(2);
                var lambda_f = Math.Sqrt(Fy / Fcr_f);
                var row_f = ((1 - (0.22 / lambda_f)) / (lambda_f)).IfNegativeReturnOne().TakeMinWithOne();
                if (lambda_f > 0.673)
                    be = row_f * b;

            }

            var result = lippedSection.GetAISIReducedZe(material, Kf, be, true, F_, isOneIter);
            return result;
        }

        private static LocalAISIMomentDto GetAISIReducedZe(this Section section, Material material, double Kf, double be, bool isLipped, double F_ = 0, bool isOneIter = false)
        {
            var lipFactor = 0;
            if (isLipped)
                lipFactor = 1;

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
            var sOver3 = s / 3;
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
            var ce = 0.0;

            //Lip
            var b_over_t = b / t;
            var Ia_1 = 399 * t.Power(4) * ((b_over_t / s) - 0.328).Power(3);
            var Ia_2 = t.Power(4) * (115 * (b_over_t / s) + 5);
            var Ia = Math.Min(Ia_1, Ia_2);
            var Is = (c.Power(3) * t) / 12;
            var Ri = Math.Min(Is / Ia, 1);
            var kc = 0.43;
            var Fcr_c = kc * e_over_v_term * (t / c).Power(2);

            do
            {
                y_bar = new_y_bar;
                var F1 = Fy * ((y_bar - (t / 2) - r) / y_bar);

                if (isLipped)
                {
                    //Lip
                    var lambda_c = Math.Sqrt(F1 / Fcr_c);
                    var row_c = (((1 - (0.22 / lambda_c)) / lambda_c)).IfNegativeReturnOne().TakeMinWithOne();
                    if (b_over_t <= sOver3)
                    {
                        ce = c;
                    }
                    else
                    {
                        ce = c * Ri;
                        if (lambda_c > 0.673)
                            ce = row_c * c * Ri;
                    }
                }

                //Web
                var F2 = Fy * ((H - y_bar - (t / 2) - r) / (y_bar));
                var sai = Math.Abs(F2 / F1);
                Kw = 4 + 2 * (1 + sai).Power(3) + 2 * (1 + sai);
                var Fcr_w = Kw * e_over_v_term * (t / a).Power(2);
                var lambda_W = Math.Sqrt(F1 / Fcr_w);
                var row_w = ((1 - (0.22 / lambda_W)) / (lambda_W)).IfNegativeReturnOne().TakeMinWithOne();
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

            return new LocalAISIMomentDto(
                kw: Kw,
                ae: (ae - hneg),
                kc: kc,
                ce: ce,
                kf: Kf,
                be: be,
                ze: Ze,
                fy: Fy,
                mn: Fy * Ze
                );

        }

        #region Compression

        #region Z Sections

        private static AISICompressionZDto AsCompressionDto(this UnStiffenedZSection section, Material material, LengthBracingConditions bracingConditions)
        {
            var lb = section.GetAISICompressionLBResistance(material);
            var fb = section.GetAISICompressionFBRessistance(material, bracingConditions);
            //var tfb = section.GetAISICompressionFTBRessistance(material, bracingConditions);
            var tb = section.GetAISICompressionTBRessistance(material, bracingConditions);
            return new AISICompressionZDto(lb, fb, tb);
        }

        private static AISICompressionZDto AsCompressionDto(this LippedZSection section, Material material, LengthBracingConditions bracingConditions)
        {
            var lb = section.GetAISICompressionLBResistance(material);
            var fb = section.GetAISICompressionFBRessistance(material, bracingConditions);
            //var tfb = section.GetAISICompressionFTBRessistance(material, bracingConditions);
            var tb = section.GetAISICompressionTBRessistance(material, bracingConditions);
            return new AISICompressionZDto(lb, fb, tb);
        }

        private static CompressionResistanceOutput AsCompressionOutput(this AISICompressionZDto dto, LippedZSection section)
        {
            var report = dto.AsReport(section);
            return new CompressionResistanceOutput(dto.GoverningCase.NominalStrength, PHI_C_AISI, PHI_C_NAME_AISI, COMP_DESIGN_RESIST_AISI, dto.GoverningCase.FailureMode, "Kip", report);
        }

        private static CompressionResistanceOutput AsCompressionOutput(this AISICompressionZDto dto, UnStiffenedZSection section)
        {
            var report = dto.AsReport(section);
            return new CompressionResistanceOutput(dto.GoverningCase.NominalStrength, PHI_C_AISI, PHI_C_NAME_AISI, COMP_DESIGN_RESIST_AISI, dto.GoverningCase.FailureMode, "Kip", report);
        }

        public static Validation<CompressionResistanceOutput> AsAISICompressionResistance(this LippedZSection lippedSection, Material material, LengthBracingConditions bracingConditions)
        {
            var result = from valid in lippedSection.IsValidForCompression()
                         select lippedSection.AsCompressionDto(material, bracingConditions).AsCompressionOutput(lippedSection);
            return result;
        }

        public static Validation<CompressionResistanceOutput> AsAISICompressionResistance(this UnStiffenedZSection unstiffenedSection, Material material, LengthBracingConditions bracingConditions)
        {
            
            var result = from valid in unstiffenedSection.IsValidForCompression()
                         select unstiffenedSection.AsCompressionDto(material, bracingConditions).AsCompressionOutput(unstiffenedSection);
            return result;
        }

        #endregion

        private static AISICompressionCDto AsCompressionDto(this UnStiffenedCSection unStiffenedSection, Material material, LengthBracingConditions bracingConditions)
        {
            var lb = unStiffenedSection.GetAISICompressionLBResistance(material);
            var fb = unStiffenedSection.GetAISICompressionFBRessistance(material, bracingConditions);
            var tfb = unStiffenedSection.GetAISICompressionFTBRessistance(material, bracingConditions);
            var tb = unStiffenedSection.GetAISICompressionTBRessistance(material, bracingConditions);
            return new AISICompressionCDto(lb, fb, tfb, tb);
        }

        private static AISICompressionCDto AsCompressionDto(this LippedCSection lippedSection, Material material, LengthBracingConditions bracingConditions)
        {
            var lb = lippedSection.GetAISICompressionLBResistance(material);
            var fb = lippedSection.GetAISICompressionFBRessistance(material, bracingConditions);
            var tfb = lippedSection.GetAISICompressionFTBRessistance(material, bracingConditions);
            var tb = lippedSection.GetAISICompressionTBRessistance(material, bracingConditions);
            return new AISICompressionCDto(lb, fb, tfb, tb);
        }

        private static CompressionResistanceOutput AsCompressionOutput(this AISICompressionCDto dto, LippedCSection section)
        {
            var report = dto.AsReport(section);
            return new CompressionResistanceOutput(dto.GoverningCase.NominalStrength, PHI_C_AISI, PHI_C_NAME_AISI, COMP_DESIGN_RESIST_AISI, dto.GoverningCase.FailureMode, "Kip", report);
        }

        private static CompressionResistanceOutput AsCompressionOutput(this AISICompressionCDto dto, UnStiffenedCSection section)
        {
            var report = dto.AsReport(section);
            return new CompressionResistanceOutput(dto.GoverningCase.NominalStrength, PHI_C_AISI, PHI_C_NAME_AISI, COMP_DESIGN_RESIST_AISI, dto.GoverningCase.FailureMode, "Kip", report);
        }

        public static Validation<CompressionResistanceOutput> AsAISICompressionResistance(this LippedCSection lippedSection, Material material, LengthBracingConditions bracingConditions)
        {
            //if (!lippedSection.IsValidForCompression())
            //    return new CompressionResistanceOutput(0.0, PHI_C_AISI, PHI_C_NAME_AISI, COMP_DESIGN_RESIST_AISI, FailureMode.UNSAFE, "Kip", null);
            //(var pn_local, var localItems) = lippedSection.GetAISICompressionLBResistance(material);
            //(var pn_FB, var flexuralItems) = lippedSection.GetAISICompressionFBRessistance(material, bracingConditions);
            //(var pn_TFB, var tfbItems) = lippedSection.GetAISICompressionFTBRessistance(material, bracingConditions);
            //(var pn_Tb, var tbItems) = lippedSection.GetAISICompressionTBRessistance(material, bracingConditions);
            //var pn1 = Tuple.Create(pn_local, FailureMode.LOCALBUCKLING);
            //var pn2 = Tuple.Create(pn_FB, FailureMode.FLEXURALBUCKLING);
            //var pn3 = Tuple.Create(pn_TFB, FailureMode.FLEXURAL_TORSIONAL_BUCKLING);
            //var pn4 = Tuple.Create(pn_Tb, FailureMode.TORSIONALBUCKLING);
            //var pns = new List<Tuple<double, FailureMode>>()
            //{
            //    pn1, pn2, pn3,pn4
            //};
            //var pn = pns.Distinct(NominalStrengthEqualComparer).OrderBy(tuple => tuple.Item1).First();
            //var items = new List<ReportItem>()
            //{
            //    new ReportItem("Governing Case",pn.Item2.GetDescription(),Units.NONE),
            //    new ReportItem("Nominal Load",pn.Item1.ToString("0.###"),Units.KIP),
            //    new ReportItem("phi",PHI_C_AISI.ToString("0.###"),Units.NONE),
            //    new ReportItem("Design Resistance",(PHI_C_AISI*pn.Item1).ToString("0.###"),Units.KIP),

            //};
            //var secDimsItems = new List<ReportItem>()
            //{
            //    new ReportItem("H",lippedSection.Dimensions.TotalHeightH.ToString("0.###"),Units.IN),
            //    new ReportItem("B",lippedSection.Dimensions.TotalFlangeWidthB.ToString("0.###"),Units.IN),
            //    new ReportItem("R",lippedSection.Dimensions.InternalRadiusR.ToString("0.###"),Units.IN),
            //    new ReportItem("t",lippedSection.Dimensions.ThicknessT.ToString("0.###"),Units.IN),
            //    new ReportItem("C",lippedSection.Dimensions.TotalFoldWidthC.ToString("0.###"),Units.IN)
            //};
            //var secDimSection = new ListReportSection("Section Dimensions", secDimsItems);
            //var localSection = new ListReportSection("Local Buckling", localItems);
            //var flexuralSection = new ListReportSection("Flexural Buckling", flexuralItems);
            //var tfbSection = new ListReportSection("Torsional Flexural Buckling", tfbItems);
            //var tbSection = new ListReportSection("Torsional Buckling", tbItems);
            //var designSections = new ListReportSection("Design Compression Load", items);
            //var sections = new List<IReportSection>() { secDimSection, localSection, flexuralSection, tfbSection, tbSection, designSections };
            //var report = new Report(UnitSystems.KIPINCH, "AISI Code - Compression", sections);


            //var result = new CompressionResistanceOutput(pn.Item1, PHI_C_AISI, PHI_C_NAME_AISI, COMP_DESIGN_RESIST_AISI, pn.Item2, "Kip", report);
            //return result;
            var result = from valid in lippedSection.IsValidForCompression()
                         select lippedSection.AsCompressionDto(material, bracingConditions).AsCompressionOutput(lippedSection);
            return result;
        }

        public static Validation<CompressionResistanceOutput> AsAISICompressionResistance(this UnStiffenedCSection unstiffenedSection, Material material, LengthBracingConditions bracingConditions)
        {
            //if (!unstiffenedSection.IsValidForCompression())
            //    return new CompressionResistanceOutput(0.0, PHI_C_AISI, PHI_C_NAME_AISI, COMP_DESIGN_RESIST_AISI, FailureMode.UNSAFE, "Kip", null);
            //(var pn_local, var localItems) = unstiffenedSection.GetAISICompressionLBResistance(material);
            //(var pn_FB, var flexuralItems) = unstiffenedSection.GetAISICompressionFBRessistance(material, bracingConditions);
            //(var pn_TFB, var tfbItems) = unstiffenedSection.GetAISICompressionFTBRessistance(material, bracingConditions);
            //(var pn_Tb, var tbItems) = unstiffenedSection.GetAISICompressionTBRessistance(material, bracingConditions); //TODO: Reports
            //var pn1 = Tuple.Create(pn_local, FailureMode.LOCALBUCKLING);
            //var pn2 = Tuple.Create(pn_FB, FailureMode.FLEXURALBUCKLING);
            //var pn3 = Tuple.Create(pn_TFB, FailureMode.FLEXURAL_TORSIONAL_BUCKLING);
            //var pn4 = Tuple.Create(pn_Tb, FailureMode.TORSIONALBUCKLING);
            //var pns = new List<Tuple<double, FailureMode>>()
            //{
            //    pn1, pn2, pn3,pn4
            //};
            //var pn = pns.Distinct(NominalStrengthEqualComparer).OrderBy(tuple => tuple.Item1).First();
            //var designItems = new List<ReportItem>()
            //{
            //    new ReportItem("Governing Case",pn.Item2.GetDescription(),Units.NONE),
            //    new ReportItem("Nominal Load",pn.Item1.ToString("0.###"),Units.KIP),
            //    new ReportItem("phi",PHI_C_AISI.ToString("0.###"),Units.NONE),
            //    new ReportItem("Design Resistance",(PHI_C_AISI*pn.Item1).ToString("0.###"),Units.KIP),

            //};
            //var secDimsItems = new List<ReportItem>()
            //{
            //    new ReportItem("H",unstiffenedSection.Dimensions.TotalHeightH.ToString("0.###"),Units.IN),
            //    new ReportItem("B",unstiffenedSection.Dimensions.TotalFlangeWidthB.ToString("0.###"),Units.IN),
            //    new ReportItem("R",unstiffenedSection.Dimensions.InternalRadiusR.ToString("0.###"),Units.IN),
            //    new ReportItem("t",unstiffenedSection.Dimensions.ThicknessT.ToString("0.###"),Units.IN),
            //};
            //var secDimSection = new ListReportSection("Section Dimensions", secDimsItems);
            //var localSection = new ListReportSection("Local Buckling", localItems);
            //var flexuralSection = new ListReportSection("Flexural Buckling", flexuralItems);
            //var tfbSection = new ListReportSection("Torsional Flexural Buckling", tfbItems);
            //var tbSection = new ListReportSection("Torsional Buckling", tbItems);
            //var designSection = new ListReportSection("Design Compression Load", designItems);
            //var sections = new List<IReportSection>() { secDimSection, localSection, flexuralSection, tfbSection, tbSection, designSection };
            //var report = new Report(UnitSystems.KIPINCH, "AISI Code - Compression", sections);

            //var result = new CompressionResistanceOutput(pn.Item1, PHI_C_AISI, PHI_C_NAME_AISI, COMP_DESIGN_RESIST_AISI, pn.Item2, "Kip", report);
            //return result;
            var result = from valid in unstiffenedSection.IsValidForCompression()
                         select unstiffenedSection.AsCompressionDto(material, bracingConditions).AsCompressionOutput(unstiffenedSection);
            return result;
        }

        private static Validation<bool> IsValidForCompression(this Section section)
        {
            var b_over_t = Tuple.Create(section.Properties.BSmall / section.Dimensions.ThicknessT, 60.0);

            var c_over_t = Tuple.Create(section.Properties.CSmall / section.Dimensions.ThicknessT, 14.0);
            var a_over_t = Tuple.Create(section.Properties.ASmall / section.Dimensions.ThicknessT, 500.0);
            var C_over_b = Tuple.Create(section.Dimensions.TotalFoldWidthC / section.Properties.BSmall, 0.8);

            var allows = new List<Tuple<double, double>>()
            {
                b_over_t,c_over_t,a_over_t,C_over_b
            };
            var result = !allows.Any(tuple => tuple.Item1 > tuple.Item2);
            if (result)
                return true;
            else
                return CantCalculateNominalStrength;
        }


        private static LocalAISICompressionDto GetAISICompressionLBResistance(this LippedSection section, Material material) =>
             section.GetAISIReducedArea(material);


        private static LocalAISICompressionDto GetAISICompressionLBResistance(this UnStiffenedSection section, Material material) =>
             section.GetAISIReducedArea(material);


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

        private static FBAISICompressionDto GetAISICompressionFBRessistance(this LippedSection lippedSection, Material material, LengthBracingConditions bracingConditions)
        {
            var F1 = lippedSection.GetAISICompressionFBStress(material, bracingConditions);
            var dto = lippedSection.GetAISIReducedArea(material, F1);
            var Pn = F1 * dto.AreaEffective;
            return new FBAISICompressionDto(
                    f: F1,
                    a: dto.AreaEffective,
                    pn: Pn
                );
        }

        private static FBAISICompressionDto GetAISICompressionFBRessistance(this UnStiffenedSection unstiffenedSection, Material material, LengthBracingConditions bracingConditions)
        {
            var F1 = unstiffenedSection.GetAISICompressionFBStress(material, bracingConditions);
            var dto = unstiffenedSection.GetAISIReducedArea(material, F1);
            var Pn = F1 * dto.AreaEffective;
            return new FBAISICompressionDto(
                    f: F1,
                    a: dto.AreaEffective,
                    pn: Pn
                );
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

        private static double GetAISICompressionTBStress(this Section section, Material material, LengthBracingConditions bracingConditions)
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
            var Fe3 = sigma_ez;
            var lambda_3_squared = Fy / Fe3;
            var F3 = 0.0;
            if (lambda_3_squared <= 1.5.Power(2))
            {
                F3 = 0.658.Power(lambda_3_squared) * Fy;
            }
            else
            {
                F3 = (0.877 / lambda_3_squared) * Fy;
            }
            return F3;
        }

        private static TBAISICompressioDto GetAISICompressionTBRessistance(this LippedSection lippedSection, Material material, LengthBracingConditions bracingConditions)
        {
            var F3 = lippedSection.GetAISICompressionTBStress(material, bracingConditions);
            var dto = lippedSection.GetAISIReducedArea(material, F3);
            var Pn = F3 * dto.AreaEffective;
            return new TBAISICompressioDto(
                    f: F3,
                    a: dto.AreaEffective,
                    pn: Pn
                );
        }

        private static TBAISICompressioDto GetAISICompressionTBRessistance(this UnStiffenedSection section, Material material, LengthBracingConditions bracingConditions)
        {
            var F3 = section.GetAISICompressionTBStress(material, bracingConditions);
            var dto = section.GetAISIReducedArea(material, F3);
            var Pn = F3 * dto.AreaEffective;
            return new TBAISICompressioDto(
                    f: F3,
                    a: dto.AreaEffective,
                    pn: Pn
                );
        }

        private static FTBAISICompressionDto GetAISICompressionFTBRessistance(this LippedCSection lippedSection, Material material, LengthBracingConditions bracingConditions)
        {
            var F2 = lippedSection.GetAISICompressionFTBStress(material, bracingConditions);
            var dto = lippedSection.GetAISIReducedArea(material, F2);
            var Pn = F2 * dto.AreaEffective;
            //var items = new List<ReportItem>()
            //{
            //    new ReportItem("Torsional Flexural Stress (F2)",F2.ToString("0.###"),Units.KSI),
            //    new ReportItem("Area (A2)",A2.ToString("0.###"),Units.IN_2),
            //    new ReportItem("Torsional Flexural Nominal Load (Pn)",Pn.ToString("0.###"),Units.KIP),
            //};
            //return Tuple.Create(Pn, items);
            return new FTBAISICompressionDto(
                f: F2,
                a: dto.AreaEffective,
                pn: Pn
                );
        }

        private static FTBAISICompressionDto GetAISICompressionFTBRessistance(this UnStiffenedCSection unstiffenedSection, Material material, LengthBracingConditions bracingConditions)
        {
            var F2 = unstiffenedSection.GetAISICompressionFTBStress(material, bracingConditions);
            var dto = unstiffenedSection.GetAISIReducedArea(material, F2);
            var Pn = F2 * dto.AreaEffective;
            //var items = new List<ReportItem>()
            //{
            //    new ReportItem("Flexural Stress (F1)",F2.ToString("0.###"),Units.KSI),
            //    new ReportItem("Area (A1)",A2.ToString("0.###"),Units.IN_2),
            //    new ReportItem("Flexural Nominal Load (Pn)",Pn.ToString("0.###"),Units.KIP),
            //};
            //return Tuple.Create(Pn, items);
            return new FTBAISICompressionDto(
              f: F2,
              a: dto.AreaEffective,
              pn: Pn
              );
        }

        #endregion



        #region Moment

        #region Z Sections

        private static AISIMomentDto AsMomentDto(this UnStiffenedZSection section , Material material , LengthBracingConditions bracingConditions)
        {
            var lb = section.GetAISIMomentLBResistance(material);
            var ltb = section.GetAISIMomentLTBRessistance(material, bracingConditions, section.CalcFeForZ(material, bracingConditions));
            return new AISIMomentDto(lb, ltb);
        }

        private static AISIMomentDto AsMomentDto(this LippedZSection section , Material material , LengthBracingConditions bracingConditions)
        {
            var lb = section.GetAISIMomentLBResistance(material);
            var ltb = section.GetAISIMomentLTBRessistance(material, bracingConditions, section.CalcFeForZ(material, bracingConditions));
            return new AISIMomentDto(lb, ltb);
        }

        private static MomentResistanceOutput AsMomentOutput(this AISIMomentDto dto, LippedZSection section)
        {
            var report = dto.AsReport(section);
            return new MomentResistanceOutput(dto.GoverningCase.NominalStrength, PHI_B_AISI, PHI_B_NAME_AISI, MOM_DESIGN_RESIST_AISI, dto.GoverningCase.FailureMode, "Kip", report);
        }

        private static MomentResistanceOutput AsMomentOutput(this AISIMomentDto dto, UnStiffenedZSection section)
        {
            var report = dto.AsReport(section);
            return new MomentResistanceOutput(dto.GoverningCase.NominalStrength, PHI_B_AISI, PHI_B_NAME_AISI, MOM_DESIGN_RESIST_AISI, dto.GoverningCase.FailureMode, "Kip", report);
        }

        public static Validation<MomentResistanceOutput> AsAISIMomentResistance(this LippedZSection section, Material material, LengthBracingConditions bracingConditions)
        {
            var result = from valid in section.IsValidForMoment()
                         select section.AsMomentDto(material, bracingConditions).AsMomentOutput(section);
            return result;
        }

        public static Validation<MomentResistanceOutput> AsAISIMomentResistance(this UnStiffenedZSection section, Material material, LengthBracingConditions bracingConditions)
        {
            var result = from valid in section.IsValidForMoment()
                         select section.AsMomentDto(material, bracingConditions).AsMomentOutput(section);
            return result;
        }

        private static double CalcFeForZ(this Section section, Material material, LengthBracingConditions bracingConditions)
        {
            var H = section.Dimensions.TotalHeightH;
            var Iy = section.Properties.Iy;
            var E = material.E;
            var ky = bracingConditions.Ky;
            var ly = bracingConditions.Ly;
            var Cb = bracingConditions.Cb;
            var Zg = section.Properties.Zg;
            var num = Cb * Math.PI.Power(2) * E * H * Iy;
            var dnum = 4 * Zg * (ky * ly).Power(2);
            var Fe = num / dnum;
            return Fe;
        }

        #endregion

        private static Validation<bool> IsValidForMoment(this Section section)
        {
            var b_over_t = Tuple.Create(section.Properties.BSmall / section.Dimensions.ThicknessT, 60.0);

            var c_over_t = Tuple.Create(section.Properties.CSmall / section.Dimensions.ThicknessT, 14.0);
            var a_over_t = Tuple.Create(section.Properties.ASmall / section.Dimensions.ThicknessT, 200.0);
            var C_over_b = Tuple.Create(section.Dimensions.TotalFoldWidthC / section.Properties.BSmall, 0.8);

            var allows = new List<Tuple<double, double>>()
            {
                b_over_t,c_over_t,a_over_t,C_over_b
            };
            var result = !allows.Any(tuple => tuple.Item1 > tuple.Item2);
            if (result)
                return true;
            else
                return CantCalculateNominalStrength;
        }

        private static AISIMomentDto AsMomentDto(this UnStiffenedCSection section, Material material, LengthBracingConditions bracingConditions)
        {
            var lb = section.GetAISIMomentLBResistance(material);
            var ltb = section.GetAISIMomentLTBRessistance(material, bracingConditions,section.CalcFeForC(material,bracingConditions));
            return new AISIMomentDto(lb, ltb);
        }

        private static AISIMomentDto AsMomentDto(this LippedCSection section, Material material, LengthBracingConditions bracingConditions)
        {
            var lb = section.GetAISIMomentLBResistance(material);
            var ltb = section.GetAISIMomentLTBRessistance(material, bracingConditions,section.CalcFeForC(material,bracingConditions));
            return new AISIMomentDto(lb, ltb);
        }

        private static MomentResistanceOutput AsMomentOutput(this AISIMomentDto dto, LippedCSection section)
        {
            var report = dto.AsReport(section);
            return new MomentResistanceOutput(dto.GoverningCase.NominalStrength, PHI_B_AISI, PHI_B_NAME_AISI, MOM_DESIGN_RESIST_AISI, dto.GoverningCase.FailureMode, "Kip", report);
        }

        private static MomentResistanceOutput AsMomentOutput(this AISIMomentDto dto, UnStiffenedCSection section)
        {
            var report = dto.AsReport(section);
            return new MomentResistanceOutput(dto.GoverningCase.NominalStrength, PHI_B_AISI, PHI_B_NAME_AISI, MOM_DESIGN_RESIST_AISI, dto.GoverningCase.FailureMode, "Kip", report);
        }

        public static Validation<MomentResistanceOutput> AsAISIMomentResistance(this LippedCSection lippedSection, Material material, LengthBracingConditions bracingConditions)
        {
            var result = from valid in lippedSection.IsValidForMoment()
                         select lippedSection.AsMomentDto(material, bracingConditions).AsMomentOutput(lippedSection);
            return result;
        }

        public static Validation<MomentResistanceOutput> AsAISIMomentResistance(this UnStiffenedCSection unstiffenedSection, Material material, LengthBracingConditions bracingConditions)
        {
            var result = from valid in unstiffenedSection.IsValidForMoment()
                         select unstiffenedSection.AsMomentDto(material, bracingConditions).AsMomentOutput(unstiffenedSection);
            return result;
        }

        private static LocalAISIMomentDto GetAISIMomentLBResistance(this LippedSection section, Material material) =>
            section.GetAISIReducedZe(material);

        private static LocalAISIMomentDto GetAISIMomentLBResistance(this UnStiffenedSection section, Material material) =>
             section.GetAISIReducedZe(material);

        private static double CalcFeForC(this Section section,Material material , LengthBracingConditions bracingConditions)
        {
            var E = material.E;
            var G = material.G;
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
            return Fe;
        }

        private static double GetAISIMomentLTBStress(this Section section, Material material, LengthBracingConditions bracingConditions,double Fe)
        {
            var Fy = material.Fy;
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


        private static LTBAISIMomentDto GetAISIMomentLTBRessistance(this LippedSection lippedSection, Material material, LengthBracingConditions bracingConditions,double Fe)
        {
            var fltb = lippedSection.GetAISIMomentLTBStress(material, bracingConditions,Fe);
            var dto = lippedSection.GetAISIReducedZe(material, fltb, true);
            var Mn = fltb * dto.Ze;
            return new LTBAISIMomentDto(
                zf: dto.Ze,
                f: fltb,
                mn: Mn,
                failureMode: FailureMode.LATERALTORSIONALBUCKLING
                );
        }

        private static LTBAISIMomentDto GetAISIMomentLTBRessistance(this UnStiffenedSection unStiffenedSection, Material material, LengthBracingConditions bracingConditions,double Fe)
        {
            var fltb = unStiffenedSection.GetAISIMomentLTBStress(material, bracingConditions,Fe);
            var dto = unStiffenedSection.GetAISIReducedZe(material, fltb, true);
            var Mn = fltb * dto.Ze;
            return new LTBAISIMomentDto(
                zf: dto.Ze,
                f: fltb,
                mn: Mn,
                failureMode: FailureMode.LATERALTORSIONALBUCKLING
                );
        }

        #endregion


    }
}
