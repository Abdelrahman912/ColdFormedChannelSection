using ColdFormedChannelSection.Core.Enums;
using CSharpHelper.Comparers;
using CSharpHelper.Extensions;
using System;
using static ColdFormedChannelSection.Core.Constants;

namespace ColdFormedChannelSection.Core.Comparers
{
    public static class Comparers
    {
        public static GenericComparer<Tuple<double, FailureMode>> NominalStrengthEqualComparer =>
            GenericComparer.Create<Tuple<double, FailureMode>>((t1, t2) => t1.Item1.IsEqual(t2.Item1, TOL), (t) =>
            {
                var prime1 = 17;
                var prime2 = 23;
                unchecked
                {
                    var hash = prime1 * prime2 * t.Item1.GetIntHash();
                    return hash;
                }
            });
    }
}
