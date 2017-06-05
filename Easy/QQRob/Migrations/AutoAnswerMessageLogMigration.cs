using System;
using Easy.Data;
namespace Easy.QQRob.Migrations
{
    public class AutoAnswerMessageLogMigration : DataMigrationImpl
    {
        public AutoAnswerMessageLogMigration()
        { }
        public int Create()
        {
            SchemaBuilder.CreateTable("AutoAnswerMessageLog", table => table
                                .Column<int>("Id", column => column.PrimaryKey().Identity())//PrimaryKey().
                                .Column<string>("Data",x=>x.Unlimited())
                                .Column<long>("FromUin")
                                 .Column<long>("ToUin")
                                  .Column<string>("MessageType")
                                   .Column<string>("P1")
                                    .Column<string>("P2")
                                    .Column<string>("P3")
                                    .Column<string>("P4")
                                .Column<DateTime>("CreateTime")

);
            return 1;
        }
    }
}
