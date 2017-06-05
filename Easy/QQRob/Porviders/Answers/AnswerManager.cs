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
   public  class AnswerManager : IAutoAnswer
    {
        public int Priority { get { return 0; } }
        public  bool MustManager { get { return true; } }
      public  string SettingKey { get { return "Enable"; } }
      public  string Name { get { return "群管理"; } }
      public  string Description { get { return "针对群管理的消息回答"; } }

        public bool ShowInMenu { get; } = true;
        public List<string> Template()
        {
            string template = "{0} 启动/关闭/状态 功能名称";
            List<string> results = new List<string>();
           
            foreach (var command in CommandKey)
            {
                results.Add(string.Format(template, command));
            }
            return results;
        }
        public List<string> Example()
        {
            string template = "{0} 启动 查天气";
            List<string> results = new List<string>();
           // foreach (var command in CommandKey)
           // {
                results.Add(string.Format(template, CommandKey[0]));
            //}
            return results;
        }
        public bool CanAnswer(AnswerContext context)
        {
            if (context.State != null && context.GetState<bool>(SettingKey))
            { return true; }
            return false;
        }
        public void Answer(AnswerContext context)
        {
            var anwer = "";
            if (CommandKey.Any(x => context.Message.StartsWith(x)) && context.IsManager)
            {
                bool EnableFlag = false;
                if (context.State != null && context.GetState<bool>(SettingKey))
                {
                    EnableFlag = true;
                }
                if (EnableFlag)
                {
                    var groupInfo = OrmManager.Get<GroupInfo>(x => x.QQNum == context.CurrentQQ && x.GroupId == context.SendToId);
                    context.SmartService.InitGroupInfo(context.SendToId);
                   var  adminuin = groupInfo.Owner;
                    var memberInfo = OrmManager.Get<MemberInfo>(x => x.QQNum == context.CurrentQQ && x.GroupId ==context.SendToId && x.Uin == context.FromUin);
                    bool HaveRight = false;
                    if (context.FromUin.Equals(adminuin) )//|| context.SmartService.InitRealQQ(context.FromUin).Equals(MasterQQ)
                        HaveRight = true;
                    else if (memberInfo != null && memberInfo.IsManager)
                        HaveRight = true;
                    else HaveRight = false;
                    string[] tmp = context.Message.Trim().Split(' ');
                    if (tmp.Length == 1)
                    {

                            context.Alerts.AddRange(Example());
                        

                        return;
                    }
                    tmp[1] = tmp[1].Replace("\r", "").Replace("\n", "").Replace(" ", "");
                    if (tmp.Length < 2 || tmp[1] == null)
                        return;
                    if ((HaveRight ) && (tmp[1].Equals("查询状态") || tmp[1].Equals("状态")))
                    {
                        string enableFormate = "{0}已经启动,关闭命令：群管理 关闭 {0}";
                        string disableFormate = "{0}暂未启动,启动命令：群管理 启动 {0}";
                        foreach (var answer in context.SmartService.GetAnswers().Except(new[] { this }))
                        {
                            if (context.GetState<bool>(answer.SettingKey))
                            {
                                context.Answers.Add(string.Format(enableFormate, answer.Name));
                            }
                            else
                            {
                                context.Answers.Add(string.Format(disableFormate, answer.Name));
                            }
                        }
                        return;
                    }
                    if (HaveRight == false)
                    {
                        if (context.FromRealQQ!=0)
                        {
                            anwer = "用户" + context.FromNick + "不是群管理，无权进行此操作";
                        }
                        else
                        {
                            anwer =  context.FromNick + "不是群管理，无权进行此操作";
                        }
                        context.Answers.Add(anwer);
                        return;
                    }
                    else
                    {
                        tmp[1] = tmp[1].Replace("开启", "启动");
                        tmp[1] = tmp[1].Replace("开起", "启动");

                        if (tmp.Length !=3 || tmp[2] == null)
                            return;
                        string enableFormate = "{0}已经启动,关闭命令：群管理 关闭 {0}";
                        string disableFormate = "{0}已经关闭,启动命令：群管理 启动 {0}";
                        string NoThisFormate = "没有这个 {0} 答话机";
                        var answers = context.SmartService.GetAnswers().Except(new[] { this });
                        var answer = answers.Where(x=>x.Name==tmp[2]);
                        if (answer == null)
                        {
                            context.Answers.Add(string.Format(NoThisFormate, tmp[2]));
                            return;
                        }
                        else
                        {
                            foreach (var item in answer)
                            {
                                if (tmp[2] == item.Name)
                                {
                                    if (tmp[1] == "启动")
                                    {
                                        context.SetState(item.SettingKey, true);
                                        context.Answers.Add(string.Format(enableFormate, item.Name));
                                    }
                                    else
                                    {
                                        context.SetState(item.SettingKey, false);
                                        context.Answers.Add(string.Format(disableFormate, item.Name));
                                    }
                                }
                            }
                        }
                    }
                }
            }
            else if(CommandKey.Any(x => context.Message.StartsWith(x)) && !context.IsManager)
            {
                anwer = "你不是管理员，没有权限";
                context.Answers.Add(anwer);
            }
            
        }
      public string []MessageType { get { return new[] {  "group_message", }; } }
      public string[] CommandKey { get { return new[] { "群管理", }; } }
      public  AnswerCommandType CommandType { get { return AnswerCommandType.Start; } }



    }
}
