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

        public AISCCodeResistanceViewModel(GeneralInfoViewModel generalInfoVM, BracingConditionsViewModel bracingConditionsVM)
            :base(generalInfoVM, bracingConditionsVM)
        {
            ResultsCommand = new RelayCommand(OnResults, CanResult);
            IsResistanceOutput = false;
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
                    var momentOut = GeneralInfoVM.IsUnstiffened ? (new SectionDimension(TotalHeightH, TotalWidthB, InternalRadiusR, ThicknessT, TotalFoldWidthC)).AsUnStiffenedSection().AsAISIMomentResistance(material, bracingConditions)  //TODO
                                                  : (new SectionDimension(TotalHeightH, TotalWidthB, InternalRadiusR, ThicknessT, TotalFoldWidthC)).AsLippedSection().AsAISIMomentResistance(material, bracingConditions); //TODO
                    IsResistanceOutput=true;
                    ResistanceOutput = momentOut;
                    break;
                case StrainingActions.COMPRESSION:
                    var compOut = GeneralInfoVM.IsUnstiffened ? (new SectionDimension(TotalHeightH, TotalWidthB, InternalRadiusR, ThicknessT, TotalFoldWidthC)).AsUnStiffenedSection().AsAISCCompressionResistance(material, bracingConditions)
                                                : (new SectionDimension(TotalHeightH, TotalWidthB, InternalRadiusR, ThicknessT, TotalFoldWidthC)).AsLippedSection().AsAISCCompressionResistance(material, bracingConditions);
                    IsResistanceOutput = true;
                    ResistanceOutput = compOut;
                    break;
            }

        }

        #endregion

    }
}
