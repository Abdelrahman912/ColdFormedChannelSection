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
        private readonly GeneralInfoViewModel _generalInfoVM;
        private readonly BracingConditionsViewModel _bracingConditionsVM;
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
            _generalInfoVM = new GeneralInfoViewModel();
            _bracingConditionsVM = new BracingConditionsViewModel();
            var geometryVM = new GeometryViewModel();
            _directStrengthResistanceVM = new Lazy<DirectStrengthResistanceViewModel>(() =>new DirectStrengthResistanceViewModel(_generalInfoVM, _bracingConditionsVM, geometryVM));
            _egyptianCodeResistanceVM = new Lazy<EgyptianCodeResistanceViewModel>(() =>new EgyptianCodeResistanceViewModel(_generalInfoVM, _bracingConditionsVM, geometryVM));
            _euroCodeReistanceVM = new Lazy<EuroCodeReistanceViewModel>(() =>new EuroCodeReistanceViewModel(_generalInfoVM, _bracingConditionsVM, geometryVM));
            _aisiCodeResistanceVM = new Lazy<AISICodeResistanceViewModel>(() =>new AISICodeResistanceViewModel(_generalInfoVM, _bracingConditionsVM, geometryVM));
            _aiscCodeResistanceVM = new Lazy<AISCCodeResistanceViewModel>(() =>new AISCCodeResistanceViewModel(_generalInfoVM, _bracingConditionsVM, geometryVM));
            Mediator.Instance.Subscribe<object>(this,  _=> OnDefaultResistance(_,_directStrengthResistanceVM.Value),Context.RESISTANCE_DIRECT_STRENGTH);
            Mediator.Instance.Subscribe<object>(this, _ => OnDefaultResistance(_, _egyptianCodeResistanceVM.Value), Context.RESISTANCE_EGYPT_CODE);
            Mediator.Instance.Subscribe<object>(this,OnEuroResistance,Context.RESISTANCE_EURO_CODE);
            Mediator.Instance.Subscribe<object>(this, _ => OnDefaultResistance(_, _aisiCodeResistanceVM.Value), Context.RESISTANCE_AISI_CODE);
            Mediator.Instance.Subscribe<object>(this, _ => OnDefaultResistance(_, _aiscCodeResistanceVM.Value), Context.RESISTANCE_AISC_CODE);
        }


        #endregion

        #region Methods


        private void OnDefaultResistance(object _ , ResistanceBaseViewModel vm)
        {
            CurrentVM = vm;
            DefaultChangeBracingConditions();
        }
      

        private void OnEuroResistance(object _)
        {
            CurrentVM = _euroCodeReistanceVM.Value;
            _generalInfoVM.OnStrainingActionsChange = () =>
            {
                switch (_generalInfoVM.StrainingAction)
                {
                    case StrainingActions.MOMENT:
                        _bracingConditionsVM.IsC1Used = true;
                        _bracingConditionsVM.IsLuUsed = true;
                        _bracingConditionsVM.IsCbUsed = true;
                        break;
                    case StrainingActions.COMPRESSION:
                        _bracingConditionsVM.IsC1Used = false;
                        _bracingConditionsVM.C1 = 0;
                        _bracingConditionsVM.IsLuUsed = false;
                        _bracingConditionsVM.Lu = 0;
                        _bracingConditionsVM.IsCbUsed = false;
                        _bracingConditionsVM.Cb = 0;
                        break;
                }
            };
        }

        private void DefaultChangeBracingConditions()
        {
            _generalInfoVM.OnStrainingActionsChange = () =>
            {
                switch (_generalInfoVM.StrainingAction)
                {
                    case StrainingActions.MOMENT:
                        _bracingConditionsVM.IsC1Used = false;
                        _bracingConditionsVM.C1 = 0;
                        _bracingConditionsVM.IsLuUsed = true;
                        _bracingConditionsVM.IsCbUsed = true;
                        break;
                    case StrainingActions.COMPRESSION:
                        _bracingConditionsVM.IsC1Used = false;
                        _bracingConditionsVM.C1 = 0;
                        _bracingConditionsVM.IsLuUsed = false;
                        _bracingConditionsVM.Lu = 0;
                        _bracingConditionsVM.IsCbUsed = false;
                        _bracingConditionsVM.Cb = 0;
                        break;
                }
            };
        }

        #endregion

    }
}
