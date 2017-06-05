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
using Easy.QQRob.Data;
namespace Easy.QQRob.Services
{/*
   public  class AnswerExchangeRate : IAutoAnswer
    {
        public int Priority { get { return 0; } }
        public  bool MustManager { get { return false; } }
      public  string SettingKey { get { return "EnableExchangeRate"; } }
      public  string Name { get { return "查汇率"; } }
      public  string Description { get { return "针对汇率消息的消息回答"; } }
        public bool ShowInMenu { get; } = true;
        public List<string> Template()
        {
            string template = "{0} 人民币 美元";
            List<string> results = new List<string>();
            foreach (var command in CommandKey)
            {
                results.Add(string.Format(template, command));
            }
            return results;
        }
        public List<string> Example()
        {
            string template = "{0} 人民币 美元";
            List<string> results = new List<string>();
            foreach (var command in CommandKey)
            {
                results.Add(string.Format(template, command));
            }
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
                bool EableFlag = false;
                if (context.State != null && context.GetState<bool>(SettingKey))
                {
                    EableFlag = true;
                }
                else if(context.MessageType=="message"){
                    EableFlag = true;
                }
                if (EableFlag)
                {

                    string[] tmp = context.Message.Split(' ');
                    if (tmp.Length == 3)
                        anwer = GetExchangeRate(tmp[1], tmp[2]);
                    if (!string.IsNullOrEmpty(anwer))
                    {
                        context.Answers.Add(anwer);
                        var newlog = new AutoAnswerMessageLog();
                        newlog.FromUin = context.SendToId;
                        newlog.ToUin = context.FromUin;
                        newlog.MessageType = "exchangeRate";
                        newlog.P1 = tmp[1];
                        newlog.P2 = tmp[2];
                        newlog.Data = anwer;
                        OrmManager.Insert(newlog);
                    }

                }
            }
            
        }
      public string []MessageType { get { return new[] { "message", "group_message", "discu_message" }; } }
      public string[] CommandKey { get { return new[] { "汇率", }; } }
      public  AnswerCommandType CommandType { get { return AnswerCommandType.Start; } }


        public  string GetExchangeRate(string p1, string p2)
        {
            string url = "https://query.yahooapis.com/v1/public/yql?q=select%20id,Rate%20from%20yahoo.finance.xchange%20where%20pair%20in%20%28%22";
            url += p1 + p2 + "%22%29&env=store://datatables.org/alltableswithkeys&format=json";
            string temp = QQClient.Get(url, "", 100000);
            JsonYahooExchangeRateModel ExchangeRateYahoo = (JsonYahooExchangeRateModel)JsonConvert.DeserializeObject(temp, typeof(JsonYahooExchangeRateModel));
            if (!ExchangeRateYahoo.query.results.rate.Rate.Equals("N/A"))
            {
                return "根据Yahoo!的信息，" + ExchangeRateYahoo.query.results.rate.id + "在UTC" + ExchangeRateYahoo.query.created + "的汇率是：" + ExchangeRateYahoo.query.results.rate.Rate + "。";
            }
            url = "https://www.cryptonator.com/api/ticker/" + p1 + "-" + p2;
            temp = QQClient.Get(url);
            JsonExchangeRateModel ExchangeRate = (JsonExchangeRateModel)JsonConvert.DeserializeObject(temp, typeof(JsonExchangeRateModel));
            if (ExchangeRate.success == true)
                return "根据cryptonator的信息，" + p1 + "-" + p2 + "的汇率：" + ExchangeRate.ticker.price;
            else return "Error:" + ExchangeRate.error;
        }
    }
    */
}
