using System;
using Easy.Data.Attributes;
namespace Easy.Data.Factory
{
    abstract class SqlExecuteFactoryBase : DataFactoryBase
    {
      
        public SqlExecuteFactoryBase(IDataFactory dataFactory, TableMapAttribute tableAttr) 
            : base(dataFactory, tableAttr)
        {
        }

        public override object GetResult()
        {
            if(DataFactory!=null)
            {
                return DoSql();
            }
            else
            {
                throw new Exception("not set SqlTransformFactory!");
            }
        }

        public abstract object DoSql();
    }
}
