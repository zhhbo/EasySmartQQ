using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Linq;
using System.Text;
using Easy;
using Easy.Data.Database.Core;
using Prism.Unity;
using Prism.Mvvm;
using Easy.Data.Schema;
using Microsoft.Practices.Unity;
using Prism.Logging;
namespace  Easy.Data.Interpreters

{
    public class DefaultDataMigrationInterpreter : AbstractDataMigrationInterpreter, IDataMigrationInterpreter
    {

        private readonly IEnumerable<ICommandInterpreter> _commandInterpreters;
        private readonly List<string> _sqlStatements;
        private readonly ILoggerFacade Logger;
        private const char Space = ' ';
        private static IDatabase _dataBase=OrmManager.CurrentDatabase();
        public DefaultDataMigrationInterpreter(IEnumerable<ICommandInterpreter> commandInterpreters,
            IUnityContainer container,ILoggerFacade loger
            )
        {

            _commandInterpreters = commandInterpreters; // FindAllClass();
            _sqlStatements = new List<string>();
            Logger = loger;

        }

        private IEnumerable<Type>  FindAllClass()
        {
            return AppDomain.CurrentDomain.GetAssemblies()
                                 .SelectMany(a => a.GetTypes().Where(t => t.GetInterfaces().Contains(typeof(ICommandInterpreter))))
                                // .Select(x=>x as ICommandInterpreter);
                                ;

        }

        public IEnumerable<string> SqlStatements
        {
            get { return _sqlStatements; }
        }

        public override void Visit(CreateTableCommand command)
        {

            if (ExecuteCustomInterpreter(command))
            {
                return;
            }

            var builder = new StringBuilder();

            builder.Append(_dataBase.CreateMultisetTableString)
                .Append(' ')
                .Append(_dataBase.QuoteForTableName(PrefixTableName(command.Name)))
                .Append(" (");

            var appendComma = false;
            foreach (var createColumn in command.TableCommands.OfType<CreateColumnCommand>())
            {
                if (appendComma)
                {
                    builder.Append(", ");
                }
                appendComma = true;

                Visit(builder, createColumn);
            }

            var primaryKeys = command.TableCommands.OfType<CreateColumnCommand>().Where(ccc => ccc.IsPrimaryKey).Select(ccc => ccc.ColumnName).ToArray();
            if (primaryKeys.Any())
            {
                if (appendComma)
                {
                    builder.Append(", ");
                }

                var primaryKeysQuoted = new List<string>(primaryKeys.Length);
                foreach (string pk in primaryKeys)
                {
                    primaryKeysQuoted.Add(_dataBase.QuoteForColumnName(pk));
                }

                builder.Append(_dataBase.PrimaryKeyString)
                    .Append(" ( ")
                    .Append(String.Join(", ", primaryKeysQuoted.ToArray()))
                    .Append(" )");
            }

            builder.Append(" )");
            _sqlStatements.Add(builder.ToString());

            RunPendingStatements();
        }

        public string PrefixTableName(string tableName)
        {
          //  if (string.IsNullOrEmpty(_shellSettings.DataTablePrefix))
                return tableName;
           // return _shellSettings.DataTablePrefix + "_" + tableName;
        }

        public string RemovePrefixFromTableName(string prefixedTableName)
        {
            //if (string.IsNullOrEmpty(_shellSettings.DataTablePrefix))
                return prefixedTableName;
            //return prefixedTableName.Substring(_shellSettings.DataTablePrefix.Length + 1);
        }

        public override void Visit(DropTableCommand command)
        {
            if (ExecuteCustomInterpreter(command))
            {
                return;
            }

            var builder = new StringBuilder();

            builder.Append(_dataBase.GetDropTableString(PrefixTableName(command.Name)));
            _sqlStatements.Add(builder.ToString());

            RunPendingStatements();
        }

        public override void Visit(AlterTableCommand command)
        {
            if (ExecuteCustomInterpreter(command))
            {
                return;
            }

            if (command.TableCommands.Count == 0)
            {
                return;
            }

            // Drop columns.
            foreach (var dropColumn in command.TableCommands.OfType<DropColumnCommand>())
            {
                var builder = new StringBuilder();
                Visit(builder, dropColumn);
                RunPendingStatements();
            }

            // Add columns.
            foreach (var addColumn in command.TableCommands.OfType<AddColumnCommand>())
            {
                var builder = new StringBuilder();
                Visit(builder, addColumn);
                RunPendingStatements();
            }

            // Alter columns.
            foreach (var alterColumn in command.TableCommands.OfType<AlterColumnCommand>())
            {
                var builder = new StringBuilder();
                Visit(builder, alterColumn);
                RunPendingStatements();
            }

            // Add index.
            foreach (var addIndex in command.TableCommands.OfType<AddIndexCommand>())
            {
                var builder = new StringBuilder();
                Visit(builder, addIndex);
                RunPendingStatements();
            }

            // Drop index.
            foreach (var dropIndex in command.TableCommands.OfType<DropIndexCommand>())
            {
                var builder = new StringBuilder();
                Visit(builder, dropIndex);
                RunPendingStatements();
            }

            // Add unique constraint.
            foreach (var addUniqueConstraint in command.TableCommands.OfType<AddUniqueConstraintCommand>())
            {
                var builder = new StringBuilder();
                Visit(builder, addUniqueConstraint);
                RunPendingStatements();
            }

            // Drop unique constraint.
            foreach (var dropUniqueConstraint in command.TableCommands.OfType<DropUniqueConstraintCommand>())
            {
                var builder = new StringBuilder();
                Visit(builder, dropUniqueConstraint);
                RunPendingStatements();
            }
        }

        public void Visit(StringBuilder builder, AddColumnCommand command)
        {
            if (ExecuteCustomInterpreter(command))
            {
                return;
            }

            builder.AppendFormat("alter table {0} add ", _dataBase.QuoteForTableName(PrefixTableName(command.TableName)));

            Visit(builder, (CreateColumnCommand)command);
            _sqlStatements.Add(builder.ToString());
        }

        public void Visit(StringBuilder builder, DropColumnCommand command)
        {
            if (ExecuteCustomInterpreter(command))
            {
                return;
            }

            builder.AppendFormat("alter table {0} drop column {1}",
                _dataBase.QuoteForTableName(PrefixTableName(command.TableName)),
                _dataBase.QuoteForColumnName(command.ColumnName));
            _sqlStatements.Add(builder.ToString());
        }

        public void Visit(StringBuilder builder, AlterColumnCommand command)
        {
            if (ExecuteCustomInterpreter(command))
            {
                return;
            }

            builder.AppendFormat("alter table {0} alter column {1} ",
                _dataBase.QuoteForTableName(PrefixTableName(command.TableName)),
                _dataBase.QuoteForColumnName(command.ColumnName));

            // type
            if (command.DbType != DbType.Object)
            {
                builder.Append(GetTypeName( command.DbType, command.Length, command.Precision, command.Scale));
            }
            else
            {
                if (command.Length > 0 || command.Precision > 0 || command.Scale > 0)
                {
                    throw new Exception(string.Format("Error while executing data migration: you need to specify the field's type in order to change its properties"));
                }
            }

            // [default value]
            if (command.Default != null)
            {
                builder.Append(" set default ").Append(ConvertToSqlValue(command.Default)).Append(Space);
            }
            _sqlStatements.Add(builder.ToString());
        }

        public void Visit(StringBuilder builder, AddIndexCommand command)
        {
            if (ExecuteCustomInterpreter(command))
            {
                return;
            }

            builder.AppendFormat("create index {1} on {0} ({2}) ",
                _dataBase.QuoteForTableName(PrefixTableName(command.TableName)),
                _dataBase.QuoteForColumnName(PrefixTableName(command.IndexName)),
                String.Join(", ", command.ColumnNames));

            _sqlStatements.Add(builder.ToString());
        }

        public void Visit(StringBuilder builder, DropIndexCommand command)
        {
            if (ExecuteCustomInterpreter(command))
            {
                return;
            }

            builder.AppendFormat("drop index {0} ON {1}",
                _dataBase.QuoteForColumnName(PrefixTableName(command.IndexName)),
                _dataBase.QuoteForTableName(PrefixTableName(command.TableName)));
            _sqlStatements.Add(builder.ToString());
        }

        public void Visit(StringBuilder builder, AddUniqueConstraintCommand command)
        {
            if (ExecuteCustomInterpreter(command))
            {
                return;
            }

            builder.AppendFormat("alter table {0} add constraint {1} unique ({2})",
                _dataBase.QuoteForTableName(PrefixTableName(command.TableName)),
                _dataBase.QuoteForColumnName(PrefixTableName(command.ConstraintName)),
                String.Join(", ", command.ColumnNames));

            _sqlStatements.Add(builder.ToString());
        }

        public void Visit(StringBuilder builder, DropUniqueConstraintCommand command)
        {
            if (ExecuteCustomInterpreter(command))
            {
                return;
            }

            builder.AppendFormat("alter table {0} drop constraint {1}",
                _dataBase.QuoteForTableName(PrefixTableName(command.TableName)),
                _dataBase.QuoteForColumnName(PrefixTableName(command.ConstraintName)));

            _sqlStatements.Add(builder.ToString());
        }

        public override void Visit(SqlStatementCommand command)
        {
          //  if (command.Providers.Count != 0 && !command.Providers.Contains(_dataBase.DatabaseType))
          //  {
           //     return;
          //  }

            if (ExecuteCustomInterpreter(command))
            {
                return;
            }
            _sqlStatements.Add(command.Sql);

            RunPendingStatements();
        }

        public override void Visit(CreateForeignKeyCommand command)
        {
            if (ExecuteCustomInterpreter(command))
            {
                return;
            }

            var builder = new StringBuilder();

            builder.Append("alter table ")
                .Append(_dataBase.QuoteForTableName(PrefixTableName(command.SrcTable)));

            builder.Append(_dataBase.GetAddForeignKeyConstraintString(PrefixTableName(command.Name),
                command.SrcColumns,
                _dataBase.QuoteForTableName(PrefixTableName(command.DestTable)),
                command.DestColumns,
                false));

            _sqlStatements.Add(builder.ToString());

            RunPendingStatements();
        }

        public override void Visit(DropForeignKeyCommand command)
        {
            if (ExecuteCustomInterpreter(command))
            {
                return;
            }

            var builder = new StringBuilder();

            builder.Append("alter table ")
                .Append(_dataBase.QuoteForTableName(PrefixTableName(command.SrcTable)))
                .Append(_dataBase.GetDropForeignKeyConstraintString(PrefixTableName(command.Name)));
            _sqlStatements.Add(builder.ToString());

            RunPendingStatements();
        }

        public static string GetTypeName( DbType dbType, int? length, byte precision, byte scale)
        {
            return precision > 0
                       ? _dataBase.GetTypeName(new SqlType(dbType, precision, scale))
                       : length.HasValue
                             ? _dataBase.GetTypeName(new SqlType(dbType, length.Value))
                             : _dataBase.GetTypeName(new SqlType(dbType));
        }

        private void Visit(StringBuilder builder, CreateColumnCommand command)
        {
            if (ExecuteCustomInterpreter(command))
            {
                return;
            }

            // name
            builder.Append(_dataBase.QuoteForColumnName(command.ColumnName)).Append(Space);

            if (!command.IsIdentity || _dataBase.HasDataTypeInIdentityColumn)
            {
                builder.Append(GetTypeName(command.DbType, command.Length, command.Precision, command.Scale));
            }

            // append identity if handled
            if (command.IsIdentity && _dataBase.SupportsIdentityColumns)
            {
                builder.Append(Space).Append(_dataBase.IdentityColumnString);
            }

            // [default value]
            if (command.Default != null)
            {
                builder.Append(" default ").Append(ConvertToSqlValue(command.Default)).Append(Space);
            }

            // nullable
            builder.Append(command.IsNotNull
                               ? " not null"
                               : !command.IsPrimaryKey && !command.IsUnique
                                     ? _dataBase.NullColumnString
                                     : string.Empty);

            // append unique if handled, otherwise at the end of the satement
            if (command.IsUnique && _dataBase.SupportsUnique)
            {
                builder.Append(" unique");
            }

        }

        [SuppressMessage("Microsoft.Security", "CA2100:Review SQL queries for security vulnerabilities", Justification = "Nothing comes from user input.")]
        private void RunPendingStatements()
        {

            //var session = _transactionManager.GetSession();

            try
            {
                _dataBase.BeginTransaction();
                foreach (var sqlStatement in _sqlStatements)
                {
                    _dataBase.ExecuteSql(sqlStatement);
                    Logger.Log(sqlStatement, Category.Info, Priority.Low);

                }
                _dataBase.CommitTransaction();
            }
            catch(Exception e)
            {
                _dataBase.RollbackTransaction();
                Logger.Log(e.Message,Category.Exception,Priority.High);
            }
            finally
            {
                _sqlStatements.Clear();
            }
        }

        private bool ExecuteCustomInterpreter<T>(T command) where T : ISchemaBuilderCommand
        {
          //  Database.Enum.DatabaseType dbtype=Database.Enum.DatabaseType.SqlCe;
            var interpreter = _commandInterpreters
                .Where(ici=>ici.DataProvider==_dataBase.DatabaseType)
                .OfType<ICommandInterpreter<T>>()
                .FirstOrDefault();

            if (interpreter != null)
            {
                _sqlStatements.AddRange(interpreter.CreateStatements(command));
                RunPendingStatements();
                return true;
            }

            return false;
        }

        public string ConvertToSqlValue(object value)
        {
            if (value == null)
            {
                return "null";
            }

            TypeCode typeCode = Type.GetTypeCode(value.GetType());
            switch (typeCode)
            {
                case TypeCode.Empty:
                case TypeCode.Object:
                case TypeCode.DBNull:
                case TypeCode.String:
                case TypeCode.Char:
                    return String.Concat("'", Convert.ToString(value).Replace("'", "''"), "'");
                case TypeCode.Boolean:
                    return _dataBase.ToBooleanValueString((bool)value);
                case TypeCode.SByte:
                case TypeCode.Int16:
                case TypeCode.UInt16:
                case TypeCode.Int32:
                case TypeCode.UInt32:
                case TypeCode.Int64:
                case TypeCode.UInt64:
                case TypeCode.Single:
                case TypeCode.Double:
                case TypeCode.Decimal:
                    return Convert.ToString(value, CultureInfo.InvariantCulture);
                case TypeCode.DateTime:
                    return String.Concat("'", ((DateTime)value).ToString("yyyy-MM-dd hh:mm:ss", CultureInfo.InvariantCulture), "'");
            }

            return "null";
        }
    }
}
