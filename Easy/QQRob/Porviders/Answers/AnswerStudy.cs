using System;
using System.Linq;
using Easy.QQRob.Models;
using Easy;
using System.Collections.Generic;
namespace Easy.QQRob.Services
{
    public  class AnswerStudy : IAutoAnswer
    {
        public int Priority { get { return 0; } }
        public  bool MustManager { get { return false; } }
      public  string SettingKey { get { return "EnableStudy"; } }
      public  string Name { get { return "学习"; } }
      public  string Description { get { return "针对学习信息的消息回答"; } }

        public bool ShowInMenu { get; } = true;
        public List<string> Template()
        {
            string template = "{0}^目标语句^替换语句";
            List<string> results = new List<string>();
            foreach (var command in CommandKey)
            {
                results.Add(string.Format(template, command));
            }
            return results;
        }
        public List<string> Example()
        {
            string template = "{0}^你好^好，大家都好";
            List<string> results = new List<string>();
           // foreach (var command in CommandKey)
            //{
                results.Add(string.Format(template, CommandKey[0]));
            //}
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
                    bool StudyFlag = true;
                    bool SuperStudy = false;
                    string[] tmp = context.Message.Split('^');
                    if (tmp.Length == 3)
                    {
                        SuperStudy = true;
                    }
                    if (tmp.Length != 3 || string.IsNullOrWhiteSpace( tmp[1]) || string.IsNullOrWhiteSpace(tmp[2]))
                    {
                        StudyFlag = false;
                        SuperStudy = false;
                            context.Alerts.AddRange(Example());
                       
                    }
                    if (SuperStudy)
                    {
                        string result = "";
                        result = AIStudy(tmp[1], tmp[2], context.FromUin, context.SendToId, true);
                       anwer= GetStudyFlagInfo(result, "@"+context.FromNick, tmp[1], tmp[2]);
                        
                        if (!string.IsNullOrWhiteSpace(anwer))
                        {context.Answers.Add(anwer);
                            var newlog = new AutoAnswerMessageLog();
                            newlog.FromUin = context.SendToId;
                            newlog.ToUin = context.FromUin;
                            newlog.MessageType = "study";
                            newlog.P1 = tmp[1];
                            newlog.P2 = tmp[2];
                            newlog.Data = anwer;
                            OrmManager.Insert(newlog);
                        }
                        return;

                    }
                    if (StudyFlag)
                    {
                        string result = "";
                        var Badwords = OrmManager.Fetch<BadWords>(x=>x.QQNum==context.CurrentQQ).Select(x=>x.Word).ToList();
                        if (Badwords.Count(x => tmp[1].Contains(x) || tmp[2].Contains(x))>0)
                            {
                            result = "ForbiddenWord";
                            anwer = "包含禁用词";
                            context.Answers.Add(anwer);
                            return;
                        }

                        if (result.Equals(""))
                            result = AIStudy(tmp[1], tmp[2], context.FromUin, context.FromUin, false);
                       anwer = GetStudyFlagInfo(result, "@" + context.FromNick, tmp[1], tmp[2]);
                        
                        if (!string.IsNullOrWhiteSpace(anwer))
                        {context.Answers.Add(anwer);
                            var newlog = new AutoAnswerMessageLog();
                            newlog.FromUin = context.SendToId;
                            newlog.ToUin = context.FromUin;                           
                            newlog.MessageType = "study";
                            newlog.P1 = tmp[1];
                            newlog.P2 = tmp[2];
                            newlog.Data = anwer;
                            OrmManager.Insert(newlog);
                        }
                    }
                }
            }
           
        }
      public string []MessageType { get { return new[] { "message", "group_message", "discu_message" }; } }
      public string[] CommandKey { get { return new[] { "学习","特权学习", }; } }
      public  AnswerCommandType CommandType { get { return AnswerCommandType.Start; } }
        /// <summary>
        /// 向服务器提交AI学习请求
        /// </summary>
        /// <param name="source">源语句</param>
        /// <param name="aim">目标语句</param>
        /// <param name="QQNum">发起学习用户的QQ</param>
        /// <param name="QunNum">发起学习的群</param>
        /// <param name="superstudy">是否为特权学习</param>
        /// <returns>用户友好的提示语</returns>
        public  string AIStudy(string source, string aim, long QQNum, long QunNum = 0, bool superstudy = false)
        {
            var study = new StudyWords();
            study.SourceQQNum = QQNum;
            study.QQNum = WorkContext.GetState<long>(Constract.CurrentQQ);
            study.GroupId = QunNum;
            study.Source = source;
            study.Aim = aim;
            if (superstudy)
            {
                study.Pass = true;
                study.Reson = "特权学习";
            }
            else
            { }
            if (OrmManager.Count<BadWords>(x=>string.Equals(x.Word,source,StringComparison.InvariantCultureIgnoreCase))>0|| OrmManager.Count<BadWords>(x => string.Equals(x.Word, aim, StringComparison.InvariantCultureIgnoreCase)) > 0)
            {
                return "ForbiddenWord";
            }
            if (OrmManager.Count<StudyWords>(x => x.SourceQQNum == QQNum && x.GroupId == QunNum && x.Source == source && x.Pass) > 0)
            {
                return "Forbidden";
            }
            else if (OrmManager.Count<StudyWords>(x => x.SourceQQNum == QQNum && x.GroupId == QunNum && x.Source == source && !x.Pass) > 0)
            {
                return "Waitting";
            }
            OrmManager.Insert(study);
            if (!study.Pass)
                return "pending";
            else
            {
                return "Success";
            }
        }

        /// <summary>
        /// 将AI学习指令的返回代码翻译为用户友好的提示语
        /// </summary>
        /// <param name="result">代码</param>
        /// <param name="QQNum">发起学习用户QQ</param>
        /// <param name="source">源语句</param>
        /// <param name="aim">目标语句</param>
        /// <returns>用户友好的提示语</returns>
        public string GetStudyFlagInfo(string result, string QQNum, string source, string aim)
        {
            switch (result)
            {
                case ("Success"): return "嗯嗯～小睿睿记住了～～" + Environment.NewLine + "主人说 " + source + " 时，小睿睿应该回答 " + aim;
                case ("Already"): return "小睿睿知道了啦～" + Environment.NewLine + "主人说 " + source + " 时，小睿睿应该回答 " + aim;
                case ("DisableStudy"): return "当前学习功能未开启";
                case ("IDDisabled"): return "小睿睿拒绝学习这句话，原因是：" + Environment.NewLine + "妈麻说，" + QQNum + " 是坏人，小睿睿不能听他的话，详询管理员。";
                case ("Waitting"): return "小睿睿记下了" + QQNum + " 提交的学习请求，不过小睿睿还得去问问语文老师呢～～主人先等等吧～～";
                case ("ForbiddenWord"): return "小睿睿拒绝学习这句话，原因是：" + Environment.NewLine + "根据相关法律法规和政策，" + QQNum + " 提交的学习内容包含敏感词，详询管理员";
                case ("Forbidden"): return "小睿睿拒绝学习这句话，原因是：" + Environment.NewLine + "" + QQNum + " 提交的学习内容被屏蔽，详询管理员";
                case ("NotSuper"): return "小睿睿拒绝学习这句话，原因是：" + Environment.NewLine + "" + QQNum + " 不是特权用户，不能使用特权学习命令。";
                case ("pending"): return "小睿睿记录下了账号" + QQNum + " 提交的学习请求，请耐心等待审核，欢迎加入小睿睿的小窝，群137777833。";
                default: return "小睿睿出错了，也许主人卖个萌就好了～～";
            }

        }
        /// <summary>
        /// 从服务器获取AI的回复
        /// </summary>
        /// <param name="message">源语句</param>
        /// <param name="QQNum">发言用户的QQ</param>
        /// <param name="QunNum">发言的群</param>
        /// <returns></returns>
        private static string AIGet(string message, long QQNum, long QunNum =0)
        {
            var study= OrmManager.Get<StudyWords>(x => x.SourceQQNum == QQNum && x.GroupId == QunNum&&x.Source==message && x.Pass);
            if (study == null)
            {
                return "";
            }
            return study.Aim;

        }
    }
}
