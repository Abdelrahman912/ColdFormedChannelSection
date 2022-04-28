using ColdFormedChannelSection.App.ViewModels.Base;
using System.Windows.Input;

namespace ColdFormedChannelSection.App.ViewModels
{
    public abstract class CheckBaseViewModel:ViewModelBase
    {

        #region Private Fields

        private GeneralInfoViewModel _generalInfoVM;

        private BracingConditionsViewModel _bracingConditionsVM;

        private GeometryViewModel _geometryVM;

        private bool _isCheckOutput;

        #endregion

        #region Properties

        public bool IsCheckOutput
        {
            get => _isCheckOutput;
            set=>NotifyPropertyChanged(ref _isCheckOutput, value);
        }

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

        public CheckOutputViewModel CheckOutputVM { get;  }

        public virtual ICommand ResultsCommand { get; }

        #endregion

        #region Constructors
        public CheckBaseViewModel(GeneralInfoViewModel generalInfoVM,
                                       BracingConditionsViewModel bracingConditionsVM,
                                       GeometryViewModel geometryVM)
        {


            GeneralInfoVM = generalInfoVM;
            BracingConditionsVM = bracingConditionsVM;
            GeometryVM = geometryVM;
            CheckOutputVM = new CheckOutputViewModel();
            IsCheckOutput = false;


        }
        #endregion



    }
}
