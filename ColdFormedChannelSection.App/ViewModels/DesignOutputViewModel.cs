using ColdFormedChannelSection.App.ViewModels.Base;
using ColdFormedChannelSection.App.ViewModels.Enums;
using ColdFormedChannelSection.Core.Entities;
using ColdFormedChannelSection.Core.Enums;

namespace ColdFormedChannelSection.App.ViewModels
{
    public class DesignOutputViewModel:ViewModelBase
    {

        #region Private Fields

        private bool _isDesignOutput;

        private double _ultimateLoad;

        private CheckOutput _checkOutput;

        private UnitSystems _unit;

        private StrainingActions _strainingActions;

        private DesignOutput _designOutput;

        #endregion

        #region Properties

        public DesignOutput DesignOutput
        {
            get => _designOutput;
            set => NotifyPropertyChanged(ref _designOutput, value);
        }

        public bool IsDesignOutput
        {
            get => _isDesignOutput;
            set => NotifyPropertyChanged(ref _isDesignOutput, value);
        }


        public UnitSystems Unit
        {
            get => _unit;
            set => NotifyPropertyChanged(ref _unit, value);
        }

        public StrainingActions StrainingActions
        {
            get => _strainingActions;
            set => NotifyPropertyChanged(ref _strainingActions, value);
        }

        public CheckOutput CheckOutput
        {
            get => _checkOutput;
            set => NotifyPropertyChanged(ref _checkOutput, value);
        }

        public double UltimateLoad
        {
            get => _ultimateLoad;
            set => NotifyPropertyChanged(ref _ultimateLoad, value);
        }

        #endregion

        #region Constructors

        public DesignOutputViewModel()
        {
            IsDesignOutput = false;
            UltimateLoad = 0;
            CheckOutput = null;
            Mediator.Mediator.Instance.Subscribe<UnitSystems>(this, (unit) => Unit = unit, Context.UNITS);
            Mediator.Mediator.Instance.Subscribe<StrainingActions>(this, (sa) => StrainingActions = sa, Context.STRAININGACTIONS);
        }

        #endregion

        #region Methods



        #endregion

    }
}
