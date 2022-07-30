﻿using ColdFormedChannelSection.Core.Dtos;
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
    public static class EuroHelper
    {

        private static Validation<TypeOfSection> IsValid(this LippedSection section, Material material)
        {
            var t = section.Dimensions.ThicknessT;
            var E = material.E;
            var Fy = material.Fy;
            if (section.Dimensions.InternalRadiusR > 0.04 * t * (E / Fy))
                return EuroInternalRadiusError;

            var b_over_t = Tuple.Create(section.Properties.BPrime / section.Dimensions.ThicknessT, 60.0);

            var c_over_t = Tuple.Create(section.Properties.CPrime / section.Dimensions.ThicknessT, 50.0);
            var a_over_t = Tuple.Create(section.Properties.APrime / section.Dimensions.ThicknessT, 500.0);
            var c_over_b = section.Properties.CPrime / section.Properties.BPrime;

            var allows = new List<Tuple<double, double>>()
            {
                b_over_t,c_over_t,a_over_t
            };
            var result = !allows.Any(tuple => tuple.Item1 > tuple.Item2) && c_over_b >= 0.2 && c_over_b <= 0.6;
            if (result)
                return TypeOfSection.LIPPED;
            else if (c_over_b < 0.2)
                return TypeOfSection.UNSTIFFENED;
            else
                return CantCalculateNominalStrength;
        }

        private static Validation<bool> IsValid(this UnStiffenedSection section, Material material)
        {
            var t = section.Dimensions.ThicknessT;
            var E = material.E;
            var Fy = material.Fy;
            if (section.Dimensions.InternalRadiusR > 0.04 * t * (E / Fy))
                return EuroInternalRadiusError;

            var b_over_t = Tuple.Create(section.Properties.BPrime / section.Dimensions.ThicknessT, 50.0);

            var a_over_t = Tuple.Create(section.Properties.APrime / section.Dimensions.ThicknessT, 500.0);

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


        #region Moment & Compression

        #region Z Sections

        public static Validation<ResistanceInteractionOutput> AsEuroInteractionResistance(this LippedZSection section, Material material, LengthBracingConditions bracingConditions, double pu, double mu)
        {
            var result = from Pn in section.AsEuroCompressionResistance(material, bracingConditions)
                         from Mn in section.AsEuroMomentResistance(material, bracingConditions, mu)
                         select section.AsEuroInteractionResistance(Pn, Mn, pu, mu);
            return result;
        }

        public static Validation<ResistanceInteractionOutput> AsEuroInteractionResistance(this UnStiffenedZSection section, Material material, LengthBracingConditions bracingConditions, double pu, double mu)
        {
            var result = from Pn in section.AsEuroCompressionResistance(material, bracingConditions)
                         from Mn in section.AsEuroMomentResistance(material, bracingConditions, mu)
                         select section.AsEuroInteractionResistance(Pn, Mn, pu, mu);
            return result;
        }

        #endregion

        private static ResistanceInteractionOutput AsEuroInteractionResistance(this Section section, CompressionResistanceOutput Pn, MomentResistanceOutput Mn, double pu, double mu)
        {
            //tex:
            //$$ (\frac{P_u}{ P_n})^{0.8} + (\frac{M_u}{M_n})^{0.8}  $$
            var ie = (pu / Pn.NominalResistance).Power(0.8) + (mu / Mn.NominalResistance).Power(0.8);

            var sections = Pn.Report.Sections.Take(2).Concat(Pn.Report.Sections.Skip(2).Select(sec => sec.AppendToName("Compression")))
                                                .Concat(Mn.Report.Sections.Skip(2).Select(sec => sec.AppendToName("Moment")))
                                                .ToList();

            var report = new Report(UnitSystems.NMM, "Euro Code - Interaction", sections);

            return new ResistanceInteractionOutput(pu, Pn.NominalResistance, mu, Mn.NominalResistance, "(\\frac{P_u}{ P_n})^{0.8} + (\\frac{M_u}{M_n})^{0.8}", ie, "N.mm", "N", report);
        }

        public static Validation<ResistanceInteractionOutput> AsEuroInteractionResistance(this LippedCSection section, Material material, LengthBracingConditions bracingConditions, double pu, double mu)
        {
            var result = from Pn in section.AsEuroCompressionResistance(material, bracingConditions, pu)
                         from Mn in section.AsEuroMomentResistance(material, bracingConditions, mu)
                         select section.AsEuroInteractionResistance(Pn, Mn, pu, mu);
            return result;
        }

        public static Validation<ResistanceInteractionOutput> AsEuroInteractionResistance(this UnStiffenedCSection section, Material material, LengthBracingConditions bracingConditions, double pu, double mu)
        {
            var result = from Pn in section.AsEuroCompressionResistance(material, bracingConditions, pu)
                         from Mn in section.AsEuroMomentResistance(material, bracingConditions, mu)
                         select section.AsEuroInteractionResistance(Pn, Mn, pu, mu);
            return result;
        }

        #endregion

        #region Compression

        #region Z Sections

        private static EuroCompressionZDto AsCompressionDto(this LippedZSection section, Material material, LengthBracingConditions bracingConditions)
        {
            var lbDto = section.GetEuroReducedArea(material);
            //var pn1 = Tuple.Create(section.GetEuroCompressionLBResistance(material, Ae), FailureMode.LOCALBUCKLING);
            var fbDto = section.GetEuroCompressionFBResistance(material, bracingConditions, lbDto.AreaEffective, 0.34);
            var tbDto = section.GetEuroCompressionTBResistance(material, bracingConditions, lbDto.AreaEffective, 0.34);
            //var ftbDto = section.GetEuroCompressionTFBResistance(material, bracingConditions, lbDto.AreaEffective, 0.34, pu);
            return new EuroCompressionZDto(lbDto, fbDto, tbDto);
        }

        private static EuroCompressionZDto AsCompressionDto(this UnStiffenedZSection section, Material material, LengthBracingConditions bracingConditions)
        {
            var lbDto = section.GetEuroReducedArea(material);
            //var pn1 = Tuple.Create(section.GetEuroCompressionLBResistance(material, Ae), FailureMode.LOCALBUCKLING);
            var fbDto = section.GetEuroCompressionFBResistance(material, bracingConditions, lbDto.AreaEffective, 0.49);
            var tbDto = section.GetEuroCompressionTBResistance(material, bracingConditions, lbDto.AreaEffective, 0.49);
            //var ftbDto = section.GetEuroCompressionTFBResistance(material, bracingConditions, lbDto.AreaEffective, 0.49, pu);
            return new EuroCompressionZDto(lbDto, fbDto, tbDto);
        }

        private static CompressionResistanceOutput AsOutput(this EuroCompressionZDto dto, LippedZSection section)
        {
            var report = dto.AsReport(section);
            return new CompressionResistanceOutput(dto.GoverningCase.NominalStrength, PHI_EURO, PHI_NAME_EURO, COMP_DESIGN_RESIST_EURO, dto.GoverningCase.FailureMode, "N", report);
        }

        private static CompressionResistanceOutput AsOutput(this EuroCompressionZDto dto, UnStiffenedZSection section)
        {
            var report = dto.AsReport(section);
            return new CompressionResistanceOutput(dto.GoverningCase.NominalStrength, PHI_EURO, PHI_NAME_EURO, COMP_DESIGN_RESIST_EURO, dto.GoverningCase.FailureMode, "N", report);
        }

        public static Validation<CompressionResistanceOutput> AsEuroCompressionResistance(this LippedZSection section, Material material, LengthBracingConditions bracingConditions)
        {
            var dict = new Dictionary<TypeOfSection, Lazy<Validation<CompressionResistanceOutput>>>()
            {
                [TypeOfSection.LIPPED] = new Lazy<Validation<CompressionResistanceOutput>>(() => section.AsCompressionDto(material, bracingConditions).AsOutput(section)),
                [TypeOfSection.UNSTIFFENED] = new Lazy<Validation<CompressionResistanceOutput>>(() => section.Dimensions.AsNewWithC(0).AsUnstiffenedZSection().Bind(sec => sec.AsEuroCompressionResistance(material, bracingConditions)))
            };

            var result = from typeOfSection in section.IsValid(material)
                         from val in dict[typeOfSection].Value
                         select val;
            return result;
        }

        public static Validation<CompressionResistanceOutput> AsEuroCompressionResistance(this UnStiffenedZSection section, Material material, LengthBracingConditions bracingConditions)
        {
            var result = from valid in section.IsValid(material)
                         select section.AsCompressionDto(material, bracingConditions).AsOutput(section);
            return result;
        }

        #endregion

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

        private static (double be1, double be2, double Kf) GetEuroReducedFlange(this UnStiffenedSection section, Material material)
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
            return (be1, be2, kf);
        }

        private static LocalEuroCompressionDto GetEuroReducedArea(this LippedSection section, Material material)
        {
            var t = section.Dimensions.ThicknessT;
            var R = section.Dimensions.InternalRadiusR;
            var bPrime = section.Properties.BPrime;
            (var be1, var be2, var ce, var Xd, var Kf, var Kc) = section.GetEuroReducedFlange(material, 1);

            //Web.
            var Kw = 4.0;
            var ae = section.ReduceWebCompression(material);

            var Ae = t * (2 * be1 + ae + 2 * Xd * (be2 + ce));
            if (R > 5 * t || R > 0.1 * bPrime)
            {
                var n = 2;
                var sumLe = 2 * be1 + ae + 2 * be2 + 2 * ce;
                var delta = 0.43 * ((n * R) / sumLe);
                Ae = Ae * (1 - delta);
            }

            return new LocalEuroCompressionDto(ae, (be1 + be2), ce, Kw, Kf, Kc, material.Fy, Ae, (material.Fy * Ae), Xd, FailureMode.LOCAL_DISTORSIONAL_BUCKLING);
        }

        private static LocalEuroCompressionDto GetEuroReducedArea(this UnStiffenedSection section, Material material)
        {
            var t = section.Dimensions.ThicknessT;
            var R = section.Dimensions.InternalRadiusR;
            var bPrime = section.Properties.BPrime;

            (var be1, var be2, var Kf) = section.GetEuroReducedFlange(material);

            //Web.
            var Kw = 4.0;
            var ae = section.ReduceWebCompression(material);

            var Ae = t * (2 * be1 + ae + 2 * be2);

            if (R > 5 * t || R > 0.1 * bPrime)
            {
                var n = 2;
                var sumLe = 2 * be1 + ae + 2 * be2;
                var delta = 0.43 * ((n * R) / sumLe);
                Ae = Ae * (1 - delta);
            }
            return new LocalEuroCompressionDto(ae, (be1 + be2), 0, Kw, Kf, 0, material.Fy, Ae, (material.Fy * Ae), 1, FailureMode.LOCALBUCKLING);
        }

        private static EuroCompressionCDto AsCompressionDto(this LippedCSection section, Material material, LengthBracingConditions bracingConditions, double pu)
        {
            var lbDto = section.GetEuroReducedArea(material);
            //var pn1 = Tuple.Create(section.GetEuroCompressionLBResistance(material, Ae), FailureMode.LOCALBUCKLING);
            var fbDto = section.GetEuroCompressionFBResistance(material, bracingConditions, lbDto.AreaEffective, 0.34);
            var tbDto = section.GetEuroCompressionTBResistance(material, bracingConditions, lbDto.AreaEffective, 0.34);
            var ftbDto = section.GetEuroCompressionTFBResistance(material, bracingConditions, lbDto.AreaEffective, 0.34);
            return new EuroCompressionCDto(lbDto, fbDto, tbDto, ftbDto);
        }

        private static EuroCompressionCDto AsCompressionDto(this UnStiffenedCSection section, Material material, LengthBracingConditions bracingConditions, double pu)
        {
            var lbDto = section.GetEuroReducedArea(material);
            //var pn1 = Tuple.Create(section.GetEuroCompressionLBResistance(material, Ae), FailureMode.LOCALBUCKLING);
            var fbDto = section.GetEuroCompressionFBResistance(material, bracingConditions, lbDto.AreaEffective, 0.49);
            var tbDto = section.GetEuroCompressionTBResistance(material, bracingConditions, lbDto.AreaEffective, 0.49);
            var ftbDto = section.GetEuroCompressionTFBResistance(material, bracingConditions, lbDto.AreaEffective, 0.49);
            return new EuroCompressionCDto(lbDto, fbDto, tbDto, ftbDto);
        }

        private static CompressionResistanceOutput AsOutput(this EuroCompressionCDto dto, LippedCSection section)
        {
            var report = dto.AsReport(section);
            return new CompressionResistanceOutput(dto.GoverningCase.NominalStrength, PHI_EURO, PHI_NAME_EURO, COMP_DESIGN_RESIST_EURO, dto.GoverningCase.FailureMode, "N", report);
        }

        private static CompressionResistanceOutput AsOutput(this EuroCompressionCDto dto, UnStiffenedCSection section)
        {
            var report = dto.AsReport(section);
            return new CompressionResistanceOutput(dto.GoverningCase.NominalStrength, PHI_EURO, PHI_NAME_EURO, COMP_DESIGN_RESIST_EURO, dto.GoverningCase.FailureMode, "N", report);
        }

        public static Validation<CompressionResistanceOutput> AsEuroCompressionResistance(this LippedCSection section, Material material, LengthBracingConditions bracingConditions, double pu)
        {
            var dict = new Dictionary<TypeOfSection, Lazy<Validation<CompressionResistanceOutput>>>()
            {
                [TypeOfSection.LIPPED] = new Lazy<Validation<CompressionResistanceOutput>>(() => section.AsCompressionDto(material, bracingConditions, pu).AsOutput(section)),
                [TypeOfSection.UNSTIFFENED] = new Lazy<Validation<CompressionResistanceOutput>>(() => section.Dimensions.AsNewWithC(0).AsUnstiffenedCSection().Bind(sec => sec.AsEuroCompressionResistance(material, bracingConditions, pu)))
            };

            var result = from typeOfSection in section.IsValid(material)
                         from val in dict[typeOfSection].Value
                         select val;
            return result;
        }

        public static Validation<CompressionResistanceOutput> AsEuroCompressionResistance(this UnStiffenedCSection section, Material material, LengthBracingConditions bracingConditions, double pu)
        {
            var result = from valid in section.IsValid(material)
                         select section.AsCompressionDto(material, bracingConditions, pu).AsOutput(section);
            return result;
        }


        private static FBEuroCompressionDto GetEuroCompressionFBResistance(this Section section, Material material, LengthBracingConditions lengthBracingConditions, double Ae, double alpa_w)
        {
            var E = material.E;
            var Fy = material.Fy;
            var ky = lengthBracingConditions.Ky;
            var Ly = lengthBracingConditions.Ly;
            var Kx = lengthBracingConditions.Kx;
            var Lx = lengthBracingConditions.Lx;
            var ix = section.Properties.Rx;
            var iy = section.Properties.Ry;
            var Ix = section.Properties.Ix;
            var Iy = section.Properties.Iy;
            var A = section.Properties.A;




            var Ncr_x = (Math.PI.Power(2) * E * Ix) / (Kx * Lx).Power(2);
            var Ncr_y = (Math.PI.Power(2) * E * Iy) / (ky * Ly).Power(2);

            var Ncr_f = Ncr_x.TakeMinWith(Ncr_y);
            var lambdaPrime = Math.Sqrt((Ae * Fy) / Ncr_f);

            var X = 1.0;
            if (lambdaPrime > 0.2)
            {
                var phiF = 0.5 * (1 + alpa_w * (lambdaPrime - 0.2) + lambdaPrime.Power(2));
                X = (1 / (phiF + Math.Sqrt(phiF.Power(2) - lambdaPrime.Power(2)))).TakeMinWithOne();
            }

            var Pn = X * Ae * Fy;

            return new FBEuroCompressionDto(Ae, X, material.Fy, Pn);
        }

        private static TBEuroCompressionDto GetEuroCompressionTBResistance(this Section section, Material material, LengthBracingConditions lengthBracingConditions, double Ae, double alpha_w)
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
            //if (pu != 0 && pu / Ncr <= 0.04)
            //{
            //    Xt = 1;
            //}
            //var Pn = Xt * Ae * Fy;

            if (lambda_t <= 0.2)
            {
                Xt = 1;
            }
            var Pn = Xt * Ae * Fy;
            return new TBEuroCompressionDto(Xt, material.Fy, Ae, Pn);
        }

        private static FTBEuroCompressionDto GetEuroCompressionTFBResistance(this Section section, Material material, LengthBracingConditions lengthBracingConditions, double Ae, double alpha_w)
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
            var Ix = section.Properties.Ix;

            var xo_squared = Xo.Power(2);
            var io_squared = ix.Power(2) + iy.Power(2) + xo_squared;
            var beta = 1 - (xo_squared / io_squared);
            var Ncr = (1 / io_squared) * (G * J + ((Math.PI.Power(2) * Cw * E) / (Kz * Lz).Power(2)));
            var Ncr_x = (Math.PI.Power(2) * E*Ix) / (Kx * Lx).Power(2);
            var Ncr_ft = (Ncr_x / (2 * beta)) * (1 + (Ncr / Ncr_x) - Math.Sqrt((1 - (Ncr / Ncr_x)).Power(2) + 4 * (xo_squared / io_squared) * (Ncr / Ncr_x)));
            var lambda_ft = Math.Sqrt((Ae * Fy) / (Ncr_ft));
            var phi_ft = 0.5 * (1 + alpha_w * (lambda_ft - 0.2) + lambda_ft.Power(2));
            var X_ft = ((1.0) / (phi_ft + Math.Sqrt(phi_ft.Power(2) - lambda_ft.Power(2)))).TakeMinWithOne();
            //if (pu != 0 && pu / Ncr_ft <= 0.04)
            //{
            //    X_ft = 1;
            //}
            if (lambda_ft <= 0.2)
            {
                X_ft = 1;
            }
            var Pn = X_ft * Ae * Fy;

            return new FTBEuroCompressionDto(X_ft, material.Fy, Ae, Pn);
        }


        #endregion


        #region Moment

        #region Z Sections

        private static EuroMomentDto AsMomentDto(this LippedZSection section, Material material, LengthBracingConditions bracingConditions, double mu)
        {
            var lbDto = section.GetZe(material);
            var ltb = section.GetEuroMomentLTBResistance(material, bracingConditions, lbDto.Ze, mu);
            return new EuroMomentDto(lbDto, ltb);
        }

        private static EuroMomentDto AsMomentDto(this UnStiffenedZSection section, Material material, LengthBracingConditions bracingConditions, double mu)
        {
            var lbDto = section.GetZe(material);
            //var lb = section.GetEuroMomentLBResistance(material, lbDto.Ze);
            var ltb = section.GetEuroMomentLTBResistance(material, bracingConditions, lbDto.Ze, mu);
            return new EuroMomentDto(lbDto, ltb);
        }

        private static MomentResistanceOutput AsOutput(this EuroMomentDto dto, LippedZSection section)
        {
            var report = dto.AsReport(section);
            return new MomentResistanceOutput(dto.GoverningCase.NominalStrength, PHI_EURO, PHI_NAME_EURO, MOM_DESIGN_RESIST_EURO, dto.GoverningCase.FailureMode, "N.mm", report);
        }

        private static MomentResistanceOutput AsOutput(this EuroMomentDto dto, UnStiffenedZSection section)
        {
            var report = dto.AsReport(section);
            return new MomentResistanceOutput(dto.GoverningCase.NominalStrength, PHI_EURO, PHI_NAME_EURO, MOM_DESIGN_RESIST_EURO, dto.GoverningCase.FailureMode, "N.mm", report);
        }

        public static Validation<MomentResistanceOutput> AsEuroMomentResistance(this LippedZSection section, Material material, LengthBracingConditions bracingConditions, double mu)
        {
            var dict = new Dictionary<TypeOfSection, Lazy<Validation<MomentResistanceOutput>>>()
            {
                [TypeOfSection.LIPPED] = new Lazy<Validation<MomentResistanceOutput>>(() => section.AsMomentDto(material, bracingConditions, mu).AsOutput(section)),
                [TypeOfSection.UNSTIFFENED] = new Lazy<Validation<MomentResistanceOutput>>(() => section.Dimensions.AsNewWithC(0).AsUnstiffenedZSection().Bind(sec => sec.AsEuroMomentResistance(material, bracingConditions, mu)))
            };

            var result = from typeOfSection in section.IsValid(material)
                         from val in dict[typeOfSection].Value
                         select val;
            return result;
        }

        public static Validation<MomentResistanceOutput> AsEuroMomentResistance(this UnStiffenedZSection section, Material material, LengthBracingConditions bracingConditions, double mu)
        {
            var result = from valid in section.IsValid(material)
                         select section.AsMomentDto(material, bracingConditions, mu).AsOutput(section);
            return result;
        }

        #endregion

        private static EuroMomentDto AsMomentDto(this LippedCSection section, Material material, LengthBracingConditions bracingConditions, double mu)
        {
            var lbDto = section.GetZe(material);
            //var lb = section.GetEuroMomentLBResistance(material, lbDto.Ze);
            var ltb = section.GetEuroMomentLTBResistance(material, bracingConditions, lbDto.Ze, mu);
            return new EuroMomentDto(lbDto, ltb);
        }

        private static EuroMomentDto AsMomentDto(this UnStiffenedCSection section, Material material, LengthBracingConditions bracingConditions, double mu)
        {
            var lbDto = section.GetZe(material);
            //var lb = section.GetEuroMomentLBResistance(material, lbDto.Ze);
            var ltb = section.GetEuroMomentLTBResistance(material, bracingConditions, lbDto.Ze, mu);
            return new EuroMomentDto(lbDto, ltb);
        }

        private static MomentResistanceOutput AsOutput(this EuroMomentDto dto, LippedCSection section)
        {
            var report = dto.AsReport(section);
            return new MomentResistanceOutput(dto.GoverningCase.NominalStrength, PHI_EURO, PHI_NAME_EURO, MOM_DESIGN_RESIST_EURO, dto.GoverningCase.FailureMode, "N.mm", report);
        }

        private static MomentResistanceOutput AsOutput(this EuroMomentDto dto, UnStiffenedCSection section)
        {
            var report = dto.AsReport(section);
            return new MomentResistanceOutput(dto.GoverningCase.NominalStrength, PHI_EURO, PHI_NAME_EURO, MOM_DESIGN_RESIST_EURO, dto.GoverningCase.FailureMode, "N.mm", report);
        }

        public static Validation<MomentResistanceOutput> AsEuroMomentResistance(this LippedCSection section, Material material, LengthBracingConditions bracingConditions, double mu)
        {
            var dict = new Dictionary<TypeOfSection, Lazy<Validation<MomentResistanceOutput>>>()
            {
                [TypeOfSection.LIPPED] = new Lazy<Validation<MomentResistanceOutput>>(() => section.AsMomentDto(material, bracingConditions, mu).AsOutput(section)),
                [TypeOfSection.UNSTIFFENED] = new Lazy<Validation<MomentResistanceOutput>>(() => section.Dimensions.AsNewWithC(0).AsUnstiffenedCSection().Bind(sec => sec.AsEuroMomentResistance(material, bracingConditions, mu)))
            };

            var result = from typeOfSection in section.IsValid(material)
                         from val in dict[typeOfSection].Value
                         select val;
            return result;
        }

        public static Validation<MomentResistanceOutput> AsEuroMomentResistance(this UnStiffenedCSection section, Material material, LengthBracingConditions bracingConditions, double mu)
        {
            var result = from valid in section.IsValid(material)
                         select section.AsMomentDto(material, bracingConditions, mu).AsOutput(section);
            return result;
        }

        private static LTBEuroMomentDto GetEuroMomentLTBResistance(this Section section, Material material, LengthBracingConditions bracingConditions, double Ze, double mu)
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
            var x_lt = 1.0;
            var Mn = x_lt * Ze * Fy;

            if (Lu > 0)
            {
                var Mcr = C1 * ((Math.PI.Power(2) * E * Iy) / (Lu.Power(2))) * ((Cw / Iy) + ((Lu.Power(2) * G * J) / (Math.PI.Power(2) * E * Iy))).Power(0.5);

                var alpha_lt = 0.21;
                var lambda_lt = Math.Sqrt((Ze * Fy) / (Mcr));
                var phi_lt = 0.5 * (1 + alpha_lt * (lambda_lt - 0.2) + lambda_lt.Power(2));
                x_lt = Math.Min(1.0, (1 / (phi_lt + Math.Sqrt(phi_lt.Power(2) - lambda_lt.Power(2)))));
                if (mu == 0 && lambda_lt <= 0.2)
                    x_lt = 1;
                else if (mu != 0 && (lambda_lt <= 0.2 || mu / Mcr <= 0.16))
                    x_lt = 1;
                Mn = x_lt * Ze * Fy;
            }

            return new LTBEuroMomentDto(Ze, x_lt * Fy, x_lt, Mn);
        }


        private static LocalEuroMomentDto GetZe(this Section section, Material material, double be1, double be2, double ce, double Xd, double n, double kf, double kc)
        {
            var c_prime = section.Properties.CPrime;
            var b_prime = section.Properties.BPrime;
            var a_prime = section.Properties.APrime;
            var t = section.Dimensions.ThicknessT;
            var Fy = material.Fy;
            var R = section.Dimensions.InternalRadiusR;


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

            if (R > 5 * t || R > 0.1 * b_prime)
            {
                var sumLe = be1 + be2 + ce + c_prime + b_prime + h1 + h2;
                var delta = 0.43 * ((n * R) / sumLe);
                Ieff = Ieff * (1 - 2 * delta);
            }

            var Ze = Ieff / y_bar;

            return new LocalEuroMomentDto(
                ze: Ze,
                fy: material.Fy,
                ae: ae,
                be: (be1 + be2),
                ce: ce,
                xd: Xd,
                mn: (Ze * material.Fy),
                kw: kw,
                kf: kf,
                kc: kc
                );


        }

        private static LocalEuroMomentDto GetZe(this LippedSection section, Material material)
        {
            var t = section.Dimensions.ThicknessT;
            (var be1, var be2, var ce, var Xd, var Kf, var Kc) = section.GetEuroReducedFlange(material, 0);

            var dto = section.GetZe(material, be1, be2, ce, Xd, 4, Kf, Kc);

            return dto;
        }

        private static LocalEuroMomentDto GetZe(this UnStiffenedSection section, Material material)
        {
            var t = section.Dimensions.ThicknessT;
            (var be1, var be2, var Kf) = section.GetEuroReducedFlange(material);

            var dto = section.GetZe(material, be1, be2, 0, 1.0, 2, Kf, 0);

            return dto;
        }

        #endregion

    }
}
