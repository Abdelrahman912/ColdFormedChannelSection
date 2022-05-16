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

        public ICommand HomeCommand { get;  }

        #endregion

        #region Constructors

        public MenuViewModel()
        {
            Name = "";
            HomeCommand = new RelayCommand(OnHome);
        }




        #endregion

        #region Methods

        private void OnHome()
        {
            Name = "Home";
            Mediator.Mediator.Instance.NotifyColleagues(new object(), Context.HOME);
        }

        #endregion

    }
}
