using System.Collections.Generic;
using Newtonsoft.Json.Serialization;
using Newtonsoft.Json;
namespace Easy.QQRob.Data.SmartQQ
{
    public class Disscuss
    {
        [JsonProperty("retcode")]
        public int RetCode;
        [JsonProperty("result")]
        public DisscussParam Result;

    }
    public class DisscussParam
    {
        [JsonProperty("dnamelist")]
        public List<DiscussNameList> DNameList;

    }
    public class DiscussNameList
    {
        [JsonProperty("did")]
        public long Did;
        [JsonProperty("name")]
        public string Name;
    }
}
