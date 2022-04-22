﻿using ColdFormedChannelSection.App.ViewModels.Base;
using ColdFormedChannelSection.App.ViewModels.Enums;
using ColdFormedChannelSection.Core.Enums;

namespace ColdFormedChannelSection.App.ViewModels
{
    internal class GeneralInfoViewModel:ViewModelBase
    {

        #region Private Fields

        private StrainingActions _strainingAction;
        private bool _isUnstiffened;
        private Units _unit;
        private double _fy;
        private double _e;

        #endregion

        #region Properties

        public StrainingActions StrainingAction
        {
            get => _strainingAction;
            set
            {
                NotifyPropertyChanged(ref _strainingAction, value);
                //_isUsedParamsAction(_strainingAction);
            }
        }

        public bool IsUnstiffened
        {
            get => _isUnstiffened;
            set => NotifyPropertyChanged(ref _isUnstiffened, value);
        }

        public Units Unit
        {
            get => _unit;
            set => NotifyPropertyChanged(ref _unit, value);
        }

        public double Fy
        {
            get => _fy;
            set => NotifyPropertyChanged(ref _fy, value);
        }

        public double E
        {
            get => _e;
            set => NotifyPropertyChanged(ref _e, value);
        }

        #endregion

        #region Constructors
        public GeneralInfoViewModel()
        {
            StrainingAction = StrainingActions.MOMENT;
            Unit = Units.TONCM;
            IsUnstiffened = true;
            Fy = 0;
            E = 0;
        }
        #endregion


    }
}