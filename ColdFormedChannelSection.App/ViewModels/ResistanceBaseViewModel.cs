using ColdFormedChannelSection.App.ViewModels.Base;
using ColdFormedChannelSection.App.ViewModels.Enums;
using ColdFormedChannelSection.Core.Entities;
using ColdFormedChannelSection.Core.Enums;
using ColdFormedChannelSection.Core.Helpers;
using System;
using System.Windows.Input;

namespace ColdFormedChannelSection.App.ViewModels
{
    internal abstract class ResistanceBaseViewModel : ViewModelBase
    {

        #region Private Fields

        protected event Action<StrainingActions> _isUsedParamsAction;
        protected  Action<Section> _onResults = _ => { };

        private StrainingActions _strainingAction;
        private bool _isUnstiffened;
        private Units _unit;
        private double _fy;
        private double _e;
        private double _totalHeightH;
        private double _totalWidthB;
        private double _internalRadiusR;
        private double _thicknessT;
        private double _totalFoldWidthC;
        private double _lx;
        private double _ly;
        private double _lz;
        private double _kx;
        private double _ky;
        private double _kz;
        private double _lu;
        private double _cb;
        private double _c1;

        private bool _isLuUsed;
        private bool _isCbUsed;
        private bool _isC1Used;

        #endregion

        #region Properties

        public bool IsLuUsed
        {
            get => _isLuUsed;
            set => NotifyPropertyChanged(ref _isLuUsed, value);
        }

        public bool IsCbUsed
        {
            get => _isCbUsed;
            set => NotifyPropertyChanged(ref _isCbUsed, value);
        }

        public bool IsC1Used
        {
            get => _isC1Used;
            set => NotifyPropertyChanged(ref _isC1Used, value);
        }

        public double Lx
        {
            get => _lx;
            set => NotifyPropertyChanged(ref _lx, value);
        }

        public double Ly
        {
            get => _ly;
            set => NotifyPropertyChanged(ref _ly, value);
        }

        public double Lz
        {
            get => _lz;
            set => NotifyPropertyChanged(ref _lz, value);
        }

        public double Kx
        {
            get => _kx;
            set => NotifyPropertyChanged(ref _kx, value);
        }

        public double Ky
        {
            get => _ky;
            set => NotifyPropertyChanged(ref _ky, value);
        }

        public double Kz
        {
            get => _kz;
            set => NotifyPropertyChanged(ref _kz, value);
        }

        public double Lu
        {
            get => _lu;
            set => NotifyPropertyChanged(ref _lu, value);
        }

        public double Cb
        {
            get => _cb;
            set => NotifyPropertyChanged(ref _cb, value);
        }

        public double C1
        {
            get => _c1;
            set => NotifyPropertyChanged(ref _c1, value);
        }

        public double TotalFoldWidthC
        {
            get => _totalFoldWidthC;
            set => NotifyPropertyChanged(ref _totalFoldWidthC, value);
        }

        public double ThicknessT
        {
            get => _thicknessT;
            set => NotifyPropertyChanged(ref _thicknessT, value);
        }

        public double InternalRadiusR
        {
            get => _internalRadiusR;
            set => NotifyPropertyChanged(ref _internalRadiusR, value);
        }

        public double TotalWidthB
        {
            get => _totalWidthB;
            set => NotifyPropertyChanged(ref _totalWidthB, value);
        }

        public double TotalHeightH
        {
            get => _totalHeightH;
            set => NotifyPropertyChanged(ref _totalHeightH, value);
        }

        public StrainingActions StrainingAction
        {
            get => _strainingAction;
            set
            {
                NotifyPropertyChanged(ref _strainingAction, value);
                _isUsedParamsAction(_strainingAction);
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

        public string Name { get; }

        public virtual ICommand ResultsCommand { get; }

        #endregion

        #region Constructors

        public ResistanceBaseViewModel(string name)
        {
            
            _isUsedParamsAction = (sa) =>
            {
                switch (sa)
                {
                    case StrainingActions.MOMENT:
                        IsCbUsed = true;
                        IsLuUsed = true;
                        break;
                    case StrainingActions.COMPRESSION:
                    default:
                        IsCbUsed = false;
                        IsLuUsed = false;
                        break;
                }
            };
            
            Name = name;
            IsUnstiffened = true;
            Unit = Units.TONCM;
            IsC1Used = false;
            IsLuUsed = false;
            IsC1Used = false;
            StrainingAction = StrainingActions.MOMENT;
            
        }



        #endregion


        #region Methods

      

       

        #endregion
    }
}
