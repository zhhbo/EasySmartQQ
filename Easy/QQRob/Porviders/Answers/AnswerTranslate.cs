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
using Easy.QQRob.Data.Youdao;
namespace Easy.QQRob.Services
{/*
   public  class AnswerTranslate : IAutoAnswer
    {
        public int Priority { get { return 0; } }
        internal static string YoudaoKeyform;
        internal static string YoudaoKey;
        public  bool MustManager { get { return false; } }
      public  string SettingKey { get { return "EnableTranslate"; } }
      public  string Name { get { return "翻译"; } }
      public  string Description { get { return "针对翻译信息的消息回答"; } }
        public bool ShowInMenu { get; } = true;
        public List<string> Template()
        {
            string template = "{0} 待翻译内容";
            List<string> results = new List<string>();
            foreach (var command in CommandKey)
            {
                results.Add(string.Format(template, command));
            }
            return results;
        }
        public List<string> Example()
        {
            string template = "{0} good";
            List<string> results = new List<string>();
            foreach (var command in CommandKey)
            {
                results.Add(string.Format(template, command));
            }
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
                        anwer = GetTranslate(tmp[1]);

                    if (!string.IsNullOrEmpty(anwer))
                    {
                        context.Answers.Add(anwer);
                        var newlog = new AutoAnswerMessageLog();
                        newlog.FromUin = context.SendToId;
                        newlog.ToUin = context.FromUin;
                        newlog.MessageType = "translate";
                        newlog.P1 = tmp[1];
                       // newlog.P2 = anwer;
                        newlog.Data = anwer;
                        OrmManager.Insert(newlog);
                    }

                }
            }
            
        }
      public string []MessageType { get { return new[] { "message", "group_message", "discu_message" }; } }
      public string[] CommandKey { get { return new[] { "翻译", }; } }
      public  AnswerCommandType CommandType { get { return AnswerCommandType.Start; } }


        public  string GetTranslate(string str)
        {
            string messagetosend = "原文：" + str;
            int strLen = str.Length;
            int bytLeng = System.Text.Encoding.UTF8.GetBytes(str).Length;
            string url;

            url = "http://fanyi.youdao.com/openapi.do?keyfrom=" + YoudaoKeyform + "&key=" + YoudaoKey + "&type=data&doctype=json&version=1.1&q=" + str;
            string temp = QQClient.Get(url);
            JsonYoudaoTranslateModel dat = (JsonYoudaoTranslateModel)JsonConvert.DeserializeObject(temp, typeof(JsonYoudaoTranslateModel));
            if (dat.errorcode == 0)
            {
                if (dat.translation[0] != null)
                    messagetosend = messagetosend + Environment.NewLine + "有道翻译：" + dat.translation[0];
            }
            else if (dat.errorcode == 20)
                messagetosend = messagetosend + Environment.NewLine + "有道翻译：不支持或文本过长";
            else if (dat.errorcode == 50)
                messagetosend = messagetosend + Environment.NewLine + "有道翻译：有道API密钥错误";

           // for (int i = 0; i < RuiRui.Badwords.Length; i++)
            //    if (messagetosend.Contains(RuiRui.Badwords[i]))
             //   {
            //        messagetosend = messagetosend.Replace(RuiRui.Badwords[i], "***");
             //   }
            return messagetosend;
        }
    }
    */
}
