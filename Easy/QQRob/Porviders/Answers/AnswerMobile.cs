using System.Collections.Generic;
using System.Linq;
using Easy.QQRob.Services;
using System.Text.RegularExpressions;
using System;
using Easy.QQRob.Extensions;
namespace Easy.QQRob.Porviders.Answers
{
    public class AnswerMobile : IAutoAnswer
    {
        public int Priority { get { return 0; } }
        public bool MustManager { get { return false; } }
        public string SettingKey { get { return "EnableMobile"; } }
        public string Name { get { return "查手机"; } }
        public string Description { get { return "查手机信息"; } }
        public bool ShowInMenu { get; } = false;
        public List<string> Template()
        {
            string template = "{0} 手机号码";
            List<string> results = new List<string>();

            foreach (var command in CommandKey)
            {
                results.Add(string.Format(template, command));
            }
            return results;
        }
        public List<string> Example()
        {
            string template = "{0} 手机号码";
            List<string> results = new List<string>();
            //foreach (var command in CommandKey)
          //  {
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
            if (CommandKey.Any(x => context.Message.StartsWith(x)))
            {
                string[] tmp = context.Message.Split(' ');
                var reg = new Regex("([0-9]{7,11})");
                if (tmp.Length == 2)
                {
                    var phone = reg.Match(tmp[1]);
                    if (phone.Success)
                    {
                        var result = GetPhone(phone.Value);
                        if (result != null)
                        {
                            context.Answers.Add(string.Join("，", result));
                        }
                    }
                    else
                    {
                        context.Alerts.Add("格式不对，参考：" + Example()[0]);
                    }
                }
            }

        }
        public string[] MessageType { get { return new[] { "message", "group_message", "discu_message" }; } }
        public string[] CommandKey { get { return new[] { "手机", }; } }
        public AnswerCommandType CommandType { get { return AnswerCommandType.Start; } }

        public static string[] GetPhone(string number)
        {
            try
            {
                string strSource = QQClient.Get("http://www.ip138.com:8080/search.asp?mobile=" + number + "&action=mobile",null,100000,System.Text.Encoding.Default);
                //归属地
                strSource = strSource.Substring(strSource.IndexOf(number));
                strSource=strSource.StripHTML();
                strSource = strSource.Replace("\r", "");
                strSource = strSource.Replace("\n", "");
                strSource = strSource.Replace("\t", "");
                strSource = strSource.Replace("&nbsp;", "");
                strSource = strSource.Replace("-->", "");
                string[] strnumber = strSource.Split(new string[] { "归属地", "卡类型", "邮 编", "区 号", "更详细", "卡号" }, StringSplitOptions.RemoveEmptyEntries);
                string[] strnumber1 = null;
                if (strnumber.Length > 4)
                {
                    strnumber1 = new string[] { strnumber[1].Trim(), strnumber[2].Trim(), strnumber[3].Trim(), strnumber[4].Trim() };
                }
                return strnumber1;
            }
            catch (Exception)
            {
                return null;
            }
        }

    }
}
