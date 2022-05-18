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

//namespace ColdFormedChannelSection.App.ViewModels
//{
//    internal class AISICodeResistanceViewModel:ResistanceBaseViewModel
//    {

//        #region Properties

//        public override ICommand ResultsCommand { get; }


//        #endregion

//        #region Cosntructors

//        public AISICodeResistanceViewModel(GeneralInfoViewModel generalInfoVM,BracingConditionsViewModel bracingConditionsVM,GeometryViewModel geometryVM, MaterialViewModel materialVM,InputLoadViewModel inputLoadVM)
//            :base(generalInfoVM, bracingConditionsVM,geometryVM,materialVM, inputLoadVM)
//        {
//            ResultsCommand = new RelayCommand(OnResults,CanResults);
//            IsResistanceOutput = false;
//            switch (GeneralInfoVM.StrainingAction)
//            {
//                case StrainingActions.MOMENT:
//                    BracingConditionsVM.IsC1Used = false;
//                    BracingConditionsVM.C1 = 0;
//                    BracingConditionsVM.IsLuUsed = true;
//                    BracingConditionsVM.IsCbUsed = true;
//                    break;
//                case StrainingActions.COMPRESSION:
//                    BracingConditionsVM.IsC1Used = false;
//                    BracingConditionsVM.C1 = 0;
//                    BracingConditionsVM.IsLuUsed = false;
//                    BracingConditionsVM.Lu = 0;
//                    BracingConditionsVM.IsCbUsed = false;
//                    BracingConditionsVM.Cb = 0;
//                    break;
//            }
//        }

//        #endregion

//        #region Methods

//        private bool CanResults()
//        {
//            return true;
//        }

//        private void OnResults()
//        {
//            IsResistanceOutput = false;
//            //var material = (new Material(GeneralInfoVM.Fy, GeneralInfoVM.E, 0.3)).Convert(GeneralInfoVM.Unit,Units.KIPINCH);
//            var bracingConditions = BracingConditionsVM.AsEntity().Convert(GeneralInfoVM.Unit,Units.KIPINCH);
//            //switch (GeneralInfoVM.StrainingAction)
//            //{
//            //    case StrainingActions.MOMENT:
//            //        var momentOut = GeneralInfoVM.IsUnstiffened ? GeometryVM.AsEntity().Convert(GeneralInfoVM.Unit,Units.KIPINCH).AsUnStiffenedSection().AsAISIMomentResistance(material, bracingConditions).Convert(Units.KIPINCH, GeneralInfoVM.Unit)
//            //                                      : GeometryVM.AsEntity().Convert(GeneralInfoVM.Unit,Units.KIPINCH).AsLippedSection().AsAISIMomentResistance(material, bracingConditions).Convert(Units.KIPINCH, GeneralInfoVM.Unit);
//            //        IsResistanceOutput = true;
//            //        ResistanceOutput = momentOut;
//            //        break;
//            //    case StrainingActions.COMPRESSION:
//            //        var compOut = GeneralInfoVM.IsUnstiffened ? GeometryVM.AsEntity().Convert(GeneralInfoVM.Unit,Units.KIPINCH).AsUnStiffenedSection().AsAISICompressionResistance(material, bracingConditions).Convert(Units.KIPINCH, GeneralInfoVM.Unit)
//            //                                    : GeometryVM.AsEntity().Convert(GeneralInfoVM.Unit,Units.KIPINCH).AsLippedSection().AsAISICompressionResistance(material, bracingConditions).Convert(Units.KIPINCH, GeneralInfoVM.Unit);
//            //        IsResistanceOutput = true;
//            //        ResistanceOutput = compOut;
//            //        break;
//            //}
//        }


//        #endregion

//    }
//}
