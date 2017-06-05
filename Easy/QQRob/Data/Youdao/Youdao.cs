using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json.Serialization;
using Newtonsoft.Json;
namespace Easy.QQRob.Data.Youdao
{
    public class JsonYoudaoTranslateModel
    {
        public int errorcode;
        public string query;
        public List<string> translation;
    }

}
