using System.Collections.Generic;
using System.Linq;
using static ColdFormedChannelSection.Core.Comparers.Comparers;

namespace ColdFormedChannelSection.Core.Dtos
{
    public class AISIMomentDto
    {
       
        #region Properties

        public LocalAISIMomentDto LB { get;}

        public  LTBAISIMomentDto LTB { get; }

        public  NominalStrengthDto GoverningCase { get; }

        #endregion

        #region Constructors

        public AISIMomentDto(LocalAISIMomentDto lB, LTBAISIMomentDto lTB)
        {
            LB = lB;
            LTB = lTB;
            var lst = new List<NominalStrengthDto>() { lB, LTB };
            GoverningCase = lst.Distinct(NominalStrengthEqualComparer).OrderBy(tuple => tuple.NominalStrength).First();
        }

        #endregion
    }
}
