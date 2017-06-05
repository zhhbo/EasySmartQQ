using Easy.Data.Common.Interface;
using Easy.Data.Database.Enum;
using System;
using System.Collections.Generic;
using System.Threading;

namespace Easy.Data.Database.Core
{
    public static class ConnectionManager
    {
        private static DbConnectionPool _connectionPool = new DbConnectionPool();
        private static Dictionary<DatabaseType, string> _connectionStrMap = new Dictionary<DatabaseType, string>();
        private static volatile bool _isEndManager = false;

        public static int ConnectionPoolMaxCount = 5;

        public static IConnection GetConnection(DatabaseType databaseType,out bool isSuccessed)
        {
            isSuccessed = false;
            IConnection result = null;
            while(!isSuccessed)
            {
                result = _connectionPool.PopuConnection(databaseType, out isSuccessed);
                if(!isSuccessed)
                {
                    if(!_connectionPool.InitPool(databaseType, 1))
                    {
                        break;
                    }
                }
            }
            if(result!=null && !result.IsConnected)
            {
                result.Connect();
            }
            return result;
        }

        public static void ReturnConnection(DatabaseType databaseType,IConnection connection)
        {
            _connectionPool.PushConnection(databaseType, connection);
        }

        public static void RegisterConnectionType(DatabaseType databaseType, Type connectionType)
        {
            DbConnectionFactory.RegisterConnectionType(databaseType, connectionType);
        }

        public static void SetConnection(DatabaseType databaseType, string connectionStr)
        {
            if(!_connectionStrMap.ContainsKey(databaseType))
            {
                _connectionStrMap.Add(databaseType, connectionStr);
            }
        }

        public static string GetConnectionStr(DatabaseType databaseType)
        {
            if(_connectionStrMap.ContainsKey(databaseType))
            {
                return _connectionStrMap[databaseType];
            }
            return string.Empty;
        }

        public static bool InitManager(DatabaseType databaseType, int capacity)
        {
            _isEndManager = false;
            bool isSuccessed = _connectionPool.InitPool(databaseType, capacity);
            //其他初始化工作
            if(isSuccessed)
            {
                Thread thread = new Thread(new ThreadStart(MaintainConnectionPool));
                thread.Start();
            }
            return isSuccessed;
        }

        public static void EndManager()
        {
            _isEndManager = true;
        }

        /**
         对连接池中连接的管理，超时连接的关闭、连接数量控制
         */
        private static void MaintainConnectionPool()
        {
            List<DatabaseType> types;
            while(!_isEndManager)
            {
                Thread.Sleep(2000);
                types = DbConnectionFactory.GetRegisterTypes();
                foreach(var type in types)
                {
                    if (_connectionPool.GetCurrentCount(type) > ConnectionPoolMaxCount)
                    {
                        _connectionPool.RemoveConnection(type, ConnectionPoolMaxCount);
                    }
                }


            }
        }
    }
}
