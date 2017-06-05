using Easy.Data.Common.Utility;
using System;
using System.Collections;
using System.Text;
using Easy.Data.Attributes;
using Easy.Data.Sql;
namespace Easy.Data.Factory
{
    class DeleteTransformFactory : SqlTransformFactory
    {
        public DeleteTransformFactory(IDataFactory dataFactory, TableMapAttribute tableAttr, object parameter)
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
                    strBuilder.Append(GetDeleteStr(item) + SQLSPLITE);
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
                return GetDeleteStr(_parameter);
            }
        }

        private string GetDeleteStr(object obj)
        {
            var val = AttributeHelper.GetProperty(obj.GetType(), TableAttr.PrimaryKeyInfo.PropertyName).GetValue(obj);
            return string.Format("delete from {0} where {1} = {2}", TableAttr.TableName,
                "["+TableAttr.PrimaryKeyInfo.FieldName+"]", val.GetType() == typeof(string)
                    ? string.Format("'{0}'", val)
                    : val);
        }
    }
}
