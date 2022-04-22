using ColdFormedChannelSection.App.Extensions;
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
        public DirectStrengthResistanceViewModel(GeneralInfoViewModel generalInfoVM,BracingConditionsViewModel bracingConditionsVM,GeometryViewModel geometryVM)
            :base(generalInfoVM,bracingConditionsVM,geometryVM)
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
