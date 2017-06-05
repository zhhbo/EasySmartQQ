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
using Easy.QQRob.Data.Wiki;
using System.Web;
namespace Easy.QQRob.Services
{
   public  class AnswerWiki : IAutoAnswer
    {
        public int Priority { get { return 0; } }
        public  bool MustManager { get { return false; } }
      public  string SettingKey { get { return "EnableWiki"; } }
      public  string Name { get { return "查百科"; } }
      public  string Description { get { return "针对百科信息的消息回答"; } }
        public bool ShowInMenu { get; } = true;
        public List<string> Template()
        {
            string template = "{0} 关键字";
            List<string> results = new List<string>();
            foreach (var command in CommandKey)
            {
                results.Add(string.Format(template, command));
            }
            return results;
        }
        public List<string> Example()
        {
            string template = "{0} 郑州";
            List<string> results = new List<string>();
            results.Add(string.Format(template, CommandKey[0]));
            // foreach (var command in CommandKey)
            // {
            //     results.Add(string.Format(template, command));
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
            var anwer = new SearchResult();
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
                    string[] tmp = context.Message.Trim().Split(' ');

                    if (tmp.Length == 2)
                    {
                        var keyword = tmp[1];
                        // anwer = GetWiki(tmp[1], "");
                        switch (tmp[0])
                        {
                            case "互动百科":
                            case "互动":
                                {
                                    string url = "http://www.baike.com/wiki/" + keyword;
                                    string temp = QQClient.Get(url);
                                    if (temp.Contains("尚未收录"))
                                    {
                                        anwer.Answer="没有找到这个词条哦～";
                                        break;
                                    }
                                    temp = temp.Replace("<meta content=\"", "&");
                                    temp = temp.Replace("\" name=\"description\">", "&");
                                    string[] result = temp.Split('&');
                                    if (!result[1].Equals(""))
                                    {
                                        anwer.Answer = result[1];
                                        anwer.Url="http://www.baike.com/wiki/" + HttpUtility.UrlEncode(keyword);
                                    }
                                    break;
                                }

                            case "维基百科":
                            case "维基":
                                {
                                    string url = "https://zh.wikipedia.org/w/api.php?action=query&prop=extracts&format=json&exsentences=2&exintro=&explaintext=&exsectionformat=plain&exvariant=zh&titles=" + keyword;
                                    string temp = QQClient.Get(url);
                                    JsonWikipediaModel temp1 = (JsonWikipediaModel)JsonConvert.DeserializeObject(temp, typeof(JsonWikipediaModel));
                                    string[] result = temp1.query.pages.ToString().Split("{}".ToCharArray());
                                    JsonWikipediaPageModel pages = (JsonWikipediaPageModel)JsonConvert.DeserializeObject("{" + tmp[2] + "}", typeof(JsonWikipediaPageModel));

                                    if (pages.extract != null)
                                    {
                                        anwer.Answer = pages.extract;
                                        anwer.Url="https://zh.wikipedia.org/wiki/" + HttpUtility.UrlEncode(keyword);
                                    }
                                    else
                                        anwer.Answer = "没有找到这个Wiki哦～";

                                    break;
                                }

                            case "百度百科":
                            case "百科":
                            default:
                                {
                                    string url = "http://wapbaike.baidu.com/item/" + keyword;
                                    string temp = QQClient.Get(url);

                                    if (temp.Contains("您所访问的页面不存在"))
                                        anwer.Answer = "没有找到这个词条哦～";
                                    if (temp.Contains("百科名片"))
                                    {
                                        temp = temp.Replace("&quot;", "");
                                        temp = temp.Replace("&", "");
                                        temp = temp.Replace("百科名片", "&");
                                        string[] result = temp.Split('&');

                                        temp = result[1];
                                        temp = temp.Replace("<p>", "&");
                                        temp = temp.Replace("</p>", "&");
                                        result = temp.Split('&');

                                        temp = result[1].Replace("</a>", "");
                                        temp = temp.Replace("<b>", "");
                                        temp = temp.Replace("</b>", "");
                                        temp = temp.Replace("<i>", "");
                                        temp = temp.Replace("</i>", "");

                                        temp = temp.Replace("<a", "&");
                                        temp = temp.Replace("\">", "&");
                                        result = temp.Split('&');

                                        temp = "";
                                        for (int i = 0; i < tmp.Length; i += 2)
                                            if ((!tmp[i].Contains("card-info")) && (!tmp[i].Contains("div class")))
                                                temp += tmp[i];
                                        if (!temp.Equals(""))
                                        {
                                            anwer.Answer = temp;
                                            anwer.Url="http://wapbaike.baidu.com/item/" + HttpUtility.UrlEncode(keyword);
                                        }
                                        else
                                            anwer.Answer = "词条 " + keyword + " 请查看http://wapbaike.baidu.com/item/" + HttpUtility.UrlEncode(keyword);
                                    }
                                    else
                                        anwer.Answer = "没有找到这个词条哦～";
                                    break;
                                }
                        }
                    }
                    else
                    {
                        context.Alerts.AddRange(Example());
                    }
                    if (!string.IsNullOrEmpty(anwer.Answer))
                    {

                        if (anwer.Answer.Length + anwer.Url.Length >= 400)
                        {
                            context.Answers.Add(anwer.Answer.Substring(0,400-anwer.Url.Length-6)+"..."+anwer.Url);
                        }
                        else
                        {
                            context.Answers.Add(anwer.Answer+anwer.Url);
                        }
                       // context.Answers.Add(anwer);
                        var newlog = new AutoAnswerMessageLog();
                        newlog.FromUin = context.SendToId;
                        newlog.ToUin = context.FromUin;
                        newlog.MessageType = "wiki";
                        newlog.P1 = tmp[1];
                        newlog.Data = anwer.Answer+anwer.Url;
                        OrmManager.Insert(newlog);
                    }
                }
            }
           
        }
      public string []MessageType { get { return new[] { "message", "group_message", "discu_message" }; } }
      public string[] CommandKey { get { return new[] { "百科", "百度百科", "互动百科", "互动", "维基百科", "维基" }; } }
      public  AnswerCommandType CommandType { get { return AnswerCommandType.Start; } }

    }
    public class SearchResult
    {
        public string Answer { get; set; } = "";
        public string Url { get; set; } = "";
    }
}
