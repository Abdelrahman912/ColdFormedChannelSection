using ColdFormedChannelSection.Core.Entities;
using ColdFormedChannelSection.Core.Extensions;
using EasyPDF;
using iText.Layout;
using iText.Layout.Element;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ColdFormedChannelSection.Core.Helpers
{
    public static class PdfHelper
    {
        public static void AddToPdf(this ListReportSection listSection,Document doc)
        {
            listSection.SectionName.AsText().AddToDocument(doc);
            var headers = new List<string>() { "Name", "Value", "Unit" };
            var table = new Table(3, false);
            headers.AsHeaderRow().AddToTable(table);
            listSection.Items.ForEach(item =>
            {
                item.Name.AsLeftTextCell().AddToTable(table);
                item.Value.AsCenterTextCell().AddToTable(table);
                item.Unit.GetDescription().AsCenterTextCell().AddToTable(table);
            });
            table.AddToDocument(doc);
        }


    }
}
