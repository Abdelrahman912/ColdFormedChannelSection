using ColdFormedChannelSection.App.ViewModels.Base;
using ColdFormedChannelSection.App.ViewModels.Enums;
using ColdFormedChannelSection.Core.Entities;
using ColdFormedChannelSection.Core.Enums;
using System;
using System.Collections.Generic;
using System.Windows.Input;

namespace ColdFormedChannelSection.App.ViewModels
{
    public abstract class ResistanceBaseViewModel : ViewModelBase
    {

        #region Private Fields

        private bool _isDisplayReport;

        private bool _isResistanceOutput;

        private ResistanceOutput _resistanceOutput;

        private GeneralInfoViewModel _generalInfoVM;

        private BracingConditionsViewModel _bracingConditionsVM;

        private GeometryViewModel _geometryVM;

        private MaterialViewModel _materialVM;

        private InputLoadViewModel _inputLoadVM;

        private double _ultimateLoad;

        private double _ultimateMoment;

        private bool _isInputLoad;

        #endregion

        #region Properties

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

       
        public double UltimateMoment
        {
            get => _ultimateMoment;
            set => NotifyPropertyChanged(ref _ultimateMoment, value);
        }

        public double UltimateLoad
        {
            get => _ultimateLoad;
            set => NotifyPropertyChanged(ref _ultimateLoad, value);
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

        public ResistanceOutput ResistanceOutput
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

        #endregion

        #region Constructors

        public ResistanceBaseViewModel(GeneralInfoViewModel generalInfoVM,
                                       BracingConditionsViewModel bracingConditionsVM,
                                       GeometryViewModel geometryVM,
                                       MaterialViewModel materialVM,
                                       InputLoadViewModel inputLoadVM)
        {

            InputLoadVM = inputLoadVM;
            GeneralInfoVM = generalInfoVM;
            GeneralInfoVM.OnRunningModuleChange = () =>
            {
                if (GeneralInfoVM.RunningModule == Module.RESISTANCE)
                    IsInputLoad = false;
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
        }



        #endregion



    }
}
