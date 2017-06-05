using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Easy;
using Easy.Data.Attributes;
namespace Easy.QQRob.Models

{
    [TableMap("StudyWords")]
    public class StudyWords
    {
        [PrimaryKey("Id", typeof(int), "Id", typeof(int))]
        public int Id { get; set; }
        [PropertyFieldMap("Source", typeof(string), "Source", typeof(string))]
        public string Source { get; set; }
        [PropertyFieldMap("Aim", typeof(string), "Aim", typeof(string))]
        public string Aim { get; set; }
        [PropertyFieldMap("QQNum", typeof(long), "QQNum", typeof(long))]
        public long QQNum { get; set; }
        [PropertyFieldMap("SourceQQNum", typeof(long), "SourceQQNum", typeof(long))]
        public long SourceQQNum { get; set; }
        [PropertyFieldMap("GroupId", typeof(long), "GroupId", typeof(long))]
        public long GroupId { get; set; }
        [PropertyFieldMap("Pass", typeof(bool), "Pass", typeof(bool))]
        public bool Pass { get; set; }
        [PropertyFieldMap("Reson", typeof(string), "Reson", typeof(string))]
        public string Reson { get; set; }
    }
}
