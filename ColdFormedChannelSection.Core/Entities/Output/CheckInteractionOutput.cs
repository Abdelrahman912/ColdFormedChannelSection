using ColdFormedChannelSection.Core.Enums;

namespace ColdFormedChannelSection.Core.Entities
{
    public class CheckInteractionOutput:ResistanceInteractionOutput
    {
       

        #region Properties

        public CheckResultStatus Status { get; }

        public string CheckResultName { get; }

        #endregion

        #region Constructors

        public CheckInteractionOutput(double pu, double pn, double mu, double mn, string iE, double iEValue,string momentUnitName, string forceUnitName)
            :base(pu,pn,mu,mn,iE,iEValue,momentUnitName,forceUnitName)
        {
           
            if(IEValue <= 1)
            {
                Status = CheckResultStatus.SAFE;
                CheckResultName = $"{IE} = {IEValue.ToString("0.##")} < 1.0";
            }
            else
            {
                Status = CheckResultStatus.UNSAFE;
                CheckResultName = $"{IE}  = {IEValue.ToString("0.##")} > 1.0";
            }
        }

        #endregion
    }
}
