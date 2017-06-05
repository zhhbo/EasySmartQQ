using System;
using System.Collections.Generic;

namespace Easy.Data.Attributes
{
    public class TableMapAttribute : Attribute
    {
        private string _tableName;
        public string TableName { get { return _tableName; } }

        public Dictionary<string,TableObjectMapInfo> Maps;

        public PrimaryKeyAttribute PrimaryKeyInfo { get; set; }

        public TableMapAttribute(string tableName)
        {
            _tableName = tableName;
            Maps = new Dictionary<string, TableObjectMapInfo>();
        }
    }
}
