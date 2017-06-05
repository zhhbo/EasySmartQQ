namespace Easy.QQRob.Data
{
    using Newtonsoft.Json.Serialization;
    using Newtonsoft.Json;
    public   class Result
    {
        [JsonProperty("retcode")]
        public int RetCode;
    }
}
