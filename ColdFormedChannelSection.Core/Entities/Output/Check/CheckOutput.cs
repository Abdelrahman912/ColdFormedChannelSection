using ColdFormedChannelSection.Core.Enums;
using ColdFormedChannelSection.Core.Extensions;
using CSharpHelper.Extensions;
using System.Collections.Generic;

namespace ColdFormedChannelSection.Core.Entities
{
    public abstract class CheckOutput : ResistanceOutput
    {

        #region Properties

        public Units Unit { get;  }

        public double UltimateLoad { get; }

        public string UltimateLoadName { get; }

        public CheckResultStatus Status { get; }

        public string CheckResultName { get; }

        #endregion

        #region Constructors

        protected CheckOutput(double ultimateLoad , string ultimateLoadName ,double nominalResistance, double phi, FailureMode governingCase, string nominalResistanceName, string phiName,string designResistanceName, Units unit, IReport report)
           : base(nominalResistance, phi, governingCase, nominalResistanceName, phiName,designResistanceName, unit.GetDescription(),report)
        {
            Unit = unit;
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
            var checkItems = new List<ReportItem>()
            {
                new ReportItem(ultimateLoadName,ultimateLoad.ToString("0.###"),Unit),
                new ReportItem(DesignResistanceName,DesignResistance.ToString("0.###"),Unit),
                new ReportItem("Status",Status.GetDescription(),Units.NONE),
                new ReportItem("Governing Case",governingCase.GetDescription(),Units.NONE),
            };
            var section = new ListReportSection("Check Results",checkItems,false);
            Report?.Sections.Add(section);
        }

        #endregion


    }
}
