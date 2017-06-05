using Easy.Data.Database.Core;
using Easy.Data.Attributes;
namespace Easy.Data.Factory
{
    abstract class DataFactoryBase : IDataFactory
    {
        private IDataFactory _dataFactory;
        public IDataFactory DataFactory { get { return _dataFactory; } }
        protected const string SQLSPLITE = "${sqlsplite}";
        private TableMapAttribute _tableAttr;
        public TableMapAttribute TableAttr { get { return _tableAttr; } }

        public IDatabase Database { get; set; }

        public DataFactoryBase(IDataFactory dataFactory, TableMapAttribute tableAttr)
        {
            _dataFactory = dataFactory;
            _tableAttr = tableAttr;
            if (dataFactory != null)
            {
                Database = dataFactory.Database;
            }
        }

        public abstract object GetResult();
    }
}
