using ColdFormedChannelSection.App.ViewModels.Base;
using ColdFormedChannelSection.App.ViewModels.Enums;
using System;
using System.Collections.Generic;

namespace ColdFormedChannelSection.App.ViewModels
{
    public class InputLoadViewModel:ViewModelBase
    {
        #region Private Fields

        private double _ultimateLoad;

        private double _ultimateMoment;

        private bool _isUltimateLoad;

        private bool _isUltimateMoment;

        private readonly Dictionary<StrainingActions, Action<InputLoadViewModel>> _displayDict;

        #endregion

        #region Properties

        public double UltimateLoad
        {
            get => _ultimateLoad;
            set=>NotifyPropertyChanged(ref _ultimateLoad, value);
        }

        public double UltimateMoment
        {
            get=> _ultimateMoment;
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
            set=>NotifyPropertyChanged(ref _isUltimateMoment, value);
        }

        #endregion

        #region Constructors

        public InputLoadViewModel()
        {
            _displayDict = new Dictionary<StrainingActions, Action<InputLoadViewModel>>()
            {
                [StrainingActions.COMPRESSION] = DisplayPu,
                [StrainingActions.MOMENT] = DisplayMu
            };
            Mediator.Mediator.Instance.Subscribe<StrainingActions>(this, OnSAChanged, Context.STRAININGACTIONS);
        }

        #endregion

        #region Methods

        private void OnSAChanged(StrainingActions sa)
        {
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
