using ColdFormedChannelSection.Core.Entities;
using ColdFormedChannelSection.Core.Extensions;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ColdFormedChannelSection.Core.Helpers
{
    public static class AmericanHelper
    {

        private enum FailureMode
        {
            [Description("Local Buckling")]
            LOCALBUCKLING,
            [Description("Flexural Torsional Buckling")]
            TORSIONALBUCKLING,
            [Description("Flexural Buckling")]
            FLEXURALBUCKLING
        }
        private static double GetReducedArea(this LippedSection lippedSection, Material material, double F_ = 0)
        {
            var b = lippedSection.Dimensions.TotalFlangeWidthB;
            var t = lippedSection.Dimensions.ThicknessT;
            var C = lippedSection.Dimensions.TotalFoldWidthC;
            var c = lippedSection.Properties.CSmall;
            var a = lippedSection.Properties.ADimension;
            var E = material.E;
            var v = material.V;
            var Fy = F_ == 0 ? material.Fy : F_;
            var s = 1.28 * Math.Sqrt(E / Fy);
            var be = lippedSection.Dimensions.TotalFlangeWidthB;
            var Ce = lippedSection.Dimensions.TotalFoldWidthC;
            var ae = lippedSection.Properties.ADimension;
            var b_over_t = b / t;
            var e_over_v_term = (Math.PI.Power(2) * E) / (12 * (1 - v.Power(2)));
            if (b_over_t > (s / 3))
            {
                var Ia_1 = 399 * t.Power(4) * ((b - b_over_t / s) - 0.328).Power(3);
                var Ia_2 = t.Power(4) * (115 * (b - b_over_t / s) + 5);
                var Ia = Math.Min(Ia_1, Ia_2);
                var Is = (c.Power(3) * t) / 12;
                var Ri = Math.Min(Is / Ia, 1);
                var n_1 = (0.582 - (b_over_t / (4 * s)));
                var n_2 = (1.0 / 3.0);
                var n = Math.Min(n_1, n_2);

                //Flange.
                var Kf = Math.Min(4.0, (4.82 - ((5 * C) / (b))) * Ri.Power(n) + 0.43);
                var Fcr_f = Kf * e_over_v_term * (t / b).Power(2);
                var lambda_f = Math.Sqrt(Fy / Fcr_f);
                var row_f = Math.Min(1.0, (1 - (0.22 / lambda_f)) / (lambda_f));
                if (lambda_f > 0.673)
                    be = row_f * b;

                //Lip.
                var kc = 0.43;
                var Fcr_c = kc * e_over_v_term * (t / c).Power(2);
                var lambda_c = Math.Sqrt(Fy / Fcr_c - c);
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
            var Ae = t * (2 * Ce + 2 * be + ae);
            return Ae;
        }

        private static double GetReducedArea(this UnStiffenedSection unstiffenedSection, Material material, double F_ = 0)
        {
            var b = unstiffenedSection.Dimensions.TotalFlangeWidthB;
            var t = unstiffenedSection.Dimensions.ThicknessT;
            var a = unstiffenedSection.Properties.ADimension;
            var E = material.E;
            var v = material.V;
            var Fy = F_ == 0 ? material.Fy : F_;
            var be = unstiffenedSection.Dimensions.TotalFlangeWidthB;
            var ae = unstiffenedSection.Properties.ADimension;
            var e_over_v_term = (Math.PI.Power(2) * E) / (12 * (1 - v.Power(2)));

            //Flange.
            var kf = 0.43;
            var Fcr_f = kf * e_over_v_term * (t / b).Power(2);
            var lambda_f = Math.Sqrt(Fy / Fcr_f);
            var row_f = Math.Min(1.0, (1 - (0.22 / lambda_f) / lambda_f));
            if (lambda_f > 0.673)
                be = row_f * b;

            //Web.
            var kw = 4.0;
            var Fcr_w = kw * e_over_v_term * (t / a).Power(2);
            var lambda_w = Math.Sqrt(Fy / Fcr_w);
            var row_w = Math.Min(1.0, (1 - (0.22 / lambda_w)) / lambda_w);
            if (lambda_w > 0.673)
                ae = row_w * a;
            var Ae = t * (2 * be + ae);
            return Ae;
        }

        #region AISI

        public static CompressionDSResistanceOutput AsAISICompressionResistance(this LippedSection lippedSection, Material material, LengthBracingConditions bracingConditions)
        {
            if (!lippedSection.IsValidForCompression())
                return new CompressionDSResistanceOutput(0.0, 0.85, "Unsafe, Try Other Dimensions");
            var pn1 =Tuple.Create( lippedSection.GetCompressionLBResistance(material),FailureMode.LOCALBUCKLING);
            var pn2 =Tuple.Create( lippedSection.GetCompressionFBRessistance(material, bracingConditions),FailureMode.FLEXURALBUCKLING);
            var pn3 =Tuple.Create( lippedSection.GetCompressionTFBRessistance(material,bracingConditions),FailureMode.TORSIONALBUCKLING);
            var pns = new List<Tuple<double, FailureMode>>()
            {
                pn1, pn2, pn3
            };
            var pn = pns.OrderBy(tuple => tuple.Item1).First();
            var result = new CompressionDSResistanceOutput(pn.Item1, 0.85, pn.Item2.GetDescription());
            return result;
        }

        public static CompressionDSResistanceOutput AsDSCompressionResistance(this UnStiffenedSection unstiffenedSection, Material material, LengthBracingConditions bracingConditions)
        {
            if (!unstiffenedSection.IsValidForCompression())
                return new CompressionDSResistanceOutput(0.0, 0.85, "Unsafe, Try Other Dimensions");
            var pn1 = Tuple.Create(unstiffenedSection.GetCompressionLBResistance(material), FailureMode.LOCALBUCKLING);
            var pn2 = Tuple.Create(unstiffenedSection.GetCompressionFBRessistance(material, bracingConditions), FailureMode.FLEXURALBUCKLING);
            var pn3 = Tuple.Create(unstiffenedSection.GetCompressionTFBRessistance(material, bracingConditions), FailureMode.TORSIONALBUCKLING);
            var pns = new List<Tuple<double, FailureMode>>()
            {
                pn1, pn2, pn3
            };
            var pn = pns.OrderBy(tuple => tuple.Item1).First();
            var result = new CompressionDSResistanceOutput(pn.Item1, 0.85, pn.Item2.GetDescription());
            return result;
        }

        private static bool IsValidForCompression(this Section section)
        {
            var b_over_t = Tuple.Create(section.Dimensions.TotalFlangeWidthB / section.Dimensions.ThicknessT, 60.0);

            var c_over_t = Tuple.Create(section.Properties.CSmall / section.Dimensions.ThicknessT, 14.0);
            var a_over_t = Tuple.Create(section.Properties.ADimension / section.Dimensions.ThicknessT, 200.0);
            var C_over_t = Tuple.Create(section.Dimensions.TotalFoldWidthC / section.Dimensions.TotalFlangeWidthB, 0.8);

            var allows = new List<Tuple<double, double>>()
            {
                b_over_t,c_over_t,a_over_t,C_over_t
            };
            return !allows.Any(tuple => tuple.Item1 > tuple.Item2);
        }

        
        private static double GetCompressionLBResistance(this LippedSection section, Material material) =>
            section.GetReducedArea(material) * material.Fy;


        private static double GetCompressionLBResistance(this UnStiffenedSection section, Material material) =>
            section.GetReducedArea(material) * material.Fy;

        private static double GetCompressionFBStress(this Section section, Material material, LengthBracingConditions bracingConditions)
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

        private static double GetCompressionFBRessistance(this LippedSection lippedSection, Material material, LengthBracingConditions bracingConditions)
        {
            var F1 = lippedSection.GetCompressionFBStress(material, bracingConditions);
            var A1 = lippedSection.GetReducedArea(material, F1);
            var Pn = F1 * A1;
            return Pn;
        }

        private static double GetCompressionFBRessistance(this UnStiffenedSection unstiffenedSection, Material material, LengthBracingConditions bracingConditions)
        {
            var F1 = unstiffenedSection.GetCompressionFBStress(material, bracingConditions);
            var A1 = unstiffenedSection.GetReducedArea(material, F1);
            var Pn = F1 * A1;
            return Pn;
        }


        private static double GetCompressionTFBStress(this Section section, Material material, LengthBracingConditions bracingConditions)
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
            var Fe2 = (1 / 2 * beta) * ((sigma_ex + sigma_ez) - Math.Sqrt((sigma_ex + sigma_ez).Power(2) - 4 * beta * sigma_ez * sigma_ex));
            var lambda_2 = Math.Sqrt(Fy / Fe2);
            var lambda_squared = lambda_2.Power(2);
            if (lambda_2 <= 1.5)
            {
                var F2 = (0.658.Power(lambda_squared)) * Fy;
                return F2;
            }
            else
            {
                var F2 = (0.877/lambda_squared) * Fy;
                return F2;
            }
        }

        private static double GetCompressionTFBRessistance(this LippedSection lippedSection, Material material, LengthBracingConditions bracingConditions)
        {
            var F2 = lippedSection.GetCompressionTFBStress(material, bracingConditions);
            var A2 = lippedSection.GetReducedArea(material, F2);
            var Pn = F2 * A2;
            return Pn;
        }

        private static double GetCompressionTFBRessistance(this UnStiffenedSection unstiffenedSection, Material material, LengthBracingConditions bracingConditions)
        {
            var F2 = unstiffenedSection.GetCompressionTFBStress(material, bracingConditions);
            var A2 = unstiffenedSection.GetReducedArea(material, F2);
            var Pn = F2 * A2;
            return Pn;
        }
        
        #endregion

    }
}
