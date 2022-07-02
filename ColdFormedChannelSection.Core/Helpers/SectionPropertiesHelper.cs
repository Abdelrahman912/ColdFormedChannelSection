using ColdFormedChannelSection.Core.Entities;
using ColdFormedChannelSection.Core.Enums;
using ColdFormedChannelSection.Core.Extensions;
using CSharp.Functional.Constructs;
using System;
using System.Collections.Generic;
using System.Linq;
using static ColdFormedChannelSection.Core.Errors.Errors;
using static CSharp.Functional.Extensions.ValidationExtension;

namespace ColdFormedChannelSection.Core.Helpers
{
    public static class SectionPropertiesHelper
    {

        public static Validation<LippedCSection> AsLippedCSection(this SectionDimension sectionDim)
        {
            return sectionDim.CaclulateCSectionProperties(TypeOfSection.LIPPED)
                       .Map(sec => new LippedCSection(sec.Dimensions, sec.Properties as CSectionProperties));
        }


        public static Validation<UnStiffenedCSection> AsUnstiffenedCSection(this SectionDimension sectionDim)
        {
            return sectionDim.CaclulateCSectionProperties(TypeOfSection.UNSTIFFENED)
                             .Map(sec => new UnStiffenedCSection(sec.Dimensions, sec.Properties as CSectionProperties));
        }

        public static Validation<LippedZSection> AsLippedZSection(this SectionDimension sectionDim)
        {
            return sectionDim.CalculateZSectionProperties(TypeOfSection.LIPPED)
                       .Map(sec => new LippedZSection(sec.Dimensions, sec.Properties as ZSectionProperties));
        }


        public static Validation<UnStiffenedZSection> AsUnstiffenedZSection(this SectionDimension sectionDim)
        {
            return sectionDim.CalculateZSectionProperties(TypeOfSection.UNSTIFFENED)
                             .Map(sec => new UnStiffenedZSection(sec.Dimensions, sec.Properties as ZSectionProperties));
        }

        private static Validation<CSection> CaclulateCSectionProperties(this SectionDimension sectionDim, TypeOfSection channel)
        {

            var alpha = (int)channel;
            var H = sectionDim.TotalHeightH;
            var B = sectionDim.TotalFlangeWidthB;
            var R = sectionDim.InternalRadiusR;
            var t = sectionDim.ThicknessT;
            var C = sectionDim.TotalFoldWidthC;

            var tover2 = t / 2;

            //radius to center line
            var r = R + tover2;
            var a = H - (2 * r + t);


            var aOver2 = a / 2;

            var aPrime = H - t;
            var b = B - (r + tover2 + alpha * (r + tover2));
            var bPrime = B - (tover2 + alpha * tover2);
            var c = alpha * (C - (r + tover2));
            var cPrime = alpha * (C - tover2);
            var u = Math.PI * (r / 2);
            var A = t * (a + 2 * b + 2 * u + alpha * (2 * c + 2 * u));
            var Ix = 2 * t * (0.0417 * a.Power(3) + b * (aOver2 + r).Power(2) + 0.149 * r.Power(3) + alpha * (0.0833 * c.Power(3) + (c / 4) * (a - c).Power(2) + 0.149 * r.Power(3) + u * (aOver2 + 0.637 * r).Power(2)) + u * (aOver2 + 0.637 * r).Power(2));

            var hOver2 = H / 2;
            var Zg = Ix / hOver2;
            var bOver2 = b / 2;
            var XcPrime = ((2 * t) / (A)) * (b * (bOver2 + r) + u * (0.363 * r) + alpha * (u * (b + 1.637 * r) + c * (b + 2 * r)));

            var Iy = 2 * t * (b * (bOver2 + r).Power(2) + b.Power(3) / 12 + 0.356 * r.Power(3) + alpha * (c * (b + 2 * r).Power(2) + u * (b + 1.637 * r).Power(2) + 0.149 * r.Power(3))) - A * XcPrime.Power(2);

            var ix = Math.Sqrt(Ix / A);
            var iy = Math.Sqrt(Iy / A);

            var mNumenator = 3 * aPrime.Power(2) * bPrime + alpha * cPrime * (6 * aPrime.Power(2) - 8 * cPrime.Power(2));
            var mDnumenator = aPrime.Power(3) + 6 * aPrime.Power(2) * bPrime + alpha * cPrime * (8 * cPrime.Power(2) - 12 * aPrime * cPrime + 6 * aPrime.Power(2));

            var m = bPrime * (mNumenator / mDnumenator);

            var Xo = XcPrime + m;
            var J = ((t.Power(3)) / 3) * (a + 2 * b + 2 * u + alpha * (2 * c + 2 * u));
            var CwNumenator = 2 * aPrime.Power(3) * bPrime + 3 * aPrime.Power(2) * bPrime.Power(2) + alpha * (48 * cPrime.Power(4) + 112 * bPrime * cPrime.Power(3) + 8 * aPrime * cPrime.Power(3) + 48 * aPrime * bPrime * cPrime.Power(2) + 12 * aPrime.Power(2) * cPrime.Power(2) + 12 * aPrime.Power(2) * bPrime * cPrime + 6 * aPrime.Power(3) * cPrime);
            var CwDunemenator = 6 * aPrime.Power(2) * bPrime + (aPrime + alpha * 2 * cPrime).Power(3) - alpha * 24 * aPrime * cPrime.Power(2);
            var Cw = ((aPrime.Power(2) * bPrime.Power(2) * t) / 12) * (CwNumenator / CwDunemenator);
            var errs = new List<Tuple<double, string>>()
            {
                Tuple.Create(aPrime,"a prime is less than zero"),
                Tuple.Create(bPrime," b prime is less tahn zero"),
                Tuple.Create(cPrime,"c prime is less than zero"),
                Tuple.Create(a,"a is less than zero"),
                Tuple.Create(b,"b is less than zero"),
                Tuple.Create(c,"c is less than zero")
            };
            var errors = errs.Where(err => err.Item1 < 0).Select(err => LessThanZeroError($"Cannot use this section because {err.Item2}")).ToList();
            if (errors.Count > 0)
                return Invalid(errors);
            else
            {
                var properties = new CSectionProperties(aPrime, bPrime, cPrime, A, Ix, Zg, Iy, ix, iy, Xo, J, Cw, c, r, u, b, alpha, a);
                var sec = new CSection(sectionDim, properties);
                return sec;
            }

        }

        private static Validation<ZSection> CalculateZSectionProperties(this SectionDimension secDim, TypeOfSection z)
        {
            var alpha = (int)z;

            var H = secDim.TotalHeightH;
            var B = secDim.TotalFlangeWidthB;
            var R = secDim.InternalRadiusR;
            var t = secDim.ThicknessT;
            var C = secDim.TotalFoldWidthC;

            var r = R + (t / 2);
            var a = H - (2 * r + t);
            var aPrime = H - t;
            var gamma = Math.PI / 2;
            var tOver2 = t / 2;
            var aOver2 = a / 2;
            var b = B - (r + tOver2 + alpha * (r + tOver2));
            var bPrime = B - (tOver2 + alpha * tOver2);
            var c = alpha * (C - (r + tOver2));
            var cOver2 = c / 2;
            var bOver2 = b / 2;
            var cPrime = alpha * (C - tOver2);
            var u = Math.PI * r / 2;

            var A = t * (a + 2 * b + 2 * u + alpha * (2 * c + 2 * u));
            var IxFirstTerm = 0.0417 * a.Power(3);
            var IxSecondTerm = b * (aOver2 + r).Power(2);
            var IxThirdTerm = 0.149 * r.Power(3);
            var IxFourthTerm = u * (aOver2 + 0.637 * r).Power(2);
            var IxFifthTerm = (((gamma + Math.Sin(gamma) * Math.Cos(gamma)) / 2) - (Math.Sin(gamma).Power(2) / gamma)) * r.Power(3)
                               + u * (aOver2 + ((r * Math.Sin(gamma)) / gamma)).Power(2) + ((c.Power(3) * Math.Sin(gamma).Power(2)) / 12)
                               + c * (aOver2 + r * Math.Cos(gamma) - cOver2 * Math.Sin(gamma)).Power(2);
            var Ix = 2 * t * (IxFirstTerm + IxSecondTerm + IxThirdTerm + IxFourthTerm + alpha * IxFifthTerm);
            var HOver2 = H / 2;
            var Zg = Ix / HOver2;


            var IyFirstTerm = b * (bOver2 + r).Power(2);
            var IySecondTerm = b.Power(3) / 12;
            var IyThirdTerm = 0.356 * r.Power(3);
            var IyFourthTerm = c * (b + r * (1 + Math.Sin(gamma)) + cOver2 * Math.Cos(gamma)).Power(2)
                               + ((c.Power(3) * Math.Cos(gamma).Power(2)) / 12) + u * (b + r + ((r * (1 - Math.Cos(gamma))) / gamma)).Power(2)
                               + r.Power(3) * (((gamma - Math.Sin(gamma) * Math.Cos(gamma)) / 2) - ((1 - Math.Cos(gamma)).Power(2) / gamma));

            var Iy = 2 * t * (IyFirstTerm + IySecondTerm + IyThirdTerm + alpha * IyFourthTerm);


            var IxyFirstTerm = b * (aOver2 + r) * (bOver2 + r);
            var IxySecondTerm = 0.5 * r.Power(3);
            var IxyThirdTerm = 0.285 * a * r.Power(2);
            var IxyFourthTerm = c * (b + r * (1 + Math.Sin(gamma)) + cOver2 * Math.Cos(gamma)) * (aOver2 + r * Math.Cos(gamma) - cOver2 * Math.Sin(gamma))
                                - ((c.Power(3) * Math.Sin(gamma) * Math.Cos(gamma)) / 12) + r.Power(3) * ((Math.Sin(gamma).Power(2) / 2) + ((Math.Sin(gamma) * (Math.Cos(gamma) - 1)) / gamma))
                                + u * (b + r + (r * (1 - Math.Cos(gamma)) / gamma)) * (aOver2 + ((r * Math.Sin(gamma)) / gamma));

            var Ixy = 2 * t * (IxyFirstTerm + IxySecondTerm + IxyThirdTerm + alpha * IxyFourthTerm);

            var thetaPrime = (Math.PI / 2) + 0.5 * Math.Atan((2 * Ixy) / (Iy - Ix));

            var Ix2 = Ix * Math.Cos(thetaPrime).Power(2) + Iy * Math.Sin(thetaPrime).Power(2) - 2 * Ixy * Math.Sin(thetaPrime) * Math.Cos(thetaPrime);

            var Iy2 = Ix * Math.Sin(thetaPrime).Power(2) + Iy * Math.Cos(thetaPrime).Power(2) + 2 * Ixy * Math.Sin(thetaPrime) * Math.Cos(thetaPrime);

            var ix = Math.Sqrt(Ix / A);
            var iy = Math.Sqrt(Iy / A);

            var ixPrincipal = Math.Sqrt(Ix2 / A);
            var iyPrincipal = Math.Sqrt(Iy2 / A);

            var J = (t.Power(3) / 3) * (a + 2 * b + 2 * u + alpha * (2 * C + 2 * u));


            var CwNumLong = bPrime.Power(2) * (4 * cPrime.Power(4) + 16 * bPrime * cPrime.Power(3) + 6 * aPrime.Power(3) * cPrime + 4 * aPrime.Power(2) * bPrime * cPrime + 8 * aPrime * cPrime)
                            + 6 * aPrime * bPrime * cPrime.Power(2) * (aPrime + bPrime) * (2 * bPrime * Math.Sin(gamma) + aPrime * Math.Cos(gamma))
                            + 4 * aPrime * bPrime * cPrime.Power(3) * (2 * aPrime + 4 * bPrime + cPrime) * Math.Sin(gamma) * Math.Cos(gamma)
                            + Math.Cos(gamma).Power(2) * cPrime.Power(3) * (2 * aPrime.Power(3) + 4 * aPrime.Power(2) * bPrime - 8 * aPrime * bPrime.Power(2) + aPrime.Power(2) * cPrime - 16 * bPrime.Power(3) - 4 * bPrime.Power(2) * cPrime);
            var CwNum = 0.0;
            var CwDnum = aPrime + 2 * bPrime + 2 * alpha * cPrime;

            var Cw = (t / 12) * (CwNum / CwDnum);

            var errs = new List<Tuple<double, string>>()
            {
                Tuple.Create(aPrime,"a prime is less than zero"),
                Tuple.Create(bPrime," b prime is less tahn zero"),
                Tuple.Create(cPrime,"c prime is less than zero"),
                Tuple.Create(a,"a is less than zero"),
                Tuple.Create(b,"b is less than zero"),
                Tuple.Create(c,"c is less than zero")
            };
            var errors = errs.Where(err => err.Item1 < 0).Select(err => LessThanZeroError($"Cannot use this section because {err.Item2}")).ToList();
            if (errors.Count > 0)
                return Invalid(errors);
            else
            {
                var properties = new ZSectionProperties(aPrime, bPrime, cPrime, A, Ix, Zg, Iy, ix, iy, 0, J, Cw, c, r, u, b, alpha, a,Ix2,Iy2,Ixy,ixPrincipal,iyPrincipal,thetaPrime);
                var sec = new ZSection(secDim, properties);
                return sec;
            }
        }

    }
}
