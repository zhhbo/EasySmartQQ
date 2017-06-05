using System;
using Easy.QQRob.Models;
using Easy;
using Newtonsoft.Json;
using Easy.QQRob.Data.Tuling;
using System.Collections.Generic;
namespace Easy.QQRob.Services
{/*
    public  class AnswerTuling : IAutoAnswer
    {
        public int Priority { get { return 0; } }
        internal static string TuLinKey;
        public  bool MustManager { get { return false; } }
      public  string SettingKey { get { return "EnableTuling"; } }
      public  string Name { get { return "图灵"; } }
      public  string Description { get { return "针对信息的消息回答"; } }
        public bool ShowInMenu { get; } = true;
        public List<string> Template()
        {
            return null;
        }
        public List<string> Example()
        {
            return null;
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
                        anwer= GetTuLin(context.Message, context.SendToId);

                        if (!string.IsNullOrEmpty(anwer))
                        {   context.Answers.Add(anwer);
                            var newlog = new AutoAnswerMessageLog();
                            newlog.FromUin = context.SendToId;
                            newlog.ToUin = context.FromUin;
                            newlog.MessageType = "tuling";
                            newlog.P1 = context.Message;
                            newlog.Data = anwer;
                            OrmManager.Insert(newlog);
                        }
                    
                }
            
            
        }
      public string []MessageType { get { return new[] { "message", "group_message", "discu_message" }; } }
      public string[] CommandKey { get { return new[] { "图灵", }; } }
      public  AnswerCommandType CommandType { get { return AnswerCommandType.Start; } }

        public static string GetTuLin(string msg, string QQNum = "NULL")
        {
            string url = "http://www.tuling123.com/openapi/api?key=#{key}&info=#{info}&userid=#{userid}".Replace("#{key}", TuLinKey).Replace("#{info}", msg).Replace("#{userid}", QQNum);
            string temp = QQClient.Get(url);
            JsonTuLinModel dat = (JsonTuLinModel)JsonConvert.DeserializeObject(temp, typeof(JsonTuLinModel));
            string MsgGet = "";
            if (dat.code == 100000)
            {
                if (dat.text.Equals(msg) || dat.text.Contains("听不懂"))
                    return "";
                MsgGet = dat.text;
            //    RuiRui.AIStudy(msg, MsgGet, "TuLin");
            }
            else if (dat.code == 302000)
            {
                for (int i = 0; i < dat.list.Count; i++)
                    MsgGet += dat.list[i].source + "：" + dat.list[i].article + Environment.NewLine;
            }
            else if (dat.code == 308000)
            {
                MsgGet = dat.list[0].name + "：" + dat.list[0].info;
            }
            else return "";
          //  for (int i = 0; i < RuiRui.Badwords.Length; i++)
          //      if (MsgGet.Contains(RuiRui.Badwords[i]))
          //          return "";
            return MsgGet;
        }
    }
*/
}
