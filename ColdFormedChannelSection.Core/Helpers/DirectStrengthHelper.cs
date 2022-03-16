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
        private enum CompressionFailure
        {
            [Description("Local Buckling")]
            LOCALBUCKLING,
            [Description("Distortional Buckling")]
            DISTRORTIONALBUCKLING,
            [Description("Global Buckling")]
            GLOBALBUCKLING
        }


        public static CompressionDSResistanceOutput AsCompressionResistance(this LippedSection lippedSection, Material material, LengthBracingConditions bracingConditions)
        {
            var p_crl = lippedSection.GetLBResistance(material);
            return lippedSection.AsCompressionResistance(material, bracingConditions,p_crl);
        }

        public static CompressionDSResistanceOutput AsCompressionResistance(this UnStiffenedSection unstiffenedSection, Material material, LengthBracingConditions bracingConditions)
        {
            var p_crl = unstiffenedSection.GetLBResistance(material);
            return unstiffenedSection.AsCompressionResistance(material, bracingConditions, p_crl);
        }

        private static CompressionDSResistanceOutput AsCompressionResistance(this Section section, Material material, LengthBracingConditions bracingConditions,double p_crl)
        {
            var Fy = material.Fy;
            var aPrime = section.Properties.APrime;
            var bPrime = section.Properties.BPrime;
            var cPrime = section.Properties.CPrime;
            var t = section.Dimensions.ThicknessT;

            var Ag = (aPrime + 2 * bPrime + 2 * cPrime) * t;
            var Py = Ag * Fy;

            var p_crd = section.GetDBResistance(material, bracingConditions);
            var p_cre = section.GetGBResistance(material, bracingConditions);

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
            var nominalLoads = new List<Tuple<double, CompressionFailure>>()
            {
                Tuple.Create(Pne,CompressionFailure.GLOBALBUCKLING),
                Tuple.Create(Pnl,CompressionFailure.LOCALBUCKLING),
                Tuple.Create(Pnd,CompressionFailure.DISTRORTIONALBUCKLING)
            };
            var nominalLoad = nominalLoads.OrderBy(tup=>tup.Item1).First();
            var result = new CompressionDSResistanceOutput(nominalLoad.Item1, 0.85, $"{nominalLoad.Item2.GetDescription()} governs");
            return result;
        }

        private static double GetLBResistance(this LippedSection lippedSection, Material material)
        {
            var Pcr = (lippedSection.Properties.CPrime / lippedSection.Properties.BPrime) < 0.6
                ? lippedSection.GetLBResistanceFromInteractionMethod(material)
                : lippedSection.GetLBResistanceFromElementMethod(material);
            return Pcr;
        }

        private static double GetLBResistanceFromInteractionMethod(this LippedSection lippedSection, Material material)
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

        private static double GetLBResistanceFromElementMethod(this LippedSection lippedSection, Material material)
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

        private static double GetLBResistance(this UnStiffenedSection lippedSection, Material material)
        {
            var EOverVTerm = ((Math.PI.Power(2) * material.E) / (12 * (1 - material.V.Power(2))));

            //Flange Local Buckling.
            var kFlange = 0.43;
            var Fcr_flange = kFlange * EOverVTerm * (lippedSection.Dimensions.ThicknessT / lippedSection.Properties.BPrime).Power(2);

            //Web Local Buckling.
            var kWeb = 4;
            var Fcr_web = kWeb * EOverVTerm * (lippedSection.Dimensions.ThicknessT / lippedSection.Properties.APrime).Power(2);


            var Fcr = Math.Min(Fcr_flange, Fcr_web);
            var Ag = (lippedSection.Properties.APrime + 2 * lippedSection.Properties.BPrime) * lippedSection.Dimensions.ThicknessT;
            var P_crl = Ag * Fcr;
            return P_crl;
        }

        private static double GetDBResistance(this Section input, Material material, LengthBracingConditions bracingConditions)
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

        private static double GetGBResistance(this Section input, Material material, LengthBracingConditions bracingConditions)
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

    }
}
