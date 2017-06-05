using System.Collections.Generic;
using Newtonsoft.Json.Serialization;
using Newtonsoft.Json;
namespace Easy.QQRob.Data.Qun
{

    public  class JsonFriend
    {
        /* /cgi-bin/qun_mgr/get_friend_list
        {"ec":0,
        "result":
        {
            "0":{"mems":[  {"name":"卖茶叶和眼镜per","uin":744891290}]},
            "1":{"gname":"朋友"},
            "2":{"gname":"家人"},
            "3":{"gname":"同学"}
        }
        }
         */
        [JsonProperty("ec")]
        public int ErrorCode { get; set; }
        [JsonProperty("result")]
        public Dictionary<int,QQFriendCategory> Result { get; set; }


    }
    public class QQFriend
    {
        /*
{"name":"卖茶叶和眼镜per","uin":744891290}
     */

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("uin")]
        public long QQ { get; set; }
        [JsonIgnore]
        public string Alias { get; set; }

    }
    public class QQFriendCategory
    {
        /*
{"name":"卖茶叶和眼镜per","uin":744891290}
     */

        [JsonProperty("gname")]
        public string Name { get; set; }

        [JsonProperty("mems")]
        public List<QQFriend> Friends { get; set; } = new List<QQFriend>();

    }

}
