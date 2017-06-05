using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Easy.QQRob.Models;
using Easy;
using Easy.QQRob.Data.Weather;
using Easy.QQRob.Data.Yahoo;
using Newtonsoft.Json;

namespace Easy.QQRob.Services
{/*
   public  class AnswerCity:IAutoAnswer
    {
        public int Priority { get { return 0; } }
        public  bool MustManager { get { return false; } }
      public  string SettingKey { get { return "EnableCityInfo"; } }
      public  string Name { get { return "查城市"; } }
      public  string Description { get { return "针对城市信息的消息回答"; } }
       
        public List< string> Template()
        {
            string template = "{0} 地点";
            List<string> results = new List<string>();
            foreach (var command in CommandKey)
            {
                results.Add(string.Format(template, command));
            }
            return results;
        }
       public  List<string> Example()
        {
            string template = "{0} 郑州";
            List<string> results = new List<string>();
            foreach (var command in CommandKey)
            {
                results.Add(string.Format(template, command));
            }
            return results;
        }
        public bool ShowInMenu { get; } = true;
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
            
            var anwer = "";
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
                    string[] tmp = context.Message.Split(' ');

                        if (tmp.Length == 2)
                          anwer= GetCityInfo(tmp[1]);
                        else if(tmp.Length==3)
                           anwer= GetCityInfo(tmp[1]);
                    if (!string.IsNullOrEmpty(anwer))
                    {
                        context.Answers.Add(anwer);
                        var newlog = new AutoAnswerMessageLog();

                        newlog.FromUin = context.SendToId;
                        newlog.ToUin = context.FromUin;
                        newlog.MessageType = "cityinfo";
                        newlog.P1 = tmp[1];
                        newlog.Data = anwer;
                        OrmManager.Insert(newlog);
                    } 
                }
            }

           
        }
      public string []MessageType { get { return new[] { "message", "group_message", "discu_message" }; } }
      public string[] CommandKey { get { return new[] { "城市信息", }; } }
      public  AnswerCommandType CommandType { get { return AnswerCommandType.Start; } }


          string GetCityInfo(string city)
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

            string url = "https://ruiruiqq.hxlxz.com/weather.php?city=" + city + "&type=forecast";
            string temp = QQClient.Get(url);
            if (temp.Equals("NoCity"))
                return "未查询到指定城市 " + city + " 的信息";

            JsonWeatherModel weather = (JsonWeatherModel)JsonConvert.DeserializeObject(temp, typeof(JsonWeatherModel));

            ans = "城市 " + weather.c.c3 + "（" + weather.c.c2 + "） 的信息如下：" + Environment.NewLine;
            ans += "所在省市：" + weather.c.c7 + "省" + weather.c.c5 + "市" + "（" + weather.c.c6 + " " + weather.c.c4 + "）" + Environment.NewLine;
            ans += "区号：" + weather.c.c11 + "，邮编：" + weather.c.c12 + "。城市级别：" + weather.c.c10 + "级城市" + Environment.NewLine;
            ans += "经度：" + weather.c.c13 + "，纬度：" + weather.c.c14 + "，海拔：" + weather.c.c15 + "。雷达站" + weather.c.c16;
            return ans;
        }
    }
    */
}
