using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json.Serialization;
using Newtonsoft.Json;
namespace Easy.QQRob.Data.Yahoo
{
    public class JsonYahooWeatherModel
    {
        public paramQuery query;
        public class paramQuery
        {
            public string created;
            public paramResults results;
            public class paramResults
            {
                public paramChannel channel;
                public class paramChannel
                {
                    public string description;
                    public paramItem item;
                    public class paramItem
                    {
                        public List<parmForecast> forecast;
                        public class parmForecast
                        {
                            public string code;
                            public string date;
                            public string day;
                            public string high;
                            public string low;
                            public string text;
                        }
                    }
                }
            }
        }
    }
}
