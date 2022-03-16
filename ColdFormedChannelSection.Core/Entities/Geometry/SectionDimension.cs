namespace ColdFormedChannelSection.Core.Entities
{
    public  class SectionDimension
    {
       


        #region Properties

        public double TotalHeightH { get; }

        public double TotalFlangeWidthB { get; }

        public double InternalRadiusR { get; }

        public double ThicknessT { get; }

        public double TotalFoldWidthC { get; }


        #endregion


        #region Constructors

        public SectionDimension(double totalHeightH, double totalFlangeWidthB, double internalRadiusR, double thicknessT, double totalFoldWidthC)
        {
            TotalHeightH = totalHeightH;
            TotalFlangeWidthB = totalFlangeWidthB;
            InternalRadiusR = internalRadiusR;
            ThicknessT = thicknessT;
            TotalFoldWidthC = totalFoldWidthC;
        }

        #endregion


    }
}
