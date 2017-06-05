using System;
using Easy.Data.Attributes;
namespace Easy.QQRob.Models

{
    [TableMap("MessageLogData")]
    public class MessageLogData
    {
        [PrimaryKey("Id", typeof(int), "Id", typeof(int))]
        public int Id { get; set; }
        [PropertyFieldMap("OriginData", typeof(string), "OriginData", typeof(string))]
        public string OriginData { get; set; }
        [PropertyFieldMap("CreateTime", typeof(DateTime), "CreateTime", typeof(DateTime))]
        public DateTime CreateTime { get; set; } = DateTime.Now;
        [PropertyFieldMap("ReCode", typeof(int), "ReCode", typeof(int))]
        public int ReCode { get; set; }
        [PropertyFieldMap("Message", typeof(string), "Message", typeof(string))]
        public string Message { get; set; }
        [PropertyFieldMap("FromUin", typeof(long), "FromUin", typeof(long))]
        public long FromUin { get; set; }
        /// <summary>
        /// 群号或讨论组号，或与本q的私聊号
        /// </summary>
        [PropertyFieldMap("OwnerUin", typeof(long), "OwnerUin", typeof(long))]
        public long OwnerUin { get; set; }
    }
}
