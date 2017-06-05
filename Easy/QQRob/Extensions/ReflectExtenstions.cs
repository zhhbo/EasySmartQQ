using System.Linq;
using System.Reflection;
using System.Collections;
using System.Net;
namespace Easy.QQRob.Services
{
    internal static class ReflectExtenstions
    {

        public static CookieCollection GetAllCookies(this CookieContainer container)
        {
            var allCookies = new CookieCollection();
            var domainTableField = container.GetType().GetRuntimeFields().FirstOrDefault(x => x.Name == "m_domainTable");
            // ReSharper disable once PossibleNullReferenceException
            var domains = (IDictionary)domainTableField.GetValue(container);

            foreach (var val in domains.Values)
            {
                var type = val.GetType().GetRuntimeFields().First(x => x.Name == "m_list");
                var values = (IDictionary)type.GetValue(val);
                foreach (CookieCollection cookies in values.Values)
                    allCookies.Add(cookies);
            }
            return allCookies;
        }
    }
}
