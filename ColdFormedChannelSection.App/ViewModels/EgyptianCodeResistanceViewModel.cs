using ColdFormedChannelSection.App.Extensions;
using ColdFormedChannelSection.App.ViewModels.Base;
using ColdFormedChannelSection.App.ViewModels.Enums;
using ColdFormedChannelSection.App.ViewModels.Interfaces;
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
    internal class EgyptianCodeResistanceViewModel:ResistanceBaseViewModel, IImportLibrary
    {

        #region Private Fields

        private readonly Lazy<Validation<List<SectionDimensionDto>>> _unStiffSections = new Lazy<Validation<List<SectionDimensionDto>>>(LoadUnstiffSections);
        private readonly Lazy<Validation<List<SectionDimensionDto>>> _stiffSections = new Lazy<Validation<List<SectionDimensionDto>>>(LoadStiffSections);

        #endregion

        #region Properties

        public override ICommand ResultsCommand { get; }

        #endregion

        #region Constructors

        public EgyptianCodeResistanceViewModel(GeneralInfoViewModel generalInfoVM,BracingConditionsViewModel bracingConditionsVM,GeometryViewModel geometryVM)
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
            IsResistanceOutput = false;
            var material = (new Material(GeneralInfoVM.Fy, GeneralInfoVM.E, 0.3)).Convert(GeneralInfoVM.Unit, Units.TONCM);
            var bracingConditions = BracingConditionsVM.AsEntity().Convert(GeneralInfoVM.Unit, Units.TONCM);
            switch (GeneralInfoVM.StrainingAction)
            {
                case StrainingActions.MOMENT:
                    var momentOut = GeneralInfoVM.IsUnstiffened ? GeometryVM.AsEntity().Convert(GeneralInfoVM.Unit, Units.TONCM).AsUnStiffenedSection().AsEgyptMomentResistance(material, bracingConditions).Convert(Units.TONCM, GeneralInfoVM.Unit)
                                                  : GeometryVM.AsEntity().Convert(GeneralInfoVM.Unit, Units.TONCM).AsLippedSection().AsEgyptMomentResistance(material, bracingConditions).Convert(Units.TONCM, GeneralInfoVM.Unit);
                    IsResistanceOutput = true;
                    ResistanceOutput = momentOut;
                    break;
                case StrainingActions.COMPRESSION:
                    var compOut = GeneralInfoVM.IsUnstiffened ? GeometryVM.AsEntity().Convert(GeneralInfoVM.Unit, Units.TONCM).AsUnStiffenedSection().AsEgyptCompressionResistance(material, bracingConditions).Convert(Units.TONCM, GeneralInfoVM.Unit)
                                                : GeometryVM.AsEntity().Convert(GeneralInfoVM.Unit, Units.TONCM).AsLippedSection().AsEgyptCompressionResistance(material, bracingConditions).Convert(Units.TONCM, GeneralInfoVM.Unit);
                    IsResistanceOutput = true;
                    ResistanceOutput = compOut;
                    break;
            }

        }

        public void ImportSectionsFromLib(GeometryViewModel vm)
        {
            if (GeneralInfoVM.IsUnstiffened)
            {
                vm.Sections = _unStiffSections.Value.Match(errs => new List<SectionDimensionDto>(), dtos => dtos);
            }
            else
            {
                vm.Sections = _stiffSections.Value.Match(errs => new List<SectionDimensionDto>(), dtos => dtos);
            }
        }

        private static Validation<List<SectionDimensionDto>> LoadUnstiffSections()
        {
            var dir = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            var filePath = $"{dir}\\{DATABASE_FOLDER}\\{EGYPT_UNSTIFF_TABLE}.csv";
            return filePath.ReadAsCsv<SectionDimensionDto>();
        }

        private static Validation<List<SectionDimensionDto>> LoadStiffSections()
        {
            var dir = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            var filePath = $"{dir}\\{DATABASE_FOLDER}\\{EGYPT_STIFF_TABLE}.csv";
            return filePath.ReadAsCsv<SectionDimensionDto>();
        }

        #endregion

    }
}
