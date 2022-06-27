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

namespace ColdFormedChannelSection.App.ViewModels
{
    public class ReportViewModel:ViewModelBase
    {

        #region Private Fields

        private readonly Func<Func<string, bool>, Option<bool>> _folderDialogService;

        #endregion

        #region Properties

        public IReport Report { get; }

        public ICommand PrintReportCommand { get; }

        #endregion

        #region Constuctors
        public ReportViewModel(IReport report,Func<Func<string,bool>,Option<bool>> folderDialogService)
        {
            _folderDialogService = folderDialogService;
            Report = report;
            PrintReportCommand = new RelayCommand(OnPrintReport);
        }

        #endregion

        #region Methods

        private void OnPrintReport()
        {
            _folderDialogService(Print).Map(b=>MessageBox.Show("done"));
        }

        private bool Print(string fileName)
        {
            return true;
        }

        #endregion

    }
}
