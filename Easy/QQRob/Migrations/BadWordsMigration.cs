namespace Easy.QQRob.Migrations
{
    using Easy.Data;
    public class BadWordsMigration : DataMigrationImpl
    {
        public BadWordsMigration()
        { }
        public int Create()
        {
            SchemaBuilder.CreateTable("BadWords", table => table
                                .Column<int>("Id", column => column.PrimaryKey().Identity())//PrimaryKey().
                                .Column<string>("Word")
                                 .Column<long>("QQNum")


);
            return 1;
        }
    }
}
