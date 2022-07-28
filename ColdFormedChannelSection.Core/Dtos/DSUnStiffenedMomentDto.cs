namespace ColdFormedChannelSection.Core.Dtos
{
    public class DSUnStiffenedMomentDto:DSMomentDto
    {
       
        

        #region Constructors

        public DSUnStiffenedMomentDto(LocalDSMomentDto lb, double mcre, double mnl,  double mne,double fy,double zg,NominalStrengthDto governingCase)
            :base(lb,mcre,mnl,mne,fy,zg,governingCase)
        {
           
        }

        #endregion
    }
}
