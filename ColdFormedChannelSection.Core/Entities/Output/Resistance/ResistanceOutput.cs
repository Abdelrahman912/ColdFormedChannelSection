using ColdFormedChannelSection.Core.Enums;

namespace ColdFormedChannelSection.Core.Entities
{
    public abstract class ResistanceOutput
    {
       

        #region properties

        public double NominalResistance { get;}

        public double Phi { get;}

        public double DesignResistance => Phi * NominalResistance;

        public string NominalResistanceName { get; }

        public string PhiName { get; set; }

        public string DesignResistanceName => $"{PhiName} * {NominalResistanceName}";

        public FailureMode GoverningCase { get; }

        #endregion

        #region Constructors

        protected ResistanceOutput(double nominalResistance, double phi,FailureMode governingCase, string nominalResistanceName, string phiName)
        {
            NominalResistance = nominalResistance;
            Phi = phi;
            NominalResistanceName = nominalResistanceName;
            PhiName = phiName;
            GoverningCase = governingCase;
        }

        #endregion
    }
}
