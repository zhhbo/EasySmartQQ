using System;
using Prism.Events;
namespace Easy.QQRob.Events
{

    public abstract class MessageSendEvent : PubSubEvent<MessageSendEventArgs>
    {
    }
    public  class CommonMessageSendEvent : MessageSendEvent
    {
    }
    public class GroupMessageSendEvent : MessageSendEvent
    {
    }
    public class DiscussionMessageSendEvent : MessageSendEvent
    {
    }
    public class MessageSendEventArgs : EventArgsBase
    {
        public string Id { get; }
        public string Message { get; }
        public MessageSendEventArgs(string id,string message)
        {
            Id = id;
            Message = message;
        }
    }
}
