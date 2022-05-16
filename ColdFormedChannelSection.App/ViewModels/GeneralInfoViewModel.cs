using ColdFormedChannelSection.App.ViewModels.Base;
using ColdFormedChannelSection.App.ViewModels.Enums;
using ColdFormedChannelSection.Core.Enums;
using System;
using System.Collections.Generic;

namespace ColdFormedChannelSection.App.ViewModels
{
    public class GeneralInfoViewModel:ViewModelBase
    {

        public Action OnStrainingActionsChange = delegate { };

        #region Private Fields
        private string _steelSectionImage;
        private bool _isDesignCode;
        private DesignCode _designCode;
        private SteelSection _steelSection;
        private StrainingActions _strainingAction;
        private bool _isUnstiffened;
        private Units _unit;
        private double _fy;
        private double _e;
        private Module _runningModule;

        private readonly Dictionary<SteelSection, string> _sectionImageDict = new Dictionary<SteelSection, string>()
        {
            [SteelSection.C_LIPPED] = "../Resources/lipped C.jpg",
            [SteelSection.C_UNSTIFFENED] = "../Resources/Unstiffened C.jpg"
        };

        #endregion

        #region Properties

        public Module RunningModule
        {
            get => _runningModule;
            set
            {
                NotifyPropertyChanged(ref _runningModule, value);
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
                SteelSectionImage = _sectionImageDict[_steelSection];
            }
        }

        public DesignCode DesignCode
        {
            get => _designCode;
            set=>NotifyPropertyChanged(ref _designCode, value);
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
                Mediator.Mediator.Instance.NotifyColleagues(_strainingAction, Context.STRAININGACTIONS);
            }
        }

        public bool IsUnstiffened
        {
            get => _isUnstiffened;
            set 
            { 
                NotifyPropertyChanged(ref _isUnstiffened, value);
                Mediator.Mediator.Instance.NotifyColleagues(_isUnstiffened, Context.STIFF_UNSTIFF);
            }
        }

        public Units Unit
        {
            get => _unit;
            set
            {
                NotifyPropertyChanged(ref _unit, value);
                Mediator.Mediator.Instance.NotifyColleagues(_unit, Context.UNITS);
            }
        }

        public double Fy
        {
            get => _fy;
            set => NotifyPropertyChanged(ref _fy, value);
        }

        public double E
        {
            get => _e;
            set => NotifyPropertyChanged(ref _e, value);
        }

        #endregion

        #region Constructors
        public GeneralInfoViewModel()
        {
            DesignCode = DesignCode.AISI;
            SteelSection = SteelSection.C_UNSTIFFENED;
            StrainingAction = StrainingActions.MOMENT;
            Unit = Units.TONCM;
            IsUnstiffened = true;
            Fy = 0;
            E = 0;
        }
        #endregion


    }
}
