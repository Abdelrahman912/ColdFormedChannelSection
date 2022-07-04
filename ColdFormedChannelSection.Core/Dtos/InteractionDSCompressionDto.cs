using ColdFormedChannelSection.Core.Entities;
using ColdFormedChannelSection.Core.Enums;
using ColdFormedChannelSection.Core.Helpers;
using System;

namespace ColdFormedChannelSection.Core.Dtos
{
    public class InteractionDSCompressionDto : LocalDSCompressionDto
    {
        

        #region Properties

        public double KFlangeLip { get; }

        public double KFlangeWeb { get; }

        #endregion

        #region Constructors

        public InteractionDSCompressionDto(double kFlangeLip, double kFlangeWeb,double pcrl)
            :base(pcrl)
        {
            KFlangeLip = kFlangeLip;
            KFlangeWeb = kFlangeWeb;
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
