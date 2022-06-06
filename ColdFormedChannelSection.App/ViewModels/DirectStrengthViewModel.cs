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

        public Dictionary<KeyValuePair<StrainingActions, SteelSection>,Func<DirectStrengthViewModel,Material,LengthBracingConditions, IOutput>> DesignDict { get; }


        public Dictionary<KeyValuePair<StrainingActions, SteelSection>, Func<DirectStrengthViewModel,Material,LengthBracingConditions, Tuple<Func<IOutput>, IOutput>>> ResistDict { get; }
        
        #endregion

        #region Constructors

        public DirectStrengthViewModel(GeneralInfoViewModel generalInfoVM, BracingConditionsViewModel bracingConditionsVM, GeometryViewModel geometryVM, MaterialViewModel materialVM, InputLoadViewModel inputLoadVM,Func<List<Error>,Unit> showErrorsService)
            : base(generalInfoVM, bracingConditionsVM, geometryVM, materialVM, inputLoadVM, showErrorsService)
        {
            ResultsCommand = new RelayCommand(OnReults, CanResults);
            _moduleDict = new Dictionary<Module, Action<DirectStrengthViewModel>>()
            {
                [Module.RESISTANCE] = Resistance,
                [Module.DESIGN] = Design,
                [Module.CHECK] = Check
            };
            ResistDict = new Dictionary<KeyValuePair<StrainingActions, SteelSection>, Func<DirectStrengthViewModel,Material,LengthBracingConditions, Tuple<Func<IOutput>, IOutput>>>()
            {
                [KeyValuePair.Create(StrainingActions.COMPRESSION,SteelSection.C_UNSTIFFENED)]= ResistanceCUnstiffComp,
                [KeyValuePair.Create(StrainingActions.MOMENT,SteelSection.C_UNSTIFFENED)] = ResistanceCUnstiffMoment,
                [KeyValuePair.Create(StrainingActions.MOMENT_COMPRESSION, SteelSection.C_UNSTIFFENED)] = ResistanceCUnstiffMomentComp,
                [KeyValuePair.Create(StrainingActions.COMPRESSION,SteelSection.C_LIPPED)] = ResistanceCLippedComp,
                [KeyValuePair.Create(StrainingActions.MOMENT,SteelSection.C_LIPPED)]=ResistanceCLippedMoment,
                [KeyValuePair.Create(StrainingActions.MOMENT_COMPRESSION, SteelSection.C_LIPPED)] = ResistanceCLippedMomentComp,
            };
            DesignDict = new Dictionary<KeyValuePair<StrainingActions, SteelSection>,Func<DirectStrengthViewModel,Material,LengthBracingConditions, IOutput>>()
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


        private Tuple<Func<IOutput>, IOutput> ResistanceCLippedMomentComp(DirectStrengthViewModel vm, Material material, LengthBracingConditions bracingConditions)
        {
            var result = vm.GeometryVM.AsEntity()
                                       .Convert(vm.GeneralInfoVM.Unit, Units.KIPINCH)
                                       .AsLippedSection()
                                       .AsDSInteractionResistance(material, bracingConditions,vm.InputLoadVM.UltimateLoad.ConvertForce(vm.GeneralInfoVM.Unit, Units.KIPINCH).Item1,vm.InputLoadVM.UltimateMoment.ConvertMoment(vm.GeneralInfoVM.Unit, Units.KIPINCH).Item1)
                                       .Convert(Units.KIPINCH, vm.GeneralInfoVM.Unit);

            Func<IOutput> fun = () => result.AsCheck();

            return Tuple.Create(fun, result as IOutput);
        }

        private Tuple<Func<IOutput>, IOutput> ResistanceCUnstiffMomentComp(DirectStrengthViewModel vm, Material material, LengthBracingConditions bracingConditions)
        {
            var result = vm.GeometryVM.AsEntity()
                                        .Convert(vm.GeneralInfoVM.Unit, Units.KIPINCH)
                                        .AsUnStiffenedSection()
                                        .AsDSInteractionResistance(material, bracingConditions, vm.InputLoadVM.UltimateLoad.ConvertForce(vm.GeneralInfoVM.Unit, Units.KIPINCH).Item1, vm.InputLoadVM.UltimateMoment.ConvertMoment(vm.GeneralInfoVM.Unit, Units.KIPINCH).Item1)
                                        .Convert(Units.KIPINCH, vm.GeneralInfoVM.Unit);

            Func<IOutput> fun = () => result.AsCheck();

            return Tuple.Create(fun, result as IOutput);
        }



        private bool CanResults()
        {
            //TODO: add some kind of validation logic.
            return true;
        }

        private void OnReults()
        {
             _validateFuncs.SelectMany(f => f())
                           .AsUnitValidation()
                           .Match(errs=>ShowErrorsService(errs.ToList()),
                                 u=>
                                 {
                                     _moduleDict[GeneralInfoVM.RunningModule](this);
                                     return u;
                                 });
           
        }

        private static void Design(DirectStrengthViewModel vm)
        {
            vm.IsResistanceOutput = false;
            var material = (new Material(vm.MaterialVM.Fy, vm.MaterialVM.E, 0.3)).Convert(vm.GeneralInfoVM.Unit, Units.KIPINCH);
            var bracingConditions = vm.BracingConditionsVM.AsEntity().Convert(vm.GeneralInfoVM.Unit, Units.KIPINCH);
            var sa = vm.GeneralInfoVM.StrainingAction;
            var section = vm.GeneralInfoVM.SteelSection;
            vm.ResistanceOutput = vm.DesignDict[KeyValuePair.Create(sa, section)](vm, material, bracingConditions);
        }


        private DesignInteractionOutput DesignCLippedMomentComp(DirectStrengthViewModel vm, Material material, LengthBracingConditions bracingConditions)
        {
            (var secDto, var ressistance) = vm.GeometryVM.Sections.Select(dto => Tuple.Create(dto, dto.AsEntity().Convert(vm.GeometryVM.TableUnit, Units.KIPINCH).AsLippedSection().AsDSInteractionResistance(material, bracingConditions, vm.InputLoadVM.UltimateLoad.ConvertForce(vm.GeneralInfoVM.Unit, Units.KIPINCH).Item1, vm.InputLoadVM.UltimateMoment.ConvertMoment(vm.GeneralInfoVM.Unit, Units.KIPINCH).Item1).Convert(Units.KIPINCH, vm.GeneralInfoVM.Unit)))
                                                                    .Where(tuple => tuple.Item2.IEValue < 1)
                                                                     .OrderByDescending(tuple => tuple.Item2.IEValue)
                                                                      .FirstOrDefault();
            vm.IsResistanceOutput = true;
            if (secDto != null)
            {
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
            (var secDto, var ressistance) = vm.GeometryVM.Sections.Select(dto => Tuple.Create(dto, dto.AsEntity().Convert(vm.GeometryVM.TableUnit, Units.KIPINCH).AsUnStiffenedSection().AsDSInteractionResistance(material, bracingConditions, vm.InputLoadVM.UltimateLoad.ConvertForce(vm.GeneralInfoVM.Unit, Units.KIPINCH).Item1,vm.InputLoadVM.UltimateMoment.ConvertMoment(vm.GeneralInfoVM.Unit, Units.KIPINCH).Item1).Convert(Units.KIPINCH, vm.GeneralInfoVM.Unit)))
                                                                   .Where(tuple => tuple.Item2.IEValue < 1)
                                                                    .OrderByDescending(tuple => tuple.Item2.IEValue)
                                                                     .FirstOrDefault();
            vm.IsResistanceOutput = true;
            if (secDto != null)
            {
                vm.GeometryVM.SelectedSection = secDto;
                return ressistance.AsDesign( secDto.ID);
            }
            else
            {
                vm.GeometryVM.SelectedSection = null;
                return null;
            }
        }

        private static DesignOutput DesignCUnstiffComp(DirectStrengthViewModel vm, Material material, LengthBracingConditions bracingConditions)
        {
            (var secDto, var ressistance) = vm.GeometryVM.Sections.Select(dto => Tuple.Create(dto, dto.AsEntity().Convert(vm.GeometryVM.TableUnit, Units.KIPINCH).AsUnStiffenedSection().AsDSCompressionResistance(material, bracingConditions).Convert(Units.KIPINCH, vm.GeneralInfoVM.Unit)))
                                                                   .Where(tuple => tuple.Item2.DesignResistance > vm.InputLoadVM.UltimateLoad)
                                                                    .OrderBy(tuple => tuple.Item2.DesignResistance)
                                                                     .FirstOrDefault();
            vm.IsResistanceOutput = true;
            if (secDto != null)
            {
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
            (var secDto, var ressistance) = vm.GeometryVM.Sections.Select(dto => Tuple.Create(dto, dto.AsEntity().Convert(vm.GeometryVM.TableUnit, Units.KIPINCH).AsLippedSection().AsDSCompressionResistance(material, bracingConditions).Convert(Units.KIPINCH, vm.GeneralInfoVM.Unit)))
                                                                  .Where(tuple => tuple.Item2.DesignResistance > vm.InputLoadVM.UltimateLoad)
                                                                   .OrderBy(tuple => tuple.Item2.DesignResistance)
                                                                    .FirstOrDefault();
            vm.IsResistanceOutput = true;
            if (secDto != null)
            {
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
            (var secDto, var ressistance) = vm.GeometryVM.Sections.Select(dto => Tuple.Create(dto, dto.AsEntity().Convert(vm.GeometryVM.TableUnit, Units.KIPINCH).AsUnStiffenedSection().AsDSMomentResistance(material, bracingConditions).Convert(Units.KIPINCH, vm.GeneralInfoVM.Unit)))
                                                                  .Where(tuple => tuple.Item2.DesignResistance > vm.InputLoadVM.UltimateMoment)
                                                                   .OrderBy(tuple => tuple.Item2.DesignResistance)
                                                                    .FirstOrDefault();
            vm.IsResistanceOutput = true;
            if (secDto != null)
            {
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
            (var secDto, var ressistance) = vm.GeometryVM.Sections.Select(dto => Tuple.Create(dto, dto.AsEntity().Convert(vm.GeometryVM.TableUnit, Units.KIPINCH).AsLippedSection().AsDSMomentResistance(material, bracingConditions).Convert(Units.KIPINCH, vm.GeneralInfoVM.Unit)))
                                                                  .Where(tuple => tuple.Item2.DesignResistance > vm.InputLoadVM.UltimateMoment)
                                                                   .OrderBy(tuple => tuple.Item2.DesignResistance)
                                                                    .FirstOrDefault();
            vm.IsResistanceOutput = true;
            if (secDto != null)
            {
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
            var material = (new Material(vm.MaterialVM.Fy, vm.MaterialVM.E, 0.3)).Convert(vm.GeneralInfoVM.Unit, Units.KIPINCH);
            var bracingConditions = vm.BracingConditionsVM.AsEntity().Convert(vm.GeneralInfoVM.Unit, Units.KIPINCH);
            var sa = vm.GeneralInfoVM.StrainingAction;
            var section = vm.GeneralInfoVM.SteelSection;
            var resistOutput = vm.ResistDict[KeyValuePair.Create(sa, section)](vm, material, bracingConditions);
            var result = resistOutput.Item1();
            vm.ResistanceOutput = result;
        }

       
        private static void Resistance(DirectStrengthViewModel vm)
        {
            vm.IsResistanceOutput = false;
            var material = (new Material(vm.MaterialVM.Fy, vm.MaterialVM.E, vm.MaterialVM.V)).Convert(vm.GeneralInfoVM.Unit, Units.KIPINCH);
            var bracingConditions = vm.BracingConditionsVM.AsEntity().Convert(vm.GeneralInfoVM.Unit, Units.KIPINCH);
            var sa = vm.GeneralInfoVM.StrainingAction;
            var section = vm.GeneralInfoVM.SteelSection;
            var result = vm.ResistDict[KeyValuePair.Create(sa, section)](vm,material,bracingConditions);
            vm.ResistanceOutput = result.Item2;
        }

        private static Tuple<Func<IOutput> ,IOutput> ResistanceCUnstiffComp(DirectStrengthViewModel vm,Material material,LengthBracingConditions bracingConditions)
        {
           var result =  vm.GeometryVM.AsEntity()
                                      .Convert(vm.GeneralInfoVM.Unit, Units.KIPINCH)
                                      .AsUnStiffenedSection()
                                      .AsDSCompressionResistance(material, bracingConditions)
                                      .Convert(Units.KIPINCH, vm.GeneralInfoVM.Unit);

            Func< IOutput> fun = () => result.AsCheck(vm.InputLoadVM.UltimateLoad);
            
            return Tuple.Create(fun, result as IOutput);

        }
        private static Tuple<Func<IOutput>, IOutput> ResistanceCUnstiffMoment(DirectStrengthViewModel vm, Material material, LengthBracingConditions bracingConditions)
        {
            var result = vm.GeometryVM.AsEntity()
                                       .Convert(vm.GeneralInfoVM.Unit, Units.KIPINCH)
                                       .AsUnStiffenedSection()
                                       .AsDSMomentResistance(material, bracingConditions)
                                       .Convert(Units.KIPINCH, vm.GeneralInfoVM.Unit);
            Func<IOutput> fun = () => result.AsCheck(vm.InputLoadVM.UltimateMoment);

            return Tuple.Create(fun, result as IOutput);
        }
        private static Tuple<Func<IOutput>, IOutput> ResistanceCLippedComp(DirectStrengthViewModel vm, Material material, LengthBracingConditions bracingConditions)
        {
            var result = vm.GeometryVM.AsEntity()
                                      .Convert(vm.GeneralInfoVM.Unit, Units.KIPINCH)
                                      .AsLippedSection()
                                      .AsDSCompressionResistance(material, bracingConditions)
                                      .Convert(Units.KIPINCH, vm.GeneralInfoVM.Unit);

            Func<IOutput> fun = () => result.AsCheck(vm.InputLoadVM.UltimateLoad);

            return Tuple.Create(fun, result as IOutput);
        }
        private static Tuple<Func<IOutput>, IOutput> ResistanceCLippedMoment(DirectStrengthViewModel vm, Material material, LengthBracingConditions bracingConditions)
        {
            var result = vm.GeometryVM.AsEntity()
                                      .Convert(vm.GeneralInfoVM.Unit, Units.KIPINCH)
                                      .AsLippedSection()
                                      .AsDSMomentResistance(material, bracingConditions).Convert(Units.KIPINCH, vm.GeneralInfoVM.Unit);
            Func<IOutput> fun = () => result.AsCheck(vm.InputLoadVM.UltimateMoment);
            return Tuple.Create(fun, result as IOutput);
        }
        #endregion

    }
}
