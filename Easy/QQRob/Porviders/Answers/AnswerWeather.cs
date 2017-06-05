using System.Collections.Generic;
using System.Linq;
using Easy.QQRob.Models;
using Easy.QQRob.Data.Yahoo;
using Newtonsoft.Json;

namespace Easy.QQRob.Services
{
    public  class AnswerWeather:IAutoAnswer
    {
        public int Priority { get { return 0; } }
        public  bool MustManager { get { return false; } }
      public  string SettingKey { get { return "EnableWeather"; } }
      public  string Name { get { return "查天气"; } }
      public  string Description { get { return "针对天气的消息回答"; } }
        public bool ShowInMenu { get; } = true;
        public List<string> Template()
        {
            string template = "{0} 地点";
            List<string> results = new List<string>();
            foreach (var command in CommandKey)
            {
                results.Add(string.Format(template, command));
            }
            return results;
        }
        public List<string> Example()
        {
            string template = "{0} 郑州";
           // string yahootemplate = "{0} 郑州";

            List<string> results = new List<string>();
            results.Add(string.Format(template, CommandKey[0]));
            //foreach (var command in CommandKey)
           // {
            //    results.Add(string.Format(yahootemplate, command));
           //     results.Add(string.Format(template, command));
          //  }
            return results;
        }
        public bool CanAnswer(AnswerContext context)
        {
            if (context.MessageType == "message")
                return true;
            if (context.State != null && context.GetState<bool>(SettingKey))
            { return true; }
            return false;
        }
        public void Answer(AnswerContext context)
        {
            if (!context.CanAnswer)
                return;
            List<string> anwer =null;
            if (CommandKey.Any(x=>context.Message.StartsWith(x)))
            {
                bool EnableFlag = false;
                if (context.State != null && context.GetState<bool>(SettingKey))
                {
                    EnableFlag = true;
                }
                else if(context.MessageType=="message"){
                    EnableFlag = true;
                }
                if (EnableFlag)
                {
                    string[] tmp = context.Message.Trim().Split(' ');
                    if (tmp.Length == 2)
                        anwer = GetWeather(tmp[1]);
                    else
                    {
                        context.Alerts.AddRange(Example());
                    }

                    if (anwer!=null&& anwer.Count>0)
                    {
                        context.Answers.AddRange(anwer);
                        var newlog = new AutoAnswerMessageLog();
                        newlog.FromUin = context.SendToId;
                        newlog.ToUin = context.FromUin;
                        newlog.MessageType = "weather";
                        newlog.P1 = tmp[1];
                        newlog.Data = string.Join("，",anwer);
                        OrmManager.Insert(newlog);
                    }

                }
            }
           
        }
      public string []MessageType { get { return new[] { "message", "group_message", "discu_message" }; } }
      public string[] CommandKey { get { return new[] { "天气", "热死了" ,"冷死了","下雪","下雨","风"}; } }
      public  AnswerCommandType CommandType { get { return AnswerCommandType.Start; } }

        public  List<string> GetWeather(string city)
        {
            if ((!city.Equals("呼市郊区")) && (!city.Equals("津市")) && (!city.Equals("沙市")))
            {
                city = city.Replace("省", "");
                city = city.Replace("市", "");
            }
            city = city.Replace(" ", "");
            city = city.Replace("\r", "");
            city = city.Replace("\n", "");

             string ans = "";
            var result = new List<string>();

            string url, temp;

                url = "https://query.yahooapis.com/v1/public/yql?q=select%20*%20from%20weather.forecast%20where%20woeid%20in%20%28select%20woeid%20from%20geo.places%281%29%20where%20text=\"" + city + "\") and%20u=%22c%22&format=json";
                temp = QQClient.Get(url);
                JsonYahooWeatherModel weather = (JsonYahooWeatherModel)JsonConvert.DeserializeObject(temp, typeof(JsonYahooWeatherModel));
            if (weather.query.results == null)
                // return ;
                result.Add("未查询到指定城市 " + city + " 的天气信息");
            else
            {
                var count = weather.query.results.channel.item.forecast.Count >= 7 ? 7 : weather.query.results.channel.item.forecast.Count;
                for (int i = 0; i <count; i++)

                {

                  var  waittoadd = System.Environment.NewLine+"周" + getYahooWeak(weather.query.results.channel.item.forecast[i].day) + "：" + getYahooWeatherCode(weather.query.results.channel.item.forecast[i].code) + "，最高气温：" + weather.query.results.channel.item.forecast[i].high + "摄氏度，最低气温：" + weather.query.results.channel.item.forecast[i].low + "摄氏度";
                    if (ans.Length + waittoadd.Length < 400)
                    {
                        ans += waittoadd;
                    }
                    else
                    {
                        result.Add(ans);
                        ans = waittoadd;
                    }
                }
                result.Add(ans);

            }
            return result;

        }

        private  string getYahooWeatherCode(string code)
        {
            switch (code)
            {
                case "0": return "龙卷风";
                case "1": return "热带风暴";
                case "2": return "暴风";
                case "3": return "大雷雨";
                case "4": return "雷阵雨";
                case "5": return "雨夹雪";
                case "6": return "雨夹雹";
                case "7": return "雪夹雹";
                case "8": return "冻雾雨";
                case "9": return "细雨";
                case "10": return "冻雨";
                case "11": return "阵雨";
                case "12": return "阵雨";
                case "13": return "阵雪";
                case "14": return "小阵雪";
                case "15": return "高吹雪";
                case "16": return "雪";
                case "17": return "冰雹";
                case "18": return "雨淞";
                case "19": return "粉尘";
                case "20": return "雾";
                case "21": return "薄雾";
                case "22": return "烟雾";
                case "23": return "大风";
                case "24": return "风";
                case "25": return "冷";
                case "26": return "阴";
                case "27": return "多云";
                case "28": return "多云";
                case "29": return "局部多云";
                case "30": return "局部多云";
                case "31": return "晴";
                case "32": return "晴";
                case "33": return "转晴";
                case "34": return "转晴";
                case "35": return "雨夹冰雹";
                case "36": return "热";
                case "37": return "局部雷雨";
                case "38": return "偶有雷雨";
                case "39": return "偶有雷雨";
                case "40": return "偶有阵雨";
                case "41": return "大雪";
                case "42": return "零星阵雪";
                case "43": return "大雪";
                case "44": return "局部多云";
                case "45": return "雷阵雨";
                case "46": return "阵雪";
                case "47": return "局部雷阵雨";
                default: return "水深火热";
            }
        }

        private  string getYahooWeak(string day)
        {
            switch (day.ToLower())
            {
                case "mon": return "一";
                case "tue": return "二";
                case "wed": return "三";
                case "thu": return "四";
                case "fri": return "五";
                case "sat": return "六";
                case "sun": return "日";
                default: return day;
            }
        }

        private  string SloveWind(string code)
        {
            switch (code)
            {
                case ("0"): return "";
                case ("1"): return "东北风";
                case ("2"): return "东风";
                case ("3"): return "东南风";
                case ("4"): return "南风";
                case ("5"): return "西南风";
                case ("6"): return "西风";
                case ("7"): return "西北风";
                case ("8"): return "北风";
                case ("9"): return "旋转风";
                default: return "";
            }
        }

        private  string SloveWindPower(string code)
        {
            switch (code)
            {
                case ("0"): return "微风";
                case ("1"): return "3-4级";
                case ("2"): return "4-5级";
                case ("3"): return "5-6级";
                case ("4"): return "6-7级";
                case ("5"): return "7-8级";
                case ("6"): return "8-9级";
                case ("7"): return "9-10级";
                case ("8"): return "10-11级";
                case ("9"): return "11-12级";
                default: return "";
            }
        }
        public  string SloveWeather(string code)
        {
            switch (code)
            {
                case ("00"): return "晴";
                case ("01"): return "多云";
                case ("02"): return "阴";
                case ("03"): return "阵雨";
                case ("04"): return "雷阵雨";
                case ("05"): return "雷阵雨伴有冰雹";
                case ("06"): return "雨夹雪";
                case ("07"): return "小雨";
                case ("08"): return "中雨";
                case ("09"): return "大雨";
                case ("10"): return "暴雨";
                case ("11"): return "大暴雨";
                case ("12"): return "特大暴雨";
                case ("13"): return "阵雪";
                case ("14"): return "小雪";
                case ("15"): return "中雪";
                case ("16"): return "大雪";
                case ("17"): return "暴雪";
                case ("18"): return "雾";
                case ("19"): return "冻雨";
                case ("20"): return "沙尘暴";
                case ("21"): return "小到中雨";
                case ("22"): return "中到大雨";
                case ("23"): return "大到暴雨";
                case ("24"): return "暴雨到大暴雨";
                case ("25"): return "大暴雨到特大暴雨";
                case ("26"): return "小到中雪";
                case ("27"): return "中到大雪";
                case ("28"): return "大到暴雪";
                case ("29"): return "浮尘";
                case ("30"): return "扬沙";
                case ("31"): return "强沙尘暴";
                case ("53"): return "霾";
                case ("99"): return "无";
                default: return "暂时无法获取";
            }
        }
    }
}
