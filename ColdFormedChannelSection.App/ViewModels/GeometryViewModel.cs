using ColdFormedChannelSection.App.ViewModels.Base;
using ColdFormedChannelSection.App.ViewModels.Enums;
using ColdFormedChannelSection.Core.Dtos;
using ColdFormedChannelSection.Core.Enums;
using ColdFormedChannelSection.Core.Extensions;
using ColdFormedChannelSection.Core.Helpers;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using static ColdFormedChannelSection.Core.Constants;

namespace ColdFormedChannelSection.App.ViewModels
{
    internal class GeometryViewModel:ViewModelBase
    {


        #region Private Fields

        private double _totalHeightH;
        private double _totalWidthB;
        private double _internalRadiusR;
        private double _thicknessT;
        private double _totalFoldWidthC;
        private bool _isUnstiffened;

        private bool _isUserDefined;
        private TablesName _selectedTableName;
        private List<SectionDimensionDto> _sections;

        private SectionDimensionDto _selectedSection;

        private Units _tableUnit;

        private Units _selectedUnit;

        private readonly Dictionary<KeyValuePair<bool, TablesName>, Tuple<Units,Lazy<Task<List<SectionDimensionDto>>>>> _tables = new Dictionary<KeyValuePair<bool, TablesName>, Tuple<Units, Lazy<Task<List<SectionDimensionDto>>>>> ()
        {
            //Unstiffened, Egypt
            {KeyValuePair.Create(true,TablesName.EGYPT_EURO),Tuple.Create(Units.NMM, new Lazy<Task<List<SectionDimensionDto>>>(()=>LoadSections(EGYPT_UNSTIFF_TABLE))) },
            //Stiffened, Egypt
            {KeyValuePair.Create(false,TablesName.EGYPT_EURO),Tuple.Create(Units.NMM, new Lazy<Task<List<SectionDimensionDto>>>(()=>LoadSections(EGYPT_STIFF_TABLE))) },
            //Unstiffened, American
            {KeyValuePair.Create(true,TablesName.AMERICAN),Tuple.Create(Units.KIPINCH, new Lazy<Task<List<SectionDimensionDto>>>(()=>LoadSections(AISI_UNSTIFF_TABLE))) },
              //Stiffened, American
            {KeyValuePair.Create(false,TablesName.AMERICAN), Tuple.Create(Units.KIPINCH,new Lazy<Task<List<SectionDimensionDto>>>(()=>LoadSections(AISI_STIFF_TABLE))) },
        };

        #endregion

        #region Properties

        public TablesName SelectedTableName
        {
            get => _selectedTableName;
            set
            {
                NotifyPropertyChanged(ref _selectedTableName,value);
                UpdateTable();
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
            Sections = new List<SectionDimensionDto>();
            IsUserDefined = true;
            TotalFoldWidthC = 0.0;
            TotalHeightH = 0.0;
            TotalWidthB = 0;
            ThicknessT = 0.0;
            InternalRadiusR = 0.0;
            _selectedUnit = Units.TONCM;
            IsUnstiffened = true;
            SelectedTableName = TablesName.EGYPT_EURO;
            Mediator.Mediator.Instance.Subscribe<bool>(this, OnStiffChanged, Context.STIFF_UNSTIFF);
            Mediator.Mediator.Instance.Subscribe<Units>(this, OnUnitsChanged, Context.UNITS);
        }



        #endregion

        #region Methods

        private async void UpdateTable()
        {
            if (!IsUserDefined)
            {
              var tuple =   _tables[KeyValuePair.Create(IsUnstiffened, SelectedTableName)];
                _tableUnit = tuple.Item1;
                Sections = await tuple.Item2.Value;
                SelectedSection = Sections.FirstOrDefault();
            }
        }
        private void UpdateSection()
        {
            if(SelectedSection != null)
            {
                var secDim = SelectedSection.AsEntity().Convert(_tableUnit,_selectedUnit);
                TotalHeightH = secDim.TotalHeightH.Round(4);
                TotalFoldWidthC = secDim.TotalFoldWidthC.Round(4);
                TotalWidthB = secDim.TotalFlangeWidthB.Round(4);
                InternalRadiusR = secDim.InternalRadiusR.Round(4);
                ThicknessT = secDim.ThicknessT.Round(4);
            }
        }

        private void OnUnitsChanged(Units selectedUnit)
        {
            _selectedUnit = selectedUnit;
            if (!IsUserDefined)
                UpdateSection();
        }
        private void OnStiffChanged(bool isUnstiff)
        {
            IsUnstiffened=isUnstiff;
           UpdateTable();
            if (isUnstiff)
                TotalFoldWidthC = 0;
        }


        private static async Task<List<SectionDimensionDto>> LoadSections(string tableName)
        {
            var dir = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            var filePath = $"{dir}\\{DATABASE_FOLDER}\\{tableName}.csv";
            return (await filePath.ReadAsCsv<SectionDimensionDto>())
                           .Match(errs=> new List<SectionDimensionDto>(),
                                  dtos =>dtos);
        }

        #endregion


    }
}
