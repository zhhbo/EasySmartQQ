using Easy;
using Easy.Data.Schema;
namespace Easy.Data
{
    public interface IDataMigrationInterpreter: ISingletonDependency
    {
        void Visit(ISchemaBuilderCommand command);
        void Visit(CreateTableCommand command);
        void Visit(DropTableCommand command);
        void Visit(AlterTableCommand command);
        void Visit(SqlStatementCommand command);
        void Visit(CreateForeignKeyCommand command);
        void Visit(DropForeignKeyCommand command);
        string PrefixTableName(string tableName);
        string RemovePrefixFromTableName(string prefixedTableName);
    }
}
