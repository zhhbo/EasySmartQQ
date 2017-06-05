using System.Linq;
using System.Collections.Generic;
using System;
namespace Easy.QQRob.Services
{
    public  class AnswerMenu : IAutoAnswer
    {
        public int Priority { get { return 0; } }
        public  bool MustManager { get { return false; } }
      public  string SettingKey { get { return "EnableMenu"; } }
      public  string Name { get { return "看菜单"; } }
      public  string Description { get { return "菜单"; } }
        public bool ShowInMenu { get; } = false;
        public List<string> Template()
        {
            string template = "{0}";
            List<string> results = new List<string>();

            foreach (var command in CommandKey)
            {
                results.Add(string.Format(template, command));
            }
            return results;
        }
        public List<string> Example()
        {
            string template = "{0}";
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

            if (CommandKey.Any(x=>context.Message.Equals(x)))
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
                    var answer = "";
                    foreach (var ans in context.SmartService.GetAnswers().Where(x => x.ShowInMenu&&x.CanAnswer(context)))
                    {
                        var commandtext = "";
                        var exampletext = "";
                        //context.Answers.Add(ans.Name);
                        if(ans.Template()!=null)
                            commandtext = "命令格式："+Environment.NewLine + string.Join(Environment.NewLine, ans.Template());
                        //context.Answers.Add();
                        if (ans.Example() != null)
                        {
                            exampletext = "示例："+Environment.NewLine  + string.Join(Environment.NewLine, ans.Example());
                        }
                        //context.Answers.Add("示例："+string.Join(",", ans.Example()));
                        if (answer.Length + ans.Name.Length < 400)
                        {
                            answer += ans.Name+ Environment.NewLine;
                        }
                        else
                        {
                            context.Answers.Add(answer);
                            answer = ans.Name+ Environment.NewLine;
                        }
                        if (answer.Length + commandtext.Length < 400)
                        {
                            answer += commandtext;
                        }
                        else
                        {
                            context.Answers.Add(answer);
                            answer = commandtext;
                        }

                        if (answer.Length + exampletext.Length < 400)
                        {
                            answer += exampletext;
                        }
                        else
                        {
                            context.Answers.Add(answer);
                            answer = exampletext;
                        }
                    }

                    if (!string.IsNullOrEmpty(answer))
                    {
                        context.Answers.Add(answer);
                    }
                }
            }
        }
      public string []MessageType { get { return new[] { "message", "group_message", "discu_message" }; } }
      public string[] CommandKey { get { return new[] { "菜单", }; } }
      public  AnswerCommandType CommandType { get { return AnswerCommandType.Start; } }
    }
}
