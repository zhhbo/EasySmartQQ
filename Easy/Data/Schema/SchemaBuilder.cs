using System;
using Easy;

namespace Easy.Data.Schema
{
    public class SchemaBuilder {

        private readonly IDataMigrationInterpreter _interpreter;


        public SchemaBuilder(IDataMigrationInterpreter interpreter) {
            _interpreter = interpreter;
        }







        public SchemaBuilder CreateTable(string name, Action<CreateTableCommand> table) {
            var createTable = new CreateTableCommand( name);
            table(createTable);
            Run(createTable);
            return this;
        }

        public SchemaBuilder AlterTable(string name, Action<AlterTableCommand> table) {
            var alterTable = new AlterTableCommand( name);
            table(alterTable);
            Run(alterTable);
            return this;
        }

        public SchemaBuilder DropTable(string name) {
            var deleteTable = new DropTableCommand(name);
            Run(deleteTable);
            return this;
        }

        public SchemaBuilder ExecuteSql(string sql, Action<SqlStatementCommand> statement = null) {
            try {
                var sqlStatmentCommand = new SqlStatementCommand(sql);
                statement?.Invoke(sqlStatmentCommand);
                Run(sqlStatmentCommand);
                return this;
            } catch (Exception ex) {
 
                throw new Exception(string.Format("An unexpected error occurred while executing the SQL statement: {0}", sql), ex); // Add the sql to the nested exception information
            }
        }

        private void Run(ISchemaBuilderCommand command) {
            _interpreter.Visit(command);
        }

        public SchemaBuilder CreateForeignKey(string name, string srcTable, string[] srcColumns, string destTable, string[] destColumns) {
            var command = new CreateForeignKeyCommand(name, srcTable, srcColumns, destTable, destColumns);
            Run(command);
            return this;
        }

        public SchemaBuilder CreateForeignKey(string name, string srcModule, string srcTable, string[] srcColumns, string destTable, string[] destColumns) {
            var command = new CreateForeignKeyCommand(name, String.Concat(srcModule, srcTable), srcColumns,  destTable, destColumns);
            Run(command);
            return this;
        }

        public SchemaBuilder CreateForeignKey(string name, string srcTable, string[] srcColumns, string destModule, string destTable, string[] destColumns) {
            var command = new CreateForeignKeyCommand(name,  srcTable, srcColumns, String.Concat(destModule, destTable), destColumns);
            Run(command);
            return this;
        }

        public SchemaBuilder CreateForeignKey(string name, string srcModule, string srcTable, string[] srcColumns, string destModule, string destTable, string[] destColumns) {
            var command = new CreateForeignKeyCommand(name, String.Concat(srcModule, srcTable), srcColumns, String.Concat(destModule, destTable), destColumns);
            Run(command);
            return this;
        }

        public SchemaBuilder DropForeignKey(string srcTable, string name) {
            var command = new DropForeignKeyCommand( srcTable, name);
            Run(command);
            return this;
        }

        public SchemaBuilder DropForeignKey(string srcModule, string srcTable, string name) {
            var command = new DropForeignKeyCommand(String.Concat(srcModule, srcTable), name);
            Run(command);
            return this;
        }

    }
}
