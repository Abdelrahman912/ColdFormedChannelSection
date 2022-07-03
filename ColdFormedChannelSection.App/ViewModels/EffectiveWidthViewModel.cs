using ColdFormedChannelSection.App.Extensions;
using ColdFormedChannelSection.App.ViewModels.Base;
using ColdFormedChannelSection.App.ViewModels.Enums;
using ColdFormedChannelSection.Core.Entities;
using ColdFormedChannelSection.Core.Enums;
using ColdFormedChannelSection.Core.Extensions;
using ColdFormedChannelSection.Core.Helpers;
using CSharp.Functional.Errors;
using CSharp.Functional.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Input;
using Unit = System.ValueTuple;
using static CSharp.Functional.Functional;
using static CSharp.Functional.Extensions.ValidationExtension;
using CSharp.Functional.Constructs;
using static ColdFormedChannelSection.Core.Errors.Errors;
using ColdFormedChannelSection.Core.Dtos;

namespace ColdFormedChannelSection.App.ViewModels
{
    public class EffectiveWidthViewModel : ResistanceBaseViewModel
    {

        #region Private Fields


        private readonly Dictionary<Module, Func<EffectiveWidthViewModel, Validation<Unit>>> _moduleDict;

        private List<Func<List<Error>>> _validateFuncs;

        private static readonly Dictionary<DesignCode, UnitSystems> _unitSystemDict = new Dictionary<DesignCode, UnitSystems>()
        {
            [DesignCode.EURO] = UnitSystems.NMM,
            [DesignCode.EGYPTIAN] = UnitSystems.TONCM,
            [DesignCode.AISI] = UnitSystems.KIPINCH
        };

        #endregion

        #region Properties

        public override ICommand ResultsCommand { get; }


        public Dictionary<DesignCode, Dictionary<KeyValuePair<StrainingActions, SteelSection>, Func<EffectiveWidthViewModel, Material, LengthBracingConditions, OutputBase>>> DesignDict { get; }

        public Dictionary<KeyValuePair<StrainingActions, SteelSection>, Func<EffectiveWidthViewModel, Material, LengthBracingConditions, OutputBase>> DesignEgyptDict { get; }
        public Dictionary<KeyValuePair<StrainingActions, SteelSection>, Func<EffectiveWidthViewModel, Material, LengthBracingConditions, OutputBase>> DesignEuroDict { get; }
        public Dictionary<KeyValuePair<StrainingActions, SteelSection>, Func<EffectiveWidthViewModel, Material, LengthBracingConditions, OutputBase>> DesignAISIDict { get; }


        public Dictionary<DesignCode, Dictionary<KeyValuePair<StrainingActions, SteelSection>, Func<EffectiveWidthViewModel, Material, LengthBracingConditions, Validation<Tuple<Func<OutputBase>, OutputBase>>>>> ResistDict { get; }

        public Dictionary<KeyValuePair<StrainingActions, SteelSection>, Func<EffectiveWidthViewModel, Material, LengthBracingConditions, Validation<Tuple<Func<OutputBase>, OutputBase>>>> ResistEgyptDict { get; }
        public Dictionary<KeyValuePair<StrainingActions, SteelSection>, Func<EffectiveWidthViewModel, Material, LengthBracingConditions, Validation<Tuple<Func<OutputBase>, OutputBase>>>> ResistEuroDict { get; }
        public Dictionary<KeyValuePair<StrainingActions, SteelSection>, Func<EffectiveWidthViewModel, Material, LengthBracingConditions, Validation<Tuple<Func<OutputBase>, OutputBase>>>> ResistAISIDict { get; }


        #endregion

        #region Constructors

        public EffectiveWidthViewModel(GeneralInfoViewModel generalInfoVM, BracingConditionsViewModel bracingConditionsVM, GeometryViewModel geometryVM, MaterialViewModel materialVM, InputLoadViewModel inputLoadVM, Func<List<Error>, Unit> showErrorsService, Action<ReportViewModel> reportService, Func<Func<string, Exceptional<Unit>>, Option<Exceptional<Unit>>> folderDialogService)
          : base(generalInfoVM, bracingConditionsVM, geometryVM, materialVM, inputLoadVM, showErrorsService, reportService, folderDialogService)
        {
            ResultsCommand = new RelayCommand(OnReults, CanResults);
            _moduleDict = new Dictionary<Module, Func<EffectiveWidthViewModel, Validation<Unit>>>()
            {
                [Module.RESISTANCE] = Resistance,
                [Module.DESIGN] = Design,
                [Module.CHECK] = Check
            };
            ResistEgyptDict = new Dictionary<KeyValuePair<StrainingActions, SteelSection>, Func<EffectiveWidthViewModel, Material, LengthBracingConditions, Validation<Tuple<Func<OutputBase>, OutputBase>>>>()
            {
                [KeyValuePair.Create(StrainingActions.COMPRESSION, SteelSection.C_UNSTIFFENED)] = EgyptResistanceCUnstiffComp,
                [KeyValuePair.Create(StrainingActions.MOMENT, SteelSection.C_UNSTIFFENED)] = EgyptResistanceCUnstiffMoment,
                [KeyValuePair.Create(StrainingActions.MOMENT_COMPRESSION, SteelSection.C_UNSTIFFENED)] = EgyptResistanceCUnstiffMomentCompression,
                [KeyValuePair.Create(StrainingActions.COMPRESSION, SteelSection.C_LIPPED)] = EgyptResistanceCLippedComp,
                [KeyValuePair.Create(StrainingActions.MOMENT, SteelSection.C_LIPPED)] = EgyptResistanceCLippedMoment,
                [KeyValuePair.Create(StrainingActions.MOMENT_COMPRESSION, SteelSection.C_LIPPED)] = EgyptResistanceCLippedMomentCompression,

                [KeyValuePair.Create(StrainingActions.COMPRESSION, SteelSection.Z_UNSTIFFENED)] = EgyptResistanceZUnstiffComp,
                [KeyValuePair.Create(StrainingActions.MOMENT, SteelSection.Z_UNSTIFFENED)] = EgyptResistanceZUnstiffMoment,
                [KeyValuePair.Create(StrainingActions.MOMENT_COMPRESSION, SteelSection.Z_UNSTIFFENED)] = EgyptResistanceZUnstiffMomentCompression,
                [KeyValuePair.Create(StrainingActions.COMPRESSION, SteelSection.Z_LIPPED)] = EgyptResistanceZLippedComp,
                [KeyValuePair.Create(StrainingActions.MOMENT, SteelSection.Z_LIPPED)] = EgyptResistanceZLippedMoment,
                [KeyValuePair.Create(StrainingActions.MOMENT_COMPRESSION, SteelSection.Z_LIPPED)] = EgyptResistanceZLippedMomentCompression,
            };

            ResistEuroDict = new Dictionary<KeyValuePair<StrainingActions, SteelSection>, Func<EffectiveWidthViewModel, Material, LengthBracingConditions, Validation<Tuple<Func<OutputBase>, OutputBase>>>>()
            {
                [KeyValuePair.Create(StrainingActions.COMPRESSION, SteelSection.C_UNSTIFFENED)] = EuroResistanceCUnstiffComp,
                [KeyValuePair.Create(StrainingActions.MOMENT, SteelSection.C_UNSTIFFENED)] = EuroResistanceCUnstiffMoment,
                [KeyValuePair.Create(StrainingActions.MOMENT_COMPRESSION, SteelSection.C_UNSTIFFENED)] = EuroResistanceCUnstiffMomentCompression,
                [KeyValuePair.Create(StrainingActions.COMPRESSION, SteelSection.C_LIPPED)] = EuroResistanceCLippedComp,
                [KeyValuePair.Create(StrainingActions.MOMENT, SteelSection.C_LIPPED)] = EuroResistanceCLippedMoment,
                [KeyValuePair.Create(StrainingActions.MOMENT_COMPRESSION, SteelSection.C_LIPPED)] = EuroResistanceCLippedMomentCompression,

                [KeyValuePair.Create(StrainingActions.COMPRESSION, SteelSection.Z_UNSTIFFENED)] = EuroResistanceZUnstiffComp,
                [KeyValuePair.Create(StrainingActions.MOMENT, SteelSection.Z_UNSTIFFENED)] = EuroResistanceZUnstiffMoment,
                [KeyValuePair.Create(StrainingActions.MOMENT_COMPRESSION, SteelSection.Z_UNSTIFFENED)] = EuroResistanceZUnstiffMomentCompression,
                [KeyValuePair.Create(StrainingActions.COMPRESSION, SteelSection.Z_LIPPED)] = EuroResistanceZLippedComp,
                [KeyValuePair.Create(StrainingActions.MOMENT, SteelSection.Z_LIPPED)] = EuroResistanceZLippedMoment,
                [KeyValuePair.Create(StrainingActions.MOMENT_COMPRESSION, SteelSection.Z_LIPPED)] = EuroResistanceZLippedMomentCompression,
            };

            ResistAISIDict = new Dictionary<KeyValuePair<StrainingActions, SteelSection>, Func<EffectiveWidthViewModel, Material, LengthBracingConditions, Validation<Tuple<Func<OutputBase>, OutputBase>>>>()
            {
                [KeyValuePair.Create(StrainingActions.COMPRESSION, SteelSection.C_UNSTIFFENED)] = AISIResistanceCUnstiffComp,
                [KeyValuePair.Create(StrainingActions.MOMENT, SteelSection.C_UNSTIFFENED)] = AISIResistanceCUnstiffMoment,
                [KeyValuePair.Create(StrainingActions.MOMENT_COMPRESSION, SteelSection.C_UNSTIFFENED)] = AISIResistanceCUnstiffMomentCompression,
                [KeyValuePair.Create(StrainingActions.COMPRESSION, SteelSection.C_LIPPED)] = AISIResistanceCLippedComp,
                [KeyValuePair.Create(StrainingActions.MOMENT, SteelSection.C_LIPPED)] = AISIResistanceCLippedMoment,
                [KeyValuePair.Create(StrainingActions.MOMENT_COMPRESSION, SteelSection.C_LIPPED)] = AISIResistanceCLippedMomentCompression,

                [KeyValuePair.Create(StrainingActions.COMPRESSION, SteelSection.Z_UNSTIFFENED)] = AISIResistanceZUnstiffComp,
                [KeyValuePair.Create(StrainingActions.MOMENT, SteelSection.Z_UNSTIFFENED)] = AISIResistanceZUnstiffMoment,
                [KeyValuePair.Create(StrainingActions.MOMENT_COMPRESSION, SteelSection.Z_UNSTIFFENED)] = AISIResistanceZUnstiffMomentCompression,
                [KeyValuePair.Create(StrainingActions.COMPRESSION, SteelSection.Z_LIPPED)] = AISIResistanceZLippedComp,
                [KeyValuePair.Create(StrainingActions.MOMENT, SteelSection.Z_LIPPED)] = AISIResistanceZLippedMoment,
                [KeyValuePair.Create(StrainingActions.MOMENT_COMPRESSION, SteelSection.Z_LIPPED)] = AISIResistanceZLippedMomentCompression
            };
            ResistDict = new Dictionary<DesignCode, Dictionary<KeyValuePair<StrainingActions, SteelSection>, Func<EffectiveWidthViewModel, Material, LengthBracingConditions, Validation<Tuple<Func<OutputBase>, OutputBase>>>>>()
            {
                [DesignCode.EGYPTIAN] = ResistEgyptDict,
                [DesignCode.EURO] = ResistEuroDict,
                [DesignCode.AISI] = ResistAISIDict
            };
            DesignEgyptDict = new Dictionary<KeyValuePair<StrainingActions, SteelSection>, Func<EffectiveWidthViewModel, Material, LengthBracingConditions, OutputBase>>()
            {
                [KeyValuePair.Create(StrainingActions.COMPRESSION, SteelSection.C_UNSTIFFENED)] = EgyptDesignCUnstiffComp,
                [KeyValuePair.Create(StrainingActions.MOMENT, SteelSection.C_UNSTIFFENED)] = EgyptDesignCUnstiffMoment,
                [KeyValuePair.Create(StrainingActions.MOMENT_COMPRESSION, SteelSection.C_UNSTIFFENED)] = EgyptDesignCUnstiffMomentCompression,
                [KeyValuePair.Create(StrainingActions.COMPRESSION, SteelSection.C_LIPPED)] = EgyptDesignCLippedComp,
                [KeyValuePair.Create(StrainingActions.MOMENT, SteelSection.C_LIPPED)] = EgyptDesignCLippedMoment,
                [KeyValuePair.Create(StrainingActions.MOMENT_COMPRESSION, SteelSection.C_LIPPED)] = EgyptDesignCLippedMomentCompression,

                [KeyValuePair.Create(StrainingActions.COMPRESSION, SteelSection.Z_UNSTIFFENED)] = EgyptDesignZUnstiffComp,
                [KeyValuePair.Create(StrainingActions.MOMENT, SteelSection.Z_UNSTIFFENED)] = EgyptDesignZUnstiffMoment,
                [KeyValuePair.Create(StrainingActions.MOMENT_COMPRESSION, SteelSection.Z_UNSTIFFENED)] = EgyptDesignZUnstiffMomentCompression,
                [KeyValuePair.Create(StrainingActions.COMPRESSION, SteelSection.Z_LIPPED)] = EgyptDesignZLippedComp,
                [KeyValuePair.Create(StrainingActions.MOMENT, SteelSection.Z_LIPPED)] = EgyptDesignZLippedMoment,
                [KeyValuePair.Create(StrainingActions.MOMENT_COMPRESSION, SteelSection.Z_LIPPED)] = EgyptDesignZLippedMomentCompression,
            };
            DesignEuroDict = new Dictionary<KeyValuePair<StrainingActions, SteelSection>, Func<EffectiveWidthViewModel, Material, LengthBracingConditions, OutputBase>>()
            {
                [KeyValuePair.Create(StrainingActions.COMPRESSION, SteelSection.C_UNSTIFFENED)] = EuroDesignCUnstiffComp,
                [KeyValuePair.Create(StrainingActions.MOMENT, SteelSection.C_UNSTIFFENED)] = EuroDesignCUnstiffMoment,
                [KeyValuePair.Create(StrainingActions.MOMENT_COMPRESSION, SteelSection.C_UNSTIFFENED)] = EuroDesignCUnstiffMomentCompression,
                [KeyValuePair.Create(StrainingActions.COMPRESSION, SteelSection.C_LIPPED)] = EuroDesignCLippedComp,
                [KeyValuePair.Create(StrainingActions.MOMENT, SteelSection.C_LIPPED)] = EuroDesignCLippedMoment,
                [KeyValuePair.Create(StrainingActions.MOMENT_COMPRESSION, SteelSection.C_LIPPED)] = EuroDesignCLippedMomentCompression,

                [KeyValuePair.Create(StrainingActions.COMPRESSION, SteelSection.Z_UNSTIFFENED)] = EuroDesignZUnstiffComp,
                [KeyValuePair.Create(StrainingActions.MOMENT, SteelSection.Z_UNSTIFFENED)] = EuroDesignZUnstiffMoment,
                [KeyValuePair.Create(StrainingActions.MOMENT_COMPRESSION, SteelSection.Z_UNSTIFFENED)] = EuroDesignZUnstiffMomentCompression,
                [KeyValuePair.Create(StrainingActions.COMPRESSION, SteelSection.Z_LIPPED)] = EuroDesignZLippedComp,
                [KeyValuePair.Create(StrainingActions.MOMENT, SteelSection.Z_LIPPED)] = EuroDesignZLippedMoment,
                [KeyValuePair.Create(StrainingActions.MOMENT_COMPRESSION, SteelSection.Z_LIPPED)] = EuroDesignZLippedMomentCompression,
            };
            DesignAISIDict = new Dictionary<KeyValuePair<StrainingActions, SteelSection>, Func<EffectiveWidthViewModel, Material, LengthBracingConditions, OutputBase>>()
            {
                [KeyValuePair.Create(StrainingActions.COMPRESSION, SteelSection.C_UNSTIFFENED)] = AISIDesignCUnstiffComp,
                [KeyValuePair.Create(StrainingActions.MOMENT, SteelSection.C_UNSTIFFENED)] = AISIDesignCUnstiffMoment,
                [KeyValuePair.Create(StrainingActions.MOMENT_COMPRESSION, SteelSection.C_UNSTIFFENED)] = AISIDesignCUnstiffMomentCompression,
                [KeyValuePair.Create(StrainingActions.COMPRESSION, SteelSection.C_LIPPED)] = AISIDesignCLippedComp,
                [KeyValuePair.Create(StrainingActions.MOMENT, SteelSection.C_LIPPED)] = AISIDesignCLippedMoment,
                [KeyValuePair.Create(StrainingActions.MOMENT_COMPRESSION, SteelSection.C_LIPPED)] = AISIDesignCLippedMomentCompression,

                [KeyValuePair.Create(StrainingActions.COMPRESSION, SteelSection.Z_UNSTIFFENED)] = AISIDesignZUnstiffComp,
                [KeyValuePair.Create(StrainingActions.MOMENT, SteelSection.Z_UNSTIFFENED)] = AISIDesignZUnstiffMoment,
                [KeyValuePair.Create(StrainingActions.MOMENT_COMPRESSION, SteelSection.Z_UNSTIFFENED)] = AISIDesignZUnstiffMomentCompression,
                [KeyValuePair.Create(StrainingActions.COMPRESSION, SteelSection.Z_LIPPED)] = AISIDesignZLippedComp,
                [KeyValuePair.Create(StrainingActions.MOMENT, SteelSection.Z_LIPPED)] = AISIDesignZLippedMoment,
                [KeyValuePair.Create(StrainingActions.MOMENT_COMPRESSION, SteelSection.Z_LIPPED)] = AISIDesignZLippedMomentCompression,
            };
            DesignDict = new Dictionary<DesignCode, Dictionary<KeyValuePair<StrainingActions, SteelSection>, Func<EffectiveWidthViewModel, Material, LengthBracingConditions, OutputBase>>>()
            {
                [DesignCode.EGYPTIAN] = DesignEgyptDict,
                [DesignCode.EURO] = DesignEuroDict,
                [DesignCode.AISI] = DesignAISIDict
            };
            _validateFuncs = new List<Func<List<Error>>>()
            {
                materialVM.Validate,
                GeometryVM.Validate,
                InputLoadVM.Validate,
                BracingConditionsVM.Validate
            };
        }





        #endregion

        #region Methods

        private bool CanResults()
        {
            //TODO: add some kind of validation logic.
            return true;
        }

        private void OnReults()
        {
            Func<Validation<Unit>> designValid = () =>
            {
                return   _moduleDict[GeneralInfoVM.RunningModule](this).Bind<Unit, Unit>(val =>
                  {
                      if (ResistanceOutput == null)
                          return CantFindSafeSection;
                      else if (ResistanceOutput.Report == null)
                          return CantCalculateNominalStrength;
                      else
                      {
                          Report = ResistanceOutput.Report.Convert(GeneralInfoVM.Unit);
                          IsDisplayReport = true;
                          return Unit();
                      }
                  });
            };

            _validateFuncs.SelectMany(f => f())
                          .AsUnitValidation()
                          .Bind(_ => designValid())
                          .Match(errs => { IsDisplayReport = false; ShowErrorsService(errs.ToList()); return Unit(); },
                                 u => u);
        }

        private static Validation<Unit> Design(EffectiveWidthViewModel vm)
        {
            vm.IsResistanceOutput = false;
            var material = (new Material(vm.MaterialVM.Fy, vm.MaterialVM.E, 0.3)).Convert(vm.GeneralInfoVM.Unit, _unitSystemDict[vm.GeneralInfoVM.DesignCode]);
            var bracingConditions = vm.BracingConditionsVM.AsEntity().Convert(vm.GeneralInfoVM.Unit, _unitSystemDict[vm.GeneralInfoVM.DesignCode]);
            var sa = vm.GeneralInfoVM.StrainingAction;
            var section = vm.GeneralInfoVM.SteelSection;
            vm.ResistanceOutput = vm.DesignDict[vm.GeneralInfoVM.DesignCode][KeyValuePair.Create(sa, section)](vm, material, bracingConditions);
            return Unit();
        }

        #region Egypt Design

        #region C Sections

        private OutputBase EgyptDesignCLippedMomentCompression(EffectiveWidthViewModel vm, Material material, LengthBracingConditions bracingConditions)
        {
            var outTuple = vm.GeometryVM.Sections.Select(dto => Tuple.Create(dto, dto.AsEntity().Convert(vm.GeometryVM.TableUnit, UnitSystems.TONCM).AsLippedCSection().Bind(c => c.AsEgyptInteractionResistance(material, bracingConditions, vm.InputLoadVM.UltimateLoad.ConvertForce(vm.GeneralInfoVM.Unit, UnitSystems.TONCM).Item1, vm.InputLoadVM.UltimateMoment.ConvertMoment(vm.GeneralInfoVM.Unit, UnitSystems.TONCM).Item1).Map(v => v.Convert(UnitSystems.TONCM, vm.GeneralInfoVM.Unit)))))
                                                                   .Aggregate(new List<Tuple<SectionDimensionDto, ResistanceInteractionOutput>>(), (soFar, current) =>
                                                                   {
                                                                       current.Item2.Match(errs => false, val =>
                                                                       {
                                                                           Tuple.Create(current.Item1, val);
                                                                           return true;
                                                                       });
                                                                       return soFar;
                                                                   })
                                                                  .Where(tuple => tuple.Item2.IEValue < 1)
                                                                   .OrderByDescending(tuple => tuple.Item2.IEValue)
                                                                   .FirstOrDefault();
            vm.IsResistanceOutput = true;
            if (outTuple != null)
            {
                (var secDto, var ressistance) = outTuple;
                vm.GeometryVM.SelectedSection = secDto;
                return ressistance.AsDesign(secDto.ID, _momentDict[vm.GeneralInfoVM.Unit], _compDict[vm.GeneralInfoVM.Unit]);
            }
            else
            {
                vm.GeometryVM.SelectedSection = null;
                return null;
            }
        }

        private OutputBase EgyptDesignCUnstiffMomentCompression(EffectiveWidthViewModel vm, Material material, LengthBracingConditions bracingConditions)
        {
            var outTuple = vm.GeometryVM.Sections.Select(dto => Tuple.Create(dto, dto.AsEntity().Convert(vm.GeometryVM.TableUnit, UnitSystems.TONCM).AsUnstiffenedCSection().Bind(c => c.AsEgyptInteractionResistance(material, bracingConditions, vm.InputLoadVM.UltimateLoad.ConvertForce(vm.GeneralInfoVM.Unit, UnitSystems.TONCM).Item1, vm.InputLoadVM.UltimateMoment.ConvertMoment(vm.GeneralInfoVM.Unit, UnitSystems.TONCM).Item1).Map(v => v.Convert(UnitSystems.TONCM, vm.GeneralInfoVM.Unit)))))
                                                                    .Aggregate(new List<Tuple<SectionDimensionDto, ResistanceInteractionOutput>>(), (soFar, current) =>
                                                                    {
                                                                        current.Item2.Match(errs => false, val =>
                                                                        {
                                                                            Tuple.Create(current.Item1, val);
                                                                            return true;
                                                                        });
                                                                        return soFar;
                                                                    })
                                                                  .Where(tuple => tuple.Item2.IEValue < 1)
                                                                   .OrderByDescending(tuple => tuple.Item2.IEValue)
                                                                   .FirstOrDefault();
            vm.IsResistanceOutput = true;
            if (outTuple != null)
            {
                (var secDto, var ressistance) = outTuple;
                vm.GeometryVM.SelectedSection = secDto;
                return ressistance.AsDesign(secDto.ID, _momentDict[vm.GeneralInfoVM.Unit], _compDict[vm.GeneralInfoVM.Unit]);
            }
            else
            {
                vm.GeometryVM.SelectedSection = null;
                return null;
            }
        }

        private static DesignOutput EgyptDesignCUnstiffComp(EffectiveWidthViewModel vm, Material material, LengthBracingConditions bracingConditions)
        {
            var outTuple = vm.GeometryVM.Sections.Select(dto => Tuple.Create(dto, dto.AsEntity().Convert(vm.GeometryVM.TableUnit, UnitSystems.TONCM).AsUnstiffenedCSection().Bind(c => c.AsEgyptCompressionResistance(material, bracingConditions).Map(v => v.Convert(UnitSystems.TONCM, vm.GeneralInfoVM.Unit)))))
                                                                     .Aggregate(new List<Tuple<SectionDimensionDto, CompressionResistanceOutput>>(), (soFar, current) =>
                                                                     {
                                                                         current.Item2.Match(errs => false, val =>
                                                                         {
                                                                             Tuple.Create(current.Item1, val);
                                                                             return true;
                                                                         });
                                                                         return soFar;
                                                                     })
                                                                   .Where(tuple => tuple.Item2.DesignResistance > vm.InputLoadVM.UltimateLoad)
                                                                    .OrderBy(tuple => tuple.Item2.DesignResistance)
                                                                     .FirstOrDefault();
            vm.IsResistanceOutput = true;
            if (outTuple != null)
            {
                (var secDto, var ressistance) = outTuple;
                vm.GeometryVM.SelectedSection = secDto;
                return ressistance.AsDesign(vm.InputLoadVM.UltimateLoad, secDto.ID, _compDict[vm.GeneralInfoVM.Unit]);
            }
            else
            {
                vm.GeometryVM.SelectedSection = null;
                return null;
            }
        }

        private static DesignOutput EgyptDesignCLippedComp(EffectiveWidthViewModel vm, Material material, LengthBracingConditions bracingConditions)
        {
            var outTuple = vm.GeometryVM.Sections.Select(dto => Tuple.Create(dto, dto.AsEntity().Convert(vm.GeometryVM.TableUnit, UnitSystems.TONCM).AsLippedCSection().Bind(c => c.AsEgyptCompressionResistance(material, bracingConditions).Map(v => v.Convert(UnitSystems.TONCM, vm.GeneralInfoVM.Unit)))))
                                                                    .Aggregate(new List<Tuple<SectionDimensionDto, CompressionResistanceOutput>>(), (soFar, current) =>
                                                                    {
                                                                        current.Item2.Match(errs => false, val =>
                                                                        {
                                                                            Tuple.Create(current.Item1, val);
                                                                            return true;
                                                                        });
                                                                        return soFar;
                                                                    })
                                                                  .Where(tuple => tuple.Item2.DesignResistance > vm.InputLoadVM.UltimateLoad)
                                                                   .OrderBy(tuple => tuple.Item2.DesignResistance)
                                                                    .FirstOrDefault();
            vm.IsResistanceOutput = true;
            if (outTuple != null)
            {
                (var secDto, var ressistance) = outTuple;
                vm.GeometryVM.SelectedSection = secDto;
                return ressistance.AsDesign(vm.InputLoadVM.UltimateLoad, secDto.ID, _compDict[vm.GeneralInfoVM.Unit]);
            }
            else
            {
                vm.GeometryVM.SelectedSection = null;
                return null;
            }
        }

        private static DesignOutput EgyptDesignCUnstiffMoment(EffectiveWidthViewModel vm, Material material, LengthBracingConditions bracingConditions)
        {
            var outTuple = vm.GeometryVM.Sections.Select(dto => Tuple.Create(dto, dto.AsEntity().Convert(vm.GeometryVM.TableUnit, UnitSystems.TONCM).AsUnstiffenedCSection().Bind(c => c.AsEgyptMomentResistance(material, bracingConditions).Map(v => v.Convert(UnitSystems.TONCM, vm.GeneralInfoVM.Unit)))))
                                                                    .Aggregate(new List<Tuple<SectionDimensionDto, MomentResistanceOutput>>(), (soFar, current) =>
                                                                    {
                                                                        current.Item2.Match(errs => false, val =>
                                                                        {
                                                                            Tuple.Create(current.Item1, val);
                                                                            return true;
                                                                        });
                                                                        return soFar;
                                                                    })
                                                                  .Where(tuple => tuple.Item2.DesignResistance > vm.InputLoadVM.UltimateMoment)
                                                                   .OrderBy(tuple => tuple.Item2.DesignResistance)
                                                                    .FirstOrDefault();
            vm.IsResistanceOutput = true;
            if (outTuple != null)
            {
                (var secDto, var ressistance) = outTuple;
                vm.GeometryVM.SelectedSection = secDto;
                return ressistance.AsDesign(vm.InputLoadVM.UltimateMoment, secDto.ID, _momentDict[vm.GeneralInfoVM.Unit]);
            }
            else
            {
                vm.GeometryVM.SelectedSection = null;
                return null;
            }
        }

        private static DesignOutput EgyptDesignCLippedMoment(EffectiveWidthViewModel vm, Material material, LengthBracingConditions bracingConditions)
        {
            var outTuple = vm.GeometryVM.Sections.Select(dto => Tuple.Create(dto, dto.AsEntity().Convert(vm.GeometryVM.TableUnit, UnitSystems.TONCM).AsLippedCSection().Bind(c => c.AsEgyptMomentResistance(material, bracingConditions).Map(v => v.Convert(UnitSystems.TONCM, vm.GeneralInfoVM.Unit)))))
                                                                    .Aggregate(new List<Tuple<SectionDimensionDto, MomentResistanceOutput>>(), (soFar, current) =>
                                                                    {
                                                                        current.Item2.Match(errs => false, val =>
                                                                        {
                                                                            Tuple.Create(current.Item1, val);
                                                                            return true;
                                                                        });
                                                                        return soFar;
                                                                    })
                                                                  .Where(tuple => tuple.Item2.DesignResistance > vm.InputLoadVM.UltimateMoment)
                                                                   .OrderBy(tuple => tuple.Item2.DesignResistance)
                                                                    .FirstOrDefault();
            vm.IsResistanceOutput = true;
            if (outTuple != null)
            {
                (var secDto, var ressistance) = outTuple;
                vm.GeometryVM.SelectedSection = secDto;
                return ressistance.AsDesign(vm.InputLoadVM.UltimateMoment, secDto.ID, _momentDict[vm.GeneralInfoVM.Unit]);
            }
            else
            {
                vm.GeometryVM.SelectedSection = null;
                return null;
            }
        }


        #endregion

        #region Z Sections

        private OutputBase EgyptDesignZLippedMomentCompression(EffectiveWidthViewModel vm, Material material, LengthBracingConditions bracingConditions)
        {
            var outTuple = vm.GeometryVM.Sections.Select(dto => Tuple.Create(dto, dto.AsEntity().Convert(vm.GeometryVM.TableUnit, UnitSystems.TONCM).AsLippedZSection().Bind(c => c.AsEgyptInteractionResistance(material, bracingConditions, vm.InputLoadVM.UltimateLoad.ConvertForce(vm.GeneralInfoVM.Unit, UnitSystems.TONCM).Item1, vm.InputLoadVM.UltimateMoment.ConvertMoment(vm.GeneralInfoVM.Unit, UnitSystems.TONCM).Item1).Map(v => v.Convert(UnitSystems.TONCM, vm.GeneralInfoVM.Unit)))))
                                                                   .Aggregate(new List<Tuple<SectionDimensionDto, ResistanceInteractionOutput>>(), (soFar, current) =>
                                                                   {
                                                                       current.Item2.Match(errs => false, val =>
                                                                       {
                                                                           Tuple.Create(current.Item1, val);
                                                                           return true;
                                                                       });
                                                                       return soFar;
                                                                   })
                                                                  .Where(tuple => tuple.Item2.IEValue < 1)
                                                                   .OrderByDescending(tuple => tuple.Item2.IEValue)
                                                                   .FirstOrDefault();
            vm.IsResistanceOutput = true;
            if (outTuple != null)
            {
                (var secDto, var ressistance) = outTuple;
                vm.GeometryVM.SelectedSection = secDto;
                return ressistance.AsDesign(secDto.ID, _momentDict[vm.GeneralInfoVM.Unit], _compDict[vm.GeneralInfoVM.Unit]);
            }
            else
            {
                vm.GeometryVM.SelectedSection = null;
                return null;
            }
        }

        private OutputBase EgyptDesignZUnstiffMomentCompression(EffectiveWidthViewModel vm, Material material, LengthBracingConditions bracingConditions)
        {
            var outTuple = vm.GeometryVM.Sections.Select(dto => Tuple.Create(dto, dto.AsEntity().Convert(vm.GeometryVM.TableUnit, UnitSystems.TONCM).AsUnstiffenedZSection().Bind(c => c.AsEgyptInteractionResistance(material, bracingConditions, vm.InputLoadVM.UltimateLoad.ConvertForce(vm.GeneralInfoVM.Unit, UnitSystems.TONCM).Item1, vm.InputLoadVM.UltimateMoment.ConvertMoment(vm.GeneralInfoVM.Unit, UnitSystems.TONCM).Item1).Map(v => v.Convert(UnitSystems.TONCM, vm.GeneralInfoVM.Unit)))))
                                                                    .Aggregate(new List<Tuple<SectionDimensionDto, ResistanceInteractionOutput>>(), (soFar, current) =>
                                                                    {
                                                                        current.Item2.Match(errs => false, val =>
                                                                        {
                                                                            Tuple.Create(current.Item1, val);
                                                                            return true;
                                                                        });
                                                                        return soFar;
                                                                    })
                                                                  .Where(tuple => tuple.Item2.IEValue < 1)
                                                                   .OrderByDescending(tuple => tuple.Item2.IEValue)
                                                                   .FirstOrDefault();
            vm.IsResistanceOutput = true;
            if (outTuple != null)
            {
                (var secDto, var ressistance) = outTuple;
                vm.GeometryVM.SelectedSection = secDto;
                return ressistance.AsDesign(secDto.ID, _momentDict[vm.GeneralInfoVM.Unit], _compDict[vm.GeneralInfoVM.Unit]);
            }
            else
            {
                vm.GeometryVM.SelectedSection = null;
                return null;
            }
        }

        private static DesignOutput EgyptDesignZUnstiffComp(EffectiveWidthViewModel vm, Material material, LengthBracingConditions bracingConditions)
        {
            var outTuple = vm.GeometryVM.Sections.Select(dto => Tuple.Create(dto, dto.AsEntity().Convert(vm.GeometryVM.TableUnit, UnitSystems.TONCM).AsUnstiffenedZSection().Bind(c => c.AsEgyptCompressionResistance(material, bracingConditions).Map(v => v.Convert(UnitSystems.TONCM, vm.GeneralInfoVM.Unit)))))
                                                                     .Aggregate(new List<Tuple<SectionDimensionDto, CompressionResistanceOutput>>(), (soFar, current) =>
                                                                     {
                                                                         current.Item2.Match(errs => false, val =>
                                                                         {
                                                                             Tuple.Create(current.Item1, val);
                                                                             return true;
                                                                         });
                                                                         return soFar;
                                                                     })
                                                                   .Where(tuple => tuple.Item2.DesignResistance > vm.InputLoadVM.UltimateLoad)
                                                                    .OrderBy(tuple => tuple.Item2.DesignResistance)
                                                                     .FirstOrDefault();
            vm.IsResistanceOutput = true;
            if (outTuple != null)
            {
                (var secDto, var ressistance) = outTuple;
                vm.GeometryVM.SelectedSection = secDto;
                return ressistance.AsDesign(vm.InputLoadVM.UltimateLoad, secDto.ID, _compDict[vm.GeneralInfoVM.Unit]);
            }
            else
            {
                vm.GeometryVM.SelectedSection = null;
                return null;
            }
        }

        private static DesignOutput EgyptDesignZLippedComp(EffectiveWidthViewModel vm, Material material, LengthBracingConditions bracingConditions)
        {
            var outTuple = vm.GeometryVM.Sections.Select(dto => Tuple.Create(dto, dto.AsEntity().Convert(vm.GeometryVM.TableUnit, UnitSystems.TONCM).AsLippedZSection().Bind(c => c.AsEgyptCompressionResistance(material, bracingConditions).Map(v => v.Convert(UnitSystems.TONCM, vm.GeneralInfoVM.Unit)))))
                                                                    .Aggregate(new List<Tuple<SectionDimensionDto, CompressionResistanceOutput>>(), (soFar, current) =>
                                                                    {
                                                                        current.Item2.Match(errs => false, val =>
                                                                        {
                                                                            Tuple.Create(current.Item1, val);
                                                                            return true;
                                                                        });
                                                                        return soFar;
                                                                    })
                                                                  .Where(tuple => tuple.Item2.DesignResistance > vm.InputLoadVM.UltimateLoad)
                                                                   .OrderBy(tuple => tuple.Item2.DesignResistance)
                                                                    .FirstOrDefault();
            vm.IsResistanceOutput = true;
            if (outTuple != null)
            {
                (var secDto, var ressistance) = outTuple;
                vm.GeometryVM.SelectedSection = secDto;
                return ressistance.AsDesign(vm.InputLoadVM.UltimateLoad, secDto.ID, _compDict[vm.GeneralInfoVM.Unit]);
            }
            else
            {
                vm.GeometryVM.SelectedSection = null;
                return null;
            }
        }

        private static DesignOutput EgyptDesignZUnstiffMoment(EffectiveWidthViewModel vm, Material material, LengthBracingConditions bracingConditions)
        {
            var outTuple = vm.GeometryVM.Sections.Select(dto => Tuple.Create(dto, dto.AsEntity().Convert(vm.GeometryVM.TableUnit, UnitSystems.TONCM).AsUnstiffenedZSection().Bind(c => c.AsEgyptMomentResistance(material, bracingConditions).Map(v => v.Convert(UnitSystems.TONCM, vm.GeneralInfoVM.Unit)))))
                                                                    .Aggregate(new List<Tuple<SectionDimensionDto, MomentResistanceOutput>>(), (soFar, current) =>
                                                                    {
                                                                        current.Item2.Match(errs => false, val =>
                                                                        {
                                                                            Tuple.Create(current.Item1, val);
                                                                            return true;
                                                                        });
                                                                        return soFar;
                                                                    })
                                                                  .Where(tuple => tuple.Item2.DesignResistance > vm.InputLoadVM.UltimateMoment)
                                                                   .OrderBy(tuple => tuple.Item2.DesignResistance)
                                                                    .FirstOrDefault();
            vm.IsResistanceOutput = true;
            if (outTuple != null)
            {
                (var secDto, var ressistance) = outTuple;
                vm.GeometryVM.SelectedSection = secDto;
                return ressistance.AsDesign(vm.InputLoadVM.UltimateMoment, secDto.ID, _momentDict[vm.GeneralInfoVM.Unit]);
            }
            else
            {
                vm.GeometryVM.SelectedSection = null;
                return null;
            }
        }

        private static DesignOutput EgyptDesignZLippedMoment(EffectiveWidthViewModel vm, Material material, LengthBracingConditions bracingConditions)
        {
            var outTuple = vm.GeometryVM.Sections.Select(dto => Tuple.Create(dto, dto.AsEntity().Convert(vm.GeometryVM.TableUnit, UnitSystems.TONCM).AsLippedZSection().Bind(c => c.AsEgyptMomentResistance(material, bracingConditions).Map(v => v.Convert(UnitSystems.TONCM, vm.GeneralInfoVM.Unit)))))
                                                                    .Aggregate(new List<Tuple<SectionDimensionDto, MomentResistanceOutput>>(), (soFar, current) =>
                                                                    {
                                                                        current.Item2.Match(errs => false, val =>
                                                                        {
                                                                            Tuple.Create(current.Item1, val);
                                                                            return true;
                                                                        });
                                                                        return soFar;
                                                                    })
                                                                  .Where(tuple => tuple.Item2.DesignResistance > vm.InputLoadVM.UltimateMoment)
                                                                   .OrderBy(tuple => tuple.Item2.DesignResistance)
                                                                    .FirstOrDefault();
            vm.IsResistanceOutput = true;
            if (outTuple != null)
            {
                (var secDto, var ressistance) = outTuple;
                vm.GeometryVM.SelectedSection = secDto;
                return ressistance.AsDesign(vm.InputLoadVM.UltimateMoment, secDto.ID, _momentDict[vm.GeneralInfoVM.Unit]);
            }
            else
            {
                vm.GeometryVM.SelectedSection = null;
                return null;
            }
        }

        #endregion

        #endregion


        #region Euro Design

        #region C Sections

        private OutputBase EuroDesignCLippedMomentCompression(EffectiveWidthViewModel vm, Material material, LengthBracingConditions bracingConditions)
        {
            var outTuple = vm.GeometryVM.Sections.Select(dto => Tuple.Create(dto, dto.AsEntity().Convert(vm.GeometryVM.TableUnit, UnitSystems.NMM).AsLippedCSection().Bind(c => c.AsEuroInteractionResistance(material, bracingConditions, vm.InputLoadVM.UltimateLoad.ConvertForce(vm.GeneralInfoVM.Unit, UnitSystems.NMM).Item1, vm.InputLoadVM.UltimateMoment.ConvertMoment(vm.GeneralInfoVM.Unit, UnitSystems.NMM).Item1).Map(v => v.Convert(UnitSystems.NMM, vm.GeneralInfoVM.Unit)))))
                                                                    .Aggregate(new List<Tuple<SectionDimensionDto, ResistanceInteractionOutput>>(), (soFar, current) =>
                                                                    {
                                                                        current.Item2.Match(errs => false, val =>
                                                                        {
                                                                            Tuple.Create(current.Item1, val);
                                                                            return true;
                                                                        });
                                                                        return soFar;
                                                                    })
                                                                  .Where(tuple => tuple.Item2.IEValue < 1)
                                                                   .OrderByDescending(tuple => tuple.Item2.IEValue)
                                                                   .FirstOrDefault();
            vm.IsResistanceOutput = true;
            if (outTuple != null)
            {
                (var secDto, var ressistance) = outTuple;
                vm.GeometryVM.SelectedSection = secDto;
                return ressistance.AsDesign(secDto.ID, _momentDict[vm.GeneralInfoVM.Unit], _compDict[vm.GeneralInfoVM.Unit]);
            }
            else
            {
                vm.GeometryVM.SelectedSection = null;
                return null;
            }
        }

        private OutputBase EuroDesignCUnstiffMomentCompression(EffectiveWidthViewModel vm, Material material, LengthBracingConditions bracingConditions)
        {
            var outTuple = vm.GeometryVM.Sections.Select(dto => Tuple.Create(dto, dto.AsEntity().Convert(vm.GeometryVM.TableUnit, UnitSystems.NMM).AsUnstiffenedCSection().Bind(c => c.AsEuroInteractionResistance(material, bracingConditions, vm.InputLoadVM.UltimateLoad.ConvertForce(vm.GeneralInfoVM.Unit, UnitSystems.NMM).Item1, vm.InputLoadVM.UltimateMoment.ConvertMoment(vm.GeneralInfoVM.Unit, UnitSystems.NMM).Item1).Map(v => v.Convert(UnitSystems.NMM, vm.GeneralInfoVM.Unit)))))
                                                                     .Aggregate(new List<Tuple<SectionDimensionDto, ResistanceInteractionOutput>>(), (soFar, current) =>
                                                                     {
                                                                         current.Item2.Match(errs => false, val =>
                                                                         {
                                                                             Tuple.Create(current.Item1, val);
                                                                             return true;
                                                                         });
                                                                         return soFar;
                                                                     })
                                                                   .Where(tuple => tuple.Item2.IEValue < 1)
                                                                    .OrderByDescending(tuple => tuple.Item2.IEValue)
                                                                    .FirstOrDefault();
            vm.IsResistanceOutput = true;
            if (outTuple != null)
            {
                (var secDto, var ressistance) = outTuple;
                vm.GeometryVM.SelectedSection = secDto;
                return ressistance.AsDesign(secDto.ID, _momentDict[vm.GeneralInfoVM.Unit], _compDict[vm.GeneralInfoVM.Unit]);
            }
            else
            {
                vm.GeometryVM.SelectedSection = null;
                return null;
            }
        }

        private static DesignOutput EuroDesignCUnstiffComp(EffectiveWidthViewModel vm, Material material, LengthBracingConditions bracingConditions)
        {
            var outTuple = vm.GeometryVM.Sections.Select(dto => Tuple.Create(dto, dto.AsEntity().Convert(vm.GeometryVM.TableUnit, UnitSystems.NMM).AsUnstiffenedCSection().Bind(c => c.AsEuroCompressionResistance(material, bracingConditions, vm.InputLoadVM.UltimateLoad.ConvertForce(vm.GeneralInfoVM.Unit, UnitSystems.NMM).Item1).Map(v => v.Convert(UnitSystems.NMM, vm.GeneralInfoVM.Unit)))))
                                                                     .Aggregate(new List<Tuple<SectionDimensionDto, CompressionResistanceOutput>>(), (soFar, current) =>
                                                                     {
                                                                         current.Item2.Match(errs => false, val =>
                                                                         {
                                                                             Tuple.Create(current.Item1, val);
                                                                             return true;
                                                                         });
                                                                         return soFar;
                                                                     })
                                                                   .Where(tuple => tuple.Item2.DesignResistance > vm.InputLoadVM.UltimateLoad)
                                                                    .OrderBy(tuple => tuple.Item2.DesignResistance)
                                                                     .FirstOrDefault();
            vm.IsResistanceOutput = true;
            if (outTuple != null)
            {
                (var secDto, var ressistance) = outTuple;
                vm.GeometryVM.SelectedSection = secDto;
                return ressistance.AsDesign(vm.InputLoadVM.UltimateLoad, secDto.ID, _compDict[vm.GeneralInfoVM.Unit]);
            }
            else
            {
                vm.GeometryVM.SelectedSection = null;
                return null;
            }
        }

        private static DesignOutput EuroDesignCLippedComp(EffectiveWidthViewModel vm, Material material, LengthBracingConditions bracingConditions)
        {
            var outTuple = vm.GeometryVM.Sections.Select(dto => Tuple.Create(dto, dto.AsEntity().Convert(vm.GeometryVM.TableUnit, UnitSystems.NMM).AsLippedCSection().Bind(c => c.AsEuroCompressionResistance(material, bracingConditions, vm.InputLoadVM.UltimateLoad.ConvertForce(vm.GeneralInfoVM.Unit, UnitSystems.NMM).Item1).Map(v => v.Convert(UnitSystems.NMM, vm.GeneralInfoVM.Unit)))))
                                                                    .Aggregate(new List<Tuple<SectionDimensionDto, CompressionResistanceOutput>>(), (soFar, current) =>
                                                                    {
                                                                        current.Item2.Match(errs => false, val =>
                                                                        {
                                                                            Tuple.Create(current.Item1, val);
                                                                            return true;
                                                                        });
                                                                        return soFar;
                                                                    })
                                                                  .Where(tuple => tuple.Item2.DesignResistance > vm.InputLoadVM.UltimateLoad)
                                                                   .OrderBy(tuple => tuple.Item2.DesignResistance)
                                                                    .FirstOrDefault();
            vm.IsResistanceOutput = true;
            if (outTuple != null)
            {
                (var secDto, var ressistance) = outTuple;
                vm.GeometryVM.SelectedSection = secDto;
                return ressistance.AsDesign(vm.InputLoadVM.UltimateLoad, secDto.ID, _compDict[vm.GeneralInfoVM.Unit]);
            }
            else
            {
                vm.GeometryVM.SelectedSection = null;
                return null;
            }
        }

        private static DesignOutput EuroDesignCUnstiffMoment(EffectiveWidthViewModel vm, Material material, LengthBracingConditions bracingConditions)
        {
            var outTuple = vm.GeometryVM.Sections.Select(dto => Tuple.Create(dto, dto.AsEntity().Convert(vm.GeometryVM.TableUnit, UnitSystems.NMM).AsUnstiffenedCSection().Bind(c => c.AsEuroMomentResistance(material, bracingConditions, vm.InputLoadVM.UltimateMoment.ConvertMoment(vm.GeneralInfoVM.Unit, UnitSystems.NMM).Item1).Map(v => v.Convert(UnitSystems.NMM, vm.GeneralInfoVM.Unit)))))
                                                                   .Aggregate(new List<Tuple<SectionDimensionDto, MomentResistanceOutput>>(), (soFar, current) =>
                                                                   {
                                                                       current.Item2.Match(errs => false, val =>
                                                                       {
                                                                           Tuple.Create(current.Item1, val);
                                                                           return true;
                                                                       });
                                                                       return soFar;
                                                                   })
                                                                  .Where(tuple => tuple.Item2.DesignResistance > vm.InputLoadVM.UltimateMoment)
                                                                   .OrderBy(tuple => tuple.Item2.DesignResistance)
                                                                    .FirstOrDefault();
            vm.IsResistanceOutput = true;
            if (outTuple != null)
            {
                (var secDto, var ressistance) = outTuple;
                vm.GeometryVM.SelectedSection = secDto;
                return ressistance.AsDesign(vm.InputLoadVM.UltimateMoment, secDto.ID, _momentDict[vm.GeneralInfoVM.Unit]);
            }
            else
            {
                vm.GeometryVM.SelectedSection = null;
                return null;
            }
        }

        private static DesignOutput EuroDesignCLippedMoment(EffectiveWidthViewModel vm, Material material, LengthBracingConditions bracingConditions)
        {
            var outTuple = vm.GeometryVM.Sections.Select(dto => Tuple.Create(dto, dto.AsEntity().Convert(vm.GeometryVM.TableUnit, UnitSystems.NMM).AsLippedCSection().Bind(c => c.AsEuroMomentResistance(material, bracingConditions, vm.InputLoadVM.UltimateMoment.ConvertMoment(vm.GeneralInfoVM.Unit, UnitSystems.NMM).Item1).Map(v => v.Convert(UnitSystems.NMM, vm.GeneralInfoVM.Unit)))))
                                                                    .Aggregate(new List<Tuple<SectionDimensionDto, MomentResistanceOutput>>(), (soFar, current) =>
                                                                    {
                                                                        current.Item2.Match(errs => false, val =>
                                                                        {
                                                                            Tuple.Create(current.Item1, val);
                                                                            return true;
                                                                        });
                                                                        return soFar;
                                                                    })
                                                                  .Where(tuple => tuple.Item2.DesignResistance > vm.InputLoadVM.UltimateMoment)
                                                                   .OrderBy(tuple => tuple.Item2.DesignResistance)
                                                                    .FirstOrDefault();
            vm.IsResistanceOutput = true;
            if (outTuple != null)
            {
                (var secDto, var ressistance) = outTuple;
                vm.GeometryVM.SelectedSection = secDto;
                return ressistance.AsDesign(vm.InputLoadVM.UltimateMoment, secDto.ID, _momentDict[vm.GeneralInfoVM.Unit]);
            }
            else
            {
                vm.GeometryVM.SelectedSection = null;
                return null;
            }
        }


        #endregion

        #region Z Sections

        private OutputBase EuroDesignZLippedMomentCompression(EffectiveWidthViewModel vm, Material material, LengthBracingConditions bracingConditions)
        {
            var outTuple = vm.GeometryVM.Sections.Select(dto => Tuple.Create(dto, dto.AsEntity().Convert(vm.GeometryVM.TableUnit, UnitSystems.NMM).AsLippedZSection().Bind(c => c.AsEuroInteractionResistance(material, bracingConditions, vm.InputLoadVM.UltimateLoad.ConvertForce(vm.GeneralInfoVM.Unit, UnitSystems.NMM).Item1, vm.InputLoadVM.UltimateMoment.ConvertMoment(vm.GeneralInfoVM.Unit, UnitSystems.NMM).Item1).Map(v => v.Convert(UnitSystems.NMM, vm.GeneralInfoVM.Unit)))))
                                                                    .Aggregate(new List<Tuple<SectionDimensionDto, ResistanceInteractionOutput>>(), (soFar, current) =>
                                                                    {
                                                                        current.Item2.Match(errs => false, val =>
                                                                        {
                                                                            Tuple.Create(current.Item1, val);
                                                                            return true;
                                                                        });
                                                                        return soFar;
                                                                    })
                                                                  .Where(tuple => tuple.Item2.IEValue < 1)
                                                                   .OrderByDescending(tuple => tuple.Item2.IEValue)
                                                                   .FirstOrDefault();
            vm.IsResistanceOutput = true;
            if (outTuple != null)
            {
                (var secDto, var ressistance) = outTuple;
                vm.GeometryVM.SelectedSection = secDto;
                return ressistance.AsDesign(secDto.ID, _momentDict[vm.GeneralInfoVM.Unit], _compDict[vm.GeneralInfoVM.Unit]);
            }
            else
            {
                vm.GeometryVM.SelectedSection = null;
                return null;
            }
        }

        private OutputBase EuroDesignZUnstiffMomentCompression(EffectiveWidthViewModel vm, Material material, LengthBracingConditions bracingConditions)
        {
            var outTuple = vm.GeometryVM.Sections.Select(dto => Tuple.Create(dto, dto.AsEntity().Convert(vm.GeometryVM.TableUnit, UnitSystems.NMM).AsUnstiffenedZSection().Bind(c => c.AsEuroInteractionResistance(material, bracingConditions, vm.InputLoadVM.UltimateLoad.ConvertForce(vm.GeneralInfoVM.Unit, UnitSystems.NMM).Item1, vm.InputLoadVM.UltimateMoment.ConvertMoment(vm.GeneralInfoVM.Unit, UnitSystems.NMM).Item1).Map(v => v.Convert(UnitSystems.NMM, vm.GeneralInfoVM.Unit)))))
                                                                     .Aggregate(new List<Tuple<SectionDimensionDto, ResistanceInteractionOutput>>(), (soFar, current) =>
                                                                     {
                                                                         current.Item2.Match(errs => false, val =>
                                                                         {
                                                                             Tuple.Create(current.Item1, val);
                                                                             return true;
                                                                         });
                                                                         return soFar;
                                                                     })
                                                                   .Where(tuple => tuple.Item2.IEValue < 1)
                                                                    .OrderByDescending(tuple => tuple.Item2.IEValue)
                                                                    .FirstOrDefault();
            vm.IsResistanceOutput = true;
            if (outTuple != null)
            {
                (var secDto, var ressistance) = outTuple;
                vm.GeometryVM.SelectedSection = secDto;
                return ressistance.AsDesign(secDto.ID, _momentDict[vm.GeneralInfoVM.Unit], _compDict[vm.GeneralInfoVM.Unit]);
            }
            else
            {
                vm.GeometryVM.SelectedSection = null;
                return null;
            }
        }

        private static DesignOutput EuroDesignZUnstiffComp(EffectiveWidthViewModel vm, Material material, LengthBracingConditions bracingConditions)
        {
            var outTuple = vm.GeometryVM.Sections.Select(dto => Tuple.Create(dto, dto.AsEntity().Convert(vm.GeometryVM.TableUnit, UnitSystems.NMM).AsUnstiffenedZSection().Bind(c => c.AsEuroCompressionResistance(material, bracingConditions, vm.InputLoadVM.UltimateLoad.ConvertForce(vm.GeneralInfoVM.Unit, UnitSystems.NMM).Item1).Map(v => v.Convert(UnitSystems.NMM, vm.GeneralInfoVM.Unit)))))
                                                                     .Aggregate(new List<Tuple<SectionDimensionDto, CompressionResistanceOutput>>(), (soFar, current) =>
                                                                     {
                                                                         current.Item2.Match(errs => false, val =>
                                                                         {
                                                                             Tuple.Create(current.Item1, val);
                                                                             return true;
                                                                         });
                                                                         return soFar;
                                                                     })
                                                                   .Where(tuple => tuple.Item2.DesignResistance > vm.InputLoadVM.UltimateLoad)
                                                                    .OrderBy(tuple => tuple.Item2.DesignResistance)
                                                                     .FirstOrDefault();
            vm.IsResistanceOutput = true;
            if (outTuple != null)
            {
                (var secDto, var ressistance) = outTuple;
                vm.GeometryVM.SelectedSection = secDto;
                return ressistance.AsDesign(vm.InputLoadVM.UltimateLoad, secDto.ID, _compDict[vm.GeneralInfoVM.Unit]);
            }
            else
            {
                vm.GeometryVM.SelectedSection = null;
                return null;
            }
        }

        private static DesignOutput EuroDesignZLippedComp(EffectiveWidthViewModel vm, Material material, LengthBracingConditions bracingConditions)
        {
            var outTuple = vm.GeometryVM.Sections.Select(dto => Tuple.Create(dto, dto.AsEntity().Convert(vm.GeometryVM.TableUnit, UnitSystems.NMM).AsLippedZSection().Bind(c => c.AsEuroCompressionResistance(material, bracingConditions, vm.InputLoadVM.UltimateLoad.ConvertForce(vm.GeneralInfoVM.Unit, UnitSystems.NMM).Item1).Map(v => v.Convert(UnitSystems.NMM, vm.GeneralInfoVM.Unit)))))
                                                                    .Aggregate(new List<Tuple<SectionDimensionDto, CompressionResistanceOutput>>(), (soFar, current) =>
                                                                    {
                                                                        current.Item2.Match(errs => false, val =>
                                                                        {
                                                                            Tuple.Create(current.Item1, val);
                                                                            return true;
                                                                        });
                                                                        return soFar;
                                                                    })
                                                                  .Where(tuple => tuple.Item2.DesignResistance > vm.InputLoadVM.UltimateLoad)
                                                                   .OrderBy(tuple => tuple.Item2.DesignResistance)
                                                                    .FirstOrDefault();
            vm.IsResistanceOutput = true;
            if (outTuple != null)
            {
                (var secDto, var ressistance) = outTuple;
                vm.GeometryVM.SelectedSection = secDto;
                return ressistance.AsDesign(vm.InputLoadVM.UltimateLoad, secDto.ID, _compDict[vm.GeneralInfoVM.Unit]);
            }
            else
            {
                vm.GeometryVM.SelectedSection = null;
                return null;
            }
        }

        private static DesignOutput EuroDesignZUnstiffMoment(EffectiveWidthViewModel vm, Material material, LengthBracingConditions bracingConditions)
        {
            var outTuple = vm.GeometryVM.Sections.Select(dto => Tuple.Create(dto, dto.AsEntity().Convert(vm.GeometryVM.TableUnit, UnitSystems.NMM).AsUnstiffenedZSection().Bind(c => c.AsEuroMomentResistance(material, bracingConditions, vm.InputLoadVM.UltimateMoment.ConvertMoment(vm.GeneralInfoVM.Unit, UnitSystems.NMM).Item1).Map(v => v.Convert(UnitSystems.NMM, vm.GeneralInfoVM.Unit)))))
                                                                   .Aggregate(new List<Tuple<SectionDimensionDto, MomentResistanceOutput>>(), (soFar, current) =>
                                                                   {
                                                                       current.Item2.Match(errs => false, val =>
                                                                       {
                                                                           Tuple.Create(current.Item1, val);
                                                                           return true;
                                                                       });
                                                                       return soFar;
                                                                   })
                                                                  .Where(tuple => tuple.Item2.DesignResistance > vm.InputLoadVM.UltimateMoment)
                                                                   .OrderBy(tuple => tuple.Item2.DesignResistance)
                                                                    .FirstOrDefault();
            vm.IsResistanceOutput = true;
            if (outTuple != null)
            {
                (var secDto, var ressistance) = outTuple;
                vm.GeometryVM.SelectedSection = secDto;
                return ressistance.AsDesign(vm.InputLoadVM.UltimateMoment, secDto.ID, _momentDict[vm.GeneralInfoVM.Unit]);
            }
            else
            {
                vm.GeometryVM.SelectedSection = null;
                return null;
            }
        }

        private static DesignOutput EuroDesignZLippedMoment(EffectiveWidthViewModel vm, Material material, LengthBracingConditions bracingConditions)
        {
            var outTuple = vm.GeometryVM.Sections.Select(dto => Tuple.Create(dto, dto.AsEntity().Convert(vm.GeometryVM.TableUnit, UnitSystems.NMM).AsLippedZSection().Bind(c => c.AsEuroMomentResistance(material, bracingConditions, vm.InputLoadVM.UltimateMoment.ConvertMoment(vm.GeneralInfoVM.Unit, UnitSystems.NMM).Item1).Map(v => v.Convert(UnitSystems.NMM, vm.GeneralInfoVM.Unit)))))
                                                                    .Aggregate(new List<Tuple<SectionDimensionDto, MomentResistanceOutput>>(), (soFar, current) =>
                                                                    {
                                                                        current.Item2.Match(errs => false, val =>
                                                                        {
                                                                            Tuple.Create(current.Item1, val);
                                                                            return true;
                                                                        });
                                                                        return soFar;
                                                                    })
                                                                  .Where(tuple => tuple.Item2.DesignResistance > vm.InputLoadVM.UltimateMoment)
                                                                   .OrderBy(tuple => tuple.Item2.DesignResistance)
                                                                    .FirstOrDefault();
            vm.IsResistanceOutput = true;
            if (outTuple != null)
            {
                (var secDto, var ressistance) = outTuple;
                vm.GeometryVM.SelectedSection = secDto;
                return ressistance.AsDesign(vm.InputLoadVM.UltimateMoment, secDto.ID, _momentDict[vm.GeneralInfoVM.Unit]);
            }
            else
            {
                vm.GeometryVM.SelectedSection = null;
                return null;
            }
        }


        #endregion

        #endregion

        #region AISI Design

        #region C Sections

        private OutputBase AISIDesignCLippedMomentCompression(EffectiveWidthViewModel vm, Material material, LengthBracingConditions bracingConditions)
        {
            var outTuple = vm.GeometryVM.Sections.Select(dto => Tuple.Create(dto, dto.AsEntity().Convert(vm.GeometryVM.TableUnit, UnitSystems.KIPINCH).AsLippedCSection().Bind(c => c.AsAISIInteractionResistance(material, bracingConditions, vm.InputLoadVM.UltimateLoad.ConvertForce(vm.GeneralInfoVM.Unit, UnitSystems.KIPINCH).Item1, vm.InputLoadVM.UltimateMoment.ConvertMoment(vm.GeneralInfoVM.Unit, UnitSystems.KIPINCH).Item1).Map(v => v.Convert(UnitSystems.KIPINCH, vm.GeneralInfoVM.Unit)))))
                                                                     .Aggregate(new List<Tuple<SectionDimensionDto, ResistanceInteractionOutput>>(), (soFar, current) =>
                                                                     {
                                                                         current.Item2.Match(errs => false, val =>
                                                                         {
                                                                             Tuple.Create(current.Item1, val);
                                                                             return true;
                                                                         });
                                                                         return soFar;
                                                                     })
                                                                  .Where(tuple => tuple.Item2.IEValue < 1)
                                                                   .OrderByDescending(tuple => tuple.Item2.IEValue)
                                                                   .FirstOrDefault();
            vm.IsResistanceOutput = true;
            if (outTuple != null)
            {
                (var secDto, var ressistance) = outTuple;
                vm.GeometryVM.SelectedSection = secDto;
                return ressistance.AsDesign(secDto.ID, _momentDict[vm.GeneralInfoVM.Unit], _compDict[vm.GeneralInfoVM.Unit]);
            }
            else
            {
                vm.GeometryVM.SelectedSection = null;
                return null;
            }
        }

        private OutputBase AISIDesignCUnstiffMomentCompression(EffectiveWidthViewModel vm, Material material, LengthBracingConditions bracingConditions)
        {
            var outTuple = vm.GeometryVM.Sections.Select(dto => Tuple.Create(dto, dto.AsEntity().Convert(vm.GeometryVM.TableUnit, UnitSystems.KIPINCH).AsUnstiffenedCSection().Bind(c => c.AsAISIInteractionResistance(material, bracingConditions, vm.InputLoadVM.UltimateLoad.ConvertForce(vm.GeneralInfoVM.Unit, UnitSystems.KIPINCH).Item1, vm.InputLoadVM.UltimateMoment.ConvertMoment(vm.GeneralInfoVM.Unit, UnitSystems.KIPINCH).Item1).Map(v => v.Convert(UnitSystems.KIPINCH, vm.GeneralInfoVM.Unit)))))
                                                                    .Aggregate(new List<Tuple<SectionDimensionDto, ResistanceInteractionOutput>>(), (soFar, current) =>
                                                                    {
                                                                        current.Item2.Match(errs => false, val =>
                                                                        {
                                                                            Tuple.Create(current.Item1, val);
                                                                            return true;
                                                                        });
                                                                        return soFar;
                                                                    })
                                                                 .Where(tuple => tuple.Item2.IEValue < 1)
                                                                  .OrderByDescending(tuple => tuple.Item2.IEValue)
                                                                  .FirstOrDefault();
            vm.IsResistanceOutput = true;
            if (outTuple != null)
            {
                (var secDto, var ressistance) = outTuple;
                vm.GeometryVM.SelectedSection = secDto;
                return ressistance.AsDesign(secDto.ID, _momentDict[vm.GeneralInfoVM.Unit], _compDict[vm.GeneralInfoVM.Unit]);
            }
            else
            {
                vm.GeometryVM.SelectedSection = null;
                return null;
            }
        }

        private static DesignOutput AISIDesignCUnstiffComp(EffectiveWidthViewModel vm, Material material, LengthBracingConditions bracingConditions)
        {
            var outTuple = vm.GeometryVM.Sections.Select(dto => Tuple.Create(dto, dto.AsEntity().Convert(vm.GeometryVM.TableUnit, UnitSystems.KIPINCH).AsUnstiffenedCSection().Bind(c => c.AsAISICompressionResistance(material, bracingConditions).Map(v => v.Convert(UnitSystems.KIPINCH, vm.GeneralInfoVM.Unit)))))
                                                                     .Aggregate(new List<Tuple<SectionDimensionDto, CompressionResistanceOutput>>(), (soFar, current) =>
                                                                     {
                                                                         current.Item2.Match(errs => false, val =>
                                                                         {
                                                                             Tuple.Create(current.Item1, val);
                                                                             return true;
                                                                         });
                                                                         return soFar;
                                                                     })
                                                                   .Where(tuple => tuple.Item2.DesignResistance > vm.InputLoadVM.UltimateLoad)
                                                                    .OrderBy(tuple => tuple.Item2.DesignResistance)
                                                                     .FirstOrDefault();
            vm.IsResistanceOutput = true;
            if (outTuple != null)
            {
                (var secDto, var ressistance) = outTuple;
                vm.GeometryVM.SelectedSection = secDto;
                return ressistance.AsDesign(vm.InputLoadVM.UltimateLoad, secDto.ID, _compDict[vm.GeneralInfoVM.Unit]);
            }
            else
            {
                vm.GeometryVM.SelectedSection = null;
                return null;
            }
        }

        private static DesignOutput AISIDesignCLippedComp(EffectiveWidthViewModel vm, Material material, LengthBracingConditions bracingConditions)
        {
            var outTuple = vm.GeometryVM.Sections.Select(dto => Tuple.Create(dto, dto.AsEntity().Convert(vm.GeometryVM.TableUnit, UnitSystems.KIPINCH).AsLippedCSection().Bind(c => c.AsAISICompressionResistance(material, bracingConditions).Map(v => v.Convert(UnitSystems.KIPINCH, vm.GeneralInfoVM.Unit)))))
                                                                    .Aggregate(new List<Tuple<SectionDimensionDto, CompressionResistanceOutput>>(), (soFar, current) =>
                                                                    {
                                                                        current.Item2.Match(errs => false, val =>
                                                                        {
                                                                            Tuple.Create(current.Item1, val);
                                                                            return true;
                                                                        });
                                                                        return soFar;
                                                                    })
                                                                  .Where(tuple => tuple.Item2.DesignResistance > vm.InputLoadVM.UltimateLoad)
                                                                   .OrderBy(tuple => tuple.Item2.DesignResistance)
                                                                    .FirstOrDefault();
            vm.IsResistanceOutput = true;
            if (outTuple != null)
            {
                (var secDto, var ressistance) = outTuple;
                vm.GeometryVM.SelectedSection = secDto;
                return ressistance.AsDesign(vm.InputLoadVM.UltimateLoad, secDto.ID, _compDict[vm.GeneralInfoVM.Unit]);
            }
            else
            {
                vm.GeometryVM.SelectedSection = null;
                return null;
            }
        }

        private static DesignOutput AISIDesignCUnstiffMoment(EffectiveWidthViewModel vm, Material material, LengthBracingConditions bracingConditions)
        {
            var outTuple = vm.GeometryVM.Sections.Select(dto => Tuple.Create(dto, dto.AsEntity().Convert(vm.GeometryVM.TableUnit, UnitSystems.KIPINCH).AsUnstiffenedCSection().Bind(c => c.AsAISIMomentResistance(material, bracingConditions).Map(v => v.Convert(UnitSystems.KIPINCH, vm.GeneralInfoVM.Unit)))))
                                                                    .Aggregate(new List<Tuple<SectionDimensionDto, MomentResistanceOutput>>(), (soFar, current) =>
                                                                    {
                                                                        current.Item2.Match(errs => false, val =>
                                                                        {
                                                                            Tuple.Create(current.Item1, val);
                                                                            return true;
                                                                        });
                                                                        return soFar;
                                                                    })
                                                                  .Where(tuple => tuple.Item2.DesignResistance > vm.InputLoadVM.UltimateMoment)
                                                                   .OrderBy(tuple => tuple.Item2.DesignResistance)
                                                                    .FirstOrDefault();
            vm.IsResistanceOutput = true;
            if (outTuple != null)
            {
                (var secDto, var ressistance) = outTuple;
                vm.GeometryVM.SelectedSection = secDto;
                return ressistance.AsDesign(vm.InputLoadVM.UltimateMoment, secDto.ID, _momentDict[vm.GeneralInfoVM.Unit]);
            }
            else
            {
                vm.GeometryVM.SelectedSection = null;
                return null;
            }
        }

        private static DesignOutput AISIDesignCLippedMoment(EffectiveWidthViewModel vm, Material material, LengthBracingConditions bracingConditions)
        {
            var outTuple = vm.GeometryVM.Sections.Select(dto => Tuple.Create(dto, dto.AsEntity().Convert(vm.GeometryVM.TableUnit, UnitSystems.KIPINCH).AsLippedCSection().Bind(c => c.AsAISIMomentResistance(material, bracingConditions).Map(v => v.Convert(UnitSystems.KIPINCH, vm.GeneralInfoVM.Unit)))))
                                                                     .Aggregate(new List<Tuple<SectionDimensionDto, MomentResistanceOutput>>(), (soFar, current) =>
                                                                     {
                                                                         current.Item2.Match(errs => false, val =>
                                                                         {
                                                                             Tuple.Create(current.Item1, val);
                                                                             return true;
                                                                         });
                                                                         return soFar;
                                                                     })
                                                                  .Where(tuple => tuple.Item2.DesignResistance > vm.InputLoadVM.UltimateMoment)
                                                                   .OrderBy(tuple => tuple.Item2.DesignResistance)
                                                                    .FirstOrDefault();
            vm.IsResistanceOutput = true;
            if (outTuple != null)
            {
                (var secDto, var ressistance) = outTuple;
                vm.GeometryVM.SelectedSection = secDto;
                return ressistance.AsDesign(vm.InputLoadVM.UltimateMoment, secDto.ID, _momentDict[vm.GeneralInfoVM.Unit]);
            }
            else
            {
                vm.GeometryVM.SelectedSection = null;
                return null;
            }
        }


        #endregion

        #region Z Sections

        private OutputBase AISIDesignZLippedMomentCompression(EffectiveWidthViewModel vm, Material material, LengthBracingConditions bracingConditions)
        {
            var outTuple = vm.GeometryVM.Sections.Select(dto => Tuple.Create(dto, dto.AsEntity().Convert(vm.GeometryVM.TableUnit, UnitSystems.KIPINCH).AsLippedZSection().Bind(c => c.AsAISIInteractionResistance(material, bracingConditions, vm.InputLoadVM.UltimateLoad.ConvertForce(vm.GeneralInfoVM.Unit, UnitSystems.KIPINCH).Item1, vm.InputLoadVM.UltimateMoment.ConvertMoment(vm.GeneralInfoVM.Unit, UnitSystems.KIPINCH).Item1).Map(v => v.Convert(UnitSystems.KIPINCH, vm.GeneralInfoVM.Unit)))))
                                                                     .Aggregate(new List<Tuple<SectionDimensionDto, ResistanceInteractionOutput>>(), (soFar, current) =>
                                                                     {
                                                                         current.Item2.Match(errs => false, val =>
                                                                         {
                                                                             Tuple.Create(current.Item1, val);
                                                                             return true;
                                                                         });
                                                                         return soFar;
                                                                     })
                                                                  .Where(tuple => tuple.Item2.IEValue < 1)
                                                                   .OrderByDescending(tuple => tuple.Item2.IEValue)
                                                                   .FirstOrDefault();
            vm.IsResistanceOutput = true;
            if (outTuple != null)
            {
                (var secDto, var ressistance) = outTuple;
                vm.GeometryVM.SelectedSection = secDto;
                return ressistance.AsDesign(secDto.ID, _momentDict[vm.GeneralInfoVM.Unit], _compDict[vm.GeneralInfoVM.Unit]);
            }
            else
            {
                vm.GeometryVM.SelectedSection = null;
                return null;
            }
        }

        private OutputBase AISIDesignZUnstiffMomentCompression(EffectiveWidthViewModel vm, Material material, LengthBracingConditions bracingConditions)
        {
            var outTuple = vm.GeometryVM.Sections.Select(dto => Tuple.Create(dto, dto.AsEntity().Convert(vm.GeometryVM.TableUnit, UnitSystems.KIPINCH).AsUnstiffenedZSection().Bind(c => c.AsAISIInteractionResistance(material, bracingConditions, vm.InputLoadVM.UltimateLoad.ConvertForce(vm.GeneralInfoVM.Unit, UnitSystems.KIPINCH).Item1, vm.InputLoadVM.UltimateMoment.ConvertMoment(vm.GeneralInfoVM.Unit, UnitSystems.KIPINCH).Item1).Map(v => v.Convert(UnitSystems.KIPINCH, vm.GeneralInfoVM.Unit)))))
                                                                    .Aggregate(new List<Tuple<SectionDimensionDto, ResistanceInteractionOutput>>(), (soFar, current) =>
                                                                    {
                                                                        current.Item2.Match(errs => false, val =>
                                                                        {
                                                                            Tuple.Create(current.Item1, val);
                                                                            return true;
                                                                        });
                                                                        return soFar;
                                                                    })
                                                                 .Where(tuple => tuple.Item2.IEValue < 1)
                                                                  .OrderByDescending(tuple => tuple.Item2.IEValue)
                                                                  .FirstOrDefault();
            vm.IsResistanceOutput = true;
            if (outTuple != null)
            {
                (var secDto, var ressistance) = outTuple;
                vm.GeometryVM.SelectedSection = secDto;
                return ressistance.AsDesign(secDto.ID, _momentDict[vm.GeneralInfoVM.Unit], _compDict[vm.GeneralInfoVM.Unit]);
            }
            else
            {
                vm.GeometryVM.SelectedSection = null;
                return null;
            }
        }

        private static DesignOutput AISIDesignZUnstiffComp(EffectiveWidthViewModel vm, Material material, LengthBracingConditions bracingConditions)
        {
            var outTuple = vm.GeometryVM.Sections.Select(dto => Tuple.Create(dto, dto.AsEntity().Convert(vm.GeometryVM.TableUnit, UnitSystems.KIPINCH).AsUnstiffenedZSection().Bind(c => c.AsAISICompressionResistance(material, bracingConditions).Map(v => v.Convert(UnitSystems.KIPINCH, vm.GeneralInfoVM.Unit)))))
                                                                     .Aggregate(new List<Tuple<SectionDimensionDto, CompressionResistanceOutput>>(), (soFar, current) =>
                                                                     {
                                                                         current.Item2.Match(errs => false, val =>
                                                                         {
                                                                             Tuple.Create(current.Item1, val);
                                                                             return true;
                                                                         });
                                                                         return soFar;
                                                                     })
                                                                   .Where(tuple => tuple.Item2.DesignResistance > vm.InputLoadVM.UltimateLoad)
                                                                    .OrderBy(tuple => tuple.Item2.DesignResistance)
                                                                     .FirstOrDefault();
            vm.IsResistanceOutput = true;
            if (outTuple != null)
            {
                (var secDto, var ressistance) = outTuple;
                vm.GeometryVM.SelectedSection = secDto;
                return ressistance.AsDesign(vm.InputLoadVM.UltimateLoad, secDto.ID, _compDict[vm.GeneralInfoVM.Unit]);
            }
            else
            {
                vm.GeometryVM.SelectedSection = null;
                return null;
            }
        }

        private static DesignOutput AISIDesignZLippedComp(EffectiveWidthViewModel vm, Material material, LengthBracingConditions bracingConditions)
        {
            var outTuple = vm.GeometryVM.Sections.Select(dto => Tuple.Create(dto, dto.AsEntity().Convert(vm.GeometryVM.TableUnit, UnitSystems.KIPINCH).AsLippedZSection().Bind(c => c.AsAISICompressionResistance(material, bracingConditions).Map(v => v.Convert(UnitSystems.KIPINCH, vm.GeneralInfoVM.Unit)))))
                                                                    .Aggregate(new List<Tuple<SectionDimensionDto, CompressionResistanceOutput>>(), (soFar, current) =>
                                                                    {
                                                                        current.Item2.Match(errs => false, val =>
                                                                        {
                                                                            Tuple.Create(current.Item1, val);
                                                                            return true;
                                                                        });
                                                                        return soFar;
                                                                    })
                                                                  .Where(tuple => tuple.Item2.DesignResistance > vm.InputLoadVM.UltimateLoad)
                                                                   .OrderBy(tuple => tuple.Item2.DesignResistance)
                                                                    .FirstOrDefault();
            vm.IsResistanceOutput = true;
            if (outTuple != null)
            {
                (var secDto, var ressistance) = outTuple;
                vm.GeometryVM.SelectedSection = secDto;
                return ressistance.AsDesign(vm.InputLoadVM.UltimateLoad, secDto.ID, _compDict[vm.GeneralInfoVM.Unit]);
            }
            else
            {
                vm.GeometryVM.SelectedSection = null;
                return null;
            }
        }

        private static DesignOutput AISIDesignZUnstiffMoment(EffectiveWidthViewModel vm, Material material, LengthBracingConditions bracingConditions)
        {
            var outTuple = vm.GeometryVM.Sections.Select(dto => Tuple.Create(dto, dto.AsEntity().Convert(vm.GeometryVM.TableUnit, UnitSystems.KIPINCH).AsUnstiffenedZSection().Bind(c => c.AsAISIMomentResistance(material, bracingConditions).Map(v => v.Convert(UnitSystems.KIPINCH, vm.GeneralInfoVM.Unit)))))
                                                                    .Aggregate(new List<Tuple<SectionDimensionDto, MomentResistanceOutput>>(), (soFar, current) =>
                                                                    {
                                                                        current.Item2.Match(errs => false, val =>
                                                                        {
                                                                            Tuple.Create(current.Item1, val);
                                                                            return true;
                                                                        });
                                                                        return soFar;
                                                                    })
                                                                  .Where(tuple => tuple.Item2.DesignResistance > vm.InputLoadVM.UltimateMoment)
                                                                   .OrderBy(tuple => tuple.Item2.DesignResistance)
                                                                    .FirstOrDefault();
            vm.IsResistanceOutput = true;
            if (outTuple != null)
            {
                (var secDto, var ressistance) = outTuple;
                vm.GeometryVM.SelectedSection = secDto;
                return ressistance.AsDesign(vm.InputLoadVM.UltimateMoment, secDto.ID, _momentDict[vm.GeneralInfoVM.Unit]);
            }
            else
            {
                vm.GeometryVM.SelectedSection = null;
                return null;
            }
        }

        private static DesignOutput AISIDesignZLippedMoment(EffectiveWidthViewModel vm, Material material, LengthBracingConditions bracingConditions)
        {
            var outTuple = vm.GeometryVM.Sections.Select(dto => Tuple.Create(dto, dto.AsEntity().Convert(vm.GeometryVM.TableUnit, UnitSystems.KIPINCH).AsLippedZSection().Bind(c => c.AsAISIMomentResistance(material, bracingConditions).Map(v => v.Convert(UnitSystems.KIPINCH, vm.GeneralInfoVM.Unit)))))
                                                                     .Aggregate(new List<Tuple<SectionDimensionDto, MomentResistanceOutput>>(), (soFar, current) =>
                                                                     {
                                                                         current.Item2.Match(errs => false, val =>
                                                                         {
                                                                             Tuple.Create(current.Item1, val);
                                                                             return true;
                                                                         });
                                                                         return soFar;
                                                                     })
                                                                  .Where(tuple => tuple.Item2.DesignResistance > vm.InputLoadVM.UltimateMoment)
                                                                   .OrderBy(tuple => tuple.Item2.DesignResistance)
                                                                    .FirstOrDefault();
            vm.IsResistanceOutput = true;
            if (outTuple != null)
            {
                (var secDto, var ressistance) = outTuple;
                vm.GeometryVM.SelectedSection = secDto;
                return ressistance.AsDesign(vm.InputLoadVM.UltimateMoment, secDto.ID, _momentDict[vm.GeneralInfoVM.Unit]);
            }
            else
            {
                vm.GeometryVM.SelectedSection = null;
                return null;
            }
        }

        #endregion

        #endregion


        private static Validation<Unit> Check(EffectiveWidthViewModel vm)
        {
            vm.IsResistanceOutput = false;
            var material = (new Material(vm.MaterialVM.Fy, vm.MaterialVM.E, 0.3)).Convert(vm.GeneralInfoVM.Unit, _unitSystemDict[vm.GeneralInfoVM.DesignCode]);
            var bracingConditions = vm.BracingConditionsVM.AsEntity().Convert(vm.GeneralInfoVM.Unit, _unitSystemDict[vm.GeneralInfoVM.DesignCode]);
            var sa = vm.GeneralInfoVM.StrainingAction;
            var section = vm.GeneralInfoVM.SteelSection;
            var resistOutput = vm.ResistDict[vm.GeneralInfoVM.DesignCode][KeyValuePair.Create(sa, section)](vm, material, bracingConditions);
            return resistOutput.Map(val =>
            {
                var result = val.Item1();
                vm.ResistanceOutput = result;
                return Unit();
            });
        }

        private static Validation<Unit> Resistance(EffectiveWidthViewModel vm)
        {
            vm.IsResistanceOutput = false;
            var material = (new Material(vm.MaterialVM.Fy, vm.MaterialVM.E, vm.MaterialVM.V)).Convert(vm.GeneralInfoVM.Unit, _unitSystemDict[vm.GeneralInfoVM.DesignCode]);
            var bracingConditions = vm.BracingConditionsVM.AsEntity().Convert(vm.GeneralInfoVM.Unit, _unitSystemDict[vm.GeneralInfoVM.DesignCode]);
            var sa = vm.GeneralInfoVM.StrainingAction;
            var section = vm.GeneralInfoVM.SteelSection;
            var result = vm.ResistDict[vm.GeneralInfoVM.DesignCode][KeyValuePair.Create(sa, section)](vm, material, bracingConditions);
            return result.Map(val =>
             {
                 vm.ResistanceOutput = val.Item2;
                 return Unit();
             });
        }


        #region Egypt Resistance

        #region C Sections

        private Validation<Tuple<Func<OutputBase>, OutputBase>> EgyptResistanceCLippedMomentCompression(EffectiveWidthViewModel vm, Material material, LengthBracingConditions bracingConditions)
        {
            var result = vm.GeometryVM.AsEntity()
                                         .Convert(vm.GeneralInfoVM.Unit, UnitSystems.TONCM)
                                         .AsLippedCSection()
                                         .Bind(c =>
                                         c.AsEgyptInteractionResistance(material, bracingConditions, vm.InputLoadVM.UltimateLoad.ConvertForce(vm.GeneralInfoVM.Unit, UnitSystems.TONCM).Item1, vm.InputLoadVM.UltimateMoment.ConvertMoment(vm.GeneralInfoVM.Unit, UnitSystems.TONCM).Item1).Map(v =>
                                         v.Convert(UnitSystems.TONCM, vm.GeneralInfoVM.Unit)))
                                         .Map(val =>
                                         {
                                             Func<OutputBase> fun = () => val.AsCheck(_momentDict[vm.GeneralInfoVM.Unit], _compDict[vm.GeneralInfoVM.Unit]);

                                             return Tuple.Create(fun, val as OutputBase);
                                         });
            return result;
        }

        private Validation<Tuple<Func<OutputBase>, OutputBase>> EgyptResistanceCUnstiffMomentCompression(EffectiveWidthViewModel vm, Material material, LengthBracingConditions bracingConditions)
        {
            var result = vm.GeometryVM.AsEntity()
                                       .Convert(vm.GeneralInfoVM.Unit, UnitSystems.TONCM)
                                       .AsUnstiffenedCSection()
                                       .Bind(c =>
                                        c.AsEgyptInteractionResistance(material, bracingConditions, vm.InputLoadVM.UltimateLoad.ConvertForce(vm.GeneralInfoVM.Unit, UnitSystems.TONCM).Item1, vm.InputLoadVM.UltimateMoment.ConvertMoment(vm.GeneralInfoVM.Unit, UnitSystems.TONCM).Item1)
                                       .Map(v => v.Convert(UnitSystems.TONCM, vm.GeneralInfoVM.Unit)))
                                       .Map(val =>
                                       {
                                           Func<OutputBase> fun = () => val.AsCheck(_momentDict[vm.GeneralInfoVM.Unit], _compDict[vm.GeneralInfoVM.Unit]);

                                           return Tuple.Create(fun, val as OutputBase);
                                       });

            return result;
        }

        private static Validation<Tuple<Func<OutputBase>, OutputBase>> EgyptResistanceCUnstiffComp(EffectiveWidthViewModel vm, Material material, LengthBracingConditions bracingConditions)
        {
            var result = vm.GeometryVM.AsEntity()
                                       .Convert(vm.GeneralInfoVM.Unit, UnitSystems.TONCM)
                                       .AsUnstiffenedCSection()
                                       .Bind(c =>
                                       c.AsEgyptCompressionResistance(material, bracingConditions)
                                       .Map(v => v.Convert(UnitSystems.TONCM, vm.GeneralInfoVM.Unit)))
                                       .Map(val =>
                                       {
                                           Func<OutputBase> fun = () => val.AsCheck(vm.InputLoadVM.UltimateLoad, _compDict[vm.GeneralInfoVM.Unit]);

                                           return Tuple.Create(fun, val as OutputBase);
                                       });
            return result;

        }
        private static Validation<Tuple<Func<OutputBase>, OutputBase>> EgyptResistanceCUnstiffMoment(EffectiveWidthViewModel vm, Material material, LengthBracingConditions bracingConditions)
        {
            var result = vm.GeometryVM.AsEntity()
                                       .Convert(vm.GeneralInfoVM.Unit, UnitSystems.TONCM)
                                       .AsUnstiffenedCSection()
                                       .Bind(c =>
                                       c.AsEgyptMomentResistance(material, bracingConditions)
                                       .Map(v => v.Convert(UnitSystems.TONCM, vm.GeneralInfoVM.Unit)))
                                       .Map(val =>
                                       {
                                           Func<OutputBase> fun = () => val.AsCheck(vm.InputLoadVM.UltimateMoment, _momentDict[vm.GeneralInfoVM.Unit]);

                                           return Tuple.Create(fun, val as OutputBase);
                                       });
            return result;
        }
        private static Validation<Tuple<Func<OutputBase>, OutputBase>> EgyptResistanceCLippedComp(EffectiveWidthViewModel vm, Material material, LengthBracingConditions bracingConditions)
        {
            var result = vm.GeometryVM.AsEntity()
                                      .Convert(vm.GeneralInfoVM.Unit, UnitSystems.TONCM)
                                      .AsLippedCSection()
                                      .Bind(c =>
                                      c.AsEgyptCompressionResistance(material, bracingConditions)
                                      .Map(v => v.Convert(UnitSystems.TONCM, vm.GeneralInfoVM.Unit)))
                                      .Map(val =>
                                      {
                                          Func<OutputBase> fun = () => val.AsCheck(vm.InputLoadVM.UltimateLoad, _compDict[vm.GeneralInfoVM.Unit]);

                                          return Tuple.Create(fun, val as OutputBase);
                                      });
            return result;

        }
        private static Validation<Tuple<Func<OutputBase>, OutputBase>> EgyptResistanceCLippedMoment(EffectiveWidthViewModel vm, Material material, LengthBracingConditions bracingConditions)
        {
            var result = vm.GeometryVM.AsEntity()
                                      .Convert(vm.GeneralInfoVM.Unit, UnitSystems.TONCM)
                                      .AsLippedCSection()
                                      .Bind(c =>
                                       c.AsEgyptMomentResistance(material, bracingConditions).Map(v => v.Convert(UnitSystems.TONCM, vm.GeneralInfoVM.Unit)))
                                      .Map(val =>
                                      {
                                          Func<OutputBase> fun = () => val.AsCheck(vm.InputLoadVM.UltimateMoment, _momentDict[vm.GeneralInfoVM.Unit]);
                                          return Tuple.Create(fun, val as OutputBase);
                                      });
            return result;
        }


        #endregion

        #region Z Sections

        private Validation<Tuple<Func<OutputBase>, OutputBase>> EgyptResistanceZLippedMomentCompression(EffectiveWidthViewModel vm, Material material, LengthBracingConditions bracingConditions)
        {
            var result = vm.GeometryVM.AsEntity()
                                         .Convert(vm.GeneralInfoVM.Unit, UnitSystems.TONCM)
                                         .AsLippedZSection()
                                         .Bind(c =>
                                         c.AsEgyptInteractionResistance(material, bracingConditions, vm.InputLoadVM.UltimateLoad.ConvertForce(vm.GeneralInfoVM.Unit, UnitSystems.TONCM).Item1, vm.InputLoadVM.UltimateMoment.ConvertMoment(vm.GeneralInfoVM.Unit, UnitSystems.TONCM).Item1).Map(v =>
                                         v.Convert(UnitSystems.TONCM, vm.GeneralInfoVM.Unit)))
                                         .Map(val =>
                                         {
                                             Func<OutputBase> fun = () => val.AsCheck(_momentDict[vm.GeneralInfoVM.Unit], _compDict[vm.GeneralInfoVM.Unit]);

                                             return Tuple.Create(fun, val as OutputBase);
                                         });
            return result;
        }

        private Validation<Tuple<Func<OutputBase>, OutputBase>> EgyptResistanceZUnstiffMomentCompression(EffectiveWidthViewModel vm, Material material, LengthBracingConditions bracingConditions)
        {
            var result = vm.GeometryVM.AsEntity()
                                       .Convert(vm.GeneralInfoVM.Unit, UnitSystems.TONCM)
                                       .AsUnstiffenedZSection()
                                       .Bind(c =>
                                        c.AsEgyptInteractionResistance(material, bracingConditions, vm.InputLoadVM.UltimateLoad.ConvertForce(vm.GeneralInfoVM.Unit, UnitSystems.TONCM).Item1, vm.InputLoadVM.UltimateMoment.ConvertMoment(vm.GeneralInfoVM.Unit, UnitSystems.TONCM).Item1)
                                       .Map(v => v.Convert(UnitSystems.TONCM, vm.GeneralInfoVM.Unit)))
                                       .Map(val =>
                                       {
                                           Func<OutputBase> fun = () => val.AsCheck(_momentDict[vm.GeneralInfoVM.Unit], _compDict[vm.GeneralInfoVM.Unit]);

                                           return Tuple.Create(fun, val as OutputBase);
                                       });

            return result;
        }

        private static Validation<Tuple<Func<OutputBase>, OutputBase>> EgyptResistanceZUnstiffComp(EffectiveWidthViewModel vm, Material material, LengthBracingConditions bracingConditions)
        {
            var result = vm.GeometryVM.AsEntity()
                                       .Convert(vm.GeneralInfoVM.Unit, UnitSystems.TONCM)
                                       .AsUnstiffenedZSection()
                                       .Bind(c =>
                                       c.AsEgyptCompressionResistance(material, bracingConditions)
                                       .Map(v => v.Convert(UnitSystems.TONCM, vm.GeneralInfoVM.Unit)))
                                       .Map(val =>
                                       {
                                           Func<OutputBase> fun = () => val.AsCheck(vm.InputLoadVM.UltimateLoad, _compDict[vm.GeneralInfoVM.Unit]);

                                           return Tuple.Create(fun, val as OutputBase);
                                       });
            return result;

        }
        private static Validation<Tuple<Func<OutputBase>, OutputBase>> EgyptResistanceZUnstiffMoment(EffectiveWidthViewModel vm, Material material, LengthBracingConditions bracingConditions)
        {
            var result = vm.GeometryVM.AsEntity()
                                       .Convert(vm.GeneralInfoVM.Unit, UnitSystems.TONCM)
                                       .AsUnstiffenedZSection()
                                       .Bind(c =>
                                       c.AsEgyptMomentResistance(material, bracingConditions)
                                       .Map(v => v.Convert(UnitSystems.TONCM, vm.GeneralInfoVM.Unit)))
                                       .Map(val =>
                                       {
                                           Func<OutputBase> fun = () => val.AsCheck(vm.InputLoadVM.UltimateMoment, _momentDict[vm.GeneralInfoVM.Unit]);

                                           return Tuple.Create(fun, val as OutputBase);
                                       });
            return result;
        }
        private static Validation<Tuple<Func<OutputBase>, OutputBase>> EgyptResistanceZLippedComp(EffectiveWidthViewModel vm, Material material, LengthBracingConditions bracingConditions)
        {
            var result = vm.GeometryVM.AsEntity()
                                      .Convert(vm.GeneralInfoVM.Unit, UnitSystems.TONCM)
                                      .AsLippedZSection()
                                      .Bind(c =>
                                      c.AsEgyptCompressionResistance(material, bracingConditions)
                                      .Map(v => v.Convert(UnitSystems.TONCM, vm.GeneralInfoVM.Unit)))
                                      .Map(val =>
                                      {
                                          Func<OutputBase> fun = () => val.AsCheck(vm.InputLoadVM.UltimateLoad, _compDict[vm.GeneralInfoVM.Unit]);

                                          return Tuple.Create(fun, val as OutputBase);
                                      });
            return result;

        }
        private static Validation<Tuple<Func<OutputBase>, OutputBase>> EgyptResistanceZLippedMoment(EffectiveWidthViewModel vm, Material material, LengthBracingConditions bracingConditions)
        {
            var result = vm.GeometryVM.AsEntity()
                                      .Convert(vm.GeneralInfoVM.Unit, UnitSystems.TONCM)
                                      .AsLippedZSection()
                                      .Bind(c =>
                                       c.AsEgyptMomentResistance(material, bracingConditions).Map(v => v.Convert(UnitSystems.TONCM, vm.GeneralInfoVM.Unit)))
                                      .Map(val =>
                                      {
                                          Func<OutputBase> fun = () => val.AsCheck(vm.InputLoadVM.UltimateMoment, _momentDict[vm.GeneralInfoVM.Unit]);
                                          return Tuple.Create(fun, val as OutputBase);
                                      });
            return result;
        }

        #endregion

        #endregion

        #region Euro Resistance

        #region C Sections

        private Validation<Tuple<Func<OutputBase>, OutputBase>> EuroResistanceCLippedMomentCompression(EffectiveWidthViewModel vm, Material material, LengthBracingConditions bracingConditions)
        {
            var result = vm.GeometryVM.AsEntity()
                                        .Convert(vm.GeneralInfoVM.Unit, UnitSystems.NMM)
                                        .AsLippedCSection()
                                        .Bind(c =>
                                         c.AsEuroInteractionResistance(material, bracingConditions, vm.InputLoadVM.UltimateLoad, vm.InputLoadVM.UltimateMoment)
                                        .Map(v => v.Convert(UnitSystems.NMM, vm.GeneralInfoVM.Unit)))
                                        .Map(val =>
                                        {
                                            Func<OutputBase> fun = () => val.AsCheck(_momentDict[vm.GeneralInfoVM.Unit], _compDict[vm.GeneralInfoVM.Unit]);

                                            return Tuple.Create(fun, val as OutputBase);
                                        });
            return result;
        }

        private Validation<Tuple<Func<OutputBase>, OutputBase>> EuroResistanceCUnstiffMomentCompression(EffectiveWidthViewModel vm, Material material, LengthBracingConditions bracingConditions)
        {
            var result = vm.GeometryVM.AsEntity()
                                       .Convert(vm.GeneralInfoVM.Unit, UnitSystems.NMM)
                                       .AsUnstiffenedCSection()
                                       .Bind(c =>
                                        c.AsEuroInteractionResistance(material, bracingConditions, vm.InputLoadVM.UltimateLoad.ConvertForce(vm.GeneralInfoVM.Unit, UnitSystems.NMM).Item1, vm.InputLoadVM.UltimateMoment.ConvertMoment(vm.GeneralInfoVM.Unit, UnitSystems.NMM).Item1)
                                       .Map(v => v.Convert(UnitSystems.NMM, vm.GeneralInfoVM.Unit)))
                                       .Map(val =>
                                       {
                                           Func<OutputBase> fun = () => val.AsCheck(_momentDict[vm.GeneralInfoVM.Unit], _compDict[vm.GeneralInfoVM.Unit]);

                                           return Tuple.Create(fun, val as OutputBase);
                                       });

            return result;
        }

        private static Validation<Tuple<Func<OutputBase>, OutputBase>> EuroResistanceCUnstiffComp(EffectiveWidthViewModel vm, Material material, LengthBracingConditions bracingConditions)
        {
            var result = vm.GeometryVM.AsEntity()
                                       .Convert(vm.GeneralInfoVM.Unit, UnitSystems.NMM)
                                       .AsUnstiffenedCSection()
                                       .Bind(c =>
                                        c.AsEuroCompressionResistance(material, bracingConditions, vm.InputLoadVM.UltimateLoad.ConvertForce(vm.GeneralInfoVM.Unit, UnitSystems.NMM).Item1)
                                       .Map(v => v.Convert(UnitSystems.NMM, vm.GeneralInfoVM.Unit)))
                                       .Map(val =>
                                       {
                                           Func<OutputBase> fun = () => val.AsCheck(vm.InputLoadVM.UltimateLoad, _compDict[vm.GeneralInfoVM.Unit]);

                                           return Tuple.Create(fun, val as OutputBase);
                                       });
            return result;
        }

        private static Validation<Tuple<Func<OutputBase>, OutputBase>> EuroResistanceCUnstiffMoment(EffectiveWidthViewModel vm, Material material, LengthBracingConditions bracingConditions)
        {
            var result = vm.GeometryVM.AsEntity()
                                       .Convert(vm.GeneralInfoVM.Unit, UnitSystems.NMM)
                                       .AsUnstiffenedCSection()
                                       .Bind(c =>
                                        c.AsEuroMomentResistance(material, bracingConditions, vm.InputLoadVM.UltimateMoment.ConvertMoment(vm.GeneralInfoVM.Unit, UnitSystems.NMM).Item1)
                                       .Map(v => v.Convert(UnitSystems.NMM, vm.GeneralInfoVM.Unit)))
                                       .Map(val =>
                                       {
                                           Func<OutputBase> fun = () => val.AsCheck(vm.InputLoadVM.UltimateMoment, _momentDict[vm.GeneralInfoVM.Unit]);

                                           return Tuple.Create(fun, val as OutputBase);
                                       });
            return result;
        }

        private static Validation<Tuple<Func<OutputBase>, OutputBase>> EuroResistanceCLippedComp(EffectiveWidthViewModel vm, Material material, LengthBracingConditions bracingConditions)
        {
            var result = vm.GeometryVM.AsEntity()
                                      .Convert(vm.GeneralInfoVM.Unit, UnitSystems.NMM)
                                      .AsLippedCSection()
                                      .Bind(c =>
                                       c.AsEuroCompressionResistance(material, bracingConditions, vm.InputLoadVM.UltimateLoad.ConvertForce(vm.GeneralInfoVM.Unit, UnitSystems.NMM).Item1)
                                      .Map(v => v.Convert(UnitSystems.NMM, vm.GeneralInfoVM.Unit)))
                                      .Map(val =>
                                      {
                                          Func<OutputBase> fun = () => val.AsCheck(vm.InputLoadVM.UltimateLoad, _compDict[vm.GeneralInfoVM.Unit]);

                                          return Tuple.Create(fun, val as OutputBase);
                                      });
            return result;
        }

        private static Validation<Tuple<Func<OutputBase>, OutputBase>> EuroResistanceCLippedMoment(EffectiveWidthViewModel vm, Material material, LengthBracingConditions bracingConditions)
        {
            var result = vm.GeometryVM.AsEntity()
                                      .Convert(vm.GeneralInfoVM.Unit, UnitSystems.NMM)
                                      .AsLippedCSection()
                                      .Bind(c =>
                                       c.AsEuroMomentResistance(material, bracingConditions, vm.InputLoadVM.UltimateMoment.ConvertMoment(vm.GeneralInfoVM.Unit, UnitSystems.NMM).Item1)
                                      .Map(v => v.Convert(UnitSystems.NMM, vm.GeneralInfoVM.Unit)))
                                      .Map(val =>
                                      {
                                          Func<OutputBase> fun = () => val.AsCheck(vm.InputLoadVM.UltimateMoment, _momentDict[vm.GeneralInfoVM.Unit]);
                                          return Tuple.Create(fun, val as OutputBase);
                                      });
            return result;
        }


        #endregion

        #region Z Sections

        private Validation<Tuple<Func<OutputBase>, OutputBase>> EuroResistanceZLippedMomentCompression(EffectiveWidthViewModel vm, Material material, LengthBracingConditions bracingConditions)
        {
            var result = vm.GeometryVM.AsEntity()
                                        .Convert(vm.GeneralInfoVM.Unit, UnitSystems.NMM)
                                        .AsLippedZSection()
                                        .Bind(c =>
                                         c.AsEuroInteractionResistance(material, bracingConditions, vm.InputLoadVM.UltimateLoad, vm.InputLoadVM.UltimateMoment)
                                        .Map(v => v.Convert(UnitSystems.NMM, vm.GeneralInfoVM.Unit)))
                                        .Map(val =>
                                        {
                                            Func<OutputBase> fun = () => val.AsCheck(_momentDict[vm.GeneralInfoVM.Unit], _compDict[vm.GeneralInfoVM.Unit]);

                                            return Tuple.Create(fun, val as OutputBase);
                                        });
            return result;
        }

        private Validation<Tuple<Func<OutputBase>, OutputBase>> EuroResistanceZUnstiffMomentCompression(EffectiveWidthViewModel vm, Material material, LengthBracingConditions bracingConditions)
        {
            var result = vm.GeometryVM.AsEntity()
                                       .Convert(vm.GeneralInfoVM.Unit, UnitSystems.NMM)
                                       .AsUnstiffenedZSection()
                                       .Bind(c =>
                                        c.AsEuroInteractionResistance(material, bracingConditions, vm.InputLoadVM.UltimateLoad.ConvertForce(vm.GeneralInfoVM.Unit, UnitSystems.NMM).Item1, vm.InputLoadVM.UltimateMoment.ConvertMoment(vm.GeneralInfoVM.Unit, UnitSystems.NMM).Item1)
                                       .Map(v => v.Convert(UnitSystems.NMM, vm.GeneralInfoVM.Unit)))
                                       .Map(val =>
                                       {
                                           Func<OutputBase> fun = () => val.AsCheck(_momentDict[vm.GeneralInfoVM.Unit], _compDict[vm.GeneralInfoVM.Unit]);

                                           return Tuple.Create(fun, val as OutputBase);
                                       });

            return result;
        }

        private static Validation<Tuple<Func<OutputBase>, OutputBase>> EuroResistanceZUnstiffComp(EffectiveWidthViewModel vm, Material material, LengthBracingConditions bracingConditions)
        {
            var result = vm.GeometryVM.AsEntity()
                                       .Convert(vm.GeneralInfoVM.Unit, UnitSystems.NMM)
                                       .AsUnstiffenedZSection()
                                       .Bind(c =>
                                        c.AsEuroCompressionResistance(material, bracingConditions, vm.InputLoadVM.UltimateLoad.ConvertForce(vm.GeneralInfoVM.Unit, UnitSystems.NMM).Item1)
                                       .Map(v => v.Convert(UnitSystems.NMM, vm.GeneralInfoVM.Unit)))
                                       .Map(val =>
                                       {
                                           Func<OutputBase> fun = () => val.AsCheck(vm.InputLoadVM.UltimateLoad, _compDict[vm.GeneralInfoVM.Unit]);

                                           return Tuple.Create(fun, val as OutputBase);
                                       });
            return result;
        }

        private static Validation<Tuple<Func<OutputBase>, OutputBase>> EuroResistanceZUnstiffMoment(EffectiveWidthViewModel vm, Material material, LengthBracingConditions bracingConditions)
        {
            var result = vm.GeometryVM.AsEntity()
                                       .Convert(vm.GeneralInfoVM.Unit, UnitSystems.NMM)
                                       .AsUnstiffenedZSection()
                                       .Bind(c =>
                                        c.AsEuroMomentResistance(material, bracingConditions, vm.InputLoadVM.UltimateMoment.ConvertMoment(vm.GeneralInfoVM.Unit, UnitSystems.NMM).Item1)
                                       .Map(v => v.Convert(UnitSystems.NMM, vm.GeneralInfoVM.Unit)))
                                       .Map(val =>
                                       {
                                           Func<OutputBase> fun = () => val.AsCheck(vm.InputLoadVM.UltimateMoment, _momentDict[vm.GeneralInfoVM.Unit]);

                                           return Tuple.Create(fun, val as OutputBase);
                                       });
            return result;
        }

        private static Validation<Tuple<Func<OutputBase>, OutputBase>> EuroResistanceZLippedComp(EffectiveWidthViewModel vm, Material material, LengthBracingConditions bracingConditions)
        {
            var result = vm.GeometryVM.AsEntity()
                                      .Convert(vm.GeneralInfoVM.Unit, UnitSystems.NMM)
                                      .AsLippedZSection()
                                      .Bind(c =>
                                       c.AsEuroCompressionResistance(material, bracingConditions, vm.InputLoadVM.UltimateLoad.ConvertForce(vm.GeneralInfoVM.Unit, UnitSystems.NMM).Item1)
                                      .Map(v => v.Convert(UnitSystems.NMM, vm.GeneralInfoVM.Unit)))
                                      .Map(val =>
                                      {
                                          Func<OutputBase> fun = () => val.AsCheck(vm.InputLoadVM.UltimateLoad, _compDict[vm.GeneralInfoVM.Unit]);

                                          return Tuple.Create(fun, val as OutputBase);
                                      });
            return result;
        }

        private static Validation<Tuple<Func<OutputBase>, OutputBase>> EuroResistanceZLippedMoment(EffectiveWidthViewModel vm, Material material, LengthBracingConditions bracingConditions)
        {
            var result = vm.GeometryVM.AsEntity()
                                      .Convert(vm.GeneralInfoVM.Unit, UnitSystems.NMM)
                                      .AsLippedZSection()
                                      .Bind(c =>
                                       c.AsEuroMomentResistance(material, bracingConditions, vm.InputLoadVM.UltimateMoment.ConvertMoment(vm.GeneralInfoVM.Unit, UnitSystems.NMM).Item1)
                                      .Map(v => v.Convert(UnitSystems.NMM, vm.GeneralInfoVM.Unit)))
                                      .Map(val =>
                                      {
                                          Func<OutputBase> fun = () => val.AsCheck(vm.InputLoadVM.UltimateMoment, _momentDict[vm.GeneralInfoVM.Unit]);
                                          return Tuple.Create(fun, val as OutputBase);
                                      });
            return result;
        }


        #endregion

        #endregion

        #region AISI Resistance

        #region C Sections

        private Validation<Tuple<Func<OutputBase>, OutputBase>> AISIResistanceCLippedMomentCompression(EffectiveWidthViewModel vm, Material material, LengthBracingConditions bracingConditions)
        {
            var result = vm.GeometryVM.AsEntity()
                                        .Convert(vm.GeneralInfoVM.Unit, UnitSystems.KIPINCH)
                                        .AsLippedCSection()
                                        .Bind(c =>
                                         c.AsAISIInteractionResistance(material, bracingConditions, vm.InputLoadVM.UltimateLoad.ConvertForce(vm.GeneralInfoVM.Unit, UnitSystems.KIPINCH).Item1, vm.InputLoadVM.UltimateMoment.ConvertMoment(vm.GeneralInfoVM.Unit, UnitSystems.KIPINCH).Item1)
                                        .Map(v => v.Convert(UnitSystems.KIPINCH, vm.GeneralInfoVM.Unit)))
                                        .Map(val =>
                                        {
                                            Func<OutputBase> fun = () => val.AsCheck(_momentDict[vm.GeneralInfoVM.Unit], _compDict[vm.GeneralInfoVM.Unit]);

                                            return Tuple.Create(fun, val as OutputBase);
                                        });
            return result;
        }

        private Validation<Tuple<Func<OutputBase>, OutputBase>> AISIResistanceCUnstiffMomentCompression(EffectiveWidthViewModel vm, Material material, LengthBracingConditions bracingConditions)
        {
            var result = vm.GeometryVM.AsEntity()
                                       .Convert(vm.GeneralInfoVM.Unit, UnitSystems.KIPINCH)
                                       .AsUnstiffenedCSection()
                                       .Bind(c =>
                                        c.AsAISIInteractionResistance(material, bracingConditions, vm.InputLoadVM.UltimateLoad.ConvertForce(vm.GeneralInfoVM.Unit, UnitSystems.KIPINCH).Item1, vm.InputLoadVM.UltimateMoment.ConvertMoment(vm.GeneralInfoVM.Unit, UnitSystems.KIPINCH).Item1)
                                       .Map(v => v.Convert(UnitSystems.KIPINCH, vm.GeneralInfoVM.Unit)))
                                       .Map(val =>
                                       {
                                           Func<OutputBase> fun = () => val.AsCheck(_momentDict[vm.GeneralInfoVM.Unit], _compDict[vm.GeneralInfoVM.Unit]);

                                           return Tuple.Create(fun, val as OutputBase);
                                       });
            return result;
        }

        private static Validation<Tuple<Func<OutputBase>, OutputBase>> AISIResistanceCUnstiffComp(EffectiveWidthViewModel vm, Material material, LengthBracingConditions bracingConditions)
        {
            var result = vm.GeometryVM.AsEntity()
                                       .Convert(vm.GeneralInfoVM.Unit, UnitSystems.KIPINCH)
                                       .AsUnstiffenedCSection()
                                       .Bind(c =>
                                        c.AsAISICompressionResistance(material, bracingConditions)
                                       .Map(v => v.Convert(UnitSystems.KIPINCH, vm.GeneralInfoVM.Unit)))
                                       .Map(val =>
                                       {
                                           Func<OutputBase> fun = () => val.AsCheck(vm.InputLoadVM.UltimateLoad, _compDict[vm.GeneralInfoVM.Unit]);

                                           return Tuple.Create(fun, val as OutputBase);
                                       });
            return result;
        }

        private static Validation<Tuple<Func<OutputBase>, OutputBase>> AISIResistanceCUnstiffMoment(EffectiveWidthViewModel vm, Material material, LengthBracingConditions bracingConditions)
        {
            var result = vm.GeometryVM.AsEntity()
                                       .Convert(vm.GeneralInfoVM.Unit, UnitSystems.KIPINCH)
                                       .AsUnstiffenedCSection()
                                       .Bind(c =>
                                        c.AsAISIMomentResistance(material, bracingConditions)
                                       .Map(v => v.Convert(UnitSystems.KIPINCH, vm.GeneralInfoVM.Unit)))
                                       .Map(val =>
                                       {
                                           Func<OutputBase> fun = () => val.AsCheck(vm.InputLoadVM.UltimateMoment, _momentDict[vm.GeneralInfoVM.Unit]);

                                           return Tuple.Create(fun, val as OutputBase);
                                       });
            return result;
        }

        private static Validation<Tuple<Func<OutputBase>, OutputBase>> AISIResistanceCLippedComp(EffectiveWidthViewModel vm, Material material, LengthBracingConditions bracingConditions)
        {
            var result = vm.GeometryVM.AsEntity()
                                      .Convert(vm.GeneralInfoVM.Unit, UnitSystems.KIPINCH)
                                      .AsLippedCSection()
                                      .Bind(c =>
                                       c.AsAISICompressionResistance(material, bracingConditions)
                                      .Map(v => v.Convert(UnitSystems.KIPINCH, vm.GeneralInfoVM.Unit)))
                                      .Map(val =>
                                      {
                                          Func<OutputBase> fun = () => val.AsCheck(vm.InputLoadVM.UltimateLoad, _compDict[vm.GeneralInfoVM.Unit]);

                                          return Tuple.Create(fun, val as OutputBase);
                                      });
            return result;
        }

        private static Validation<Tuple<Func<OutputBase>, OutputBase>> AISIResistanceCLippedMoment(EffectiveWidthViewModel vm, Material material, LengthBracingConditions bracingConditions)
        {
            var result = vm.GeometryVM.AsEntity()
                                      .Convert(vm.GeneralInfoVM.Unit, UnitSystems.KIPINCH)
                                      .AsLippedCSection()
                                      .Bind(c =>
                                       c.AsAISIMomentResistance(material, bracingConditions)
                                       .Map(v => v.Convert(UnitSystems.KIPINCH, vm.GeneralInfoVM.Unit)))
                                      .Map(val =>
                                      {
                                          Func<OutputBase> fun = () => val.AsCheck(vm.InputLoadVM.UltimateMoment, _momentDict[vm.GeneralInfoVM.Unit]);
                                          return Tuple.Create(fun, val as OutputBase);
                                      });
            return result;
        }


        #endregion

        #region Z Sections

        private Validation<Tuple<Func<OutputBase>, OutputBase>> AISIResistanceZLippedMomentCompression(EffectiveWidthViewModel vm, Material material, LengthBracingConditions bracingConditions)
        {
            var result = vm.GeometryVM.AsEntity()
                                        .Convert(vm.GeneralInfoVM.Unit, UnitSystems.KIPINCH)
                                        .AsLippedZSection()
                                        .Bind(c =>
                                         c.AsAISIInteractionResistance(material, bracingConditions, vm.InputLoadVM.UltimateLoad.ConvertForce(vm.GeneralInfoVM.Unit, UnitSystems.KIPINCH).Item1, vm.InputLoadVM.UltimateMoment.ConvertMoment(vm.GeneralInfoVM.Unit, UnitSystems.KIPINCH).Item1)
                                        .Map(v => v.Convert(UnitSystems.KIPINCH, vm.GeneralInfoVM.Unit)))
                                        .Map(val =>
                                        {
                                            Func<OutputBase> fun = () => val.AsCheck(_momentDict[vm.GeneralInfoVM.Unit], _compDict[vm.GeneralInfoVM.Unit]);

                                            return Tuple.Create(fun, val as OutputBase);
                                        });
            return result;
        }

        private Validation<Tuple<Func<OutputBase>, OutputBase>> AISIResistanceZUnstiffMomentCompression(EffectiveWidthViewModel vm, Material material, LengthBracingConditions bracingConditions)
        {
            var result = vm.GeometryVM.AsEntity()
                                       .Convert(vm.GeneralInfoVM.Unit, UnitSystems.KIPINCH)
                                       .AsUnstiffenedZSection()
                                       .Bind(c =>
                                        c.AsAISIInteractionResistance(material, bracingConditions, vm.InputLoadVM.UltimateLoad.ConvertForce(vm.GeneralInfoVM.Unit, UnitSystems.KIPINCH).Item1, vm.InputLoadVM.UltimateMoment.ConvertMoment(vm.GeneralInfoVM.Unit, UnitSystems.KIPINCH).Item1)
                                       .Map(v => v.Convert(UnitSystems.KIPINCH, vm.GeneralInfoVM.Unit)))
                                       .Map(val =>
                                       {
                                           Func<OutputBase> fun = () => val.AsCheck(_momentDict[vm.GeneralInfoVM.Unit], _compDict[vm.GeneralInfoVM.Unit]);

                                           return Tuple.Create(fun, val as OutputBase);
                                       });
            return result;
        }

        private static Validation<Tuple<Func<OutputBase>, OutputBase>> AISIResistanceZUnstiffComp(EffectiveWidthViewModel vm, Material material, LengthBracingConditions bracingConditions)
        {
            var result = vm.GeometryVM.AsEntity()
                                       .Convert(vm.GeneralInfoVM.Unit, UnitSystems.KIPINCH)
                                       .AsUnstiffenedZSection()
                                       .Bind(c =>
                                        c.AsAISICompressionResistance(material, bracingConditions)
                                       .Map(v => v.Convert(UnitSystems.KIPINCH, vm.GeneralInfoVM.Unit)))
                                       .Map(val =>
                                       {
                                           Func<OutputBase> fun = () => val.AsCheck(vm.InputLoadVM.UltimateLoad, _compDict[vm.GeneralInfoVM.Unit]);

                                           return Tuple.Create(fun, val as OutputBase);
                                       });
            return result;
        }

        private static Validation<Tuple<Func<OutputBase>, OutputBase>> AISIResistanceZUnstiffMoment(EffectiveWidthViewModel vm, Material material, LengthBracingConditions bracingConditions)
        {
            var result = vm.GeometryVM.AsEntity()
                                       .Convert(vm.GeneralInfoVM.Unit, UnitSystems.KIPINCH)
                                       .AsUnstiffenedZSection()
                                       .Bind(c =>
                                        c.AsAISIMomentResistance(material, bracingConditions)
                                       .Map(v => v.Convert(UnitSystems.KIPINCH, vm.GeneralInfoVM.Unit)))
                                       .Map(val =>
                                       {
                                           Func<OutputBase> fun = () => val.AsCheck(vm.InputLoadVM.UltimateMoment, _momentDict[vm.GeneralInfoVM.Unit]);

                                           return Tuple.Create(fun, val as OutputBase);
                                       });
            return result;
        }

        private static Validation<Tuple<Func<OutputBase>, OutputBase>> AISIResistanceZLippedComp(EffectiveWidthViewModel vm, Material material, LengthBracingConditions bracingConditions)
        {
            var result = vm.GeometryVM.AsEntity()
                                      .Convert(vm.GeneralInfoVM.Unit, UnitSystems.KIPINCH)
                                      .AsLippedZSection()
                                      .Bind(c =>
                                       c.AsAISICompressionResistance(material, bracingConditions)
                                      .Map(v => v.Convert(UnitSystems.KIPINCH, vm.GeneralInfoVM.Unit)))
                                      .Map(val =>
                                      {
                                          Func<OutputBase> fun = () => val.AsCheck(vm.InputLoadVM.UltimateLoad, _compDict[vm.GeneralInfoVM.Unit]);

                                          return Tuple.Create(fun, val as OutputBase);
                                      });
            return result;
        }

        private static Validation<Tuple<Func<OutputBase>, OutputBase>> AISIResistanceZLippedMoment(EffectiveWidthViewModel vm, Material material, LengthBracingConditions bracingConditions)
        {
            var result = vm.GeometryVM.AsEntity()
                                      .Convert(vm.GeneralInfoVM.Unit, UnitSystems.KIPINCH)
                                      .AsLippedZSection()
                                      .Bind(c =>
                                       c.AsAISIMomentResistance(material, bracingConditions)
                                       .Map(v => v.Convert(UnitSystems.KIPINCH, vm.GeneralInfoVM.Unit)))
                                      .Map(val =>
                                      {
                                          Func<OutputBase> fun = () => val.AsCheck(vm.InputLoadVM.UltimateMoment, _momentDict[vm.GeneralInfoVM.Unit]);
                                          return Tuple.Create(fun, val as OutputBase);
                                      });
            return result;
        }

        #endregion

        #endregion


        #endregion

    }
}
