using ColdFormedChannelSection.Core.Enums;
using ColdFormedChannelSection.Core.Extensions;

namespace ColdFormedChannelSection.Core.Entities
{
    public class CompressionDesignOutput : DesignOutput
    {
        public CompressionDesignOutput(double ultimateLoad, string designSection, double nominalResistance, double phi,string factorName, FailureMode governingCase, Units unit, IReport report)
           : base(ultimateLoad, "Pu", designSection, nominalResistance, phi, governingCase, "Pn", factorName, unit, report)
        {
        }
        public CompressionDesignOutput(double ultimateLoad , string designSection, double nominalResistance, double phi, FailureMode governingCase, Units unit,IReport report) 
            : base(ultimateLoad, "Pu", designSection, nominalResistance, phi, governingCase, "Pn", "(phi)c", unit,report)
        {
        }
    }
}
