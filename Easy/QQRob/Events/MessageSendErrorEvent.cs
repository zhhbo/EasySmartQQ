using System;
using Prism.Events;
namespace Easy.QQRob.Events
{

    public class MessageSendErrorEvent : PubSubEvent<MessageSendErrorArgs>
    {
    }

    public class MessageSendErrorArgs : EventArgsBase
    {
        public int Type { get; }
        public string Id { get; }
        public string Result { get; }
        public string Message { get; }
        public MessageSendErrorArgs(int type,string id,string result,string message)
        {
            Type = type;
            Id = id;
            Result = result;
            Message = message;
        }
    }
}
