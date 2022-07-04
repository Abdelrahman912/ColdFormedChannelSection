using ColdFormedChannelSection.App.ViewModels.Base;
using ColdFormedChannelSection.App.ViewModels.Enums;
using ColdFormedChannelSection.Core.Dtos;
using ColdFormedChannelSection.Core.Enums;
using ColdFormedChannelSection.Core.Extensions;
using ColdFormedChannelSection.Core.Helpers;
using CSharp.Functional.Errors;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using static ColdFormedChannelSection.Core.Constants;
using static ColdFormedChannelSection.Core.Errors.Errors;

namespace ColdFormedChannelSection.App.ViewModels
{
    public class GeometryViewModel : ViewModelBase
    {


        #region Private Fields

        private readonly List<TablesName> _allTables = new List<TablesName>() { TablesName.EGYPT_EURO, TablesName.AMERICAN };

        private readonly List<TablesName> _americanTables = new List<TablesName>() { TablesName.AMERICAN };

        private bool _isDesign;

        private double _totalHeightH;
        private double _totalWidthB;
        private double _internalRadiusR;
        private double _thicknessT;
        private double _totalFoldWidthC;
        private bool _isUnstiffened;
        //private SteelSection _section;
        private bool _isUserDefined;
        private TablesName _selectedTableName;
        private List<SectionDimensionDto> _sections;

        private SectionDimensionDto _selectedSection;

        private List<TablesName> _tables;


        private UnitSystems _unit;

        private readonly Dictionary<KeyValuePair<SteelSection, TablesName>, Tuple<UnitSystems, Lazy<Task<List<SectionDimensionDto>>>>> _tablesDict = new Dictionary<KeyValuePair<SteelSection, TablesName>, Tuple<UnitSystems, Lazy<Task<List<SectionDimensionDto>>>>>()
        {
            //Unstiffened, Egypt
            [KeyValuePair.Create(SteelSection.C_UNSTIFFENED, TablesName.EGYPT_EURO)] = Tuple.Create(UnitSystems.NMM, new Lazy<Task<List<SectionDimensionDto>>>(() => LoadSections(EGYPT_UNSTIFF_TABLE_C))),
            //Stiffened, Egypt
            [KeyValuePair.Create(SteelSection.C_LIPPED, TablesName.EGYPT_EURO)] = Tuple.Create(UnitSystems.NMM, new Lazy<Task<List<SectionDimensionDto>>>(() => LoadSections(EGYPT_STIFF_TABL_C))),
            //Unstiffened, American
            [KeyValuePair.Create(SteelSection.C_UNSTIFFENED, TablesName.AMERICAN)] = Tuple.Create(UnitSystems.KIPINCH, new Lazy<Task<List<SectionDimensionDto>>>(() => LoadSections(AISI_UNSTIFF_TABLE_C))),
            //Stiffened, American
            [KeyValuePair.Create(SteelSection.C_LIPPED, TablesName.AMERICAN)] = Tuple.Create(UnitSystems.KIPINCH, new Lazy<Task<List<SectionDimensionDto>>>(() => LoadSections(AISI_STIFF_TABLE_C))),

            //Unstiffened, Egypt
            [KeyValuePair.Create(SteelSection.Z_UNSTIFFENED, TablesName.EGYPT_EURO)] = Tuple.Create(UnitSystems.NMM, new Lazy<Task<List<SectionDimensionDto>>>(() => LoadSections(EGYPT_UNSTIFF_TABLE_Z))),
            //Stiffened, Egypt
            [KeyValuePair.Create(SteelSection.Z_LIPPED, TablesName.EGYPT_EURO)] = Tuple.Create(UnitSystems.NMM, new Lazy<Task<List<SectionDimensionDto>>>(() => LoadSections(EGYPT_STIFF_TABL_Z))),
            //Unstiffened, American
            [KeyValuePair.Create(SteelSection.Z_UNSTIFFENED, TablesName.AMERICAN)] = Tuple.Create(UnitSystems.KIPINCH, new Lazy<Task<List<SectionDimensionDto>>>(() => LoadSections(AISI_UNSTIFF_TABLE_Z))),
            //Stiffened, American
            [KeyValuePair.Create(SteelSection.Z_LIPPED, TablesName.AMERICAN)] = Tuple.Create(UnitSystems.KIPINCH, new Lazy<Task<List<SectionDimensionDto>>>(() => LoadSections(AISI_STIFF_TABLE_Z))),
        };

        #endregion

        #region Properties

        public List<TablesName> Tables
        {
            get => _tables;
            set => NotifyPropertyChanged(ref _tables, value);
        }

        public UnitSystems TableUnit { get; set; }


        public bool IsDesign
        {
            get => _isDesign;
            set => NotifyPropertyChanged(ref _isDesign, value);
        }

        public UnitSystems Unit
        {
            get => _unit;
            set => NotifyPropertyChanged(ref _unit, value);
        }

        public TablesName SelectedTableName
        {
            get => _selectedTableName;
            set
            {
                NotifyPropertyChanged(ref _selectedTableName, value);
                 if(!IsUserDefined)
                    LoadTable();
            }
        }

        public List<SectionDimensionDto> Sections
        {
            get => _sections;
            set => NotifyPropertyChanged(ref _sections, value);
        }

        public SectionDimensionDto SelectedSection
        {
            get => _selectedSection;
            set
            {
                NotifyPropertyChanged(ref _selectedSection, value);
                UpdateSection();
            }
        }

        public bool IsUserDefined
        {
            get => _isUserDefined;
            set
            {
                NotifyPropertyChanged(ref _isUserDefined, value);
                UpdateTable();
            }
        }

        public SteelSection Section { get; set; }

        public bool IsUnstiffened
        {
            get => _isUnstiffened;
            set => NotifyPropertyChanged(ref _isUnstiffened, value);
        }

        public double TotalFoldWidthC
        {
            get => _totalFoldWidthC;
            set => NotifyPropertyChanged(ref _totalFoldWidthC, value);
        }

        public double ThicknessT
        {
            get => _thicknessT;
            set => NotifyPropertyChanged(ref _thicknessT, value);
        }

        public double InternalRadiusR
        {
            get => _internalRadiusR;
            set => NotifyPropertyChanged(ref _internalRadiusR, value);
        }

        public double TotalWidthB
        {
            get => _totalWidthB;
            set => NotifyPropertyChanged(ref _totalWidthB, value);
        }

        public double TotalHeightH
        {
            get => _totalHeightH;
            set => NotifyPropertyChanged(ref _totalHeightH, value);
        }

        #endregion

        #region Constructors

        public GeometryViewModel()
        {
            IsDesign = false;
            Sections = new List<SectionDimensionDto>();
            IsUserDefined = true;
            TotalFoldWidthC = 0.0;
            TotalHeightH = 0.0;
            TotalWidthB = 0;
            ThicknessT = 0.0;
            InternalRadiusR = 0.0;
            Unit = UnitSystems.TONCM;
            IsUnstiffened = true;
            SelectedTableName = TablesName.EGYPT_EURO;
            Mediator.Mediator.Instance.Subscribe<SteelSection>(this, OnStiffChanged, Context.STIFF_UNSTIFF);
            Mediator.Mediator.Instance.Subscribe<UnitSystems>(this, OnUnitsChanged, Context.UNITS);
            Mediator.Mediator.Instance.Subscribe<Tuple<Core.Enums.Module, StrainingActions>>(this, OnModuleChanged, Context.SA_MODULE);
        }





        #endregion

        #region Methods

        private void OnModuleChanged(Tuple<Core.Enums.Module, StrainingActions> tuple)
        {
            (var module, var _) = tuple;
            if (module == Core.Enums.Module.DESIGN)
            {
                IsUserDefined = false;
                IsDesign = true;
            }
            else
            {
                IsDesign = false;
            }
        }

        private  void UpdateTable()
        {

            if (!IsUserDefined)
            {
                if (Section == SteelSection.Z_UNSTIFFENED)
                {
                    Tables = _americanTables;
                    SelectedTableName = Tables.FirstOrDefault();
                }
                else
                {
                    Tables = _allTables;
                    SelectedTableName = Tables.FirstOrDefault();
                }
            }
        }
        private async void LoadTable()
        {
            var tuple = _tablesDict[KeyValuePair.Create(Section, SelectedTableName)];
            TableUnit = tuple.Item1;
            Sections = await tuple.Item2.Value;
            SelectedSection = Sections.FirstOrDefault();
        }

        private void UpdateSection()
        {
            if (SelectedSection != null)
            {
                var secDim = SelectedSection.AsEntity().Convert(TableUnit, Unit);
                TotalHeightH = secDim.TotalHeightH.Round(4);
                TotalFoldWidthC = secDim.TotalFoldWidthC.Round(4);
                TotalWidthB = secDim.TotalFlangeWidthB.Round(4);
                InternalRadiusR = secDim.InternalRadiusR.Round(4);
                ThicknessT = secDim.ThicknessT.Round(4);
            }
            else
            {
                TotalHeightH = 0.0;
                TotalFoldWidthC = 0.0;
                TotalWidthB = 0.0;
                InternalRadiusR = 0.0;
                ThicknessT = 0.0;
            }
        }

        private void OnUnitsChanged(UnitSystems selectedUnit)
        {
            Unit = selectedUnit;
            if (!IsUserDefined)
                UpdateSection();
        }
        private void OnStiffChanged(SteelSection steelSection)
        {

            IsUnstiffened = steelSection == SteelSection.C_UNSTIFFENED || steelSection == SteelSection.Z_UNSTIFFENED;
            Section = steelSection;
            UpdateTable();
            if (IsUnstiffened)
                TotalFoldWidthC = 0;
        }

        public List<Error> Validate()
        {
            if (!IsDesign)
            {
                var errs = new List<Error>();
                if (TotalHeightH <= 0)
                    errs.Add(LessThanZeroError("H"));
                if (TotalWidthB <= 0)
                    errs.Add(LessThanZeroError("B"));
                if (ThicknessT <= 0)
                    errs.Add(LessThanZeroError("t"));
                if (InternalRadiusR <= 0)
                    errs.Add(LessThanZeroError("R"));
                if (!_isUnstiffened && TotalFoldWidthC <= 0)
                    errs.Add(LessThanZeroError("C"));
                return errs;
            }
            else
            {
                return new List<Error>();
            }
        }

        private static async Task<List<SectionDimensionDto>> LoadSections(string tableName)
        {
            var dir = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            var filePath = $"{dir}\\{DATABASE_FOLDER}\\{tableName}.csv";
            return (await filePath.ReadAsCsv<SectionDimensionDto>())
                           .Match(errs => new List<SectionDimensionDto>(),
                                  dtos => dtos);
        }

        #endregion


    }
}
