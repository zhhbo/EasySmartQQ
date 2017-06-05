using Easy.Data.Common.Partten_Class;
using System;
using System.Collections.Generic;
using Easy.Data.Attributes;
namespace Easy.Data
{
    internal class OrmMapCache : SingletonPattern<OrmMapCache>
    {
        private Dictionary<Type, TableMapAttribute> _ormMap;

        public Dictionary<Type, TableMapAttribute> OrmMap { get{return _ormMap;}}

        public OrmMapCache()
        {
            _ormMap = new Dictionary<Type, TableMapAttribute>();
        }
    }
}
