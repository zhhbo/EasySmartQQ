using System.Collections.Generic;
namespace Easy
{
    public static  class WorkContext
    {
        public static IDictionary<string, object> Datas = new Dictionary<string,object>();
        public static void SetState<T>(string key, T value)
        {
            if (Datas.ContainsKey(key) == true)
            {
                Datas[key] = value;
            }
            else
            {

                Datas.Add(key, value);
            }
        }
        public static T GetState<T>(string key)
        {
            if (Datas == null)
            {
                return default(T);
            }
            if (Datas.ContainsKey(key))
                return (T)Datas[key];
            return default(T);
        }
        public static object GetState(string key)
        {
            if (Datas == null)
            {
                return null;
            }
            if (Datas.ContainsKey(key))
                return Datas[key];
            return null;
        }

    }
}
