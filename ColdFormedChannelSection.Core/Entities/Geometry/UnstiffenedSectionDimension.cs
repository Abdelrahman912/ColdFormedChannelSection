using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ColdFormedChannelSection.Core.Entities
{
    public class UnstiffenedSectionDimension : SectionDimension
    {
        public UnstiffenedSectionDimension(double totalHeightH, double totalFlangeWidthB, double internalRadius, double thicknessT) 
            : base(totalHeightH, totalFlangeWidthB, internalRadius, thicknessT)
        {
        }
    }
}
