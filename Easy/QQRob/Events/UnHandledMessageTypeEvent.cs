using System;
using Prism.Events;
namespace Easy.QQRob.Events
{

    public class UnHandledMessageTypeEvent : PubSubEvent<UnHandledMessageTypeEventArgs>
    {
    }

    public class UnHandledMessageTypeEventArgs : EventArgsBase
    {    
        public string Type { get; }
        public string Message { get; }
        public UnHandledMessageTypeEventArgs(string type,string message)
        {
            Type = type;

            Message = message;
        }
    }
}
