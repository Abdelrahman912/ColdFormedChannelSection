using ColdFormedChannelSection.Core.Dtos;
using ColdFormedChannelSection.Core.Entities;
using ColdFormedChannelSection.Core.Enums;
using ColdFormedChannelSection.Core.Extensions;
using CSharpHelper.Extensions;
using System;
using System.Collections.Generic;
using static ColdFormedChannelSection.Core.Constants;

namespace ColdFormedChannelSection.Core.Helpers
{
    public static class ReportHelper
    {

        #region Report

        public static Report AsReport(this AISICompressionZDto dto, LippedZSection section)
        {
            var dimSection = section.Dimensions.AsLippedReportSection();
            var propSection = section.Properties.AsReportSection(UnitSystems.KIPINCH);
            var lbSection = dto.LB.AsLippedReportSection();
            var fbSection = dto.FB.AsReportSection();
            var tbSection = dto.TB.AsReportSection();

            var items = new List<ReportItem>()
            {
                new ReportItem("Governing Case",dto.GoverningCase.FailureMode.GetDescription(),Units.NONE),
                new ReportItem("Nominal Load",dto.GoverningCase.NominalStrength.ToString("0.###"),Units.KIP),
                new ReportItem("phi",PHI_C_AISI.ToString("0.###"),Units.NONE),
               new ReportItem("Design Resistance",(PHI_C_AISI*dto.GoverningCase.NominalStrength).ToString("0.###"),Units.KIP),
            };
            var designSection = new ListReportSection("Design Compression", items);

            var sections = new List<IReportSection>() { dimSection, propSection, lbSection, fbSection, tbSection, designSection };

            var report = new Report(UnitSystems.KIPINCH, "AISI Code - Compression", sections);
            return report;
        }

        public static Report AsReport(this AISICompressionZDto dto, UnStiffenedZSection section)
        {
            var dimSection = section.Dimensions.AsUnStiffenedReportSection();
            var propSection = section.Properties.AsReportSection(UnitSystems.KIPINCH);
            var lbSection = dto.LB.AsUnStiffenedReportSection();
            var fbSection = dto.FB.AsReportSection();
            var tbSection = dto.TB.AsReportSection();

            var items = new List<ReportItem>()
            {
                new ReportItem("Governing Case",dto.GoverningCase.FailureMode.GetDescription(),Units.NONE),
                new ReportItem("Nominal Load",dto.GoverningCase.NominalStrength.ToString("0.###"),Units.KIP),
                new ReportItem("phi",PHI_C_AISI.ToString("0.###"),Units.NONE),
               new ReportItem("Design Resistance",(PHI_C_AISI*dto.GoverningCase.NominalStrength).ToString("0.###"),Units.KIP),
            };
            var designSection = new ListReportSection("Design Compression", items);

            var sections = new List<IReportSection>() { dimSection, propSection, lbSection, fbSection, tbSection, designSection };

            var report = new Report(UnitSystems.KIPINCH, "AISI Code - Compression", sections);
            return report;
        }

        public static Report AsReport(this AISICompressionCDto dto, LippedCSection section)
        {
            var dimSection = section.Dimensions.AsLippedReportSection();
            var propSection = section.Properties.AsReportSection(UnitSystems.KIPINCH);
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

            var sections = new List<IReportSection>() { dimSection, propSection, lbSection, fbSection, tfbSection, tbSection, designSection };

            var report = new Report(UnitSystems.KIPINCH, "AISI Code - Compression", sections);
            return report;
        }

        public static Report AsReport(this AISICompressionCDto dto, UnStiffenedCSection section)
        {
            var dimSection = section.Dimensions.AsUnStiffenedReportSection();
            var propSection = section.Properties.AsReportSection(UnitSystems.KIPINCH);
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

            var sections = new List<IReportSection>() { dimSection, propSection, lbSection, fbSection, tfbSection, tbSection, designSection };

            var report = new Report(UnitSystems.KIPINCH, "AISI Code - Compression", sections);
            return report;
        }

        public static Report AsReport(this AISIMomentDto dto, LippedSection section)
        {
            var dimSection = section.Dimensions.AsLippedReportSection();
            var propSection = section.Properties.AsReportSection(UnitSystems.KIPINCH);
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

            var sections = new List<IReportSection>() { dimSection, propSection, lbSection, ltbSection, designSection };

            var report = new Report(UnitSystems.KIPINCH, "AISI Code - Moment", sections);
            return report;
        }

        public static Report AsReport(this AISIMomentDto dto, UnStiffenedSection section)
        {
            var dimSection = section.Dimensions.AsUnStiffenedReportSection();
            var propSection = section.Properties.AsReportSection(UnitSystems.KIPINCH);
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

            var sections = new List<IReportSection>() { dimSection, propSection, lbSection, ltbSection, designSection };

            var report = new Report(UnitSystems.KIPINCH, "AISI Code - Moment", sections);
            return report;
        }

        private static Report AsReport(this DSCompressionDto dto, ListReportSection dimSection, ListReportSection propSection, ListReportSection lbSection)
        {

            var bucklingMomentItems = new List<ReportItem>()
            {
                new ReportItem("Local Buckling Moment (Pcrl)", dto.LB.Pcrl.ToString("0.###"), Units.KIP),
                new ReportItem("Distortional Buckling Load (Pcrd)", dto.Pcrd.ToString("0.###"), Units.KIP),
                new ReportItem("Global Buckling Moment (Pcre)", dto.Pcre.ToString("0.###"), Units.KIP),
            };

            var nominalItems = new List<ReportItem>()
            {
                new ReportItem("Nominal Local Buckling Load (Pnl)" , dto.Pnl.ToString("0.###"),Units.KIP),
                new ReportItem("Nominal Distortional Buckling Load (Pnd)" , dto.Pnd.ToString("0.###"),Units.KIP),
                new ReportItem("Nominal Global Buckling Load (Pne)" , dto.Pne.ToString("0.###"),Units.KIP),
            };
            var squash_items = new List<ReportItem>()
            {
                 new ReportItem("Yield Stress (Fy)",dto.Fy.ToString("0.###"),Units.KSI),
                  new ReportItem("Area (Ag)",dto.Ag.ToString("0.###"),Units.IN_2),
                new ReportItem("Squash Load (My)",dto.Py.ToString("0.###"),Units.KIP)
            };

            var designItems = new List<ReportItem>()
            {
                new ReportItem("Governing Case",dto.GoverningCase.FailureMode.GetDescription(),Units.NONE),
                new ReportItem("Nominal Load (Pn)",dto.GoverningCase.NominalStrength.ToString("0.###"),Units.KIP),
                new ReportItem("phi",PHI_C_DS.ToString("0.###"),Units.NONE),
                new ReportItem("Design Strength (phi*Pn)",(PHI_C_DS*dto.GoverningCase.NominalStrength).ToString("0.###"),Units.KIP),
            };

            var elasticSection = new ListReportSection("Elastic Buckling Load", bucklingMomentItems);
            var squashSection = new ListReportSection("Yield Load", squash_items);
            var nominalSection = new ListReportSection("Nominal Flexural Strength", nominalItems);
            var designSection = new ListReportSection("Design Load", designItems);
            var sections = new List<IReportSection>() { dimSection, propSection, lbSection, elasticSection, nominalSection, squashSection, designSection };
            var report = new Report(UnitSystems.KIPINCH, "Direct Strength - Compression", sections);
            return report;
        }

        public static Report AsReport(this DSCompressionDto dto, LippedSection section)
        {
            var dimSection = section.Dimensions.AsLippedReportSection();
            var propSection = section.Properties.AsReportSection(UnitSystems.KIPINCH);
            var lbSection = dto.LB.AsReportSection(TypeOfSection.LIPPED);
            return dto.AsReport(dimSection, propSection, lbSection);
        }

        public static Report AsReport(this DSCompressionDto dto, UnStiffenedSection section)
        {
            var dimSection = section.Dimensions.AsUnStiffenedReportSection();
            var propSection = section.Properties.AsReportSection(UnitSystems.KIPINCH);
            var lbSection = dto.LB.AsReportSection(TypeOfSection.UNSTIFFENED);
            return dto.AsReport(dimSection, propSection, lbSection);
        }

        private static Report AsReport(this DSMomentDto dto, ListReportSection lbSection, ListReportSection dimSection, ListReportSection propSection)
        {
            var bucklingMomentItems = new List<ReportItem>()
            {
                new ReportItem("Local Buckling Moment (Mcrl)", dto.LB.Mcrl.ToString("0.###"), Units.KIP_IN),
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
                new ReportItem("phi",PHI_B_DS.ToString("0.###"),Units.NONE),
                new ReportItem("Design Moment",(PHI_B_DS*dto.GoverningCase.NominalStrength).ToString("0.###"),Units.KIP_IN)
            };

            var elasticSection = new ListReportSection("Elastic Buckling Moment", bucklingMomentItems);
            var squashSection = new ListReportSection("Yield Moment", squash_items);
            var nominalSection = new ListReportSection("Nominal Flexural Strength", nominalItems);
            var designSection = new ListReportSection("Design Moment", designItems);
            var sections = new List<IReportSection>() { dimSection, propSection, lbSection, elasticSection, nominalSection, squashSection, designSection };
            var report = new Report(UnitSystems.KIPINCH, "Direct Strength - Moment", sections);
            return report;
        }

        public static Report AsReport(this DSMomentDto dto, LippedSection section)
        {
            var lbSection = dto.LB.AsReportSection(TypeOfSection.LIPPED);
            var dimSection = section.Dimensions.AsLippedReportSection();
            var propSection = section.Properties.AsReportSection(UnitSystems.KIPINCH);
            return dto.AsReport(lbSection, dimSection, propSection);
        }

        public static Report AsReport(this DSMomentDto dto, UnStiffenedSection section)
        {
            var lbSection = dto.LB.AsReportSection(TypeOfSection.UNSTIFFENED);
            var dimSection = section.Dimensions.AsUnStiffenedReportSection();
            var propSection = section.Properties.AsReportSection(UnitSystems.KIPINCH);
            return dto.AsReport(lbSection, dimSection, propSection);
        }

        private static Report AsReport(this EuroMomentDto dto, ListReportSection lbSection, ListReportSection dimSection, ListReportSection propSection)
        {
            var ltbSection = dto.LTB.AsReportSection();
            var designItems = new List<ReportItem>()
            {
                new ReportItem("Governing Case",dto.GoverningCase.FailureMode.GetDescription(),Units.NONE),
                new ReportItem("Nominal Moment",dto.GoverningCase.NominalStrength.ToString("0.###"),Units.N_MM),
                new ReportItem("gamma",(PHI_EURO).ToString("0.###"),Units.NONE),
                new ReportItem("Design Moment (Mn/gamma)",(PHI_EURO*dto.GoverningCase.NominalStrength).ToString("0.###"),Units.N_MM)
            };
            var designSection = new ListReportSection("Design moment", designItems);
            var sections = new List<IReportSection>() { dimSection, propSection, lbSection, ltbSection, designSection };
            var report = new Report(UnitSystems.NMM, "Euro Code - Moment", sections);
            return report;
        }

        public static Report AsReport(this EuroMomentDto dto, LippedSection section)
        {
            var dimSection = section.Dimensions.AsLippedReportSection();
            var lbSection = dto.LB.AsLippedReportSection();
            var propSection = section.Properties.AsReportSection(UnitSystems.NMM);
            return dto.AsReport(lbSection, dimSection, propSection);
        }

        public static Report AsReport(this EuroMomentDto dto, UnStiffenedSection section)
        {
            var dimSection = section.Dimensions.AsUnStiffenedReportSection();
            var lbSection = dto.LB.AsUnStiffenedReportSection();
            var propSection = section.Properties.AsReportSection(UnitSystems.NMM);
            return dto.AsReport(lbSection, dimSection, propSection);
        }

        public static Report AsReport(this EuroCompressionZDto dto, LippedZSection section)
        {
            var dimSection = section.Dimensions.AsLippedReportSection();
            var lbSection = dto.LB.AsLippedReportSection();
            var propSection = section.Properties.AsReportSection(UnitSystems.NMM);
            return dto.AsReport(lbSection, dimSection, propSection);
        }

        public static Report AsReport(this EuroCompressionZDto dto, UnStiffenedZSection section)
        {
            var dimSection = section.Dimensions.AsUnStiffenedReportSection();
            var lbSection = dto.LB.AsUnStiffenedReportSection();
            var propSection = section.Properties.AsReportSection(UnitSystems.NMM);
            return dto.AsReport(lbSection, dimSection, propSection);
        }

        public static Report AsReport(this EuroCompressionZDto dto, ListReportSection lbSection, ListReportSection dimSection, ListReportSection propSection)
        {
            var fbSection = dto.FB.AsReportSection();
            var tbSection = dto.TB.AsReportSection();
            var designItems = new List<ReportItem>()
            {
                new ReportItem("Governing Case",dto.GoverningCase.FailureMode.GetDescription(),Units.NONE),
                new ReportItem("Nominal Load (Pn)",dto.GoverningCase.NominalStrength.ToString("0.###"),Units.N),
                new ReportItem("Gamma",(PHI_EURO).ToString("0.###"),Units.NONE),
                new ReportItem("Design Load (Pn/gamma)",dto.GoverningCase.NominalStrength.ToString("0.###"),Units.N),
            };
            var designSection = new ListReportSection("Euro Code - Compression", designItems);
            var sections = new List<IReportSection>() { dimSection, propSection, lbSection, fbSection, tbSection, designSection };
            return new Report(UnitSystems.NMM, "Euro Code - Compression", sections);
        }

        public static Report AsReport(this EuroCompressionCDto dto, LippedCSection section)
        {
            var dimSection = section.Dimensions.AsLippedReportSection();
            var lbSection = dto.LB.AsLippedReportSection();
            var propSection = section.Properties.AsReportSection(UnitSystems.NMM);
            return dto.AsReport(lbSection, dimSection, propSection);
        }

        public static Report AsReport(this EuroCompressionCDto dto, UnStiffenedCSection section)
        {
            var dimSection = section.Dimensions.AsUnStiffenedReportSection();
            var lbSection = dto.LB.AsUnStiffenedReportSection();
            var propSection = section.Properties.AsReportSection(UnitSystems.NMM);
            return dto.AsReport(lbSection, dimSection, propSection);
        }

        public static Report AsReport(this EuroCompressionCDto dto, ListReportSection lbSection, ListReportSection dimSection, ListReportSection propSection)
        {
            var fbSection = dto.FB.AsReportSection();
            var tbSection = dto.TB.AsReportSection();
            var ftbSection = dto.FTB.AsReportSection();
            var designItems = new List<ReportItem>()
            {
                new ReportItem("Governing Case",dto.GoverningCase.FailureMode.GetDescription(),Units.NONE),
                new ReportItem("Nominal Load (Pn)",dto.GoverningCase.NominalStrength.ToString("0.###"),Units.N),
                new ReportItem("Gamma",(PHI_EURO).ToString("0.###"),Units.NONE),
                new ReportItem("Design Load (Pn/gamma)",dto.GoverningCase.NominalStrength.ToString("0.###"),Units.N),
            };
            var designSection = new ListReportSection("Euro Code - Compression", designItems);
            var sections = new List<IReportSection>() { dimSection, propSection, lbSection, fbSection, tbSection, ftbSection, designSection };
            return new Report(UnitSystems.NMM, "Euro Code - Compression", sections);
        }


        private static Report AsReport(this EgyptMomentDto dto, ListReportSection lbSection, ListReportSection dimSection, ListReportSection propSection)
        {
            var nominalSection = dto.GoverningCase.AsReportSection();
            var designItems = new List<ReportItem>()
            {
                new ReportItem("Governing Case",dto.GoverningCase.GoverningCase.FailureMode.GetDescription(),Units.NONE),
                new ReportItem("Nominal Moment (Mn)",dto.GoverningCase.GoverningCase.NominalStrength.ToString("0.###"),Units.TON_CM),
                new ReportItem("phi",PHI_B_EGYPT.ToString("0.###"),Units.NONE),
                new ReportItem("Design Moment",(PHI_B_EGYPT*dto.GoverningCase.GoverningCase.NominalStrength).ToString("0.###"),Units.TON_CM)
            };
            var designSection = new ListReportSection("Design Moment", designItems);
            var sections = new List<IReportSection>() { dimSection, propSection, lbSection, nominalSection, designSection };

            var report = new Report(UnitSystems.TONCM, "Egyptian Code - Moment", sections);
            return report;
        }

        public static Report AsReport(this EgyptMomentDto dto, LippedSection section)
        {
            var lbSection = dto.LB.AsLippedReportSection();
            var dimSection = section.Dimensions.AsLippedReportSection();
            var propSection = section.Properties.AsReportSection(UnitSystems.NMM);
            return dto.AsReport(lbSection, dimSection, propSection);
        }

        public static Report AsReport(this EgyptMomentDto dto, UnStiffenedSection section)
        {
            var lbSection = dto.LB.AsUnStiffenedSection();
            var dimSection = section.Dimensions.AsUnStiffenedReportSection();
            var propSection = section.Properties.AsReportSection(UnitSystems.TONCM);
            return dto.AsReport(lbSection, dimSection, propSection);
        }


        private static Report AsReport(this EgyptCompressionCDto dto, ListReportSection lbSection, ListReportSection dimSection, ListReportSection propSection)
        {
            var fbSection = dto.FB.AsReportSection();
            var tfbSection = dto.TFB.AsReportSection();
            var tbSection = dto.TB.AsReportSection();
            var designItems = new List<ReportItem>()
            {
                new ReportItem("Governing Case",dto.GoverningCase.FailureMode.GetDescription(),Units.NONE),
                new ReportItem("Nomial Load (Pn)",dto.GoverningCase.NominalStrength.ToString("0.###"),Units.TON),
                new ReportItem("phi",$"{PHI_C_EGYPT}",Units.TON),
                new ReportItem("Design Load (phi*Pn)",$"{(PHI_C_EGYPT*dto.GoverningCase.NominalStrength).ToString("0.###")}",Units.TON),
            };
            var sections = new List<IReportSection> { dimSection, propSection, lbSection, fbSection, tfbSection, tbSection };
            var report = new Report(UnitSystems.TONCM, "Egyptian Code - Compression", sections);
            return report;
        }

        public static Report AsReport(this EgyptCompressionCDto dto, LippedCSection section)
        {
            var lbSection = dto.LB.AsLippedReportSection();
            var dimSection = section.Dimensions.AsLippedReportSection();
            var propSection = section.Properties.AsReportSection(UnitSystems.TONCM);
            return dto.AsReport(lbSection, dimSection, propSection);
        }

        public static Report AsReport(this EgyptCompressionCDto dto, UnStiffenedCSection section)
        {
            var lbSection = dto.LB.AsUnStiffenedReportSection();
            var dimSection = section.Dimensions.AsUnStiffenedReportSection();
            var propSection = section.Properties.AsReportSection(UnitSystems.TONCM);
            return dto.AsReport(lbSection, dimSection, propSection);
        }


        private static Report AsReport(this EgyptCompressionZDto dto, ListReportSection lbSection, ListReportSection dimSection, ListReportSection propSection)
        {
            var fbSection = dto.FB.AsReportSection();
            var tbSection = dto.TB.AsReportSection();
            var designItems = new List<ReportItem>()
            {
                new ReportItem("Governing Case",dto.GoverningCase.FailureMode.GetDescription(),Units.NONE),
                new ReportItem("Nomial Load (Pn)",dto.GoverningCase.NominalStrength.ToString("0.###"),Units.TON),
                new ReportItem("phi",$"{PHI_C_EGYPT}",Units.TON),
                new ReportItem("Design Load (phi*Pn)",$"{(PHI_C_EGYPT*dto.GoverningCase.NominalStrength).ToString("0.###")}",Units.TON),
            };
            var sections = new List<IReportSection> { dimSection, propSection, lbSection, fbSection, tbSection };
            var report = new Report(UnitSystems.TONCM, "Egyptian Code - Compression", sections);
            return report;
        }

        public static Report AsReport(this EgyptCompressionZDto dto, LippedZSection section)
        {
            var lbSection = dto.LB.AsLippedReportSection();
            var dimSection = section.Dimensions.AsLippedReportSection();
            var propSection = section.Properties.AsReportSection(UnitSystems.TONCM);
            return dto.AsReport(lbSection, dimSection, propSection);
        }

        public static Report AsReport(this EgyptCompressionZDto dto, UnStiffenedZSection section)
        {
            var lbSection = dto.LB.AsUnStiffenedReportSection();
            var dimSection = section.Dimensions.AsUnStiffenedReportSection();
            var propSection = section.Properties.AsReportSection(UnitSystems.TONCM);
            return dto.AsReport(lbSection, dimSection, propSection);
        }

        #endregion

        #region Report Section



        public static ListReportSection AsReportSection(this InteractionDSCompressionDto dto)
        {
            var items = new List<ReportItem>()
            {
                new ReportItem("K_flange_web",dto.KFlangeWeb.ToString("0.###"),Units.NONE),
                new ReportItem("K_flange_lip",dto.KFlangeLip.ToString("0.###"),Units.NONE)
            };
            var section = new ListReportSection("Interaction method K values", items);
            return section;
        }

        public static ListReportSection AsLippedReportSection(this ElementDSCompressionDto dto)
        {
            var items = new List<ReportItem>()
            {
                new ReportItem("Kw",dto.Kw.ToString("0.###"),Units.NONE),
                new ReportItem("Kf",dto.Kf.ToString("0.###"),Units.NONE),
                new ReportItem("Kc",dto.Kc.ToString("0.###"),Units.NONE)
            };
            var section = new ListReportSection("Element method K values", items);
            return section;
        }

        public static ListReportSection AsUnStiffenedReportSection(this ElementDSCompressionDto dto)
        {
            var items = new List<ReportItem>()
            {
                new ReportItem("Kw",dto.Kw.ToString("0.###"),Units.NONE),
                new ReportItem("Kf",dto.Kf.ToString("0.###"),Units.NONE),
            };
            var section = new ListReportSection("Element method K values", items);
            return section;
        }

        public static ListReportSection AsReportSection(this CSectionProperties sec, UnitSystems system)
        {
            var items = new List<ReportItem>()
            {
                new ReportItem("Area",sec.A.ToString("0.###"),Power2Dict[system]),
                new ReportItem("Ix",sec.Ix.ToString("0.###"),Power4Dict[system]),
                new ReportItem("Sx",sec.Zg.ToString("0.###"),Power3Dict[system]),
                new ReportItem("rx",sec.Rx.ToString("0.###"),Power1Dict[system]),
                new ReportItem("Iy",sec.Iy.ToString("0.###"),Power4Dict[system]),
                new ReportItem("ry",sec.Ry.ToString("0.###"),Power1Dict[system]),
                new ReportItem("J",sec.J.ToString("0.###"),Power4Dict[system]),
                new ReportItem("Cw",sec.Cw.ToString("0.###"),Power6Dict[system]),
                new ReportItem("m",sec.M.ToString("0.###"),Power1Dict[system]),
                new ReportItem("Xo",sec.Xo.ToString("0.###"),Power1Dict[system])
            };
            var section = new ListReportSection("Section Properties", items);
            return section;
        }

        public static ListReportSection AsReportSection(this ZSectionProperties sec, UnitSystems system)
        {
            var items = new List<ReportItem>()
            {
                new ReportItem("Area",sec.A.ToString("0.###"),Power2Dict[system]),
                new ReportItem("Ix",sec.Ix.ToString("0.###"),Power4Dict[system]),
                new ReportItem("Sx",sec.Zg.ToString("0.###"),Power3Dict[system]),
                new ReportItem("rx",sec.RxGeom.ToString("0.###"),Power1Dict[system]),
                new ReportItem("Iy",sec.Iy.ToString("0.###"),Power4Dict[system]),
                new ReportItem("ry",sec.RyGeom.ToString("0.###"),Power1Dict[system]),
                new ReportItem("Ixy",sec.Ixy.ToString("0.###"),Power4Dict[system]),
                new ReportItem("Ix2",sec.IxPrincipal.ToString("0.###"),Power4Dict[system]),
                new ReportItem("Iy2",sec.IyPrincipal.ToString("0.###"),Power4Dict[system]),
                new ReportItem("rmin",sec.Rx.ToString("0.###"),Power1Dict[system]),
                new ReportItem("rmax",sec.Ry.ToString("0.###"),Power1Dict[system]),
                new ReportItem("theta",sec.ThetaPrime.ToDegree().ToString("0.#"),Units.NONE),
                new ReportItem("J",sec.J.ToString("0.###"),Power4Dict[system]),
                new ReportItem("Cw",sec.Cw.ToString("0.###"),Power6Dict[system]),
            };
            var section = new ListReportSection("Section Properties", items);
            return section;
        }

        public static ListReportSection AsReportSection(this ZSectionProperties sec)
        {
            var items = new List<ReportItem>()
            {

            };
            var section = new ListReportSection("Section Properties", items);
            return section;
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
                new ReportItem("Local Nominal Load (Pn)", dto.NominalStrength.ToString("0.###"), Units.KIP)
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
                new ReportItem("Local Nominal Load (Pn)", dto.NominalStrength.ToString("0.###"), Units.KIP)
            };
            return new ListReportSection("Local Buckling", items);
        }


        public static ListReportSection AsReportSection(this InteractionDSMomentDto dto)
        {
            var items = new List<ReportItem>()
            {
                new ReportItem("K_flange_web" , dto.KFlangeWeb.ToString("0.###"),Units.NONE),
                new ReportItem("K_flange_lip" , dto.KFlangeLip.ToString("0.###"),Units.NONE),
            };
            var section = new ListReportSection("Interaction method k values", items);
            return section;
        }

        public static ListReportSection AsUnStiffenedReportSection(this ElementDSMomentDto dto)
        {
            var items = new List<ReportItem>()
            {
                new ReportItem("Kw" , dto.Kw.ToString("0.###"),Units.NONE),
                new ReportItem("Kf" , dto.Kf.ToString("0.###"),Units.NONE),
            };
            var section = new ListReportSection("Element method k values", items);
            return section;
        }

        public static ListReportSection AsLippedReportSection(this ElementDSMomentDto dto)
        {
            var items = new List<ReportItem>()
            {
                new ReportItem("Kw" , dto.Kw.ToString("0.###"),Units.NONE),
                new ReportItem("Kf" , dto.Kf.ToString("0.###"),Units.NONE),
                new ReportItem("Kc" , dto.Kc.ToString("0.###"),Units.NONE),

            };
            var section = new ListReportSection("Element k values", items);
            return section;
        }

        public static ListReportSection AsLippedReportSection(this LocalEuroMomentDto dto)
        {
            var items = new List<ReportItem>()
            {
                new ReportItem("Effective Heigh (ae)",dto.Ae.ToString("0.###"),Units.MM),
                new ReportItem("Effective Flange Width (be)",dto.Be.ToString("0.###"),Units.MM),
                new ReportItem("Effective Lip (Ce)", dto.Ce.ToString("0.###"), Units.MM),
                new ReportItem("Reduction Factor (Xd)", dto.Xd.ToString("0.###"), Units.NONE),
                new ReportItem("Effective Section Modulus (Ze)", dto.Ze.ToString("0.###"), Units.MM_3),
                new ReportItem("Yield Stress (Fy)", dto.Fy.ToString("0.###"), Units.N_MM_2),
                new ReportItem("Local Nominal Moment (Mn)", (dto.Ze * dto.Fy).ToString("0.###"), Units.N_MM),
            };
            var section = new ListReportSection("Local Buckling", items);
            return section;
        }

        public static ListReportSection AsUnStiffenedReportSection(this LocalEuroMomentDto dto)
        {
            var items = new List<ReportItem>()
            {
                new ReportItem("Effective Heigh (ae)",dto.Ae.ToString("0.###"),Units.MM),
                new ReportItem("Effective Flange Width (be)",dto.Be.ToString("0.###"),Units.MM),
                new ReportItem("Reduction Factor (Xd)", dto.Xd.ToString("0.###"), Units.NONE),
                new ReportItem("Effective Section Modulus (Ze)", dto.Ze.ToString("0.###"), Units.MM_3),
                new ReportItem("Yield Stress (Fy)", dto.Fy.ToString("0.###"), Units.N_MM_2),
                new ReportItem("Local Nominal Moment (Mn)", (dto.Ze * dto.Fy).ToString("0.###"), Units.N_MM),
            };
            var section = new ListReportSection("Local Buckling", items);
            return section;
        }

        public static ListReportSection AsReportSection(this LTBEuroMomentDto dto)
        {
            var items = new List<ReportItem>()
            {
                new ReportItem("Lateral Torsional Section Modulus (Z)",dto.Z.ToString("0.###"),Units.MM_3),
                new ReportItem("Xlt",dto.X.ToString("0.###"),Units.NONE),
                new ReportItem("Lateral Torsional Stress (F)",dto.F.ToString("0.###"),Units.N_MM_2),
                new ReportItem("Lateral Torsional Nominal Moment (Mn)",dto.NominalStrength.ToString("0.###"),Units.N_MM),

            };
            var section = new ListReportSection("Lateral Torsional Buckling", items);
            return section;
        }

        public static ListReportSection AsLippedReportSection(this LocalEuroCompressionDto dto)
        {
            var items = new List<ReportItem>()
            {
                 new ReportItem("Kw",dto.Kw.ToString("0.###"),Units.NONE),
                new ReportItem("Effective Height (ae)",dto.Ae.ToString("0.###"),Units.MM),
                new ReportItem("Kf",dto.Kf.ToString("0.###"),Units.NONE),
                new ReportItem("Effective Flange Width (be)",dto.Be.ToString("0.###"),Units.MM),
                new ReportItem("Effective Lip (ce)",dto.Ce.ToString("0.###"),Units.MM),
                new ReportItem("Reduction Factor (Xd)",dto.Xd.ToString("0.###"),Units.NONE),
                new ReportItem("Effective Area (Ae)",dto.AreaEffective.ToString("0.###"),Units.MM_2),
                new ReportItem("Yield Stress (Fy)",dto.Fy.ToString("0.###"),Units.N_MM_2),
                new ReportItem("Nominal Load (Pn)",(dto.Fy*dto.AreaEffective).ToString("0.###"),Units.N),
            };
            var section = new ListReportSection("Local Buckling", items);
            return section;
        }

        public static ListReportSection AsUnStiffenedReportSection(this LocalEuroCompressionDto dto)
        {
            var items = new List<ReportItem>()
            {
                 new ReportItem("Kw",dto.Kw.ToString("0.###"),Units.NONE),
                new ReportItem("Effective Height (ae)",dto.Ae.ToString("0.###"),Units.MM),
                new ReportItem("Kf",dto.Kf.ToString("0.###"),Units.NONE),
                new ReportItem("Effective Flange Width (be)",dto.Be.ToString("0.###"),Units.MM),
                new ReportItem("Reduction Factor (Xd)",dto.Xd.ToString("0.###"),Units.NONE),
                new ReportItem("Effective Area (Ae)",dto.AreaEffective.ToString("0.###"),Units.MM_2),
                new ReportItem("Yield Stress (Fy)",dto.Fy.ToString("0.###"),Units.N_MM_2),
                new ReportItem("Nominal Load (Pn)",(dto.Fy*dto.AreaEffective).ToString("0.###"),Units.N),
            };
            var section = new ListReportSection("Local Buckling", items);
            return section;
        }

        public static ListReportSection AsReportSection(this FBEuroCompressionDto dto)
        {
            var items = new List<ReportItem>()
            {
                new ReportItem("X",dto.X.ToString("0.###"),Units.NONE),
                new ReportItem("Flexural Stress (X.Fy)",(dto.X*dto.Fy).ToString("0.###"),Units.N_MM_2),
                new ReportItem("Flexural Area (A)",dto.A.ToString("0.###"),Units.MM_2),
                new ReportItem("Nominal Load (Pn)",dto.NominalStrength.ToString("0.###"),Units.N)
            };
            var section = new ListReportSection("Flexural Buckling", items);
            return section;
        }

        public static ListReportSection AsReportSection(this TBEuroCompressionDto dto)
        {
            var items = new List<ReportItem>()
            {
                new ReportItem("Xt",dto.X.ToString("0.###"),Units.NONE),
                new ReportItem("Torsional Stress (Xt.Fy)",(dto.X*dto.Fy).ToString("0.###"),Units.N_MM_2),
                new ReportItem("Torsional Area (Ae)",dto.A.ToString("0.###"),Units.MM_2),
                new ReportItem("Nominal Load (Pn)",dto.NominalStrength.ToString("0.###"),Units.N)
            };
            var section = new ListReportSection("Torsional Buckling", items);
            return section;
        }

        public static ListReportSection AsReportSection(this FTBEuroCompressionDto dto)
        {
            var items = new List<ReportItem>()
            {
                new ReportItem("Xft",dto.X.ToString("0.###"),Units.NONE),
                new ReportItem("Torsional Flexural Stress (Xft.Fy)",(dto.X*dto.Fy).ToString("0.###"),Units.N_MM_2),
                new ReportItem("Torsional Flexural Area (Ae)",dto.A.ToString("0.###"),Units.MM_2),
                new ReportItem("Nominal Load (Pn)",dto.NominalStrength.ToString("0.###"),Units.N)
            };
            var section = new ListReportSection("Torsional Flexural Buckling", items);
            return section;
        }

        public static ListReportSection AsLippedReportSection(this LocalEgyptMomentDto dto)
        {
            var items = new List<ReportItem>()
            {
                new ReportItem("Effective Height (ae)",dto.Ae.ToString("0.###"),Units.CM),
                new ReportItem("Effective Flange Width (be)",dto.Be.ToString("0.###"),Units.CM),
                new ReportItem("Effective Lip (ce)", dto.Ce.ToString("0.###"), Units.CM)
            };
            var section = new ListReportSection("Effective Section", items);
            return section;
        }

        public static ListReportSection AsUnStiffenedSection(this LocalEgyptMomentDto dto)
        {
            var items = new List<ReportItem>()
            {
                new ReportItem("Effective Height (ae)",dto.Ae.ToString("0.###"),Units.CM),
                new ReportItem("Effective Flange Width (be)",dto.Be.ToString("0.###"),Units.CM),
            };
            var section = new ListReportSection("Effective Section", items);
            return section;
        }

        public static ListReportSection AsReportSection(this EgyptMomentLBDto dto)
        {
            var items = new List<ReportItem>()
                {
                    new ReportItem("Flange Slenderness Ratio (lambada_f)",dto.LambdaF.ToString("0.###"),Units.NONE),
                    new ReportItem("Nominal Moment (Mn)",dto.GoverningCase.NominalStrength.ToString("0.###"),Units.TON_CM)
                };
            var section = new ListReportSection("Nominal Moment", items);
            return section;
        }

        public static ListReportSection AsReportSection(this EgyptMomentLTBDto dto)
        {
            var items = new List<ReportItem>()
                {
                    new ReportItem("Flange Slenderness Ratio (lambda_f)",dto.LambdaF.ToString("0.###"),Units.NONE),
                    new ReportItem("Local Section Modulus",dto.ZLocal.ToString("0.###"),Units.CM_3),
                    new ReportItem("Local Stress (F)",(dto.FLocal).ToString("0.###"),Units.TON_CM_2),
                    new ReportItem("Local Nominal Moment (Mn)",dto.MnLocal.ToString("0.###"),Units.TON_CM),
                    new ReportItem("Lateral Torsional Modulus (Z)",dto.ZLTB.ToString("0.###"),Units.CM_3),
                    new ReportItem("Lateral Torsional Stress (F)",dto.FLTB.ToString("0.###"),Units.TON_CM_2),
                    new ReportItem("Lateral Torsional Nominal Moment (Mn)",dto.MnLTB.ToString("0.###"),Units.TON_CM)
                };
            var section = new ListReportSection("Nominal Moment", items);
            return section;
        }

        public static ListReportSection AsLippedReportSection(this LocalEgyptCompressionDto dto)
        {
            var items = new List<ReportItem>()
            {
                new ReportItem("Kw",dto.Kw.ToString("0.###"),Units.NONE),
                new ReportItem("Effective Height (ae)",dto.Ae.ToString("0.###"),Units.CM),
                new ReportItem("Kf",dto.Kf.ToString("0.###"),Units.NONE),
                new ReportItem("Effective Width (be)",dto.Be.ToString("0.###"),Units.CM),
                new ReportItem("Kf",dto.Kc.ToString("0.###"),Units.NONE),
                new ReportItem("Effective Lip (ce)",dto.Ce.ToString("0.###"),Units.CM),
                new ReportItem("Effective Area (Ae)",dto.AreaEffective.ToString("0.###"),Units.CM_2),
            };
            var section = new ListReportSection("Local Buckling", items);
            return section;
        }

        public static ListReportSection AsUnStiffenedReportSection(this LocalEgyptCompressionDto dto)
        {
            var items = new List<ReportItem>()
            {
                new ReportItem("Kw",dto.Kw.ToString("0.###"),Units.NONE),
                new ReportItem("Effective Height (ae)",dto.Ae.ToString("0.###"),Units.CM),
                new ReportItem("Kf",dto.Kf.ToString("0.###"),Units.NONE),
                new ReportItem("Effective Width (be)",dto.Be.ToString("0.###"),Units.CM),
                new ReportItem("Kf",dto.Kc.ToString("0.###"),Units.NONE),
                new ReportItem("Effective Area (Ae)",dto.AreaEffective.ToString("0.###"),Units.CM_2),
            };
            var section = new ListReportSection("Local Buckling", items);
            return section;
        }

        public static ListReportSection AsReportSection(this FBEgyptCompressionDto dto)
        {
            var items = new List<ReportItem>()
            {
                new ReportItem("Flexural Stress (Fcr)",dto.F.ToString("0.###"),Units.TON_CM_2),
                new ReportItem("Flexural Area (A)",dto.A.ToString("0.###"),Units.CM_2),
                new ReportItem("Nominal Load (Pn)",dto.NominalStrength.ToString("0.###"),Units.TON),

            };
            var section = new ListReportSection("Flexural Buckling", items);
            return section;
        }

        public static ListReportSection AsReportSection(this TFBEgyptCompressionDto dto)
        {
            var items = new List<ReportItem>()
            {
                new ReportItem("Torsional Flexural Stress (Fcr)",dto.F.ToString("0.###"),Units.TON_CM_2),
                new ReportItem("Torsional Flexural Area (A)",dto.A.ToString("0.###"),Units.CM_2),
                new ReportItem("Nominal Load (Pn)",dto.NominalStrength.ToString("0.###"),Units.TON),
            };
            return new ListReportSection("Torsional Fleaxural Buckling", items);
        }

        public static ListReportSection AsReportSection(this TBEgyptCompressionDto dto)
        {
            var items = new List<ReportItem>()
            {
                new ReportItem("Torsional Buckling Stress (Fcrt)",dto.F.ToString("0.###"),Units.TON_CM_2),
                new ReportItem("Torsional  Area (A)",dto.A.ToString("0.###"),Units.CM_2),
                new ReportItem("Nominal Load (Pn)",dto.NominalStrength.ToString("0.###"),Units.TON),
            };
            var section = new ListReportSection("Torsional Buckling", items);
            return section;
        }

        #endregion


    }
}
