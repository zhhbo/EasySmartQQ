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
   public  class FilterFainalMessageBadWords : IAutoAnswer
    {
        public int Priority { get { return int.MinValue; } }
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
                    var words= OrmManager.Fetch<BadWords>(x => x.QQNum == context.CurrentQQ).Select(x=>x.Word);
                    if (words == null||words.Count()==0)
                        return;
                    else
                    {
                        context.Answers.RemoveAll(x=>string.IsNullOrEmpty(x));
                        for (int i=0;i< context.Answers.Count;i++)
                        {                              
                                foreach (var word in words)
                                {
                                if (context.Answers[i].Contains(word))
                                {
                                    var newlog = new AutoAnswerMessageLog();
                                    newlog.FromUin = context.SendToId;
                                    newlog.ToUin = context.FromUin;
                                    newlog.MessageType = "badword";
                                    newlog.P1 = word;
                                    newlog.Data = context.Answers[i];
                                    OrmManager.Insert(newlog);
                                    context.Answers[i] = context.Answers[i].Replace(word, "***");
  
                                }                               
                              }
                        }
                    }
                }


        }
      public string []MessageType { get { return new[] { "message", "group_message", "discu_message" }; } }
      public string[] CommandKey { get { return new[] { "", }; } }
      public  AnswerCommandType CommandType { get { return AnswerCommandType.Containt; } }
    }
}
