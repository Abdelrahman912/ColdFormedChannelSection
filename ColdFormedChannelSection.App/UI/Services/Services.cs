using ColdFormedChannelSection.App.Models;
using ColdFormedChannelSection.App.UI.Windows;
using ColdFormedChannelSection.App.ViewModels;
using ColdFormedChannelSection.Core.Entities;
using CSharp.Functional.Constructs;
using System;
using System.Collections.Generic;
using static CSharp.Functional.Functional;
using Unit = System.ValueTuple;
using static CSharp.Functional.Extensions.OptionExtension;

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

        public static Unit ReportService(this ReportViewModel report)
        {
            var wnd = new ReportWindow();
            wnd.DataContext = report;
            wnd.ShowDialog();
            return Unit();
        }

        public static Option<T> FolderDialogService<T>(Func<string, T> onOk)
        {
            var outputDialog = new Microsoft.Win32.SaveFileDialog();

            outputDialog.DefaultExt = ".pdf";
            outputDialog.Filter = "PDF documents (.pdf)|*.pdf";
           var result =  outputDialog.ShowDialog();
            if (result == true)
                return Some(onOk(outputDialog.FileName));
            else
                return None;
        }
    }
}
