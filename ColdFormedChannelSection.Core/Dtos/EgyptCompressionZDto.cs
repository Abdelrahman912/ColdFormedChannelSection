using System.Collections.Generic;
using System.Linq;
using static ColdFormedChannelSection.Core.Comparers.Comparers;

namespace ColdFormedChannelSection.Core.Dtos
{
    public class EgyptCompressionZDto
    {
        #region Properties

        public LocalEgyptCompressionDto LB { get; }

        public FBEgyptCompressionDto FB { get; }

        public TBEgyptCompressionDto TB { get; }

        public NominalStrengthDto GoverningCase { get; set; }

        #endregion

        #region Constructors

        public EgyptCompressionZDto(LocalEgyptCompressionDto lB, FBEgyptCompressionDto fB, TBEgyptCompressionDto tB)
        {
            LB = lB;
            FB = fB;
            TB = tB;
            var lst = new List<NominalStrengthDto>() { LB, FB, TB };
            GoverningCase = lst.Distinct(NominalStrengthEqualComparer).OrderBy(c => c.NominalStrength).First();
        }

        #endregion
    }
}
