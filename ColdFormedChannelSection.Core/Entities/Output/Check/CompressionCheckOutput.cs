using ColdFormedChannelSection.Core.Enums;
using ColdFormedChannelSection.Core.Extensions;

namespace ColdFormedChannelSection.Core.Entities
{
    public class CompressionCheckOutput : CheckOutput
    {
        public CompressionCheckOutput(double ultimateLoad,  double nominalResistance, double phi, FailureMode governingCase, Units unit, IReport report) 
            : base(ultimateLoad, "Pu", nominalResistance, phi, governingCase, "Pn", "(phi)c", unit,report)
        {

        }
    }
}
