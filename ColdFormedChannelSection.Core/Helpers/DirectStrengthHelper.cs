using ColdFormedChannelSection.Core.Entities;
using ColdFormedChannelSection.Core.Extensions;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

namespace ColdFormedChannelSection.Core.Helpers
{
    public static class DirectStrengthHelper
    {

        private enum FailureMode
        {
            [Description("Local Buckling")]
            LOCALBUCKLING,
            [Description("Distortional Buckling")]
            DISTRORTIONALBUCKLING,
            [Description("Global Buckling")]
            GLOBALBUCKLING
        }

        #region Compression


        public static CompressionDSResistanceOutput AsCompressionResistance(this LippedSection lippedSection, Material material, LengthBracingConditions bracingConditions)
        {
            var p_crl = lippedSection.GetCompressionLBResistance(material);
            return lippedSection.AsCompressionResistance(material, bracingConditions, p_crl);
        }

        public static CompressionDSResistanceOutput AsCompressionResistance(this UnStiffenedSection unstiffenedSection, Material material, LengthBracingConditions bracingConditions)
        {
            var p_crl = unstiffenedSection.GetCompressionLBResistance(material);
            return unstiffenedSection.AsCompressionResistance(material, bracingConditions, p_crl);
        }


        private static CompressionDSResistanceOutput AsCompressionResistance(this Section section, Material material, LengthBracingConditions bracingConditions, double p_crl)
        {
            var Fy = material.Fy;
            var aPrime = section.Properties.APrime;
            var bPrime = section.Properties.BPrime;
            var cPrime = section.Properties.CPrime;
            var t = section.Dimensions.ThicknessT;

            var Ag = (aPrime + 2 * bPrime + 2 * cPrime) * t;
            var Py = Ag * Fy;

            var p_crd = section.GetCompressionDBResistance(material, bracingConditions);
            var p_cre = section.GetCompressionGBResistance(material, bracingConditions);

            //Nominal axial strength (Pne) for flexural , torsional or flexural torsional buckling
            var lambda_c = Math.Sqrt(Py / p_cre);
            var lambda_c_squared = lambda_c.Power(2);
            var Pne = lambda_c <= 1.5
                ? 0.658.Power(lambda_c_squared) * Py
                : (0.877 / lambda_c_squared) * Py;

            //Nominal axial strength (Pnl) for local buckling.
            var lambda_L = Math.Sqrt(Pne / p_crl);
            var Pnl = lambda_L <= 0.776
                ? Pne
                : (1 - 0.15 * (p_crl / Pne).Power(0.4)) * (p_crl / Pne).Power(0.4) * Pne;

            //Nominal axial strength for distortional buckling.
            var lambda_d = Math.Sqrt(Py / p_crd);
            var Pnd = lambda_d <= 0.561
                ? Py
                : (1 - 0.25 * (p_crd / Py).Power(0.6)) * ((p_crd / Py).Power(0.6)) * Py;
            var nominalLoads = new List<Tuple<double, FailureMode>>()
            {
                Tuple.Create(Pne,FailureMode.GLOBALBUCKLING),
                Tuple.Create(Pnl,FailureMode.LOCALBUCKLING),
                Tuple.Create(Pnd,FailureMode.DISTRORTIONALBUCKLING)
            };
            var nominalLoad = nominalLoads.OrderBy(tup => tup.Item1).First();
            var result = new CompressionDSResistanceOutput(nominalLoad.Item1, 0.85, $"{nominalLoad.Item2.GetDescription()} governs");
            return result;
        }

        private static double GetCompressionLBResistance(this LippedSection lippedSection, Material material)
        {
            var Pcr = (lippedSection.Properties.CPrime / lippedSection.Properties.BPrime) < 0.6
                ? lippedSection.GetCompressionLBResistanceFromInteractionMethod(material)
                : lippedSection.GetCompressionLBResistanceFromElementMethod(material);
            return Pcr;
        }

        private static double GetCompressionLBResistanceFromInteractionMethod(this LippedSection lippedSection, Material material)
        {
            var cPrimeOverbPrime = lippedSection.Properties.CPrime / lippedSection.Properties.BPrime;
            var aPrimeOverbPrime = lippedSection.Properties.APrime / lippedSection.Properties.BPrime;
            var bPrimeOveraPrime = lippedSection.Properties.APrime / lippedSection.Properties.BPrime;

            var EOverVTerm = ((Math.PI.Power(2) * material.E) / (12 * (1 - material.V.Power(2))));
            //Flange - lip local buckling.
            var kFlange_Lip = -11.07 * cPrimeOverbPrime.Power(2) + 3.95 * cPrimeOverbPrime + 4;
            var Fcr_Flange_Lip = kFlange_Lip * EOverVTerm * (lippedSection.Dimensions.ThicknessT / lippedSection.Properties.BPrime).Power(2);

            //Flange - web local buckling
            var kFlange_Web = aPrimeOverbPrime >= 1
                ? (2 - (bPrimeOveraPrime).Power(0.4)) * 4 * bPrimeOveraPrime.Power(2)
                : (2 - aPrimeOverbPrime.Power(0.2)) * 4;
            var Fcr_Flange_Web = kFlange_Web * EOverVTerm * (lippedSection.Dimensions.ThicknessT / lippedSection.Properties.BPrime).Power(2);

            var Fcr = Math.Min(Fcr_Flange_Lip, Fcr_Flange_Web);
            var Ag = (lippedSection.Properties.APrime + 2 * lippedSection.Properties.BPrime + 2 * lippedSection.Properties.CPrime) * lippedSection.Dimensions.ThicknessT;
            var P_crl = Ag * Fcr;

            return P_crl;
        }

        private static double GetCompressionLBResistanceFromElementMethod(this LippedSection lippedSection, Material material)
        {
            var EOverVTerm = ((Math.PI.Power(2) * material.E) / (12 * (1 - material.V.Power(2))));
            //Flange Local buckling
            var kFlange = 4;
            var Fcr_Flange = kFlange * EOverVTerm * (lippedSection.Dimensions.ThicknessT / lippedSection.Properties.BPrime).Power(2);

            //Web local buckling
            var kWeb = 4;
            var Fcr_Web = kWeb * EOverVTerm * (lippedSection.Dimensions.ThicknessT / lippedSection.Properties.APrime).Power(2);

            //lip local buckling
            var kLip = 0.43;
            var Fcr_Lip = kLip * EOverVTerm * (lippedSection.Dimensions.ThicknessT / lippedSection.Properties.CPrime).Power(2);

            var Fcr = Math.Min(Math.Min(Fcr_Flange, Fcr_Web), Fcr_Lip);
            var Ag = (lippedSection.Properties.APrime + 2 * lippedSection.Properties.BPrime + 2 * lippedSection.Properties.CPrime) * lippedSection.Dimensions.ThicknessT;
            var P_crl = Ag * Fcr;
            return P_crl;
        }

        private static double GetCompressionLBResistance(this UnStiffenedSection unstiffenedSection, Material material)
        {
            var EOverVTerm = ((Math.PI.Power(2) * material.E) / (12 * (1 - material.V.Power(2))));

            //Flange Local Buckling.
            var kFlange = 0.43;
            var Fcr_flange = kFlange * EOverVTerm * (unstiffenedSection.Dimensions.ThicknessT / unstiffenedSection.Properties.BPrime).Power(2);

            //Web Local Buckling.
            var kWeb = 4;
            var Fcr_web = kWeb * EOverVTerm * (unstiffenedSection.Dimensions.ThicknessT / unstiffenedSection.Properties.APrime).Power(2);


            var Fcr = Math.Min(Fcr_flange, Fcr_web);
            var Ag = (unstiffenedSection.Properties.APrime + 2 * unstiffenedSection.Properties.BPrime) * unstiffenedSection.Dimensions.ThicknessT;
            var P_crl = Ag * Fcr;
            return P_crl;
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

            var Lcr_VTerm = (6 * Math.PI.Power(4) * aPrime * (1 - v.Power(2))) / (t.Power(3));
            var Lcr_ITerm = (Ixf * (Xof - hxf).Power(2) + Cwf - (Ixyf.Power(2) / Iyf) * (Xof - hxf).Power(2));
            var Lcr = (Lcr_VTerm * Lcr_ITerm).Power(0.25);
            Lcr = Math.Min(Lcr, Lu);

            //Elastic & geometric rotational spring stiffness of the flange.
            var piOverLCr = Math.PI / Lcr;
            var K_phi_fe = (piOverLCr).Power(4) * (E * Ixf * (Xof - hxf).Power(2) + E * Cwf - E * (Ixyf.Power(2) / Iyf) * (Xof - hxf).Power(2)) * piOverLCr.Power(2) * G * Jf;

            var K_phi_fg = piOverLCr.Power(2) * (Af * ((Xof - hxf).Power(2) * (Ixyf / Iyf).Power(2) - 2 * Yof * (Xof - hxf) * (Ixf / Iyf) + hxf.Power(2) + Yof.Power(2)) + Ixf + Iyf);

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

            var sigma_t = ((1.0) / (A * ro.Power(2))) * (G * J * ((Math.PI.Power(2) * E * Cw) / (Kz * Lz).Power(2)));

            //Torsional flexural buckling.
            var beta = 1 - (Xo / ro).Power(2);
            var F_e2 = ((1) / (2 * beta)) * ((sigma_ex + sigma_t) - Math.Sqrt((sigma_ex + sigma_t).Power(2) + 4 * beta * sigma_ex * sigma_ey));
            var F_cr = Math.Min(F_e1, F_e2);
            var Ag = (aPrime + 2 * bPrime + 2 * cPrime) * t;
            var P_cre = F_cr * Ag;
            return P_cre;
        }

        #endregion



        #region Moment

        public static MomentDSResistanceOutput AsMomentResistance(this UnStiffenedSection unstiffenedSection, Material material, LengthBracingConditions bracingConditions)
        {
            var m_crl = unstiffenedSection.GetMomentLBResistance(material);
            return unstiffenedSection.AsMomentResistance(material,bracingConditions,m_crl);
        }

        public static MomentDSResistanceOutput AsMomentResistance(this LippedSection lippedSection, Material material, LengthBracingConditions bracingConditions)
        {
            var m_crl = lippedSection.GetMomentLBResistance(material);
            return lippedSection.AsMomentResistance(material, bracingConditions, m_crl);
        }

        private static MomentDSResistanceOutput AsMomentResistance(this Section section, Material material, LengthBracingConditions bracingConditions,double m_crl)
        {
            var Zg = section.Properties.Zg;
            var Fy = material.Fy;

            var My = Zg * Fy;

            var M_cre = section.GetMomentGBResistance(material,bracingConditions);
            var M_crd = section.GetMomentDBResistance(material,bracingConditions);

            //Nominal Flexural strength (Mne) for LTB.
            var Mne = 0.0;
            if(M_cre < 0.56 * My)
            {
                Mne = M_cre;
            }else if( M_cre > 2.78 * My)
            {
                Mne = My;
            }
            else
            {
                Mne = (10 / 9.0) * My * (1 - ((10*My)/(36*M_cre)));
            }

            //Nominal Flexural strength (Mnl) for local buckling.
            var lambda_L = Math.Sqrt(Mne / m_crl);
            var Mnl = lambda_L <= 0.776
                ? Mne
                :(1-0.15*(m_crl/Mne).Power(0.4))*(m_crl/Mne).Power(0.4)*Mne;

            var lambda_d = Math.Sqrt(My / M_crd);
            var Mnd = lambda_d <= 0.673
                ? My
                : (1-0.22*(M_crd/My).Power(0.5))*(M_crd/My).Power(0.5)*My;
            var nominalLoads = new List<Tuple<double, FailureMode>>()
            {
                Tuple.Create(Mne,FailureMode.GLOBALBUCKLING),
                Tuple.Create(Mnl,FailureMode.LOCALBUCKLING),
                Tuple.Create(Mnd,FailureMode.DISTRORTIONALBUCKLING)
            };
            var nominalLoad = nominalLoads.OrderBy(tup => tup.Item1).First();
            var result = new MomentDSResistanceOutput(nominalLoad.Item1, 0.9, $"{nominalLoad.Item2.GetDescription()} governs");
            return result;
        }

        public static double GetMomentLBResistance(this LippedSection lippedSection, Material material)
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

        public static double GetMomentLBResistance(this UnStiffenedSection unstiffenedSection, Material material)
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
            return m_crl;
        }

        private static double GetMomentLBResistanceFromInteractionMethod(this LippedSection lippedSection, Material material)
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
            var f2 = (0.5*H-cPrime) / (0.5*H);
            var epslon = (f1-f2) / (f1);
            var k_flange_lip = (8.55 * epslon - 11.07) * (cPrimeOverbPrime.Power(2)) + ((3.95 - 1.59 * epslon) * (cPrimeOverbPrime)) + 4;
            var Fcr_flange_lip = k_flange_lip * EOverVTerm * tOverbPrime.Power(2);

            //Flange - web local buckling
             f1 = 1;
            f2 = -1;
            epslon = (f1-f2) / (f1);

            var k_flange_web = 1.125 * Math.Min(4,(0.5*epslon.Power(3)+4*epslon.Power(2)+4)*(bPrimeOveraPrime.Power(2)));
            var Fcr_flange_web = k_flange_web * EOverVTerm * tOverbPrime.Power(2);
            
            var F_crl = Math.Min(Fcr_flange_lip, Fcr_flange_web);
            var M_crl = Zg * F_crl;

            return M_crl;
        }

        private static double GetMomentLBResistanceFromElementMethod(this LippedSection lippedSection, Material material)
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
            var f2 =  (0.5 * H - cPrime) / (0.5 * H);
            var epslon = (f1 - f2) / f1;
            var k_lip = 1.4 * epslon.Power(2) - 0.25 * epslon + 0.425;
            var Fcr_lip = k_lip * EOverVTerm * (t / cPrime).Power(2);

            var F_crl = Math.Min(Math.Min(Fcr_flange, Fcr_web), Fcr_lip);
            var M_crl = Zg * F_crl;
            return M_crl;
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

            var Lcr_vTerm = (4*Math.PI.Power(4)*aPrime*(1-v.Power(2))) / (t.Power(3));
            var Lcr_ITerm = Ixf * (Xof - hxf).Power(2) + Cwf - (Ixyf.Power(2)/Iyf) * (Xof - hxf).Power(2);
            var Lcr = (Lcr_vTerm * Lcr_ITerm + ((Math.PI.Power(4) * aPrime.Power(4)) / (720.0)));
            Lcr = Math.Min(Lcr,Lu);

            var piOverLCr = Math.PI / Lcr;
            var K_phi_fe = (piOverLCr).Power(4) * (E * Ixf * (Xof - hxf).Power(2) + E * Cwf - E * (Ixyf.Power(2) / Iyf) * (Xof - hxf).Power(2)) * piOverLCr.Power(2) * G * Jf;

            var K_phi_fg = piOverLCr.Power(2) * (Af * ((Xof - hxf).Power(2) * (Ixyf / Iyf).Power(2) - 2 * Yof * (Xof - hxf) * (Ixf / Iyf) + hxf.Power(2) + Yof.Power(2)) + Ixf + Iyf);

            var k_phi_we = ((E * t.Power(3)) / (12 * (1 - v.Power(2)))) * ((3/aPrime) + piOverLCr.Power(2)*((19*aPrime)/(60.0)) + piOverLCr.Power(4)*(aPrime.Power(3)/240));

            var sai = -1;
            var k_phi_wg_term1 = ((aPrime*t*Math.PI.Power(2)) / (13440));
            var LcrOveraPrime = Lcr / aPrime;
            var aPrimeOverLcr = aPrime / Lcr;
            var k_phi_wg_term2_Numen = (45360*sai+62160) * LcrOveraPrime.Power(2) +448*Math.PI.Power(2) + aPrimeOverLcr.Power(2)*(53+3*sai)*Math.PI.Power(4);
            var k_phi_wg_term2_Dunem = Math.PI.Power(4) + 28 * Math.PI.Power(2) * LcrOveraPrime.Power(2) + 420 * LcrOveraPrime.Power(4);
            var k_phi_wg = k_phi_wg_term1 * (k_phi_wg_term2_Numen / k_phi_wg_term2_Dunem);
            var F_crd = (K_phi_fe + k_phi_we) / (K_phi_fg + k_phi_wg);
            var M_crd = F_crd * Zg;
            return M_crd;
        }

        private static double GetMomentGBResistance(this Section input, Material material, LengthBracingConditions bracingConditions)
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

            var sigma_t = ((1.0) / (A * ro.Power(2))) * (G * J * ((Math.PI.Power(2) * E * Cw) / (Kz * Lz).Power(2)));

            var beta = 1 - (Xo / ro).Power(2);
            var F_e2 = (1 / (2 * beta)) * ((sigma_ex - sigma_t) - Math.Sqrt((sigma_ex + sigma_t).Power(2) - 4 * beta * sigma_ex * sigma_t));
            var F_cre = Math.Min(F_e1, F_e2);
            var M_cre = A* F_cre;
            return M_cre;
        }

        #endregion

    }
}
