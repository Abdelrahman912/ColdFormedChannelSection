using ColdFormedChannelSection.Core.Enums;

namespace ColdFormedChannelSection.Core.Entities
{
    public abstract class CheckOutput : ResistanceOutput
    {

        #region Properties

        public double UltimateLoad { get; }

        public string UltimateLoadName { get; }

        public CheckResultStatus Status { get; }

        public string CheckResultName { get; }

        #endregion

        #region Constructors

        protected CheckOutput(double ultimateLoad , string ultimateLoadName ,double nominalResistance, double phi, FailureMode governingCase, string nominalResistanceName, string phiName, string unitName,IReport report)
           : base(nominalResistance, phi, governingCase, nominalResistanceName, phiName, unitName,report)
        {
            UltimateLoad = ultimateLoad;
            UltimateLoadName = ultimateLoadName;
            if( UltimateLoad > DesignResistance)
            {
                Status = CheckResultStatus.UNSAFE;
                CheckResultName = $"{UltimateLoadName} > {DesignResistanceName}";
            }
            else
            {
                Status = CheckResultStatus.SAFE;
                CheckResultName = $"{UltimateLoadName} < {DesignResistanceName}";
            }
        }

        #endregion


    }
}
