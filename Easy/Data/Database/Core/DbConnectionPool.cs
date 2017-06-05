using Easy.Data.Common.Interface;
using Easy.Data.Common.Partten_Class;
using Easy.Data.Common.Structure_Class;
using Easy.Data.Database.Enum;

namespace Easy.Data.Database.Core
{
    internal class DbConnectionPool : SingletonPattern<DbConnectionPool>
    {
        private SafeDictionaryQueue<DatabaseType,IConnection> _safeQueue;

        public bool InitPool(DatabaseType databaseType, int capacity)
        {
            bool bResult = false;
            if (_safeQueue == null)
            {
                _safeQueue = new SafeDictionaryQueue<DatabaseType, IConnection>();
            }
            var items = DbConnectionFactory.BuildUpConnection(databaseType, capacity);
            foreach(var item in items)
            {
                (item as DbConnectionBase).ConnectionStr = ConnectionManager.GetConnectionStr(databaseType);
                if(item.Connect())
                {
                    bResult = true;
                    _safeQueue.Add(databaseType, item);
                }
            }
            return bResult;
        }

        public IConnection PopuConnection(DatabaseType databaseType, out bool isSuccessed)
        {
            return _safeQueue.GetSingle(databaseType, out isSuccessed);
        }

        public void PushConnection(DatabaseType databaseType,IConnection connection)
        {
            _safeQueue.Add(databaseType, connection);
        }

        public int GetCurrentCount(DatabaseType databaseType)
        {
            return _safeQueue.GetSingleCount(databaseType);
        }

        public void RemoveConnection(DatabaseType databaseType, int remainMaxCount)
        {
            var result = _safeQueue.RemainMaxCount(databaseType, remainMaxCount);
            foreach(var item in result)
            {
                if(item.IsConnected)
                {
                    (item as DbConnectionBase).Dispose();
                }
            }
        }
    }
}
