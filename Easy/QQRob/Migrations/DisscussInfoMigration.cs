namespace Easy.QQRob.Migrations
{
    using Easy.Data;
    public class DisscussInfoMigration : DataMigrationImpl
    {
        public DisscussInfoMigration()
        { }
        public int Create()
        {
            SchemaBuilder.CreateTable("DiscussInfomation", table => table
                                .Column<int>("Id", column => column.PrimaryKey().Identity())//.PrimaryKey().
                                 .Column<long>("QQNum")
                                .Column<long>("Did")
                                .Column<bool>("StillIn")
                                 .Column<string>("State", x => x.Unlimited().WithDefault("{}"))//
                                .Column<string>("Name"));
            SchemaBuilder.CreateTable("DiscussMemberInfo", table => table
                    .Column<int>("Id", column => column.PrimaryKey().Identity())//PrimaryKey().
                     .Column<long>("QQNum")
                    .Column<long>("Did") 
                    .Column<long>("Uin")
                    .Column<long>("RealQQ")
                    .Column<string>("Nick")
                    .Column<string>("Status")
                    .Column<int>("ClientType"));
            return 1;
        }
    }
}
