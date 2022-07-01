using System.Collections.Generic;
using System.Linq;
using static ColdFormedChannelSection.Core.Comparers.Comparers;

namespace ColdFormedChannelSection.Core.Dtos
{
    public class AISICompressionDto
    {
       
        #region Properties

        public LocalAISICompressionDto LB { get; }

        public FBAISICompressionDto FB { get; }

        public FTBAISICompressionDto FTB { get; }

        public TBAISICompressioDto TB { get; }

        public NominalStrengthDto GoverningCase { get; }

        #endregion

        #region Constructors

        public AISICompressionDto(LocalAISICompressionDto lB, FBAISICompressionDto fB, FTBAISICompressionDto fTB, TBAISICompressioDto tB)
        {
            LB = lB;
            FB = fB;
            FTB = fTB;
            TB = tB;
            var lst = new List<NominalStrengthDto>() { LB, FB, FTB, TB };
            GoverningCase = lst.Distinct(NominalStrengthEqualComparer).OrderBy(tuple => tuple.NominalStrength).First();
        }

        #endregion

    }
}
