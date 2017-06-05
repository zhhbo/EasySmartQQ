using Easy.Data.Common.Partten_Class;
using Easy.Data.Common.Utility;
using System;
using Easy.Data.Attributes;
namespace Easy.Data
{
    internal class MapCollector : SingletonPattern<MapCollector>
    {
        public bool ContainsType(Type objectType)
        {
            return OrmMapCache.Instance.OrmMap.ContainsKey(objectType);
        }

        public void CollectMapInfo(Type objectType)
        {
            var tableAttr = AttributeHelper.GetAttribute<TableMapAttribute>(objectType, true);
            if (tableAttr == null)
            {
                throw new Exception(string.Format("type [{0}] dose not bind [TableMapAttribute] attribute", objectType.Name));
            }
            var properties = AttributeHelper.GetProperties(objectType);
            bool hasPrimaryKeyAttr = false;
            foreach(var property in properties)
            {
                var primaryKeyAttr = AttributeHelper.GetPropertyAttribute<PrimaryKeyAttribute>(property);
                if(primaryKeyAttr!=null)
                {
                    hasPrimaryKeyAttr = true;
                    tableAttr.PrimaryKeyInfo = primaryKeyAttr;
                }
                else
                {
                    var propertyAttr = AttributeHelper.GetPropertyAttribute<PropertyFieldMapAttribute>(property);
                    if(propertyAttr!=null)
                    {
                        tableAttr.Maps.Add(propertyAttr.PropertyName, new TableObjectMapInfo()
                        {
                            PropertyName = propertyAttr.PropertyName,
                            PropertyType = propertyAttr.PropertyType,
                            FieldName = propertyAttr.FieldName,
                            FieldType = propertyAttr.FieldType
                        });
                    }
                }
            }
            if(hasPrimaryKeyAttr)
            {
                OrmMapCache.Instance.OrmMap.Add(objectType, tableAttr);
            }
            else
            {
                throw new Exception(string.Format("type [{0}] has no one property that binds [PrimaryKeyAttribute] attribute", objectType.Name));
            }
        }

        public TableMapAttribute GetMapInfo(Type objectType)
        {
            return OrmMapCache.Instance.OrmMap[objectType];
        }
    }
}
