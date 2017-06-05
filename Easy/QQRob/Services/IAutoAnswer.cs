using System.Collections.Generic;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
namespace Easy.QQRob.Services
{
    public  interface IAutoAnswer:IEventHandler
    {
        /// <summary>
        /// 答话优先级，由高到低排序进行
        /// </summary>
        int Priority { get; }
        /// <summary>
        /// 要求管理员才能启动的标识 没什么用
        /// </summary>
        bool MustManager { get; }
        bool ShowInMenu { get; }
        /// <summary>
        /// 设置的关键字
        /// </summary>
        string SettingKey { get; }
        /// <summary>
        /// 答话机名字
        /// </summary>
        string Name { get; }
        /// <summary>
        /// 答话机描述
        /// </summary>
        string Description { get; }
       bool CanAnswer(AnswerContext context);
       /// <summary>
       /// 答话程序
       /// </summary>
       /// <param name="context"></param>
        void Answer(AnswerContext context);
        /// <summary>
        /// 可处理消息类型 没什么 用
        /// </summary>
        string[] MessageType { get; }
        /// <summary>
        /// 命令关键字 没什么用
        /// </summary>
        string [] CommandKey { get; }
        /// <summary>
        /// 使用该答话机的模板
        /// </summary>
        List<string> Template();
        List<string> Example();
        /// <summary>
        /// 命令类型 没什么 用
        /// </summary>
        AnswerCommandType CommandType { get; }
    }
    public enum AnswerCommandType
    {
        /// <summary>
        /// 以此开始
        /// </summary>
        Start,
        /// <summary>
        /// 包含
        /// </summary>
        Containt,
        /// <summary>
        /// 提醒类应答
        /// </summary>
        Alert,
        /// <summary>
        /// 过滤信息
        /// </summary>
        Filter,
        /// <summary>
        /// 无条件应答
        /// </summary>
        None

        
    }

    public class AnswerContext
    {
        public int Type { get; set; }
        public string Message { get; set; }
        public long FromUin { get; set; }
        public string FromNick { get; set; }
        public long FromRealQQ { get; set; }
       // public string FromGroupId { get; set; }
       // public string FromDId { get; set; }
        public long SendToId { get; set; }
        public long CurrentQQ { get; set; }
        public bool CanAnswer { get; set; } = true;
        public bool IsManager { get; set; } = false;
        public bool AnswerBySingle { get; set; } = false;
        public string MessageType { get; set; } = "message";
        public SmartQQClient SmartService { get; set; }
        public bool IsStateChanged { get; private set; } = false;
        // public GroupManager Setting { get; set; }
        public string State { get; set; }
        private dynamic state
        {
            get { return JsonConvert.DeserializeObject<dynamic>(State); }
        } //= 
         public T GetState<T>(string key)
        {
            if (state == null)
            {
                return default(T);
            }

            var value = state[key];
            return value != null ? value.ToObject<T>() : default(T);
        }
        public void SetState<T>(string key, T value)
        {
            state[key] = JToken.FromObject(value);
            IsStateChanged = true;
        }
        public object GetState(string key)
        {
            if (state == null)
            {
                return null;
            }

            return state[key];
        }
        public string QQNum { get; set; }
        public List<string> Answers { get; } = new List<string>();
        public List<string> Alerts { get; } = new List<string>();
    }
}
