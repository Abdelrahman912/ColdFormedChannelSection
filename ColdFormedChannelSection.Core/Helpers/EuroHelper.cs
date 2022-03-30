using ColdFormedChannelSection.Core.Entities;
using ColdFormedChannelSection.Core.Enums;
using ColdFormedChannelSection.Core.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ColdFormedChannelSection.Core.Helpers
{
    public static class EuroHelper
    {

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


        #region Compression


        private static double ReduceLippedSection(this LippedSection section,Material material , double Kw , double be2_intial, double ce_intial)
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
            var isEqual = false;
            do
            {
                var As = t * (be2 + ce);
                var b1 = b_prime - ((be2.Power(2)*(t/2.0))/(As));
                var k = ((E*t.Power(3))/(4*(1-v.Power(2))))*((1)/(b1.Power(2)*a_prime + b1.Power(3)+0.5*b1.Power(2)*a_prime*Kw));
                var Is = ((be2*t.Power(3))/(12.0)) + ((ce.Power(3)*t)/(12.0)) + (be2*t *((ce.Power(2))/(2*(be2+ce))).Power(2))+(ce*t*((ce/2)-((ce.Power(2))/(2*(be2+ce)))).Power(2));
                var sigma_cr = (2*Math.Sqrt(k*E*Is)) / (As);
                var lambda_d = Math.Sqrt(Fy/sigma_cr);
                var Xd_new = 1.0;
                if (lambda_d <= 0.65)
                    Xd_new = 1.0;
                else if (lambda_d >= 1.38)
                    Xd_new = (0.66) / (lambda_d);
                else
                    Xd_new = 1.47 - 0.723 * lambda_d;

                isEqual = Math.Abs(Xd - Xd_new) <= 0.0001;
                Xd = Xd_new;
            } while (!isEqual);
            return Xd;
        }

        private static double ReduceWeb(this Section section , Material material)
        {
            var a_prime = section.Properties.APrime;
            var t = section.Dimensions.ThicknessT;
            var Fy = material.Fy;

            var sai_w = 1.0;
            var kw = 4.0;
            var epslon = Math.Sqrt(235.0 / Fy);
            var lambda_w = (a_prime/t) / (28.4*epslon*Math.Sqrt(kw));

            var lambda_w_limit = 0.5 + Math.Sqrt(0.85 - 0.055 * sai_w);
            var row_w = 1.0;
            if (lambda_w > lambda_w_limit)
                row_w = Math.Min(1.0, ((lambda_w - 0.055 * (3 + lambda_w)) / (lambda_w.Power(2))));
            var ae = row_w * a_prime;
            return ae;
        }

        private static double GetEuroReducedArea(this LippedSection section, Material material)
        {
            var Fy = material.Fy;
            var b_prime = section.Properties.BPrime;
            var t = section.Dimensions.ThicknessT;
            var c_prime = section.Properties.CPrime;

            

            //Flange.
            var epslon = Math.Sqrt(235.0 / Fy);
            var sai_f = 1.0;
            var kf = 4.0;
            var lambda_f = (b_prime/t) / (28.4*epslon*Math.Sqrt(kf));

            var lambda_f_limit = 0.5 + Math.Sqrt(0.085-0.055*sai_f);
            var row_f = 1.0;
            if (lambda_f > lambda_f_limit)
                row_f = Math.Min(1.0, ((lambda_f-0.055*(3+sai_f))/(lambda_f.Power(2))));
            var be = row_f * b_prime;
            var be1 = 0.5 * be;
            var be2 = 0.5 * be;
            //Lip.
            var kc = 0.5;
            if (c_prime / b_prime > 0.35)
                kc = 0.5 + 0.83 * ((c_prime / b_prime) - 0.35).Power(2.0 / 3.0);

            var lambda_c = (c_prime / t) / (28.4 * epslon * Math.Sqrt(kc));
            var row_c = 1.0;
            if (lambda_c > 0.748)
                row_c = Math.Min(1.0, (lambda_c - 0.188) / (lambda_c.Power(2)));
            var ce = row_c * c_prime;

           var Xd = section.ReduceLippedSection(material, 1.0, be2, ce);

            //Flange.
            var lambda_f_red = lambda_f * Math.Sqrt(Xd);
            if (lambda_f_red > lambda_f_limit)
                row_f = Math.Min(1.0, ((lambda_f_red - 0.055 * (3 + sai_f)) / (lambda_f_red.Power(2))));
            var be_red = row_f * b_prime;
            be2 = 0.5 * be_red;

            //Lip
            var lambda_c_red = lambda_c * Math.Sqrt(Xd);
            if (lambda_c_red > 0.748)
                row_c = Math.Min(1.0, (lambda_c_red - 0.188) / (lambda_c_red.Power(2)));
            ce = row_c * c_prime;


            //Web.
            var ae = section.ReduceWeb(material);

            var Ae = t * (2 * be1 + ae + 2 * Xd * (be2 + ce));

            return Ae;
        }

        private static double GetEuroReducedArea(this UnStiffenedSection section, Material material)
        {
            var Fy = material.Fy;
            var b_prime = section.Properties.BPrime;
            var t = section.Dimensions.ThicknessT;

            var epslon = Math.Sqrt(235.0 / Fy);
            var kf = 0.43;
            var lambda_f = (b_prime / t) / (28.4 * epslon * Math.Sqrt(kf));
            var row_f = 1.0;
            if (lambda_f > 0.748)
                row_f = Math.Min(1.0, (lambda_f - 0.188) / (lambda_f.Power(2)));

            var be = row_f* b_prime;
            var be1  = 0.5 * be;
            var be2 = 0.5 * be;

            var ae = section.ReduceWeb(material);

            var Ae = t * (2 * be1 + ae + 2 * be2);

            return Ae;
        }


        public static CompressionResistanceOutput AsEuroCompressionResistance(this LippedSection section, Material material, LengthBracingConditions bracingConditions)
        {
            if (!section.IsValid())
                return new CompressionResistanceOutput(0.0, 0.85, FailureMode.UNSAFE);
            var Ae = section.GetEuroReducedArea(material);
            var pn1 = Tuple.Create(section.GetEuroCompressionLBResistance(material,Ae), FailureMode.LOCALBUCKLING);
            var pn2 = Tuple.Create(section.GetEuroCompressionFBResistance(material, bracingConditions,Ae , 0.34), FailureMode.FLEXURALBUCKLING);
            var pn3 = Tuple.Create(section.GetEuroCompressionTFBResistance(material, bracingConditions,Ae,0.34), FailureMode.TORSIONALBUCKLING);
            var pns = new List<Tuple<double, FailureMode>>()
            {
                pn1, pn2, pn3
            };
            var pn = pns.OrderBy(tuple => tuple.Item1).First();
            var result = new CompressionResistanceOutput(pn.Item1, 0.85, pn.Item2);
            return result;
        }

        public static CompressionResistanceOutput AsEuroCompressionResistance(this UnStiffenedSection section, Material material, LengthBracingConditions bracingConditions)
        {
            if (!section.IsValid())
                return new CompressionResistanceOutput(0.0, 0.85, FailureMode.UNSAFE);
            var Ae = section.GetEuroReducedArea(material);
            var pn1 = Tuple.Create(section.GetEuroCompressionLBResistance(material, Ae), FailureMode.LOCALBUCKLING);
            var pn2 = Tuple.Create(section.GetEuroCompressionFBResistance(material, bracingConditions, Ae, 0.49), FailureMode.FLEXURALBUCKLING);
            var pn3 = Tuple.Create(section.GetEuroCompressionTFBResistance(material, bracingConditions, Ae, 0.49), FailureMode.TORSIONALBUCKLING);
            var pns = new List<Tuple<double, FailureMode>>()
            {
                pn1, pn2, pn3
            };
            var pn = pns.OrderBy(tuple => tuple.Item1).First();
            var result = new CompressionResistanceOutput(pn.Item1, 0.85, pn.Item2);
            return result;
        }


        private static double GetEuroCompressionLBResistance(this Section section, Material material, double Ae) =>
            Ae * material.Fy;

        private static double GetEuroCompressionFBResistance(this Section section , Material material,LengthBracingConditions lengthBracingConditions, double Ae , double alpa_w)
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

            var lambda_1 = Math.PI * Math.Sqrt(E/Fy);

            var lambda_y = ((ky*Ly)/iy) * (Math.Sqrt(Ae/A)/lambda_1);
            var lambda_x = ((Kx * Lx) / ix) * (Math.Sqrt(Ae / A) / lambda_1);

            var phi_y = 0.5 * (1 + alpa_w * (lambda_y-0.2) + lambda_y.Power(2));
            var phi_x = 0.5 * (1+alpa_w *(lambda_x-0.2)+lambda_x.Power(2));

            var Xy = Math.Min(1.0, (1 / (phi_y + Math.Sqrt(phi_y.Power(2) - lambda_y.Power(2)))));
            var Xx = Math.Min(1.0, (1.0 / (phi_x + Math.Sqrt(phi_x.Power(2) - lambda_x.Power(2)))));
            var X = Math.Min(Xx, Xy);
            var Pn = X * Ae * Fy;
            return Pn;
        }


        private static double GetEuroCompressionTFBResistance(this Section section , Material material , LengthBracingConditions lengthBracingConditions, double Ae, double alpha_w)
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
            var Ncr = (1/io_squared) * (G*J + ((Math.PI.Power(2)*Cw*E)/(Kz*Lz).Power(2)));
            var lambda_t = Math.Sqrt((Ae * Fy) / (Ncr));
            var phi_t = 0.5 * (1 + alpha_w * (lambda_t - 0.2) + lambda_t.Power(2));
            var Xt = Math.Min(1.0, (1 / (phi_t + Math.Sqrt(phi_t.Power(2) - lambda_t.Power(2)))));
            var Pn = Xt * Ae * Fy;
            return Pn;
        }

        #endregion



    }
}
