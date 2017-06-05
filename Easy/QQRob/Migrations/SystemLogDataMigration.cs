using System;
namespace Easy.QQRob.Migrations
{
    using Easy.Data;
    public class SystemLogDataMigration : DataMigrationImpl
    {
        public SystemLogDataMigration()
        { }
        public int Create()
        {
            SchemaBuilder.CreateTable("SystemLogData", table => table
                                .Column<int>("Id", column => column.PrimaryKey().Identity())//PrimaryKey().
                                .Column<string>("OriginData", x=>x.Unlimited())
                                .Column<int>("ReCode")
                                .Column<string>("Message")
                                .Column<DateTime>("CreateTime")

);
            return 1;
        }
    }
}
