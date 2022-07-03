using System.Collections.Generic;
using System.Linq;
using static ColdFormedChannelSection.Core.Comparers.Comparers;

namespace ColdFormedChannelSection.Core.Dtos
{
    public class EuroCompressionZDto
    {
        #region Properties

        public LocalEuroCompressionDto LB { get; }

        public FBEuroCompressionDto FB { get; }

        public TBEuroCompressionDto TB { get; }

        public NominalStrengthDto GoverningCase { get; }

        #endregion

        #region Constructors

        public EuroCompressionZDto(LocalEuroCompressionDto lB, FBEuroCompressionDto fB, TBEuroCompressionDto tB)
        {
            LB = lB;
            FB = fB;
            TB = tB;
            var nominalLoads = new List<NominalStrengthDto>() { LB, FB, TB };
            GoverningCase = nominalLoads.Distinct(NominalStrengthEqualComparer).OrderBy(tup => tup.NominalStrength).First();
        }

        #endregion
    }
}
