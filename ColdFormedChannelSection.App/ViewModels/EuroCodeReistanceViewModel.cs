using ColdFormedChannelSection.App.ViewModels.Base;
using ColdFormedChannelSection.App.ViewModels.Enums;
using ColdFormedChannelSection.Core.Entities;
using ColdFormedChannelSection.Core.Enums;
using ColdFormedChannelSection.Core.Helpers;
using System.Windows.Input;

namespace ColdFormedChannelSection.App.ViewModels
{
    internal class EuroCodeReistanceViewModel : ResistanceBaseViewModel
    {


        #region Properties

        public override ICommand ResultsCommand { get; }

        #endregion

       

        #region Constructors

        public EuroCodeReistanceViewModel()
            : base("Resistance | Euro Code")
        {
            _isUsedParamsAction += (sa) =>
              {
                  if (sa == StrainingActions.MOMENT)
                      IsC1Used = true;
                  else
                      IsC1Used = false;
              };
            StrainingAction = StrainingActions.MOMENT;

            ResultsCommand = new RelayCommand(OnResults, CanResults);
            IsResistanceOutput = false;
        }

        #endregion


        #region Methods

        private bool CanResults()
        {
            return true;
        }

        private void OnResults()
        {
            IsResistanceOutput = false;
            var material = (new Material(Fy, E, 0.3)).Convert(Unit,Units.NMM);
            var bracingConditions = (new LengthBracingConditions(Lx, Ly, Lz, Kx, Ky, Kz, Lu, Cb, C1)).Convert(Unit,Units.NMM);
            switch (StrainingAction)
            {
                case StrainingActions.MOMENT:
                    var momentOut = IsUnstiffened ? (new SectionDimension(TotalHeightH, TotalWidthB, InternalRadiusR, ThicknessT, TotalFoldWidthC)).Convert(Unit,Units.NMM).AsUnStiffenedSection().AsEuroMomentResistance(material, bracingConditions)
                                                  : (new SectionDimension(TotalHeightH, TotalWidthB, InternalRadiusR, ThicknessT, TotalFoldWidthC)).Convert(Unit,Units.NMM).AsLippedSection().AsEuroMomentResistance(material, bracingConditions);
                    IsResistanceOutput = true;
                    ResistanceOutput = momentOut;
                    break;
                case StrainingActions.COMPRESSION:
                    var compOut = IsUnstiffened ? (new SectionDimension(TotalHeightH, TotalWidthB, InternalRadiusR, ThicknessT, TotalFoldWidthC)).Convert(Unit,Units.NMM).AsUnStiffenedSection().AsEuroCompressionResistance(material, bracingConditions)
                                                : (new SectionDimension(TotalHeightH, TotalWidthB, InternalRadiusR, ThicknessT, TotalFoldWidthC)).Convert(Unit,Units.NMM).AsLippedSection().AsEuroCompressionResistance(material, bracingConditions);
                    IsResistanceOutput = true;
                    ResistanceOutput = compOut;
                    break;
            }

        }

        #endregion

    }
}
