using ColdFormedChannelSection.App.Extensions;
using ColdFormedChannelSection.App.ViewModels.Base;
using ColdFormedChannelSection.App.ViewModels.Enums;
using ColdFormedChannelSection.Core.Entities;
using ColdFormedChannelSection.Core.Helpers;
using System.Windows.Input;

namespace ColdFormedChannelSection.App.ViewModels
{
    internal class AISCCodeResistanceViewModel:ResistanceBaseViewModel
    {

        #region Properties

        public override ICommand ResultsCommand { get; }

        #endregion

        #region Constructors

        public AISCCodeResistanceViewModel(GeneralInfoViewModel generalInfoVM, BracingConditionsViewModel bracingConditionsVM,GeometryViewModel geometryVM)
            :base(generalInfoVM, bracingConditionsVM,geometryVM)
        {
            ResultsCommand = new RelayCommand(OnResults, CanResult);
            IsResistanceOutput = false;
            switch (GeneralInfoVM.StrainingAction)
            {
                case StrainingActions.MOMENT:
                    BracingConditionsVM.IsC1Used = false;
                    BracingConditionsVM.C1 = 0;
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

        private bool CanResult()
        {
            return true;
        }

        private void OnResults()
        {
            var material = new Material(GeneralInfoVM.Fy,GeneralInfoVM.E, 0.3);
            var bracingConditions = BracingConditionsVM.AsEntity();
            switch (GeneralInfoVM.StrainingAction)
            {
                case StrainingActions.MOMENT:
                    var momentOut = GeneralInfoVM.IsUnstiffened ? (GeometryVM.AsEntity()).AsUnStiffenedSection().AsAISIMomentResistance(material, bracingConditions)  //TODO
                                                  : (GeometryVM.AsEntity()).AsLippedSection().AsAISIMomentResistance(material, bracingConditions); //TODO
                    IsResistanceOutput=true;
                    ResistanceOutput = momentOut;
                    break;
                case StrainingActions.COMPRESSION:
                    var compOut = GeneralInfoVM.IsUnstiffened ? (GeometryVM.AsEntity()).AsUnStiffenedSection().AsAISCCompressionResistance(material, bracingConditions)
                                                : (GeometryVM.AsEntity()).AsLippedSection().AsAISCCompressionResistance(material, bracingConditions);
                    IsResistanceOutput = true;
                    ResistanceOutput = compOut;
                    break;
            }

        }

        #endregion

    }
}
