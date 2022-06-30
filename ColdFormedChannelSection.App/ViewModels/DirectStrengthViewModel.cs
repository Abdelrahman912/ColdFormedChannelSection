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
using System.Windows;
using ColdFormedChannelSection.Core.Dtos;

namespace ColdFormedChannelSection.App.ViewModels
{
    public class DirectStrengthViewModel : ResistanceBaseViewModel
    {
        #region Private Fields


       

        private readonly Dictionary<Module, Func<DirectStrengthViewModel,Validation<Unit>>> _moduleDict;

        private List<Func<List<Error>>> _validateFuncs;
        #endregion

        #region Properties
        public override ICommand ResultsCommand { get; }

        public Dictionary<KeyValuePair<StrainingActions, SteelSection>, Func<DirectStrengthViewModel, Material, LengthBracingConditions, OutputBase>> DesignDict { get; }


        public Dictionary<KeyValuePair<StrainingActions, SteelSection>, Func<DirectStrengthViewModel, Material, LengthBracingConditions, Validation<Tuple<Func<OutputBase>, OutputBase>>>> ResistDict { get; }

        #endregion

        #region Constructors

        public DirectStrengthViewModel(GeneralInfoViewModel generalInfoVM, BracingConditionsViewModel bracingConditionsVM, GeometryViewModel geometryVM, MaterialViewModel materialVM, InputLoadViewModel inputLoadVM, Func<List<Error>, Unit> showErrorsService, Action<ReportViewModel> reportService, Func<Func<string, Exceptional<Unit>>, Option<Exceptional<Unit>>> folderDialogService)
            : base(generalInfoVM, bracingConditionsVM, geometryVM, materialVM, inputLoadVM, showErrorsService, reportService, folderDialogService)
        {
            ResultsCommand = new RelayCommand(OnReults, CanResults);
            _moduleDict = new Dictionary<Module, Func<DirectStrengthViewModel,Validation<Unit>>>()
            {
                [Module.RESISTANCE] = Resistance,
                [Module.DESIGN] = Design,
                [Module.CHECK] = Check
            };
            ResistDict = new Dictionary<KeyValuePair<StrainingActions, SteelSection>, Func<DirectStrengthViewModel, Material, LengthBracingConditions, Validation<Tuple<Func<OutputBase>, OutputBase>>>>()
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




        private Validation<Tuple<Func<OutputBase>, OutputBase>> ResistanceCLippedMomentComp(DirectStrengthViewModel vm, Material material, LengthBracingConditions bracingConditions)
        {
            var result = vm.GeometryVM.AsEntity()
                                       .Convert(vm.GeneralInfoVM.Unit, UnitSystems.KIPINCH)
                                       .AsLippedSection()
                                       .Map(c=>
                                        c.AsDSInteractionResistance(material, bracingConditions, vm.InputLoadVM.UltimateLoad.ConvertForce(vm.GeneralInfoVM.Unit, UnitSystems.KIPINCH).Item1, vm.InputLoadVM.UltimateMoment.ConvertMoment(vm.GeneralInfoVM.Unit, UnitSystems.KIPINCH).Item1)
                                       .Convert(UnitSystems.KIPINCH, vm.GeneralInfoVM.Unit))
                                       .Map(val =>
                                       {
                                           Func<OutputBase> fun = () => val.AsCheck(_momentDict[vm.GeneralInfoVM.Unit], _compDict[vm.GeneralInfoVM.Unit]);

                                           return Tuple.Create(fun, val as OutputBase);
                                       });
            return result;
           
        }

        private Validation<Tuple<Func<OutputBase>, OutputBase>> ResistanceCUnstiffMomentComp(DirectStrengthViewModel vm, Material material, LengthBracingConditions bracingConditions)
        {
            var result = vm.GeometryVM.AsEntity()
                                        .Convert(vm.GeneralInfoVM.Unit, UnitSystems.KIPINCH)
                                        .AsUnStiffenedSection()
                                        .Map(c=>
                                         c.AsDSInteractionResistance(material, bracingConditions, vm.InputLoadVM.UltimateLoad.ConvertForce(vm.GeneralInfoVM.Unit, UnitSystems.KIPINCH).Item1, vm.InputLoadVM.UltimateMoment.ConvertMoment(vm.GeneralInfoVM.Unit, UnitSystems.KIPINCH).Item1)
                                        .Convert(UnitSystems.KIPINCH, vm.GeneralInfoVM.Unit))
                                        .Map(val =>
                                        {
                                            Func<OutputBase> fun = () => val.AsCheck(_momentDict[vm.GeneralInfoVM.Unit], _compDict[vm.GeneralInfoVM.Unit]);

                                            return Tuple.Create(fun, val as OutputBase);
                                        });

            return result;
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
               return _moduleDict[GeneralInfoVM.RunningModule](this).Bind<Unit,Unit>(val =>
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

        private static Validation<Unit> Design(DirectStrengthViewModel vm)
        {
            vm.IsResistanceOutput = false;
            var material = (new Material(vm.MaterialVM.Fy, vm.MaterialVM.E, 0.3)).Convert(vm.GeneralInfoVM.Unit, UnitSystems.KIPINCH);
            var bracingConditions = vm.BracingConditionsVM.AsEntity().Convert(vm.GeneralInfoVM.Unit, UnitSystems.KIPINCH);
            var sa = vm.GeneralInfoVM.StrainingAction;
            var section = vm.GeneralInfoVM.SteelSection;
            vm.ResistanceOutput = vm.DesignDict[KeyValuePair.Create(sa, section)](vm, material, bracingConditions);
            return Unit();
        }


        private DesignInteractionOutput DesignCLippedMomentComp(DirectStrengthViewModel vm, Material material, LengthBracingConditions bracingConditions)
        {
            var outTuple = vm.GeometryVM.Sections.Select(dto => Tuple.Create(dto, dto.AsEntity().Convert(vm.GeometryVM.TableUnit, UnitSystems.KIPINCH).AsLippedSection().Map(e=>e.AsDSInteractionResistance(material, bracingConditions, vm.InputLoadVM.UltimateLoad.ConvertForce(vm.GeneralInfoVM.Unit, UnitSystems.KIPINCH).Item1, vm.InputLoadVM.UltimateMoment.ConvertMoment(vm.GeneralInfoVM.Unit, UnitSystems.KIPINCH).Item1).Convert(UnitSystems.KIPINCH, vm.GeneralInfoVM.Unit))))
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

        private DesignInteractionOutput DesignCUnstiffMomentComp(DirectStrengthViewModel vm, Material material, LengthBracingConditions bracingConditions)
        {

            var outTuple = vm.GeometryVM.Sections.Select(dto => Tuple.Create(dto, dto.AsEntity().Convert(vm.GeometryVM.TableUnit, UnitSystems.KIPINCH).AsUnStiffenedSection().Map(c=>c.AsDSInteractionResistance(material, bracingConditions, vm.InputLoadVM.UltimateLoad.ConvertForce(vm.GeneralInfoVM.Unit, UnitSystems.KIPINCH).Item1, vm.InputLoadVM.UltimateMoment.ConvertMoment(vm.GeneralInfoVM.Unit, UnitSystems.KIPINCH).Item1).Convert(UnitSystems.KIPINCH, vm.GeneralInfoVM.Unit))))
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

        private static DesignOutput DesignCUnstiffComp(DirectStrengthViewModel vm, Material material, LengthBracingConditions bracingConditions)
        {
            var outTuple  = vm.GeometryVM.Sections.Select(dto => Tuple.Create(dto, dto.AsEntity().Convert(vm.GeometryVM.TableUnit, UnitSystems.KIPINCH).AsUnStiffenedSection().Map(c=>c.AsDSCompressionResistance(material, bracingConditions).Convert(UnitSystems.KIPINCH, vm.GeneralInfoVM.Unit))))
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

        private static DesignOutput DesignCLippedComp(DirectStrengthViewModel vm, Material material, LengthBracingConditions bracingConditions)
        {
            var outTuple = vm.GeometryVM.Sections.Select(dto => Tuple.Create(dto, dto.AsEntity().Convert(vm.GeometryVM.TableUnit, UnitSystems.KIPINCH).AsLippedSection().Map(c=>c.AsDSCompressionResistance(material, bracingConditions).Convert(UnitSystems.KIPINCH, vm.GeneralInfoVM.Unit))))
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


        private static DesignOutput DesignCUnstiffMoment(DirectStrengthViewModel vm, Material material, LengthBracingConditions bracingConditions)
        {
            var outTuple = vm.GeometryVM.Sections.Select(dto => Tuple.Create(dto, dto.AsEntity().Convert(vm.GeometryVM.TableUnit, UnitSystems.KIPINCH).AsUnStiffenedSection().Map(c=>c.AsDSMomentResistance(material, bracingConditions).Convert(UnitSystems.KIPINCH, vm.GeneralInfoVM.Unit))))
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

        private static DesignOutput DesignCLippedMoment(DirectStrengthViewModel vm, Material material, LengthBracingConditions bracingConditions)
        {
            var outTuple = vm.GeometryVM.Sections.Select(dto => Tuple.Create(dto, dto.AsEntity().Convert(vm.GeometryVM.TableUnit, UnitSystems.KIPINCH).AsLippedSection().Map(c=>c.AsDSMomentResistance(material, bracingConditions).Convert(UnitSystems.KIPINCH, vm.GeneralInfoVM.Unit))))
                                                                     .Aggregate(new List<Tuple<SectionDimensionDto, MomentResistanceOutput >>(), (soFar, current) =>
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

        private static Validation<Unit> Check(DirectStrengthViewModel vm)
        {
            vm.IsResistanceOutput = false;
            var material = (new Material(vm.MaterialVM.Fy, vm.MaterialVM.E, 0.3)).Convert(vm.GeneralInfoVM.Unit, UnitSystems.KIPINCH);
            var bracingConditions = vm.BracingConditionsVM.AsEntity().Convert(vm.GeneralInfoVM.Unit, UnitSystems.KIPINCH);
            var sa = vm.GeneralInfoVM.StrainingAction;
            var section = vm.GeneralInfoVM.SteelSection;
            var resistOutput = vm.ResistDict[KeyValuePair.Create(sa, section)](vm, material, bracingConditions);
            return  resistOutput.Map(c =>
            {
                var result =  c.Item1();
                vm.ResistanceOutput = result;
                return Unit();
            });
        }


        private static Validation<Unit> Resistance(DirectStrengthViewModel vm)
        {
            vm.IsResistanceOutput = false;
            var material = (new Material(vm.MaterialVM.Fy, vm.MaterialVM.E, vm.MaterialVM.V)).Convert(vm.GeneralInfoVM.Unit, UnitSystems.KIPINCH);
            var bracingConditions = vm.BracingConditionsVM.AsEntity().Convert(vm.GeneralInfoVM.Unit, UnitSystems.KIPINCH);
            var sa = vm.GeneralInfoVM.StrainingAction;
            var section = vm.GeneralInfoVM.SteelSection;
            var result = vm.ResistDict[KeyValuePair.Create(sa, section)](vm, material, bracingConditions);
            return result.Map(c =>
            {
                var res = c.Item2;
                vm.ResistanceOutput = res;
                return Unit();
            });
        }

        private static Validation<Tuple<Func<OutputBase>, OutputBase>> ResistanceCUnstiffComp(DirectStrengthViewModel vm, Material material, LengthBracingConditions bracingConditions)
        {
            var result = vm.GeometryVM.AsEntity()
                                       .Convert(vm.GeneralInfoVM.Unit, UnitSystems.KIPINCH)
                                       .AsUnStiffenedSection()
                                       .Map(c=>
                                       c.AsDSCompressionResistance(material, bracingConditions)
                                       .Convert(UnitSystems.KIPINCH, vm.GeneralInfoVM.Unit))
                                       .Map(val =>
                                       {
                                           Func<OutputBase> fun = () => val.AsCheck(vm.InputLoadVM.UltimateLoad, _compDict[vm.GeneralInfoVM.Unit]);

                                           return Tuple.Create(fun, val as OutputBase);
                                       });
            return result;
        }
        private static Validation<Tuple<Func<OutputBase>, OutputBase>> ResistanceCUnstiffMoment(DirectStrengthViewModel vm, Material material, LengthBracingConditions bracingConditions)
        {
            var result = vm.GeometryVM.AsEntity()
                                       .Convert(vm.GeneralInfoVM.Unit, UnitSystems.KIPINCH)
                                       .AsUnStiffenedSection()
                                       .Map(c=>
                                        c.AsDSMomentResistance(material, bracingConditions)
                                       .Convert(UnitSystems.KIPINCH, vm.GeneralInfoVM.Unit))
                                       .Map(val =>
                                       {
                                           Func<OutputBase> fun = () => val.AsCheck(vm.InputLoadVM.UltimateMoment, _momentDict[vm.GeneralInfoVM.Unit]);

                                           return Tuple.Create(fun, val as OutputBase);
                                       });
            return result;
        }
        private static Validation<Tuple<Func<OutputBase>, OutputBase>> ResistanceCLippedComp(DirectStrengthViewModel vm, Material material, LengthBracingConditions bracingConditions)
        {
            var result = vm.GeometryVM.AsEntity()
                                      .Convert(vm.GeneralInfoVM.Unit, UnitSystems.KIPINCH)
                                      .AsLippedSection()
                                      .Map(c=>
                                       c.AsDSCompressionResistance(material, bracingConditions)
                                      .Convert(UnitSystems.KIPINCH, vm.GeneralInfoVM.Unit))
                                      .Map(val =>
                                      {
                                          Func<OutputBase> fun = () => val.AsCheck(vm.InputLoadVM.UltimateLoad, _compDict[vm.GeneralInfoVM.Unit]);

                                          return Tuple.Create(fun, val as OutputBase);
                                      });
            return result;
          
        }
        private static Validation<Tuple<Func<OutputBase>, OutputBase>> ResistanceCLippedMoment(DirectStrengthViewModel vm, Material material, LengthBracingConditions bracingConditions)
        {
            var result = vm.GeometryVM.AsEntity()
                                      .Convert(vm.GeneralInfoVM.Unit, UnitSystems.KIPINCH)
                                      .AsLippedSection()
                                      .Map(c=>
                                       c.AsDSMomentResistance(material, bracingConditions).Convert(UnitSystems.KIPINCH, vm.GeneralInfoVM.Unit))
                                      .Map(val =>
                                      {
                                          Func<OutputBase> fun = () => val.AsCheck(vm.InputLoadVM.UltimateMoment, _momentDict[vm.GeneralInfoVM.Unit]);
                                          return Tuple.Create(fun, val as OutputBase);
                                      });
            return result;
        }
        #endregion

    }
}
