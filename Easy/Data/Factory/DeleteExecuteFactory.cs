using System;
using Easy.Data.Attributes;
namespace Easy.Data.Factory
{
    class DeleteExecuteFactory : SqlExecuteFactoryBase
    {
        public DeleteExecuteFactory(IDataFactory dataFactory, TableMapAttribute tableAttr) 
            : base(dataFactory, tableAttr)
        {
        }

        public override object DoSql()
        {
            string strSqlResult = DataFactory.GetResult().ToString();
            string[] strSqls = strSqlResult.Split(new[] { SQLSPLITE }, StringSplitOptions.RemoveEmptyEntries);//';'
            if (strSqls.Length == 1)
            {
                return Database.ExecuteSql(strSqls[0]);
            }
            if (strSqls.Length > 1)
            {
                try
                {
                    Database.BeginTransaction();
                    foreach (var strsql in strSqls)
                    {
                        Database.ExecuteSql(strsql);
                    }
                    Database.CommitTransaction();
                    return true;
                }
                catch (Exception exp)
                {
                    Database.RollbackTransaction();
                    return false;
                }
            }
            throw new Exception("no delete sql executed!");
        }
    }
}
