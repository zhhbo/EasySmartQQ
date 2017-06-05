using System.Collections.Generic;
using Newtonsoft.Json.Serialization;
using Newtonsoft.Json;
namespace Easy.QQRob.Data.Qun
{

    public  class JsonQun
    {
        [JsonProperty("adm_max")]
        public int MaxAdminNum { get; set; }
        [JsonProperty("adm_num")]
        public int AdminNum { get; set; }
        [JsonProperty("count")]
        public int Count { get; set; }
        [JsonProperty("ec")]
        public int ErrorCode { get; set; }
        [JsonProperty("levelname")]
        public Dictionary<int,string> Level { get; set; }
        [JsonProperty("mems")]
        public List<QunMember> Members { get; set; }
        [JsonProperty("search_count")]
        public int SearchCount { get; set; }
        [JsonProperty("svr_time")]
        public long ServerTime { get; set; }
        [JsonProperty("vecsize")]
        public int Vecsize { get; set; }

    }
    public class QunMember
    {
        /*
         role  0   =>  "owner",   1   =>  "admin",     2   =>  "member"
            */
        /*
         { "card":"",
     *          "flag":0,
     *          "g":0,
     *          "join_time":1410241477,
     *          "last_speak_time":1427191050,
     *          "lv":{
     *              "level":2,
     *              "point":404
     *              },
     *           "nick":"灰灰",
     *           "qage":10,
     *           "role":0,
     *           "tags":"-1",
     *           "uin":308165330
     *       },
     */
        [JsonProperty("flag")]
        public int Flag { get; set; }
        [JsonProperty("card")]
        public string Card { get; set; }
        [JsonProperty("join_time")]
        public long JoinTime { get; set; }
        [JsonProperty("g")]
        public int G { get; set; }
        [JsonProperty("last_speak_time")]
        public long LastSpeakTime { get; set; }
        [JsonProperty("nick")]
        public string Nick { get; set; }
        [JsonProperty("qage")]
        public int QQage { get; set; }
        /// <summary>
        /// 0   =>  "owner",   1   =>  "admin",     2   =>  "member"
        /// </summary>
        [JsonProperty("role")]
        public int Role { get; set; }

        [JsonProperty("tags")]
        public int Tags { get; set; }
        [JsonProperty("uin")]
        public long QQ { get; set; }

    }
    /*
     *  {"adm_max":10,
     *  "adm_num":1,
     *  "count":4,
     *  "ec":0,
     *  "levelname":{"1":"潜水","2":"冒泡","3":"吐槽","4":"活跃","5":"话唠","6":"传说"},
     *  "max_count":500,
     *  "mems":[
     *      { "card":"",
     *          "flag":0,
     *          "g":0,
     *          "join_time":1410241477,
     *          "last_speak_time":1427191050,
     *          "lv":{
     *              "level":2,
     *              "point":404
     *              },
     *           "nick":"灰灰",
     *           "qage":10,
     *           "role":0,
     *           "tags":"-1",
     *           "uin":308165330
     *       },
     *    {"card":"",
     *    "flag":0,
     *    "g":0,
     *    "join_time":1423016758,
     *    "last_speak_time":1427210847,
     *    "lv":{
     *          "level":2,
     *          "point":275
     *         },
     *     "nick":"小灰",
     *     "qage":0,
     *     "role":1,
     *     "tags":"-1",
     *     "uin":3072574066
     *     },
     *     {"card":"","flag":0,"g":0,"join_time":1427210502,"last_speak_time":1427210858,"lv":{"level":2,"point":1},"nick":"王鹏飞","qage":8,"role":2,"tags":"-1","uin":470869063},
     *     {"card":"小灰2号","flag":0,"g":0,"join_time":1422946743,"last_speak_time":1424144472,"lv":{"level":1,"point":0},"nick":"小灰2号","qage":0,"role":2,"tags":"-1","uin":1876225186}],
     *     "search_count":4,
     *     "svr_time":1427291710,
     *     "vecsize":1
     *     }
     

     */
}
