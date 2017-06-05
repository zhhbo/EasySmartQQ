using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Easy;
using Easy.Data.Attributes;
namespace Easy.QQRob.Models

{
    [TableMap("BadWords")]
    public class BadWords
    {
        [PrimaryKey("Id", typeof(int), "Id", typeof(int))]
        public int Id { get; set; }
        [PropertyFieldMap("Word", typeof(string), "Word", typeof(string))]
        public string Word { get; set; }
        [PropertyFieldMap("QQNum", typeof(long), "QQNum", typeof(long))]
        public long QQNum { get; set; }
    }
}
