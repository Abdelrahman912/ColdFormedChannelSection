﻿using ColdFormedChannelSection.App.ViewModels.Base;
using ColdFormedChannelSection.App.ViewModels.Enums;
using ColdFormedChannelSection.Core.Entities;
using ColdFormedChannelSection.Core.Enums;
using ColdFormedChannelSection.Core.Helpers;
using System.Windows.Input;

namespace ColdFormedChannelSection.App.ViewModels
{
    internal class EgyptianCodeResistanceViewModel:ResistanceBaseViewModel
    {

        #region Properties

        public override ICommand ResultsCommand { get; }

        #endregion

        #region Constructors

        public EgyptianCodeResistanceViewModel()
        {
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
            var material = (new Material(Fy, E, 0.3)).Convert(Unit, Units.TONCM);
            var bracingConditions = (new LengthBracingConditions(Lx, Ly, Lz, Kx, Ky, Kz, Lu, Cb, C1)).Convert(Unit, Units.TONCM);
            switch (StrainingAction)
            {
                case StrainingActions.MOMENT:
                    var momentOut = IsUnstiffened ? (new SectionDimension(TotalHeightH, TotalWidthB, InternalRadiusR, ThicknessT, TotalFoldWidthC)).Convert(Unit, Units.TONCM).AsUnStiffenedSection().AsEgyptMomentResistance(material, bracingConditions).Convert(Units.TONCM, Unit)
                                                  : (new SectionDimension(TotalHeightH, TotalWidthB, InternalRadiusR, ThicknessT, TotalFoldWidthC)).Convert(Unit, Units.TONCM).AsLippedSection().AsEgyptMomentResistance(material, bracingConditions).Convert(Units.TONCM, Unit);
                    IsResistanceOutput = true;
                    ResistanceOutput = momentOut;
                    break;
                case StrainingActions.COMPRESSION:
                    var compOut = IsUnstiffened ? (new SectionDimension(TotalHeightH, TotalWidthB, InternalRadiusR, ThicknessT, TotalFoldWidthC)).Convert(Unit, Units.TONCM).AsUnStiffenedSection().AsEgyptCompressionResistance(material, bracingConditions).Convert(Units.TONCM, Unit)
                                                : (new SectionDimension(TotalHeightH, TotalWidthB, InternalRadiusR, ThicknessT, TotalFoldWidthC)).Convert(Unit, Units.TONCM).AsLippedSection().AsEgyptCompressionResistance(material, bracingConditions).Convert(Units.TONCM, Unit);
                    IsResistanceOutput = true;
                    ResistanceOutput = compOut;
                    break;
            }

        }

        #endregion

    }
}
