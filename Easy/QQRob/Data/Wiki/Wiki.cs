using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json.Serialization;
using Newtonsoft.Json;
namespace Easy.QQRob.Data.Wiki
{
    public class JsonWikipediaModel
    {
        public string batchcomplete;
        public paramQuery query;
        public class paramQuery
        {
            public object pages;
        }
    }

}
