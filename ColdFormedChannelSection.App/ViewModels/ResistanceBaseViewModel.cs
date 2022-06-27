using ColdFormedChannelSection.App.ViewModels.Base;
using ColdFormedChannelSection.App.ViewModels.Enums;
using ColdFormedChannelSection.Core.Entities;
using ColdFormedChannelSection.Core.Enums;
using CSharp.Functional.Constructs;
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

        private OutputBase _resistanceOutput;

        private GeneralInfoViewModel _generalInfoVM;

        private BracingConditionsViewModel _bracingConditionsVM;

        private GeometryViewModel _geometryVM;

        private MaterialViewModel _materialVM;

        private InputLoadViewModel _inputLoadVM;

        private bool _isInputLoad;

        private Action<ReportViewModel> _reportService;

        private IReport _report;

        private readonly Func<Func<string, Exceptional<Unit>>, Option<Exceptional<Unit>>> _folderDialogService;

        #endregion

        #region Properties

        protected static Dictionary<UnitSystems, Units> _compDict = new Dictionary<UnitSystems, Units>()
        {
            [UnitSystems.KIPINCH] = Units.KIP,
            [UnitSystems.TONCM] = Units.TON,
            [UnitSystems.NMM] = Units.N
        };

        protected static Dictionary<UnitSystems, Units> _momentDict = new Dictionary<UnitSystems, Units>()
        {
            [UnitSystems.KIPINCH] = Units.KIP_IN,
            [UnitSystems.TONCM] = Units.TON_CM,
            [UnitSystems.NMM] = Units.N_MM
        };

        public IReport Report
        {
            get => _report;
            set => NotifyPropertyChanged(ref _report, value);
        }


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

        public OutputBase ResistanceOutput
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
                                       Action<ReportViewModel> reportService,
                                       Func<Func<string, Exceptional<Unit>>, Option<Exceptional<Unit>>> folderDialogService)
        {
            _folderDialogService = folderDialogService;
            _reportService = reportService;
            ShowErrorsService = showErrorsService;
            InputLoadVM = inputLoadVM;
            GeneralInfoVM = generalInfoVM;
            Report = null;
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
            GeneralInfoVM.Unit = UnitSystems.TONCM;
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
            var report = new ReportViewModel(Report,_folderDialogService);
            _reportService(report);
        }

        private bool CanPrintReport()
        {
            return IsDisplayReport && Report!= null;
        }

        #endregion

    }
}
