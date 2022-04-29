using ColdFormedChannelSection.Core.Enums;

namespace ColdFormedChannelSection.Core.Entities.Output.Design
{
    public abstract class DesignOutput : ResistanceOutput
    {

        #region Properties

        public double UltimateLoad { get; }

        public string UltimateLoadName { get; }

        public string DesignSection { get; }

        #endregion

        #region Constructors

        protected DesignOutput(double ultimateLoad , string ultimateLoadName , string designSection,double nominalResistance, double phi, FailureMode governingCase, string nominalResistanceName, string phiName, string unitName) 
            : base(nominalResistance, phi, governingCase, nominalResistanceName, phiName, unitName)
        {
            UltimateLoad = ultimateLoad;
            UltimateLoadName = ultimateLoadName;
            DesignSection = designSection;
        }

        #endregion


    }
}
