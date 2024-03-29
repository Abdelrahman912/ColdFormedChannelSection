﻿using ColdFormedChannelSection.Core.Enums;

namespace ColdFormedChannelSection.Core.Entities
{
    public class MomentCheckOutput : CheckOutput
    {

        public MomentCheckOutput(double ultimateLoad, double nominalResistance, double phi,string phiName, string designResistanceName, FailureMode governingCase, Units unit, IReport report) 
            : base(ultimateLoad, "Mu", nominalResistance, phi, governingCase, "Mn", phiName,designResistanceName, unit,report)
        {
        }
    }
}
