using System.Collections.Generic;
using Newtonsoft.Json;
using Easy.Data.Attributes;
namespace Easy.QQRob.Models
{
    /// <summary>
    /// 讨论组资料类
    /// </summary>
    [TableMap("DiscussInfomation")]
    public class DiscussInfo//:IMessageable
    {
        [PrimaryKey("Id", typeof(int), "Id", typeof(int))]
        public int Id { get; set; }
        [PropertyFieldMap("QQNum", typeof(long), "QQNum", typeof(long))]
        public long QQNum { get; set; }
        [PropertyFieldMap("Did", typeof(long), "Did", typeof(long))]
        public long Did { get; set; }
        [PropertyFieldMap("Name", typeof(string), "Name", typeof(string))]
        public string Name { get; set; }
        [PropertyFieldMap("State", typeof(string), "State", typeof(string))]
        public string State { get; set; } = "{}";
        [PropertyFieldMap("StillIn", typeof(bool), "StillIn", typeof(bool))]
        public bool StillIn { get; set; } = true;
        public Dictionary<string, DiscussMemberInfo> Members = new Dictionary<string, DiscussMemberInfo>();
        private Dictionary<string, object> _values;
        private Dictionary<string, object> state
        {
            get
            {
                if (_values == null)
                {
                    _values = JsonConvert.DeserializeObject<Dictionary<string, object>>(State);
                }
                return _values;
            }
        }
        public T GetState<T>(string key)
        {
            if (state == null)
            {
                return default(T);
            }
            if (state.ContainsKey(key))
                return (T)state[key];
            return default(T);
        }
        public void SetState(string key, bool value)
        {
            if (state.ContainsKey(key) == true)
            {
                state[key] = value;
            }
            else
            {

                state.Add(key, value);
            }
            State = JsonConvert.SerializeObject(state);
        }
        // public string Messages = "";
    }
    /// <summary>
    /// 
    /// </summary>
    [TableMap("DiscussMemberInfo")]
    public class DiscussMemberInfo
    {
        [PrimaryKey("Id", typeof(int), "Id", typeof(int))]
        public int Id { get; set; }
        [PropertyFieldMap("QQNum", typeof(long), "QQNum", typeof(long))]
        public long QQNum { get; set; }
        [PropertyFieldMap("Did", typeof(long), "Did", typeof(long))]
        public long Did { get; set; }
        [PropertyFieldMap("Uin", typeof(long), "Uin", typeof(long))]
        public long Uin { get; set; }
        [PropertyFieldMap("Nick", typeof(string), "Nick", typeof(string))]
        public string Nick { get; set; }
        [PropertyFieldMap("Status", typeof(string), "Status", typeof(string))]
        public string Status { get; set; }
        [PropertyFieldMap("ClientType", typeof(int), "ClientType", typeof(int))]
        public int ClientType { get; set; }
        [PropertyFieldMap("RealQQ", typeof(long), "RealQQ", typeof(long))]
        public long RealQQ { get; set; }

    }
}
