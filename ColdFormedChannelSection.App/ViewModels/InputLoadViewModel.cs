using ColdFormedChannelSection.App.ViewModels.Base;
using ColdFormedChannelSection.App.ViewModels.Enums;
using ColdFormedChannelSection.Core.Enums;
using CSharp.Functional.Errors;
using System;
using System.Collections.Generic;
using static ColdFormedChannelSection.Core.Errors.Errors;

namespace ColdFormedChannelSection.App.ViewModels
{
    public class InputLoadViewModel : ViewModelBase
    {
        #region Private Fields

        private double _ultimateLoad;

        private double _ultimateMoment;

        private bool _isUltimateLoad;

        private bool _isUltimateMoment;

        private readonly Dictionary<StrainingActions, Action<InputLoadViewModel>> _displayDict;

        private Units _unit;

        #endregion

        #region Properties

        public Units Unit
        {
            get => _unit;
            set => NotifyPropertyChanged(ref _unit, value);
        }

        public double UltimateLoad
        {
            get => _ultimateLoad;
            set => NotifyPropertyChanged(ref _ultimateLoad, value);
        }

        public double UltimateMoment
        {
            get => _ultimateMoment;
            set => NotifyPropertyChanged(ref _ultimateMoment, value);
        }

        public bool IsUltimateLoad
        {
            get => _isUltimateLoad;
            set => NotifyPropertyChanged(ref _isUltimateLoad, value);
        }

        public bool IsUltimateMoment
        {
            get => _isUltimateMoment;
            set => NotifyPropertyChanged(ref _isUltimateMoment, value);
        }

        #endregion

        #region Constructors

        public InputLoadViewModel()
        {
            _displayDict = new Dictionary<StrainingActions, Action<InputLoadViewModel>>()
            {
                [StrainingActions.COMPRESSION] = DisplayPu,
                [StrainingActions.MOMENT] = DisplayMu,
                [StrainingActions.MOMENT_COMPRESSION] = DisplayMuPU,
            };
            Mediator.Mediator.Instance.Subscribe<Tuple<Module,StrainingActions>>(this, OnSAChanged, Context.SA_MODULE);
            Mediator.Mediator.Instance.Subscribe<Units>(this, OnUnitChanged, Context.UNITS);
            IsUltimateLoad = false;
            IsUltimateMoment = false;

        }


        #endregion

        #region Methods

        private void DisplayMuPU(InputLoadViewModel obj)
        {

            IsUltimateLoad = true;
            IsUltimateMoment = true;

        }

        public List<Error> Validate()
        {
            var errs = new List<Error>();
            if (IsUltimateLoad && UltimateLoad <= 0)
                errs.Add(LessThanZeroError("Pu"));
            if (IsUltimateMoment && UltimateMoment <= 0)
                errs.Add(LessThanZeroError("Mu"));
            return errs;

        }

        private void OnUnitChanged(Units unit)
        {
            Unit = unit;
        }

        private void OnSAChanged(Tuple<Module  , StrainingActions > tuple)
        {
            (var module, var sa) = tuple;
            if(module != Module.RESISTANCE)
                     _displayDict[sa](this);
        }

        private static void DisplayPu(InputLoadViewModel vm)
        {
            vm.IsUltimateLoad = true;
            vm.IsUltimateMoment = false;
            vm.UltimateMoment = 0;
        }

        private static void DisplayMu(InputLoadViewModel vm)
        {
            vm.IsUltimateMoment = true;
            vm.IsUltimateLoad = false;
            vm.UltimateLoad = 0;
        }

        #endregion
    }
}
