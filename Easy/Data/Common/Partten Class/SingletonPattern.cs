namespace Easy.Data.Common.Partten_Class
{
    public class SingletonPattern<T>
        where T : class, new()
    {
        private static object _lockObj = new object();

        private static T _instance = null;

        public static T Instance
        {
            get
            {
                lock (_lockObj)
                {
                    if (_instance == null)
                    {
                        _instance = new T();
                    }
                }
                return _instance;
            }
        }
    }
}
