using ColdFormedChannelSection.App.ViewModels.Base;
using ColdFormedChannelSection.Core.Entities;
using ColdFormedChannelSection.Core.Enums;
using System;
using System.Collections.Generic;
using System.Windows.Input;

namespace ColdFormedChannelSection.App.ViewModels
{
    public abstract class ResistanceBaseViewModel : ViewModelBase
    {

        #region Private Fields

       

        private bool _isResistanceOutput;

        private ResistanceOutput _resistanceOutput;

        private GeneralInfoViewModel _generalInfoVM;

        private BracingConditionsViewModel _bracingConditionsVM;

        private GeometryViewModel _geometryVM;

        #endregion

        #region Properties


        public bool IsResistanceOutput
        {
            get => _isResistanceOutput;
            set => NotifyPropertyChanged(ref _isResistanceOutput, value);
        }

        public ResistanceOutput ResistanceOutput
        {
            get => _resistanceOutput;
            set => NotifyPropertyChanged(ref _resistanceOutput, value);
        }


        public GeneralInfoViewModel GeneralInfoVM
        {
            get => _generalInfoVM;
            set => NotifyPropertyChanged(ref _generalInfoVM, value);
        }

        public BracingConditionsViewModel BracingConditionsVM
        {
            get => _bracingConditionsVM;
            set => NotifyPropertyChanged(ref _bracingConditionsVM, value);
        }

        public GeometryViewModel GeometryVM
        {
            get => _geometryVM;
            set=>NotifyPropertyChanged(ref _geometryVM, value);
        }

        public virtual ICommand ResultsCommand { get; }

        #endregion

        #region Constructors

        public ResistanceBaseViewModel(GeneralInfoViewModel generalInfoVM,
                                       BracingConditionsViewModel bracingConditionsVM,
                                       GeometryViewModel geometryVM)
        {
            
            GeneralInfoVM = generalInfoVM;
            BracingConditionsVM = bracingConditionsVM;
            GeometryVM = geometryVM;
        }



        #endregion


       
    }
}
