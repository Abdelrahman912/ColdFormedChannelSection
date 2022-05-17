﻿using ColdFormedChannelSection.App.Extensions;
using ColdFormedChannelSection.App.ViewModels.Base;
using ColdFormedChannelSection.App.ViewModels.Enums;
using ColdFormedChannelSection.Core.Entities;
using ColdFormedChannelSection.Core.Enums;
using ColdFormedChannelSection.Core.Helpers;
using System.Windows.Input;

namespace ColdFormedChannelSection.App.ViewModels
{
    public class EgyptianCodeCheckViewModel : CheckBaseViewModel
    {

        #region Properties

        public override ICommand ResultsCommand { get; }


        #endregion

        #region Constructors
        public EgyptianCodeCheckViewModel(GeneralInfoViewModel generalInfoVM, BracingConditionsViewModel bracingConditionsVM, GeometryViewModel geometryVM) 
            : base(generalInfoVM, bracingConditionsVM, geometryVM)
        {
            ResultsCommand = new RelayCommand(OnResults, CanResults);
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

        private bool CanResults()
        {
            return true;
        }

        private void OnResults()
        {
            IsCheckOutput = false;
            //var material = (new Material(GeneralInfoVM.Fy, GeneralInfoVM.E, 0.3)).Convert(GeneralInfoVM.Unit, Units.TONCM);
            var bracingConditions = BracingConditionsVM.AsEntity().Convert(GeneralInfoVM.Unit, Units.TONCM);
            //switch (GeneralInfoVM.StrainingAction)
            //{
            //    case StrainingActions.MOMENT:
            //        var momentOut = GeneralInfoVM.IsUnstiffened ? GeometryVM.AsEntity().Convert(GeneralInfoVM.Unit, Units.TONCM).AsUnStiffenedSection().AsEgyptMomentResistance(material, bracingConditions).Convert(Units.TONCM, GeneralInfoVM.Unit)
            //                                      : GeometryVM.AsEntity().Convert(GeneralInfoVM.Unit, Units.TONCM).AsLippedSection().AsEgyptMomentResistance(material, bracingConditions).Convert(Units.TONCM, GeneralInfoVM.Unit);
            //        IsCheckOutput = true;
            //        CheckOutputVM.CheckOutput = momentOut.AsCheck(CheckOutputVM.UltimateLoad);
            //        break;
            //    case StrainingActions.COMPRESSION:
            //        var compOut = GeneralInfoVM.IsUnstiffened ? GeometryVM.AsEntity().Convert(GeneralInfoVM.Unit, Units.TONCM).AsUnStiffenedSection().AsEgyptCompressionResistance(material, bracingConditions).Convert(Units.TONCM, GeneralInfoVM.Unit)
            //                                    : GeometryVM.AsEntity().Convert(GeneralInfoVM.Unit, Units.TONCM).AsLippedSection().AsEgyptCompressionResistance(material, bracingConditions).Convert(Units.TONCM, GeneralInfoVM.Unit);
            //        IsCheckOutput=true;
            //        CheckOutputVM.CheckOutput = compOut.AsCheck(CheckOutputVM.UltimateLoad);
            //        break;
            //}
        }

        #endregion

    }
}