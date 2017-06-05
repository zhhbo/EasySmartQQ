using System.Collections.Generic;
using Newtonsoft.Json.Serialization;
using Newtonsoft.Json;
namespace Easy.QQRob.Data.SmartQQ
{
    public class PollMessage
    {
        [JsonProperty("retcode")]
       ////状态码
        public int RetCode;    
        [JsonProperty("errmsg")]
        /// 错误信息
        public string ErrMsg;  
        [JsonProperty("t")]
        public string T;        //被迫下线说明
        [JsonProperty("p")]
        public string P;        //需要更换的ptwebqq
        [JsonProperty("result")]
        public List<PollDetailResult> Result;


    }
    public class PollDetailResult
    {
        [JsonProperty("poll_type")]
        public string PollType;
        [JsonProperty("value")]
        public PollValue Value;

    }
    public class PollValue
    {
        [JsonProperty("content")]
        //收到消息
        public List<object> Content;
        [JsonProperty("from_uin")]
        public long FromUin;
        [JsonProperty("time")]
        public string Time;
        [JsonProperty("send_uin")]
        //群消息有send_uin，为特征群号；info_seq为群号
        public long SendUin;
        //public string info_seq;              
        //上线提示
        //public string uin;
        //public string status;
        //异地登录
        [JsonProperty("reason")]
        public string Reason;
        //临时会话
        //public string id;
        //public string ruin;
        //public string service_type;
        //讨论组
        [JsonProperty("did")]
        public long Did;
    }
}
