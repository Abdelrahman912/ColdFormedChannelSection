using ColdFormedChannelSection.Core.Entities;
using ColdFormedChannelSection.Core.Enums;
using ColdFormedChannelSection.Core.Helpers;
using System;

namespace ColdFormedChannelSection.Core.Dtos
{
    public class ElementDSCompressionDto : LocalDSCompressionDto
    {


        #region Properties

        public double Kw { get; }

        public double Kf { get; }

        public double Kc { get; }

        #endregion

        #region Constructors

        public ElementDSCompressionDto(double kw, double kf, double kc, double pcrl)
            : base(pcrl)
        {
            Kw = kw;
            Kf = kf;
            Kc = kc;
        }

        #endregion

        #region Methods

        public override ListReportSection AsReportSection(TypeOfSection section)
        {
            switch (section)
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
