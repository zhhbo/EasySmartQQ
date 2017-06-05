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
{
   public  class FilterOriginMessageBadWords : IAutoAnswer
    {
        public int Priority { get { return int.MaxValue; } }
        public  bool MustManager { get { return false; } }
      public  string SettingKey { get { return "EnableBadWords"; } }
      public  string Name { get { return "禁用词过滤"; } }
      public  string Description { get { return "针对消息中的禁用词过滤"; } }

        public bool ShowInMenu { get; } = false;
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
            if (context.State != null && context.GetState<bool>(SettingKey))
            { return true; }
            return false;
        }
        public void Answer(AnswerContext context)
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
                    var words= OrmManager.Fetch<BadWords>(x => x.QQNum == context.CurrentQQ && context.Message.Contains(x.Word)).Select(x=>x.Word);
                    if (words == null||words.Count()==0)
                        return;
                    else
                    {
                        var template = "{0} 含有禁用词 {1},提出警告";
                        foreach (var word in words)
                        {
                            context.Alerts.Add(string.Format(template,context.FromUin,word));
                        }

                        var newlog = new AutoAnswerMessageLog();

                        newlog.FromUin = context.SendToId;
                        newlog.ToUin = context.FromUin;
                        newlog.MessageType = "badword";
                        newlog.P1 = string.Join(",",words);                      
                    newlog.Data = context.Message;
                    OrmManager.Insert(newlog);
                        context.CanAnswer = false;
                    }
                }
            

        }
      public string []MessageType { get { return new[] { "message", "group_message", "discu_message" }; } }
      public string[] CommandKey { get { return new[] { "", }; } }
      public  AnswerCommandType CommandType { get { return AnswerCommandType.Containt; } }

    }
}
