using System;

namespace ColdFormedChannelSection.Core.Extensions
{
    public static class DoubleExtension
    {
        public static double Power(this double num , double power)=>
            Math.Pow(num, power);

        public static double Round(this double num , int digits) =>
            Math.Round(num, digits);
    }
}
