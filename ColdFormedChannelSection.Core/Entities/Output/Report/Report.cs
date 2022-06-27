using ColdFormedChannelSection.Core.Enums;
using CSharp.Functional.Constructs;
using EasyPDF;
using iText.Layout;
using System.Collections.Generic;
using System.Linq;
using Unit = System.ValueTuple;
using static CSharp.Functional.Functional;

namespace ColdFormedChannelSection.Core.Entities
{
    public class Report : IReport
    {

        #region Properties

        public UnitSystems UnitSystem { get; }

        public string Name { get; }

        public List<IReportSection> Sections { get; }

        #endregion

        #region Constructors

        public Report(UnitSystems unitSystem, string name, List<IReportSection> sections)
        {
            UnitSystem = unitSystem;
            Name = name;
            Sections = sections;
        }

        #endregion


        #region Methods

        public IReport Convert(UnitSystems target)
        {
            if (UnitSystem == target)
                return this;
            var newSections = Sections.Select(sec => sec.Convert(UnitSystem, target)).ToList();
            return new Report(target, Name, newSections);
        }

        public Exceptional<Unit> CreatePdf(string fileName)
        {
            try
            {
               return fileName.WritePdf((pdf, doc) =>
                {
                    Sections.ForEach(section => section.AddToPdf(doc));
                    pdf.AddPageNumberAsFooter(doc);
                    return Unit();
                });
            }
            catch (System.Exception e)
            {
                return e;
               
            }
        }

       

        #endregion


    }
}
