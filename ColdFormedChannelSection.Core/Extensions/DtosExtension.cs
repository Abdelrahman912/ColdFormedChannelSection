using ColdFormedChannelSection.Core.Dtos;
using ColdFormedChannelSection.Core.Entities;

namespace ColdFormedChannelSection.Core.Extensions
{
    public static class DtosExtension
    {
        public static SectionDimension AsEntity(this SectionDimensionDto dto)
        {
            return new SectionDimension(
                totalHeightH: dto.H,
                totalFlangeWidthB: dto.B,
                totalFoldWidthC: dto.C,
                thicknessT: dto.T,
                internalRadiusR: dto.R
                );
        }
       
    }
}
