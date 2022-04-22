using ColdFormedChannelSection.App.ViewModels.Base;
using ColdFormedChannelSection.App.ViewModels.Enums;
using ColdFormedChannelSection.Core.Entities;
using System;
using System.Windows.Input;

namespace ColdFormedChannelSection.App.ViewModels
{
    internal abstract class ResistanceBaseViewModel : ViewModelBase
    {

        #region Private Fields


        
        protected  Action<Section> _onResults = _ => { };

        
       
       

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
            
            //_isUsedParamsAction = (sa) =>
            //{
            //    switch (sa)
            //    {
            //        case StrainingActions.MOMENT:
            //            IsCbUsed = true;
            //            IsLuUsed = true;
            //            break;
            //        case StrainingActions.COMPRESSION:
            //        default:
            //            IsCbUsed = false;
            //            IsLuUsed = false;
            //            break;
            //    }
            //};
            GeneralInfoVM = generalInfoVM;
            BracingConditionsVM = bracingConditionsVM;
            GeometryVM = geometryVM;
           
            //IsC1Used = false;
            //IsLuUsed = false;
            //IsC1Used = false;
           
            
        }



        #endregion


        #region Methods

      

       

        #endregion
    }
}
