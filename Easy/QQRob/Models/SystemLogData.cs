using System;
using Easy.Data.Attributes;
namespace Easy.QQRob.Models

{
    [TableMap("SystemLogData")]
    public class SystemLogData
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
    }
}
