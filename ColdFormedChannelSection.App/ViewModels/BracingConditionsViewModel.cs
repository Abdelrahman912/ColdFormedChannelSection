using ColdFormedChannelSection.App.ViewModels.Base;
using ColdFormedChannelSection.App.ViewModels.Enums;
using ColdFormedChannelSection.Core.Enums;
using CSharp.Functional.Errors;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using static ColdFormedChannelSection.Core.Errors.Errors;

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
        private double _cm;

        private bool _isLuUsed;
        private bool _isCbUsed;
        private bool _isC1Used;
        private bool _isCmUsed;

        private UnitSystems _unit;

        private Dictionary<KeyValuePair<DesignCode, StrainingActions>, Action> _bracingDict;

        private Task _initTask;

        #endregion

        #region Properties


        public double Cm
        {
            get => _cm;
            set => NotifyPropertyChanged(ref _cm, value);
        }

        public UnitSystems Unit
        {
            get => _unit;
            set => NotifyPropertyChanged(ref _unit,value);
        }

        public bool IsCmUsed
        {
            get => _isCmUsed;
            set => NotifyPropertyChanged(ref _isCmUsed, value);
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
            Cb = 1;
            C1 = 1;
            Kx = 1;
            Ky = 1;
            Kz = 1;

            IsCbUsed = false;
            IsLuUsed = false;
            IsC1Used = false;
           
           _initTask = init();
        }




        #endregion

        #region Methods

        private  Task init()
        {
           return Task.Run(() =>
            {
                Mediator.Mediator.Instance.Subscribe<UnitSystems>(this, OnUnitsChanged, Context.UNITS);
                Mediator.Mediator.Instance.Subscribe<KeyValuePair<DesignCode, StrainingActions>>(this, OnBracingChanged, Context.BRACING);
                _bracingDict = new Dictionary<KeyValuePair<DesignCode, StrainingActions>, Action>()
                {
                    [KeyValuePair.Create(DesignCode.EGYPTIAN, StrainingActions.COMPRESSION)] = DefaultCompression,
                    [KeyValuePair.Create(DesignCode.EGYPTIAN,StrainingActions.MOMENT)]=DefaultMoment,
                    [KeyValuePair.Create(DesignCode.EGYPTIAN, StrainingActions.MOMENT_COMPRESSION)] = DefaultMoment,
                    [KeyValuePair.Create(DesignCode.EURO,StrainingActions.COMPRESSION)]=DefaultCompression,
                    [KeyValuePair.Create(DesignCode.EURO,StrainingActions.MOMENT)] = EuroMoment,
                    [KeyValuePair.Create(DesignCode.EURO, StrainingActions.MOMENT_COMPRESSION)] = EuroMoment,
                    [KeyValuePair.Create(DesignCode.AISI,StrainingActions.COMPRESSION)] = DefaultCompression,
                    [KeyValuePair.Create(DesignCode.AISI,StrainingActions.MOMENT)] = DefaultMoment,
                    [KeyValuePair.Create(DesignCode.AISI, StrainingActions.MOMENT_COMPRESSION)] = AISIMomentCompression
                };
            });
        }

       

        private async void OnBracingChanged(KeyValuePair<DesignCode, StrainingActions> kvp)
        {
            await _initTask;
            _bracingDict[kvp]();
        }

        public List<Error> Validate()
        {
            var errs = new List<Error>();
            if (Lx <= 0)
                errs.Add(LessThanZeroError("Lx"));
            if (Kx <= 0)
                errs.Add(LessThanZeroError("Kx"));
            if (Ly <= 0)
                errs.Add(LessThanZeroError("Ly"));
            if (Ky <= 0)
                errs.Add(LessThanZeroError("Ky"));
            if (Lz <= 0)
                errs.Add(LessThanZeroError("Lz"));
            if (Kz <= 0)
                errs.Add(LessThanZeroError("Kz"));
            if (IsCbUsed && Cb <= 0)
                errs.Add(LessThanZeroError("Cb"));
            if (IsLuUsed && Lu <= 0)
                errs.Add(LessThanZeroError("Lu"));
            if (IsC1Used && C1 <= 0)
                errs.Add(LessThanZeroError("C1"));
            if (IsCmUsed && Cm <= 0)
                errs.Add(LessThanZeroError("Cm"));
            return errs;
        }

        private void AISIMomentCompression()
        {
            IsCbUsed = true;
            IsLuUsed = true;
            IsC1Used = false;
            IsCmUsed = true;
            C1 = 0;
        }

        private  void DefaultCompression()
        {
            IsCbUsed = false;
            Cb = 0;
            IsLuUsed = false;
            Lu = 0;
            IsC1Used = false;
            C1 = 0;
            IsCmUsed = false;
            Cm = 0;
        }

        private void DefaultMoment()
        {
            IsCbUsed = true;
            Cb = 1;
            IsLuUsed = true;
            IsC1Used = false;
            IsCmUsed = false;
            Cm = 0;
            C1 = 0;
        }

       private void EuroMoment()
        {
            IsC1Used = true;
            C1 = 1;
            IsLuUsed = true;
            IsCbUsed = true;
            Cb = 1;
        }

        private void OnUnitsChanged(UnitSystems unit)
        {
            Unit = unit;
        }


        #endregion

    }
}
