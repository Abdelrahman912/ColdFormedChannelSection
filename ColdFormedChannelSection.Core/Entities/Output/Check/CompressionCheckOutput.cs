using ColdFormedChannelSection.Core.Enums;

namespace ColdFormedChannelSection.Core.Entities
{
    public class CompressionCheckOutput : CheckOutput
    {
        public CompressionCheckOutput(double ultimateLoad,  double nominalResistance, double phi, FailureMode governingCase, string unitName) 
            : base(ultimateLoad, "Pu", nominalResistance, phi, governingCase, "Pn", "(phi)c", unitName)
        {

        }
    }
}
