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
    public static class EgyptianHelper
    {

        #region Moment & Compression

        #region Z Sections

        public static Validation<ResistanceInteractionOutput> AsEgyptInteractionResistance(this LippedZSection section, Material material, LengthBracingConditions bracingConditions, double pu, double mu)
        {
            var result = from Pn in section.AsEgyptCompressionResistance(material, bracingConditions)
                         from Mn in section.AsEgyptMomentResistance(material, bracingConditions)
                         select section.AsEgyptInteractionResistance(Pn, Mn, pu, mu);
            return result;
        }

        public static Validation<ResistanceInteractionOutput> AsEgyptInteractionResistance(this UnStiffenedZSection section, Material material, LengthBracingConditions bracingConditions, double pu, double mu)
        {
            var result = from Pn in section.AsEgyptCompressionResistance(material, bracingConditions)
                         from Mn in section.AsEgyptMomentResistance(material, bracingConditions)
                         select section.AsEgyptInteractionResistance(Pn, Mn, pu, mu);
            return result;
        }

        #endregion

        private static ResistanceInteractionOutput AsEgyptInteractionResistance(this Section section, CompressionResistanceOutput Pn, MomentResistanceOutput Mn, double pu, double mu)
        {
            //tex:
            //Load Ratio =$$ \frac {P_u} {\phi_c P_n} $$
            var loadRatio = pu / (PHI_C_EGYPT * Pn.NominalResistance);
            var ie = 0.0;
            var ieName = "";
            if (loadRatio >= 0.2)
            {
                //tex:
                //$$\frac {P_u} {\phi_c P_n} + \frac {8} {9} \frac {M_u} {\phi_b M_n}$$
                ie = (pu / (PHI_C_EGYPT * Pn.NominalResistance)) + (8.0 / 9.0) * (mu / (PHI_B_EGYPT * Mn.NominalResistance));
                ieName = "\\frac {P_u} {\\phi_c P_n} + \\frac {8} {9} \\frac {M_u} {\\phi_b M_n}";
            }
            else
            {
                //tex:
                //$$\frac {P_u} {2 \phi_c P_n} +  \frac {M_u} {\phi_b M_n}$$
                ie = (pu / (2 * PHI_C_EGYPT * Pn.NominalResistance)) + (mu / (PHI_B_EGYPT * Mn.NominalResistance));
                ieName = "\\frac {P_u} {2 \\phi_c P_n} +  \frac {M_u} {\\phi_b M_n}";
            }
            var sections = Pn.Report.Sections.Take(2).Concat(Pn.Report.Sections.Skip(2).Select(sec => sec.AppendToName("Compression")))
                                                .Concat(Mn.Report.Sections.Skip(2).Select(sec => sec.AppendToName("Moment")))
                                                .ToList();

            var report = new Report(UnitSystems.TONCM, "Egyptian Code - Interaction", sections);
            return new ResistanceInteractionOutput(pu, Pn.NominalResistance, mu, Mn.NominalResistance, ieName, ie, "t.cm", "ton", report);
        }

        public static Validation<ResistanceInteractionOutput> AsEgyptInteractionResistance(this LippedCSection section, Material material, LengthBracingConditions bracingConditions, double pu, double mu)
        {
            var result = from Pn in section.AsEgyptCompressionResistance(material, bracingConditions)
                         from Mn in section.AsEgyptMomentResistance(material, bracingConditions)
                         select section.AsEgyptInteractionResistance(Pn, Mn, pu, mu);
            return result;
        }


        public static Validation<ResistanceInteractionOutput> AsEgyptInteractionResistance(this UnStiffenedCSection section, Material material, LengthBracingConditions bracingConditions, double pu, double mu)
        {
            var result = from Pn in section.AsEgyptCompressionResistance(material, bracingConditions)
                         from Mn in section.AsEgyptMomentResistance(material, bracingConditions)
                         select section.AsEgyptInteractionResistance(Pn, Mn, pu, mu);
            return result;
        }

        #endregion

        #region Moment

        #region Z Sections

        private static EgyptMomentDto AsMomentDto(this LippedZSection section, Material material, LengthBracingConditions bracingConditions)
        {
            var dto = section.GetEgyptReducedZe(material);

            var momDto = section.GetEgyptMomentResistance(material, bracingConditions, dto.Ze);
            return new EgyptMomentDto(dto, momDto);
        }

        private static EgyptMomentDto AsMomentDto(this UnStiffenedZSection section, Material material, LengthBracingConditions bracingConditions)
        {
            var dto = section.GetEgyptReducedZe(material);

            var momDto = section.GetEgyptMomentResistance(material, bracingConditions, dto.Ze);
            return new EgyptMomentDto(dto, momDto);
        }

        private static MomentResistanceOutput AsOutput(this EgyptMomentDto dto, LippedZSection section)
        {
            var report = dto.AsReport(section);
            return new MomentResistanceOutput(dto.GoverningCase.GoverningCase.NominalStrength, PHI_B_EGYPT, PHI_B_NAME_EGYPT, MOM_DESIGN_RESIST_EGYPT, dto.GoverningCase.GoverningCase.FailureMode, "t.cm", report);
        }

        private static MomentResistanceOutput AsOutput(this EgyptMomentDto dto, UnStiffenedZSection section)
        {
            var report = dto.AsReport(section);
            return new MomentResistanceOutput(dto.GoverningCase.GoverningCase.NominalStrength, PHI_B_EGYPT, PHI_B_NAME_EGYPT, MOM_DESIGN_RESIST_EGYPT, dto.GoverningCase.GoverningCase.FailureMode, "t.cm", report);
        }

        public static Validation<MomentResistanceOutput> AsEgyptMomentResistance(this LippedZSection section, Material material, LengthBracingConditions bracingConditions)
        {
            var result = from valid in section.IsValidMoment()
                         select section.AsMomentDto(material, bracingConditions).AsOutput(section);
            return result;
        }

        public static Validation<MomentResistanceOutput> AsEgyptMomentResistance(this UnStiffenedZSection section, Material material, LengthBracingConditions bracingConditions)
        {
            var result = from valid in section.IsValidMoment()
                         select section.AsMomentDto(material, bracingConditions).AsOutput(section);
            return result;
        }

        #endregion

        private static Validation<bool> IsValidMoment(this LippedSection section)
        {
            var c_over_t = Tuple.Create(section.Properties.CPrime / section.Dimensions.ThicknessT, 40.0);
            var b_over_t = Tuple.Create(section.Properties.BPrime / section.Dimensions.ThicknessT, 60.0);
            var a_over_t = Tuple.Create(section.Properties.APrime / section.Dimensions.ThicknessT, 200.0);

            var allows = new List<Tuple<double, double>>()
            {
                b_over_t,c_over_t,a_over_t
            };
            var result = !allows.Any(tuple => tuple.Item1 > tuple.Item2);
            if (result)
                return true;
            else
                return CantCalculateNominalStrength;
        }

        private static Validation<bool> IsValidMoment(this UnStiffenedSection section)
        {
            var b_over_t = Tuple.Create(section.Properties.BPrime / section.Dimensions.ThicknessT, 40.0);

            var a_over_t = Tuple.Create(section.Properties.APrime / section.Dimensions.ThicknessT, 200.0);

            var allows = new List<Tuple<double, double>>()
            {
                b_over_t,a_over_t
            };
            var result = !allows.Any(tuple => tuple.Item1 > tuple.Item2);
            if (result)
                return true;
            else
                return CantCalculateNominalStrength;
        }

        private static EgyptMomentDto AsMomentDto(this LippedCSection section, Material material, LengthBracingConditions bracingConditions)
        {
            var dto = section.GetEgyptReducedZe(material);

            var momDto = section.GetEgyptMomentResistance(material, bracingConditions, dto.Ze);
            return new EgyptMomentDto(dto, momDto);
        }

        private static EgyptMomentDto AsMomentDto(this UnStiffenedCSection section, Material material, LengthBracingConditions bracingConditions)
        {
            var dto = section.GetEgyptReducedZe(material);

            var momDto = section.GetEgyptMomentResistance(material, bracingConditions, dto.Ze);
            return new EgyptMomentDto(dto, momDto);
        }

        private static MomentResistanceOutput AsOutput(this EgyptMomentDto dto, LippedCSection section)
        {
            var report = dto.AsReport(section);
            return new MomentResistanceOutput(dto.GoverningCase.GoverningCase.NominalStrength, PHI_B_EGYPT, PHI_B_NAME_EGYPT, MOM_DESIGN_RESIST_EGYPT, dto.GoverningCase.GoverningCase.FailureMode, "t.cm", report);
        }

        private static MomentResistanceOutput AsOutput(this EgyptMomentDto dto, UnStiffenedCSection section)
        {
            var report = dto.AsReport(section);
            return new MomentResistanceOutput(dto.GoverningCase.GoverningCase.NominalStrength, PHI_B_EGYPT, PHI_B_NAME_EGYPT, MOM_DESIGN_RESIST_EGYPT, dto.GoverningCase.GoverningCase.FailureMode, "t.cm", report);
        }

        public static Validation<MomentResistanceOutput> AsEgyptMomentResistance(this LippedCSection section, Material material, LengthBracingConditions bracingConditions)
        {
            var result = from valid in section.IsValidMoment()
                         select section.AsMomentDto(material, bracingConditions).AsOutput(section);
            return result;
        }

        public static Validation<MomentResistanceOutput> AsEgyptMomentResistance(this UnStiffenedCSection section, Material material, LengthBracingConditions bracingConditions)
        {
            var result = from valid in section.IsValidMoment()
                         select section.AsMomentDto(material, bracingConditions).AsOutput(section);
            return result;
        }

        private static EgyptMomentBaseDto GetEgyptMomentResistance(this Section section, Material material, LengthBracingConditions bracingConditions, double Ze)
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
                var fLocal = (C_star / lambda_f.Power(2));
                if (Mn1 < Mn2)
                {
                    var dto = new NominalStrengthDto(Mn1, FailureMode.LOCALBUCKLING);
                    return new EgyptMomentLTBDto(Zg, C_star, lambda_f, fLocal, Ze_report, F_report, Mn1, Mn2, dto);
                    //return Tuple.Create(Mn1, FailureMode.LOCALBUCKLING, items);
                }
                else
                {
                    var dto = new NominalStrengthDto(Mn2, FailureMode.LATERALTORSIONALBUCKLING);
                    return new EgyptMomentLTBDto(Zg, C_star, lambda_f, fLocal, Ze_report, F_report, Mn1, Mn2, dto);
                    //return Tuple.Create(Mn2, FailureMode.LATERALTORSIONALBUCKLING, items);
                }
            }
            else
            {
                var Mn = Ze * Fy;
                var items = new List<ReportItem>()
                {
                    new ReportItem("Flange Slenderness Ratio (lambadaF)",lambda_f.ToString("0.###"),Units.NONE),
                    new ReportItem("Nominal Moment (Mn)",Mn.ToString("0.###"),Units.TON_CM)
                };
                //return Tuple.Create(Mn, FailureMode.LOCALBUCKLING, items);
                var dto = new NominalStrengthDto(Mn, FailureMode.LOCALBUCKLING);
                return new EgyptMomentLBDto(lambda_f, dto);
            }

        }

        private static LocalEgyptMomentDto GetEgyptReducedZe(this UnStiffenedSection section, Material material)
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
            var dto = section.GetEgyptReducedZe(material, be, false, 0,Kf);
            //items.Add(new ReportItem("Effective Section Modulus (Ze)", Ze.ToString("0.###"), Units.CM_3));
            //items.Add(new ReportItem("Yield Stress (Fy)", material.Fy.ToString("0.###"), Units.TON_CM_2));
            //return Tuple.Create(Ze, items);
            return dto;
        }

        private static LocalEgyptMomentDto GetEgyptReducedZe(this LippedSection section, Material material)
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
            var Kf = 0.0;
            if (b_over_t <= s / 3 /*&& c_over_b <= 0.25*/)
            {
                be = b_prime;
            }
            else if (b_over_t < s && b_over_t > s / 3 && c_over_b > 0.25 && c_over_b <= 0.8)
            {
                Ia = 399 * (((b_over_t) / s) - 0.33).Power(3) * t.Power(4);
                var Kf_1 = (4.82 - 5 * (C / b)) * (Is / Ia).Power(0.5) + 0.43;
                var Kf_2 = 5.25 - 5 * (C / b);
                Kf = Math.Min(Kf_1, Kf_2);
                var lambda_f = ((b_prime / t) / 44) * Math.Sqrt(Fy / Kf);
                var sai_f = 1.0;
                var row_f = Math.Min(1, Math.Abs((lambda_f - 0.15 - 0.05 * sai_f) / (lambda_f.Power(2))));
                be = row_f * b_prime;

            }
            else if (b_over_t < s && b_over_t > s / 3 && c_over_b <= 0.25)
            {
                Ia = 399 * (((b_over_t) / s) - 0.33).Power(3) * t.Power(4);
                Kf = Math.Min(3.57 * (Is / Ia).Power(0.5) + 0.43, 4);
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
                Kf = Math.Min(Kf_1, Kf_2);
                var lambda_f = ((b_prime / t) / 44) * Math.Sqrt(Fy / Kf);
                var sai_f = 1.0;
                var row_f = Math.Min(1, Math.Abs((lambda_f - 0.15 - 0.05 * sai_f) / (lambda_f.Power(2))));
                be = row_f * b_prime;


            }
            else if (b_over_t >= s && c_over_b <= 0.25)
            {
                Ia = (((115 * (b / t)) / (s)) + 5) * t.Power(4);
                Kf = Math.Min(3.57 * (Is / Ia).Power(1 / 3.0) + 0.43, 4);
                var lambda_f = ((b_prime / t) / 44) * Math.Sqrt(Fy / Kf);
                var sai_f = 1.0;
                var row_f = Math.Min(1, Math.Abs((lambda_f - 0.15 - 0.05 * sai_f) / (lambda_f.Power(2))));
                be = row_f * b_prime;
            }
            var Ri = Math.Min(1, Is / Ia);



            var dto = section.GetEgyptReducedZe(material, be, true, Ri,Kf);
            //items.Add(new ReportItem("Effective Section Modulus (Ze)", Ze.ToString("0.###"), Units.CM_3));
            //items.Add(new ReportItem("Yield Stress (Fy)", material.Fy.ToString("0.###"), Units.TON_CM_2));
            //return Tuple.Create(Ze, items);
            return dto;
        }

        private static LocalEgyptMomentDto GetEgyptReducedZe(this Section section, Material material, double be, bool isLipped, double Ri,double kf)
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
            var kw = 0.0;

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
            //var items = new List<ReportItem>()
            //{
            //    new ReportItem("Effective Height (ae)",(h1+h2).ToString("0.###"),Units.CM),
            //    new ReportItem("Effective Flange Width (be)",(be).ToString("0.###"),Units.CM),
            //};
            //if (isLipped)
            //{
                //items.Add(new ReportItem("Effective Lip (ce)", ce.ToString("0.###"), Units.CM));
                return new LocalEgyptMomentDto((h1 + h2), be, ce, Ze, material.Fy,kw,kf,Kc);
            //}
            //return Tuple.Create(Ze, items);
            //return new LocalEgyptMomentDto((h1 + h2), be, ce, Ze, material.Fy);
        }

        #endregion

        #region Compression

        #region Z Sections

        private static EgyptCompressionZDto AsCompressionDto(this LippedZSection section, Material material, LengthBracingConditions bracingConditions)
        {
            var lbDto = section.GetEgyptCompressionLBResistance(material);
            var Aee = section.GetEgyptReducedAreaEE(material);
            var fbDto = section.GetEgyptCompressionFBResistance(material, bracingConditions, Aee);
            //var tfbDto = section.GetEgyptCompressionTFBResistance(material, bracingConditions, Aee);
            var tbDto = section.GetEgyptCompressionTBResistance(material, bracingConditions, Aee);
            return new EgyptCompressionZDto(lbDto, fbDto, tbDto);
        }

        private static EgyptCompressionZDto AsCompressionDto(this UnStiffenedZSection section, Material material, LengthBracingConditions bracingConditions)
        {
            var lbDto = section.GetEgyptCompressionLBResistance(material);
            var Aee = section.GetEgyptReducedAreaEE(material);
            var fbDto = section.GetEgyptCompressionFBResistance(material, bracingConditions, Aee);
            //var tfbDto = section.GetEgyptCompressionTFBResistance(material, bracingConditions, Aee);
            var tbDto = section.GetEgyptCompressionTBResistance(material, bracingConditions, Aee);
            return new EgyptCompressionZDto(lbDto, fbDto, tbDto);
        }

        private static CompressionResistanceOutput AsOutput(this EgyptCompressionZDto dto, LippedZSection section)
        {
            var report = dto.AsReport(section);
            return new CompressionResistanceOutput(dto.GoverningCase.NominalStrength, PHI_C_EGYPT, PHI_C_NAME_EGYPT, COMP_DESIGN_RESIST_EGYPT, dto.GoverningCase.FailureMode, "ton", report);
        }

        private static CompressionResistanceOutput AsOutput(this EgyptCompressionZDto dto, UnStiffenedZSection section)
        {
            var report = dto.AsReport(section);
            return new CompressionResistanceOutput(dto.GoverningCase.NominalStrength, PHI_C_EGYPT, PHI_C_NAME_EGYPT, COMP_DESIGN_RESIST_EGYPT, dto.GoverningCase.FailureMode, "ton", report);
        }

        public static Validation<CompressionResistanceOutput> AsEgyptCompressionResistance(this LippedZSection section, Material material, LengthBracingConditions bracingConditions)
        {
            var result = from valid in section.IsValidCompression()
                         select section.AsCompressionDto(material, bracingConditions).AsOutput(section);
            return result;
        }

        public static Validation<CompressionResistanceOutput> AsEgyptCompressionResistance(this UnStiffenedZSection section, Material material, LengthBracingConditions bracingConditions)
        {
            var result = from valid in section.IsValidCompression()
                         select section.AsCompressionDto(material, bracingConditions).AsOutput(section);
            return result;
        }

        #endregion

        private static Validation<bool> IsValidCompression(this LippedSection section)
        {
            var c_over_t = Tuple.Create(section.Properties.CPrime / section.Dimensions.ThicknessT, 40.0);
            var b_over_t = Tuple.Create(section.Properties.BPrime / section.Dimensions.ThicknessT, 60.0);
            var a_over_t = Tuple.Create(section.Properties.APrime / section.Dimensions.ThicknessT, 300.0);

            var allows = new List<Tuple<double, double>>()
            {
                b_over_t,c_over_t,a_over_t
            };
            var result = !allows.Any(tuple => tuple.Item1 > tuple.Item2);
            if (result)
                return true;
            else
                return CantCalculateNominalStrength;
        }


        private static Validation<bool> IsValidCompression(this UnStiffenedSection section)
        {
            var b_over_t = Tuple.Create(section.Properties.BPrime / section.Dimensions.ThicknessT, 40.0);

            var a_over_t = Tuple.Create(section.Properties.APrime / section.Dimensions.ThicknessT, 300.0);

            var allows = new List<Tuple<double, double>>()
            {
                b_over_t,a_over_t
            };
            var result = !allows.Any(tuple => tuple.Item1 > tuple.Item2);
            if (result)
                return true;
            else
                return CantCalculateNominalStrength;
        }


        private static LocalEgyptCompressionDto GetEgyptReducedArea(this LippedSection section, Material material)
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

            if (b_over_t <= s / 3 )
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
            //var items = new List<ReportItem>()
            //{
            //    new ReportItem("Kw",Kw.ToString("0.###"),Units.NONE),
            //    new ReportItem("Effective Height (ae)",ae.ToString("0.###"),Units.CM),
            //    new ReportItem("Kf",Kf.ToString("0.###"),Units.NONE),
            //    new ReportItem("Effective Width (be)",be.ToString("0.###"),Units.CM),
            //    new ReportItem("Kf",Kc.ToString("0.###"),Units.NONE),
            //    new ReportItem("Effective Lip (ce)",ce.ToString("0.###"),Units.CM),
            //    new ReportItem("Effective Area (Ae)",Ae.ToString("0.###"),Units.CM_2),
            //};
            //return Tuple.Create(Ae, items);
            return new LocalEgyptCompressionDto(ae, be, ce, Kw, Kc, Kf, Fy, Ae, Ae * Fy);
        }

        private static LocalEgyptCompressionDto GetEgyptReducedArea(this UnStiffenedSection section, Material material)
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
            //var items = new List<ReportItem>()
            //{
            //    new ReportItem("Kw",Kw.ToString("0.###"),Units.NONE),
            //    new ReportItem("Effective Height (ae)",ae.ToString("0.###"),Units.CM),
            //    new ReportItem("Kf",Kf.ToString("0.###"),Units.NONE),
            //    new ReportItem("Effective Flange Width (be)",be.ToString("0.###"),Units.CM),
            //    new ReportItem("Effective Area (Ae)",Ae.ToString("0.###"),Units.CM_2),
            //};
            //return Tuple.Create(Ae, items);
            return new LocalEgyptCompressionDto(ae, be, 0, Kw, 0, Kf, Fy, Ae, Ae * Fy);
        }

        private static double GetEgyptReducedAreaEE(this LippedSection section, Material material)
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
            var a_ee = (1.92 * t * E_over_Fy_sqrt * (1 - 0.385 * (E_over_Fy_sqrt / a_over_t))).IfNegativeReturn(a_prime).TakeMinWith(a_prime);

            //Flange 
            var b_over_t = b_prime / t;
            var b_ee = (1.92 * t * E_over_Fy_sqrt * (1 - 0.385 * (E_over_Fy_sqrt / b_over_t))).IfNegativeReturn(b_prime).TakeMinWith(b_prime);

            //Lip
            var c_over_t = c_prime / t;
            var c_ee = (0.78 * t * E_over_Fy_sqrt * (1 - (0.13 / c_over_t) * (E_over_Fy_sqrt))).IfNegativeReturn(c_prime).TakeMinWith(c_prime);

            var A_ee = t * (a_ee + 2 * b_ee + 2 * c_ee);
            return A_ee;
        }

        private static double GetEgyptReducedAreaEE(this UnStiffenedSection section, Material material)
        {
            var a_prime = section.Properties.APrime;
            var b_prime = section.Properties.BPrime;
            var t = section.Dimensions.ThicknessT;
            var E = material.E;
            var Fy = material.Fy;

            //Web
            var E_over_Fy_sqrt = Math.Sqrt(E / Fy);
            var a_over_t = a_prime / t;
            var a_ee = (1.92 * t * E_over_Fy_sqrt * (1 - 0.385 * (E_over_Fy_sqrt / a_over_t))).IfNegativeReturn(a_prime).TakeMinWith(a_prime);

            //Flange
            var b_ee = (0.78 * t * E_over_Fy_sqrt * (1 - (0.13 / (b_prime / t)) * E_over_Fy_sqrt)).IfNegativeReturn(b_prime).TakeMinWith(b_prime);


            var A_ee = t * (a_ee + 2 * b_ee);
            return A_ee;
        }

        private static LocalEgyptCompressionDto GetEgyptCompressionLBResistance(this LippedSection section, Material material) =>
             section.GetEgyptReducedArea(material);


        private static LocalEgyptCompressionDto GetEgyptCompressionLBResistance(this UnStiffenedSection section, Material material) =>
             section.GetEgyptReducedArea(material);


        private static FBEgyptCompressionDto GetEgyptCompressionFBResistance(this Section section, Material material, LengthBracingConditions bracingConditions, double Aee)
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
            //var items = new List<ReportItem>()
            //{
            //    new ReportItem("Flexural Stress (Fcr)",Fcr.ToString("0.###"),Units.TON_CM_2),
            //    new ReportItem("Flexural Area (A)",A.ToString("0.###"),Units.CM_2),
            //    new ReportItem("Nominal Load (Pn)",Fcr.ToString("0.###"),Units.TON),

            //};
            //return Tuple.Create(pn, items);
            return new FBEgyptCompressionDto(Fcr, A, pn);
        }


        private static TFBEgyptCompressionDto GetEgyptCompressionTFBResistance(this Section section, Material material, LengthBracingConditions bracingConditions, double Aee)
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
            //var items = new List<ReportItem>()
            //{
            //    new ReportItem("Torsional Flexural Stress (Fcr)",Fcr.ToString("0.###"),Units.TON_CM_2),
            //    new ReportItem("Torsional Flexural Area (A)",A.ToString("0.###"),Units.CM_2),
            //    new ReportItem("Nominal Load (Pn)",pn.ToString("0.###"),Units.TON),
            //};
            //return Tuple.Create(pn, items);
            return new TFBEgyptCompressionDto(Fcr, A, pn);
        }

        private static TBEgyptCompressionDto GetEgyptCompressionTBResistance(this Section section, Material material, LengthBracingConditions bracingConditions, double Aee)
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
            //var items = new List<ReportItem>()
            //{
            //    new ReportItem("Torsional Buckling Stress (Fcrt)",Fcrt.ToString("0.###"),Units.TON_CM_2),
            //    new ReportItem("Torsional  Area (A)",A.ToString("0.###"),Units.CM_2),
            //    new ReportItem("Nominal Load (Pn)",pn.ToString("0.###"),Units.TON),
            //};
            //return Tuple.Create(pn, items);
            return new TBEgyptCompressionDto(Fcrt, A, pn);
        }

        private static EgyptCompressionCDto AsCompressionDto(this LippedCSection section, Material material, LengthBracingConditions bracingConditions)
        {
            var lbDto = section.GetEgyptCompressionLBResistance(material);
            var Aee = section.GetEgyptReducedAreaEE(material);
            var fbDto = section.GetEgyptCompressionFBResistance(material, bracingConditions, Aee);
            var tfbDto = section.GetEgyptCompressionTFBResistance(material, bracingConditions, Aee);
            var tbDto = section.GetEgyptCompressionTBResistance(material, bracingConditions, Aee);
            return new EgyptCompressionCDto(lbDto, fbDto, tfbDto, tbDto);
        }

        private static EgyptCompressionCDto AsCompressionDto(this UnStiffenedCSection section, Material material, LengthBracingConditions bracingConditions)
        {
            var lbDto = section.GetEgyptCompressionLBResistance(material);
            var Aee = section.GetEgyptReducedAreaEE(material);
            var fbDto = section.GetEgyptCompressionFBResistance(material, bracingConditions, Aee);
            var tfbDto = section.GetEgyptCompressionTFBResistance(material, bracingConditions, Aee);
            var tbDto = section.GetEgyptCompressionTBResistance(material, bracingConditions, Aee);
            return new EgyptCompressionCDto(lbDto, fbDto, tfbDto, tbDto);
        }

        private static CompressionResistanceOutput AsOutput(this EgyptCompressionCDto dto, LippedCSection section)
        {
            var report = dto.AsReport(section);
            return new CompressionResistanceOutput(dto.GoverningCase.NominalStrength, PHI_C_EGYPT, PHI_C_NAME_EGYPT, COMP_DESIGN_RESIST_EGYPT, dto.GoverningCase.FailureMode, "ton", report);
        }

        private static CompressionResistanceOutput AsOutput(this EgyptCompressionCDto dto, UnStiffenedCSection section)
        {
            var report = dto.AsReport(section);
            return new CompressionResistanceOutput(dto.GoverningCase.NominalStrength, PHI_C_EGYPT, PHI_C_NAME_EGYPT, COMP_DESIGN_RESIST_EGYPT, dto.GoverningCase.FailureMode, "ton", report);
        }

        public static Validation<CompressionResistanceOutput> AsEgyptCompressionResistance(this LippedCSection section, Material material, LengthBracingConditions bracingConditions)
        {
            var result = from valid in section.IsValidCompression()
                         select section.AsCompressionDto(material, bracingConditions).AsOutput(section);
            return result;
        }

        public static Validation<CompressionResistanceOutput> AsEgyptCompressionResistance(this UnStiffenedCSection section, Material material, LengthBracingConditions bracingConditions)
        {
            var result = from valid in section.IsValidCompression()
                         select section.AsCompressionDto(material, bracingConditions).AsOutput(section);
            return result;
        }

#endregion

    }
}
