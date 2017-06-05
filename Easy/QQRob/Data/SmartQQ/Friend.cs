using System.Collections.Generic;
using Newtonsoft.Json.Serialization;
using Newtonsoft.Json;
namespace Easy.QQRob.Data.SmartQQ
{
    public class Friend
    {
        [JsonProperty("retcode")]
        public int RetCode;
        [JsonProperty("result")]
        public FriendsResult Result;

    }
    public class FriendsResult
    {        /// 分组信息
        /// 
        [JsonProperty("categories")]
        public List<MyCategories> MyCategories;
        [JsonProperty("friends")]
        /// 好友汇总
        public List<FriendCategory> Friends;
        [JsonProperty("info")]
        /// 好友信息
        public List<FriendBasicInfo> BasicInfos;
        [JsonProperty("marknames")]
        /// 备注
        public List<FriendMarkNames> MarkNames;
        [JsonProperty("vipinfo")]
        /// 备注
        public List<FriendVipInfo> VipInfo;

    }
    /// 分组
    public class MyCategories
    {
        [JsonProperty("index")]
        public int Index;
        [JsonProperty("sort")]
        public int Sort;
        [JsonProperty("name")]
        public string Name;
    }
    /// 好友汇总 
    public class FriendCategory
    {
        [JsonProperty("flag")]
        public int Flag;
        [JsonProperty("uin")]
        public long Uin;
        [JsonProperty("categories")]
        public int Categories;
    }
    /// 好友信息
    public class FriendBasicInfo
    {
        [JsonProperty("face")]
        public int Face;
        [JsonProperty("nick")]
        public string Nick;
        [JsonProperty("uin")]
        public long Uin;
        [JsonProperty("flag")]
        public int Flag;
    }
    /// 备注 
    public class FriendMarkNames
    {
        [JsonProperty("uin")]
        public long Uin;
        [JsonProperty("markname")]
        public string MarkName;
    }
    /// vip信息   
    public class FriendVipInfo
    {
        [JsonProperty("vip_level")]
        public int VipLevel;
        [JsonProperty("u")]
        public long U;
        [JsonProperty("is_vip")]
        public int IsVip;
    }
}
