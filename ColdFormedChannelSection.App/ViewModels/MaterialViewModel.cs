using ColdFormedChannelSection.App.ViewModels.Base;
using ColdFormedChannelSection.App.ViewModels.Enums;
using ColdFormedChannelSection.Core.Enums;
using CSharp.Functional.Errors;
using static ColdFormedChannelSection.Core.Errors.Errors;
using System;
using CSharp.Functional.Constructs;
using CSharp.Functional.Extensions;
using Unit = System.ValueTuple;
using System.Collections.Generic;
using static CSharp.Functional.Extensions.ValidationExtension;
using static CSharp.Functional.Functional;

namespace ColdFormedChannelSection.App.ViewModels
{
    public class MaterialViewModel : ViewModelBase
    {

        #region Private Fields

        private double _fy;
        private double _e;
        private double _v;
        private double _g;
        private UnitSystems _unit;
        #endregion

        #region Properties

        public UnitSystems Unit
        {
            get => _unit;
            set => NotifyPropertyChanged(ref _unit, value);
        }

        public double G
        {
            get => _g;
            set => NotifyPropertyChanged(ref _g, value);
        }

        public double V
        {
            get => _v;
            set
            {
                NotifyPropertyChanged(ref _v, value);
                G = E / (2 * (1 + V));
            }
        }

        public double Fy
        {
            get => _fy;
            set
            {
                NotifyPropertyChanged(ref _fy, value);
                G = E / (2 * (1 + V));
            }
        }

        public double E
        {
            get => _e;
            set
            {
                NotifyPropertyChanged(ref _e, value);
                G = E / (2 * (1 + V));
            }
        }

        #endregion

        #region Constructors

        public MaterialViewModel()
        {
            V = 0.3;
            Fy = 0;
            E = 0;
            Mediator.Mediator.Instance.Subscribe<UnitSystems>(this, OnUnitsChanged, Context.UNITS);
        }



        #endregion

        #region Methods

        private void OnUnitsChanged(UnitSystems selectedUnit)
        {
            Unit = selectedUnit;
        }

        public List<Error> Validate()
        {
            var errs = new List<Error>();
            if (Fy <= 0)
                errs.Add(LessThanZeroError("Fy"));
            if (E <= 0)
                errs.Add(LessThanZeroError("E"));
            if (V <= 0)
                errs.Add(LessThanZeroError("v"));
            return errs;
        }

        #endregion

    }
}
