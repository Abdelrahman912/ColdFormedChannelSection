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
    public class EffectiveWidthViewModel : ResistanceBaseViewModel
    {

        #region Private Fields

        public override ICommand ResultsCommand { get; }

        private readonly Dictionary<Module, Action<EffectiveWidthViewModel>> _moduleDict;

        #endregion

        #region Properties

        public Dictionary<DesignCode, Dictionary<KeyValuePair<StrainingActions, SteelSection>, Func<EffectiveWidthViewModel, Material, LengthBracingConditions, DesignOutput>>> DesignDict { get; }

        public Dictionary<KeyValuePair<StrainingActions, SteelSection>, Func<EffectiveWidthViewModel, Material, LengthBracingConditions, DesignOutput>> DesignEgyptDict { get; }
        public Dictionary<KeyValuePair<StrainingActions, SteelSection>, Func<EffectiveWidthViewModel, Material, LengthBracingConditions, DesignOutput>> DesignEuroDict { get; }
        public Dictionary<KeyValuePair<StrainingActions, SteelSection>, Func<EffectiveWidthViewModel, Material, LengthBracingConditions, DesignOutput>> DesignAISIDict { get; }


        public Dictionary<DesignCode, Dictionary<KeyValuePair<StrainingActions, SteelSection>, Func<EffectiveWidthViewModel, Material, LengthBracingConditions, Tuple<Func<CheckOutput>, ResistanceOutput>>>> ResistDict { get; }

        public Dictionary<KeyValuePair<StrainingActions, SteelSection>, Func<EffectiveWidthViewModel, Material, LengthBracingConditions, Tuple<Func<CheckOutput>, ResistanceOutput>>> ResistEgyptDict { get; }
        public Dictionary<KeyValuePair<StrainingActions, SteelSection>, Func<EffectiveWidthViewModel, Material, LengthBracingConditions, Tuple<Func<CheckOutput>, ResistanceOutput>>> ResistEuroDict { get; }
        public Dictionary<KeyValuePair<StrainingActions, SteelSection>, Func<EffectiveWidthViewModel, Material, LengthBracingConditions, Tuple<Func<CheckOutput>, ResistanceOutput>>> ResistAISIDict { get; }


        #endregion

        #region Constructors

        public EffectiveWidthViewModel(GeneralInfoViewModel generalInfoVM, BracingConditionsViewModel bracingConditionsVM, GeometryViewModel geometryVM, MaterialViewModel materialVM, InputLoadViewModel inputLoadVM)
          : base(generalInfoVM, bracingConditionsVM, geometryVM, materialVM, inputLoadVM)
        {
            ResultsCommand = new RelayCommand(OnReults, CanResults);
            _moduleDict = new Dictionary<Module, Action<EffectiveWidthViewModel>>()
            {
                [Module.RESISTANCE] = Resistance,
                [Module.DESIGN] = Design,
                [Module.CHECK] = Check
            };
            ResistEgyptDict = new Dictionary<KeyValuePair<StrainingActions, SteelSection>, Func<EffectiveWidthViewModel, Material, LengthBracingConditions, Tuple<Func<CheckOutput>, ResistanceOutput>>>()
            {
                [KeyValuePair.Create(StrainingActions.COMPRESSION, SteelSection.C_UNSTIFFENED)] = EgyptResistanceCUnstiffComp,
                [KeyValuePair.Create(StrainingActions.MOMENT, SteelSection.C_UNSTIFFENED)] = EgyptResistanceCUnstiffMoment,
                [KeyValuePair.Create(StrainingActions.COMPRESSION, SteelSection.C_LIPPED)] = EgyptResistanceCLippedComp,
                [KeyValuePair.Create(StrainingActions.MOMENT, SteelSection.C_LIPPED)] = EgyptResistanceCLippedMoment
            };

            ResistEuroDict = new Dictionary<KeyValuePair<StrainingActions, SteelSection>, Func<EffectiveWidthViewModel, Material, LengthBracingConditions, Tuple<Func<CheckOutput>, ResistanceOutput>>>()
            {
                [KeyValuePair.Create(StrainingActions.COMPRESSION, SteelSection.C_UNSTIFFENED)] = EuroResistanceCUnstiffComp,
                [KeyValuePair.Create(StrainingActions.MOMENT, SteelSection.C_UNSTIFFENED)] = EuroResistanceCUnstiffMoment,
                [KeyValuePair.Create(StrainingActions.COMPRESSION, SteelSection.C_LIPPED)] = EuroResistanceCLippedComp,
                [KeyValuePair.Create(StrainingActions.MOMENT, SteelSection.C_LIPPED)] = EuroResistanceCLippedMoment
            };

            ResistAISIDict = new Dictionary<KeyValuePair<StrainingActions, SteelSection>, Func<EffectiveWidthViewModel, Material, LengthBracingConditions, Tuple<Func<CheckOutput>, ResistanceOutput>>>()
            {
                [KeyValuePair.Create(StrainingActions.COMPRESSION, SteelSection.C_UNSTIFFENED)] = AISIResistanceCUnstiffComp,
                [KeyValuePair.Create(StrainingActions.MOMENT, SteelSection.C_UNSTIFFENED)] = AISIResistanceCUnstiffMoment,
                [KeyValuePair.Create(StrainingActions.COMPRESSION, SteelSection.C_LIPPED)] = AISIResistanceCLippedComp,
                [KeyValuePair.Create(StrainingActions.MOMENT, SteelSection.C_LIPPED)] = AISIResistanceCLippedMoment
            };
            ResistDict = new Dictionary<DesignCode, Dictionary<KeyValuePair<StrainingActions, SteelSection>, Func<EffectiveWidthViewModel, Material, LengthBracingConditions, Tuple<Func<CheckOutput>, ResistanceOutput>>>>()
            {
                [DesignCode.EGYPTIAN] = ResistEgyptDict,
                [DesignCode.EURO] = ResistEuroDict,
                [DesignCode.AISI] = ResistAISIDict
            };
            DesignEgyptDict = new Dictionary<KeyValuePair<StrainingActions, SteelSection>, Func<EffectiveWidthViewModel, Material, LengthBracingConditions, DesignOutput>>()
            {
                [KeyValuePair.Create(StrainingActions.COMPRESSION, SteelSection.C_UNSTIFFENED)] = EgyptDesignCUnstiffComp,
                [KeyValuePair.Create(StrainingActions.MOMENT, SteelSection.C_UNSTIFFENED)] = EgyptDesignCUnstiffMoment,
                [KeyValuePair.Create(StrainingActions.COMPRESSION, SteelSection.C_LIPPED)] = EgyptDesignCLippedComp,
                [KeyValuePair.Create(StrainingActions.MOMENT, SteelSection.C_LIPPED)] = EgyptDesignCLippedMoment
            };
            DesignEuroDict = new Dictionary<KeyValuePair<StrainingActions, SteelSection>, Func<EffectiveWidthViewModel, Material, LengthBracingConditions, DesignOutput>>()
            {
                [KeyValuePair.Create(StrainingActions.COMPRESSION, SteelSection.C_UNSTIFFENED)] = EuroDesignCUnstiffComp,
                [KeyValuePair.Create(StrainingActions.MOMENT, SteelSection.C_UNSTIFFENED)] = EuroDesignCUnstiffMoment,
                [KeyValuePair.Create(StrainingActions.COMPRESSION, SteelSection.C_LIPPED)] = EuroDesignCLippedComp,
                [KeyValuePair.Create(StrainingActions.MOMENT, SteelSection.C_LIPPED)] = EuroDesignCLippedMoment
            };
            DesignAISIDict = new Dictionary<KeyValuePair<StrainingActions, SteelSection>, Func<EffectiveWidthViewModel, Material, LengthBracingConditions, DesignOutput>>()
            {
                [KeyValuePair.Create(StrainingActions.COMPRESSION, SteelSection.C_UNSTIFFENED)] = AISIDesignCUnstiffComp,
                [KeyValuePair.Create(StrainingActions.MOMENT, SteelSection.C_UNSTIFFENED)] = AISIDesignCUnstiffMoment,
                [KeyValuePair.Create(StrainingActions.COMPRESSION, SteelSection.C_LIPPED)] = AISIDesignCLippedComp,
                [KeyValuePair.Create(StrainingActions.MOMENT, SteelSection.C_LIPPED)] = AISIDesignCLippedMoment
            };
            DesignDict = new Dictionary<DesignCode, Dictionary<KeyValuePair<StrainingActions, SteelSection>, Func<EffectiveWidthViewModel, Material, LengthBracingConditions, DesignOutput>>>()
            {
                [DesignCode.EGYPTIAN] = DesignEgyptDict,
                [DesignCode.EURO] = DesignEuroDict,
                [DesignCode.AISI] = DesignAISIDict
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

        private static void Design(EffectiveWidthViewModel vm)
        {
            vm.IsResistanceOutput = false;
            var material = (new Material(vm.MaterialVM.Fy, vm.MaterialVM.E, 0.3)).Convert(vm.GeneralInfoVM.Unit, Units.KIPINCH);
            var bracingConditions = vm.BracingConditionsVM.AsEntity().Convert(vm.GeneralInfoVM.Unit, Units.KIPINCH);
            var sa = vm.GeneralInfoVM.StrainingAction;
            var section = vm.GeneralInfoVM.SteelSection;
            vm.ResistanceOutput = vm.DesignDict[vm.GeneralInfoVM.DesignCode][KeyValuePair.Create(sa, section)](vm, material, bracingConditions);
        }

        private static DesignOutput EgyptDesignCUnstiffComp(EffectiveWidthViewModel vm, Material material, LengthBracingConditions bracingConditions)
        {
            (var secDto, var ressistance) = vm.GeometryVM.Sections.Select(dto => Tuple.Create(dto, dto.AsEntity().Convert(vm.GeneralInfoVM.Unit, Units.TONCM).AsUnStiffenedSection().AsEgyptCompressionResistance(material, bracingConditions).Convert(Units.TONCM, vm.GeneralInfoVM.Unit)))
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

        private static DesignOutput EgyptDesignCLippedComp(EffectiveWidthViewModel vm, Material material, LengthBracingConditions bracingConditions)
        {
            (var secDto, var ressistance) = vm.GeometryVM.Sections.Select(dto => Tuple.Create(dto, dto.AsEntity().Convert(vm.GeneralInfoVM.Unit, Units.TONCM).AsLippedSection().AsEgyptCompressionResistance(material, bracingConditions).Convert(Units.TONCM, vm.GeneralInfoVM.Unit)))
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

        private static DesignOutput EgyptDesignCUnstiffMoment(EffectiveWidthViewModel vm, Material material, LengthBracingConditions bracingConditions)
        {
            (var secDto, var ressistance) = vm.GeometryVM.Sections.Select(dto => Tuple.Create(dto, dto.AsEntity().Convert(vm.GeneralInfoVM.Unit, Units.TONCM).AsUnStiffenedSection().AsEgyptMomentResistance(material, bracingConditions).Convert(Units.TONCM, vm.GeneralInfoVM.Unit)))
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

        private static DesignOutput EgyptDesignCLippedMoment(EffectiveWidthViewModel vm, Material material, LengthBracingConditions bracingConditions)
        {
            (var secDto, var ressistance) = vm.GeometryVM.Sections.Select(dto => Tuple.Create(dto, dto.AsEntity().Convert(vm.GeneralInfoVM.Unit, Units.TONCM).AsLippedSection().AsEgyptMomentResistance(material, bracingConditions).Convert(Units.TONCM, vm.GeneralInfoVM.Unit)))
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



        private static DesignOutput EuroDesignCUnstiffComp(EffectiveWidthViewModel vm, Material material, LengthBracingConditions bracingConditions)
        {
            (var secDto, var ressistance) = vm.GeometryVM.Sections.Select(dto => Tuple.Create(dto, dto.AsEntity().Convert(vm.GeneralInfoVM.Unit, Units.NMM).AsUnStiffenedSection().AsEuroCompressionResistance(material, bracingConditions).Convert(Units.NMM, vm.GeneralInfoVM.Unit)))
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

        private static DesignOutput EuroDesignCLippedComp(EffectiveWidthViewModel vm, Material material, LengthBracingConditions bracingConditions)
        {
            (var secDto, var ressistance) = vm.GeometryVM.Sections.Select(dto => Tuple.Create(dto, dto.AsEntity().Convert(vm.GeneralInfoVM.Unit, Units.NMM).AsLippedSection().AsEuroCompressionResistance(material, bracingConditions).Convert(Units.NMM, vm.GeneralInfoVM.Unit)))
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

        private static DesignOutput EuroDesignCUnstiffMoment(EffectiveWidthViewModel vm, Material material, LengthBracingConditions bracingConditions)
        {
            (var secDto, var ressistance) = vm.GeometryVM.Sections.Select(dto => Tuple.Create(dto, dto.AsEntity().Convert(vm.GeneralInfoVM.Unit, Units.NMM).AsUnStiffenedSection().AsEuroMomentResistance(material, bracingConditions).Convert(Units.NMM, vm.GeneralInfoVM.Unit)))
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

        private static DesignOutput EuroDesignCLippedMoment(EffectiveWidthViewModel vm, Material material, LengthBracingConditions bracingConditions)
        {
            (var secDto, var ressistance) = vm.GeometryVM.Sections.Select(dto => Tuple.Create(dto, dto.AsEntity().Convert(vm.GeneralInfoVM.Unit, Units.NMM).AsLippedSection().AsEuroMomentResistance(material, bracingConditions).Convert(Units.NMM, vm.GeneralInfoVM.Unit)))
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


        private static DesignOutput AISIDesignCUnstiffComp(EffectiveWidthViewModel vm, Material material, LengthBracingConditions bracingConditions)
        {
            (var secDto, var ressistance) = vm.GeometryVM.Sections.Select(dto => Tuple.Create(dto, dto.AsEntity().Convert(vm.GeneralInfoVM.Unit, Units.KIPINCH).AsUnStiffenedSection().AsAISICompressionResistance(material, bracingConditions).Convert(Units.KIPINCH, vm.GeneralInfoVM.Unit)))
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

        private static DesignOutput AISIDesignCLippedComp(EffectiveWidthViewModel vm, Material material, LengthBracingConditions bracingConditions)
        {
            (var secDto, var ressistance) = vm.GeometryVM.Sections.Select(dto => Tuple.Create(dto, dto.AsEntity().Convert(vm.GeneralInfoVM.Unit, Units.KIPINCH).AsLippedSection().AsAISICompressionResistance(material, bracingConditions).Convert(Units.KIPINCH, vm.GeneralInfoVM.Unit)))
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

        private static DesignOutput AISIDesignCUnstiffMoment(EffectiveWidthViewModel vm, Material material, LengthBracingConditions bracingConditions)
        {
            (var secDto, var ressistance) = vm.GeometryVM.Sections.Select(dto => Tuple.Create(dto, dto.AsEntity().Convert(vm.GeneralInfoVM.Unit, Units.KIPINCH).AsUnStiffenedSection().AsAISIMomentResistance(material, bracingConditions).Convert(Units.KIPINCH, vm.GeneralInfoVM.Unit)))
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

        private static DesignOutput AISIDesignCLippedMoment(EffectiveWidthViewModel vm, Material material, LengthBracingConditions bracingConditions)
        {
            (var secDto, var ressistance) = vm.GeometryVM.Sections.Select(dto => Tuple.Create(dto, dto.AsEntity().Convert(vm.GeneralInfoVM.Unit, Units.KIPINCH).AsLippedSection().AsAISIMomentResistance(material, bracingConditions).Convert(Units.KIPINCH, vm.GeneralInfoVM.Unit)))
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

        private static void Check(EffectiveWidthViewModel vm)
        {
            vm.IsResistanceOutput = false;
            var material = (new Material(vm.MaterialVM.Fy, vm.MaterialVM.E, 0.3)).Convert(vm.GeneralInfoVM.Unit, Units.KIPINCH);
            var bracingConditions = vm.BracingConditionsVM.AsEntity().Convert(vm.GeneralInfoVM.Unit, Units.KIPINCH);
            var sa = vm.GeneralInfoVM.StrainingAction;
            var section = vm.GeneralInfoVM.SteelSection;
            var resistOutput = vm.ResistDict[vm.GeneralInfoVM.DesignCode][KeyValuePair.Create(sa, section)](vm, material, bracingConditions);
            var result = resistOutput.Item1();
            vm.ResistanceOutput = result;

        }

        private static void Resistance(EffectiveWidthViewModel vm)
        {
            vm.IsResistanceOutput = false;
            var material = (new Material(vm.MaterialVM.Fy, vm.MaterialVM.E, vm.MaterialVM.V)).Convert(vm.GeneralInfoVM.Unit, Units.KIPINCH);
            var bracingConditions = vm.BracingConditionsVM.AsEntity().Convert(vm.GeneralInfoVM.Unit, Units.KIPINCH);
            var sa = vm.GeneralInfoVM.StrainingAction;
            var section = vm.GeneralInfoVM.SteelSection;
            var result = vm.ResistDict[vm.GeneralInfoVM.DesignCode][KeyValuePair.Create(sa, section)](vm, material, bracingConditions);
            vm.ResistanceOutput = result.Item2;
        }


        private static Tuple<Func<CheckOutput>, ResistanceOutput> EgyptResistanceCUnstiffComp(EffectiveWidthViewModel vm, Material material, LengthBracingConditions bracingConditions)
        {
            var result = vm.GeometryVM.AsEntity()
                                       .Convert(vm.GeneralInfoVM.Unit, Units.TONCM)
                                       .AsUnStiffenedSection()
                                       .AsEgyptCompressionResistance(material, bracingConditions)
                                       .Convert(Units.TONCM, vm.GeneralInfoVM.Unit);

            Func<CheckOutput> fun = () => result.AsCheck(vm.InputLoadVM.UltimateLoad);

            return Tuple.Create(fun, result as ResistanceOutput);

        }
        private static Tuple<Func<CheckOutput>, ResistanceOutput> EgyptResistanceCUnstiffMoment(EffectiveWidthViewModel vm, Material material, LengthBracingConditions bracingConditions)
        {
            var result = vm.GeometryVM.AsEntity()
                                       .Convert(vm.GeneralInfoVM.Unit, Units.TONCM)
                                       .AsUnStiffenedSection()
                                       .AsEgyptMomentResistance(material, bracingConditions)
                                       .Convert(Units.TONCM, vm.GeneralInfoVM.Unit);
            Func<CheckOutput> fun = () => result.AsCheck(vm.InputLoadVM.UltimateMoment);

            return Tuple.Create(fun, result as ResistanceOutput);
        }
        private static Tuple<Func<CheckOutput>, ResistanceOutput> EgyptResistanceCLippedComp(EffectiveWidthViewModel vm, Material material, LengthBracingConditions bracingConditions)
        {
            var result = vm.GeometryVM.AsEntity()
                                      .Convert(vm.GeneralInfoVM.Unit, Units.TONCM)
                                      .AsLippedSection()
                                      .AsEgyptCompressionResistance(material, bracingConditions)
                                      .Convert(Units.TONCM, vm.GeneralInfoVM.Unit);

            Func<CheckOutput> fun = () => result.AsCheck(vm.InputLoadVM.UltimateLoad);

            return Tuple.Create(fun, result as ResistanceOutput);
        }
        private static Tuple<Func<CheckOutput>, ResistanceOutput> EgyptResistanceCLippedMoment(EffectiveWidthViewModel vm, Material material, LengthBracingConditions bracingConditions)
        {
            var result = vm.GeometryVM.AsEntity()
                                      .Convert(vm.GeneralInfoVM.Unit, Units.TONCM)
                                      .AsLippedSection()
                                      .AsEgyptMomentResistance(material, bracingConditions).Convert(Units.TONCM, vm.GeneralInfoVM.Unit);
            Func<CheckOutput> fun = () => result.AsCheck(vm.InputLoadVM.UltimateMoment);
            return Tuple.Create(fun, result as ResistanceOutput);
        }


        private static Tuple<Func<CheckOutput>, ResistanceOutput> EuroResistanceCUnstiffComp(EffectiveWidthViewModel vm, Material material, LengthBracingConditions bracingConditions)
        {
            var result = vm.GeometryVM.AsEntity()
                                       .Convert(vm.GeneralInfoVM.Unit, Units.NMM)
                                       .AsUnStiffenedSection()
                                       .AsEuroCompressionResistance(material, bracingConditions)
                                       .Convert(Units.NMM, vm.GeneralInfoVM.Unit);

            Func<CheckOutput> fun = () => result.AsCheck(vm.InputLoadVM.UltimateLoad);

            return Tuple.Create(fun, result as ResistanceOutput);

        }
        private static Tuple<Func<CheckOutput>, ResistanceOutput> EuroResistanceCUnstiffMoment(EffectiveWidthViewModel vm, Material material, LengthBracingConditions bracingConditions)
        {
            var result = vm.GeometryVM.AsEntity()
                                       .Convert(vm.GeneralInfoVM.Unit, Units.NMM)
                                       .AsUnStiffenedSection()
                                       .AsEuroMomentResistance(material, bracingConditions)
                                       .Convert(Units.NMM, vm.GeneralInfoVM.Unit);
            Func<CheckOutput> fun = () => result.AsCheck(vm.InputLoadVM.UltimateMoment);

            return Tuple.Create(fun, result as ResistanceOutput);
        }
        private static Tuple<Func<CheckOutput>, ResistanceOutput> EuroResistanceCLippedComp(EffectiveWidthViewModel vm, Material material, LengthBracingConditions bracingConditions)
        {
            var result = vm.GeometryVM.AsEntity()
                                      .Convert(vm.GeneralInfoVM.Unit, Units.NMM)
                                      .AsLippedSection()
                                      .AsEuroCompressionResistance(material, bracingConditions)
                                      .Convert(Units.NMM, vm.GeneralInfoVM.Unit);

            Func<CheckOutput> fun = () => result.AsCheck(vm.InputLoadVM.UltimateLoad);

            return Tuple.Create(fun, result as ResistanceOutput);
        }
        private static Tuple<Func<CheckOutput>, ResistanceOutput> EuroResistanceCLippedMoment(EffectiveWidthViewModel vm, Material material, LengthBracingConditions bracingConditions)
        {
            var result = vm.GeometryVM.AsEntity()
                                      .Convert(vm.GeneralInfoVM.Unit, Units.NMM)
                                      .AsLippedSection()
                                      .AsEuroMomentResistance(material, bracingConditions).Convert(Units.NMM, vm.GeneralInfoVM.Unit);
            Func<CheckOutput> fun = () => result.AsCheck(vm.InputLoadVM.UltimateMoment);
            return Tuple.Create(fun, result as ResistanceOutput);
        }


        private static Tuple<Func<CheckOutput>, ResistanceOutput> AISIResistanceCUnstiffComp(EffectiveWidthViewModel vm, Material material, LengthBracingConditions bracingConditions)
        {
            var result = vm.GeometryVM.AsEntity()
                                       .Convert(vm.GeneralInfoVM.Unit, Units.KIPINCH)
                                       .AsUnStiffenedSection()
                                       .AsAISICompressionResistance(material, bracingConditions)
                                       .Convert(Units.KIPINCH, vm.GeneralInfoVM.Unit);

            Func<CheckOutput> fun = () => result.AsCheck(vm.InputLoadVM.UltimateLoad);

            return Tuple.Create(fun, result as ResistanceOutput);

        }
        private static Tuple<Func<CheckOutput>, ResistanceOutput> AISIResistanceCUnstiffMoment(EffectiveWidthViewModel vm, Material material, LengthBracingConditions bracingConditions)
        {
            var result = vm.GeometryVM.AsEntity()
                                       .Convert(vm.GeneralInfoVM.Unit, Units.KIPINCH)
                                       .AsUnStiffenedSection()
                                       .AsAISIMomentResistance(material, bracingConditions)
                                       .Convert(Units.KIPINCH, vm.GeneralInfoVM.Unit);
            Func<CheckOutput> fun = () => result.AsCheck(vm.InputLoadVM.UltimateMoment);

            return Tuple.Create(fun, result as ResistanceOutput);
        }
        private static Tuple<Func<CheckOutput>, ResistanceOutput> AISIResistanceCLippedComp(EffectiveWidthViewModel vm, Material material, LengthBracingConditions bracingConditions)
        {
            var result = vm.GeometryVM.AsEntity()
                                      .Convert(vm.GeneralInfoVM.Unit, Units.KIPINCH)
                                      .AsLippedSection()
                                      .AsAISICompressionResistance(material, bracingConditions)
                                      .Convert(Units.KIPINCH, vm.GeneralInfoVM.Unit);

            Func<CheckOutput> fun = () => result.AsCheck(vm.InputLoadVM.UltimateLoad);

            return Tuple.Create(fun, result as ResistanceOutput);
        }
        private static Tuple<Func<CheckOutput>, ResistanceOutput> AISIResistanceCLippedMoment(EffectiveWidthViewModel vm, Material material, LengthBracingConditions bracingConditions)
        {
            var result = vm.GeometryVM.AsEntity()
                                      .Convert(vm.GeneralInfoVM.Unit, Units.KIPINCH)
                                      .AsLippedSection()
                                      .AsAISIMomentResistance(material, bracingConditions).Convert(Units.KIPINCH, vm.GeneralInfoVM.Unit);
            Func<CheckOutput> fun = () => result.AsCheck(vm.InputLoadVM.UltimateMoment);
            return Tuple.Create(fun, result as ResistanceOutput);
        }

        #endregion

    }
}
