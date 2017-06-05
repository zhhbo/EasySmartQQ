using System.Collections.Generic;
using Newtonsoft.Json.Serialization;
using Newtonsoft.Json;
namespace Easy.QQRob.Data.SmartQQ
{
    public class GroupMessageModel
    {            /* 提取群屏蔽的字段
             * 0: 接收消息; 1: 接收不提示消息; 2: 完全阻止群消息
             */
        [JsonProperty("retcode")]
        public int RetCode;
        [JsonProperty("result")]
        public GroupNameResult Result;

    }
    public class GroupNameResult
    {
        [JsonProperty("gnamelist")]
        public List<GroupName> GroupNames;

    }
    public class GroupName
    {
        [JsonProperty("flag")]
        public string Flag;
        [JsonProperty("gid")]
        public long Gid;
        [JsonProperty("code")]
        public long Code;
        [JsonProperty("name")]
        public string Name;
    }
}
