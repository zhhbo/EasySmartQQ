namespace Easy.QQRob.Migrations
{
    using Easy.Data;
    public class StudyWordsMigration : DataMigrationImpl
    {
        public StudyWordsMigration()
        { }
        public int Create()
        {
            SchemaBuilder.CreateTable("StudyWords", table => table
                                .Column<int>("Id", column => column.PrimaryKey().Identity())//PrimaryKey().
                                .Column<string>("Source")
                                 .Column<string>("Aim")
                                 .Column<long>("QQNum")
                                 .Column<long>("SourceQQNum")
                                 .Column<long>("GroupId")
                                 .Column<string>("Reson")
                                 .Column<bool>("Pass")


);
            return 1;
        }
    }
}
