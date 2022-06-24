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
    public class DirectStrengthViewModel : ResistanceBaseViewModel
    {
        #region Properties

        public override ICommand ResultsCommand { get; }


        private readonly Dictionary<Module, Action<DirectStrengthViewModel>> _moduleDict;

        private List<Func<List<Error>>> _validateFuncs;
        #endregion

        #region Properties

        public Dictionary<KeyValuePair<StrainingActions, SteelSection>, Func<DirectStrengthViewModel, Material, LengthBracingConditions, OutputBase>> DesignDict { get; }


        public Dictionary<KeyValuePair<StrainingActions, SteelSection>, Func<DirectStrengthViewModel, Material, LengthBracingConditions, Tuple<Func<OutputBase>, OutputBase>>> ResistDict { get; }

        #endregion

        #region Constructors

        public DirectStrengthViewModel(GeneralInfoViewModel generalInfoVM, BracingConditionsViewModel bracingConditionsVM, GeometryViewModel geometryVM, MaterialViewModel materialVM, InputLoadViewModel inputLoadVM, Func<List<Error>, Unit> showErrorsService, Action<IReport> reportService)
            : base(generalInfoVM, bracingConditionsVM, geometryVM, materialVM, inputLoadVM, showErrorsService, reportService)
        {
            ResultsCommand = new RelayCommand(OnReults, CanResults);
            _moduleDict = new Dictionary<Module, Action<DirectStrengthViewModel>>()
            {
                [Module.RESISTANCE] = Resistance,
                [Module.DESIGN] = Design,
                [Module.CHECK] = Check
            };
            ResistDict = new Dictionary<KeyValuePair<StrainingActions, SteelSection>, Func<DirectStrengthViewModel, Material, LengthBracingConditions, Tuple<Func<OutputBase>, OutputBase>>>()
            {
                [KeyValuePair.Create(StrainingActions.COMPRESSION, SteelSection.C_UNSTIFFENED)] = ResistanceCUnstiffComp,
                [KeyValuePair.Create(StrainingActions.MOMENT, SteelSection.C_UNSTIFFENED)] = ResistanceCUnstiffMoment,
                [KeyValuePair.Create(StrainingActions.MOMENT_COMPRESSION, SteelSection.C_UNSTIFFENED)] = ResistanceCUnstiffMomentComp,
                [KeyValuePair.Create(StrainingActions.COMPRESSION, SteelSection.C_LIPPED)] = ResistanceCLippedComp,
                [KeyValuePair.Create(StrainingActions.MOMENT, SteelSection.C_LIPPED)] = ResistanceCLippedMoment,
                [KeyValuePair.Create(StrainingActions.MOMENT_COMPRESSION, SteelSection.C_LIPPED)] = ResistanceCLippedMomentComp,
            };
            DesignDict = new Dictionary<KeyValuePair<StrainingActions, SteelSection>, Func<DirectStrengthViewModel, Material, LengthBracingConditions, OutputBase>>()
            {
                [KeyValuePair.Create(StrainingActions.COMPRESSION, SteelSection.C_UNSTIFFENED)] = DesignCUnstiffComp,
                [KeyValuePair.Create(StrainingActions.MOMENT, SteelSection.C_UNSTIFFENED)] = DesignCUnstiffMoment,
                [KeyValuePair.Create(StrainingActions.MOMENT_COMPRESSION, SteelSection.C_UNSTIFFENED)] = DesignCUnstiffMomentComp,
                [KeyValuePair.Create(StrainingActions.COMPRESSION, SteelSection.C_LIPPED)] = DesignCLippedComp,
                [KeyValuePair.Create(StrainingActions.MOMENT, SteelSection.C_LIPPED)] = DesignCLippedMoment,
                [KeyValuePair.Create(StrainingActions.MOMENT_COMPRESSION, SteelSection.C_LIPPED)] = DesignCLippedMomentComp,
            };
            _validateFuncs = new List<Func<List<Error>>>()
            {
                MaterialVM.Validate,
                GeometryVM.Validate,
                InputLoadVM.Validate,
                BracingConditionsVM.Validate
            };
        }


        #endregion


        #region Methods




        private Tuple<Func<OutputBase>, OutputBase> ResistanceCLippedMomentComp(DirectStrengthViewModel vm, Material material, LengthBracingConditions bracingConditions)
        {
            var result = vm.GeometryVM.AsEntity()
                                       .Convert(vm.GeneralInfoVM.Unit, UnitSystems.KIPINCH)
                                       .AsLippedSection()
                                       .AsDSInteractionResistance(material, bracingConditions, vm.InputLoadVM.UltimateLoad.ConvertForce(vm.GeneralInfoVM.Unit, UnitSystems.KIPINCH).Item1, vm.InputLoadVM.UltimateMoment.ConvertMoment(vm.GeneralInfoVM.Unit, UnitSystems.KIPINCH).Item1)
                                       .Convert(UnitSystems.KIPINCH, vm.GeneralInfoVM.Unit);

            Func<OutputBase> fun = () => result.AsCheck();

            return Tuple.Create(fun, result as OutputBase);
        }

        private Tuple<Func<OutputBase>, OutputBase> ResistanceCUnstiffMomentComp(DirectStrengthViewModel vm, Material material, LengthBracingConditions bracingConditions)
        {
            var result = vm.GeometryVM.AsEntity()
                                        .Convert(vm.GeneralInfoVM.Unit, UnitSystems.KIPINCH)
                                        .AsUnStiffenedSection()
                                        .AsDSInteractionResistance(material, bracingConditions, vm.InputLoadVM.UltimateLoad.ConvertForce(vm.GeneralInfoVM.Unit, UnitSystems.KIPINCH).Item1, vm.InputLoadVM.UltimateMoment.ConvertMoment(vm.GeneralInfoVM.Unit, UnitSystems.KIPINCH).Item1)
                                        .Convert(UnitSystems.KIPINCH, vm.GeneralInfoVM.Unit);

            Func<OutputBase> fun = () => result.AsCheck();

            return Tuple.Create(fun, result as OutputBase);
        }



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

        private static void Design(DirectStrengthViewModel vm)
        {
            vm.IsResistanceOutput = false;
            var material = (new Material(vm.MaterialVM.Fy, vm.MaterialVM.E, 0.3)).Convert(vm.GeneralInfoVM.Unit, UnitSystems.KIPINCH);
            var bracingConditions = vm.BracingConditionsVM.AsEntity().Convert(vm.GeneralInfoVM.Unit, UnitSystems.KIPINCH);
            var sa = vm.GeneralInfoVM.StrainingAction;
            var section = vm.GeneralInfoVM.SteelSection;
            vm.ResistanceOutput = vm.DesignDict[KeyValuePair.Create(sa, section)](vm, material, bracingConditions);
        }


        private DesignInteractionOutput DesignCLippedMomentComp(DirectStrengthViewModel vm, Material material, LengthBracingConditions bracingConditions)
        {
            var outTuple = vm.GeometryVM.Sections.Select(dto => Tuple.Create(dto, dto.AsEntity().Convert(vm.GeometryVM.TableUnit, UnitSystems.KIPINCH).AsLippedSection().AsDSInteractionResistance(material, bracingConditions, vm.InputLoadVM.UltimateLoad.ConvertForce(vm.GeneralInfoVM.Unit, UnitSystems.KIPINCH).Item1, vm.InputLoadVM.UltimateMoment.ConvertMoment(vm.GeneralInfoVM.Unit, UnitSystems.KIPINCH).Item1).Convert(UnitSystems.KIPINCH, vm.GeneralInfoVM.Unit)))
                                                                    .Where(tuple => tuple.Item2.IEValue < 1)
                                                                     .OrderByDescending(tuple => tuple.Item2.IEValue)
                                                                      .FirstOrDefault();
            vm.IsResistanceOutput = true;
            if (outTuple != null)
            {
                (var secDto, var ressistance) = outTuple;
                vm.GeometryVM.SelectedSection = secDto;
                return ressistance.AsDesign(secDto.ID);
            }
            else
            {
                vm.GeometryVM.SelectedSection = null;
                return null;
            }
        }

        private DesignInteractionOutput DesignCUnstiffMomentComp(DirectStrengthViewModel vm, Material material, LengthBracingConditions bracingConditions)
        {

            var outTuple = vm.GeometryVM.Sections.Select(dto => Tuple.Create(dto, dto.AsEntity().Convert(vm.GeometryVM.TableUnit, UnitSystems.KIPINCH).AsUnStiffenedSection().AsDSInteractionResistance(material, bracingConditions, vm.InputLoadVM.UltimateLoad.ConvertForce(vm.GeneralInfoVM.Unit, UnitSystems.KIPINCH).Item1, vm.InputLoadVM.UltimateMoment.ConvertMoment(vm.GeneralInfoVM.Unit, UnitSystems.KIPINCH).Item1).Convert(UnitSystems.KIPINCH, vm.GeneralInfoVM.Unit)))
                                                                   .Where(tuple => tuple.Item2.IEValue < 1)
                                                                    .OrderByDescending(tuple => tuple.Item2.IEValue)
                                                                     .FirstOrDefault();
            vm.IsResistanceOutput = true;
            if (outTuple != null)
            {
                (var secDto, var ressistance) = outTuple;
                vm.GeometryVM.SelectedSection = secDto;
                return ressistance.AsDesign(secDto.ID);
            }
            else
            {
                vm.GeometryVM.SelectedSection = null;
                return null;
            }
        }

        private static DesignOutput DesignCUnstiffComp(DirectStrengthViewModel vm, Material material, LengthBracingConditions bracingConditions)
        {
            var outTuple  = vm.GeometryVM.Sections.Select(dto => Tuple.Create(dto, dto.AsEntity().Convert(vm.GeometryVM.TableUnit, UnitSystems.KIPINCH).AsUnStiffenedSection().AsDSCompressionResistance(material, bracingConditions).Convert(UnitSystems.KIPINCH, vm.GeneralInfoVM.Unit)))
                                                                   .Where(tuple => tuple.Item2.DesignResistance > vm.InputLoadVM.UltimateLoad)
                                                                    .OrderBy(tuple => tuple.Item2.DesignResistance)
                                                                     .FirstOrDefault();
            vm.IsResistanceOutput = true;
            if (outTuple != null)
            {
                (var secDto, var ressistance) = outTuple;
                vm.GeometryVM.SelectedSection = secDto;
                return ressistance.AsDesign(vm.InputLoadVM.UltimateLoad, secDto.ID);
            }
            else
            {
                vm.GeometryVM.SelectedSection = null;
                return null;
            }
        }

        private static DesignOutput DesignCLippedComp(DirectStrengthViewModel vm, Material material, LengthBracingConditions bracingConditions)
        {
            var outTuple = vm.GeometryVM.Sections.Select(dto => Tuple.Create(dto, dto.AsEntity().Convert(vm.GeometryVM.TableUnit, UnitSystems.KIPINCH).AsLippedSection().AsDSCompressionResistance(material, bracingConditions).Convert(UnitSystems.KIPINCH, vm.GeneralInfoVM.Unit)))
                                                                  .Where(tuple => tuple.Item2.DesignResistance > vm.InputLoadVM.UltimateLoad)
                                                                   .OrderBy(tuple => tuple.Item2.DesignResistance)
                                                                    .FirstOrDefault();
            vm.IsResistanceOutput = true;
            if (outTuple != null)
            {
                (var secDto, var ressistance) = outTuple;
                vm.GeometryVM.SelectedSection = secDto;
                return ressistance.AsDesign(vm.InputLoadVM.UltimateLoad, secDto.ID);
            }
            else
            {
                vm.GeometryVM.SelectedSection = null;
                return null;
            }
        }


        private static DesignOutput DesignCUnstiffMoment(DirectStrengthViewModel vm, Material material, LengthBracingConditions bracingConditions)
        {
            var outTuple = vm.GeometryVM.Sections.Select(dto => Tuple.Create(dto, dto.AsEntity().Convert(vm.GeometryVM.TableUnit, UnitSystems.KIPINCH).AsUnStiffenedSection().AsDSMomentResistance(material, bracingConditions).Convert(UnitSystems.KIPINCH, vm.GeneralInfoVM.Unit)))
                                                                  .Where(tuple => tuple.Item2.DesignResistance > vm.InputLoadVM.UltimateMoment)
                                                                   .OrderBy(tuple => tuple.Item2.DesignResistance)
                                                                    .FirstOrDefault();
            vm.IsResistanceOutput = true;
            if (outTuple != null)
            {
                (var secDto, var ressistance) = outTuple;
                vm.GeometryVM.SelectedSection = secDto;
                return ressistance.AsDesign(vm.InputLoadVM.UltimateMoment, secDto.ID);
            }
            else
            {
                vm.GeometryVM.SelectedSection = null;
                return null;
            }
        }

        private static DesignOutput DesignCLippedMoment(DirectStrengthViewModel vm, Material material, LengthBracingConditions bracingConditions)
        {
            var outTuple = vm.GeometryVM.Sections.Select(dto => Tuple.Create(dto, dto.AsEntity().Convert(vm.GeometryVM.TableUnit, UnitSystems.KIPINCH).AsLippedSection().AsDSMomentResistance(material, bracingConditions).Convert(UnitSystems.KIPINCH, vm.GeneralInfoVM.Unit)))
                                                                  .Where(tuple => tuple.Item2.DesignResistance > vm.InputLoadVM.UltimateMoment)
                                                                   .OrderBy(tuple => tuple.Item2.DesignResistance)
                                                                    .FirstOrDefault();
            vm.IsResistanceOutput = true;
            if (outTuple != null)
            {
                (var secDto, var ressistance) = outTuple;
                vm.GeometryVM.SelectedSection = secDto;
                return ressistance.AsDesign(vm.InputLoadVM.UltimateMoment, secDto.ID);
            }
            else
            {
                vm.GeometryVM.SelectedSection = null;
                return null;
            }
        }

        private static void Check(DirectStrengthViewModel vm)
        {
            vm.IsResistanceOutput = false;
            var material = (new Material(vm.MaterialVM.Fy, vm.MaterialVM.E, 0.3)).Convert(vm.GeneralInfoVM.Unit, UnitSystems.KIPINCH);
            var bracingConditions = vm.BracingConditionsVM.AsEntity().Convert(vm.GeneralInfoVM.Unit, UnitSystems.KIPINCH);
            var sa = vm.GeneralInfoVM.StrainingAction;
            var section = vm.GeneralInfoVM.SteelSection;
            var resistOutput = vm.ResistDict[KeyValuePair.Create(sa, section)](vm, material, bracingConditions);
            var result = resistOutput.Item1();
            vm.ResistanceOutput = result;
        }


        private static void Resistance(DirectStrengthViewModel vm)
        {
            vm.IsResistanceOutput = false;
            var material = (new Material(vm.MaterialVM.Fy, vm.MaterialVM.E, vm.MaterialVM.V)).Convert(vm.GeneralInfoVM.Unit, UnitSystems.KIPINCH);
            var bracingConditions = vm.BracingConditionsVM.AsEntity().Convert(vm.GeneralInfoVM.Unit, UnitSystems.KIPINCH);
            var sa = vm.GeneralInfoVM.StrainingAction;
            var section = vm.GeneralInfoVM.SteelSection;
            var result = vm.ResistDict[KeyValuePair.Create(sa, section)](vm, material, bracingConditions);
            vm.ResistanceOutput = result.Item2;
        }

        private static Tuple<Func<OutputBase>, OutputBase> ResistanceCUnstiffComp(DirectStrengthViewModel vm, Material material, LengthBracingConditions bracingConditions)
        {
            var result = vm.GeometryVM.AsEntity()
                                       .Convert(vm.GeneralInfoVM.Unit, UnitSystems.KIPINCH)
                                       .AsUnStiffenedSection()
                                       .AsDSCompressionResistance(material, bracingConditions)
                                       .Convert(UnitSystems.KIPINCH, vm.GeneralInfoVM.Unit);

            Func<OutputBase> fun = () => result.AsCheck(vm.InputLoadVM.UltimateLoad);

            return Tuple.Create(fun, result as OutputBase);

        }
        private static Tuple<Func<OutputBase>, OutputBase> ResistanceCUnstiffMoment(DirectStrengthViewModel vm, Material material, LengthBracingConditions bracingConditions)
        {
            var result = vm.GeometryVM.AsEntity()
                                       .Convert(vm.GeneralInfoVM.Unit, UnitSystems.KIPINCH)
                                       .AsUnStiffenedSection()
                                       .AsDSMomentResistance(material, bracingConditions)
                                       .Convert(UnitSystems.KIPINCH, vm.GeneralInfoVM.Unit);
            Func<OutputBase> fun = () => result.AsCheck(vm.InputLoadVM.UltimateMoment);

            return Tuple.Create(fun, result as OutputBase);
        }
        private static Tuple<Func<OutputBase>, OutputBase> ResistanceCLippedComp(DirectStrengthViewModel vm, Material material, LengthBracingConditions bracingConditions)
        {
            var result = vm.GeometryVM.AsEntity()
                                      .Convert(vm.GeneralInfoVM.Unit, UnitSystems.KIPINCH)
                                      .AsLippedSection()
                                      .AsDSCompressionResistance(material, bracingConditions)
                                      .Convert(UnitSystems.KIPINCH, vm.GeneralInfoVM.Unit);

            Func<OutputBase> fun = () => result.AsCheck(vm.InputLoadVM.UltimateLoad);

            return Tuple.Create(fun, result as OutputBase);
        }
        private static Tuple<Func<OutputBase>, OutputBase> ResistanceCLippedMoment(DirectStrengthViewModel vm, Material material, LengthBracingConditions bracingConditions)
        {
            var result = vm.GeometryVM.AsEntity()
                                      .Convert(vm.GeneralInfoVM.Unit, UnitSystems.KIPINCH)
                                      .AsLippedSection()
                                      .AsDSMomentResistance(material, bracingConditions).Convert(UnitSystems.KIPINCH, vm.GeneralInfoVM.Unit);
            Func<OutputBase> fun = () => result.AsCheck(vm.InputLoadVM.UltimateMoment);
            return Tuple.Create(fun, result as OutputBase);
        }
        #endregion

    }
}
