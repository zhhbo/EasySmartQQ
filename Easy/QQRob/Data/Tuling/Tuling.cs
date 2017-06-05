using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json.Serialization;
using Newtonsoft.Json;
namespace Easy.QQRob.Data.Tuling
{
    public class JsonTuLinModel
    {
        public int code;
        public string text;
        public string url;
        public List<paramList> list;
        public class paramList
        {
            public string article;
            public string source;
            public string icon;
            public string detailurl;
            public string info;
            public string name;
        }
    }
}
