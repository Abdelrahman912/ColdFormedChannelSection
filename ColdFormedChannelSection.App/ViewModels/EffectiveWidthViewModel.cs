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
using CSharp.Functional.Constructs;
using static ColdFormedChannelSection.Core.Errors.Errors;

namespace ColdFormedChannelSection.App.ViewModels
{
    public class EffectiveWidthViewModel : ResistanceBaseViewModel
    {

        #region Private Fields


        private readonly Dictionary<Module, Action<EffectiveWidthViewModel>> _moduleDict;

        private List<Func<List<Error>>> _validateFuncs;

        private static readonly Dictionary<DesignCode, UnitSystems> _unitSystemDict = new Dictionary<DesignCode, UnitSystems>()
        {
            [DesignCode.EURO] = UnitSystems.NMM,
            [DesignCode.EGYPTIAN]=UnitSystems.TONCM,
            [DesignCode.AISI] = UnitSystems.KIPINCH
        };

        #endregion

        #region Properties

        public override ICommand ResultsCommand { get; }


        public Dictionary<DesignCode, Dictionary<KeyValuePair<StrainingActions, SteelSection>, Func<EffectiveWidthViewModel, Material, LengthBracingConditions, OutputBase>>> DesignDict { get; }

        public Dictionary<KeyValuePair<StrainingActions, SteelSection>, Func<EffectiveWidthViewModel, Material, LengthBracingConditions, OutputBase>> DesignEgyptDict { get; }
        public Dictionary<KeyValuePair<StrainingActions, SteelSection>, Func<EffectiveWidthViewModel, Material, LengthBracingConditions, OutputBase>> DesignEuroDict { get; }
        public Dictionary<KeyValuePair<StrainingActions, SteelSection>, Func<EffectiveWidthViewModel, Material, LengthBracingConditions, OutputBase>> DesignAISIDict { get; }


        public Dictionary<DesignCode, Dictionary<KeyValuePair<StrainingActions, SteelSection>, Func<EffectiveWidthViewModel, Material, LengthBracingConditions, Tuple<Func<OutputBase>, OutputBase>>>> ResistDict { get; }

        public Dictionary<KeyValuePair<StrainingActions, SteelSection>, Func<EffectiveWidthViewModel, Material, LengthBracingConditions, Tuple<Func<OutputBase>, OutputBase>>> ResistEgyptDict { get; }
        public Dictionary<KeyValuePair<StrainingActions, SteelSection>, Func<EffectiveWidthViewModel, Material, LengthBracingConditions, Tuple<Func<OutputBase>, OutputBase>>> ResistEuroDict { get; }
        public Dictionary<KeyValuePair<StrainingActions, SteelSection>, Func<EffectiveWidthViewModel, Material, LengthBracingConditions, Tuple<Func<OutputBase>, OutputBase>>> ResistAISIDict { get; }


        #endregion

        #region Constructors

        public EffectiveWidthViewModel(GeneralInfoViewModel generalInfoVM, BracingConditionsViewModel bracingConditionsVM, GeometryViewModel geometryVM, MaterialViewModel materialVM, InputLoadViewModel inputLoadVM,Func<List<Error>,Unit> showErrorsService , Action<IReport> reportService)
          : base(generalInfoVM, bracingConditionsVM, geometryVM, materialVM, inputLoadVM, showErrorsService,reportService)
        {
            ResultsCommand = new RelayCommand(OnReults, CanResults);
            _moduleDict = new Dictionary<Module, Action<EffectiveWidthViewModel>>()
            {
                [Module.RESISTANCE] = Resistance,
                [Module.DESIGN] = Design,
                [Module.CHECK] = Check
            };
            ResistEgyptDict = new Dictionary<KeyValuePair<StrainingActions, SteelSection>, Func<EffectiveWidthViewModel, Material, LengthBracingConditions, Tuple<Func<OutputBase>, OutputBase>>>()
            {
                [KeyValuePair.Create(StrainingActions.COMPRESSION, SteelSection.C_UNSTIFFENED)] = EgyptResistanceCUnstiffComp,
                [KeyValuePair.Create(StrainingActions.MOMENT, SteelSection.C_UNSTIFFENED)] = EgyptResistanceCUnstiffMoment,
                [KeyValuePair.Create(StrainingActions.MOMENT_COMPRESSION, SteelSection.C_UNSTIFFENED)] = EgyptResistanceCUnstiffMomentCompression,
                [KeyValuePair.Create(StrainingActions.COMPRESSION, SteelSection.C_LIPPED)] = EgyptResistanceCLippedComp,
                [KeyValuePair.Create(StrainingActions.MOMENT, SteelSection.C_LIPPED)] = EgyptResistanceCLippedMoment,
                [KeyValuePair.Create(StrainingActions.MOMENT_COMPRESSION, SteelSection.C_LIPPED)] = EgyptResistanceCLippedMomentCompression,
            };

            ResistEuroDict = new Dictionary<KeyValuePair<StrainingActions, SteelSection>, Func<EffectiveWidthViewModel, Material, LengthBracingConditions, Tuple<Func<OutputBase>, OutputBase>>>()
            {
                [KeyValuePair.Create(StrainingActions.COMPRESSION, SteelSection.C_UNSTIFFENED)] = EuroResistanceCUnstiffComp,
                [KeyValuePair.Create(StrainingActions.MOMENT, SteelSection.C_UNSTIFFENED)] = EuroResistanceCUnstiffMoment,
                [KeyValuePair.Create(StrainingActions.MOMENT_COMPRESSION, SteelSection.C_UNSTIFFENED)] = EuroResistanceCUnstiffMomentCompression,
                [KeyValuePair.Create(StrainingActions.COMPRESSION, SteelSection.C_LIPPED)] = EuroResistanceCLippedComp,
                [KeyValuePair.Create(StrainingActions.MOMENT, SteelSection.C_LIPPED)] = EuroResistanceCLippedMoment,
                [KeyValuePair.Create(StrainingActions.MOMENT_COMPRESSION, SteelSection.C_LIPPED)] = EuroResistanceCLippedMomentCompression,
            };

            ResistAISIDict = new Dictionary<KeyValuePair<StrainingActions, SteelSection>, Func<EffectiveWidthViewModel, Material, LengthBracingConditions, Tuple<Func<OutputBase>, OutputBase>>>()
            {
                [KeyValuePair.Create(StrainingActions.COMPRESSION, SteelSection.C_UNSTIFFENED)] = AISIResistanceCUnstiffComp,
                [KeyValuePair.Create(StrainingActions.MOMENT, SteelSection.C_UNSTIFFENED)] = AISIResistanceCUnstiffMoment,
                [KeyValuePair.Create(StrainingActions.MOMENT_COMPRESSION, SteelSection.C_UNSTIFFENED)] = AISIResistanceCUnstiffMomentCompression,
                [KeyValuePair.Create(StrainingActions.COMPRESSION, SteelSection.C_LIPPED)] = AISIResistanceCLippedComp,
                [KeyValuePair.Create(StrainingActions.MOMENT, SteelSection.C_LIPPED)] = AISIResistanceCLippedMoment,
                [KeyValuePair.Create(StrainingActions.MOMENT_COMPRESSION, SteelSection.C_LIPPED)] = AISIResistanceCLippedMomentCompression
            };
            ResistDict = new Dictionary<DesignCode, Dictionary<KeyValuePair<StrainingActions, SteelSection>, Func<EffectiveWidthViewModel, Material, LengthBracingConditions, Tuple<Func<OutputBase>, OutputBase>>>>()
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
            };
            DesignEuroDict = new Dictionary<KeyValuePair<StrainingActions, SteelSection>, Func<EffectiveWidthViewModel, Material, LengthBracingConditions, OutputBase>>()
            {
                [KeyValuePair.Create(StrainingActions.COMPRESSION, SteelSection.C_UNSTIFFENED)] = EuroDesignCUnstiffComp,
                [KeyValuePair.Create(StrainingActions.MOMENT, SteelSection.C_UNSTIFFENED)] = EuroDesignCUnstiffMoment,
                [KeyValuePair.Create(StrainingActions.MOMENT_COMPRESSION, SteelSection.C_UNSTIFFENED)] = EuroDesignCUnstiffMomentCompression,
                [KeyValuePair.Create(StrainingActions.COMPRESSION, SteelSection.C_LIPPED)] = EuroDesignCLippedComp,
                [KeyValuePair.Create(StrainingActions.MOMENT, SteelSection.C_LIPPED)] = EuroDesignCLippedMoment,
                [KeyValuePair.Create(StrainingActions.MOMENT_COMPRESSION, SteelSection.C_LIPPED)] = EuroDesignCLippedMomentCompression,
            };
            DesignAISIDict = new Dictionary<KeyValuePair<StrainingActions, SteelSection>, Func<EffectiveWidthViewModel, Material, LengthBracingConditions, OutputBase>>()
            {
                [KeyValuePair.Create(StrainingActions.COMPRESSION, SteelSection.C_UNSTIFFENED)] = AISIDesignCUnstiffComp,
                [KeyValuePair.Create(StrainingActions.MOMENT, SteelSection.C_UNSTIFFENED)] = AISIDesignCUnstiffMoment,
                [KeyValuePair.Create(StrainingActions.MOMENT_COMPRESSION, SteelSection.C_UNSTIFFENED)] = AISIDesignCUnstiffMomentCompression,
                [KeyValuePair.Create(StrainingActions.COMPRESSION, SteelSection.C_LIPPED)] = AISIDesignCLippedComp,
                [KeyValuePair.Create(StrainingActions.MOMENT, SteelSection.C_LIPPED)] = AISIDesignCLippedMoment,
                [KeyValuePair.Create(StrainingActions.MOMENT_COMPRESSION, SteelSection.C_LIPPED)] = AISIDesignCLippedMomentCompression,
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
                _moduleDict[GeneralInfoVM.RunningModule](this);
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
            };

            _validateFuncs.SelectMany(f => f())
                          .AsUnitValidation()
                          .Bind(_ => designValid())
                          .Match(errs => { IsDisplayReport = false; ShowErrorsService(errs.ToList()); return Unit(); },
                                 u => u);
        }

        private static void Design(EffectiveWidthViewModel vm)
        {
            vm.IsResistanceOutput = false;
            var material = (new Material(vm.MaterialVM.Fy, vm.MaterialVM.E, 0.3)).Convert(vm.GeneralInfoVM.Unit, _unitSystemDict[vm.GeneralInfoVM.DesignCode]);
            var bracingConditions = vm.BracingConditionsVM.AsEntity().Convert(vm.GeneralInfoVM.Unit, _unitSystemDict[vm.GeneralInfoVM.DesignCode]);
            var sa = vm.GeneralInfoVM.StrainingAction;
            var section = vm.GeneralInfoVM.SteelSection;
            vm.ResistanceOutput = vm.DesignDict[vm.GeneralInfoVM.DesignCode][KeyValuePair.Create(sa, section)](vm, material, bracingConditions);
        }



        private OutputBase EgyptDesignCLippedMomentCompression(EffectiveWidthViewModel vm, Material material, LengthBracingConditions bracingConditions)
        {
            var outTuple = vm.GeometryVM.Sections.Select(dto => Tuple.Create(dto, dto.AsEntity().Convert(vm.GeometryVM.TableUnit, UnitSystems.TONCM).AsLippedSection().AsEgyptInteractionResistance(material, bracingConditions,vm.InputLoadVM.UltimateLoad.ConvertForce(vm.GeneralInfoVM.Unit,UnitSystems.TONCM).Item1,vm.InputLoadVM.UltimateMoment.ConvertMoment(vm.GeneralInfoVM.Unit, UnitSystems.TONCM).Item1).Convert(UnitSystems.TONCM, vm.GeneralInfoVM.Unit)))
                                                                  .Where(tuple => tuple.Item2.IEValue  < 1)
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
            var outTuple = vm.GeometryVM.Sections.Select(dto => Tuple.Create(dto, dto.AsEntity().Convert(vm.GeometryVM.TableUnit, UnitSystems.TONCM).AsUnStiffenedSection().AsEgyptInteractionResistance(material, bracingConditions, vm.InputLoadVM.UltimateLoad.ConvertForce(vm.GeneralInfoVM.Unit,UnitSystems.TONCM).Item1, vm.InputLoadVM.UltimateMoment.ConvertMoment(vm.GeneralInfoVM.Unit, UnitSystems.TONCM).Item1).Convert(UnitSystems.TONCM, vm.GeneralInfoVM.Unit)))
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
            var outTuple = vm.GeometryVM.Sections.Select(dto => Tuple.Create(dto, dto.AsEntity().Convert(vm.GeometryVM.TableUnit, UnitSystems.TONCM).AsUnStiffenedSection().AsEgyptCompressionResistance(material, bracingConditions).Convert(UnitSystems.TONCM, vm.GeneralInfoVM.Unit)))
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
            var outTuple = vm.GeometryVM.Sections.Select(dto => Tuple.Create(dto, dto.AsEntity().Convert(vm.GeometryVM.TableUnit, UnitSystems.TONCM).AsLippedSection().AsEgyptCompressionResistance(material, bracingConditions).Convert(UnitSystems.TONCM, vm.GeneralInfoVM.Unit)))
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
            var outTuple = vm.GeometryVM.Sections.Select(dto => Tuple.Create(dto, dto.AsEntity().Convert(vm.GeometryVM.TableUnit, UnitSystems.TONCM).AsUnStiffenedSection().AsEgyptMomentResistance(material, bracingConditions).Convert(UnitSystems.TONCM, vm.GeneralInfoVM.Unit)))
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
            var outTuple = vm.GeometryVM.Sections.Select(dto => Tuple.Create(dto, dto.AsEntity().Convert(vm.GeometryVM.TableUnit, UnitSystems.TONCM).AsLippedSection().AsEgyptMomentResistance(material, bracingConditions).Convert(UnitSystems.TONCM, vm.GeneralInfoVM.Unit)))
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


        private OutputBase EuroDesignCLippedMomentCompression(EffectiveWidthViewModel vm, Material material, LengthBracingConditions bracingConditions)
        {
            var outTuple = vm.GeometryVM.Sections.Select(dto => Tuple.Create(dto, dto.AsEntity().Convert(vm.GeometryVM.TableUnit, UnitSystems.NMM).AsLippedSection().AsEuroInteractionResistance(material, bracingConditions, vm.InputLoadVM.UltimateLoad.ConvertForce(vm.GeneralInfoVM.Unit,UnitSystems.NMM).Item1, vm.InputLoadVM.UltimateMoment.ConvertMoment(vm.GeneralInfoVM.Unit, UnitSystems.NMM).Item1).Convert(UnitSystems.NMM, vm.GeneralInfoVM.Unit)))
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
            var outTuple = vm.GeometryVM.Sections.Select(dto => Tuple.Create(dto, dto.AsEntity().Convert(vm.GeometryVM.TableUnit, UnitSystems.NMM).AsUnStiffenedSection().AsEuroInteractionResistance(material, bracingConditions, vm.InputLoadVM.UltimateLoad.ConvertForce(vm.GeneralInfoVM.Unit,UnitSystems.NMM).Item1, vm.InputLoadVM.UltimateMoment.ConvertMoment(vm.GeneralInfoVM.Unit, UnitSystems.NMM).Item1).Convert(UnitSystems.NMM, vm.GeneralInfoVM.Unit)))
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
            var outTuple = vm.GeometryVM.Sections.Select(dto => Tuple.Create(dto, dto.AsEntity().Convert(vm.GeometryVM.TableUnit, UnitSystems.NMM).AsUnStiffenedSection().AsEuroCompressionResistance(material, bracingConditions).Convert(UnitSystems.NMM, vm.GeneralInfoVM.Unit)))
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
            var outTuple = vm.GeometryVM.Sections.Select(dto => Tuple.Create(dto, dto.AsEntity().Convert(vm.GeometryVM.TableUnit, UnitSystems.NMM).AsLippedSection().AsEuroCompressionResistance(material, bracingConditions).Convert(UnitSystems.NMM, vm.GeneralInfoVM.Unit)))
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
            var outTuple = vm.GeometryVM.Sections.Select(dto => Tuple.Create(dto, dto.AsEntity().Convert(vm.GeometryVM.TableUnit, UnitSystems.NMM).AsUnStiffenedSection().AsEuroMomentResistance(material, bracingConditions, vm.InputLoadVM.UltimateMoment.ConvertMoment(vm.GeneralInfoVM.Unit, UnitSystems.NMM).Item1).Convert(UnitSystems.NMM, vm.GeneralInfoVM.Unit)))
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
            var outTuple = vm.GeometryVM.Sections.Select(dto => Tuple.Create(dto, dto.AsEntity().Convert(vm.GeometryVM.TableUnit, UnitSystems.NMM).AsLippedSection().AsEuroMomentResistance(material, bracingConditions, vm.InputLoadVM.UltimateMoment.ConvertMoment(vm.GeneralInfoVM.Unit, UnitSystems.NMM).Item1).Convert(UnitSystems.NMM, vm.GeneralInfoVM.Unit)))
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


        private OutputBase AISIDesignCLippedMomentCompression(EffectiveWidthViewModel vm, Material material, LengthBracingConditions bracingConditions)
        {
            var outTuple = vm.GeometryVM.Sections.Select(dto => Tuple.Create(dto, dto.AsEntity().Convert(vm.GeometryVM.TableUnit, UnitSystems.KIPINCH).AsLippedSection().AsAISIInteractionResistance(material, bracingConditions, vm.InputLoadVM.UltimateLoad.ConvertForce(vm.GeneralInfoVM.Unit,UnitSystems.KIPINCH).Item1, vm.InputLoadVM.UltimateMoment.ConvertMoment(vm.GeneralInfoVM.Unit, UnitSystems.KIPINCH).Item1).Convert(UnitSystems.KIPINCH, vm.GeneralInfoVM.Unit)))
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
            var outTuple = vm.GeometryVM.Sections.Select(dto => Tuple.Create(dto, dto.AsEntity().Convert(vm.GeometryVM.TableUnit, UnitSystems.KIPINCH).AsUnStiffenedSection().AsAISIInteractionResistance(material, bracingConditions, vm.InputLoadVM.UltimateLoad.ConvertForce(vm.GeneralInfoVM.Unit,UnitSystems.KIPINCH).Item1, vm.InputLoadVM.UltimateMoment.ConvertMoment(vm.GeneralInfoVM.Unit, UnitSystems.KIPINCH).Item1).Convert(UnitSystems.KIPINCH, vm.GeneralInfoVM.Unit)))
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
            var outTuple = vm.GeometryVM.Sections.Select(dto => Tuple.Create(dto, dto.AsEntity().Convert(vm.GeometryVM.TableUnit, UnitSystems.KIPINCH).AsUnStiffenedSection().AsAISICompressionResistance(material, bracingConditions).Convert(UnitSystems.KIPINCH, vm.GeneralInfoVM.Unit)))
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
            var outTuple = vm.GeometryVM.Sections.Select(dto => Tuple.Create(dto, dto.AsEntity().Convert(vm.GeometryVM.TableUnit, UnitSystems.KIPINCH).AsLippedSection().AsAISICompressionResistance(material, bracingConditions).Convert(UnitSystems.KIPINCH, vm.GeneralInfoVM.Unit)))
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
            var outTuple = vm.GeometryVM.Sections.Select(dto => Tuple.Create(dto, dto.AsEntity().Convert(vm.GeometryVM.TableUnit, UnitSystems.KIPINCH).AsUnStiffenedSection().AsAISIMomentResistance(material, bracingConditions).Convert(UnitSystems.KIPINCH, vm.GeneralInfoVM.Unit)))
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
            var outTuple = vm.GeometryVM.Sections.Select(dto => Tuple.Create(dto, dto.AsEntity().Convert(vm.GeometryVM.TableUnit, UnitSystems.KIPINCH).AsLippedSection().AsAISIMomentResistance(material, bracingConditions).Convert(UnitSystems.KIPINCH, vm.GeneralInfoVM.Unit)))
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

        private static void Check(EffectiveWidthViewModel vm)
        {
            vm.IsResistanceOutput = false;
            var material = (new Material(vm.MaterialVM.Fy, vm.MaterialVM.E, 0.3)).Convert(vm.GeneralInfoVM.Unit, _unitSystemDict[vm.GeneralInfoVM.DesignCode]);
            var bracingConditions = vm.BracingConditionsVM.AsEntity().Convert(vm.GeneralInfoVM.Unit, _unitSystemDict[vm.GeneralInfoVM.DesignCode]);
            var sa = vm.GeneralInfoVM.StrainingAction;
            var section = vm.GeneralInfoVM.SteelSection;
            var resistOutput = vm.ResistDict[vm.GeneralInfoVM.DesignCode][KeyValuePair.Create(sa, section)](vm, material, bracingConditions);
            var result = resistOutput.Item1();
            vm.ResistanceOutput = result;

        }

        private static void Resistance(EffectiveWidthViewModel vm)
        {
            vm.IsResistanceOutput = false;
            var material = (new Material(vm.MaterialVM.Fy, vm.MaterialVM.E, vm.MaterialVM.V)).Convert(vm.GeneralInfoVM.Unit, _unitSystemDict[vm.GeneralInfoVM.DesignCode]);
            var bracingConditions = vm.BracingConditionsVM.AsEntity().Convert(vm.GeneralInfoVM.Unit, _unitSystemDict[vm.GeneralInfoVM.DesignCode]);
            var sa = vm.GeneralInfoVM.StrainingAction;
            var section = vm.GeneralInfoVM.SteelSection;
            var result = vm.ResistDict[vm.GeneralInfoVM.DesignCode][KeyValuePair.Create(sa, section)](vm, material, bracingConditions);
            vm.ResistanceOutput = result.Item2;
        }

        private Tuple<Func<OutputBase>, OutputBase> EgyptResistanceCLippedMomentCompression(EffectiveWidthViewModel vm, Material material, LengthBracingConditions bracingConditions)
        {
            var result = vm.GeometryVM.AsEntity()
                                         .Convert(vm.GeneralInfoVM.Unit, UnitSystems.TONCM)
                                         .AsLippedSection()
                                         .AsEgyptInteractionResistance(material, bracingConditions, vm.InputLoadVM.UltimateLoad.ConvertForce(vm.GeneralInfoVM.Unit, UnitSystems.TONCM).Item1, vm.InputLoadVM.UltimateMoment.ConvertMoment(vm.GeneralInfoVM.Unit, UnitSystems.TONCM).Item1)
                                         .Convert(UnitSystems.TONCM, vm.GeneralInfoVM.Unit);

            Func<OutputBase> fun = () => result.AsCheck(_momentDict[vm.GeneralInfoVM.Unit], _compDict[vm.GeneralInfoVM.Unit]);

            return Tuple.Create(fun, result as OutputBase);
        }

        private Tuple<Func<OutputBase>, OutputBase> EgyptResistanceCUnstiffMomentCompression(EffectiveWidthViewModel vm, Material material, LengthBracingConditions bracingConditions)
        {
            var result = vm.GeometryVM.AsEntity()
                                       .Convert(vm.GeneralInfoVM.Unit, UnitSystems.TONCM)
                                       .AsUnStiffenedSection()
                                       .AsEgyptInteractionResistance(material, bracingConditions, vm.InputLoadVM.UltimateLoad.ConvertForce(vm.GeneralInfoVM.Unit, UnitSystems.TONCM).Item1, vm.InputLoadVM.UltimateMoment.ConvertMoment(vm.GeneralInfoVM.Unit, UnitSystems.TONCM).Item1)
                                       .Convert(UnitSystems.TONCM, vm.GeneralInfoVM.Unit);

            Func<OutputBase> fun = () => result.AsCheck(_momentDict[vm.GeneralInfoVM.Unit], _compDict[vm.GeneralInfoVM.Unit]);

            return Tuple.Create(fun, result as OutputBase);
        }


        private static Tuple<Func<OutputBase>, OutputBase> EgyptResistanceCUnstiffComp(EffectiveWidthViewModel vm, Material material, LengthBracingConditions bracingConditions)
        {
            var result = vm.GeometryVM.AsEntity()
                                       .Convert(vm.GeneralInfoVM.Unit, UnitSystems.TONCM)
                                       .AsUnStiffenedSection()
                                       .AsEgyptCompressionResistance(material, bracingConditions)
                                       .Convert(UnitSystems.TONCM, vm.GeneralInfoVM.Unit);

            Func<OutputBase> fun = () => result.AsCheck(vm.InputLoadVM.UltimateLoad, _compDict[vm.GeneralInfoVM.Unit]);

            return Tuple.Create(fun, result as OutputBase);

        }
        private static Tuple<Func<OutputBase>, OutputBase> EgyptResistanceCUnstiffMoment(EffectiveWidthViewModel vm, Material material, LengthBracingConditions bracingConditions)
        {
            var result = vm.GeometryVM.AsEntity()
                                       .Convert(vm.GeneralInfoVM.Unit, UnitSystems.TONCM)
                                       .AsUnStiffenedSection()
                                       .AsEgyptMomentResistance(material, bracingConditions)
                                       .Convert(UnitSystems.TONCM, vm.GeneralInfoVM.Unit);
            Func<OutputBase> fun = () => result.AsCheck(vm.InputLoadVM.UltimateMoment, _momentDict[vm.GeneralInfoVM.Unit]);

            return Tuple.Create(fun, result as OutputBase);
        }
        private static Tuple<Func<OutputBase>, OutputBase> EgyptResistanceCLippedComp(EffectiveWidthViewModel vm, Material material, LengthBracingConditions bracingConditions)
        {
            var result = vm.GeometryVM.AsEntity()
                                      .Convert(vm.GeneralInfoVM.Unit, UnitSystems.TONCM)
                                      .AsLippedSection()
                                      .AsEgyptCompressionResistance(material, bracingConditions)
                                      .Convert(UnitSystems.TONCM, vm.GeneralInfoVM.Unit);

            Func<OutputBase> fun = () => result.AsCheck(vm.InputLoadVM.UltimateLoad, _compDict[vm.GeneralInfoVM.Unit]);

            return Tuple.Create(fun, result as OutputBase);
        }
        private static Tuple<Func<OutputBase>, OutputBase> EgyptResistanceCLippedMoment(EffectiveWidthViewModel vm, Material material, LengthBracingConditions bracingConditions)
        {
            var result = vm.GeometryVM.AsEntity()
                                      .Convert(vm.GeneralInfoVM.Unit, UnitSystems.TONCM)
                                      .AsLippedSection()
                                      .AsEgyptMomentResistance(material, bracingConditions).Convert(UnitSystems.TONCM, vm.GeneralInfoVM.Unit);
            Func<OutputBase> fun = () => result.AsCheck(vm.InputLoadVM.UltimateMoment, _momentDict[vm.GeneralInfoVM.Unit]);
            return Tuple.Create(fun, result as OutputBase);
        }


        private Tuple<Func<OutputBase>, OutputBase> EuroResistanceCLippedMomentCompression(EffectiveWidthViewModel vm, Material material, LengthBracingConditions bracingConditions)
        {
            var result = vm.GeometryVM.AsEntity()
                                        .Convert(vm.GeneralInfoVM.Unit, UnitSystems.NMM)
                                        .AsLippedSection()
                                        .AsEuroInteractionResistance(material, bracingConditions,vm.InputLoadVM.UltimateLoad,vm.InputLoadVM.UltimateMoment)
                                        .Convert(UnitSystems.NMM, vm.GeneralInfoVM.Unit);

            Func<OutputBase> fun = () => result.AsCheck(_momentDict[vm.GeneralInfoVM.Unit], _compDict[vm.GeneralInfoVM.Unit]);

            return Tuple.Create(fun, result as OutputBase);
        }

        private Tuple<Func<OutputBase>, OutputBase> EuroResistanceCUnstiffMomentCompression(EffectiveWidthViewModel vm, Material material, LengthBracingConditions bracingConditions)
        {
            var result = vm.GeometryVM.AsEntity()
                                       .Convert(vm.GeneralInfoVM.Unit, UnitSystems.NMM)
                                       .AsUnStiffenedSection()
                                       .AsEuroInteractionResistance(material, bracingConditions, vm.InputLoadVM.UltimateLoad.ConvertForce(vm.GeneralInfoVM.Unit, UnitSystems.NMM).Item1, vm.InputLoadVM.UltimateMoment.ConvertMoment(vm.GeneralInfoVM.Unit, UnitSystems.NMM).Item1)
                                       .Convert(UnitSystems.NMM, vm.GeneralInfoVM.Unit);

            Func<OutputBase> fun = () => result.AsCheck(_momentDict[vm.GeneralInfoVM.Unit], _compDict[vm.GeneralInfoVM.Unit]);

            return Tuple.Create(fun, result as OutputBase);
        }


        private static Tuple<Func<OutputBase>, OutputBase> EuroResistanceCUnstiffComp(EffectiveWidthViewModel vm, Material material, LengthBracingConditions bracingConditions)
        {
            var result = vm.GeometryVM.AsEntity()
                                       .Convert(vm.GeneralInfoVM.Unit, UnitSystems.NMM)
                                       .AsUnStiffenedSection()
                                       .AsEuroCompressionResistance(material, bracingConditions)
                                       .Convert(UnitSystems.NMM, vm.GeneralInfoVM.Unit);

            Func<OutputBase> fun = () => result.AsCheck(vm.InputLoadVM.UltimateLoad, _compDict[vm.GeneralInfoVM.Unit]);

            return Tuple.Create(fun, result as OutputBase);

        }

        private static Tuple<Func<OutputBase>, OutputBase> EuroResistanceCUnstiffMoment(EffectiveWidthViewModel vm, Material material, LengthBracingConditions bracingConditions)
        {
            var result = vm.GeometryVM.AsEntity()
                                       .Convert(vm.GeneralInfoVM.Unit, UnitSystems.NMM)
                                       .AsUnStiffenedSection()
                                       .AsEuroMomentResistance(material, bracingConditions, vm.InputLoadVM.UltimateMoment.ConvertMoment(vm.GeneralInfoVM.Unit, UnitSystems.NMM).Item1)
                                       .Convert(UnitSystems.NMM, vm.GeneralInfoVM.Unit);
            Func<OutputBase> fun = () => result.AsCheck(vm.InputLoadVM.UltimateMoment, _momentDict[vm.GeneralInfoVM.Unit]);

            return Tuple.Create(fun, result as OutputBase);
        }

        private static Tuple<Func<OutputBase>, OutputBase> EuroResistanceCLippedComp(EffectiveWidthViewModel vm, Material material, LengthBracingConditions bracingConditions)
        {
            var result = vm.GeometryVM.AsEntity()
                                      .Convert(vm.GeneralInfoVM.Unit, UnitSystems.NMM)
                                      .AsLippedSection()
                                      .AsEuroCompressionResistance(material, bracingConditions)
                                      .Convert(UnitSystems.NMM, vm.GeneralInfoVM.Unit);

            Func<OutputBase> fun = () => result.AsCheck(vm.InputLoadVM.UltimateLoad, _compDict[vm.GeneralInfoVM.Unit]);

            return Tuple.Create(fun, result as OutputBase);
        }
        private static Tuple<Func<OutputBase>, OutputBase> EuroResistanceCLippedMoment(EffectiveWidthViewModel vm, Material material, LengthBracingConditions bracingConditions)
        {
            var result = vm.GeometryVM.AsEntity()
                                      .Convert(vm.GeneralInfoVM.Unit, UnitSystems.NMM)
                                      .AsLippedSection()
                                      .AsEuroMomentResistance(material, bracingConditions, vm.InputLoadVM.UltimateMoment.ConvertMoment(vm.GeneralInfoVM.Unit, UnitSystems.NMM).Item1).Convert(UnitSystems.NMM, vm.GeneralInfoVM.Unit);
            Func<OutputBase> fun = () => result.AsCheck(vm.InputLoadVM.UltimateMoment, _momentDict[vm.GeneralInfoVM.Unit]);
            return Tuple.Create(fun, result as OutputBase);
        }


        private Tuple<Func<OutputBase>, OutputBase> AISIResistanceCLippedMomentCompression(EffectiveWidthViewModel vm, Material material, LengthBracingConditions bracingConditions)
        {
            var result = vm.GeometryVM.AsEntity()
                                        .Convert(vm.GeneralInfoVM.Unit, UnitSystems.KIPINCH)
                                        .AsLippedSection()
                                        .AsAISIInteractionResistance(material, bracingConditions, vm.InputLoadVM.UltimateLoad.ConvertForce(vm.GeneralInfoVM.Unit, UnitSystems.KIPINCH).Item1, vm.InputLoadVM.UltimateMoment.ConvertMoment(vm.GeneralInfoVM.Unit, UnitSystems.KIPINCH).Item1)
                                        .Convert(UnitSystems.KIPINCH, vm.GeneralInfoVM.Unit);

            Func<OutputBase> fun = () => result.AsCheck(_momentDict[vm.GeneralInfoVM.Unit], _compDict[vm.GeneralInfoVM.Unit]);

            return Tuple.Create(fun, result as OutputBase);
        }

        private Tuple<Func<OutputBase>, OutputBase> AISIResistanceCUnstiffMomentCompression(EffectiveWidthViewModel vm, Material material, LengthBracingConditions bracingConditions)
        {
            var result = vm.GeometryVM.AsEntity()
                                       .Convert(vm.GeneralInfoVM.Unit, UnitSystems.KIPINCH)
                                       .AsUnStiffenedSection()
                                       .AsAISIInteractionResistance(material, bracingConditions, vm.InputLoadVM.UltimateLoad.ConvertForce(vm.GeneralInfoVM.Unit, UnitSystems.KIPINCH).Item1, vm.InputLoadVM.UltimateMoment.ConvertMoment(vm.GeneralInfoVM.Unit, UnitSystems.KIPINCH).Item1)
                                       .Convert(UnitSystems.KIPINCH, vm.GeneralInfoVM.Unit);

            Func<OutputBase> fun = () => result.AsCheck(_momentDict[vm.GeneralInfoVM.Unit], _compDict[vm.GeneralInfoVM.Unit]);

            return Tuple.Create(fun, result as OutputBase);
        }

        private static Tuple<Func<OutputBase>, OutputBase> AISIResistanceCUnstiffComp(EffectiveWidthViewModel vm, Material material, LengthBracingConditions bracingConditions)
        {
            var result = vm.GeometryVM.AsEntity()
                                       .Convert(vm.GeneralInfoVM.Unit, UnitSystems.KIPINCH)
                                       .AsUnStiffenedSection()
                                       .AsAISICompressionResistance(material, bracingConditions)
                                       .Convert(UnitSystems.KIPINCH, vm.GeneralInfoVM.Unit);

            Func<OutputBase> fun = () => result.AsCheck(vm.InputLoadVM.UltimateLoad, _compDict[vm.GeneralInfoVM.Unit]);

            return Tuple.Create(fun, result as OutputBase);

        }
        private static Tuple<Func<OutputBase>, OutputBase> AISIResistanceCUnstiffMoment(EffectiveWidthViewModel vm, Material material, LengthBracingConditions bracingConditions)
        {
            var result = vm.GeometryVM.AsEntity()
                                       .Convert(vm.GeneralInfoVM.Unit, UnitSystems.KIPINCH)
                                       .AsUnStiffenedSection()
                                       .AsAISIMomentResistance(material, bracingConditions)
                                       .Convert(UnitSystems.KIPINCH, vm.GeneralInfoVM.Unit);
            Func<OutputBase> fun = () => result.AsCheck(vm.InputLoadVM.UltimateMoment, _momentDict[vm.GeneralInfoVM.Unit]);

            return Tuple.Create(fun, result as OutputBase);
        }

        private static Tuple<Func<OutputBase>, OutputBase> AISIResistanceCLippedComp(EffectiveWidthViewModel vm, Material material, LengthBracingConditions bracingConditions)
        {
            var result = vm.GeometryVM.AsEntity()
                                      .Convert(vm.GeneralInfoVM.Unit, UnitSystems.KIPINCH)
                                      .AsLippedSection()
                                      .AsAISICompressionResistance(material, bracingConditions)
                                      .Convert(UnitSystems.KIPINCH, vm.GeneralInfoVM.Unit);

            Func<OutputBase> fun = () => result.AsCheck(vm.InputLoadVM.UltimateLoad, _compDict[vm.GeneralInfoVM.Unit]);

            return Tuple.Create(fun, result as OutputBase);
        }

        private static Tuple<Func<OutputBase>, OutputBase> AISIResistanceCLippedMoment(EffectiveWidthViewModel vm, Material material, LengthBracingConditions bracingConditions)
        {
            var result = vm.GeometryVM.AsEntity()
                                      .Convert(vm.GeneralInfoVM.Unit, UnitSystems.KIPINCH)
                                      .AsLippedSection()
                                      .AsAISIMomentResistance(material, bracingConditions).Convert(UnitSystems.KIPINCH, vm.GeneralInfoVM.Unit);
            Func<OutputBase> fun = () => result.AsCheck(vm.InputLoadVM.UltimateMoment, _momentDict[vm.GeneralInfoVM.Unit]);
            return Tuple.Create(fun, result as OutputBase);
        }

        #endregion

    }
}
