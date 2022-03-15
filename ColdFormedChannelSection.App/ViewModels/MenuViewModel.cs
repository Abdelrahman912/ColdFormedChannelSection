using ColdFormedChannelSection.App.ViewModels.Base;
using ColdFormedChannelSection.App.ViewModels.Enums;
using System.Windows.Input;

namespace ColdFormedChannelSection.App.ViewModels
{
    internal class MenuViewModel:ViewModelBase
    {

        #region Properties
        public ICommand ResistanceDSCommand { get;  }
        public ICommand ResistanceEgyptCommand { get; }
        public ICommand ResistanceEuroCommand { get; }
        public ICommand ResistanceAISICommand { get; }
        public ICommand ResistanceAISCCommand { get; }

        #endregion

        #region Constructors

        public MenuViewModel()
        {
            ResistanceDSCommand = new RelayCommand(OnResistanceDS);
            ResistanceEgyptCommand = new RelayCommand(OnResistanceEgypt);
            ResistanceEuroCommand = new RelayCommand(OnReistanceEuro);
            ResistanceAISICommand = new RelayCommand(OnResistanceAISI);
            ResistanceAISCCommand = new RelayCommand(OnResistanceAISC);
        }


        #endregion

        #region Methods

        private void OnResistanceAISC() =>
            Mediator.Mediator.Instance.NotifyColleagues(new object(), Context.ResistanceAISCCode);
       

        private void OnResistanceAISI() =>
            Mediator.Mediator.Instance.NotifyColleagues(new object(), Context.ResistanceAISICode);
       

        private void OnReistanceEuro() =>
            Mediator.Mediator.Instance.NotifyColleagues(new object(), Context.ResistanceEuroCode);
       

        private void OnResistanceEgypt() =>
            Mediator.Mediator.Instance.NotifyColleagues(new object(), Context.ResistanceEgyptianCode);
       

        private void OnResistanceDS() =>
            Mediator.Mediator.Instance.NotifyColleagues(new object(), Context.ResistanceDirectStrength);

        #endregion

    }
}
