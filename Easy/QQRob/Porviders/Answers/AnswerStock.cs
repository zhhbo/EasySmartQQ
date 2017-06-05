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
   public  class AnswerStock : IAutoAnswer
    {
        public int Priority { get { return 0; } }
        public  bool MustManager { get { return false; } }
      public  string SettingKey { get { return "EnableStock"; } }
      public  string Name { get { return "查行情"; } }
      public  string Description { get { return "针对股票信息的消息回答"; } }

        public bool ShowInMenu { get; } = true;
        public List<string> Template()
        {           
            List<string> results = new List<string>();

                results.Add("行情 上证指数");
            results.Add("行情 深证综指");
            results.Add("行情 中小板指数");
            results.Add("行情 创业板指数");
            results.Add("行情 深证成指");
            results.Add("行情 中小板综指");
            results.Add("行情 创业板综指");
            results.Add("行情 代码");
            results.Add("行情 上海 代码");
            results.Add("行情 上证 代码");
            results.Add("行情 沪市 代码");
            results.Add("行情 深圳 代码");
            results.Add("行情 深市 代码");
            results.Add("行情 深证 代码");
            results.Add("行情 创业板 代码");
            results.Add("行情 中小板 代码");
            return results;
        }
        public List<string> Example()
        {
            List<string> results = new List<string>();
            results.Add("行情 上证指数");
           // results.Add("行情 深证综指");
           // results.Add("行情 中小板指数");
           // results.Add("行情 创业板指数");
           // results.Add("行情 深证成指");
           // results.Add("行情 中小板综指");
           // results.Add("行情 创业板综指");
          //  results.Add("行情 002594");
           // results.Add("行情 上海 代码");
           // results.Add("行情 上证 代码");
           // results.Add("行情 沪市 代码");
           // results.Add("行情 深圳 代码");
           // results.Add("行情 深市 代码");
           // results.Add("行情 深证 代码");
           // results.Add("行情 创业板 代码");
            //results.Add("行情 中小板 代码");
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
              //  bool EnableFlag = false;
              //  if (context.State != null && context.GetState<bool>(SettingKey))
              //  {
              //      EnableFlag = true;
              //  }
              //  else if(context.MessageType=="message"){
              //      EnableFlag = true;
               // }
               // if (EnableFlag)
              //  {

                    string[] tmp = context.Message.Split(' ');

                if (tmp.Length == 2)
                    anwer = GetStock(tmp[1], "");
                else if (tmp.Length == 3)
                    anwer = GetStock(tmp[1], tmp[2]);
                else
                {
                    context.Alerts.AddRange(Example());
                }
                    if (!string.IsNullOrEmpty(anwer))
                    {context.Answers.Add(anwer);
                        var newlog = new AutoAnswerMessageLog();
                        newlog.FromUin = context.SendToId;
                        newlog.ToUin = context.FromUin;
                        newlog.MessageType = "stock";
                        newlog.P1 = tmp[tmp.Length - 1];
                        newlog.Data = anwer;
                        OrmManager.Insert(newlog);

                   // }
                }
            }
            
        }
      public string []MessageType { get { return new[] { "message", "group_message", "discu_message" }; } }
      public string[] CommandKey { get { return new[] { "行情", }; } }
      public  AnswerCommandType CommandType { get { return AnswerCommandType.Start; } }


        public static string GetStock(string p1, string p2 = "")
        {
            string url = "";

            p1 = p1.Replace(" ", "");
            p1 = p1.Replace("\r", "");
            p1 = p1.Replace("\n", "");
            if (!p2.Equals(""))
            {
                p2 = p2.Replace(" ", "");
                p2 = p2.Replace("\r", "");
                p2 = p2.Replace("\n", "");
            }
            switch (p1)
            {
                case ("上证指数"): url = "http://hq.sinajs.cn/list=s_sh000001"; break;
                case ("深证综指"): url = "http://hq.sinajs.cn/list=s_sz399106"; break;
                case ("中小板指数"): url = "http://hq.sinajs.cn/list=s_sz399005"; break;
                case ("创业板指数"): url = "http://hq.sinajs.cn/list=s_sz399006"; break;
                case ("深证成指"): url = "http://hq.sinajs.cn/list=s_sz399001"; break;
                case ("中小板综指"): url = "http://hq.sinajs.cn/list=s_sz399101"; break;
                case ("创业板综指"): url = "http://hq.sinajs.cn/list=s_sz399102"; break;
                default:
                    {
                        if (p1.ToCharArray()[0] == '6')
                            url = "http://hq.sinajs.cn/list=s_sh" + p1;
                        else if (p1.ToCharArray()[0] == '0' || p1.ToCharArray()[0] == '3')
                            url = "http://hq.sinajs.cn/list=s_sz" + p1;
                        else if (p1.Equals("上海") || p1.Equals("沪市") || p1.Equals("上证"))
                        {
                            url = "http://hq.sinajs.cn/list=s_sh" + p2;
                        }
                        else if (p1.Equals("深圳") || p1.Equals("深市") || p1.Equals("深证") || p1.Equals("创业板") || p1.Equals("中小板"))
                        {
                            url = "http://hq.sinajs.cn/list=s_sz" + p2;
                        }
                        else
                            return "参数错误";
                        break;
                    }
            }
            string dat = QQClient.Get(url, "", 100000, Encoding.GetEncoding("GB2312"));

            string[] tmp = dat.Split('\"');
            tmp = tmp[1].Split(',');
            if (tmp.Length == 1)
                return "参数错误";
            string ans = "根据新浪财经的信息，" + tmp[0] + "：现价，" + tmp[1] + "；涨跌" + tmp[2] + "，" + tmp[3] + "%；成交量，" + tmp[4] + "手，" + tmp[5] + "万元。";
            return ans;
        }
    }
}
