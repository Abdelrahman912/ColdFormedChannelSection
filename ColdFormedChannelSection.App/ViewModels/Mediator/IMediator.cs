using ColdFormedChannelSection.App.ViewModels.Enums;
using System;

namespace ColdFormedChannelSection.App.ViewModels.Mediator
{
    internal interface IMediator
    {
        bool Subscribe<T>(object subscriber, Action<T> onNotified, Context context);

        bool UnSubscribe<T>(object subscriber, Context context, Action onSubscribtionEnd);

        void NotifyColleagues<T>(T message, Context context);

        void Reset();
    }
}
