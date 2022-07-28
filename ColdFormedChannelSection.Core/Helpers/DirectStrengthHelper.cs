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
    public static class DirectStrengthHelper
    {



        #region Moment & Compression

        #region Z Sections

        public static Validation<ResistanceInteractionOutput> AsDSInteractionResistance(this LippedZSection section, Material material, LengthBracingConditions bracingConditions, double pu, double mu)
        {
            var result = from Pn in section.AsDSCompressionResistance(material, bracingConditions)
                         from Mn in section.AsDSMomentResistance(material, bracingConditions)
                         select section.AsDSInteractionResistance(material, bracingConditions, pu, mu, Pn, Mn, () => section.GetAISIReducedArea(material).Ae);
            return result;
        }

        public static Validation<ResistanceInteractionOutput> AsDSInteractionResistance(this UnStiffenedZSection section, Material material, LengthBracingConditions bracingConditions, double pu, double mu)
        {
            var result = from Pn in section.AsDSCompressionResistance(material, bracingConditions)
                         from Mn in section.AsDSMomentResistance(material, bracingConditions)
                         select section.AsDSInteractionResistance(material, bracingConditions, pu, mu, Pn, Mn, () => section.GetAISIReducedArea(material).Ae);
            return result;
        }

        #endregion

        private static ResistanceInteractionOutput AsDSInteractionResistance(this Section section, Material material, LengthBracingConditions bracingconditions, double pu, double mu, CompressionResistanceOutput pn_out, MomentResistanceOutput mn_out, Func<double> getAe)
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

            var loadRatio = (pu / (PHI_C_DS * pn));
            var ieName = "";
            var ie = 0.0;
            if (loadRatio <= 0.15)
            {
                ie = (pu / (PHI_C_DS * pn)) + (mu / (PHI_B_DS * mn));
                //tex:
                //$$ \frac {P_u}{\phi_c P_n} + \frac {M_u} {\phi_b M_n} $$
                ieName = "\\frac {P_u}{\\phi_c P_n} + \\frac {M_u} {\\phi_b M_n}";
            }
            else
            {
                var ie_1 = (pu / (PHI_C_DS * pn)) + ((Cm * mu) / (PHI_B_DS * mn * alpha_x));
                var Ae = getAe();
                var Pno = Ae * Fy;
                var ie_2 = (pu / (PHI_C_DS * Pno)) + (mu / (PHI_B_DS * mn));
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
            var sections = pn_out.Report.Sections.Take(2).Concat(pn_out.Report.Sections.Skip(2).Select(sec => sec.AppendToName("Compression")))
                                                 .Concat(mn_out.Report.Sections.Skip(2).Select(sec => sec.AppendToName("Moment")))
                                                 .ToList();

            var report = new Report(UnitSystems.KIPINCH, "Direct Strength - Interaction", sections);
            return new ResistanceInteractionOutput(pu, pn, mu, mn, ieName, ie, "kip.in", "kip", report);
        }

        public static Validation<ResistanceInteractionOutput> AsDSInteractionResistance(this LippedCSection section, Material material, LengthBracingConditions bracingConditions, double pu, double mu)
        {
            var result = from Pn in section.AsDSCompressionResistance(material, bracingConditions)
                         from Mn in section.AsDSMomentResistance(material, bracingConditions)
                         select section.AsDSInteractionResistance(material, bracingConditions, pu, mu, Pn, Mn, () => section.GetAISIReducedArea(material).Ae);
            return result;
        }

        public static Validation<ResistanceInteractionOutput> AsDSInteractionResistance(this UnStiffenedCSection section, Material material, LengthBracingConditions bracingConditions, double pu, double mu)
        {
            var result = from Pn in section.AsDSCompressionResistance(material, bracingConditions)
                         from Mn in section.AsDSMomentResistance(material, bracingConditions)
                         select section.AsDSInteractionResistance(material, bracingConditions, pu, mu, Pn, Mn, () => section.GetAISIReducedArea(material).Ae);
            return result;
        }

        #endregion

        #region Compression


        #region Z Sections

        public static Validation<CompressionResistanceOutput> AsDSCompressionResistance(this LippedZSection section, Material material, LengthBracingConditions bracingConditions)
        {
            var result = from valid in section.IsValidForCompression()
                         select section.AsCompressionDto(material, bracingConditions).AsOutput(section);
            return result;
        }

        public static Validation<CompressionResistanceOutput> AsDSCompressionResistance(this UnStiffenedZSection section, Material material, LengthBracingConditions bracingConditions)
        {
            var result = from valid in section.IsValidForCompression()
                         select section.AsCompressionDto(material, bracingConditions).AsOutput(section);
            return result;
        }

        private static CompressionResistanceOutput AsOutput(this DSLippedCompressionDto dto, LippedZSection section)
        {
            var report = dto.AsReport(section);
            return new CompressionResistanceOutput(dto.GoverningCase.NominalStrength, PHI_C_DS, PHI_C_NAME_DS, COMP_DESIGN_RESIST_DS, dto.GoverningCase.FailureMode, "Kip", report);
        }

        private static CompressionResistanceOutput AsOutput(this DSUnStiffenedCompressionDto dto, UnStiffenedZSection section)
        {
            var report = dto.AsReport(section);
            return new CompressionResistanceOutput(dto.GoverningCase.NominalStrength, PHI_C_DS, PHI_C_NAME_DS, COMP_DESIGN_RESIST_DS, dto.GoverningCase.FailureMode, "Kip", report);
        }

        private static DSLippedCompressionDto AsCompressionDto(this LippedZSection section, Material material, LengthBracingConditions bracingConditions)
        {
            var p_crl = section.GetCompressionLBResistance(material);
            return section.AsCompressionDto(material, bracingConditions, p_crl, (lst) => section.GetLippedCompressionDto(material, bracingConditions, lst, p_crl)) as DSLippedCompressionDto;
        }

        private static DSUnStiffenedCompressionDto AsCompressionDto(this UnStiffenedZSection section, Material material, LengthBracingConditions bracingConditions)
        {
            var p_crl = section.GetCompressionLBResistance(material);
            return section.AsCompressionDto(material, bracingConditions, p_crl, (lst) => section.GetUnstiffenedCompressionDto(material, bracingConditions, lst, p_crl)) as DSUnStiffenedCompressionDto;
        }

        #endregion

        public static Validation<CompressionResistanceOutput> AsDSCompressionResistance(this LippedCSection lippedSection, Material material, LengthBracingConditions bracingConditions)
        {
            var result = from valid in lippedSection.IsValidForCompression()
                         select lippedSection.AsCompressionDto(material, bracingConditions).AsOutput(lippedSection);
            return result;
        }

        public static Validation<CompressionResistanceOutput> AsDSCompressionResistance(this UnStiffenedCSection section, Material material, LengthBracingConditions bracingConditions)
        {
            var result = from valid in section.IsValidForCompression()
                         select section.AsCompressionDto(material, bracingConditions).AsOutput(section);
            return result;
        }

        private static CompressionResistanceOutput AsOutput(this DSLippedCompressionDto dto, LippedCSection section)
        {
            var report = dto.AsReport(section);
            return new CompressionResistanceOutput(dto.GoverningCase.NominalStrength, PHI_C_DS, PHI_C_NAME_DS, COMP_DESIGN_RESIST_DS, dto.GoverningCase.FailureMode, "Kip", report);
        }

        private static CompressionResistanceOutput AsOutput(this DSUnStiffenedCompressionDto dto, UnStiffenedCSection section)
        {
            var report = dto.AsReport(section);
            return new CompressionResistanceOutput(dto.GoverningCase.NominalStrength, PHI_C_DS, PHI_C_NAME_DS, COMP_DESIGN_RESIST_DS, dto.GoverningCase.FailureMode, "Kip", report);
        }

        private static DSLippedCompressionDto AsCompressionDto(this LippedCSection section, Material material, LengthBracingConditions bracingConditions)
        {
            var p_crl = section.GetCompressionLBResistance(material);
            return section.AsCompressionDto(material, bracingConditions, p_crl, (lst) => section.GetLippedCompressionDto(material, bracingConditions, lst, p_crl)) as DSLippedCompressionDto;
        }

        private static DSUnStiffenedCompressionDto AsCompressionDto(this UnStiffenedCSection section, Material material, LengthBracingConditions bracingConditions)
        {
            var p_crl = section.GetCompressionLBResistance(material);
            return section.AsCompressionDto(material, bracingConditions, p_crl, (lst) => section.GetUnstiffenedCompressionDto(material, bracingConditions, lst, p_crl)) as DSUnStiffenedCompressionDto;
        }


        private static DSLippedCompressionDto GetLippedCompressionDto(this Section section, Material material, LengthBracingConditions bracingConditions, List<Tuple<double, FailureMode, double>> lst, LocalDSCompressionDto localDto)
        {
            var govTuple = lst.OrderBy(tuple => tuple.Item1).First();
            var governingCase = new NominalStrengthDto(govTuple.Item3, govTuple.Item2);
            var creTuple = lst.First(tup => tup.Item2 == FailureMode.GLOBALBUCKLING);
            var P_crd = section.GetCompressionDBResistance(material, bracingConditions);
            var Fy = material.Fy;
            var aPrime = section.Properties.APrime;
            var bPrime = section.Properties.BPrime;
            var cPrime = section.Properties.CPrime;
            var t = section.Dimensions.ThicknessT;

            var Ag = (aPrime + 2 * bPrime + 2 * cPrime) * t;
            var Py = Ag * Fy;
            var lambda_d = Math.Sqrt(Py / P_crd);
            var Pnd = lambda_d <= 0.561
                ? Py
                : (1 - 0.25 * (P_crd / Py).Power(0.6)) * ((P_crd / Py).Power(0.6)) * Py;
            if(P_crd>3.18 * Py)
            {
                P_crd = Py;
                Pnd = Py;
            }
            var distTuple = Tuple.Create(P_crd, FailureMode.DISTRORSIONALBUCKLING, Pnd);
            lst.Add(distTuple);
            return new DSLippedCompressionDto(
                lb: localDto,
                pcrd: P_crd,
                pcre: creTuple.Item1,
                pnl: lst.First(tup => tup.Item2 == FailureMode.LOCALBUCKLING).Item3,
                pnd: Pnd,
                pne: creTuple.Item3,
                ag: section.Properties.Zg,
                fy: material.Fy,
                governingCase
                );
        }

        private static DSUnStiffenedCompressionDto GetUnstiffenedCompressionDto(this Section section, Material material, LengthBracingConditions bracingConditions, List<Tuple<double, FailureMode, double>> lst, LocalDSCompressionDto localDto)
        {
            var govTuple = lst.OrderBy(tuple => tuple.Item1).First();
            var governingCase = new NominalStrengthDto(govTuple.Item3, govTuple.Item2);
            var creTuple = lst.First(tup => tup.Item2 == FailureMode.GLOBALBUCKLING);
            var Fy = material.Fy;
            var aPrime = section.Properties.APrime;
            var bPrime = section.Properties.BPrime;
            var cPrime = section.Properties.CPrime;
            var t = section.Dimensions.ThicknessT;

            var Ag = (aPrime + 2 * bPrime + 2 * cPrime) * t;
            var Py = Ag * Fy;

            return new DSUnStiffenedCompressionDto(
                lb: localDto,
                pcre: creTuple.Item1,
                pnl: lst.First(tup => tup.Item2 == FailureMode.LOCALBUCKLING).Item3,
                pne: creTuple.Item3,
                ag: Ag,
                fy: Fy,
                governingCase
                );
        }


        private static DSCompressionDto AsCompressionDto(this Section section, Material material, LengthBracingConditions bracingConditions, LocalDSCompressionDto localDto, Func<List<Tuple<double, FailureMode, double>>, DSCompressionDto> getDto)
        {
            var Fy = material.Fy;
            var aPrime = section.Properties.APrime;
            var bPrime = section.Properties.BPrime;
            var cPrime = section.Properties.CPrime;
            var t = section.Dimensions.ThicknessT;

            var Ag = (aPrime + 2 * bPrime + 2 * cPrime) * t;
            var Py = Ag * Fy;

            var p_cre = section.GetCompressionGBResistance(material, bracingConditions);

            //Nominal axial strength (Pne) for flexural , torsional or flexural torsional buckling
            var lambda_c = Math.Sqrt(Py / p_cre);
            var lambda_c_squared = lambda_c.Power(2);
            var Pne = lambda_c <= 1.5
                ? 0.658.Power(lambda_c_squared) * Py
                : (0.877 / lambda_c_squared) * Py;

            //Nominal axial strength (Pnl) for local buckling.
            var lambda_L = Math.Sqrt(Pne / localDto.Pcrl);
            var Pnl = lambda_L <= 0.776
                ? Pne
                : (1 - 0.15 * (localDto.Pcrl / Pne).Power(0.4)) * (localDto.Pcrl / Pne).Power(0.4) * Pne;

            var pcrl = localDto.Pcrl;
            if (pcrl > 1.66 * Py)
            {
                pcrl = Py;
                Pnl = Py;
            }
            var lst = new List<Tuple<double, FailureMode, double>>()
            {
                Tuple.Create(pcrl,FailureMode.LOCALBUCKLING,Pnl),
                Tuple.Create(p_cre,FailureMode.GLOBALBUCKLING,Pne),
            };

            return getDto(lst);
        }


        private static LocalDSCompressionDto GetCompressionLBResistance(this LippedSection section, Material material)
        {
            var Pcr = (section.Properties.CPrime / section.Properties.BPrime) < 0.6
                ? section.GetCompressionLBResistanceFromInteractionMethod(material)
                : section.GetCompressionLBResistanceFromElementMethod(material);
            return Pcr;
        }

        private static LocalDSCompressionDto GetCompressionLBResistanceFromInteractionMethod(this LippedSection section, Material material)
        {
            var cPrimeOverbPrime = section.Properties.CPrime / section.Properties.BPrime;
            var aPrimeOverbPrime = section.Properties.APrime / section.Properties.BPrime;
            var bPrimeOveraPrime = section.Properties.BPrime / section.Properties.APrime;

            var EOverVTerm = ((Math.PI.Power(2) * material.E) / (12 * (1 - material.V.Power(2))));
            //Flange - lip local buckling.
            var kFlange_Lip = -11.07 * cPrimeOverbPrime.Power(2) + 3.95 * cPrimeOverbPrime + 4;
            var Fcr_Flange_Lip = kFlange_Lip * EOverVTerm * (section.Dimensions.ThicknessT / section.Properties.BPrime).Power(2);

            //Flange - web local buckling
            var kFlange_Web = aPrimeOverbPrime >= 1
                ? (2 - (bPrimeOveraPrime).Power(0.4)) * 4 * bPrimeOveraPrime.Power(2)
                : (2 - aPrimeOverbPrime.Power(0.2)) * 4;
            var Fcr_Flange_Web = kFlange_Web * EOverVTerm * (section.Dimensions.ThicknessT / section.Properties.BPrime).Power(2);

            var Fcr = Math.Min(Fcr_Flange_Lip, Fcr_Flange_Web);
            var Ag = (section.Properties.APrime + 2 * section.Properties.BPrime + 2 * section.Properties.CPrime) * section.Dimensions.ThicknessT;
            var P_crl = Ag * Fcr;
            return new InteractionDSCompressionDto(kFlange_Lip, kFlange_Web, P_crl);
            //return P_crl;
        }

        private static LocalDSCompressionDto GetCompressionLBResistanceFromElementMethod(this LippedSection section, Material material)
        {
            var EOverVTerm = ((Math.PI.Power(2) * material.E) / (12 * (1 - material.V.Power(2))));
            //Flange Local buckling
            var kFlange = 4;
            var Fcr_Flange = kFlange * EOverVTerm * (section.Dimensions.ThicknessT / section.Properties.BPrime).Power(2);

            //Web local buckling
            var kWeb = 4;
            var Fcr_Web = kWeb * EOverVTerm * (section.Dimensions.ThicknessT / section.Properties.APrime).Power(2);

            //lip local buckling
            var kLip = 0.43;
            var Fcr_Lip = kLip * EOverVTerm * (section.Dimensions.ThicknessT / section.Properties.CPrime).Power(2);

            var Fcr = Math.Min(Math.Min(Fcr_Flange, Fcr_Web), Fcr_Lip);
            var Ag = (section.Properties.APrime + 2 * section.Properties.BPrime + 2 * section.Properties.CPrime) * section.Dimensions.ThicknessT;
            var P_crl = Ag * Fcr;
            return new ElementDSCompressionDto(kWeb, kFlange, kLip, P_crl);
        }

        private static LocalDSCompressionDto GetCompressionLBResistance(this UnStiffenedSection section, Material material)
        {
            var EOverVTerm = ((Math.PI.Power(2) * material.E) / (12 * (1 - material.V.Power(2))));

            //Flange Local Buckling.
            var kFlange = 0.43;
            var Fcr_flange = kFlange * EOverVTerm * (section.Dimensions.ThicknessT / section.Properties.BPrime).Power(2);

            //Web Local Buckling.
            var kWeb = 4;
            var Fcr_web = kWeb * EOverVTerm * (section.Dimensions.ThicknessT / section.Properties.APrime).Power(2);


            var Fcr = Math.Min(Fcr_flange, Fcr_web);
            var Ag = (section.Properties.APrime + 2 * section.Properties.BPrime) * section.Dimensions.ThicknessT;
            var P_crl = Ag * Fcr;
            return new ElementDSCompressionDto(kWeb, kFlange, 0, P_crl);
        }

        private static double GetCompressionDBResistance(this Section input, Material material, LengthBracingConditions bracingConditions)
        {
            var aPrime = input.Properties.APrime;
            var bPrime = input.Properties.BPrime;
            var cPrime = input.Properties.CPrime;
            var t = input.Dimensions.ThicknessT;
            var v = material.V;
            var E = material.E;
            var G = material.G;
            var kxLx = bracingConditions.Kx * bracingConditions.Lx;
            var kyLy = bracingConditions.Ky * bracingConditions.Ly;
            var minK_L_ = Math.Min(kxLx, kyLy);

            var Af = (bPrime + cPrime) * t;
            var Jf = (1.0 / 3.0) * bPrime * t.Power(3) + (1.0 / 3.0) * cPrime * t.Power(3);
            var Ixf_Numen = t * (t.Power(2) * bPrime.Power(2) + 4 * bPrime * cPrime.Power(3) + t.Power(2) * bPrime * cPrime + cPrime.Power(4));
            var Ixf_Dnumen = 12 * (bPrime + cPrime);
            var Ixf = Ixf_Numen / Ixf_Dnumen;

            var Iyf_Numen = t * (bPrime.Power(4) + 4 * cPrime * bPrime.Power(3));
            var Iyf_Dnumen = 12 * (bPrime + cPrime);
            var Iyf = Iyf_Numen / Iyf_Dnumen;

            var Ixyf_Numen = t * bPrime.Power(2) * cPrime.Power(2);
            var Ixyf_Dnumen = 4 * (bPrime + cPrime);
            var Ixyf = Ixyf_Numen / Ixyf_Dnumen;

            var Iof = ((t * bPrime.Power(3)) / (3)) + (bPrime * t.Power(3) / 12) + (t * cPrime.Power(3) / 3);

            var Xof = (bPrime.Power(2)) / (2 * (bPrime + cPrime));

            var Yof = (-cPrime.Power(2)) / (2 * (bPrime + cPrime));
            var hxf = -(bPrime.Power(2) + 2 * bPrime * cPrime) / (2 * (bPrime + cPrime));
            var hyf = -(cPrime.Power(2)) / (2 * (bPrime + cPrime));
            var Cwf = 0.0;

            var Lcr_VTerm = (6 * Math.PI.Power(4) * aPrime * (1 - v.Power(2))) / (t.Power(3));
            var Lcr_ITerm = (Ixf * (Xof - hxf).Power(2) + Cwf - (Ixyf.Power(2) / Iyf) * (Xof - hxf).Power(2));
            var Lcr = (Lcr_VTerm * Lcr_ITerm).Power(0.25);
            Lcr = Math.Min(Lcr, minK_L_);

            //Elastic & geometric rotational spring stiffness of the flange.
            var piOverLCr = Math.PI / Lcr;
            var K_phi_fe = (piOverLCr).Power(4) * (E * Ixf * (Xof - hxf).Power(2) + E * Cwf - E * (Ixyf.Power(2) / Iyf) * (Xof - hxf).Power(2)) + piOverLCr.Power(2) * G * Jf;

            var K_phi_fg = piOverLCr.Power(2) * (Af * ((Xof - hxf).Power(2) * (Ixyf / Iyf).Power(2) - 2 * Yof * (Xof - hxf) * (Ixyf / Iyf) + hxf.Power(2) + Yof.Power(2)) + Ixf + Iyf);

            var K_phi_we = (t.Power(3) * E) / (6 * aPrime * (1 - v.Power(2)));
            var K_phi_wg = piOverLCr.Power(2) * ((t * aPrime.Power(3)) / (60.0));

            var Fcr = (K_phi_fe + K_phi_we) / (K_phi_fg + K_phi_wg);
            var Ag = (aPrime + 2 * bPrime + 2 * cPrime) * t;
            var P_crd = Fcr * Ag;

            return P_crd;
        }

        private static double GetCompressionGBResistance(this Section input, Material material, LengthBracingConditions bracingConditions)
        {
            var E = material.E;
            var G = material.G;
            var Kx = bracingConditions.Kx;
            var Ky = bracingConditions.Ky;
            var Kz = bracingConditions.Kz;
            var Lx = bracingConditions.Lx;
            var Ly = bracingConditions.Ly;
            var Lz = bracingConditions.Lz;
            var rx = input.Properties.Rx;
            var ry = input.Properties.Ry;
            var Xo = input.Properties.Xo;
            var Cw = input.Properties.Cw;
            var J = input.Properties.J;
            var A = input.Properties.A;
            var aPrime = input.Properties.APrime;
            var bPrime = input.Properties.BPrime;
            var cPrime = input.Properties.CPrime;
            var t = input.Dimensions.ThicknessT;
            //Individual Buckling modes.
            var sigma_ex = (Math.PI.Power(2) * E) / ((Kx * Lx) / (rx)).Power(2);

            var sigma_ey = (Math.PI.Power(2) * E) / ((Ky * Ly) / (ry)).Power(2);

            var F_e1 = Math.Min(sigma_ex, sigma_ey);
            var ro = Math.Sqrt((rx.Power(2) + ry.Power(2) + Xo.Power(2)));

            var sigma_t = ((1.0) / (A * ro.Power(2))) * (G * J + ((Math.PI.Power(2) * E * Cw) / (Kz * Lz).Power(2)));

            //Torsional flexural buckling.
            var beta = 1 - (Xo / ro).Power(2);
            var F_e2 = ((1) / (2 * beta)) * ((sigma_ex + sigma_t) - Math.Sqrt((sigma_ex + sigma_t).Power(2) - 4 * beta * sigma_ex * sigma_t));
            var F_cr = Math.Min(F_e1, F_e2);
            var Ag = (aPrime + 2 * bPrime + 2 * cPrime) * t;
            var P_cre = F_cr * Ag;
            return P_cre;
        }

        private static Validation<bool> IsValidForCompression(this UnStiffenedSection section)
        {
            var a_over_t = Tuple.Create(section.Properties.ASmall / section.Dimensions.ThicknessT, 500.0);
            var b_over_t = Tuple.Create(section.Properties.BSmall / section.Dimensions.ThicknessT, 60.0);

            var C_over_b = Tuple.Create(section.Dimensions.TotalFoldWidthC / section.Properties.BSmall, 0.8);
            var c_over_t = Tuple.Create(section.Properties.CSmall / section.Dimensions.ThicknessT, 60.0);


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

        private static Validation<bool> IsValidForCompression(this LippedSection section)
        {
            var a_over_t = Tuple.Create(section.Properties.ASmall / section.Dimensions.ThicknessT, 500.0);
            var b_over_t = Tuple.Create(section.Properties.BSmall / section.Dimensions.ThicknessT, 160.0);

            var C_over_b = Tuple.Create(section.Dimensions.TotalFoldWidthC / section.Properties.BSmall, 0.8);
            var c_over_t = Tuple.Create(section.Properties.CSmall / section.Dimensions.ThicknessT, 60.0);


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


        #endregion



        #region Moment


        #region Z Sections

        public static Validation<MomentResistanceOutput> AsDSMomentResistance(this UnStiffenedZSection section, Material material, LengthBracingConditions bracingConditions)
        {
            var result = from valid in section.IsValidForMoment()
                         select section.AsMomentDto(material, bracingConditions).AsMomentOutput(section);
            return result;
        }

        public static Validation<MomentResistanceOutput> AsDSMomentResistance(this LippedZSection section, Material material, LengthBracingConditions bracingConditions)
        {
            var result = from valid in section.IsValidForMoment()
                         select section.AsMomentDto(material, bracingConditions).AsMomentOutput(section);
            return result;
        }

        private static MomentResistanceOutput AsMomentOutput(this DSLippedMomentDto dto, LippedZSection section)
        {
            var report = dto.AsReport(section);
            return new MomentResistanceOutput(dto.GoverningCase.NominalStrength, PHI_B_DS, PHI_B_NAME_DS, MOM_DESIGN_RESIST_DS, dto.GoverningCase.FailureMode, "Kip.in", report);
        }

        private static MomentResistanceOutput AsMomentOutput(this DSUnStiffenedMomentDto dto, UnStiffenedZSection section)
        {
            var report = dto.AsReport(section);
            return new MomentResistanceOutput(dto.GoverningCase.NominalStrength, PHI_B_DS, PHI_B_NAME_DS, MOM_DESIGN_RESIST_DS, dto.GoverningCase.FailureMode, "Kip.in", report);
        }

        private static DSLippedMomentDto GetLippedMomentDTo(this Section section, Material material, LengthBracingConditions bracingConditions, List<Tuple<double, FailureMode, double>> lst, LocalDSMomentDto lbDto)
        {
            var govTuple = lst.OrderBy(tuple => tuple.Item1).First();
            var governingCase = new NominalStrengthDto(govTuple.Item3, govTuple.Item2);
            var creTuple = lst.First(tup => tup.Item2 == FailureMode.GLOBALBUCKLING);
            var M_crd = section.GetMomentDBResistance(material, bracingConditions);
            var Zg = section.Properties.Zg;
            var Fy = material.Fy;
            var My = Zg * Fy;
            var lambda_d = Math.Sqrt(My / M_crd);
            var Mnd = lambda_d <= 0.673
                ? My
                : (1 - 0.22 * (M_crd / My).Power(0.5)) * (M_crd / My).Power(0.5) * My;
            if(M_crd > 2.21 * My)
            {
                M_crd = My;
                Mnd = My;
            }

            var distTuple = Tuple.Create(M_crd, FailureMode.DISTRORSIONALBUCKLING, Mnd);
            lst.Add(distTuple);
            return new DSLippedMomentDto(
                lb: lbDto,
                mcre: creTuple.Item1,
                mcrd: M_crd,
                mnl: lst.First(tup => tup.Item2 == FailureMode.LOCALBUCKLING).Item3,
                mnd: Mnd,
                mne: creTuple.Item3,
                fy: material.Fy,
                zg: section.Properties.Zg,
                governingCase
                );
        }

        private static DSUnStiffenedMomentDto GetUnStiffenedMomentDTo(this Section section, Material material, LengthBracingConditions bracingConditions, List<Tuple<double, FailureMode, double>> lst, LocalDSMomentDto lbDto)
        {
            var govTuple = lst.OrderBy(tuple => tuple.Item1).First();
            var governingCase = new NominalStrengthDto(govTuple.Item3, govTuple.Item2);
            var creTuple = lst.First(tup => tup.Item2 == FailureMode.GLOBALBUCKLING);
            var Zg = section.Properties.Zg;
            var Fy = material.Fy;

            return new DSUnStiffenedMomentDto(
                lb: lbDto,
                mcre: creTuple.Item1,
                mnl: lst.First(tup => tup.Item2 == FailureMode.LOCALBUCKLING).Item3,
                mne: creTuple.Item3,
                fy: Fy,
                zg: Zg,
                governingCase
                );
        }

        private static DSLippedMomentDto AsMomentDto(this LippedZSection section, Material material, LengthBracingConditions bracingConditions)
        {
            var lbDto = section.GetMomentLBResistance(material);
            return section.AsMomentDto(material, bracingConditions, lbDto, section.CalcFeForZ(material, bracingConditions), (lst) => section.GetLippedMomentDTo(material, bracingConditions, lst, lbDto)) as DSLippedMomentDto;
        }

        private static DSUnStiffenedMomentDto AsMomentDto(this UnStiffenedZSection section, Material material, LengthBracingConditions bracingConditions)
        {
            var lbDto = section.GetMomentLBResistance(material);

            return section.AsMomentDto(material, bracingConditions, lbDto, section.CalcFeForZ(material, bracingConditions), (lst) => section.GetUnStiffenedMomentDTo(material, bracingConditions, lst, lbDto)) as DSUnStiffenedMomentDto;
        }

        private static double CalcFeForZ(this Section section, Material material, LengthBracingConditions bracingConditions)
        {
            var H = section.Dimensions.TotalHeightH;
            var E = material.E;
            var Ky = bracingConditions.Ky;
            var Ly = bracingConditions.Ly;
            var Cb = bracingConditions.Cb;
            var Iy = section.Properties.Iy;
            var Zg = section.Properties.Zg;
            var num = Cb * Math.PI.Power(2) * E * H * Iy;
            var dnum = 4 * Zg * (Ky * Ly).Power(2);
            var Fe = num / dnum;
            return Fe;
        }


        #endregion

        private static Validation<bool> IsValidForMoment(this UnStiffenedSection section)
        {
            var a_over_t = Tuple.Create(section.Properties.ASmall / section.Dimensions.ThicknessT, 300.0);
            var b_over_t = Tuple.Create(section.Properties.BSmall / section.Dimensions.ThicknessT, 60.0);

            var C_over_b = Tuple.Create(section.Dimensions.TotalFoldWidthC / section.Properties.BSmall, 0.8);
            var c_over_t = Tuple.Create(section.Properties.CSmall / section.Dimensions.ThicknessT, 60.0);
            var R_over_t = Tuple.Create(section.Dimensions.InternalRadiusR / section.Dimensions.ThicknessT, 20.0);


            var allows = new List<Tuple<double, double>>()
            {
                b_over_t,c_over_t,a_over_t,C_over_b,R_over_t
            };
            var result = !allows.Any(tuple => tuple.Item1 > tuple.Item2);
            if (result)
                return true;
            else
                return CantCalculateNominalStrength;
        }

        private static Validation<bool> IsValidForMoment(this LippedSection section)
        {
            var a_over_t = Tuple.Create(section.Properties.ASmall / section.Dimensions.ThicknessT, 300.0);
            var b_over_t = Tuple.Create(section.Properties.BSmall / section.Dimensions.ThicknessT, 160.0);

            var C_over_b = Tuple.Create(section.Dimensions.TotalFoldWidthC / section.Properties.BSmall, 0.8);
            var c_over_t = Tuple.Create(section.Properties.CSmall / section.Dimensions.ThicknessT, 60.0);
            var R_over_t = Tuple.Create(section.Dimensions.InternalRadiusR / section.Dimensions.ThicknessT, 20.0);


            var allows = new List<Tuple<double, double>>()
            {
                b_over_t,c_over_t,a_over_t,C_over_b,R_over_t
            };
            var result = !allows.Any(tuple => tuple.Item1 > tuple.Item2);
            if (result)
                return true;
            else
                return CantCalculateNominalStrength;
        }


        public static Validation<MomentResistanceOutput> AsDSMomentResistance(this UnStiffenedCSection unstiffenedSection, Material material, LengthBracingConditions bracingConditions)
        {
            var result = from valid in unstiffenedSection.IsValidForMoment()
                         select unstiffenedSection.AsMomentDto(material, bracingConditions).AsMomentOutput(unstiffenedSection);
            return result;
        }

        public static Validation<MomentResistanceOutput> AsDSMomentResistance(this LippedCSection lippedSection, Material material, LengthBracingConditions bracingConditions)
        {
            var result = from valid in lippedSection.IsValidForMoment()
                         select lippedSection.AsMomentDto(material, bracingConditions).AsMomentOutput(lippedSection);
            return result;
        }

        private static MomentResistanceOutput AsMomentOutput(this DSLippedMomentDto dto, LippedCSection section)
        {
            var report = dto.AsReport(section);
            return new MomentResistanceOutput(dto.GoverningCase.NominalStrength, PHI_B_DS, PHI_B_NAME_DS, MOM_DESIGN_RESIST_DS, dto.GoverningCase.FailureMode, "Kip.in", report);
        }

        private static MomentResistanceOutput AsMomentOutput(this DSUnStiffenedMomentDto dto, UnStiffenedCSection section)
        {
            var report = dto.AsReport(section);
            return new MomentResistanceOutput(dto.GoverningCase.NominalStrength, PHI_B_DS, PHI_B_NAME_DS, MOM_DESIGN_RESIST_DS, dto.GoverningCase.FailureMode, "Kip.in", report);
        }

        private static DSLippedMomentDto AsMomentDto(this LippedCSection section, Material material, LengthBracingConditions bracingConditions)
        {
            var lbDto = section.GetMomentLBResistance(material);
            return section.AsMomentDto(material, bracingConditions, lbDto, section.CalcFeForC(material, bracingConditions), (lst) => section.GetLippedMomentDTo(material, bracingConditions, lst, lbDto)) as DSLippedMomentDto;
        }

        private static DSUnStiffenedMomentDto AsMomentDto(this UnStiffenedCSection section, Material material, LengthBracingConditions bracingConditions)
        {
            var lbDto = section.GetMomentLBResistance(material);
            return section.AsMomentDto(material, bracingConditions, lbDto, section.CalcFeForC(material, bracingConditions), (lst) => section.GetUnStiffenedMomentDTo(material, bracingConditions, lst, lbDto)) as DSUnStiffenedMomentDto;
        }

        private static DSMomentDto AsMomentDto(this Section section, Material material, LengthBracingConditions bracingConditions, LocalDSMomentDto lbDto, double Fe, Func<List<Tuple<double, FailureMode, double>>, DSMomentDto> getDto)
        {
            var Zg = section.Properties.Zg;
            var Fy = material.Fy;

            var My = Zg * Fy;

            var M_cre = section.GetMomentGBResistance(Fe);

            var Mne = 0.0;
            if (M_cre < 0.56 * My)
            {
                Mne = M_cre;
            }
            else if (M_cre > 2.78 * My)
            {
                Mne = My;
            }
            else
            {
                Mne = (10 / 9.0) * My * (1 - ((10 * My) / (36 * M_cre)));
            }

            //Nominal Flexural strength (Mnl) for local buckling.
            var lambda_L = Math.Sqrt(Mne / lbDto.Mcrl);
            var Mnl = lambda_L <= 0.776
                ? Mne
                : (1 - 0.15 * (lbDto.Mcrl / Mne).Power(0.4)) * (lbDto.Mcrl / Mne).Power(0.4) * Mne;

            var mcrl = lbDto.Mcrl;
            if(mcrl > 1.66 * My)
            {
                mcrl = My;
                Mnl = My;
            }
            if(M_cre > 2.78 * My)
            {
                M_cre = My;
                Mne = My;
            }
            var lst = new List<Tuple<double, FailureMode, double>>()
            {
                Tuple.Create(mcrl,FailureMode.LOCALBUCKLING,Mnl),
                Tuple.Create(M_cre,FailureMode.GLOBALBUCKLING,Mne),
            };
            return getDto(lst);

        }


        public static LocalDSMomentDto GetMomentLBResistance(this LippedSection lippedSection, Material material)
        {
            var H = lippedSection.Dimensions.TotalHeightH;
            var cPrime = lippedSection.Properties.CPrime;
            var bPrime = lippedSection.Properties.BPrime;
            var cPrimeOverbPrime = cPrime / bPrime;
            var f1 = 1.0;
            var f2 = (0.5 * H - cPrime) / (0.5 * H);
            var epslon = (f1 - f2) / (f1);
            if (cPrimeOverbPrime < 0.6 && epslon < 1)
            {
                //Interaction Method.
                return lippedSection.GetMomentLBResistanceFromInteractionMethod(material);
            }
            else if (cPrimeOverbPrime >= 0.6 && epslon > 1)
            {
                //Element Method.
                return lippedSection.GetMomentLBResistanceFromElementMethod(material);
            }
            else
            {
                throw new ArgumentException($"c/b = {cPrimeOverbPrime} & epslon = {epslon}");
            }
            throw new NotImplementedException();
        }

        private static LocalDSMomentDto GetMomentLBResistance(this UnStiffenedSection unstiffenedSection, Material material)
        {

            var EOverVTerm = ((Math.PI.Power(2) * material.E) / (12 * (1 - material.V.Power(2))));

            //Flange Local Buckling.
            var kFlange = 0.43;
            var Fcr_flange = kFlange * EOverVTerm * (unstiffenedSection.Dimensions.ThicknessT / unstiffenedSection.Properties.BPrime).Power(2);

            //Web Local Buckling.
            var kWeb = 24;
            var Fcr_web = kWeb * EOverVTerm * (unstiffenedSection.Dimensions.ThicknessT / unstiffenedSection.Properties.APrime).Power(2);


            var Fcr = Math.Min(Fcr_flange, Fcr_web);
            var Zg = unstiffenedSection.Properties.Zg;
            var m_crl = Zg * Fcr;

            return new ElementDSMomentDto(
                kw: kWeb,
                kf: kFlange,
                kc: 0,
                mcrl: m_crl
                );
        }

        private static LocalDSMomentDto GetMomentLBResistanceFromInteractionMethod(this LippedSection lippedSection, Material material)
        {
            var H = lippedSection.Dimensions.TotalHeightH;
            var t = lippedSection.Dimensions.ThicknessT;
            var cPrime = lippedSection.Properties.CPrime;
            var bPrime = lippedSection.Properties.BPrime;
            var aPrime = lippedSection.Properties.APrime;
            var Zg = lippedSection.Properties.Zg;
            var cPrimeOverbPrime = cPrime / bPrime;
            var tOverbPrime = t / bPrime;
            var bPrimeOveraPrime = bPrime / aPrime;
            var EOverVTerm = ((Math.PI.Power(2) * material.E) / (12 * (1 - material.V.Power(2))));

            //Flange - lip local buckling.
            var f1 = 1.0;
            var f2 = (0.5 * H - cPrime) / (0.5 * H);
            var epslon = (f1 - f2) / (f1);
            var k_flange_lip = (8.55 * epslon - 11.07) * (cPrimeOverbPrime.Power(2)) + ((3.95 - 1.59 * epslon) * (cPrimeOverbPrime)) + 4;
            var Fcr_flange_lip = k_flange_lip * EOverVTerm * tOverbPrime.Power(2);

            //Flange - web local buckling
            f1 = 1;
            f2 = -1;
            epslon = (f1 - f2) / (f1);

            var k_flange_web = 1.125 * Math.Min(4, (0.5 * epslon.Power(3) + 4 * epslon.Power(2) + 4) * (bPrimeOveraPrime.Power(2)));
            var Fcr_flange_web = k_flange_web * EOverVTerm * tOverbPrime.Power(2);

            var F_crl = Math.Min(Fcr_flange_lip, Fcr_flange_web);
            var M_crl = Zg * F_crl;

            return new InteractionDSMomentDto(k_flange_web, k_flange_lip, M_crl);
        }

        private static LocalDSMomentDto GetMomentLBResistanceFromElementMethod(this LippedSection lippedSection, Material material)
        {
            var EOverVTerm = ((Math.PI.Power(2) * material.E) / (12 * (1 - material.V.Power(2))));
            var H = lippedSection.Dimensions.TotalHeightH;
            var t = lippedSection.Dimensions.ThicknessT;
            var cPrime = lippedSection.Properties.CPrime;
            var Zg = lippedSection.Properties.Zg;


            //Flange Local buckling.
            var kFlange = 4;
            var Fcr_flange = kFlange * EOverVTerm * (lippedSection.Dimensions.ThicknessT / lippedSection.Properties.BPrime).Power(2);

            //Web Local Buckling.
            var kWeb = 24;
            var Fcr_web = kWeb * EOverVTerm * (lippedSection.Dimensions.ThicknessT / lippedSection.Properties.APrime).Power(2);

            //Lip Local buckling.
            var f1 = 1;
            var f2 = (0.5 * H - cPrime) / (0.5 * H);
            var epslon = (f1 - f2) / f1;
            var k_lip = 1.4 * epslon.Power(2) - 0.25 * epslon + 0.425;
            var Fcr_lip = k_lip * EOverVTerm * (t / cPrime).Power(2);

            var F_crl = Math.Min(Math.Min(Fcr_flange, Fcr_web), Fcr_lip);
            var M_crl = Zg * F_crl;


            return new ElementDSMomentDto(kWeb, kFlange, k_lip, M_crl);
        }

        private static double GetMomentDBResistance(this Section input, Material material, LengthBracingConditions bracingConditions)
        {
            var aPrime = input.Properties.APrime;
            var bPrime = input.Properties.BPrime;
            var cPrime = input.Properties.CPrime;
            var Zg = input.Properties.Zg;
            var t = input.Dimensions.ThicknessT;
            var v = material.V;
            var E = material.E;
            var G = material.G;
            var Lu = bracingConditions.Lu;

            var Af = (bPrime + cPrime) * t;
            var Jf = (1.0 / 3.0) * bPrime * t.Power(3) + (1.0 / 3.0) * cPrime * t.Power(3);
            var Ixf_Numen = t * (t.Power(2) * bPrime.Power(2) + 4 * bPrime * cPrime.Power(3) + t.Power(2) * bPrime * cPrime + cPrime.Power(4));
            var Ixf_Dnumen = 12 * (bPrime + cPrime);
            var Ixf = Ixf_Numen / Ixf_Dnumen;

            var Iyf_Numen = t * (bPrime.Power(4) + 4 * cPrime * bPrime.Power(3));
            var Iyf_Dnumen = 12 * (bPrime + cPrime);
            var Iyf = Iyf_Numen / Iyf_Dnumen;

            var Ixyf_Numen = t * bPrime.Power(2) * cPrime.Power(2);
            var Ixyf_Dnumen = 4 * (bPrime + cPrime);
            var Ixyf = Ixyf_Numen / Ixyf_Dnumen;

            var Iof = ((t * bPrime.Power(3)) / (3)) + (bPrime * t.Power(3) / 12) + (t * cPrime.Power(3) / 3);

            var Xof = (bPrime.Power(2)) / (2 * (bPrime + cPrime));

            var Yof = (-cPrime.Power(2)) / (2 * (bPrime + cPrime));
            var hxf = -(bPrime.Power(2) + 2 * bPrime * cPrime) / (2 * (bPrime + cPrime));
            var hyf = -(cPrime.Power(2)) / (2 * (bPrime + cPrime));
            var Cwf = 0.0;

            var Lcr_vTerm = (4 * Math.PI.Power(4) * aPrime * (1 - v.Power(2))) / (t.Power(3));
            var Lcr_ITerm = Ixf * (Xof - hxf).Power(2) + Cwf - (Ixyf.Power(2) / Iyf) * (Xof - hxf).Power(2);
            var Lcr = (Lcr_vTerm * Lcr_ITerm + ((Math.PI.Power(4) * aPrime.Power(4)) / (720.0))).Power(0.25);
            if(Lu != 0)
                 Lcr = Math.Min(Lcr, Lu);

            var piOverLCr = Math.PI / Lcr;
            var K_phi_fe = (piOverLCr).Power(4) * (E * Ixf * (Xof - hxf).Power(2) + E * Cwf - E * (Ixyf.Power(2) / Iyf) * (Xof - hxf).Power(2)) + piOverLCr.Power(2) * G * Jf;

            var K_phi_fg = piOverLCr.Power(2) * (Af * ((Xof - hxf).Power(2) * (Ixyf / Iyf).Power(2) - 2 * Yof * (Xof - hxf) * (Ixyf / Iyf) + hxf.Power(2) + Yof.Power(2)) + Ixf + Iyf);

            var k_phi_we = ((E * t.Power(3)) / (12 * (1 - v.Power(2)))) * ((3 / aPrime) + piOverLCr.Power(2) * ((19 * aPrime) / (60.0)) + piOverLCr.Power(4) * (aPrime.Power(3) / 240));

            var sai = -1;
            var k_phi_wg_term1 = ((aPrime * t * Math.PI.Power(2)) / (13440));
            var LcrOveraPrime = Lcr / aPrime;
            var aPrimeOverLcr = aPrime / Lcr;
            var k_phi_wg_term2_Numen = (45360 * sai + 62160) * LcrOveraPrime.Power(2) + 448 * Math.PI.Power(2) + aPrimeOverLcr.Power(2) * (53 + 3 * sai) * Math.PI.Power(4);
            var k_phi_wg_term2_Dunem = Math.PI.Power(4) + 28 * Math.PI.Power(2) * LcrOveraPrime.Power(2) + 420 * LcrOveraPrime.Power(4);
            var k_phi_wg = k_phi_wg_term1 * (k_phi_wg_term2_Numen / k_phi_wg_term2_Dunem);
            var F_crd = (K_phi_fe + k_phi_we) / (K_phi_fg + k_phi_wg);
            var M_crd = F_crd * Zg;
            return M_crd;
        }


        private static double CalcFeForC(this Section section, Material material, LengthBracingConditions bracingConditions)
        {
            var E = material.E;
            var G = material.G;
            var Ky = bracingConditions.Ky;
            var Kz = bracingConditions.Kz;
            var Ly = bracingConditions.Ly;
            var Lz = bracingConditions.Lz;
            var Cb = bracingConditions.Cb;
            var rx = section.Properties.Rx;
            var ry = section.Properties.Ry;
            var Xo = section.Properties.Xo;
            var Cw = section.Properties.Cw;
            var J = section.Properties.J;
            var A = section.Properties.A;
            var Zg = section.Properties.Zg;
            var sigma_ey = (Math.PI.Power(2) * E) / ((Ky * Ly) / (ry)).Power(2);

            var ro = Math.Sqrt((rx.Power(2) + ry.Power(2) + Xo.Power(2)));

            var sigma_t = ((1.0) / (A * ro.Power(2))) * (G * J + ((Math.PI.Power(2) * E * Cw) / (Kz * Lz).Power(2)));
            var Fe = ((Cb * ro * A) / (Zg)) * (Math.Sqrt(sigma_ey * sigma_t));
            return Fe;
        }

        private static double GetMomentGBResistance(this Section section, double Fe)
        {
            var Zg = section.Properties.Zg;
            var F_cre = Fe;
            var M_cre = Zg * F_cre;
            return M_cre;
        }

        #endregion
    }
}
