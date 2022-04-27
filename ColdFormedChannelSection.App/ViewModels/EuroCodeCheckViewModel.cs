using ColdFormedChannelSection.App.Extensions;
using ColdFormedChannelSection.App.ViewModels.Base;
using ColdFormedChannelSection.App.ViewModels.Enums;
using ColdFormedChannelSection.Core.Entities;
using ColdFormedChannelSection.Core.Enums;
using ColdFormedChannelSection.Core.Helpers;
using System;
using System.Windows.Input;

namespace ColdFormedChannelSection.App.ViewModels
{
    public class EuroCodeCheckViewModel:CheckBaseViewModel
    {
        
        #region Properties

        public override ICommand ResultsCommand { get; }

        #endregion

        #region Constructors

        public EuroCodeCheckViewModel(GeneralInfoViewModel generalInfoVM, BracingConditionsViewModel bracingConditionsVM, GeometryViewModel geometryVM) :
            base(generalInfoVM, bracingConditionsVM, geometryVM)
        {
            ResultsCommand = new RelayCommand(OnResults, CanResults);
            switch (GeneralInfoVM.StrainingAction)
            {
                case StrainingActions.MOMENT:
                    BracingConditionsVM.IsC1Used = true;
                    BracingConditionsVM.IsLuUsed = true;
                    BracingConditionsVM.IsCbUsed = true;
                    break;
                case StrainingActions.COMPRESSION:
                    BracingConditionsVM.IsC1Used = false;
                    BracingConditionsVM.C1 = 0;
                    BracingConditionsVM.IsLuUsed = false;
                    BracingConditionsVM.Lu = 0;
                    BracingConditionsVM.IsCbUsed = false;
                    BracingConditionsVM.Cb = 0;
                    break;
            }
        }



        #endregion

        #region Methods

        private bool CanResults()
        {
            throw new NotImplementedException();
        }

        private void OnResults()
        {
            var material = (new Material(GeneralInfoVM.Fy, GeneralInfoVM.E, 0.3)).Convert(GeneralInfoVM.Unit, Units.NMM);
            var bracingConditions = BracingConditionsVM.AsEntity().Convert(GeneralInfoVM.Unit, Units.NMM);
            switch (GeneralInfoVM.StrainingAction)
            {
                case StrainingActions.MOMENT:
                    var momentOut = GeneralInfoVM.IsUnstiffened ? GeometryVM.AsEntity().Convert(GeneralInfoVM.Unit, Units.NMM).AsUnStiffenedSection().AsEuroMomentResistance(material, bracingConditions).Convert(Units.NMM, GeneralInfoVM.Unit)
                                                  : GeometryVM.AsEntity().Convert(GeneralInfoVM.Unit, Units.NMM).AsLippedSection().AsEuroMomentResistance(material, bracingConditions).Convert(Units.NMM, GeneralInfoVM.Unit);
                    break;
                case StrainingActions.COMPRESSION:
                    var compOut = GeneralInfoVM.IsUnstiffened ? GeometryVM.AsEntity().Convert(GeneralInfoVM.Unit, Units.NMM).AsUnStiffenedSection().AsEuroCompressionResistance(material, bracingConditions).Convert(Units.NMM, GeneralInfoVM.Unit)
                                                : GeometryVM.AsEntity().Convert(GeneralInfoVM.Unit, Units.NMM).AsLippedSection().AsEuroCompressionResistance(material, bracingConditions).Convert(Units.NMM, GeneralInfoVM.Unit);
                    break;
            }
        }

        #endregion

    }
}
