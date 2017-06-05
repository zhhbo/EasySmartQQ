using Newtonsoft.Json.Serialization;
using Newtonsoft.Json;
namespace Easy.QQRob.Data.SmartQQ
{
    public class FriendDetail
    {
        [JsonProperty("retcode")]
        public int RetCode;
        [JsonProperty("result")]
        public FriendDetailInfoResult Result;

    }
    public class FriendDetailInfoResult
    {
        [JsonProperty("allow")]
        public int Allow;

        [JsonProperty("birthday")]
        public Birthday BirthDay;
        [JsonProperty("blood")]
        public int Blood;
        [JsonProperty("city")]
        public string City;
        [JsonProperty("client_type")]
        public int ClientType;
        [JsonProperty("college")]
        public string College;
        [JsonProperty("constel")]
        public int Constel;
       [JsonProperty("country")]
        public string Country;
        [JsonProperty("email")]
        public string Email;
        [JsonProperty("face")]
        public int Face;
        [JsonProperty("gender")]
        public string Gender;
        [JsonProperty("homepage")]
        public string Homepage;
        [JsonProperty("mobile")]
        public string Mobile;
        [JsonProperty("nick")]
        public string Nick;
        [JsonProperty("occupation")]
        public string Occupation;
        [JsonProperty("personal")]
        public string Personal;
        [JsonProperty("phone")]
        public string Phone;
        [JsonProperty("province")]
        public string Province;

        [JsonProperty("shengxiao")]
        public int ShengXiao;
        [JsonProperty("stat")]
        public int Stat;
        [JsonProperty("vip_info")]
        public int VipInfo;
        [JsonProperty("uin")]
        public string Uin;
    }
    public class Birthday
    {
        [JsonProperty("month")]
        public int Month;
        [JsonProperty("year")]
        public int Year;
        [JsonProperty("day")]
        public int Day;
    }
}
