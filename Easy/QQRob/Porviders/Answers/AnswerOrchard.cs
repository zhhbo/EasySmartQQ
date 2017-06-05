using System;
using System.Linq;
using Easy.QQRob.Models;
using Easy.QQRob.Data.Easy;
using Newtonsoft.Json;
using System.Web;
using System.Collections.Generic;
namespace Easy.QQRob.Services
{
    public class AnswerEasy : IAutoAnswer
    {
        public int Priority { get { return 0; } }
        public bool MustManager { get { return false; } }
        public string SettingKey { get { return "EnableOrchard"; } }
        public string Name { get { return "查orchard"; } }
        public string Description { get { return "针对Easy关键字的消息回答"; } }
        public bool ShowInMenu { get; } = true;
        public List<string> Template()
        {
            string template = " 包含关键字：{0}";
            List<string> results = new List<string>();
            foreach (var command in CommandKey)
            {
                results.Add(string.Format(template, command));
            }
            return results;
        }
        public List<string> Example()
        {
            string template = "{0}使用 或 {0}开发";
            List<string> results = new List<string>();
           // foreach (var command in CommandKey)
          //  {}
                results.Add(string.Format(template, CommandKey[0]));
            
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

            //var message = context.Message.ToLower();
            if (CommandKey.Any(x => context.Message.IndexOf(x, StringComparison.InvariantCultureIgnoreCase) >= 0))
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
                    else {
                        context.Answers.AddRange(result);
                        try
                        {
                            var newlog = new AutoAnswerMessageLog();
                            newlog.FromUin = context.SendToId;
                            newlog.ToUin = context.FromUin;
                            newlog.MessageType = "orchard";
                            newlog.P1 = context.Message;
                            newlog.Data = string.Join("回复了：", result);
                            OrmManager.Insert(newlog);
                        }
                        catch
                        { }
                    }

                }
            }
        }
        public string[] MessageType { get { return new[] { "message", "group_message", "discu_message" }; } }
        public string[] CommandKey { get { return new[] { "orchard", "模块", "模板", "候补", "租户","工作流","shape", "theme", "workflow", "tenant", "module", "layout" }; } }
        public AnswerCommandType CommandType { get { return AnswerCommandType.Containt; } }
        public string[] GetResult(string keyword)
        {

            string ans = "";
            string url, temp;

            url = "http://www.orchardcn.org/api/search/Get";
            var data = new ApiPostData { Page = 0, Keyword = HttpUtility.UrlEncode(keyword) };
            temp = QQClient.PostJsonData(url, JsonConvert.SerializeObject(data));
            var result = (ApiContentResult)JsonConvert.DeserializeObject(temp, typeof(ApiContentResult));
            if (!result.IsSuccess)
                //  return "查询 " + keyword + " 出错，提示信息："+result.Message;
                return new[] { "" };
            else if (result.Count > 0)
            {
                int takeNum = result.Contents.Count > 5 ? 5 : result.Contents.Count;
                ans = "我在www.orchardcn.org上找到了和 " + keyword + "相关的前" + takeNum + "个信息，你看有用没";
                List<string> answers = new List<string>();
              //  answers.Add(ans);
                for (int i = 0; i < takeNum; i++)
                {           
                  
                    var waittoadd= Environment.NewLine + result.Contents[i].Title + Environment.NewLine + "http://www.orchardcn.org/" + result.Contents[i].Url;
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
                //return ans;
            }
            return new[] { "" };

        }
    }
}
