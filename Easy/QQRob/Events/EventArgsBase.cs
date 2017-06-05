using System;

namespace Easy.QQRob.Events
{
    public class EventArgsBase : EventArgs
    {
        public DateTime Timestamp { get; } = DateTime.Now;
    }
}
