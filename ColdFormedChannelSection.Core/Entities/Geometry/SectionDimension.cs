namespace ColdFormedChannelSection.Core.Entities
{
    public abstract class SectionDimension
    {
        

        #region Properties

        public double TotalHeightH { get; }

        public double TotalFlangeWidthB { get; }

        public double InternalRadiusR { get; }

        public double ThicknessT { get; }


        #endregion

        #region Constructors

        public SectionDimension(double totalHeightH, double totalFlangeWidthB, double internalRadius, double thicknessT)
        {
            TotalHeightH = totalHeightH;
            TotalFlangeWidthB = totalFlangeWidthB;
            InternalRadiusR = internalRadius;
            ThicknessT = thicknessT;
            
        }

        #endregion

    }
}
