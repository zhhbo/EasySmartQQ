namespace Easy.QQRob.Migrations
{
    using Easy.Data;
    public class GroupInfoMigration : DataMigrationImpl
    {
        public GroupInfoMigration()
        { }
        public int Create()
        {
            SchemaBuilder.CreateTable("GroupInfomation", table => table
                                .Column<int>("Id", column => column.PrimaryKey().Identity())//PrimaryKey().
                                 .Column<long>("QQNum")
                                .Column<long>("GroupId")
                                 .Column<string>("Alias")
                                .Column<string>("Nick")
                                .Column<string>("Name")
                                .Column<long>("Code")
                                .Column<string>("MarkName")
                                .Column<string>("Memo")
                                .Column<int>("Face")
                                .Column<bool>("StillIn")
                                 .Column<string>("State", x => x.Unlimited().WithDefault("{}"))
                                .Column<long>("CreateTime")
                                .Column<int>("Level")
                                .Column<long>("Owner"));
            return 1;
        }
    }
}
