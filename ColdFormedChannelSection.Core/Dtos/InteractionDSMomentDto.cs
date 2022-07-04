using ColdFormedChannelSection.Core.Entities;
using ColdFormedChannelSection.Core.Enums;
using ColdFormedChannelSection.Core.Helpers;
using System;

namespace ColdFormedChannelSection.Core.Dtos
{
    public class InteractionDSMomentDto:LocalDSMomentDto
    {

        #region Properties

        public double KFlangeWeb { get; }

        public double KFlangeLip { get; }

        #endregion

        #region Constructors

        public InteractionDSMomentDto(double kFlangeWeb, double kFlangeLip,double mcrl)
            :base(mcrl)
        {
            KFlangeWeb = kFlangeWeb;
            KFlangeLip = kFlangeLip;
        }

        #endregion

        #region Methods

        public override ListReportSection AsReportSection(TypeOfSection _)
        {
            return ReportHelper.AsReportSection(this);
        }

        #endregion
    }
}
