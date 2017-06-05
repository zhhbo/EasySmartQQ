using System;
using Prism.Events;
namespace Easy.QQRob.Events
{

    public class LoginStatusChangedEvent : PubSubEvent<LoginEventArgs>
    {
    }

    public class LoginEventArgs : EventArgsBase
    {
        public string Code { get; }
        public string Message { get; }
        public LoginEventArgs(string code,string message)
        {
            Code = code;
            Message = message;
        }
    }
}
