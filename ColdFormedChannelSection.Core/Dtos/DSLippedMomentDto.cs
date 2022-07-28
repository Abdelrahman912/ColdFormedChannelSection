namespace ColdFormedChannelSection.Core.Dtos
{
    public class DSLippedMomentDto:DSMomentDto
    {
       
        #region Properties

       

        public double Mcrd { get; }

      
        public double Mnd { get; }

        #endregion

        #region Constructors

        public DSLippedMomentDto(LocalDSMomentDto lb, double mcre, double mcrd, double mnl, double mnd, double mne,double fy,double zg,NominalStrengthDto governingCase)
            :base(lb,mcre,mnl,mne,fy,zg,governingCase)
        {
           
            Mcrd = mcrd;
            
            Mnd = mnd;
        }

        #endregion
    }
}
