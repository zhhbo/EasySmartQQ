using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json.Serialization;
using Newtonsoft.Json;
namespace Easy.QQRob.Data.Weather
{
    public class JsonWeatherModel
    {
        public paramC c;
        public paramF f;
        public class paramC
        {
            public string c1;   //区域ID
            public string c2;   //城市英文名
            public string c3;   //城市中文名
            public string c4;   //城市所在市英文名
            public string c5;   //城市所在市中文名
            public string c6;   //城市所在省英文名
            public string c7;   //城市所在省中文名
            public string c8;   //城市所在国英文名
            public string c9;   //城市所在国中文名
            public string c10;  //城市级别
            public string c11;  //城市区号
            public string c12;  //邮编
            public string c13;  //经度
            public string c14;  //纬度
            public string c15;  //海拔
            public string c16;  //雷达站号
            public string c17;  //时区
        }
        public class paramF
        {
            public string f0;   //预报发布时间
            public List<paramF1> f1;
        }
        public class paramF1
        {
            public string fa;   //白天天气现象编号
            public string fb;   //晚上天气现象编号
            public string fc;   //白天气温
            public string fd;   //晚上气温
            public string fe;   //白天风向编号
            public string ff;   //晚上风向编号
            public string fg;   //白天风力编号
            public string fh;   //晚上风力编号
            public string fi;   //日出日落时间
        }
    }
}
