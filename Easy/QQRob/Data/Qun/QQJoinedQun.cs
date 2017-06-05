using System.Collections.Generic;
using Newtonsoft.Json.Serialization;
using Newtonsoft.Json;
namespace Easy.QQRob.Data.Qun
{
    public class QunSummary
    {
        /*
         *  {"gc":1299322,
            "gn":"perl技术",
            "owner":4866832
            },
         */
        [JsonProperty("gc")]
        public long GroupId { get; set; }
        [JsonProperty("gn")]
        public string Name { get; set; }
        [JsonProperty("owner")]
        public long Owner { get; set; }
    }
    public class QQJoinedQun
    {
        public QQJoinedQun()
        {

        }
        ///cgi-bin/qun_mgr/get_group_list
        [JsonProperty("ec")]
        public int ErrorCode { get; set; }
        [JsonProperty("join")]
        public List<QunSummary> Joined { get; set; } = new List<QunSummary>();
        [JsonProperty("manage")]
        public List<QunSummary> Managed { get; set; } = new List<QunSummary>();
        [JsonProperty("create")]
        public List<QunSummary> Created { get; set; } = new List<QunSummary>();
        /*
        {"ec":0,
        "join":[

             {"gc":144539789,
             "gn":"PERL学习交流",
             "owner":419902730
             },
             {"gc":213925424,
             "gn":"PERL",
             "owner":913166583
             }
           ],
         "manage":[
             {"gc":390179723,
             "gn":"IT狂人",
             "owner":308165330
             }
          ]
         }  
        */
    }
}
