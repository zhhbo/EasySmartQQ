using System;
using Prism.Events;
namespace Easy.QQRob.Events
{

    public class ErrorEvent : PubSubEvent<ErrorEventArgs>
    {
    }

    public class ErrorEventArgs : EventArgsBase
    {
        public int Code { get; }
        public string Message { get; }
        public ErrorEventArgs(int code,string message="")
        {
            Code = code;
            Message = message;
        }
    }
}
