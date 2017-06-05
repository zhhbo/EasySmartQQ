using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Easy;
using Easy.Data.Attributes;
namespace Easy.QQRob.Models

{
    [TableMap("AutoAnswerMessageLog")]
    public class AutoAnswerMessageLog
    {
        [PrimaryKey("Id", typeof(int), "Id", typeof(int))]
        public int Id { get; set; }
        [PropertyFieldMap("Data", typeof(string), "Data", typeof(string))]
        public string Data { get; set; }
        [PropertyFieldMap("FromUin", typeof(long), "FromUin", typeof(long))]
        public long FromUin { get; set; }
        [PropertyFieldMap("ToUin", typeof(long), "ToUin", typeof(long))]
        public long ToUin { get; set; }
        [PropertyFieldMap("MessageType", typeof(string), "MessageType", typeof(string))]
        public string MessageType { get; set; }
        [PropertyFieldMap("CreateTime", typeof(DateTime), "CreateTime", typeof(DateTime))]
        public DateTime CreateTime { get; set; } = DateTime.Now;

        [PropertyFieldMap("P1", typeof(string), "P1", typeof(string))]
        public string P1 { get; set; }
        [PropertyFieldMap("P2", typeof(string), "P2", typeof(string))]
        public string P2 { get; set; }
        [PropertyFieldMap("P3", typeof(string), "P3", typeof(string))]
        public string P3 { get; set; }
        [PropertyFieldMap("P4", typeof(string), "P4", typeof(string))]
        public string P4 { get; set; }
    }
}
