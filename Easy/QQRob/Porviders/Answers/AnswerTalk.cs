using System;
using System.Linq;
using Easy.QQRob.Models;
using Easy;
using System.Collections.Generic;
namespace Easy.QQRob.Services
{
    public  class AnswerTalk : IAutoAnswer
    {
        public int Priority { get { return 0; } }
        public  bool MustManager { get { return false; } }
      public  string SettingKey { get { return "EnableTalk"; } }
      public  string Name { get { return "聊天"; } }
      public  string Description { get { return "针对学习信息的消息回答"; } }

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

                bool EnableTalkFlag = false;
                if (context.State != null && context.GetState<bool>(SettingKey))
                {
                EnableTalkFlag = true;
                }
                else if(context.MessageType=="message"){
                EnableTalkFlag = true;
                }
            bool MsgAdapterFlag = false;
            if (EnableTalkFlag)
            {

                anwer = AIGet(context.Message);
                if (!anwer.Equals(""))
                {
                    var newlog = new AutoAnswerMessageLog();
                    newlog.FromUin = context.FromUin;
                    newlog.ToUin = context.SendToId;
                    newlog.MessageType = "talk";
                    newlog.P1 = context.Message;
                    newlog.Data = anwer;
                    OrmManager.Insert(newlog);
                    context.Answers.Add(anwer);
                    return;
                }
                string[] tmp1 = context.Message.Split("@#$(),，.。:：;^&；“”～~！!#（）%？?》《、· \r\n\"".ToCharArray());
                bool RepeatFlag = false;
                for (int i = 0; i < tmp1.Length && i < 10; i++)
                {
                    if (tmp1[i].Equals(context.Message))
                        continue;
                    for (int k = 0; k < i; k++)
                        if (tmp1[k].Equals(tmp1[i]))
                            RepeatFlag = true;
                    if (RepeatFlag)
                    {
                        RepeatFlag = false;
                        continue;
                    }
                    if (!tmp1[i].Equals(""))
                    {
                        context.Answers.Add(AIGet(tmp1[i]));
                        MsgAdapterFlag = true;
                    }
                }
                if (!MsgAdapterFlag)
                {
                    string[] tmp2 = context.Message.Split("@#$(),，.。:：;^&；“”～~！!#（）%？?》《、· \r\n\"啊喔是的么吧呀恩嗯了呢很吗".ToCharArray());
                    RepeatFlag = false;
                    for (int i = 0; i < tmp2.Length && i < 10; i++)
                    {
                        if (tmp2[i].Equals(context.Message))
                            continue;
                        for (int k = 0; k < i; k++)
                            if (tmp2[k].Equals(tmp2[i]))
                                RepeatFlag = true;
                        for (int k = 0; k < tmp1.Length; k++)
                            if (tmp1[k].Equals(tmp2[i]))
                                RepeatFlag = true;

                        if (RepeatFlag)
                        {
                            RepeatFlag = false;
                            continue;
                        }
                        if (!tmp2[i].Equals(""))
                        {
                            context.Answers.Add(AIGet(tmp2[i]));
                            MsgAdapterFlag = true;
                        }
                    }
                }


            }
        }
      public string []MessageType { get { return new[] { "message", "group_message", "discu_message" }; } }
      public string[] CommandKey { get { return new[] { "", }; } }
      public  AnswerCommandType CommandType { get { return AnswerCommandType.Start; } }

        /// <summary>
        /// 从服务器获取AI的回复
        /// </summary>
        /// <param name="message">源语句</param>
        /// <param name="QQNum">发言用户的QQ</param>
        /// <param name="QunNum">发言的群</param>
        /// <returns></returns>
        private static string AIGet(string message)
        {
            var study= OrmManager.Get<StudyWords>(x => x.Source==message);
            if (study == null)
            {
                return "";
            }
            else
            {
                if (!study.Pass)
                    return "None3";
            }
            return study.Aim;

        }
    }
}
