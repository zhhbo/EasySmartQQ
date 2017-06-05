using Newtonsoft.Json;
using System.Text.RegularExpressions;
using Newtonsoft.Json.Linq;
namespace Easy.QQRob.Extensions
{


    public static class StringExtenstions
    {              /// <summary>
                   /// 计算GTK(gtk啥东东？)这个数在操作群和空间时会用到。
                   /// </summary>
                   /// <param name="skey"></param>
                   /// <returns></returns>
        public static string GetBkn(this string skey)
        {

            var hash = 5381;
            for (var i = 0; i < skey.Length; i++)
            {
                hash += (hash << 5) + skey[i];
            }
            return (hash & 2147483647).ToString();
        } 
        /// <summary>
          /// 获取Hash33,用于计算ptqrtoken 
          /// </summary>
          /// <returns>ptqrtoken</returns>
        public static int GetHash33(this string s)
        {
            int e = 0, i = 0, n = s.Length;
            for (; n > i; ++i)
                e += (e << 5) + s[i];
            return 2147483647 & e;
        }
        /// <summary>
        ///     将一个消息JSON中的节点转换为表情的文字描述或文字本身。
        /// </summary>
        /// <param name="token">JSON节点。</param>
        /// <returns>文字。</returns>
        public static string ParseEmoticons(JToken token)
        {
            if (token is JArray)
                return token.ToString(Formatting.None);
            return token.Value<string>();
        }

        /// <summary>
        ///     将消息中的表情文字（e.g. "/微笑"）转换为节点内容以实现发送内置表情。
        /// </summary>
        /// <param name="message"></param>
        /// <returns></returns>
        public static JToken[] TranslateEmoticons(this string message)
        {
            // TODO: 实现将文本中的表情转换为JSON节点
            return new[]
            {
                JToken.FromObject(message)
            };
        }
        /// <summary>
        /// 过滤html
        /// </summary>
        /// <param name="stringToStrip"></param>
        /// <returns></returns>
        public static string StripHTML(this string stringToStrip)
        {
            // paring using RegEx           //
            stringToStrip = Regex.Replace(stringToStrip, "</p(?:\\s*)>(?:\\s*)<p(?:\\s*)>", "\n\n", RegexOptions.IgnoreCase | RegexOptions.Compiled);
            stringToStrip = Regex.Replace(stringToStrip, "<br(?:\\s*)/>", "\n", RegexOptions.IgnoreCase | RegexOptions.Compiled);
            stringToStrip = Regex.Replace(stringToStrip, "\"", "''", RegexOptions.IgnoreCase | RegexOptions.Compiled);
            //stringToStrip = StripHtmlXmlTags(stringToStrip);
            return stringToStrip.StripHtmlXmlTags();
        }

        private static string StripHtmlXmlTags(this string content)
        {
            return Regex.Replace(content, "<[^>]+>", "", RegexOptions.IgnoreCase | RegexOptions.Compiled);
        }
    }
}
