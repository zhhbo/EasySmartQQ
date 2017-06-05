using System.Collections.Generic;
using System.Linq;
using Easy.QQRob.Services;
using System.Text.RegularExpressions;
using System;
using Easy.QQRob.Extensions;
namespace Easy.QQRob.Porviders.Answers
{
    public class AnswerID : IAutoAnswer
    {
        public int Priority { get { return 0; } }
        public bool MustManager { get { return false; } }
        public string SettingKey { get { return "EnableIDCard"; } }
        public string Name { get { return "查身份证号"; } }
        public string Description { get { return "查身份证号"; } }
        public bool ShowInMenu { get; } = false;
        public List<string> Template()
        {
            string template = "{0} 身份证号码";
            List<string> results = new List<string>();

            foreach (var command in CommandKey)
            {
                results.Add(string.Format(template, command));
            }
            return results;
        }
        public List<string> Example()
        {
            string template = "{0} 身份证号码";
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
                var reg15 = new Regex("^[1-9]\\d{7}((0\\d)|(1[0-2]))(([0|1|2]\\d)|3[0-1])\\d{3$");
                var reg18 = new Regex("^[1-9]\\d{5}[1-9]\\d{3}((0\\d)|(1[0-2]))(([0|1|2]\\d)|3[0-1])\\d{3}([0-9]|X)$");
                if (tmp.Length == 2)
                {
                    var id15 = reg15.Match(tmp[1]);
                    if (id15.Success)
                    {
                        var result = GetID(id15.Value);
                        if (result != null)
                        {
                            context.Answers.Add(string.Join(Environment.NewLine, result));
                        }
                    }
                    else
                    {
                        var id18 = reg18.Match(tmp[1]);
                        if (id18.Success)
                        {
                            var result = GetID(id18.Value);
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
                else
                {
                    context.Alerts.Add("格式不对，参考：" + Example()[0]);
                }

            }
        }
        public string[] MessageType { get { return new[] { "message", "group_message", "discu_message" }; } }
        public string[] CommandKey { get { return new[] { "身份证", }; } }
        public AnswerCommandType CommandType { get { return AnswerCommandType.Start; } }

        public static string[] GetID(string number)
        {
            try
            {
                string strSource = QQClient.Get("http://qq.ip138.com/idsearch/index.asp?action=idcard&userid=" + number.Trim() + "&B1=%B2%E9+%D1%AF", null, 100000, System.Text.Encoding.Default);
                //归属地
                strSource = strSource.Substring(strSource.IndexOf("查询结果"));
                strSource = strSource.StripHTML();
                strSource = strSource.Replace("\r", "");
                strSource = strSource.Replace("\n", "");
                strSource = strSource.Replace("\t", "");
                strSource = strSource.Replace("&nbsp;", "");
                strSource = strSource.Replace("查询结果 * ++", "");
                strSource = strSource.Replace("-->", "");
                string[] strnumber = strSource.Split(new string[] { "性别：", "出生日期：", "发证地：", "点击这里查询验证身份", "提示：" }, StringSplitOptions.RemoveEmptyEntries);
                string[] strnumber1 = null;
                if (strnumber.Length > 3)
                {
                    strnumber1 = new string[] { strnumber[1].Trim(), strnumber[2].Trim(), strnumber[3].Trim() };
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
