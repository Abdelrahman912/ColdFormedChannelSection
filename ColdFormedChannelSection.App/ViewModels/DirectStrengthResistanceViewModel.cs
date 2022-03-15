using ColdFormedChannelSection.App.ViewModels.Enums;

namespace ColdFormedChannelSection.App.ViewModels
{
    internal class DirectStrengthResistanceViewModel:ResistanceBaseViewModel
    {

       


        #region Constructors
        public DirectStrengthResistanceViewModel()
            :base("Resistance | Direct Strength")
        {
            IsLuUsed = true;
            _isUsedParamsAction += (sa) =>
            {
                if (sa == StrainingActions.COMPRESSION)
                    IsLuUsed = true;
            };
        }
        #endregion

    }
}
