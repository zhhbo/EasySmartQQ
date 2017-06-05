namespace Easy.Tables
{
    using Easy.Data.Attributes;
    [TableMap("OrmTablesInfo")]
  public  class OrmTablesInfo
    {
        [PrimaryKey("Id", typeof(int), "Id", typeof(int))]
        public int Id { get; set; }

        [PropertyFieldMap("DataMigrationClass", typeof(string), "DataMigrationClass", typeof(string))]
        public string DataMigrationClass { get; set; }

        [PropertyFieldMap("Version", typeof(int), "Version", typeof(int))]
        public int Version { get; set; }
    }
}
