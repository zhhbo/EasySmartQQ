using System;
using System.Text;
using Easy.Data.Attributes;
using Easy.Data.Sql;
namespace Easy.Data.Factory
{
    class SelectTransformFactory : SqlTransformFactory
    {
        public SelectTransformFactory(IDataFactory dataFactory, TableMapAttribute tableAttr, object parameter)
            : base(dataFactory, tableAttr, parameter)
        {
        }

        public override string TransformToSql()
        {
            Type type = _parameter.GetType();
            if (type == typeof(string))
            {
                return string.Format("select * from {0} where {1}", TableAttr.TableName, _parameter);
            }
            else if (type == typeof(QueryCondition))
            {
                StringBuilder sqlBuilder = new StringBuilder(string.Format("select * from {0} where", TableAttr.TableName));
                QueryCondition conds = _parameter as QueryCondition;
                foreach (var item in conds)
                {
                    sqlBuilder.Append(" " + string.Format(item.SqlExpress,"["+ TableAttr.Maps[item.PropertyName]) + "] and");
                }
                string strSql = sqlBuilder.ToString();
                if (strSql.EndsWith("and"))
                {
                    strSql = strSql.Substring(0, strSql.IndexOf("and") - 1);
                }
                return sqlBuilder.ToString();
            }
            else //SqlString
            {
                return (_parameter as SqlString).SqlStr;
            }
        }
    }
}
