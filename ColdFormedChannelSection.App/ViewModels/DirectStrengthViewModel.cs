﻿using ColdFormedChannelSection.App.Extensions;
using ColdFormedChannelSection.App.ViewModels.Base;
using ColdFormedChannelSection.App.ViewModels.Enums;
using ColdFormedChannelSection.Core.Entities;
using ColdFormedChannelSection.Core.Enums;
using ColdFormedChannelSection.Core.Extensions;
using ColdFormedChannelSection.Core.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Input;

namespace ColdFormedChannelSection.App.ViewModels
{
    public class DirectStrengthViewModel : ResistanceBaseViewModel
    {
        #region Properties

        public override ICommand ResultsCommand { get; }
        private readonly Dictionary<Module, Action<DirectStrengthViewModel>> _moduleDict;
        #endregion

        #region Properties

        public Dictionary<KeyValuePair<StrainingActions, SteelSection>,Func<DirectStrengthViewModel,Material,LengthBracingConditions, DesignOutput>> DesignDict { get; }

        #endregion

        public Dictionary<KeyValuePair<StrainingActions, SteelSection>, Func<DirectStrengthViewModel,Material,LengthBracingConditions, Tuple<Func<CheckOutput>, ResistanceOutput>>> ResistDict { get; }

        #region Constructors

        public DirectStrengthViewModel(GeneralInfoViewModel generalInfoVM, BracingConditionsViewModel bracingConditionsVM, GeometryViewModel geometryVM, MaterialViewModel materialVM, InputLoadViewModel inputLoadVM)
            : base(generalInfoVM, bracingConditionsVM, geometryVM, materialVM, inputLoadVM)
        {
            ResultsCommand = new RelayCommand(OnReults, CanResults);
            _moduleDict = new Dictionary<Module, Action<DirectStrengthViewModel>>()
            {
                [Module.RESISTANCE] = Resistance,
                [Module.DESIGN] = Design,
                [Module.CHECK] = Check
            };
            ResistDict = new Dictionary<KeyValuePair<StrainingActions, SteelSection>, Func<DirectStrengthViewModel,Material,LengthBracingConditions, Tuple<Func<CheckOutput>, ResistanceOutput>>>()
            {
                [KeyValuePair.Create(StrainingActions.COMPRESSION,SteelSection.C_UNSTIFFENED)]= ResistanceCUnstiffComp,
                [KeyValuePair.Create(StrainingActions.MOMENT,SteelSection.C_UNSTIFFENED)] = ResistanceCUnstiffMoment,
                [KeyValuePair.Create(StrainingActions.COMPRESSION,SteelSection.C_LIPPED)] = ResistanceCLippedComp,
                [KeyValuePair.Create(StrainingActions.MOMENT,SteelSection.C_LIPPED)]=ResistanceCLippedMoment
            };
            DesignDict = new Dictionary<KeyValuePair<StrainingActions, SteelSection>,Func<DirectStrengthViewModel,Material,LengthBracingConditions, DesignOutput>>()
            {
                [KeyValuePair.Create(StrainingActions.COMPRESSION, SteelSection.C_UNSTIFFENED)] = DesignCUnstiffComp,
                [KeyValuePair.Create(StrainingActions.MOMENT, SteelSection.C_UNSTIFFENED)] = DesignCUnstiffMoment,
                [KeyValuePair.Create(StrainingActions.COMPRESSION, SteelSection.C_LIPPED)] = DesignCLippedComp,
                [KeyValuePair.Create(StrainingActions.MOMENT, SteelSection.C_LIPPED)] = DesignCLippedMoment
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
            _moduleDict[GeneralInfoVM.RunningModule](this);
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

        private static DesignOutput DesignCUnstiffComp(DirectStrengthViewModel vm, Material material, LengthBracingConditions bracingConditions)
        {
            (var secDto, var ressistance) = vm.GeometryVM.Sections.Select(dto => Tuple.Create(dto, dto.AsEntity().Convert(vm.GeneralInfoVM.Unit, Units.KIPINCH).AsUnStiffenedSection().AsDSCompressionResistance(material, bracingConditions).Convert(Units.KIPINCH, vm.GeneralInfoVM.Unit)))
                                                                   .Where(tuple => tuple.Item2.DesignResistance > vm.UltimateLoad)
                                                                    .OrderBy(tuple => tuple.Item2.DesignResistance)
                                                                     .FirstOrDefault();
            vm.IsResistanceOutput = true;
            if (secDto != null)
            {
                vm.GeometryVM.SelectedSection = secDto;
                return ressistance.AsDesign(vm.UltimateLoad, secDto.ID);
            }
            else
            {
                vm.GeometryVM.SelectedSection = null;
                return null;
            }
        }

        private static DesignOutput DesignCLippedComp(DirectStrengthViewModel vm, Material material, LengthBracingConditions bracingConditions)
        {
            (var secDto, var ressistance) = vm.GeometryVM.Sections.Select(dto => Tuple.Create(dto, dto.AsEntity().Convert(vm.GeneralInfoVM.Unit, Units.KIPINCH).AsLippedSection().AsDSCompressionResistance(material, bracingConditions).Convert(Units.KIPINCH, vm.GeneralInfoVM.Unit)))
                                                                  .Where(tuple => tuple.Item2.DesignResistance > vm.UltimateLoad)
                                                                   .OrderBy(tuple => tuple.Item2.DesignResistance)
                                                                    .FirstOrDefault();
            vm.IsResistanceOutput = true;
            if (secDto != null)
            {
                vm.GeometryVM.SelectedSection = secDto;
                return ressistance.AsDesign(vm.UltimateLoad, secDto.ID);
            }
            else
            {
                vm.GeometryVM.SelectedSection = null;
                return null;
            }
        }


        private static DesignOutput DesignCUnstiffMoment(DirectStrengthViewModel vm, Material material, LengthBracingConditions bracingConditions)
        {
            (var secDto, var ressistance) = vm.GeometryVM.Sections.Select(dto => Tuple.Create(dto, dto.AsEntity().Convert(vm.GeneralInfoVM.Unit, Units.KIPINCH).AsUnStiffenedSection().AsDSMomentResistance(material, bracingConditions).Convert(Units.KIPINCH, vm.GeneralInfoVM.Unit)))
                                                                  .Where(tuple => tuple.Item2.DesignResistance > vm.UltimateMoment)
                                                                   .OrderBy(tuple => tuple.Item2.DesignResistance)
                                                                    .FirstOrDefault();
            vm.IsResistanceOutput = true;
            if (secDto != null)
            {
                vm.GeometryVM.SelectedSection = secDto;
                return ressistance.AsDesign(vm.UltimateLoad, secDto.ID);
            }
            else
            {
                vm.GeometryVM.SelectedSection = null;
                return null;
            }
        }

        private static DesignOutput DesignCLippedMoment(DirectStrengthViewModel vm, Material material, LengthBracingConditions bracingConditions)
        {
            (var secDto, var ressistance) = vm.GeometryVM.Sections.Select(dto => Tuple.Create(dto, dto.AsEntity().Convert(vm.GeneralInfoVM.Unit, Units.KIPINCH).AsLippedSection().AsDSMomentResistance(material, bracingConditions).Convert(Units.KIPINCH, vm.GeneralInfoVM.Unit)))
                                                                  .Where(tuple => tuple.Item2.DesignResistance > vm.UltimateMoment)
                                                                   .OrderBy(tuple => tuple.Item2.DesignResistance)
                                                                    .FirstOrDefault();
            vm.IsResistanceOutput = true;
            if (secDto != null)
            {
                vm.GeometryVM.SelectedSection = secDto;
                return ressistance.AsDesign(vm.UltimateLoad, secDto.ID);
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

        private static Tuple<Func<CheckOutput> ,ResistanceOutput> ResistanceCUnstiffComp(DirectStrengthViewModel vm,Material material,LengthBracingConditions bracingConditions)
        {
           var result =  vm.GeometryVM.AsEntity()
                                      .Convert(vm.GeneralInfoVM.Unit, Units.KIPINCH)
                                      .AsUnStiffenedSection()
                                      .AsDSCompressionResistance(material, bracingConditions)
                                      .Convert(Units.KIPINCH, vm.GeneralInfoVM.Unit);

            Func< CheckOutput> fun = () => result.AsCheck(vm.UltimateLoad);
            
            return Tuple.Create(fun, result as ResistanceOutput);

        }
        private static Tuple<Func<CheckOutput>, ResistanceOutput> ResistanceCUnstiffMoment(DirectStrengthViewModel vm, Material material, LengthBracingConditions bracingConditions)
        {
            var result = vm.GeometryVM.AsEntity()
                                       .Convert(vm.GeneralInfoVM.Unit, Units.KIPINCH)
                                       .AsUnStiffenedSection()
                                       .AsDSMomentResistance(material, bracingConditions)
                                       .Convert(Units.KIPINCH, vm.GeneralInfoVM.Unit);
            Func<CheckOutput> fun = () => result.AsCheck(vm.UltimateMoment);

            return Tuple.Create(fun, result as ResistanceOutput);
        }
        private static Tuple<Func<CheckOutput>, ResistanceOutput> ResistanceCLippedComp(DirectStrengthViewModel vm, Material material, LengthBracingConditions bracingConditions)
        {
            var result = vm.GeometryVM.AsEntity()
                                      .Convert(vm.GeneralInfoVM.Unit, Units.KIPINCH)
                                      .AsLippedSection()
                                      .AsDSCompressionResistance(material, bracingConditions)
                                      .Convert(Units.KIPINCH, vm.GeneralInfoVM.Unit);

            Func<CheckOutput> fun = () => result.AsCheck(vm.UltimateLoad);

            return Tuple.Create(fun, result as ResistanceOutput);
        }
        private static Tuple<Func<CheckOutput>, ResistanceOutput> ResistanceCLippedMoment(DirectStrengthViewModel vm, Material material, LengthBracingConditions bracingConditions)
        {
            var result = vm.GeometryVM.AsEntity()
                                      .Convert(vm.GeneralInfoVM.Unit, Units.KIPINCH)
                                      .AsLippedSection()
                                      .AsDSMomentResistance(material, bracingConditions).Convert(Units.KIPINCH, vm.GeneralInfoVM.Unit);
            Func<CheckOutput> fun = () => result.AsCheck(vm.UltimateMoment);
            return Tuple.Create(fun, result as ResistanceOutput);
        }
        #endregion

    }
}