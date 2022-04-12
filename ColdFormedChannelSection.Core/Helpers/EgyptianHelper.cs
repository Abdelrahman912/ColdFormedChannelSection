using ColdFormedChannelSection.Core.Entities;
using ColdFormedChannelSection.Core.Enums;
using ColdFormedChannelSection.Core.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ColdFormedChannelSection.Core.Helpers
{
    public static class EgyptianHelper
    {

        #region Moment

        public static MomentResistanceOutput AsEgyptMomentResistance(this LippedSection section, Material material, LengthBracingConditions bracingConditions)
        {
            throw new NotImplementedException();
        }

        public static MomentResistanceOutput AsEgyptMomentResistance(this UnStiffenedSection section, Material material, LengthBracingConditions bracingConditions)
        {
            throw new NotImplementedException();
        }

        #endregion

        #region Compression

        private static bool IsValid(this LippedSection section)
        {
            var c_over_t = Tuple.Create(section.Properties.CPrime / section.Dimensions.ThicknessT, 40.0);
            var b_over_t = Tuple.Create(section.Properties.BPrime / section.Dimensions.ThicknessT, 60.0);
            var a_over_t = Tuple.Create(section.Properties.APrime / section.Dimensions.ThicknessT, 300.0);

            var allows = new List<Tuple<double, double>>()
            {
                b_over_t,c_over_t,a_over_t
            };
            return !allows.Any(tuple => tuple.Item1 > tuple.Item2) ;
        }


        private static bool IsValid(this UnStiffenedSection section)
        {
            var b_over_t = Tuple.Create(section.Properties.BPrime / section.Dimensions.ThicknessT, 40.0);

            var a_over_t = Tuple.Create(section.Properties.APrime / section.Dimensions.ThicknessT, 300.0);

            var allows = new List<Tuple<double, double>>()
            {
                b_over_t,a_over_t
            };
            return !allows.Any(tuple => tuple.Item1 > tuple.Item2);

        }


        private static double GetEgyptReducedArea(this LippedSection section , Material material)
        {
            var E = material.E;
            var Fy = material.Fy;
            var C = section.Dimensions.TotalFoldWidthC;
            var b = section.Properties.BSmall;
            var t = section.Dimensions.ThicknessT;
            var a_prime = section.Properties.APrime;
            var b_prime = section.Properties.BPrime;
            var c_prime = section.Properties .CPrime;
            var c = section.Properties.CSmall;

            var b_over_t = b / t;
            var c_over_b = C / b;
            var s = 1.28 * Math.Sqrt(E/Fy);

            var be = b_prime;
            var ce = c_prime;
            var Is = (t * c.Power(3)) / 12;

            if(b_over_t <=s/3 && c_over_b <= 0.25)
            {
                be = b_prime;
                ce = c_prime;
            }else if( b_over_t < s && b_over_t > s/3 && c_over_b > 0.25 && c_over_b<= 0.8)
            {
                var Ia = 399 * (((b_over_t)/s)-0.33).Power(3) * t.Power(4);
                var Kf_1 = (4.82-5*(C/b)) * (Is/Ia).Power(0.5) + 0.43;
                var Kf_2 = 5.25 - 5 * (C/b);
                var Kf = Math.Min(Kf_1,Kf_2);
                var lambda_f = ((b_prime/t)/44) * Math.Sqrt(Fy/Kf);
                var sai_f = 1.0;
                var row_f = Math.Min(1, (lambda_f - 0.15 - 0.05 * sai_f) / (lambda_f.Power(2)));
                be = row_f * b_prime;
                var Kc = 0.43;
                var lambda_c = ((c_prime / t) / 59) * Math.Sqrt(Fy / Kc);
                var sai_c = 1.0;
                var row_c = Math.Min(1, ((lambda_c - 0.15 - 0.05 * sai_c) / lambda_c.Power(2)));
                var Ri = Math.Min(Is / Ia , 1);
                ce = row_c * c_prime * Ri;
            }
            else if(b_over_t < s && b_over_t > s / 3 && c_over_b <= 0.25)
            {
                var Ia = 399 * (((b_over_t) / s) - 0.33).Power(3) * t.Power(4);
                var Kf = Math.Min(3.57 * (Is / Ia).Power(0.5) + 0.43, 4);
                var lambda_f = ((b_prime / t) / 44) * Math.Sqrt(Fy / Kf);
                var sai_f = 1.0;
                var row_f = Math.Min(1, (lambda_f - 0.15 - 0.05 * sai_f) / (lambda_f.Power(2)));
                be = row_f * b_prime;
                var Kc = 0.43;
                var lambda_c = ((c_prime / t) / 59) * Math.Sqrt(Fy / Kc);
                var sai_c = 1.0;
                var row_c = Math.Min(1, ((lambda_c - 0.15 - 0.05 * sai_c) / lambda_c.Power(2)));
                var Ri = Math.Min(Is / Ia, 1);
                ce = row_c * c_prime * Ri;
            }
            else if (b_over_t >=s & c_over_b>0.25 && c_over_b <= 0.8)
            {

                var Ia = (((115*(b/t))/(s)) + 5) * t.Power(4);
                var Kf_1 = (4.82-5*(C/b)) * (Is/Ia).Power(1/3.0) + 0.43;
                var Kf_2 = 5.25 - 5 * (C/b);
                var Kf = Math.Min(Kf_1,Kf_2);
                var lambda_f = ((b_prime / t) / 44) * Math.Sqrt(Fy / Kf);
                var sai_f = 1.0;
                var row_f = Math.Min(1, (lambda_f - 0.15 - 0.05 * sai_f) / (lambda_f.Power(2)));
                be = row_f * b_prime;
                var Kc = 0.43;
                var lambda_c = ((c_prime / t) / 59) * Math.Sqrt(Fy / Kc);
                var sai_c = 1.0;
                var row_c = Math.Min(1, ((lambda_c - 0.15 - 0.05 * sai_c) / lambda_c.Power(2)));
                var Ri = Math.Min(Is / Ia, 1);
                ce = row_c * c_prime * Ri;

            }
            else if (b_over_t >= s && c_over_b <= 0.25)
            {
                var Ia = (((115 * (b / t)) / (s)) + 5) * t.Power(4);
                var Kf = Math.Min(3.57 * (Is / Ia).Power(1 / 3.0) + 0.43, 4);
                var lambda_f = ((b_prime / t) / 44) * Math.Sqrt(Fy / Kf);
                var sai_f = 1.0;
                var row_f = Math.Min(1, (lambda_f - 0.15 - 0.05 * sai_f) / (lambda_f.Power(2)));
                be = row_f * b_prime;
                var Kc = 0.43;
                var lambda_c = ((c_prime / t) / 59) * Math.Sqrt(Fy / Kc);
                var sai_c = 1.0;
                var row_c = Math.Min(1, ((lambda_c - 0.15 - 0.05 * sai_c) / lambda_c.Power(2)));
                var Ri = Math.Min(Is / Ia, 1);
                ce = row_c * c_prime * Ri;
            }

            var sai_w = 1.0;
            var Kw = 4.0;
            var lambda_w = ((a_prime/t)/44) * Math.Sqrt(Fy/Kw);
            var row_w = Math.Min(1,(1.1*lambda_w - 0.16 - 0.1 *sai_w)/lambda_w.Power(2));
            var ae = row_w*a_prime;
            var Ae = t * (2 * be + 2 * ce + ae);
            return Ae;
        }

        private static double GetEgyptReducedArea(this UnStiffenedSection section, Material material)
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
            var lambda_f = ((b_prime/t)/59) * Math.Sqrt(Fy / Kf);
            var row_f = Math.Min(1, (lambda_f - 0.15 - 0.05 * sai_f) / (lambda_f.Power(2)));
            var be = row_f * b_prime;

            var sai_w = 1.0;
            var Kw = 4.0;
            var lambda_w = ((a_prime/t)/44) * Math.Sqrt(Fy/Kw);
            var row_w = Math.Min(1, (1.1 * lambda_w - 0.16 - 0.1 * sai_w) / lambda_w.Power(2));
            var ae = row_w * a_prime;
            var Ae = t * (2 * be + ae);
            return Ae;
        }

        private static double GetEgyptReducedAreaEE(this LippedSection section , Material material)
        {
            var a_prime = section.Properties.APrime;
            var b_prime = section.Properties.BPrime;
            var c_prime = section .Properties.CPrime;
            var t = section.Dimensions.ThicknessT;
            var E = material.E;
            var Fy = material.Fy;

            //Web
            var E_over_Fy_sqrt = Math.Sqrt(E / Fy);
            var a_over_t = a_prime / t;
            var a_ee = Math.Min(1.92 * t * E_over_Fy_sqrt * (1 - 0.385 * (E_over_Fy_sqrt / a_over_t)), a_prime);

            //Flange 
            var b_over_t = b_prime / t;
            var b_ee =Math.Min( 1.92 * t * E_over_Fy_sqrt * (1 - 0.385 * (E_over_Fy_sqrt / b_over_t)),b_prime);

            //Lip
            var c_over_t = c_prime / t;
            var c_ee =Math.Min( 0.78 * t * E_over_Fy_sqrt * (1 - (0.13 /c_over_t) * (E_over_Fy_sqrt)),c_prime);

            var A_ee = t * (a_ee + 2 * b_ee+2*c_ee);
            return A_ee;
        }

        private static double GetEgyptReducedAreaEE(this UnStiffenedSection section , Material material )
        {
            var a_prime = section.Properties.APrime;
            var b_prime = section.Properties.BPrime;
            var t = section.Dimensions.ThicknessT;
            var E = material.E;
            var Fy = material.Fy;

            //Web
            var E_over_Fy_sqrt = Math.Sqrt(E / Fy);
            var a_over_t = a_prime/t;
            var a_ee = Math.Min(1.92 * t * E_over_Fy_sqrt * (1 - 0.385 * (E_over_Fy_sqrt / a_over_t)),a_prime);

            //Flange
            var b_ee =Math.Min(b_prime, 0.78 * t * E_over_Fy_sqrt * (1-(0.13/(b_prime/t))*E_over_Fy_sqrt));


            var A_ee = t * (a_ee + 2 * b_ee );
            return A_ee;
        }

        private static double GetEgyptCompressionLBResistance(this LippedSection section , Material material)=>
            section.GetEgyptReducedArea(material) * material.Fy;

        private static double GetEgyptCompressionLBResistance(this UnStiffenedSection section , Material material)=>
            section.GetEgyptReducedArea(material) * material.Fy;

        private static double GetEgyptCompressionFBResistance(this Section section , Material material , LengthBracingConditions bracingConditions , double Aee)
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
            var Fex = (Math.PI.Power(2)*E) / ((Kx*Lx)/(ix)).Power(2);
            var Fey = (Math.PI.Power(2) * E) / ((Ky * Ly) / (iy)).Power(2);

            var Fe = Math.Min(Fex, Fey);
            var lambda_c = Math.Sqrt(Fy / Fe);
            var Fcr = 0.648 * (Fy / lambda_c.Power(2));
            if (lambda_c * Math.Sqrt(Q) <= 1.1)
                Fcr = Fy * Q * (1-0.384 * Q * lambda_c.Power(2));
            var pn = Fcr * A;
            return pn;
        }

        //private static double GetEgyptCompressionFBResistance(this LippedSection section, Material material , LengthBracingConditions bracingConditions)
        //{
        //    var Aee = section.GetEgyptReducedAreaEE(material);
        //    var pn = section.GetEgyptCompressionFBResistance(material,bracingConditions,Aee);
        //    return pn;
        //}

        //private static double GetEgyptCompressionFBResistance(this UnStiffenedSection section, Material material, LengthBracingConditions bracingConditions)
        //{
        //    var Aee = section.GetEgyptReducedAreaEE(material);
        //    var pn = section.GetEgyptCompressionFBResistance(material, bracingConditions, Aee);
        //    return pn;
        //}

        //private static double GetEgyptCompressionTFBResistance(this LippedSection section , Material material , LengthBracingConditions bracingConditions)
        //{
        //    var Aee= section.GetEgyptReducedAreaEE(material);
        //    var pn = section.GetEgyptCompressionTFBResistance(material , bracingConditions, Aee);
        //    return pn;
        //}

        //private static double GetEgyptCompressionTFBResistance(this UnStiffenedSection section, Material material, LengthBracingConditions bracingConditions)
        //{
        //    var Aee = section.GetEgyptReducedAreaEE(material);
        //    var pn = section.GetEgyptCompressionTFBResistance(material, bracingConditions, Aee);
        //    return pn;
        //}

        private static double GetEgyptCompressionTFBResistance(this Section section , Material material , LengthBracingConditions bracingConditions,double Aee)
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
            var beta = 1 - (Xo.Power(2)/ro_squared);
            var Fez = (((Math.PI.Power(2)*E*Cw)/(Kz * Lz).Power(2))+G*J) * (1/(A*ro_squared));
            var Fex = (Math.PI.Power(2) * E) / ((Kx * Lx) / (ix)).Power(2);

            var Fe = ((Fex+Fez)/(2*beta)) * (1-Math.Sqrt(1-((4*beta*Fex*Fez)/(Fex+Fez).Power(2))));

            var lambda_c = Math.Sqrt(Fy / Fe);
            var Fcr = 0.648 * (Fy / lambda_c.Power(2));
            var Q = Aee / A;
            if (lambda_c * Math.Sqrt(Q) <= 1.1)
                Fcr = Fy * Q * (1 - 0.384 * Q * lambda_c.Power(2));
            var pn = Fcr * A;
            return pn;
        }

        public static CompressionResistanceOutput AsEgyptCompressionResistance(this LippedSection section, Material material, LengthBracingConditions bracingConditions)
        {
            if (!section.IsValid())
                return new CompressionResistanceOutput(0.0, 0.8, FailureMode.UNSAFE, "ton");
            var pn1 = Tuple.Create(section.GetEgyptCompressionLBResistance(material), FailureMode.LOCALBUCKLING);
            var Aee = section.GetEgyptReducedAreaEE(material);
            var pn2 = Tuple.Create(section.GetEgyptCompressionFBResistance(material, bracingConditions,Aee), FailureMode.FLEXURALBUCKLING);
            var pn3 = Tuple.Create(section.GetEgyptCompressionTFBResistance(material, bracingConditions,Aee), FailureMode.TORSIONALBUCKLING);
            var pns = new List<Tuple<double, FailureMode>>()
            {
                pn1, pn2, pn3
            };
            var pn = pns.OrderBy(tuple => tuple.Item1).First();
            var result = new CompressionResistanceOutput(pn.Item1, 0.8, pn.Item2, "ton");
            return result;
        }

        public static CompressionResistanceOutput AsEgyptCompressionResistance(this UnStiffenedSection section, Material material, LengthBracingConditions bracingConditions)
        {
            if (!section.IsValid())
                return new CompressionResistanceOutput(0.0, 0.8, FailureMode.UNSAFE, "ton");
            var pn1 = Tuple.Create(section.GetEgyptCompressionLBResistance(material), FailureMode.LOCALBUCKLING);
            var Aee = section.GetEgyptReducedAreaEE(material);
            var pn2 = Tuple.Create(section.GetEgyptCompressionFBResistance(material, bracingConditions,Aee), FailureMode.FLEXURALBUCKLING);
            var pn3 = Tuple.Create(section.GetEgyptCompressionTFBResistance(material, bracingConditions,Aee), FailureMode.TORSIONALBUCKLING);
            var pns = new List<Tuple<double, FailureMode>>()
            {
                pn1, pn2, pn3
            };
            var pn = pns.OrderBy(tuple => tuple.Item1).First();
            var result = new CompressionResistanceOutput(pn.Item1, 0.8, pn.Item2, "ton");
            return result;
        }

        #endregion

    }
}
