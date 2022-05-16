using ColdFormedChannelSection.App.ViewModels.Base;
using ColdFormedChannelSection.App.ViewModels.Enums;
using ColdFormedChannelSection.Core.Enums;
using System;

namespace ColdFormedChannelSection.App.ViewModels
{
    public class BracingConditionsViewModel:ViewModelBase
    {

        #region Private Fields

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

        private Units _unit;

        #endregion

        #region Properties

        public Units Unit
        {
            get => _unit;
            set => NotifyPropertyChanged(ref _unit,value);
        }

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

        #endregion

        #region Constructors

        public BracingConditionsViewModel()
        {
            Lx = 0;
            Ly = 0;
            Lz= 0;
            Cb = 0;
            C1 = 0;
            Kx = 0;
            Ky = 0;
            Kz = 0;

            IsCbUsed = true;
            IsLuUsed = true;
            IsC1Used = true;
            Mediator.Mediator.Instance.Subscribe<Units>(this, OnUnitsChanged, Context.UNITS);
        }


        #endregion

        #region Methods

        private void OnUnitsChanged(Units unit)
        {
            Unit = unit;
        }


        #endregion

    }
}
