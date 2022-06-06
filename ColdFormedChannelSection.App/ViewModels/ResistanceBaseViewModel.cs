using ColdFormedChannelSection.App.ViewModels.Base;
using ColdFormedChannelSection.App.ViewModels.Enums;
using ColdFormedChannelSection.Core.Entities;
using ColdFormedChannelSection.Core.Enums;
using CSharp.Functional.Errors;
using System;
using System.Collections.Generic;
using System.Windows.Input;
using Unit = System.ValueTuple;

namespace ColdFormedChannelSection.App.ViewModels
{
    public abstract class ResistanceBaseViewModel : ViewModelBase
    {

        #region Private Fields

        private bool _isDisplayReport;

        private bool _isResistanceOutput;

        private IOutput _resistanceOutput;

        private GeneralInfoViewModel _generalInfoVM;

        private BracingConditionsViewModel _bracingConditionsVM;

        private GeometryViewModel _geometryVM;

        private MaterialViewModel _materialVM;

        private InputLoadViewModel _inputLoadVM;

        private bool _isInputLoad;

        private Action<IReport> _reportService;

        #endregion

        #region Properties


        public Func<List<Error>,Unit> ShowErrorsService { get; }

        public InputLoadViewModel InputLoadVM
        {
            get => _inputLoadVM;
            set => NotifyPropertyChanged(ref _inputLoadVM, value);
        }

        public bool IsInputLoad
        {
            get => _isInputLoad;
            set=>NotifyPropertyChanged(ref _isInputLoad, value);
        }


        public bool IsDisplayReport
        {
            get => _isDisplayReport;
            set=>NotifyPropertyChanged(ref _isDisplayReport, value);
        }

        public bool IsResistanceOutput
        {
            get => _isResistanceOutput;
            set => NotifyPropertyChanged(ref _isResistanceOutput, value);
        }

        public IOutput ResistanceOutput
        {
            get => _resistanceOutput;
            set => NotifyPropertyChanged(ref _resistanceOutput, value);
        }


        public GeneralInfoViewModel GeneralInfoVM
        {
            get => _generalInfoVM;
            set => NotifyPropertyChanged(ref _generalInfoVM, value);
        }

        public BracingConditionsViewModel BracingConditionsVM
        {
            get => _bracingConditionsVM;
            set => NotifyPropertyChanged(ref _bracingConditionsVM, value);
        }

        public GeometryViewModel GeometryVM
        {
            get => _geometryVM;
            set=>NotifyPropertyChanged(ref _geometryVM, value);
        }

        public MaterialViewModel MaterialVM
        {
            get => _materialVM;
            set => NotifyPropertyChanged(ref _materialVM, value);
        }

        public virtual ICommand ResultsCommand { get; }

        public virtual ICommand PrintReportCommand { get; }

        #endregion

        #region Constructors

        public ResistanceBaseViewModel(GeneralInfoViewModel generalInfoVM,
                                       BracingConditionsViewModel bracingConditionsVM,
                                       GeometryViewModel geometryVM,
                                       MaterialViewModel materialVM,
                                       InputLoadViewModel inputLoadVM,
                                       Func<List<Error>,Unit> showErrorsService,
                                       Action<IReport> reportService)
        {
            _reportService = reportService;
            ShowErrorsService = showErrorsService;
            InputLoadVM = inputLoadVM;
            GeneralInfoVM = generalInfoVM;
            GeneralInfoVM.OnRunningModuleChange = () =>
            {
                if (GeneralInfoVM.RunningModule == Module.RESISTANCE)
                {
                    IsInputLoad = false;
                    InputLoadVM.IsUltimateLoad = false;
                    InputLoadVM.IsUltimateMoment = false;
                }
                else
                    IsInputLoad = true;
            };
            GeneralInfoVM.RunningModule = Module.RESISTANCE;
            GeneralInfoVM.StrainingAction = StrainingActions.COMPRESSION;
            GeneralInfoVM.Unit = Units.TONCM;
            BracingConditionsVM = bracingConditionsVM;
            GeometryVM = geometryVM;
            MaterialVM = materialVM;
            IsInputLoad = false;
            IsDisplayReport = false;
            PrintReportCommand = new RelayCommand(OnPrintReport, CanPrintReport);
        }



        #endregion

        #region Methods

        private void OnPrintReport()
        {
            _reportService(null);
        }

        private bool CanPrintReport()
        {
            return IsDisplayReport;
        }

        #endregion

    }
}
