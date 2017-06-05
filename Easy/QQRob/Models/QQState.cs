using Newtonsoft.Json;
using System.Collections.Generic;
using Easy.QQRob.Services;
using Easy.Data.Attributes;
namespace Easy.QQRob.Models

{
    [TableMap("QQState")]
    public class QQState
    {
        [PrimaryKey("Id", typeof(int), "Id", typeof(int))]
        public int Id { get; set; }
        [PropertyFieldMap("QQNum", typeof(long), "QQNum", typeof(long))]
        public long QQNum { get; set; }
        [PropertyFieldMap("State", typeof(string), "State", typeof(string))]
        public string State { get; set; } = "{}";
        [PropertyFieldMap("Cookies", typeof(string), "Cookies", typeof(string))]
        public string Cookies { get; set; } = "{}";
        [PropertyFieldMap("Logined", typeof(bool), "Logined", typeof(bool))]
        public bool Logined { get; set; }
        public SmartQQClient GetClient()
        {
            return ClientManager.GetClientManagerUser(QQNum)?.Client;
        }
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
        public void SetState<T>(string key, T value)
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

        private Dictionary<string, object> _cookies;
        private Dictionary<string, object> cookies
        {
            get
            {
                if (_cookies == null)
                {
                    _cookies = JsonConvert.DeserializeObject<Dictionary<string, object>>(Cookies);
                }
                return _cookies;
            }
        }
        public T GetCookies<T>(string key)
        {
            if (cookies == null)
            {
                return default(T);
            }
            if (cookies.ContainsKey(key))
                return (T)cookies[key];
            return default(T);
        }
        public void SetCookies<T>(string key, T value)
        {
            if (cookies.ContainsKey(key) == true)
            {
                cookies[key] = value;
            }
            else
            {

                cookies.Add(key, value);
            }
            Cookies = JsonConvert.SerializeObject(cookies);
        }
    }
}
