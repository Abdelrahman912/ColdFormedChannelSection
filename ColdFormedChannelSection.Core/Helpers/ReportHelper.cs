using ColdFormedChannelSection.Core.Dtos;
using ColdFormedChannelSection.Core.Entities;
using ColdFormedChannelSection.Core.Enums;
using ColdFormedChannelSection.Core.Extensions;
using System.Collections.Generic;
using static ColdFormedChannelSection.Core.Constants;

namespace ColdFormedChannelSection.Core.Helpers
{
    public static class ReportHelper
    {

        public static Report AsReport(this AISIMomentDto dto, LippedCSection section)
        {
            var dimSection = section.Dimensions.AsLippedReportSection();
            var lbSection = dto.LB.AsLippedReportSection();
            var ltbSection = dto.LTB.AsReportSection();
            var items = new List<ReportItem>()
            {
                new ReportItem("Governing Case",dto.GoverningCase.FailureMode.GetDescription(),Units.NONE),
                new ReportItem("Nominal Moment (Mn)",dto.GoverningCase.NominalStrength.ToString("0.###"),Units.KIP_IN),
                new ReportItem("phi",PHI_B_AISI.ToString("0.###"),Units.NONE),
                new ReportItem("Design Moment (phi * Mn)",(PHI_B_AISI*dto.GoverningCase.NominalStrength).ToString("0.###"),Units.KIP_IN)
            };
            var designSection = new ListReportSection("Design Moment", items);

            var sections = new List<IReportSection>() { dimSection, lbSection, ltbSection, designSection };

            var report = new Report(UnitSystems.KIPINCH, "AISI Code - Moment", sections);
            return report;
        }

        public static Report AsReport(this AISIMomentDto dto, UnStiffenedCSection section)
        {
            var dimSection = section.Dimensions.AsUnStiffenedReportSection();
            var lbSection = dto.LB.AsUnStiffenedReportSection();
            var ltbSection = dto.LTB.AsReportSection();
            var items = new List<ReportItem>()
            {
                new ReportItem("Governing Case",dto.GoverningCase.FailureMode.GetDescription(),Units.NONE),
                new ReportItem("Nominal Moment (Mn)",dto.GoverningCase.NominalStrength.ToString("0.###"),Units.KIP_IN),
                new ReportItem("phi",PHI_B_AISI.ToString("0.###"),Units.NONE),
                new ReportItem("Design Moment (phi * Mn)",(PHI_B_AISI*dto.GoverningCase.NominalStrength).ToString("0.###"),Units.KIP_IN)
            };
            var designSection = new ListReportSection("Design Moment", items);

            var sections = new List<IReportSection>() { dimSection, lbSection, ltbSection, designSection };

            var report = new Report(UnitSystems.KIPINCH, "AISI Code - Moment", sections);
            return report;
        }

        public static ListReportSection AsLippedReportSection(this SectionDimension dims)
        {
            var items = new List<ReportItem>()
            {
                new ReportItem("H",dims.TotalHeightH.ToString("0.###"),Units.IN),
                new ReportItem("B",dims.TotalFlangeWidthB.ToString("0.###"),Units.IN),
                new ReportItem("R",dims.InternalRadiusR.ToString("0.###"),Units.IN),
                new ReportItem("t",dims.ThicknessT.ToString("0.###"),Units.IN),
                new ReportItem("C",dims.TotalFoldWidthC.ToString("0.###"),Units.IN)
            };
            var section = new ListReportSection("Section Dimensions", items);
            return section;
        }

        public static ListReportSection AsUnStiffenedReportSection(this SectionDimension dims)
        {
            var items = new List<ReportItem>()
            {
                new ReportItem("H",dims.TotalHeightH.ToString("0.###"),Units.IN),
                new ReportItem("B",dims.TotalFlangeWidthB.ToString("0.###"),Units.IN),
                new ReportItem("R",dims.InternalRadiusR.ToString("0.###"),Units.IN),
                new ReportItem("t",dims.ThicknessT.ToString("0.###"),Units.IN),
            };
            var section = new ListReportSection("Section Dimensions", items);
            return section;
        }

        public static ListReportSection AsReportSection(this LTBAISIMomentDto dto)
        {
            var items = new List<ReportItem>()
            {
                 new ReportItem("Lateral Torsional Section Modulus (Zf)",dto.Zf.ToString("0.###"),Units.IN_3),
                new ReportItem("Lateral Torsional Stress (F)",dto.F.ToString("0.###"),Units.KSI),
                 new ReportItem("Lateral Torsional Nominal Moment (Mn)",dto.NominalStrength.ToString("0.###"),Units.KIP_IN),
            };
            return new ListReportSection("Lateral Torsional Buckling", items);
        }

        public static ListReportSection AsLippedReportSection(this LocalAISIMomentDto dto)
        {
            var items = new List<ReportItem>()
            {
                new ReportItem("Kw",dto.Kw.ToString("0.###"),Units.NONE),
                new ReportItem("Effective Height (ae)",dto.Ae.ToString("0.###"),Units.IN),
                new ReportItem("Kc", dto.Kc.ToString("0.###"), Units.NONE),
                new ReportItem("Effective Lip (ce)", dto.Ce.ToString("0.###"), Units.IN),
                new ReportItem("Kf", dto.Kf.ToString("0.###"), Units.NONE),
                new ReportItem("Effective Flange Width (be)", dto.Be.ToString("0.###"), Units.IN),
                new ReportItem("Effective Section Modulus (Ze)", dto.Ze.ToString("0.###"), Units.IN_3),
                new ReportItem("Yield stress (Fy)", dto.Fy.ToString("0.###"), Units.KSI),
                new ReportItem("Local Nominal Moment (Mn)", dto.NominalStrength.ToString("0.###"), Units.KIP_IN)
            };
            return new ListReportSection("Local Buckling", items);
        }

        public static ListReportSection AsUnStiffenedReportSection(this LocalAISIMomentDto dto)
        {
            var items = new List<ReportItem>()
            {
                new ReportItem("Kw",dto.Kw.ToString("0.###"),Units.NONE),
                new ReportItem("Effective Height (ae)",dto.Ae.ToString("0.###"),Units.IN),
                new ReportItem("Kf", dto.Kf.ToString("0.###"), Units.NONE),
                new ReportItem("Effective Flange Width (be)", dto.Be.ToString("0.###"), Units.IN),
                new ReportItem("Effective Section Modulus (Ze)", dto.Ze.ToString("0.###"), Units.IN_3),
                new ReportItem("Yield stress (Fy)", dto.Fy.ToString("0.###"), Units.KSI),
                new ReportItem("Local Nominal Moment (Mn)", dto.NominalStrength.ToString("0.###"), Units.KIP_IN)
            };
            return new ListReportSection("Local Buckling", items);
        }
    }
}
