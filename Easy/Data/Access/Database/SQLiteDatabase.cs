using System;
using System.Data.SQLite;
using Easy.Data.Database.Core;
using Easy.Data.Database.Enum;
using System.Data;
using System.Text;
namespace Easy.Data.Access.Database
{
    public class SQLiteDatabase : CommonDatabase
    {
        private SQLiteTransaction _transaction; 
        

        public SQLiteDatabase(DatabaseType databaseType = DatabaseType.SQLite)
            : base(databaseType)
        {
            if (_connection == null)
            {
                throw new Exception("not get a connection!");
            }
            if(!_connection.IsConnected)
            {
                throw new Exception(GetConnectionBase().ErrorMsg);
            }
            RegisterColumnType(DbType.Binary, "BLOB");
            RegisterColumnType(DbType.Byte, "TINYINT");
            RegisterColumnType(DbType.Int16, "SMALLINT");
            RegisterColumnType(DbType.Int32, "INT");
            RegisterColumnType(DbType.Int64, "BIGINT");
            RegisterColumnType(DbType.SByte, "INTEGER");
            RegisterColumnType(DbType.UInt16, "INTEGER");
            RegisterColumnType(DbType.UInt32, "INTEGER");
            RegisterColumnType(DbType.UInt64, "INTEGER");
            RegisterColumnType(DbType.Currency, "NUMERIC");
            RegisterColumnType(DbType.Decimal, "NUMERIC");
            RegisterColumnType(DbType.Double, "DOUBLE");
            RegisterColumnType(DbType.Single, "DOUBLE");
            RegisterColumnType(DbType.VarNumeric, "NUMERIC");
            RegisterColumnType(DbType.AnsiString, "TEXT");
            RegisterColumnType(DbType.String, "TEXT");
            RegisterColumnType(DbType.AnsiStringFixedLength, "TEXT");
            RegisterColumnType(DbType.StringFixedLength, "TEXT");

            RegisterColumnType(DbType.Date, "DATE");
            RegisterColumnType(DbType.DateTime, "DATETIME");
            RegisterColumnType(DbType.Time, "TIME");
            RegisterColumnType(DbType.Boolean, "BOOL");
            RegisterColumnType(DbType.Guid, "UNIQUEIDENTIFIER");

            RegisterKeyword("int"); // Used in our function templates.
            RegisterKeyword("integer"); // a commonly-used alias for 'int'
            RegisterKeyword("tinyint");
            RegisterKeyword("smallint");
            RegisterKeyword("bigint");
            RegisterKeyword("numeric");
            RegisterKeyword("decimal");
            RegisterKeyword("bit");
            RegisterKeyword("money");
            RegisterKeyword("smallmoney");
            RegisterKeyword("float");
            RegisterKeyword("real");
            RegisterKeyword("datetime");
            RegisterKeyword("smalldatetime");
            RegisterKeyword("char");
            RegisterKeyword("varchar");
            RegisterKeyword("text");
            RegisterKeyword("nchar");
            RegisterKeyword("nvarchar");
            RegisterKeyword("ntext");
            RegisterKeyword("binary");
            RegisterKeyword("varbinary");
            RegisterKeyword("image");
            RegisterKeyword("cursor");
            RegisterKeyword("timestamp");
            RegisterKeyword("uniqueidentifier");
            RegisterKeyword("sql_variant");
        }

        public override void BeginTransaction()
        {

            _transaction = GetConnectionBase().GetRealConnection<SQLiteTransaction>();
        }

        public override void CommitTransaction()
        {
            if(_transaction!=null)
            {
                _transaction.Commit();
            }
            GetConnectionBase().EndUse();
        }

        public override bool ExecuteSql(string strSql)
        {
            SQLiteCommand cmd = new SQLiteCommand();
            cmd.CommandText = strSql;
            cmd.Connection = GetConnectionBase().GetRealConnection<SQLiteConnection>();
            bool bResult = cmd.ExecuteNonQuery() > 0;
            GetConnectionBase().EndUse();
            return bResult;
        }

        public override object Query(string strSql)
        {
            SQLiteDataAdapter dataAdapter = new SQLiteDataAdapter(strSql,
                GetConnectionBase().GetRealConnection<SQLiteConnection>());
            DataTable dt = new DataTable();
            dataAdapter.Fill(dt);
            GetConnectionBase().EndUse();
            return dt;
        }

        public override void RollbackTransaction()
        {
            if(_transaction!=null)
            {
                _transaction.Rollback();
            }
            GetConnectionBase().EndUse();
        }

        public override string GetFieldValue(object val, Type fieldType)
        {

            if(fieldType==typeof(DateTime))
            {
                return string.Format("'{0}'", ((DateTime)val).ToString("yyyy-MM-dd HH:mm:ss"));
            }
            //其它格式的支持
            if (fieldType == typeof(Boolean))
            {
                return string.Format("{0}", ((Boolean)val) ? "1" : "0");
            }
            if (val == null || string.IsNullOrEmpty(val.ToString()))
            {
                return "NULL";
            }
            return base.GetFieldValue(val, fieldType);
        }




        public override string AddColumnString
        {
            get
            {
                return "add column";
            }
        }

        public override string IdentitySelectString
        {
            get { return "select last_insert_rowid()"; }
        }



        public override bool SupportsInsertSelectIdentity
        {
            get { return true; }
        }

        public override bool DropConstraints
        {
            get { return false; }
        }

        public override string ForUpdateString
        {
            get { return string.Empty; }
        }

        public override bool SupportsSubSelects
        {
            get { return true; }
        }

        public override bool SupportsIfExistsBeforeTableName
        {
            get { return true; }
        }

        public override bool HasDataTypeInIdentityColumn
        {
            get { return false; }
        }

        public override bool SupportsIdentityColumns
        {
            get { return true; }
        }

        public override bool SupportsLimit
        {
            get { return true; }
        }

        public override bool SupportsLimitOffset
        {
            get { return true; }
        }

        public override string IdentityColumnString
        {
            get
            {
                // Adding the "autoincrement" keyword ensures that the same id will
                // not be generated twice.  When just utilizing "integer primary key",
                // SQLite just takes the max value currently in the table and adds one.
                // This causes problems with caches that use primary keys of deleted
                // entities.
                return "integer primary key autoincrement";//
            }
        }

        public override bool GenerateTablePrimaryKeyConstraintForIdentityColumn
        {
            get { return false; }
        }

        public override string Qualify(string catalog, string schema, string table)
        {
            StringBuilder qualifiedName = new StringBuilder();
            bool quoted = false;

            if (!string.IsNullOrEmpty(catalog))
            {
                if (catalog.StartsWith(OpenQuote.ToString()))
                {
                    catalog = catalog.Substring(1, catalog.Length - 1);
                    quoted = true;
                }
                if (catalog.EndsWith(CloseQuote.ToString()))
                {
                    catalog = catalog.Substring(0, catalog.Length - 1);
                    quoted = true;
                }
                qualifiedName.Append(catalog).Append(StringHelper.Underscore);
            }
            if (!string.IsNullOrEmpty(schema))
            {
                if (schema.StartsWith(OpenQuote.ToString()))
                {
                    schema = schema.Substring(1, schema.Length - 1);
                    quoted = true;
                }
                if (schema.EndsWith(CloseQuote.ToString()))
                {
                    schema = schema.Substring(0, schema.Length - 1);
                    quoted = true;
                }
                qualifiedName.Append(schema).Append(StringHelper.Underscore);
            }

            if (table.StartsWith(OpenQuote.ToString()))
            {
                table = table.Substring(1, table.Length - 1);
                quoted = true;
            }
            if (table.EndsWith(CloseQuote.ToString()))
            {
                table = table.Substring(0, table.Length - 1);
                quoted = true;
            }

            string name = qualifiedName.Append(table).ToString();
            if (quoted)
                return OpenQuote + name + CloseQuote;
            return name;

        }

        public override string NoColumnsInsertString
        {
            get { return "DEFAULT VALUES"; }
        }


        public override bool SupportsTemporaryTables
        {
            get { return true; }
        }

        public override string CreateTemporaryTableString
        {
            get { return "create temp table"; }
        }

        public override bool DropTemporaryTableAfterUse()
        {
            return true;
        }

        public override string SelectGUIDString
        {
            get { return "select randomblob(16)"; }
        }

        /// <summary>
        /// SQLite does not currently support dropping foreign key constraints by alter statements.
        /// This means that tables cannot be dropped if there are any rows that depend on those.
        /// If there are cycles between tables, it would even be excessively difficult to delete
        /// the data in the right order first.  Because of this, we just turn off the foreign
        /// constraints before we drop the schema and hope that we're not going to break anything. :(
        /// We could theoretically check for data consistency afterwards, but we don't currently.
        /// </summary>
        public override string DisableForeignKeyConstraintsString
        {
            get { return "PRAGMA foreign_keys = OFF"; }
        }

        public override string EnableForeignKeyConstraintsString
        {
            get { return "PRAGMA foreign_keys = ON"; }
        }

        public override bool SupportsForeignKeyConstraintInAlterTable
        {
            get { return false; }
        }
    }
}
