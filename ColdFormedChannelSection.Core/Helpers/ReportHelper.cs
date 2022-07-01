using ColdFormedChannelSection.Core.Dtos;
using ColdFormedChannelSection.Core.Entities;
using ColdFormedChannelSection.Core.Enums;
using ColdFormedChannelSection.Core.Extensions;
using System;
using System.Collections.Generic;
using static ColdFormedChannelSection.Core.Constants;

namespace ColdFormedChannelSection.Core.Helpers
{
    public static class ReportHelper
    {

        public static Report AsReport(this AISICompressionDto dto, LippedCSection section)
        {
            var dimSection = section.Dimensions.AsLippedReportSection();
            var lbSection = dto.LB.AsLippedReportSection();
            var fbSection = dto.FB.AsReportSection();
            var tfbSection = dto.FTB.AsReportSection();
            var tbSection = dto.TB.AsReportSection();

            var items = new List<ReportItem>()
            {
                new ReportItem("Governing Case",dto.GoverningCase.FailureMode.GetDescription(),Units.NONE),
                new ReportItem("Nominal Load",dto.GoverningCase.NominalStrength.ToString("0.###"),Units.KIP),
                new ReportItem("phi",PHI_C_AISI.ToString("0.###"),Units.NONE),
               new ReportItem("Design Resistance",(PHI_C_AISI*dto.GoverningCase.NominalStrength).ToString("0.###"),Units.KIP),
            };
            var designSection = new ListReportSection("Design Compression", items);

            var sections = new List<IReportSection>() { dimSection, lbSection, fbSection, tfbSection, tbSection, designSection };

            var report = new Report(UnitSystems.KIPINCH, "AISI Code - Compression", sections);
            return report;
        }

        public static Report AsReport(this AISICompressionDto dto, UnStiffenedCSection section)
        {
            var dimSection = section.Dimensions.AsUnStiffenedReportSection();
            var lbSection = dto.LB.AsUnStiffenedReportSection();
            var fbSection = dto.FB.AsReportSection();
            var tfbSection = dto.FTB.AsReportSection();
            var tbSection = dto.TB.AsReportSection();

            var items = new List<ReportItem>()
            {
                new ReportItem("Governing Case",dto.GoverningCase.FailureMode.GetDescription(),Units.NONE),
                new ReportItem("Nominal Load",dto.GoverningCase.NominalStrength.ToString("0.###"),Units.KIP),
                new ReportItem("phi",PHI_C_AISI.ToString("0.###"),Units.NONE),
               new ReportItem("Design Resistance",(PHI_C_AISI*dto.GoverningCase.NominalStrength).ToString("0.###"),Units.KIP),
            };
            var designSection = new ListReportSection("Design Compression", items);

            var sections = new List<IReportSection>() { dimSection, lbSection, fbSection, tfbSection, tbSection, designSection };

            var report = new Report(UnitSystems.KIPINCH, "AISI Code - Compression", sections);
            return report;
        }

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

        public static ListReportSection AsReportSection(this FBAISICompressionDto dto)
        {
            var items = new List<ReportItem>()
            {
                new ReportItem("Flexural Stress (F1)",dto.F.ToString("0.###"),Units.KSI),
                new ReportItem("Area (A1)",dto.A.ToString("0.###"),Units.IN_2),
                new ReportItem("Flexural Nominal Load (Pn)",dto.NominalStrength.ToString("0.###"),Units.KIP),
            };
            return new ListReportSection("Flexural Buckling", items);
        }

        public static ListReportSection AsReportSection(this FTBAISICompressionDto dto)
        {
            var items = new List<ReportItem>()
            {
                new ReportItem("Torsional Flexural Stress (F2)",dto.F.ToString("0.###"),Units.KSI),
                new ReportItem("Area (A2)",dto.A.ToString("0.###"),Units.IN_2),
                new ReportItem("Torsional Flexural Nominal Load (Pn)",dto.NominalStrength.ToString("0.###"),Units.KIP),
            };
            return new ListReportSection("Torsional Flexural Buckling", items);
        }

        public static ListReportSection AsReportSection(this TBAISICompressioDto dto)
        {
            var items = new List<ReportItem>()
            {
                new ReportItem("Torsional Stress (F3)",dto.F.ToString("0.###"),Units.KSI),
                new ReportItem("Area (A3)",dto.A.ToString("0.###"),Units.IN_2),
                new ReportItem("Torsional Nominal Load (Pn)",dto.NominalStrength.ToString("0.###"),Units.KIP),
            };
            return new ListReportSection("Torsional Buckling", items);
        }

        public static ListReportSection AsLippedReportSection(this LocalAISICompressionDto dto)
        {
            var items = new List<ReportItem>()
            {
                new ReportItem("Kw",dto.Kw.ToString("0.###"),Units.NONE),
                new ReportItem("Effective Height (ae)",dto.Ae.ToString("0.###"),Units.IN),
                new ReportItem("Kc", dto.Kc.ToString("0.###"), Units.NONE),
                new ReportItem("Effective Lip (ce)", dto.Ce.ToString("0.###"), Units.IN),
                new ReportItem("Kf", dto.Kf.ToString("0.###"), Units.NONE),
                new ReportItem("Effective Flange Width (be)", dto.Be.ToString("0.###"), Units.IN),
                new ReportItem("Yield stress (Fy)", dto.Fy.ToString("0.###"), Units.KSI),
                new ReportItem("Local Nominal Load (Pn)", dto.NominalStrength.ToString("0.###"), Units.KIP_IN)
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

        public static ListReportSection AsUnStiffenedReportSection(this LocalAISICompressionDto dto)
        {
            var items = new List<ReportItem>()
            {
                new ReportItem("Kw",dto.Kw.ToString("0.###"),Units.NONE),
                new ReportItem("Effective Height (ae)",dto.Ae.ToString("0.###"),Units.IN),
                new ReportItem("Kf", dto.Kf.ToString("0.###"), Units.NONE),
                new ReportItem("Effective Flange Width (be)", dto.Be.ToString("0.###"), Units.IN),
                new ReportItem("Yield stress (Fy)", dto.Fy.ToString("0.###"), Units.KSI),
                new ReportItem("Local Nominal Load (Pn)", dto.NominalStrength.ToString("0.###"), Units.KIP_IN)
            };
            return new ListReportSection("Local Buckling", items);
        }



        public static ListReportSection AsUnStiffenedReportSection(this LocalDSMomentDto dto)
        {
            var items = new List<ReportItem>()
            {
                new ReportItem("Kw" , dto.Kw.ToString("0.###"),Units.NONE),
                new ReportItem("Kf" , dto.Kf.ToString("0.###"),Units.NONE),
            };
            var section = new ListReportSection("Local Buckling k values", items);
            return section;
        }

        public static ListReportSection AsLippedReportSection(this LocalDSMomentDto dto)
        {
            var items = new List<ReportItem>()
            {
                new ReportItem("Kw" , dto.Kw.ToString("0.###"),Units.NONE),
                new ReportItem("Kf" , dto.Kf.ToString("0.###"),Units.NONE),
                new ReportItem("Kc" , dto.Kc.ToString("0.###"),Units.NONE),

            };
            var section = new ListReportSection("Local Buckling k values", items);
            return section;
        }

        public static Report AsReport(this DSMomentDto dto, LippedCSection section)
        {
            var lb = dto.LB.AsLippedReportSection();
            var dimSection = section.Dimensions.AsLippedReportSection();
            var bucklingMomentItems = new List<ReportItem>()
            {
                new ReportItem("Local Buckling Moment (Mcrd)", dto.LB.Mcrl.ToString("0.###"), Units.KIP_IN),
                new ReportItem("Distortional Buckling Moment (Mcrd)", dto.Mcrd.ToString("0.###"), Units.KIP_IN),
                new ReportItem("Global Buckling Moment (Mcre)", dto.Mcre.ToString("0.###"), Units.KIP_IN),
            };
            
            var nominalItems = new List<ReportItem>()
            {
                new ReportItem("Nominal Local Buckling Moment (Mnl)" , dto.Mnl.ToString("0.###"),Units.KIP_IN),
                new ReportItem("Nominal Distortional Buckling Moment (Mnd)" , dto.Mnd.ToString("0.###"),Units.KIP_IN),
                new ReportItem("Nominal Global Buckling Moment (Mne)" , dto.Mne.ToString("0.###"),Units.KIP_IN),
            };
            var squash_items = new List<ReportItem>()
            {
                 new ReportItem("Yield Stress (Fy)",dto.Fy.ToString("0.###"),Units.KSI),
                  new ReportItem("Section Modulus (Zg)",dto.Zg.ToString("0.###"),Units.IN_3),
                new ReportItem("Squash Moment (My)",dto.My.ToString("0.###"),Units.KIP_IN)
            };

            var designItems = new List<ReportItem>()
            {
                new ReportItem("Governing Case",dto.GoverningCase.FailureMode.GetDescription(),Units.NONE),
                new ReportItem("Nominal Moment (Mn)",dto.GoverningCase.NominalStrength.ToString("0.###"),Units.KIP_IN),
                new ReportItem("phi",phib.ToString("0.###"),Units.NONE),
                new ReportItem("Design Moment",(PHI_B*nominalLoad.Item1).ToString("0.###"),Units.KIP_IN)
            };

            var elasticSection = new ListReportSection("Elstic Buckling Moment", bucklingItems);
            var nominalSection = new ListReportSection("Nominal Flexural Strength", nominalItems);
            var designSection = new ListReportSection("Design Moment", designItems);
            var sections = new List<IReportSection>() { secDimSection, elasticSection, nominalSection, designSection };
            var report = new Report(UnitSystems.KIPINCH, "Direct Strength - Moment", sections);
        }

        public static Report AsReport(this DSMomentDto dto, UnStiffenedCSection section)
        {

        }

    }
}
