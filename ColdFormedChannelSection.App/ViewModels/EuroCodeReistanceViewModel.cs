using ColdFormedChannelSection.App.ViewModels.Enums;

namespace ColdFormedChannelSection.App.ViewModels
{
    internal class EuroCodeReistanceViewModel : ResistanceBaseViewModel
    {

        #region Private Fields


        #endregion

        #region Private Fields



        #endregion

        #region Properties

        #endregion

        #region Constructors

        public EuroCodeReistanceViewModel()
            : base("Resistance | Euro Code")
        {
            _isUsedParamsAction += (sa) =>
              {
                  if (sa == StrainingActions.MOMENT)
                      IsC1Used = true;
                  else
                      IsC1Used = false;
              };
            StrainingAction = StrainingActions.MOMENT;
        }

        #endregion

    }
}
