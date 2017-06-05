using Easy;
using Easy.Data.Schema;
namespace Easy.Data
{
    /// <summary>
    /// Data Migration classes can inherit from this class to get a SchemaBuilder instance configured with the current tenant database prefix
    /// </summary>
    public abstract class DataMigrationImpl:IDataMigration  {
        public virtual SchemaBuilder SchemaBuilder { get; set; }
    }
}