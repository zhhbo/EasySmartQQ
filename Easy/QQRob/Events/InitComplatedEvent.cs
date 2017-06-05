using System;
using Prism.Events;
namespace Easy.QQRob.Events
{

    public class InitComplatedEvent : PubSubEvent<InitComplatedEventArgs>
    {
    }

    public class InitComplatedEventArgs : EventArgsBase
    {    
        public string Code { get; }
        public string Message { get; }
        public InitComplatedEventArgs(string code,string message)
        {
            Code = code;

            Message = message;
        }
    }
}
