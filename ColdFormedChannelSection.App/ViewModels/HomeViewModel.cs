using ColdFormedChannelSection.App.ViewModels.Base;
using ColdFormedChannelSection.App.ViewModels.Enums;
using System;
using System.Windows.Input;

namespace ColdFormedChannelSection.App.ViewModels
{
    public class HomeViewModel:ViewModelBase
    {
        #region Private Fields

        #endregion

        #region Properties

        public ICommand AboutCommand { get; }

        public ICommand DirectStrengthCommand { get; }

        public ICommand EffectiveWidthCommand { get; }

        #endregion

        #region Constructors

        public HomeViewModel()
        {
            AboutCommand = new RelayCommand(OnAbout);
            DirectStrengthCommand = new RelayCommand(OnDirectStrength);
            EffectiveWidthCommand = new RelayCommand(OnEffectiveWidth);
        }

        #endregion

        #region Methods

        private void OnEffectiveWidth()
        {
            Mediator.Mediator.Instance.NotifyColleagues(new object(), Context.EFFECTIVE_WIDTH);
        }


        private void OnDirectStrength()
        {
            Mediator.Mediator.Instance.NotifyColleagues(new object(), Context.DIRECT_STRENGTH);
        }

        private void OnAbout()
        {
            Mediator.Mediator.Instance.NotifyColleagues(new object(), Context.ABOUT);
        }
        #endregion


    }
}
