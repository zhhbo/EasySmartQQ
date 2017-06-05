using System.Collections.Generic;
using Newtonsoft.Json;
namespace Easy.QQRob.Models
{
    using Easy.Data.Attributes;

    /// <summary>
    /// 群资料类
    /// </summary>
    [TableMap("GroupInfomation")]
    public class GroupInfo
    {
        [PrimaryKey("Id", typeof(int), "Id", typeof(int))]
        public int Id { get; set; }
        [PropertyFieldMap("StillIn", typeof(bool), "StillIn", typeof(bool))]
        public bool StillIn { get; set; } = true;
        [PropertyFieldMap("QQNum", typeof(long), "QQNum", typeof(long))]
        public long QQNum { get; set; }
        [PropertyFieldMap("GroupId", typeof(long), "GroupId", typeof(long))]
        public long GroupId { get; set; }
        [PropertyFieldMap("Name", typeof(string), "Name", typeof(string))]
        public string Name { get; set; }
        [PropertyFieldMap("Code", typeof(long), "Code", typeof(long))]
        public long Code { get; set; }
        [PropertyFieldMap("MarkName", typeof(string), "MarkName", typeof(string))]
        public string MarkName { get; set; }
        [PropertyFieldMap("Memo", typeof(string), "Memo", typeof(string))]
        public string Memo { get; set; }
        [PropertyFieldMap("Face", typeof(int), "Face", typeof(int))]
        public int Face { get; set; }
        [PropertyFieldMap("CreateTime", typeof(long), "CreateTime", typeof(long))]
        public long CreateTime { get; set; }
        [PropertyFieldMap("Level", typeof(int), "Level", typeof(int))]
        public int Level { get; set; }
        [PropertyFieldMap("Owner", typeof(long), "Owner", typeof(long))]
        public long Owner { get; set; }
        [PropertyFieldMap("State", typeof(string), "State", typeof(string))]
        public string State { get; set; } = "{}";
        [PropertyFieldMap("Alias", typeof(string), "Alias", typeof(string))]
        public string Alias { get; set; }//
        // public GroupManager GroupManage = new GroupManager();
        public Dictionary<string, MemberInfo> MemberList = new Dictionary<string, MemberInfo>();
       private Dictionary<string, object> _values;
        private Dictionary<string, object> state
        {
            get
            {
                 if(_values==null)
                {
                    _values = JsonConvert.DeserializeObject<Dictionary<string, object>>(State);
                }
                return _values;
            }
        }
        public  T  GetState <T>(string key)
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
                 state[key]=value;
            }
            else
            {

                state.Add(key, value);
            }
            State = JsonConvert.SerializeObject(state);
        }
    }
}
