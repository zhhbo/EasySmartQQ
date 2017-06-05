namespace Easy.Data.Factory
{
    using Easy.Data.Attributes;
    class SelectExecuteFactory : SqlExecuteFactoryBase
    {
        public SelectExecuteFactory(IDataFactory dataFactory, TableMapAttribute tableAttr) 
            : base(dataFactory, tableAttr)
        {
        }

        public override object DoSql()
        {
            string strSql = DataFactory.GetResult().ToString();
            return Database.Query(strSql);
        }
    }
}
