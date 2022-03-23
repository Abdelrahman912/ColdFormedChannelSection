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

        public AISCCodeResistanceViewModel()
            :base("Resistance | AISC Code")
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
            var material = new Material(Fy, E, 0.3);
            var bracingConditions = new LengthBracingConditions(Lx, Ly, Lz, Kx, Ky, Kz, Lu, Cb, C1);
            switch (StrainingAction)
            {
                case StrainingActions.MOMENT:
                    var momentOut = IsUnstiffened ? (new SectionDimension(TotalHeightH, TotalWidthB, InternalRadiusR, ThicknessT, TotalFoldWidthC)).AsUnStiffenedSection().AsAISIMomentResistance(material, bracingConditions)  //TODO
                                                  : (new SectionDimension(TotalHeightH, TotalWidthB, InternalRadiusR, ThicknessT, TotalFoldWidthC)).AsLippedSection().AsAISIMomentResistance(material, bracingConditions); //TODO
                    IsResistanceOutput=true;
                    ResistanceOutput = momentOut;
                    break;
                case StrainingActions.COMPRESSION:
                    var compOut = IsUnstiffened ? (new SectionDimension(TotalHeightH, TotalWidthB, InternalRadiusR, ThicknessT, TotalFoldWidthC)).AsUnStiffenedSection().AsAISCCompressionResistance(material, bracingConditions)
                                                : (new SectionDimension(TotalHeightH, TotalWidthB, InternalRadiusR, ThicknessT, TotalFoldWidthC)).AsLippedSection().AsAISCCompressionResistance(material, bracingConditions);
                    IsResistanceOutput = true;
                    ResistanceOutput = compOut;
                    break;
            }

        }

        #endregion

    }
}
