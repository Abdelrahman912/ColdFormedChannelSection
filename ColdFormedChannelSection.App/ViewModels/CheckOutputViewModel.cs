using ColdFormedChannelSection.App.ViewModels.Base;
using ColdFormedChannelSection.App.ViewModels.Enums;
using ColdFormedChannelSection.Core.Entities;
using ColdFormedChannelSection.Core.Enums;

namespace ColdFormedChannelSection.App.ViewModels
{
    public class CheckOutputViewModel:ViewModelBase
    {
        #region Private Fields

        private double _ultimateLoad;

        private CheckOutput _checkOutput;

        private UnitSystems _unit;

        private StrainingActions _strainingActions;

        #endregion

        #region Properties

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

        public CheckOutputViewModel()
        {
            UltimateLoad = 0;
            CheckOutput = null;
            Mediator.Mediator.Instance.Subscribe<UnitSystems>(this, (unit) => Unit = unit, Context.UNITS);
            Mediator.Mediator.Instance.Subscribe<StrainingActions>(this, (sa) => StrainingActions = sa, Context.STRAININGACTIONS);
        }

        #endregion



    }
}
