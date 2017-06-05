namespace Easy.QQRob.Migrations
{
    using Easy.Data;
    public class MemberInfoMigration : DataMigrationImpl
    {
        public MemberInfoMigration()
        { }
        public int Create()
        {
            SchemaBuilder.CreateTable("MemberInfomation", table => table
                                .Column<int>("Id", column => column.PrimaryKey().Identity())//PrimaryKey().
                                .Column<long>("QQNum").Column<long>("Uin").Column<long>("RealQQ")
                                 .Column<string>("Alias")
                                .Column<string>("Nick")
                                .Column<string>("Country")
                                .Column<string>("Province")
                                .Column<string>("City")                               
                                .Column<string>("Gender")                              
                                .Column<string>("Card")
                                .Column<bool>("IsManager")
                                .Column<long>("GroupId")
                                
                                .Column<string>("Status")
                                .Column<int>("ClientType")
                                .Column<bool>("IsVip")
                                .Column<int>("VipLevel")
);
            return 1;
        }
    }
}
