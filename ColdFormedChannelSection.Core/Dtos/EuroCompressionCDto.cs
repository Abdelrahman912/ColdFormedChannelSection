using System.Collections.Generic;
using System.Linq;
using static ColdFormedChannelSection.Core.Comparers.Comparers;

namespace ColdFormedChannelSection.Core.Dtos
{
    public class EuroCompressionCDto
    {
       
        #region Properties

        public LocalEuroCompressionDto LB { get; }

        public FBEuroCompressionDto FB { get; }

        public TBEuroCompressionDto TB { get;  }

        public FTBEuroCompressionDto FTB { get;}

        public NominalStrengthDto GoverningCase { get;}

        #endregion

        #region Constructors

        public EuroCompressionCDto(LocalEuroCompressionDto lB, FBEuroCompressionDto fB, TBEuroCompressionDto tB, FTBEuroCompressionDto fTB)
        {
            LB = lB;
            FB = fB;
            TB = tB;
            FTB = fTB;
            var nominalLoads = new List<NominalStrengthDto>() { LB,FB, TB ,FTB};
            GoverningCase = nominalLoads.Distinct(NominalStrengthEqualComparer).OrderBy(tup => tup.NominalStrength).First();
        }

        #endregion
    }
}
