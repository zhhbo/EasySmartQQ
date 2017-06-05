using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json.Serialization;
using Newtonsoft.Json;
namespace Easy.QQRob.Data.Yahoo
{
    public class JsonYahooExchangeRateModel
    {
        public paramQuery query;
        public class paramQuery
        {
            public string created;
            public paramResults results;
            public class paramResults
            {
                public paramRate rate;
                public class paramRate
                {
                    public string id;
                    public string Rate;
                }
            }
        }
    }
}
