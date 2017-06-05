namespace Easy.QQRob.Migrations
{
    using Easy.Data;
    public class QQStateMigration : DataMigrationImpl
    {
        public QQStateMigration()
        { }
        public int Create()
        {
            SchemaBuilder.CreateTable("QQState", table => table
                                .Column<int>("Id", column => column.PrimaryKey().Identity())//PrimaryKey().
                                .Column<string>("State", x=>x.Unlimited().WithDefault("{}"))//
                                  .Column<long>("QQNum")
                                     .Column<string>("Cookies",x=>x.Unlimited())
                                .Column<bool>("Logined")

);
            return 1;
        }
    }
}
