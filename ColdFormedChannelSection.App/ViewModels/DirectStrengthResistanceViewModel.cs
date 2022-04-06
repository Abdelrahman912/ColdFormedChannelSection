using ColdFormedChannelSection.App.ViewModels.Base;
using ColdFormedChannelSection.App.ViewModels.Enums;
using ColdFormedChannelSection.Core.Entities;
using ColdFormedChannelSection.Core.Enums;
using ColdFormedChannelSection.Core.Helpers;
using System.Windows.Input;

namespace ColdFormedChannelSection.App.ViewModels
{
    internal class DirectStrengthResistanceViewModel:ResistanceBaseViewModel
    {

        #region Properties

        public override ICommand ResultsCommand { get; }

        #endregion


        #region Constructors
        public DirectStrengthResistanceViewModel()
            :base("Resistance | Direct Strength")
        {
            ResultsCommand = new RelayCommand(OnResults, CanResults);
            IsResistanceOutput = false;
            //IsLuUsed = true;
            //_isUsedParamsAction += (sa) =>
            //{
            //    if (sa == StrainingActions.COMPRESSION)
            //        IsLuUsed = true;
            //};
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
            var material = (new Material(Fy, E, 0.3)).Convert(Unit,Units.KIPINCH);
            var bracingConditions = (new LengthBracingConditions(Lx, Ly, Lz, Kx, Ky, Kz, Lu, Cb, C1)).Convert(Unit,Units.KIPINCH);
            switch (StrainingAction)
            {
                case StrainingActions.MOMENT:
                    var momentOut = IsUnstiffened ? (new SectionDimension(TotalHeightH, TotalWidthB, InternalRadiusR, ThicknessT, TotalFoldWidthC)).Convert(Unit,Units.KIPINCH).AsUnStiffenedSection().AsDSMomentResistance(material, bracingConditions).Convert(Units.KIPINCH,Unit)
                                                  : (new SectionDimension(TotalHeightH, TotalWidthB, InternalRadiusR, ThicknessT, TotalFoldWidthC)).Convert(Unit,Units.KIPINCH).AsLippedSection().AsDSMomentResistance(material, bracingConditions).Convert(Units.KIPINCH,Unit);
                    IsResistanceOutput=true;
                    ResistanceOutput = momentOut;
                    break;
                case StrainingActions.COMPRESSION:
                    var compOut = IsUnstiffened ? (new SectionDimension(TotalHeightH, TotalWidthB, InternalRadiusR, ThicknessT, TotalFoldWidthC)).Convert(Unit,Units.KIPINCH).AsUnStiffenedSection().AsDSCompressionResistance(material, bracingConditions).Convert(Units.KIPINCH,Unit)
                                                : (new SectionDimension(TotalHeightH, TotalWidthB, InternalRadiusR, ThicknessT, TotalFoldWidthC)).Convert(Unit,Units.KIPINCH).AsLippedSection().AsDSCompressionResistance(material, bracingConditions).Convert(Units.KIPINCH,Unit);
                    IsResistanceOutput = true;
                    ResistanceOutput = compOut;
                    break;
            }

        }

        #endregion

    }
}
