using System;
using System.Collections.Generic;
using System.Linq;
using Easy.Data.Common.Interface;
using Easy.Data.Database.Enum;
using Easy.Data.Common.Partten_Class;

namespace Easy.Data.Database.Core
{
    internal static class DbConnectionFactory
    {
        private static Dictionary<DatabaseType, Type> _factoryMaps = new Dictionary<DatabaseType, Type>();

        public static void RegisterConnectionType(DatabaseType databaseType, Type connectionType)
        {
            if (!_factoryMaps.ContainsKey(databaseType))
            {
                _factoryMaps.Add(databaseType, connectionType);
            }
        }

        public static IEnumerable<IConnection> BuildUpConnection(DatabaseType databaseType, int capacity)
        {
            if (!_factoryMaps.ContainsKey(databaseType))
            {
                throw new Exception("database type not registered!");
            }
            var items = new ObjectFactory().BuildUp(_factoryMaps[databaseType], capacity);
            return items.Select(item => item as IConnection);
        }

        public static IConnection BuildUpConnection(DatabaseType databaseType)
        {
            if (!_factoryMaps.ContainsKey(databaseType))
            {
                throw new Exception("database type not registered!");
            }
            return new ObjectFactory().BuildUp(_factoryMaps[databaseType]) as IConnection;
        }

        public static List<DatabaseType> GetRegisterTypes()
        {
            return _factoryMaps.Keys.ToList();
        }
    }
}
