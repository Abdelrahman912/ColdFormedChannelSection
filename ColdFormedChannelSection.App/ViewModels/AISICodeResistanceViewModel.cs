using ColdFormedChannelSection.App.ViewModels.Base;
using ColdFormedChannelSection.App.ViewModels.Enums;
using ColdFormedChannelSection.Core.Entities;
using ColdFormedChannelSection.Core.Helpers;
using System.Windows.Input;

namespace ColdFormedChannelSection.App.ViewModels
{
    internal class AISICodeResistanceViewModel:ResistanceBaseViewModel
    {

        #region Properties

        public override ICommand ResultsCommand { get; }

        #endregion

        #region Cosntructors

        public AISICodeResistanceViewModel()
            :base("Resistance | AISI Code")
        {
            ResultsCommand = new RelayCommand(OnResults,CanResults);
        }

        #endregion

        #region Methods

        private bool CanResults()
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
                    var momentOut = IsUnstiffened ? (new SectionDimension(TotalHeightH, TotalWidthB, InternalRadiusR, ThicknessT, TotalFoldWidthC)).AsUnStiffenedSection().AsMomentResistance(material, bracingConditions)
                                                  : (new SectionDimension(TotalHeightH, TotalWidthB, InternalRadiusR, ThicknessT, TotalFoldWidthC)).AsLippedSection().AsMomentResistance(material, bracingConditions);
                    break;
                case StrainingActions.COMPRESSION:
                    var compOut = IsUnstiffened ? (new SectionDimension(TotalHeightH, TotalWidthB, InternalRadiusR, ThicknessT, TotalFoldWidthC)).AsUnStiffenedSection().AsCompressionResistance(material, bracingConditions)
                                                : (new SectionDimension(TotalHeightH, TotalWidthB, InternalRadiusR, ThicknessT, TotalFoldWidthC)).AsLippedSection().AsCompressionResistance(material, bracingConditions);
                    break;
            }

        }

        #endregion

    }
}
