using ColdFormedChannelSection.Core.Enums;

namespace ColdFormedChannelSection.Core.Entities
{
    public class CompressionCheckOutput : CheckOutput
    {
     

        public CompressionCheckOutput(double ultimateLoad,  double nominalResistance, double phi,string phiName,string designResistanceName, FailureMode governingCase, Units unit, IReport report) 
            : base(ultimateLoad, "Pu", nominalResistance, phi, governingCase, "Pn", phiName,designResistanceName, unit,report)
        {

        }
    }
}
