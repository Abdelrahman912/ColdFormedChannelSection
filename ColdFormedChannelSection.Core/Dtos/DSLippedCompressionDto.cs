namespace ColdFormedChannelSection.Core.Dtos
{
    public class DSLippedCompressionDto:DSCompressionDto
    {
        
        #region Properties

        public double Pcrd { get; }

        public double Pnd { get; }

        #endregion

        #region Contructors

        public DSLippedCompressionDto(LocalDSCompressionDto lb, double pcrd, double pcre, double pnl, double pnd, double pne, double ag, double fy,NominalStrengthDto governingCase)
            :base(lb,pcre,pnl,pne,ag,fy,governingCase)
        {
            Pcrd = pcrd;
            Pnd = pnd;
        }

        #endregion
    }
}
