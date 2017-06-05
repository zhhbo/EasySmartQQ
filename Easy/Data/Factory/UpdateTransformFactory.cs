using Easy.Data.Common.Utility;
using System;
using System.Collections;
using System.Text;
using Easy.Data.Attributes;
using Easy.Data.Sql;
namespace Easy.Data.Factory
{
    class UpdateTransformFactory : SqlTransformFactory
    {
        public UpdateTransformFactory(IDataFactory dataFactory, TableMapAttribute tableAttr, object parameter)
            : base(dataFactory, tableAttr, parameter)
        {
        }

        public override string TransformToSql()
        {
            Type type = _parameter.GetType();
            if ((_parameter as IList) != null)
            {
                var items = _parameter as IList;
                StringBuilder strBuilder = new StringBuilder();
                foreach (var item in items)
                {
                    strBuilder.Append(GetInsertStr(item) + SQLSPLITE);//"${}"
                }
                string strSql = strBuilder.ToString();
                return strSql.Substring(0, strSql.Length - 1);
            }
            else if (type.IsSubclassOf(typeof(SqlString)))
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
            Type objecType = obj.GetType();
            var pkVal = AttributeHelper.GetProperty(objecType, TableAttr.PrimaryKeyInfo.PropertyName).GetValue(obj);
            string updateTemplate = string.Format("update {0} set $1 where {1} = {2}", TableAttr.TableName,
               "["+ TableAttr.PrimaryKeyInfo.FieldName+"]", pkVal.GetType() == typeof(string)
                    ? string.Format("'{0}'", pkVal)
                    : pkVal);
            StringBuilder setValBuilder = new StringBuilder();
            foreach (var item in TableAttr.Maps)
            {
                object val = AttributeHelper.GetProperty(objecType, item.Key).GetValue(obj);
                setValBuilder.Append(string.Format("{0} = {1},","["+ item.Value.FieldName+"]", Database.GetFieldValue(val, item.Value.FieldType)));
            }
            string strSetValue = setValBuilder.ToString();
            return updateTemplate.Replace("$1", strSetValue.Substring(0, strSetValue.Length - 1));
        }
    }
}
