using Easy.Data.Common.Interface;
using Easy.Data.Common.Partten_Class;
using Easy.Data.Common.Utility;
using System;
using System.Collections.Generic;
using System.Data;
using Easy.Data.Attributes;
namespace Easy.Data.Factory
{
    class DataToObjectFactory<T> : DataFactoryBase
        where T : class, new()
    {
        public DataToObjectFactory(IDataFactory dataFactory, TableMapAttribute tableAttr)
            : base(dataFactory, tableAttr)
        {
        }

        public override object GetResult()
        {
            DataTable dt = DataFactory.GetResult() as DataTable;
            List<T> result = new List<T>();
            T obj;
            IFactory objFactory = new ObjectFactory();
            Type objectType = typeof(T);
            foreach (DataRow dr in dt.Rows)
            {
                obj = objFactory.BuildUp<T>();
                AttributeHelper.GetProperty(objectType, TableAttr.PrimaryKeyInfo.PropertyName).SetValue(obj, dr[TableAttr.PrimaryKeyInfo.FieldName]);
                foreach(var mapInfo in TableAttr.Maps)
                {
                    if(dr[mapInfo.Value.FieldName] is DBNull)
                    {
                        continue;
                    }
                    objectType.GetProperty(mapInfo.Value.PropertyName).SetValue(obj, dr[mapInfo.Value.FieldName], null);
                }
                result.Add(obj);
            }
            return result;
        }
    }
}
