using Easy.Data.Common.Utility;
using System;
using System.Collections;
using System.Text;
using Easy.Data.Attributes;
using Easy.Data.Sql;
namespace Easy.Data.Factory
{
    class InsertTransformFactory : SqlTransformFactory
    {
       // private long _curId;

        public InsertTransformFactory(IDataFactory dataFactory, TableMapAttribute tableAttr, object parameter)
            : base(dataFactory, tableAttr, parameter)
        {
        }

        public override string TransformToSql()
        {
            Type type = _parameter.GetType();
          //  _curId = Database.GetNextId(TableAttr.PrimaryKeyInfo.FieldName, TableAttr.TableName);
            if ((_parameter as IList) != null)
            {
                var items = _parameter as IList;
                StringBuilder strBuilder = new StringBuilder();
                foreach (var item in items)
                {
                    strBuilder.Append(GetInsertStr(item) +SQLSPLITE);// ";"
                 //   _curId++;
                }
                string strSql = strBuilder.ToString();
                return strSql.Substring(0, strSql.Length - 1);
            }
            else if (type.IsSubclassOf( typeof(SqlString)))
            {
                return (_parameter as SqlString).SqlStr;
            }
            else  //单个对象
            {
                return GetInsertStr(_parameter);
            }
        }

        private string GetInsertStr(object obj)
        {
            var pkType = TableAttr.PrimaryKeyInfo.PropertyType;
            Type objecType = obj.GetType();
            var pkProperty = AttributeHelper.GetProperty(objecType, TableAttr.PrimaryKeyInfo.PropertyName);
            string insertTemplate = string.Format("insert into {0}($1) values($2)", TableAttr.TableName);
            StringBuilder filedBuilder=new StringBuilder();
            StringBuilder valueBuilder=new StringBuilder();
           // if (pkType == typeof (int))
           // {
            //    pkProperty.SetValue(obj, (int)_curId);
            //    valueBuilder.Append(pkProperty.GetValue(obj) + ",");
           // }
          //  else if (pkType == typeof (long))
           // {
           //     pkProperty.SetValue(obj, _curId);
          //      valueBuilder.Append(pkProperty.GetValue(obj) + ",");
           // }
           // else 
            if (pkType == typeof(string))
            {
                pkProperty.SetValue(obj, Guid.NewGuid().ToString());
                valueBuilder.Append(string.Format("'{0}',", pkProperty.GetValue(obj)));
                filedBuilder.Append("["+TableAttr.PrimaryKeyInfo.FieldName + "],");
            } 
           
            foreach (var item in TableAttr.Maps)
            {
                object val = AttributeHelper.GetProperty(objecType, item.Key).GetValue(obj);
                filedBuilder.Append("["+item.Value.FieldName + "],");
                valueBuilder.Append(Database.GetFieldValue(val, item.Value.FieldType) + ",");
            }
            string strField = filedBuilder.ToString();
            string strValue = valueBuilder.ToString();
            return insertTemplate.Replace("$1", strField.Substring(0, strField.Length - 1))
                .Replace("$2", strValue.Substring(0, strValue.Length - 1));
        }
    }
}
