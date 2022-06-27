using ColdFormedChannelSection.App.ViewModels.Base;
using ColdFormedChannelSection.Core.Entities;
using CSharp.Functional.Constructs;
using CSharp.Functional.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using static CSharp.Functional.Extensions.OptionExtension;
using static CSharp.Functional.Extensions.ExceptionalExtension;
using Unit = System.ValueTuple;
using static CSharp.Functional.Functional;

namespace ColdFormedChannelSection.App.ViewModels
{
    public class ReportViewModel:ViewModelBase
    {

        #region Private Fields

        private readonly Func<Func<string, Exceptional<Unit>>, Option<Exceptional<Unit>>> _folderDialogService;

        #endregion

        #region Properties

        public IReport Report { get; }

        public ICommand PrintReportCommand { get; }

        #endregion

        #region Constuctors
        public ReportViewModel(IReport report,Func<Func<string, Exceptional<Unit>>, Option<Exceptional<Unit>>> folderDialogService)
        {
            _folderDialogService = folderDialogService;
            Report = report;
            PrintReportCommand = new RelayCommand(OnPrintReport);
        }

        #endregion

        #region Methods

        private void OnPrintReport()
        {
            _folderDialogService(fileName =>
            {
            var x = from exp in Report.CreatePdf(fileName)
                    select exp;
            var result = x.Match(e => MessageBox.Show(e.Message), _ => MessageBox.Show("Pdf created successfully!"));
                return Exceptional(Unit());
           
            });
        }
       

        #endregion

    }
}
