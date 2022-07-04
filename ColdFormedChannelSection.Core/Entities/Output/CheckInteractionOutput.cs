using ColdFormedChannelSection.Core.Enums;
using CSharpHelper.Extensions;
using System.Collections.Generic;

namespace ColdFormedChannelSection.Core.Entities
{
    public class CheckInteractionOutput:ResistanceInteractionOutput
    {
       

        #region Properties

        public CheckResultStatus Status { get; }

        public string CheckResultName { get; }

        #endregion

        #region Constructors

        public CheckInteractionOutput(double pu, double pn, double mu, double mn, string iE, double iEValue,Units momentUnit, Units forceUnit,IReport report)
            :base(pu,pn,mu,mn,iE,iEValue,momentUnit.GetDescription(),forceUnit.GetDescription(),report)
        {
           
            if(IEValue <= 1)
            {
                Status = CheckResultStatus.SAFE;
                CheckResultName = $"{IE} = {IEValue.ToString("0.###")} < 1.0";
            }
            else
            {
                Status = CheckResultStatus.UNSAFE;
                CheckResultName = $"{IE}  = {IEValue.ToString("0.###")} > 1.0";
            }
            var checkItems = new List<ReportItem>()
            {
                new ReportItem("Pu",pu.ToString("0.###"),forceUnit),
                new ReportItem("Mu",mu.ToString("0.###"),momentUnit),
                new ReportItem("Interaction Equation Value",IEValue.ToString("0.###"),Units.NONE),
                new ReportItem("Status",Status.GetDescription(),Units.NONE)
            };
            var section = new ListReportSection("Check Results", checkItems, false);
            Report.Sections.Add(section);
        }

        #endregion
    }
}
