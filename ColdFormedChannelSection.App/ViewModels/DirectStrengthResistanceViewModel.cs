using ColdFormedChannelSection.App.Extensions;
using ColdFormedChannelSection.App.ViewModels.Base;
using ColdFormedChannelSection.App.ViewModels.Enums;
using ColdFormedChannelSection.Core.Dtos;
using ColdFormedChannelSection.Core.Entities;
using ColdFormedChannelSection.Core.Enums;
using ColdFormedChannelSection.Core.Helpers;
using CSharp.Functional.Constructs;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Windows.Input;
using static ColdFormedChannelSection.Core.Constants;

namespace ColdFormedChannelSection.App.ViewModels
{
    internal class DirectStrengthResistanceViewModel:ResistanceBaseViewModel
    {


        #region Properties

        public override ICommand ResultsCommand { get; }

        #endregion


        #region Constructors
        public DirectStrengthResistanceViewModel(GeneralInfoViewModel generalInfoVM,BracingConditionsViewModel bracingConditionsVM,GeometryViewModel geometryVM)
            :base(generalInfoVM,bracingConditionsVM,geometryVM)
        {
            ResultsCommand = new RelayCommand(OnResults, CanResults);
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

        private bool CanResults()
        {
            return true;
        }

        private void OnResults()
        {
            //var dir =Path.GetDirectoryName( Assembly.GetExecutingAssembly().Location);
            //var filePath = $"{dir}\\{DATABASE_FOLDER}\\{EGYPT_UNSTIFF_TABLE}.csv";
            //var data = filePath.ReadAsCsv<SectionDimensionDto>()
            //                   .Match(errs => new List<SectionDimensionDto>(), dt =>
            //                   {
            //                       return dt;
            //                   });
            IsResistanceOutput = false;
            var material = (new Material(GeneralInfoVM.Fy, GeneralInfoVM.E, 0.3)).Convert(GeneralInfoVM.Unit, Units.KIPINCH);
            var bracingConditions = BracingConditionsVM.AsEntity().Convert(GeneralInfoVM.Unit, Units.KIPINCH);
            switch (GeneralInfoVM.StrainingAction)
            {
                case StrainingActions.MOMENT:
                    var momentOut = GeneralInfoVM.IsUnstiffened ? GeometryVM.AsEntity().Convert(GeneralInfoVM.Unit, Units.KIPINCH).AsUnStiffenedSection().AsDSMomentResistance(material, bracingConditions).Convert(Units.KIPINCH, GeneralInfoVM.Unit)
                                                  : GeometryVM.AsEntity().Convert(GeneralInfoVM.Unit, Units.KIPINCH).AsLippedSection().AsDSMomentResistance(material, bracingConditions).Convert(Units.KIPINCH, GeneralInfoVM.Unit);
                    IsResistanceOutput = true;
                    ResistanceOutput = momentOut;
                    break;
                case StrainingActions.COMPRESSION:
                    var compOut = GeneralInfoVM.IsUnstiffened ? GeometryVM.AsEntity().Convert(GeneralInfoVM.Unit, Units.KIPINCH).AsUnStiffenedSection().AsDSCompressionResistance(material, bracingConditions).Convert(Units.KIPINCH, GeneralInfoVM.Unit)
                                                : GeometryVM.AsEntity().Convert(GeneralInfoVM.Unit, Units.KIPINCH).AsLippedSection().AsDSCompressionResistance(material, bracingConditions).Convert(Units.KIPINCH, GeneralInfoVM.Unit);
                    IsResistanceOutput = true;
                    ResistanceOutput = compOut;
                    break;
            }

        }


        #endregion

    }
}
