using ColdFormedChannelSection.Core.Entities;
using ColdFormedChannelSection.Core.Enums;
using ColdFormedChannelSection.Core.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ColdFormedChannelSection.Core.Dtos
{
    public class ElementDSMomentDto : LocalDSMomentDto
    {
       
        #region Properties

        public double Kw { get; }

        public double Kf { get; }

        public double Kc { get; }

        #endregion

        #region Constructors

        public ElementDSMomentDto(double kw, double kf, double kc,double mcrl)
            :base(mcrl)
        {
            Kw = kw;
            Kf = kf;
            Kc = kc;
        }

        #endregion

        #region Methods

        public override ListReportSection AsReportSection(TypeOfSection sectionType)
        {
            switch (sectionType)
            {
                case TypeOfSection.UNSTIFFENED:
                    return ReportHelper.AsUnStiffenedReportSection(this);
                case TypeOfSection.LIPPED:
                    return ReportHelper.AsLippedReportSection(this);
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
        #endregion
    }
}
