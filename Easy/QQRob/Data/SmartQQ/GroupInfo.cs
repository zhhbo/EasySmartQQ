using System.Collections.Generic;
using Newtonsoft.Json.Serialization;
using Newtonsoft.Json;
namespace Easy.QQRob.Data.SmartQQ
{
    public class GroupDetailInfo
    {
        [JsonProperty("retcode")]
        public int RetCode;
        [JsonProperty("result")]
        public GroupDetailResult Result;

    }
    public class GroupDetailResult
    {
        [JsonProperty("minfo")]
        public List<GroupMemberInfo> Members;
        [JsonProperty("cards")]
        public List<GroupCard> Cards;
        [JsonProperty("ginfo")]
        public GroupDetail GroupDetail;
        [JsonProperty("stats")]
        public List<GroupMemberStatus> Status;
        [JsonProperty("vipinfo")]
        public List<GroupMemberVipInfo> VipInfo;


    }
    public class GroupMemberInfo
    {      [JsonProperty("city")]
        public string City;
        [JsonProperty("country")]
        public string Country;
        [JsonProperty("gender")]
        public string Gender;
        [JsonProperty("nick")]
        public string Nick;
        [JsonProperty("province")]
        public string Province;
        [JsonProperty("uin")]
        public long Uin;

  
    }
    public class GroupCard
    {
        [JsonProperty("muin")]
        public long MUin;
        [JsonProperty("card")]
        public string Card;
    }
    public class GroupDetail
    {
        [JsonProperty("class")]
        public string Class;
        [JsonProperty("code")]
        public long Code;
        [JsonProperty("createtime")]
        public long CreateTime;
       [JsonProperty("face")]
        public int Face;
        [JsonProperty("fingermemo")]
        public string FingerMemo;
        [JsonProperty("flag")]
        public string Flag;
 
        [JsonProperty("gid")]      
        public long Gid;
        [JsonProperty("level")]
        public int Level;

        [JsonProperty("members")]
        public List<GroupMember> Members;
     [JsonProperty("memo")]
        public string Memo;
        [JsonProperty("name")]
        public string Name;

        [JsonProperty("owner")]
        public long Owner;

 
   
        [JsonProperty("markname")]
        public string MarkName;



    }

    public class GroupMember
    {
        [JsonProperty("muin")]
        public long MemberUin;
        [JsonProperty("mflag")]
        public int MemberFlag;
    }
    public class GroupMemberVipInfo
    {
        [JsonProperty("is_vip")]
        public bool IsVip;
        [JsonProperty("u")]
        public long Uin;
        [JsonProperty("vip_level")]
        public int VipLevel;
    }
    public class GroupMemberStatus
    {
        [JsonProperty("uin")]
        public long Uin;
        [JsonProperty("stat")]
        public string Status;
        [JsonProperty("client_type")]
        public int ClientType;
    }
}
    
