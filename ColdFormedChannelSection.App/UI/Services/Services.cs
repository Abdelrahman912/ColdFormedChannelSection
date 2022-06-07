using ColdFormedChannelSection.App.Models;
using ColdFormedChannelSection.App.UI.Windows;
using ColdFormedChannelSection.Core.Entities;
using System.Collections.Generic;
using static CSharp.Functional.Functional;
using Unit = System.ValueTuple;

namespace ColdFormedChannelSection.App.UI.Services
{
    public enum ResultMessageType
    {
        DONE,
        ERROR,
        WARNING
    }
    public static class Services
    {
        public static Unit ResultMessagesService(this List<ResultMessage> messages)
        {
            var wnd = new ResultMessagesWindow();
            wnd.DataContext = messages;
            wnd.ShowDialog();
            return Unit();
        }

        public static Unit ReportService(this ReportBase report)
        {
            var wnd = new ReportWindow();
            wnd.DataContext = report;
            wnd.ShowDialog();
            return Unit();
        }
    }
}
