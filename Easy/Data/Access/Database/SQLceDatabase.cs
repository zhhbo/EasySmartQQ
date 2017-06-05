using System;
using System.Data.SqlServerCe;
using Easy.Data.Database.Core;
using Easy.Data.Database.Enum;
using System.Data;
using System.Data.SqlTypes;
using System.Text;
using Prism.Logging;
namespace Easy.Data.Access.Database
{
    public class SqlCeDatabase : CommonDatabase
    {
        private SqlCeTransaction _transaction; 
        

        public SqlCeDatabase(ILoggerFacade logger,DatabaseType databaseType = DatabaseType.SqlCe)
            : base(databaseType)
        {
            Logger = logger;
            if (_connection == null)
            {
                throw new Exception("not get a connection!");
            }
            if(!_connection.IsConnected)
            {
                throw new Exception(GetConnectionBase().ErrorMsg);
            }
            RegisterColumnType(DbType.AnsiStringFixedLength, "NCHAR(255)");
            RegisterColumnType(DbType.AnsiStringFixedLength, 4000, "NCHAR($l)");
            RegisterColumnType(DbType.AnsiString, "NVARCHAR(255)");
            RegisterColumnType(DbType.AnsiString, 4000, "NVARCHAR($l)");
            RegisterColumnType(DbType.AnsiString, 1073741823, "NTEXT");
            RegisterColumnType(DbType.Binary, "VARBINARY(8000)");
            RegisterColumnType(DbType.Binary, 8000, "VARBINARY($l)");
            RegisterColumnType(DbType.Binary, 1073741823, "IMAGE");
            RegisterColumnType(DbType.Boolean, "BIT");
            RegisterColumnType(DbType.Byte, "TINYINT");
            RegisterColumnType(DbType.Currency, "MONEY");
            RegisterColumnType(DbType.Date, "DATETIME");
            RegisterColumnType(DbType.DateTime, "DATETIME");
            RegisterColumnType(DbType.Decimal, "NUMERIC(19,5)");
            RegisterColumnType(DbType.Decimal, 19, "NUMERIC($p, $s)");
            RegisterColumnType(DbType.Double, "FLOAT");
            RegisterColumnType(DbType.Guid, "UNIQUEIDENTIFIER");
            RegisterColumnType(DbType.Int16, "SMALLINT");
            RegisterColumnType(DbType.Int32, "INT");
            RegisterColumnType(DbType.Int64, "BIGINT");
            RegisterColumnType(DbType.Single, "REAL"); //synonym for FLOAT(24) 
            RegisterColumnType(DbType.StringFixedLength, "NCHAR(255)");
            RegisterColumnType(DbType.StringFixedLength, 4000, "NCHAR($l)");
            RegisterColumnType(DbType.String, "NVARCHAR(255)");
            RegisterColumnType(DbType.String, 4000, "NVARCHAR($l)");
            RegisterColumnType(DbType.String, 1073741823, "NTEXT");
            RegisterColumnType(DbType.Time, "DATETIME");
        }
        private  ILoggerFacade Logger { get; set; }
        public override void BeginTransaction()
        {

            _transaction = GetConnectionBase().GetRealConnection<SqlCeTransaction>();//.BeginTransaction();
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
            SqlCeCommand cmd = new SqlCeCommand();
            cmd.CommandText = strSql;
            cmd.Connection = GetConnectionBase().GetRealConnection<SqlCeConnection>();
            bool bResult = cmd.ExecuteNonQuery() > 0;
            GetConnectionBase().EndUse();
            Logger.Log(strSql,Category.Debug,Priority.High);
            return bResult;
        }

        public override object Query(string strSql)
        {
            SqlCeDataAdapter dataAdapter = new SqlCeDataAdapter(strSql,
                GetConnectionBase().GetRealConnection<SqlCeConnection>());
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
            if (fieldType == typeof(Boolean))
           {
                return string.Format("{0}", ((Boolean)val)?"1":"0");
            }
            if (val == null || string.IsNullOrEmpty(val.ToString()))
            {
                return "NULL";
            }
            //其它格式的支持

            return base.GetFieldValue(val, fieldType);
        }
        public override bool SupportsLimitOffset => true;

        public override bool SupportsVariableLimit => true;

        public override string AddColumnString => " add ";

        public override string NullColumnString => " null";


        public override bool QualifyIndexName => false;


        public override string ForUpdateString => string.Empty;


        public override bool SupportsIdentityColumns => true;


        public override string IdentitySelectString
        {
            get { return "select @@IDENTITY"; }
        }

        public override string IdentityColumnString
        {
            get { return "IDENTITY NOT NULL"; }
        }

        public override string SelectGUIDString
        {
            get { return "select newid()"; }
        }

        public override bool SupportsLimit => true;



        public override bool SupportsCircularCascadeDeleteConstraints => false;





        public override string Qualify(string catalog, string schema, string table)
        {
            // SQL Server Compact doesn't support Schemas. So join schema name and table name with underscores
            // similar to the SQLLite dialect.

            var qualifiedName = new StringBuilder();
            bool quoted = false;

            if (!string.IsNullOrEmpty(catalog))
            {
                qualifiedName.Append(catalog).Append(StringHelper.Dot);
            }

            var tableName = new StringBuilder();
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
                tableName.Append(schema).Append(StringHelper.Underscore);
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

            string name = tableName.Append(table).ToString();
            if (quoted)
                name = OpenQuote + name + CloseQuote;
            return qualifiedName.Append(name).ToString();
        }



        public override long TimestampResolutionInTicks
        {
            get
            {
                // MS SQL resolution is actually 3.33 ms, rounded here to 10 ms
                return TimeSpan.TicksPerMillisecond * 10L;
            }
        }
    }
}
