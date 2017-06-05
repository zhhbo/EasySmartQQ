namespace Easy.QQRob.Services
{
    using System.Collections.Generic;
    using System;
    public  class FilterFace : IAutoAnswer
    {
      public  int Priority { get { return int.MinValue; } }
        public  bool MustManager { get { return false; } }
      public  string SettingKey { get { return "EnableFace"; } }
      public  string Name { get { return "表情过滤"; } }
      public  string Description { get { return "表情过滤"; } }
        public bool ShowInMenu { get; } = false;
        public List<string> Template()
        {
            return null;
        }
        public List<string> Example()
        {
            return null;
        }
         public bool CanAnswer(AnswerContext context)
        {
            if (context.State != null && context.GetState<bool>(SettingKey))
            { return true; }
            return false;
         }
        public void Answer(AnswerContext context)
        {
            if (!context.CanAnswer)
                return;
            if (context.State != null && context.GetState<bool>(SettingKey))
            {
                for(int j=0;j<context.Answers.Count;j++)
                {
                    string[] tmp = context.Answers[j].Split('{');
                     var   temp = "";
                    for (int i = 0; i < tmp.Length; i++)
                        if (!tmp[i].StartsWith("..[face"))
                            temp += ("{}" + tmp[i]);
                        else temp += tmp[i].Remove(0, 7);
                    context.Answers[j] = temp;

                    string[] filter = context.Answers[j].Split("{}".ToCharArray());

                    for (int i = 0; i < filter.Length; i++)
                        if (!filter[i].Trim().StartsWith("..[face") || !filter[i].Trim().EndsWith("].."))
                            context.Answers[j] += "\\\"" + filter[i] + "\\\",";
                        else
                            context.Answers[j] += filter[i].Replace("..[face", "[\\\"face\\\",").Replace("]..", "],");
                    context.Answers[j] = context.Answers[j].Remove(context.Answers[j].LastIndexOf(','));
                    context.Answers[j] = context.Answers[j].Replace("\r\n", "\n").Replace("\n\r", "\n").Replace("\r", "\n").Replace("\n", Environment.NewLine);


                }
            }


        }
      public string []MessageType { get { return new[] { "message", "group_message", "discu_message" }; } }
      public string[] CommandKey { get { return new[] { "表情", }; } }
      public  AnswerCommandType CommandType { get { return AnswerCommandType.Start; } }
    }
}
