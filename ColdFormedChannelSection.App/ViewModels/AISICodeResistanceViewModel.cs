using ColdFormedChannelSection.App.Extensions;
using ColdFormedChannelSection.App.ViewModels.Base;
using ColdFormedChannelSection.App.ViewModels.Enums;
using ColdFormedChannelSection.Core.Entities;
using ColdFormedChannelSection.Core.Enums;
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

        public AISICodeResistanceViewModel(GeneralInfoViewModel generalInfoVM,BracingConditionsViewModel bracingConditionsVM,GeometryViewModel geometryVM)
            :base(generalInfoVM, bracingConditionsVM,geometryVM)
        {
            ResultsCommand = new RelayCommand(OnResults,CanResults);
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
            var material = (new Material(GeneralInfoVM.Fy, GeneralInfoVM.E, 0.3)).Convert(GeneralInfoVM.Unit,Units.KIPINCH);
            var bracingConditions = BracingConditionsVM.AsEntity().Convert(GeneralInfoVM.Unit,Units.KIPINCH);
            switch (GeneralInfoVM.StrainingAction)
            {
                case StrainingActions.MOMENT:
                    var momentOut = GeneralInfoVM.IsUnstiffened ? GeometryVM.AsEntity().Convert(GeneralInfoVM.Unit,Units.KIPINCH).AsUnStiffenedSection().AsAISIMomentResistance(material, bracingConditions).Convert(Units.KIPINCH, GeneralInfoVM.Unit)
                                                  : GeometryVM.AsEntity().Convert(GeneralInfoVM.Unit,Units.KIPINCH).AsLippedSection().AsAISIMomentResistance(material, bracingConditions).Convert(Units.KIPINCH, GeneralInfoVM.Unit);
                    IsResistanceOutput = true;
                    ResistanceOutput = momentOut;
                    break;
                case StrainingActions.COMPRESSION:
                    var compOut = GeneralInfoVM.IsUnstiffened ? GeometryVM.AsEntity().Convert(GeneralInfoVM.Unit,Units.KIPINCH).AsUnStiffenedSection().AsAISICompressionResistance(material, bracingConditions).Convert(Units.KIPINCH, GeneralInfoVM.Unit)
                                                : GeometryVM.AsEntity().Convert(GeneralInfoVM.Unit,Units.KIPINCH).AsLippedSection().AsAISICompressionResistance(material, bracingConditions).Convert(Units.KIPINCH, GeneralInfoVM.Unit);
                    IsResistanceOutput = true;
                    ResistanceOutput = compOut;
                    break;
            }
        }

        #endregion

    }
}
