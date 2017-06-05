using Easy.Data.Common.Enum;

namespace Easy.Data.Common.Interface
{
    /// <summary>
    /// 消息
    /// </summary>
    public interface IMessage
    {
        MessageContentType MsgContentType { get; }
        object Content { get; set; }
    }
}
