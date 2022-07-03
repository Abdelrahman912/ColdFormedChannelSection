using ColdFormedChannelSection.App.ViewModels.Base;
using ColdFormedChannelSection.App.ViewModels.Enums;
using ColdFormedChannelSection.Core.Enums;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ColdFormedChannelSection.App.ViewModels
{
    public class GeneralInfoViewModel : ViewModelBase
    {

        public Action OnStrainingActionsChange = delegate { };
        public Action OnRunningModuleChange = delegate { };


        #region Private Fields
        private string _steelSectionImage;
        private bool _isDesignCode;
        private DesignCode _designCode;
        private SteelSection _steelSection;
        private StrainingActions _strainingAction;
        private UnitSystems _unit;

        private List<StrainingActions> _availableSa;
        private readonly List<StrainingActions> _resistSa = new List<StrainingActions>() { StrainingActions.COMPRESSION, StrainingActions.MOMENT };
        private readonly List<StrainingActions> _designCheckSa = new List<StrainingActions>() { StrainingActions.COMPRESSION, StrainingActions.MOMENT, StrainingActions.MOMENT_COMPRESSION };

        private Module _runningModule;

        private readonly Dictionary<SteelSection, string> _sectionImageDict = new Dictionary<SteelSection, string>()
        {
            [SteelSection.C_LIPPED] = "../Resources/lipped C.jpg",
            [SteelSection.C_UNSTIFFENED] = "../Resources/Unstiffened C.jpg",
            [SteelSection.Z_LIPPED] = "../Resources/lipped C.jpg",
            [SteelSection.Z_UNSTIFFENED] = "../Resources/Unstiffened C.jpg",
        };

        #endregion

        #region Properties

        public List<StrainingActions> AvailabeSa
        {
            get => _availableSa;
            set => NotifyPropertyChanged(ref _availableSa, value);
        }

        public Module RunningModule
        {
            get => _runningModule;
            set
            {
                NotifyPropertyChanged(ref _runningModule, value);
                OnRunningModuleChange();
                if (_runningModule != Module.RESISTANCE)
                {
                    AvailabeSa = _designCheckSa;
                    //StrainingAction = AvailabeSa.First();
                    Mediator.Mediator.Instance.NotifyColleagues(Tuple.Create(RunningModule, StrainingAction), Context.SA_MODULE);
                }
                else
                {
                    Mediator.Mediator.Instance.NotifyColleagues(Tuple.Create(RunningModule, StrainingAction), Context.SA_MODULE);
                    AvailabeSa = _resistSa;
                    //StrainingAction = AvailabeSa.First();
                }
            }
        }

        public string SteelSectionImage
        {
            get => _steelSectionImage;
            set => NotifyPropertyChanged(ref _steelSectionImage, value);
        }

        public SteelSection SteelSection
        {
            get => _steelSection;
            set
            {
                NotifyPropertyChanged(ref _steelSection, value);
                Mediator.Mediator.Instance.NotifyColleagues(SteelSection, Context.STIFF_UNSTIFF);
                SteelSectionImage = _sectionImageDict[_steelSection];

            }
        }

        public DesignCode DesignCode
        {
            get => _designCode;
            set

            {
                NotifyPropertyChanged(ref _designCode, value);
                Mediator.Mediator.Instance.NotifyColleagues(KeyValuePair.Create(_designCode, StrainingAction), Context.BRACING);
            }

        }

        public bool IsDesignCode
        {
            get => _isDesignCode;
            set => NotifyPropertyChanged(ref _isDesignCode, value);
        }

        public StrainingActions StrainingAction
        {
            get => _strainingAction;
            set
            {
                NotifyPropertyChanged(ref _strainingAction, value);
                OnStrainingActionsChange();
                if (RunningModule != Module.RESISTANCE)
                    Mediator.Mediator.Instance.NotifyColleagues(Tuple.Create(RunningModule, _strainingAction), Context.SA_MODULE);
                Mediator.Mediator.Instance.NotifyColleagues(_strainingAction, Context.STRAININGACTIONS);
                Mediator.Mediator.Instance.NotifyColleagues(KeyValuePair.Create(DesignCode, _strainingAction), Context.BRACING);
            }
        }

        //public bool IsUnstiffened
        //{
        //    get => _isUnstiffened;
        //    set 
        //    { 
        //        NotifyPropertyChanged(ref _isUnstiffened, value);
        //    }
        //}

        public UnitSystems Unit
        {
            get => _unit;
            set
            {
                NotifyPropertyChanged(ref _unit, value);
                Mediator.Mediator.Instance.NotifyColleagues(_unit, Context.UNITS);
            }
        }



        #endregion

        #region Constructors
        public GeneralInfoViewModel()
        {
            RunningModule = Module.RESISTANCE;
            DesignCode = DesignCode.AISI;
            SteelSection = SteelSection.C_UNSTIFFENED;
            StrainingAction = StrainingActions.MOMENT;
            Unit = UnitSystems.TONCM;
        }
        #endregion


    }
}
