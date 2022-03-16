namespace ColdFormedChannelSection.Core.Entities
{
    public class LippedSectionDimension:SectionDimension
    {
        
        #region Properties

        public double TotalFoldWidthC { get; }


        #endregion

        #region Constructors

        public LippedSectionDimension(double totalHeightH, double totalFlangeWidthB, double internalRadius, double thicknessT,double totalFoldWidthC) 
            : base(totalHeightH, totalFlangeWidthB, internalRadius, thicknessT)
        {

            TotalFoldWidthC = totalFoldWidthC;

        }

        #endregion

    }
}
