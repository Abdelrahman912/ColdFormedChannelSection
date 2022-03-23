using ColdFormedChannelSection.Core.Entities;
using ColdFormedChannelSection.Core.Enums;
using ColdFormedChannelSection.Core.Extensions;
using System;

namespace ColdFormedChannelSection.Core.Helpers
{
    public static class SectionPropertiesHelper
    {

        public static LippedSection AsLippedSection(this SectionDimension sectionDim)
        {
          var sec = sectionDim.CaclulateSectionProperties(TypeOfChannel.LIPPED);
            return new LippedSection(sec.Dimensions, sec.Properties);
        }


        public static UnStiffenedSection AsUnStiffenedSection(this SectionDimension sectionDim)
        {
           var sec =  sectionDim.CaclulateSectionProperties(TypeOfChannel.UNSTIFFENED);
            return new UnStiffenedSection(sec.Dimensions, sec.Properties);
        }

        private static Section CaclulateSectionProperties(this SectionDimension sectionDim , TypeOfChannel channel)
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
            var cPrime = alpha * (C-tover2);
            var u =Math.PI * (r / 2);
            var A = t * (a + 2 * b + 2 * u + alpha * (2 * c + 2 * u));
            var Ix = 2 * t * (0.0417 * a.Power(3) + b * (aOver2 + r).Power(2) + 0.149 * r.Power(3) + alpha * (0.0833 * c.Power(3)+(c/4)*(a-c).Power(2)+0.149*r.Power(3)+u*(aOver2+0.637*r).Power(2)) + u * (aOver2 + 0.637 * r).Power(2));

            var hOver2 = H / 2;
            var Zg = Ix / hOver2;
            var bOver2 = b / 2;
            var XcPrime = ((2*t) / (A)) * (b*(bOver2+r)+u*(0.363*r)+alpha*(u*(b+1.637*r)+c*(b+2*r)));

            var Iy = 2 * t * (b*(bOver2+r).Power(2)+b.Power(3)/12 + 0.356*r.Power(3)+alpha*(c*(b+2*r).Power(2)+u*(b+1.637*r).Power(2)+0.149*r.Power(3)))-A*XcPrime.Power(2);

            var ix = Math.Sqrt(Ix / A);
            var iy = Math.Sqrt(Iy/A);

            var mNumenator = 3 * aPrime.Power(2) * bPrime + alpha * cPrime * (6*aPrime.Power(2)-8*cPrime.Power(2));
            var mDnumenator = aPrime.Power(3) + 6 * aPrime.Power(2) * bPrime + alpha * cPrime * (8*cPrime.Power(2)-12*aPrime*cPrime+6*aPrime.Power(2));

            var m = bPrime * (mNumenator / mDnumenator);

            var Xo = XcPrime + m;
            var J = ((t.Power(3))/3) * (a+2*b+2*u+alpha*(2*c+2*u));
            var CwNumenator = 2 * aPrime.Power(3) * bPrime + 3 * aPrime.Power(2) * bPrime.Power(2) + alpha * (48 * cPrime.Power(4) + 112 * bPrime * cPrime.Power(3) + 8 * aPrime * cPrime.Power(3) + 48 * aPrime * bPrime * cPrime.Power(2) + 12 * aPrime.Power(2) * cPrime.Power(2) + 12 * aPrime.Power(2) * bPrime * cPrime + 6 * aPrime.Power(3) * cPrime);
            var CwDunemenator = 6 * aPrime.Power(2) * bPrime + (aPrime + alpha * 2 * cPrime).Power(3) - alpha * 24 * aPrime * cPrime.Power(2);
            var Cw = ((aPrime.Power(2)*bPrime.Power(2)*t)/12) * (CwNumenator / CwDunemenator) ;
            var properties = new SectionProperties(aPrime,bPrime,cPrime,A,Ix,Zg,Iy,ix,iy,Xo,J,Cw,c,a);
            var sec =   new Section(sectionDim,properties);
            return sec;
        }

    }
}
