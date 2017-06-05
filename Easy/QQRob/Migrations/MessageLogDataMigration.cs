using System;
using Easy.Data;
namespace Easy.QQRob.Migrations
{
    public class MessageLogDataMigration : DataMigrationImpl
    {
        public MessageLogDataMigration()
        { }
        public int Create()
        {
            SchemaBuilder.CreateTable("MessageLogData", table => table
                                .Column<int>("Id", column => column.PrimaryKey().Identity())//PrimaryKey().
                                .Column<string>("OriginData", x => x.Unlimited())
                                .Column<int>("ReCode")
                                .Column<string>("Message")
                                .Column<long>("FromUin")
                                 .Column<long>("OwnerUin")
                                .Column<DateTime>("CreateTime")

);
            return 1;
        }
    }
}
