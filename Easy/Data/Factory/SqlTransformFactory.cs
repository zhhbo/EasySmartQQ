using System;
using Easy.Data.Attributes;
namespace Easy.Data.Factory
{
    abstract class SqlTransformFactory : DataFactoryBase
    {
        protected object _parameter;
       // protected const string SQLSPLITE = "${sqlsplite}";
        public SqlTransformFactory(IDataFactory dataFactory, TableMapAttribute tableAttr, object parameter)
            : base(dataFactory, tableAttr)
        {
            _parameter = parameter;
        }

        public override object GetResult()
        {
            return TransformToSql();
        }

        public abstract string TransformToSql();
    }
}
