using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json.Serialization;
using Newtonsoft.Json;
namespace Easy.QQRob.Data.Weather
{
    public class JsonWeatherIndexModel
    {
        public List<paramI> i;
        public class paramI
        {
            public string i1;   //指数简称
            public string i2;   //指数名称
            public string i3;   //指数别称
            public string i4;   //指数级别
            public string i5;   //级别说明
        }
    }
}
