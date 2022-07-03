using System.Collections.Generic;
using System.Linq;
using static ColdFormedChannelSection.Core.Comparers.Comparers;

namespace ColdFormedChannelSection.Core.Dtos
{
    public class EgyptCompressionCDto
    {
        
        #region Properties

        public LocalEgyptCompressionDto LB { get; }

        public FBEgyptCompressionDto FB { get;  }

        public TFBEgyptCompressionDto TFB { get;}

        public TBEgyptCompressionDto TB { get; }

        public NominalStrengthDto GoverningCase { get; set; }

        #endregion

        #region Constructors

        public EgyptCompressionCDto(LocalEgyptCompressionDto lB, FBEgyptCompressionDto fB, TFBEgyptCompressionDto tFB, TBEgyptCompressionDto tB)
        {
            LB = lB;
            FB = fB;
            TFB = tFB;
            TB = tB;
            var lst = new List<NominalStrengthDto>() { LB, FB, TFB, TB };
            GoverningCase = lst.Distinct(NominalStrengthEqualComparer).OrderBy(c => c.NominalStrength).First();
        }

        #endregion

    }
}
