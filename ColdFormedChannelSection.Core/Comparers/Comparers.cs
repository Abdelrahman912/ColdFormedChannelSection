using ColdFormedChannelSection.Core.Dtos;
using CSharpHelper.Comparers;
using CSharpHelper.Extensions;
using static ColdFormedChannelSection.Core.Constants;

namespace ColdFormedChannelSection.Core.Comparers
{
    public static class Comparers
    {
        public static GenericComparer<NominalStrengthDto> NominalStrengthEqualComparer =>
            GenericComparer.Create<NominalStrengthDto>((t1, t2) => t1.NominalStrength.IsEqual(t2.NominalStrength, TOL), (t) =>
            {
                var prime1 = 17;
                var prime2 = 23;
                unchecked
                {
                    var hash = prime1 * prime2 * t.NominalStrength.GetIntHash();
                    return hash;
                }
            });
    }
}
