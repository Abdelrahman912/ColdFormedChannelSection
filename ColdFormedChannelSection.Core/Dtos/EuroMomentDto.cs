using System.Collections.Generic;
using System.Linq;
using static ColdFormedChannelSection.Core.Comparers.Comparers;

namespace ColdFormedChannelSection.Core.Dtos
{
    public class EuroMomentDto
    {
       
        #region Properties

        public LocalEuroMomentDto LB { get;}

        public LTBEuroMomentDto LTB { get;}

        public NominalStrengthDto GoverningCase { get; }

        #endregion

        #region Constructors

        public EuroMomentDto(LocalEuroMomentDto lB, LTBEuroMomentDto lTB)
        {
            LB = lB;
            LTB = lTB;
            var lst = new List<NominalStrengthDto>() { lB, lTB };
            GoverningCase = lst.Distinct(NominalStrengthEqualComparer).OrderBy(tuple => tuple.NominalStrength).First();
        }

        #endregion

    }
}
