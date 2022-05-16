using ColdFormedChannelSection.App.Extensions;
using ColdFormedChannelSection.App.ViewModels.Base;
using ColdFormedChannelSection.App.ViewModels.Enums;
using ColdFormedChannelSection.Core.Entities;
using ColdFormedChannelSection.Core.Enums;
using ColdFormedChannelSection.Core.Extensions;
using ColdFormedChannelSection.Core.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace ColdFormedChannelSection.App.ViewModels
{
    public class EgyptianCodeDesignViewModel : DesignBaseViewModel
    {

        #region Properties

        public override ICommand ResultsCommand { get; }


        #endregion


        #region Constructors

        public EgyptianCodeDesignViewModel(GeneralInfoViewModel generalInfoVM, BracingConditionsViewModel bracingConditionsVM)
           : base(generalInfoVM, bracingConditionsVM)
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
            DesignOutputVM.IsDesignOutput = false;
            var material = (new Material(GeneralInfoVM.Fy, GeneralInfoVM.E, 0.3)).Convert(GeneralInfoVM.Unit, Units.KIPINCH);
            var bracingConditions = BracingConditionsVM.AsEntity().Convert(GeneralInfoVM.Unit, Units.KIPINCH);
            //switch (GeneralInfoVM.StrainingAction)
            //{
            //    case StrainingActions.MOMENT:

            //        if (GeneralInfoVM.IsUnstiffened)
            //        {
            //            var tuple = GeometryVM.Sections.Select(dto => Tuple.Create(dto, dto.AsEntity().Convert(GeneralInfoVM.Unit, Units.KIPINCH).AsUnStiffenedSection().AsEgyptMomentResistance(material, bracingConditions).Convert(Units.KIPINCH, GeneralInfoVM.Unit)))
            //                                                                 .Where(tuple => tuple.Item2.DesignResistance > DesignOutputVM.UltimateLoad)
            //                                                                 .OrderBy(tuple => tuple.Item2.DesignResistance)
            //                                                                 .FirstOrDefault();
            //            DesignOutputVM.IsDesignOutput = true;
            //            if (tuple != null)
            //            {
            //                (var secDto, var resistance) = tuple;
            //                GeometryVM.SelectedSection = secDto;
            //                DesignOutputVM.DesignOutput = resistance.AsDesign(DesignOutputVM.UltimateLoad, secDto.ID);
            //            }
            //            else
            //            {
            //                GeometryVM.SelectedSection = null;
            //                DesignOutputVM.DesignOutput = null;
            //            }
            //        }
            //        else
            //        {
            //            var tuple = GeometryVM.Sections.Select(dto => Tuple.Create(dto, dto.AsEntity().Convert(GeneralInfoVM.Unit, Units.KIPINCH).AsLippedSection().AsEgyptMomentResistance(material, bracingConditions).Convert(Units.KIPINCH, GeneralInfoVM.Unit)))
            //                                                                .Where(tuple => tuple.Item2.DesignResistance > DesignOutputVM.UltimateLoad)
            //                                                                .OrderBy(tuple => tuple.Item2.DesignResistance)
            //                                                                .FirstOrDefault();
            //            DesignOutputVM.IsDesignOutput = true;
            //            if (tuple != null)
            //            {
            //                (var secDto , var resistance) = tuple;  
            //                GeometryVM.SelectedSection = secDto;
            //                DesignOutputVM.DesignOutput = resistance.AsDesign(DesignOutputVM.UltimateLoad, secDto.ID);
            //            }
            //            else
            //            {
            //                GeometryVM.SelectedSection = null;
            //                DesignOutputVM.DesignOutput = null;
            //            }
            //        }

            //        break;
            //    case StrainingActions.COMPRESSION:
            //        if (GeneralInfoVM.IsUnstiffened)
            //        {
            //            (var secDto, var ressistance) = GeometryVM.Sections.Select(dto => Tuple.Create(dto, dto.AsEntity().Convert(GeneralInfoVM.Unit, Units.KIPINCH).AsUnStiffenedSection().AsEgyptCompressionResistance(material, bracingConditions).Convert(Units.KIPINCH, GeneralInfoVM.Unit)))
            //                                                                 .Where(tuple => tuple.Item2.DesignResistance > DesignOutputVM.UltimateLoad)
            //                                                                 .OrderBy(tuple => tuple.Item2.DesignResistance)
            //                                                                 .FirstOrDefault();
            //            DesignOutputVM.IsDesignOutput = true;
            //            if (secDto != null)
            //            {
            //                GeometryVM.SelectedSection = secDto;
            //                DesignOutputVM.DesignOutput = ressistance.AsDesign(DesignOutputVM.UltimateLoad, secDto.ID);
            //            }
            //            else
            //            {
            //                GeometryVM.SelectedSection = null;
            //                DesignOutputVM.DesignOutput = null;
            //            }
            //        }
            //        else
            //        {
            //            (var secDto, var ressistance) = GeometryVM.Sections.Select(dto => Tuple.Create(dto, dto.AsEntity().Convert(GeneralInfoVM.Unit, Units.KIPINCH).AsLippedSection().AsEgyptCompressionResistance(material, bracingConditions).Convert(Units.KIPINCH, GeneralInfoVM.Unit)))
            //                                                                .Where(tuple => tuple.Item2.DesignResistance > DesignOutputVM.UltimateLoad)
            //                                                                .OrderBy(tuple => tuple.Item2.DesignResistance)
            //                                                                .FirstOrDefault();
            //            DesignOutputVM.IsDesignOutput = true;
            //            if (secDto != null)
            //            {
            //                GeometryVM.SelectedSection = secDto;
            //                DesignOutputVM.DesignOutput = ressistance.AsDesign(DesignOutputVM.UltimateLoad, secDto.ID);
            //            }
            //            else
            //            {
            //                GeometryVM.SelectedSection = null;
            //                DesignOutputVM.DesignOutput = null;
            //            }
            //        }

            //        break;
            //}
        }

        #endregion

    }
}
