﻿using ColdFormedChannelSection.Core.Enums;

namespace ColdFormedChannelSection.Core.Entities
{
    public class MomentDesignOutput : DesignOutput
    {
        public MomentDesignOutput(double ultimateLoad, string designSection, double nominalResistance, double phi,string factorName, FailureMode governingCase, Units unit, IReport report)
           : base(ultimateLoad, "Mu", designSection, nominalResistance, phi, governingCase, "Mn", factorName, unit, report)
        {
        }
        public MomentDesignOutput(double ultimateLoad, string designSection, double nominalResistance, double phi, FailureMode governingCase, Units unit,IReport report) 
            : base(ultimateLoad, "Mu", designSection, nominalResistance, phi, governingCase, "Mn", "(phi)b", unit,report)
        {
        }
    }
}
