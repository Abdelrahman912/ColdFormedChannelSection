using ColdFormedChannelSection.App.ViewModels.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace ColdFormedChannelSection.App.ViewModels
{
    public abstract class DesignBaseViewModel:ViewModelBase
    {
        #region Private Fields

        private GeneralInfoViewModel _generalInfoVM;

        private BracingConditionsViewModel _bracingConditionsVM;

        private GeometryViewModel _geometryVM;

        #endregion

        #region Properties

        

        public GeneralInfoViewModel GeneralInfoVM
        {
            get => _generalInfoVM;
            set => NotifyPropertyChanged(ref _generalInfoVM, value);
        }

        public BracingConditionsViewModel BracingConditionsVM
        {
            get => _bracingConditionsVM;
            set => NotifyPropertyChanged(ref _bracingConditionsVM, value);
        }

        public GeometryViewModel GeometryVM
        {
            get => _geometryVM;
            set => NotifyPropertyChanged(ref _geometryVM, value);
        }

        public DesignOutputViewModel DesignOutputVM { get; }

        public virtual ICommand ResultsCommand { get; }

        #endregion

        #region Constructors

        public DesignBaseViewModel(GeneralInfoViewModel generalInfoVM,
                                       BracingConditionsViewModel bracingConditionsVM)

        {

            GeneralInfoVM = generalInfoVM;
            BracingConditionsVM = bracingConditionsVM;
            GeometryVM = new GeometryViewModel();
            GeometryVM.IsDesign = true;
            GeometryVM.IsUserDefined = false;
            DesignOutputVM = new DesignOutputViewModel();
        }

        #endregion
    }
}
