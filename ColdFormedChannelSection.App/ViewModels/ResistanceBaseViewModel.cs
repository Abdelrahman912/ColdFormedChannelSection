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


        protected event Action<StrainingActions> _isUsedParamsAction;
        protected  Action<Section> _onResults = _ => { };

        
        private double _totalHeightH;
        private double _totalWidthB;
        private double _internalRadiusR;
        private double _thicknessT;
        private double _totalFoldWidthC;
       

        private bool _isResistanceOutput;

        private ResistanceOutput _resistanceOutput;

        private GeneralInfoViewModel _generalInfoVM;

        private BracingConditionsViewModel _bracingConditionsVM;

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

       

        public double TotalFoldWidthC
        {
            get => _totalFoldWidthC;
            set => NotifyPropertyChanged(ref _totalFoldWidthC, value);
        }

        public double ThicknessT
        {
            get => _thicknessT;
            set => NotifyPropertyChanged(ref _thicknessT, value);
        }

        public double InternalRadiusR
        {
            get => _internalRadiusR;
            set => NotifyPropertyChanged(ref _internalRadiusR, value);
        }

        public double TotalWidthB
        {
            get => _totalWidthB;
            set => NotifyPropertyChanged(ref _totalWidthB, value);
        }

        public double TotalHeightH
        {
            get => _totalHeightH;
            set => NotifyPropertyChanged(ref _totalHeightH, value);
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


        public virtual ICommand ResultsCommand { get; }

        #endregion

        #region Constructors

        public ResistanceBaseViewModel(GeneralInfoViewModel generalInfoVM,
                                       BracingConditionsViewModel bracingConditionsVM)
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
           
            //IsC1Used = false;
            //IsLuUsed = false;
            //IsC1Used = false;
           
            
        }



        #endregion


        #region Methods

      

       

        #endregion
    }
}
