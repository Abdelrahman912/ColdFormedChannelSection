namespace ColdFormedChannelSection.Core.Dtos
{
    public class DSUnStiffenedCompressionDto:DSCompressionDto
    {
        
        #region Contructors

        public DSUnStiffenedCompressionDto(LocalDSCompressionDto lb, double pcre, double pnl, double pne, double ag, double fy,NominalStrengthDto governingCase)
            :base(lb,pcre,pnl,pne,ag,fy,governingCase)
        {
        }

        #endregion
    }
}
