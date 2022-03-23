using ColdFormedChannelSection.App.ViewModels.Base;
using ColdFormedChannelSection.App.ViewModels.Enums;
using ColdFormedChannelSection.Core.Entities;
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
            var material = new Material(Fy, E, 0.3);
            var bracingConditions = new LengthBracingConditions(Lx, Ly, Lz, Kx, Ky, Kz, Lu, Cb, C1);
            switch (StrainingAction)
            {
                case StrainingActions.MOMENT:
                    var momentOut = IsUnstiffened ? (new SectionDimension(TotalHeightH, TotalWidthB, InternalRadiusR, ThicknessT, TotalFoldWidthC)).AsUnStiffenedSection().AsDSMomentResistance(material, bracingConditions)
                                                  : (new SectionDimension(TotalHeightH, TotalWidthB, InternalRadiusR, ThicknessT, TotalFoldWidthC)).AsLippedSection().AsDSMomentResistance(material, bracingConditions);
                    break;
                case StrainingActions.COMPRESSION:
                    var compOut = IsUnstiffened ? (new SectionDimension(TotalHeightH, TotalWidthB, InternalRadiusR, ThicknessT, TotalFoldWidthC)).AsUnStiffenedSection().AsDSCompressionResistance(material, bracingConditions)
                                                : (new SectionDimension(TotalHeightH, TotalWidthB, InternalRadiusR, ThicknessT, TotalFoldWidthC)).AsLippedSection().AsDSCompressionResistance(material, bracingConditions);
                    break;
            }

        }

        #endregion

    }
}
