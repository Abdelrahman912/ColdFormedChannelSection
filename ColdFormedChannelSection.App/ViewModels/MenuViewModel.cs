using ColdFormedChannelSection.App.ViewModels.Base;
using ColdFormedChannelSection.App.ViewModels.Enums;
using System;
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


        public ICommand CheckDSCommand { get; }
        public ICommand CheckEgyptCommand { get; }
        public ICommand CheckEuroCommand { get; }
        public ICommand CheckAISICommand { get; }

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

            CheckDSCommand = new RelayCommand(OnCheckDS);
            CheckEgyptCommand = new RelayCommand(OnCheckEgypt);
            CheckEuroCommand = new RelayCommand(OnCheckEuro);
            CheckAISICommand = new RelayCommand(OnCheckAISI);
        }




        #endregion

        #region Methods

        private void OnCheckAISI()
        {
            Name = "Check | AISI Code";
            Mediator.Mediator.Instance.NotifyColleagues(new object(), Context.CHECK_AISI_CODE);
        }

        private void OnCheckEuro()
        {
            Name = "Check | Euro Code";
            Mediator.Mediator.Instance.NotifyColleagues(new object(), Context.CHECK_EURO_CODE);
        }

        private void OnCheckEgypt()
        {
            Name = "Check | Egyptian Code";
            Mediator.Mediator.Instance.NotifyColleagues(new object(), Context.CHECK_EGYPT_CODE);
        }

        private void OnCheckDS()
        {
            Name = "Check | Direct Strength";
            Mediator.Mediator.Instance.NotifyColleagues(new object(), Context.CHECK_DIRECT_STRENGTH);
        }

        private void OnResistanceAISC()
        {
            Name = "Resistance | AISC Code";
            Mediator.Mediator.Instance.NotifyColleagues(new object(), Context.RESISTANCE_AISC_CODE);
        }
       

        private void OnResistanceAISI()
        {
            Name = "Resistance | AISI Code";
            Mediator.Mediator.Instance.NotifyColleagues(new object(), Context.RESISTANCE_AISI_CODE);
        }
       

        private void OnReistanceEuro()
        {
            Name = "Resistance | Euro Code";
            Mediator.Mediator.Instance.NotifyColleagues(new object(), Context.RESISTANCE_EURO_CODE);
        }
       

        private void OnResistanceEgypt()
        {
            Name = "Resistance | Egyptian Code";
            Mediator.Mediator.Instance.NotifyColleagues(new object(), Context.RESISTANCE_EGYPT_CODE);
        }
       

        private void OnResistanceDS()
        {
            Name = "Resistance | Direct Strength";
            Mediator.Mediator.Instance.NotifyColleagues(new object(), Context.RESISTANCE_DIRECT_STRENGTH);
        }

        #endregion

    }
}
