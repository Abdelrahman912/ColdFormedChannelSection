using ColdFormedChannelSection.App.ViewModels.Base;

namespace ColdFormedChannelSection.App.ViewModels
{
    internal abstract class ResistanceBaseViewModel:ViewModelBase
    {

        #region Properties

        public string Name { get; }

        #endregion

        #region Constructors

        public ResistanceBaseViewModel(string name)
        {
            Name = name;
        }

        #endregion

    }
}
