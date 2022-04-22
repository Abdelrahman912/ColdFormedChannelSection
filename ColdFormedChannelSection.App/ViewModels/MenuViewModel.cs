using ColdFormedChannelSection.App.ViewModels.Base;
using ColdFormedChannelSection.App.ViewModels.Enums;
using System.Windows.Input;

namespace ColdFormedChannelSection.App.ViewModels
{
    internal class MenuViewModel:ViewModelBase
    {

        #region Private Fields

        private string _name;

        #endregion

        #region Properties

        public string Name
        {
            get => _name;
            set => NotifyPropertyChanged(ref _name ,value);
        }

        public ICommand ResistanceDSCommand { get;  }
        public ICommand ResistanceEgyptCommand { get; }
        public ICommand ResistanceEuroCommand { get; }
        public ICommand ResistanceAISICommand { get; }
        public ICommand ResistanceAISCCommand { get; }

        #endregion

        #region Constructors

        public MenuViewModel()
        {
            Name = "";
            ResistanceDSCommand = new RelayCommand(OnResistanceDS);
            ResistanceEgyptCommand = new RelayCommand(OnResistanceEgypt);
            ResistanceEuroCommand = new RelayCommand(OnReistanceEuro);
            ResistanceAISICommand = new RelayCommand(OnResistanceAISI);
            ResistanceAISCCommand = new RelayCommand(OnResistanceAISC);
        }


        #endregion

        #region Methods

        private void OnResistanceAISC()
        {
            Name = "Resistance | AISC Code";
            Mediator.Mediator.Instance.NotifyColleagues(new object(), Context.ResistanceAISCCode);
        }
       

        private void OnResistanceAISI()
        {
            Name = "Resistance | AISI Code";
            Mediator.Mediator.Instance.NotifyColleagues(new object(), Context.ResistanceAISICode);
        }
       

        private void OnReistanceEuro()
        {
            Name = "Resistance | Euro Code";
            Mediator.Mediator.Instance.NotifyColleagues(new object(), Context.ResistanceEuroCode);
        }
       

        private void OnResistanceEgypt()
        {
            Name = "Resistance | Egyptian Code";
            Mediator.Mediator.Instance.NotifyColleagues(new object(), Context.ResistanceEgyptianCode);
        }
       

        private void OnResistanceDS()
        {
            Name = "Resistance | Direct Strength";
            Mediator.Mediator.Instance.NotifyColleagues(new object(), Context.ResistanceDirectStrength);
        }

        #endregion

    }
}
