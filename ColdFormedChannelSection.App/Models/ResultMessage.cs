using ColdFormedChannelSection.App.UI.Services;

namespace ColdFormedChannelSection.App.Models
{
    public class ResultMessage
    {
        #region Properties

        public string Message { get; }

        public ResultMessageType MessageType { get; }

        #endregion

        #region Constructors

        public ResultMessage(string message, ResultMessageType messageType)
        {
            Message = message;
            MessageType = messageType;
        }

        #endregion
    }
}
