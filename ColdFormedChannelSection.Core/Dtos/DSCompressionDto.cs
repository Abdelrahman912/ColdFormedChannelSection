namespace ColdFormedChannelSection.Core.Dtos
{
    public abstract class DSCompressionDto
    {
        
        #region Properties

        public LocalDSCompressionDto LB { get; }

        public double Pcre { get; }

        public double Pnl { get; }

        public double Pne { get; }

        public double Ag { get; }

        public double Fy { get; }

        public double Py { get; }

        public NominalStrengthDto GoverningCase { get; }

        #endregion

        #region Contructors

        public DSCompressionDto(LocalDSCompressionDto lb,  double pcre, double pnl,  double pne, double ag, double fy,NominalStrengthDto governingCase)
        {
            LB = lb;
            Pcre = pcre;
            Pnl = pnl;
            Pne = pne;
            Ag = ag;
            Fy = fy;
            Py = Fy*Ag;
            GoverningCase = governingCase;
        }

        #endregion
    }
}
