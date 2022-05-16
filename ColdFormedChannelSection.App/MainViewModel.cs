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
        //private readonly Lazy<AISCCodeResistanceViewModel> _aiscCodeResistanceVM;

        private readonly Lazy<DirectStrengthCheckViewModel> _directStrengthCheckVM;
        private readonly Lazy<EgyptianCodeCheckViewModel> _egyptianCodeCheckVM;
        private readonly Lazy<EuroCodeCheckViewModel> _euroCodeCheckVM;
        private readonly Lazy<AISICodeCheckViewModel> _aisiCodeCheckVM;


        private readonly Lazy<DirectStrengthDesignViewModel> _directStrengthDesignVM;
        private readonly Lazy<EgyptianCodeDesignViewModel> _egyptianCodeDesignVM;
        private readonly Lazy<EuroCodeDesignViewModel> _euroCodeDesignVM;
        private readonly Lazy<AISICodeDesignViewModel> _aisiCodeDesignVM;

        private ViewModelBase _currentVM;
        private readonly GeneralInfoViewModel _generalInfoVM;
        private readonly BracingConditionsViewModel _bracingConditionsVM;
        private readonly AboutViewModel _aboutVM;
        private readonly HomeViewModel _homeVM;

        private bool _isMenu;
        #endregion

        #region Properties

        public bool IsMenu
        {
            get => _isMenu;
            set => NotifyPropertyChanged(ref _isMenu, value);
        }

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
            _aboutVM = new AboutViewModel();
            _bracingConditionsVM = new BracingConditionsViewModel();
            _homeVM = new HomeViewModel();
            var geometryVM = new GeometryViewModel();
            //_directStrengthResistanceVM = new Lazy<DirectStrengthResistanceViewModel>(() =>new DirectStrengthResistanceViewModel(_generalInfoVM, _bracingConditionsVM, geometryVM));
            //_egyptianCodeResistanceVM = new Lazy<EgyptianCodeResistanceViewModel>(() =>new EgyptianCodeResistanceViewModel(_generalInfoVM, _bracingConditionsVM, geometryVM));
            //_euroCodeReistanceVM = new Lazy<EuroCodeReistanceViewModel>(() =>new EuroCodeReistanceViewModel(_generalInfoVM, _bracingConditionsVM, geometryVM));
            //_aisiCodeResistanceVM = new Lazy<AISICodeResistanceViewModel>(() =>new AISICodeResistanceViewModel(_generalInfoVM, _bracingConditionsVM, geometryVM));
            ////_aiscCodeResistanceVM = new Lazy<AISCCodeResistanceViewModel>(() =>new AISCCodeResistanceViewModel(_generalInfoVM, _bracingConditionsVM, geometryVM));

            //_directStrengthCheckVM = new Lazy<DirectStrengthCheckViewModel>(() => new DirectStrengthCheckViewModel(_generalInfoVM, _bracingConditionsVM, geometryVM));
            //_egyptianCodeCheckVM = new Lazy<EgyptianCodeCheckViewModel>(() => new EgyptianCodeCheckViewModel(_generalInfoVM, _bracingConditionsVM, geometryVM));
            //_euroCodeCheckVM = new Lazy<EuroCodeCheckViewModel>(() => new EuroCodeCheckViewModel(_generalInfoVM, _bracingConditionsVM, geometryVM));
            //_aisiCodeCheckVM = new Lazy<AISICodeCheckViewModel>(() => new AISICodeCheckViewModel(_generalInfoVM, _bracingConditionsVM, geometryVM));

            //_directStrengthDesignVM = new Lazy<DirectStrengthDesignViewModel>(() => new DirectStrengthDesignViewModel(_generalInfoVM, _bracingConditionsVM));
            //_egyptianCodeDesignVM = new Lazy<EgyptianCodeDesignViewModel>(() => new EgyptianCodeDesignViewModel(_generalInfoVM, _bracingConditionsVM));
            //_euroCodeDesignVM = new Lazy<EuroCodeDesignViewModel>(() => new EuroCodeDesignViewModel(_generalInfoVM, _bracingConditionsVM));
            //_aisiCodeDesignVM = new Lazy<AISICodeDesignViewModel>(() => new AISICodeDesignViewModel(_generalInfoVM, _bracingConditionsVM));

            //Mediator.Instance.Subscribe<object>(this,  _=> OnDefaultResistance(_,_directStrengthResistanceVM.Value),Context.RESISTANCE_DIRECT_STRENGTH);
            //Mediator.Instance.Subscribe<object>(this, _ => OnDefaultResistance(_, _egyptianCodeResistanceVM.Value), Context.RESISTANCE_EGYPT_CODE);
            //Mediator.Instance.Subscribe<object>(this,_=>OnEuroResistance(_,_euroCodeReistanceVM.Value),Context.RESISTANCE_EURO_CODE);
            //Mediator.Instance.Subscribe<object>(this, _ => OnDefaultResistance(_, _aisiCodeResistanceVM.Value), Context.RESISTANCE_AISI_CODE);
            ////Mediator.Instance.Subscribe<object>(this, _ => OnDefaultResistance(_, _aiscCodeResistanceVM.Value), Context.RESISTANCE_AISC_CODE);


            //Mediator.Instance.Subscribe<object>(this, _ => OnDefaultResistance(_, _directStrengthCheckVM.Value), Context.CHECK_DIRECT_STRENGTH);
            //Mediator.Instance.Subscribe<object>(this, _ => OnDefaultResistance(_, _egyptianCodeCheckVM.Value), Context.CHECK_EGYPT_CODE);
            //Mediator.Instance.Subscribe<object>(this,_=> OnEuroResistance(_,_euroCodeCheckVM.Value), Context.CHECK_EURO_CODE);
            //Mediator.Instance.Subscribe<object>(this, _ => OnDefaultResistance(_, _aisiCodeCheckVM.Value), Context.CHECK_AISI_CODE);

            //Mediator.Instance.Subscribe<object>(this, _ => OnDefaultResistance(_, _directStrengthDesignVM.Value), Context.DESIGN_DIRECT_STRENGTH);
            //Mediator.Instance.Subscribe<object>(this, _ => OnDefaultResistance(_, _egyptianCodeDesignVM.Value), Context.DESIGN_EGYPT_CODE);
            //Mediator.Instance.Subscribe<object>(this, _ => OnEuroResistance(_, _euroCodeDesignVM.Value), Context.DESIGN_EURO_CODE);
            //Mediator.Instance.Subscribe<object>(this, _ => OnDefaultResistance(_, _aisiCodeDesignVM.Value), Context.DESIGN_AISI_CODE);

            Mediator.Instance.Subscribe<object>(this,  OnAbout, Context.ABOUT);
            Mediator.Instance.Subscribe<object>(this, OnHome, Context.HOME);

            IsMenu = false;
            CurrentVM = _homeVM;
        }


        #endregion

        #region Methods

        private void OnHome(object _)
        {
            IsMenu = false;
            CurrentVM = _homeVM;
        }

        private void OnAbout(object _)
        {
            CurrentVM = _aboutVM;
            MenuVM.Name = "About";
            IsMenu = true;
        }

        private void OnDefaultResistance(object _ , ViewModelBase vm)
        {
            CurrentVM = vm;
            DefaultChangeBracingConditions();
        }
      

        private void OnEuroResistance(object _,ViewModelBase euroVM)
        {
            CurrentVM = euroVM;
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
