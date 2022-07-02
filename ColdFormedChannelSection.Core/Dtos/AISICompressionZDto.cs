using System.Collections.Generic;
using System.Linq;
using static ColdFormedChannelSection.Core.Comparers.Comparers;

namespace ColdFormedChannelSection.Core.Dtos
{
    public class AISICompressionZDto
    {

        #region Properties

        public LocalAISICompressionDto LB { get; }

        public FBAISICompressionDto FB { get; }

        public TBAISICompressioDto TB { get; }

        public NominalStrengthDto GoverningCase { get; }

        #endregion

        #region Constructors

        public AISICompressionZDto(LocalAISICompressionDto lB, FBAISICompressionDto fB, TBAISICompressioDto tB)
        {
            LB = lB;
            FB = fB;
            TB = tB;
            var lst = new List<NominalStrengthDto>() { LB, FB, TB };
            GoverningCase = lst.Distinct(NominalStrengthEqualComparer).OrderBy(tuple => tuple.NominalStrength).First();
        }

        #endregion
    }
}
