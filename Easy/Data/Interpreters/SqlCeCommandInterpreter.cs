using System;
using Easy;
using Easy.Data.Database.Enum;
using Easy.Data.Schema;
namespace Easy.Data.Interpreters
{
    public class SqlCeCommandInterpreter : ICommandInterpreter<DropIndexCommand> {


        public DatabaseType DataProvider {
            get { return DatabaseType.SqlCe; }
        }

        public SqlCeCommandInterpreter(
            ) {
}

        public string[] CreateStatements(DropIndexCommand command) {
            
            return new [] { String.Format("drop index {0}.{1}",
               PrefixTableName(command.TableName),
               PrefixTableName(command.IndexName))
            };
        }

        private string PrefixTableName(string tableName) {
           // if (string.IsNullOrEmpty(_shellSettings.DataTablePrefix))
                return tableName;
           // return _shellSettings.DataTablePrefix + "_" + tableName;
        }
    }
}
