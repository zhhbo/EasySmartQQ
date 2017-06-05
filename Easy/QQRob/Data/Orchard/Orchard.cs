using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
namespace Easy.QQRob.Data.Easy
{
    public class ApiPostData
    {
        [JsonProperty("keyword")]
        public string Keyword { get; set; }
        [JsonProperty("page")]
        public int Page { get; set; }
    }
    public class ApiContentResult
    {
        [JsonProperty("count")]
        public int Count { get; set; }
        [JsonProperty("page")]
        public int Page { get; set; }
        [JsonProperty("issuccess")]
        public bool IsSuccess { get; set; }
        [JsonProperty("message")]
        public string Message { get; set; }
        [JsonProperty("content")]
        public List<ContentMessage> Contents { get; set; }
    }
    public class ContentMessage
    {
        [JsonProperty("summary")]
        public string Summary { get; set; }
        [JsonProperty("title")]
        public string Title { get; set; }
        [JsonProperty("html")]
        public string Html { get; set; }
        [JsonProperty("url")]
        public string Url { get; set; }
    }
}
