using System;
using System.Linq;
using Easy.QQRob.Models;
using Easy.QQRob.Data.Easy;
using Newtonsoft.Json;
using System.Web;
using System.Collections.Generic;
namespace Easy.QQRob.Services
{
    public class AnswerWuli : IAutoAnswer
    {
        public int Priority { get { return 0; } }
        public bool MustManager { get { return false; } }
        public string SettingKey { get { return "Enablewuli"; } }
        public string Name { get { return "查物理题"; } }
        public string Description { get { return "针对物理关键字的消息回答"; } }
        public bool ShowInMenu { get; } = true;
        public List<string> Template()
        {
            string template = " 包含 {0} 关键字";
            List<string> results = new List<string>();
            foreach (var command in CommandKey)
            {
                results.Add(string.Format(template, command));
            }
            return results;
        }
        public List<string> Example()
        {
            string template = "{0}知识点";
            List<string> results = new List<string>();
            results.Add(string.Format(template, CommandKey[0]));
            // foreach (var command in CommandKey)
            // {
            //    results.Add(string.Format(template, command));
            // }
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
            if (CommandKey.Any(x => context.Message.IndexOf(x,StringComparison.InvariantCultureIgnoreCase)>=0))
            {
                bool EnableFlag = false;
                if (context.State != null && context.GetState<bool>(SettingKey))
                {
                    EnableFlag = true;
                }
                else if (context.MessageType == "message")
                {
                    EnableFlag = true;
                }
                if (EnableFlag)
                {
                    var result = GetResult(context.Message);
                    if (result.Count() == 1 && string.IsNullOrEmpty(result[0]))
                        return;
                    else
                    {
                        context.Answers.AddRange(result);
                        //context.AnswerBySingle = true;
                        try
                        {
                            var newlog = new AutoAnswerMessageLog();
                            newlog.FromUin = context.SendToId;
                            newlog.ToUin = context.FromUin;
                            newlog.MessageType = "wuli";
                            newlog.P1 = context.Message;
                            newlog.Data = string.Join("回复了:", result);
                            OrmManager.Insert(newlog);
                        }
                        catch
                        { }
                    }
                }
            }

        }
        public string[] MessageType { get { return new[] { "message", "group_message", "discu_message" }; } }
        public string[] CommandKey { get { return new[] { "物理", "题", "3-1", "3-2", "3-3", "3-4", "3-5", "必修1", "必修2", "牛顿", "功能关系" }; } }
        public AnswerCommandType CommandType { get { return AnswerCommandType.Containt; } }
        public string []GetResult(string keyword)
        {

            string ans = "";
            string url, temp;

            url = "https://www.xtiku.cn/api/search/Get";
            var data = new ApiPostData { Page=0, Keyword =HttpUtility.UrlEncode( keyword )};
            temp = QQClient.PostJsonData(url,JsonConvert.SerializeObject(data));
            var result = (ApiContentResult)JsonConvert.DeserializeObject(temp, typeof(ApiContentResult));
            if (!result.IsSuccess)
                // return "查询 " + keyword + " 出错，提示信息："+result.Message;
                return new[] { "" };
            else if(result.Count > 0)
            {
                int takeNum = result.Contents.Count > 5 ? 5 : result.Contents.Count;
                List<string> answers = new List<string>();
                ans = "我在www.xtiku.cn上找到了和 " + keyword + "相关的前" + takeNum + "个信息，你看有用没";
               
                for (int i = 0; i < takeNum; i++)
                {
                    //#0-sqq-1-39139-9737f6f9e09dfaf5d3fd14d775bfee85

                    //answers.Add(result.Contents[i].Title + Environment.NewLine + "https://www.xtiku.cn/" + result.Contents[i].Url);
                    var  waittoadd=Environment.NewLine + result.Contents[i].Title + Environment.NewLine + "https://www.xtiku.cn/" + result.Contents[i].Url;
                    if (ans.Length + waittoadd.Length < 400)
                    {
                        ans += waittoadd;
                    }
                    else
                    {
                        answers.Add(ans);
                        ans = waittoadd;
                    }
                }
                answers.Add(ans);
                return answers.ToArray();
                // return ans;
            }
            return new[] { "" };

        }
    }
}
