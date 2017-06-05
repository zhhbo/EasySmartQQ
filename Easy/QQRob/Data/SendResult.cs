namespace Easy.QQRob.Data
{
    using Newtonsoft.Json.Serialization;
    using Newtonsoft.Json;
    public   class SendResult
    {
        [JsonProperty("errCode")]
        public int ErrCode;
        [JsonProperty("msg")]
        public string Message;
    }
}
