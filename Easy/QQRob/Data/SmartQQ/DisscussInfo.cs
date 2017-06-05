using System.Collections.Generic;
using Newtonsoft.Json.Serialization;
using Newtonsoft.Json;
namespace Easy.QQRob.Data.SmartQQ
{
    public class DisscussInfo
    {
        [JsonProperty("retcode")]
        public int RetCode;
        [JsonProperty("result")]
        public DisscussInfoParam Result;

    }
    public class DisscussInfoParam
    {
        [JsonProperty("info")]
        public DisscussBasicInfo Info;
        [JsonProperty("mem_info")]
        public List<DisscussMember> Members;
        [JsonProperty("mem_status")]
        public List<DisscussMemberStatus> Status;



    }
    public class DisscussMember
    {
        [JsonProperty("uin")]
        public long Uin;
        [JsonProperty("nick")]
        public string Nick;
    }
    public class DisscussMemberBasicInfo
    {
        [JsonProperty("mem_uin")]
        public long Uin;
        [JsonProperty("ruin")]
        public long RealUin;
    }
    public class DisscussBasicInfo
    {
       // [JsonProperty("discu_owner")]
       // public string Owner;
        [JsonProperty("discu_name")]
        public string Name;
        [JsonProperty("did")]
        public long Disscussid;
        [JsonProperty("mem_list")]
        public List<DisscussMemberBasicInfo> MemberList;
    }
    public class DisscussMemberStatus
    {
        [JsonProperty("uin")]
        public long Uin;
        [JsonProperty("status")]
        public string Status;
        [JsonProperty("client_type")]
        public int ClientType;
    }
}
