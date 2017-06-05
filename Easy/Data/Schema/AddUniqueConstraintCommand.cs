namespace Easy.Data.Schema
{
    public class AddUniqueConstraintCommand : TableCommand {
        public string ConstraintName { get; set; }

        public AddUniqueConstraintCommand(string tableName, string constraintName, params string[] columnNames)
            : base(tableName) {
            ColumnNames = columnNames;
            ConstraintName = constraintName;
        }

        public string[] ColumnNames { get; private set; }
    }
}
