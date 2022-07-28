using ColdFormedChannelSection.App.ViewModels;
using ColdFormedChannelSection.App.ViewModels.Base;
using ColdFormedChannelSection.App.ViewModels.Enums;
using ColdFormedChannelSection.App.ViewModels.Mediator;
using ColdFormedChannelSection.Core.Enums;
using CSharp.Functional.Errors;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Unit = System.ValueTuple;
using static ColdFormedChannelSection.App.UI.Services.Services;
using static CSharp.Functional.Functional;
using System.Linq;
using ColdFormedChannelSection.App.Models;
using ColdFormedChannelSection.App.UI.Services;
using CSharp.Functional.Constructs;

namespace ColdFormedChannelSection.App
{
    internal class MainViewModel:ViewModelBase
    {

        #region Pivate Fields

        private  Lazy<DirectStrengthViewModel> _directStrengthVM;
        private Lazy<EffectiveWidthViewModel> _effectiveWidthVM;

        private ViewModelBase _currentVM;
        private Lazy< GeneralInfoViewModel> _generalInfoVM;
        private  Lazy<BracingConditionsViewModel> _bracingConditionsVM;
        private Lazy<MaterialViewModel> _materialVM;
        private  Lazy<AboutViewModel> _aboutVM;
        private Lazy<InputLoadViewModel> _inputLoadVM;
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
            Mediator.Instance.NotifyColleagues(KeyValuePair.Create(_generalInfoVM.Value.DesignCode, _generalInfoVM.Value.StrainingAction), Context.BRACING);

            MenuVM.Name = "Effective Width";
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
                _inputLoadVM =new Lazy<InputLoadViewModel>(()=> new InputLoadViewModel());
                _generalInfoVM =new Lazy<GeneralInfoViewModel>(()=> new GeneralInfoViewModel());
                _aboutVM =new Lazy<AboutViewModel>(()=> new AboutViewModel());
                _bracingConditionsVM =new Lazy<BracingConditionsViewModel>(()=> new BracingConditionsViewModel());
                _materialVM =new Lazy<MaterialViewModel>(()=> new MaterialViewModel());
                var geometryVM =new Lazy<GeometryViewModel>( new GeometryViewModel());
                _directStrengthVM = new Lazy<DirectStrengthViewModel>(() => new DirectStrengthViewModel(_generalInfoVM.Value, _bracingConditionsVM.Value, geometryVM.Value,_materialVM.Value,_inputLoadVM.Value,ShowErrorService(),(rep)=>Services.ReportService(rep),FolderDialogService));
                _effectiveWidthVM = new Lazy<EffectiveWidthViewModel>(() => new EffectiveWidthViewModel(_generalInfoVM.Value, _bracingConditionsVM.Value, geometryVM.Value,_materialVM.Value,_inputLoadVM.Value,ShowErrorService(), (rep) => Services.ReportService(rep), FolderDialogService));
            });
        }

        private Func<List<Error>,Unit> ShowErrorService()
        {
            return (errs) =>
            {
                errs.Select(err => new ResultMessage(err.Message, ResultMessageType.ERROR)).ToList().ResultMessagesService();
                return Unit();
            };
        }

        private void OnDirectStrength(object _)
        {
            IsMenu = true;
            CurrentVM = _directStrengthVM.Value;
            _directStrengthVM.Value.GeneralInfoVM.IsDesignCode = false;
            Mediator.Instance.NotifyColleagues( _generalInfoVM.Value.StrainingAction,Context.BRACING_DS);
            MenuVM.Name = "Direct Strength";
        }

        private void OnHome(object _)
        {
            IsMenu = false;
            CurrentVM = _homeVM;
        }

        private void OnAbout(object _)
        {
            CurrentVM = _aboutVM.Value;
            MenuVM.Name = "About";
            IsMenu = true;
        }

       
      

        

       

        #endregion

    }
}
