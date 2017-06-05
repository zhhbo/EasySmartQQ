using Easy.Data.Database.Enum;
using Easy.Data.Schema;
using System.Text;
using System.Linq;
namespace Easy.Data.Interpreters
{
    public class SQLiteCommandInterpreter :
        ICommandInterpreter<DropColumnCommand>,
        ICommandInterpreter<AlterColumnCommand>,
        ICommandInterpreter<CreateForeignKeyCommand>,
        ICommandInterpreter<DropForeignKeyCommand>,
        ICommandInterpreter<AddIndexCommand>,
    
        ICommandInterpreter<DropIndexCommand> {

        public string[] CreateStatements(DropColumnCommand command) {
            return new string[0];
        }

        public string[] CreateStatements(AlterColumnCommand command) {
            return new string[0];
        }

        public string[] CreateStatements(CreateForeignKeyCommand command) {
            return new string[0];
        }

        public string[] CreateStatements(DropForeignKeyCommand command) {
            return new string[0];
        }

        public string[] CreateStatements(AddIndexCommand command) {
            return new string[0];
        }

        public string[] CreateStatements(DropIndexCommand command) {
            return new string[0];
        }

        public DatabaseType DataProvider {
            get { return DatabaseType.SQLite; }
        }
    }
}
