using ColdFormedChannelSection.Core.Enums;
using CSharpHelper.Extensions;
using System.Collections.Generic;

namespace ColdFormedChannelSection.Core.Entities
{
    public abstract class DesignOutput : ResistanceOutput
    {

        #region Properties

        public double UltimateLoad { get; }

        public string UltimateLoadName { get; }

        public string DesignSection { get; }

        #endregion

        #region Constructors

        protected DesignOutput(double ultimateLoad , string ultimateLoadName , string designSection,double nominalResistance, double phi, FailureMode governingCase, string nominalResistanceName, string phiName,string designResistanceName, Units unit,IReport report) 
            : base(nominalResistance, phi, governingCase, nominalResistanceName, phiName,designResistanceName, unit.GetDescription(),report)
        {
            UltimateLoad = ultimateLoad;
            UltimateLoadName = ultimateLoadName;
            DesignSection = designSection;
            var designItems = new List<ReportItem>()
            {
                new ReportItem(ultimateLoadName,ultimateLoad.ToString("0.###"),unit),
                new ReportItem(DesignResistanceName,DesignResistance.ToString("0.###"),unit),
                new ReportItem("Governing Case",governingCase.ToString(),Units.NONE),
                new ReportItem("Section",DesignSection,Units.NONE)
            };
            var section = new ListReportSection("Design Results", designItems, false);
            Report?.Sections.Add(section);
        }

        #endregion


    }
}
