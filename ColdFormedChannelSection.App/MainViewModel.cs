using ColdFormedChannelSection.App.ViewModels;
using ColdFormedChannelSection.App.ViewModels.Base;
using ColdFormedChannelSection.App.ViewModels.Enums;
using ColdFormedChannelSection.App.ViewModels.Mediator;
using ColdFormedChannelSection.Core.Enums;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ColdFormedChannelSection.App
{
    internal class MainViewModel:ViewModelBase
    {

        #region Pivate Fields

        private  Lazy<DirectStrengthViewModel> _directStrengthVM;
        private Lazy<EffectiveWidthViewModel> _effectiveWidthVM;

        private ViewModelBase _currentVM;
        private  GeneralInfoViewModel _generalInfoVM;
        private  BracingConditionsViewModel _bracingConditionsVM;
        private MaterialViewModel _materialVM;
        private  AboutViewModel _aboutVM;
        private  HomeViewModel _homeVM;

        private bool _isMenu;
        #endregion

        #region Properties

        public bool IsMenu
        {
            get => _isMenu;
            set => NotifyPropertyChanged(ref _isMenu, value);
        }

        public MenuViewModel MenuVM { get; set; }

        public ViewModelBase CurrentVM
        {
            get => _currentVM;
            set => NotifyPropertyChanged( ref _currentVM, value );
        }
        #endregion

        #region Constructors

        public MainViewModel()
        {
            _homeVM = new HomeViewModel();
            IsMenu = false;
            CurrentVM = _homeVM;
            Init();
        }


        #endregion

        #region Methods

        private void OnEffectiveWidth(object _)
        {
            IsMenu = true;
            CurrentVM = _effectiveWidthVM.Value;
            _effectiveWidthVM.Value.GeneralInfoVM.IsDesignCode = true;
            Mediator.Instance.NotifyColleagues(KeyValuePair.Create(_generalInfoVM.DesignCode, _generalInfoVM.StrainingAction), Context.BRACING);

            MenuVM.Name = "Effective Wide Width";
        }

        private  void Init()
        {
            Task.Run(() =>
            {
                Mediator.Instance.Subscribe<object>(this, OnAbout, Context.ABOUT);
                Mediator.Instance.Subscribe<object>(this, OnHome, Context.HOME);
                Mediator.Instance.Subscribe<object>(this, OnDirectStrength, Context.DIRECT_STRENGTH);
                Mediator.Instance.Subscribe<object>(this, OnEffectiveWidth, Context.EFFECTIVE_WIDTH);
                MenuVM = new MenuViewModel();
                _generalInfoVM = new GeneralInfoViewModel();
                _aboutVM = new AboutViewModel();
                _bracingConditionsVM = new BracingConditionsViewModel();
                _materialVM = new MaterialViewModel();
                var geometryVM = new GeometryViewModel();
                _directStrengthVM = new Lazy<DirectStrengthViewModel>(() => new DirectStrengthViewModel(_generalInfoVM, _bracingConditionsVM, geometryVM,_materialVM));
                _effectiveWidthVM = new Lazy<EffectiveWidthViewModel>(() => new EffectiveWidthViewModel(_generalInfoVM, _bracingConditionsVM, geometryVM,_materialVM));
            });
        }

        private void OnDirectStrength(object _)
        {
            IsMenu = true;
            CurrentVM = _directStrengthVM.Value;
            _directStrengthVM.Value.GeneralInfoVM.IsDesignCode = false;
            Mediator.Instance.NotifyColleagues(KeyValuePair.Create(DesignCode.AISI, _generalInfoVM.StrainingAction),Context.BRACING);
            MenuVM.Name = "Direct Strength";
        }

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

       
      

        

       

        #endregion

    }
}
