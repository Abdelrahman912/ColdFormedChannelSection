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
using System.Windows.Controls;

namespace ColdFormedChannelSection.App.ViewModels
{
    public class ReportViewModel:ViewModelBase
    {


        #region Properties

        public IReport Report { get; }

        #endregion

        #region Constuctors
        public ReportViewModel(IReport report)
        {
           
            Report = report;
        }

        #endregion

        #region Methods

       
       

        #endregion

    }
}
