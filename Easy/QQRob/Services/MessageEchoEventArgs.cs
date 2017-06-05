using System;
using Easy.QQRob.Data.SmartQQ;
namespace Easy.QQRob.Services
{
    /// <summary>
    ///     消息回显事件参数。
    /// </summary>
    public class MessageEchoEventArgs : EventArgs
    {
        internal MessageEchoEventArgs(IMessageable target, string content)
        {
            Target = target;
            Content = content;
        }

        /// <summary>
        ///     消息目标。
        /// </summary>
        public IMessageable Target { get; }

        /// <summary>
        ///     消息内容。
        /// </summary>
        public string Content { get; }
    }
    /// <summary>
    ///     消息回显事件参数。
    /// </summary>
    public class MessageRecivedEventArgs : EventArgs
    {
        internal MessageRecivedEventArgs(PollValue poll,string originData="")
        {
            Poll = poll;
            OriginData = originData;
        }

        /// <summary>
        ///     消息目标。
        /// </summary>
        public PollValue Poll { get; }

        /// <summary>
        ///     消息内容。
        /// </summary>
        public string OriginData { get; }
    }
    /// <summary>
    ///     消息回显事件参数。
    /// </summary>
    public class LoginSucessedEventArgs : EventArgs
    {
        internal LoginSucessedEventArgs(QQSession session, string jsonResult,bool showWindow=false)
        {
            Session = session;
            JsonResult = jsonResult;
            ShowMainWindow = showWindow;
        }

        /// <summary>
        ///     消息目标。
        /// </summary>
        public QQSession Session { get; }

        /// <summary>
        ///     消息内容。
        /// </summary>
        public string JsonResult { get; }
        public bool ShowMainWindow { get; set; }
    }
}
