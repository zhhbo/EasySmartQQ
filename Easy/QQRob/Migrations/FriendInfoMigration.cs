namespace Easy.QQRob.Migrations
{
    using Easy.Data;
    public class FriendInfoMigration : DataMigrationImpl
    {
        public FriendInfoMigration()
        { }
        public int Create()
        {
            SchemaBuilder.CreateTable("FriendInfomation", table => table
                                .Column<int>("Id", column => column.PrimaryKey().Identity())//PrimaryKey().
                                .Column<long>("QQNum")
                                .Column<long>("Uin")
                                .Column<long>("RealQQ")
                                 .Column<string>("Alias")
                                .Column<string>("MarkName")
                                .Column<string>("Nick")
                                .Column<string>("Gender")
                                .Column<int>("Face")
                                .Column<int>("ClientType")
                                .Column<string>("Categories")
                                .Column<string>("Status")
                                .Column<string>("Occupation")
                                .Column<string>("College")
                                .Column<string>("Country")
                                .Column<string>("Province")
                                .Column<string>("City")
                                .Column<string>("Personal")
                                .Column<string>("Homepage")
                                .Column<string>("Email")                                
                                .Column<string>("Mobile")
                                .Column<string>("Phone")
                                .Column<string>("Birthday")
                                .Column<int>("Blood")
                                .Column<int>("ShengXiao")
                                .Column<int>("VipInfo"));
            return 1;
        }
    }
}
