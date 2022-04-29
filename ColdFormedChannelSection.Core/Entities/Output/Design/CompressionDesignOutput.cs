using ColdFormedChannelSection.Core.Enums;

namespace ColdFormedChannelSection.Core.Entities.Output.Design
{
    public class CompressionDesignOutput : DesignOutput
    {
        public CompressionDesignOutput(double ultimateLoad , string designSection, double nominalResistance, double phi, FailureMode governingCase, string unitName) 
            : base(ultimateLoad, "Pu", designSection, nominalResistance, phi, governingCase, "Pn", "(phi)c", unitName)
        {
        }
    }
}
