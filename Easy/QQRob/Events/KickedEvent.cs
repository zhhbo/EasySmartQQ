using Prism.Events;
namespace Easy.QQRob.Events
{

    public class KickedEvent : PubSubEvent<KickedEventArgs>
    {
    }

    public class KickedEventArgs : EventArgsBase
    {    
        public long Uin { get; }
        public string Message { get; }
        public KickedEventArgs(long uin,string message)
        {
            Uin = uin;

            Message = message;
        }
    }
}
