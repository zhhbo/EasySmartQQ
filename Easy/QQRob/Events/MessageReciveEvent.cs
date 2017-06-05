using System;
using Prism.Events;
namespace Easy.QQRob.Events
{

    public abstract class MessageReciveEvent : PubSubEvent<MessageReciveEventArgs>
    {
    }
    public  class CommonMessageReciveEvent : MessageReciveEvent
    {
    }
    public class GroupMessageReciveEvent : MessageReciveEvent
    {
    }
    public class DiscussionMessageReciveEvent : MessageReciveEvent
    {
    }
    public class MessageReciveEventArgs : EventArgsBase
    {
        public long Id { get; }
        public string Message { get; }
        public MessageReciveEventArgs(long id,string message)
        {
            Id = id;
            Message = message;
        }
    }
}
