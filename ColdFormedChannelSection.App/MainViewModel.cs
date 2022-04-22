using ColdFormedChannelSection.App.ViewModels;
using ColdFormedChannelSection.App.ViewModels.Base;
using ColdFormedChannelSection.App.ViewModels.Enums;
using ColdFormedChannelSection.App.ViewModels.Mediator;
using System;

namespace ColdFormedChannelSection.App
{
    internal class MainViewModel:ViewModelBase
    {

        #region Pivate Fields

        private readonly Lazy<DirectStrengthResistanceViewModel> _directStrengthResistanceVM;
        private readonly Lazy<EgyptianCodeResistanceViewModel> _egyptianCodeResistanceVM;
        private readonly Lazy<EuroCodeReistanceViewModel> _euroCodeReistanceVM;
        private readonly Lazy<AISICodeResistanceViewModel> _aisiCodeResistanceVM;
        private readonly Lazy<AISCCodeResistanceViewModel> _aiscCodeResistanceVM;

        private ViewModelBase _currentVM;

        #endregion

        #region Properties

        public MenuViewModel MenuVM { get;  }

        public ViewModelBase CurrentVM
        {
            get => _currentVM;
            set => NotifyPropertyChanged( ref _currentVM, value );
        }
        #endregion

        #region Constructors

        public MainViewModel()
        {
            MenuVM = new MenuViewModel();
            var generalInfoVM = new GeneralInfoViewModel();
            var bracingConditionsVM = new BracingConditionsViewModel();
            var geometryVM = new GeometryViewModel();
            _directStrengthResistanceVM = new Lazy<DirectStrengthResistanceViewModel>(() =>new DirectStrengthResistanceViewModel(generalInfoVM, bracingConditionsVM, geometryVM));
            _egyptianCodeResistanceVM = new Lazy<EgyptianCodeResistanceViewModel>(() =>new EgyptianCodeResistanceViewModel(generalInfoVM, bracingConditionsVM, geometryVM));
            _euroCodeReistanceVM = new Lazy<EuroCodeReistanceViewModel>(() =>new EuroCodeReistanceViewModel(generalInfoVM, bracingConditionsVM, geometryVM));
            _aisiCodeResistanceVM = new Lazy<AISICodeResistanceViewModel>(() =>new AISICodeResistanceViewModel(generalInfoVM, bracingConditionsVM, geometryVM));
            _aiscCodeResistanceVM = new Lazy<AISCCodeResistanceViewModel>(() =>new AISCCodeResistanceViewModel(generalInfoVM, bracingConditionsVM, geometryVM));
            Mediator.Instance.Subscribe<object>(this, _ => CurrentVM = _directStrengthResistanceVM.Value,Context.ResistanceDirectStrength);
            Mediator.Instance.Subscribe<object>(this, _ => CurrentVM = _egyptianCodeResistanceVM.Value, Context.ResistanceEgyptianCode);
            Mediator.Instance.Subscribe<object>(this,_=>CurrentVM=_euroCodeReistanceVM.Value,Context.ResistanceEuroCode);
            Mediator.Instance.Subscribe<object>(this,_=>CurrentVM=_aisiCodeResistanceVM.Value,Context.ResistanceAISICode);
            Mediator.Instance.Subscribe<object>(this,_=>CurrentVM=_aiscCodeResistanceVM.Value,Context.ResistanceAISCCode);
        }

        #endregion

    }
}
