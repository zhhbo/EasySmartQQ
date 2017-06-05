using System;
using System.Data;
using Easy.Data.Common.Interface;
using Easy.Data.Database.Enum;
using System.Collections;
using System.Collections.Generic;
using System.Data.SqlTypes;
using System.Text;
namespace Easy.Data.Database.Core
{
    public abstract class CommonDatabase : IDatabase
    {
        protected IConnection _connection;

        protected bool _canUse;

        public bool CanUse { get { return _canUse; } }

        private DatabaseType _dbType;
        public DatabaseType DatabaseType { get { return _dbType; }  }

        protected CommonDatabase(DatabaseType databaseType)
        {
            AttachConnection(databaseType);
            _dbType = databaseType;

            // register hibernate types for default use in scalar sqlquery type auto detection
            RegisterHibernateType(System.Data.DbType.Int64, "Int64");
            RegisterHibernateType(System.Data.DbType.Binary, "Byte[]");
            RegisterHibernateType(System.Data.DbType.Boolean, "Boolean");
            RegisterHibernateType(System.Data.DbType.AnsiString, "Char");
            RegisterHibernateType(System.Data.DbType.Date, "Date");
            RegisterHibernateType(System.Data.DbType.Double, "Double");
            RegisterHibernateType(System.Data.DbType.Single, "Single");
            RegisterHibernateType(System.Data.DbType.Int32, "Int32");
            RegisterHibernateType(System.Data.DbType.Int16, "Int16");
            RegisterHibernateType(System.Data.DbType.SByte, "SByte");
            RegisterHibernateType(System.Data.DbType.Time, "Time");
            RegisterHibernateType(System.Data.DbType.DateTime, "Timestamp");
            RegisterHibernateType(System.Data.DbType.String, "String");
            RegisterHibernateType(System.Data.DbType.VarNumeric, "Decimal");
            RegisterHibernateType(System.Data.DbType.Decimal, "Decimal");
        }

        protected virtual void AttachConnection(DatabaseType databaseType)
        {
            _connection = ConnectionManager.GetConnection(databaseType, out _canUse);
        }

        public void Close()
        {
            _canUse = false;
            if (_connection != null)
            {
                ConnectionManager.ReturnConnection(DatabaseType, _connection);
            }
            _connection = null;
        }

        ~CommonDatabase()
        {
            Close();
        }

        protected DbConnectionBase GetConnectionBase()
        {
            if(_connection!=null)
            {
                return _connection as DbConnectionBase;
            }
            return null;
        }

        public abstract object Query(string strSql);

        public abstract bool ExecuteSql(string strSql);

        public abstract void BeginTransaction();

        public abstract void CommitTransaction();

        public abstract void RollbackTransaction();

        public virtual long GetNextId(string idFieldName,string tableName)
        {
            string strSql = string.Format("select max({0}) from {1}", idFieldName, tableName);
            DataTable dt = Query(strSql) as DataTable;
            long id = -1;
            if (dt != null && dt.Rows.Count > 0)
            {
                if (dt.Rows[0].ItemArray[0] is DBNull)
                {
                    id = 1;
                }
                else
                {
                    id = Convert.ToInt64(dt.Rows[0].ItemArray[0]) + 1;
                }
            }
            return id;
        }

        public virtual string GetFieldValue(object val, Type fieldType)
        {
            if (val == null || string.IsNullOrEmpty(val.ToString()))
            {
                return "NULL";
            }
            if (fieldType == typeof(int))
            {
                return val.ToString();
            }
            if (fieldType == typeof(long))
            {
                return val.ToString();
            }
            if (fieldType == typeof(char))
            {
                return string.Format("'{0}'", val);
            }
            if (fieldType == typeof(double))
            {
                return val.ToString();
            }
            if (fieldType == typeof(decimal))
            {
                return val.ToString();
            }
            if (fieldType == typeof(string))
            {
                return string.Format("'{0}'", val.ToString().Replace("'", "\"").Replace("\r\n",""));
            }
            //datetime类型需要具体的database类实现
            return string.Empty;
        }


 

        protected const string DefaultBatchSize = "15";
        protected const string NoBatch = "0";

        /// <summary> Characters used for quoting sql identifiers </summary>
        public const string PossibleQuoteChars = "`'\"[";

        /// <summary> Characters used for closing quoted sql identifiers </summary>
        public const string PossibleClosedQuoteChars = "`'\"]";

        private readonly TypeNames _typeNames = new TypeNames();
        private readonly TypeNames _hibernateTypeNames = new TypeNames();
        private readonly IDictionary<string, string> _properties = new Dictionary<string, string>();
       // private readonly IDictionary<string, ISQLFunction> _sqlFunctions;
        private readonly HashSet<string> _sqlKeywords = new HashSet<string>();

      //  private static readonly IDictionary<string, ISQLFunction> StandardAggregateFunctions = CollectionHelper.CreateCaseInsensitiveHashtable<ISQLFunction>();

       // private static readonly IViolatedConstraintNameExtracter Extracter;

        #region Constructors and factory methods

      #endregion

        #region Database type mapping support

        /// <summary>
        /// Get the name of the database type associated with the given 
        /// <see cref="SqlTypes.SqlType"/>,
        /// </summary>
        /// <param name="sqlType">The SqlType</param>
        /// <returns>The database type name used by ddl.</returns>
        public virtual string GetTypeName(SqlType sqlType)
        {
            if (sqlType.LengthDefined || sqlType.PrecisionDefined)
            {
                string resultWithLength = _typeNames.Get(sqlType.DbType, sqlType.Length, sqlType.Precision, sqlType.Scale);
                if (resultWithLength != null) return resultWithLength;
            }

            string result = _typeNames.Get(sqlType.DbType);
            if (result == null)
            {
                throw new Exception(string.Format("No default type mapping for SqlType {0}", sqlType));
            }

            return result;
        }

        /// <summary>
        /// Get the name of the database type associated with the given
        /// <see cref="SqlType"/>.
        /// </summary>
        /// <param name="sqlType">The SqlType </param>
        /// <param name="length">The datatype length </param>
        /// <param name="precision">The datatype precision </param>
        /// <param name="scale">The datatype scale </param>
        /// <returns>The database type name used by ddl.</returns>
        public virtual string GetTypeName(SqlType sqlType, int length, int precision, int scale)
        {
            string result = _typeNames.Get(sqlType.DbType, length, precision, scale);
            if (result == null)
            {
                throw new Exception(string.Format("No type mapping for SqlType {0} of length {1}", sqlType, length));
            }
            return result;
        }

        /// <summary>
        /// Gets the name of the longest registered type for a particular DbType.
        /// </summary>
        /// <param name="dbType"></param>
        /// <returns></returns>
        public virtual string GetLongestTypeName(DbType dbType)
        {
            return _typeNames.GetLongest(dbType);
        }


        /// <summary>
        /// Subclasses register a typename for the given type code and maximum
        /// column length. <c>$l</c> in the type name will be replaced by the column
        /// length (if appropriate)
        /// </summary>
        /// <param name="code">The typecode</param>
        /// <param name="capacity">Maximum length of database type</param>
        /// <param name="name">The database type name</param>
        protected void RegisterColumnType(DbType code, int capacity, string name)
        {
            _typeNames.Put(code, capacity, name);
        }

        /// <summary>
        /// Subclasses register a typename for the given type code. <c>$l</c> in the 
        /// typename will be replaced by the column length (if appropriate).
        /// </summary>
        /// <param name="code">The typecode</param>
        /// <param name="name">The database type name</param>
        protected void RegisterColumnType(DbType code, string name)
        {
            _typeNames.Put(code, name);
        }

        #endregion

        #region DDL support

        /// <summary>
        /// Do we need to drop constraints before dropping tables in the dialect?
        /// </summary>
        public virtual bool DropConstraints
        {
            get { return true; }
        }

        /// <summary>
        /// Do we need to qualify index names with the schema name?
        /// </summary>
        public virtual bool QualifyIndexName
        {
            get { return true; }
        }

        /// <summary>
        /// Does this dialect support the <c>UNIQUE</c> column syntax?
        /// </summary>
        public virtual bool SupportsUnique
        {
            get { return true; }
        }

        /// <summary> Does this dialect support adding Unique constraints via create and alter table ?</summary>
        public virtual bool SupportsUniqueConstraintInCreateAlterTable
        {
            get { return true; }
        }

        /// <summary>
        /// Does this dialect support adding foreign key constraints via alter table?  If not, it's assumed they can only be added through create table.
        /// </summary>
        public virtual bool SupportsForeignKeyConstraintInAlterTable
        {
            get { return true; }
        }

        /// <summary> 
        /// The syntax used to add a foreign key constraint to a table.  If SupportsForeignKeyConstraintInAlterTable is false, the returned string will be added to the create table statement instead.  In this case, extra strings, like "add", that apply when using alter table should be omitted.
        /// </summary>
        /// <param name="constraintName">The FK constraint name. </param>
        /// <param name="foreignKey">The names of the columns comprising the FK </param>
        /// <param name="referencedTable">The table referenced by the FK </param>
        /// <param name="primaryKey">The explicit columns in the referencedTable referenced by this FK. </param>
        /// <param name="referencesPrimaryKey">
        /// if false, constraint should be explicit about which column names the constraint refers to 
        /// </param>
        /// <returns> the "add FK" fragment </returns>
        public virtual string GetAddForeignKeyConstraintString(string constraintName, string[] foreignKey, string referencedTable, string[] primaryKey, bool referencesPrimaryKey)
        {
            var res = new StringBuilder(200);

            if (SupportsForeignKeyConstraintInAlterTable)
                res.Append(" add");

            res.Append(" constraint ")
                .Append(constraintName)
                .Append(" foreign key (")
                .Append(StringHelper.Join(StringHelper.CommaSpace, foreignKey))
                .Append(") references ")
                .Append(referencedTable);

            if (!referencesPrimaryKey)
            {
                res.Append(" (")
                    .Append(StringHelper.Join(StringHelper.CommaSpace, primaryKey))
                    .Append(')');
            }

            return res.ToString();
        }

        /// <summary>
        /// The syntax used to add a primary key constraint to a table
        /// </summary>
        /// <param name="constraintName"></param>
        public virtual string GetAddPrimaryKeyConstraintString(string constraintName)
        {
            return " add constraint " + constraintName + " primary key ";
        }

        public virtual bool HasSelfReferentialForeignKeyBug
        {
            get { return false; }
        }

        public virtual bool SupportsCommentOn
        {
            get { return false; }
        }

        public virtual string GetTableComment(string comment)
        {
            return string.Empty;
        }

        public virtual string GetColumnComment(string comment)
        {
            return string.Empty;
        }

        /// <summary>
        /// Does the dialect support the syntax 'drop table if exists NAME'
        /// </summary>
        public virtual bool SupportsIfExistsBeforeTableName
        {
            get { return false; }
        }

        /// <summary>
        /// Does the dialect support the syntax 'drop table NAME if exists'
        /// </summary>
        public virtual bool SupportsIfExistsAfterTableName
        {
            get { return false; }
        }

        /// <summary> Does this dialect support column-level check constraints? </summary>
        /// <returns> True if column-level CHECK constraints are supported; false otherwise. </returns>
        public virtual bool SupportsColumnCheck
        {
            get { return true; }
        }

        /// <summary> Does this dialect support table-level check constraints? </summary>
        /// <returns> True if table-level CHECK constraints are supported; false otherwise. </returns>
        public virtual bool SupportsTableCheck
        {
            get { return true; }
        }

        public virtual bool SupportsCascadeDelete
        {
            get { return true; }
        }

        public virtual bool SupportsNotNullUnique
        {
            get { return true; }
        }


        #endregion

        #region Lock acquisition support

        /// <summary>
        /// Get the string to append to SELECT statements to acquire locks
        /// for this dialect.
        /// </summary>
        /// <value>The appropriate <c>FOR UPDATE</c> clause string.</value>
        public virtual string ForUpdateString
        {
            get { return " for update"; }
        }

        /// <summary> Is <tt>FOR UPDATE OF</tt> syntax supported? </summary>
        /// <value> True if the database supports <tt>FOR UPDATE OF</tt> syntax; false otherwise. </value>
        public virtual bool ForUpdateOfColumns
        {
            // by default we report no support
            get { return false; }
        }

        /// <summary> 
        /// Does this dialect support <tt>FOR UPDATE</tt> in conjunction with outer joined rows?
        /// </summary>
        /// <value> True if outer joined rows can be locked via <tt>FOR UPDATE</tt>. </value>
        public virtual bool SupportsOuterJoinForUpdate
        {
            get { return true; }
        }

        /// <summary> 
        /// Get the <tt>FOR UPDATE OF column_list</tt> fragment appropriate for this
        /// dialect given the aliases of the columns to be write locked.
        ///  </summary>
        /// <param name="aliases">The columns to be write locked. </param>
        /// <returns> The appropriate <tt>FOR UPDATE OF column_list</tt> clause string. </returns>
        public virtual string GetForUpdateString(string aliases)
        {
            // by default we simply return the ForUpdateString result since
            // the default is to say no support for "FOR UPDATE OF ..."
            return ForUpdateString;
        }

        /// <summary>
        /// Retrieves the <c>FOR UPDATE NOWAIT</c> syntax specific to this dialect
        /// </summary>
        /// <value>The appropriate <c>FOR UPDATE NOWAIT</c> clause string.</value>
        public virtual string ForUpdateNowaitString
        {
            // by default we report no support for NOWAIT lock semantics
            get { return ForUpdateString; }
        }

        /// <summary> 
        /// Get the <tt>FOR UPDATE OF column_list NOWAIT</tt> fragment appropriate
        /// for this dialect given the aliases of the columns to be write locked.
        /// </summary>
        /// <param name="aliases">The columns to be write locked. </param>
        /// <returns> The appropriate <tt>FOR UPDATE colunm_list NOWAIT</tt> clause string. </returns>
        public virtual string GetForUpdateNowaitString(string aliases)
        {
            return GetForUpdateString(aliases);
        }




        #endregion

        #region Table support

        /// <summary>
        /// Return SQL needed to drop the named table. May (and should) use
        /// some form of "if exists" clause, and cascade constraints.
        /// </summary>
        /// <param name="tableName"></param>
        /// <returns></returns>
        public virtual string GetDropTableString(string tableName)
        {
            var buf = new StringBuilder("drop table ");
            if (SupportsIfExistsBeforeTableName)
            {
                buf.Append("if exists ");
            }

            buf.Append(tableName).Append(CascadeConstraintsString);

            if (SupportsIfExistsAfterTableName)
            {
                buf.Append(" if exists");
            }
            return buf.ToString();
        }

        #region Temporary table support

        /// <summary> Does this dialect support temporary tables? </summary>
        public virtual bool SupportsTemporaryTables
        {
            get { return false; }
        }

        /// <summary> Generate a temporary table name given the bas table. </summary>
        /// <param name="baseTableName">The table name from which to base the temp table name. </param>
        /// <returns> The generated temp table name. </returns>
        public virtual string GenerateTemporaryTableName(string baseTableName)
        {
            return "HT_" + baseTableName;
        }

        /// <summary> 
        /// Does the dialect require that temporary table DDL statements occur in
        /// isolation from other statements?  This would be the case if the creation
        /// would cause any current transaction to get committed implicitly.
        ///  </summary>
        /// <returns> see the result matrix above. </returns>
        /// <remarks>
        /// JDBC defines a standard way to query for this information via the
        /// {@link java.sql.DatabaseMetaData#dataDefinitionCausesTransactionCommit()}
        /// method.  However, that does not distinguish between temporary table
        /// DDL and other forms of DDL; MySQL, for example, reports DDL causing a
        /// transaction commit via its driver, even though that is not the case for
        /// temporary table DDL.
        /// <p/>
        /// Possible return values and their meanings:<ul>
        /// <li>{@link Boolean#TRUE} - Unequivocally, perform the temporary table DDL in isolation.</li>
        /// <li>{@link Boolean#FALSE} - Unequivocally, do <b>not</b> perform the temporary table DDL in isolation.</li>
        /// <li><i>null</i> - defer to the JDBC driver response in regards to {@link java.sql.DatabaseMetaData#dataDefinitionCausesTransactionCommit()}</li>
        /// </ul>
        /// </remarks>
        public virtual bool? PerformTemporaryTableDDLInIsolation()
        {
            return null;
        }

        /// <summary> Do we need to drop the temporary table after use? </summary>
        public virtual bool DropTemporaryTableAfterUse()
        {
            return true;
        }

        #endregion

        #endregion


        #region Current timestamp support

        /// <summary> Does this dialect support a way to retrieve the database's current timestamp value? </summary>
        public virtual bool SupportsCurrentTimestampSelection
        {
            get { return false; }
        }

        /// <summary>
        /// Gives the best resolution that the database can use for storing
        /// date/time values, in ticks.
        /// </summary>
        /// <remarks>
        /// <para>
        /// For example, if the database can store values with 100-nanosecond
        /// precision, this property is equal to 1L. If the database can only
        /// store values with 1-millisecond precision, this property is equal
        /// to 10000L (number of ticks in a millisecond).
        /// </para>
        /// <para>
        /// Used in TimestampType.
        /// </para>
        /// </remarks>
        public virtual long TimestampResolutionInTicks
        {
            get { return 1L; } // Maximum precision (one tick)
        }

        #endregion

        #region Constraint support

        /// <summary>
        /// The syntax used to drop a foreign key constraint from a table.
        /// </summary>
        /// <param name="constraintName">The name of the foreign key constraint to drop.</param>
        /// <returns>
        /// The SQL string to drop the foreign key constraint.
        /// </returns>
        public virtual string GetDropForeignKeyConstraintString(string constraintName)
        {
            return " drop constraint " + constraintName;
        }

        /// <summary>
        /// The syntax used to drop a primary key constraint from a table.
        /// </summary>
        /// <param name="constraintName">The name of the primary key constraint to drop.</param>
        /// <returns>
        /// The SQL string to drop the primary key constraint.
        /// </returns>
        public virtual string GetDropPrimaryKeyConstraintString(string constraintName)
        {
            return " drop constraint " + constraintName;
        }

        /// <summary>
        /// The syntax used to drop an index constraint from a table.
        /// </summary>
        /// <param name="constraintName">The name of the index constraint to drop.</param>
        /// <returns>
        /// The SQL string to drop the primary key constraint.
        /// </returns>
        public virtual string GetDropIndexConstraintString(string constraintName)
        {
            return " drop constraint " + constraintName;
        }

        /// <summary>
        /// Completely optional cascading drop clause
        /// </summary>
        public virtual string CascadeConstraintsString
        {
            get { return String.Empty; }
        }

        /// <summary> Only needed if the Dialect does not have SupportsForeignKeyConstraintInAlterTable. </summary>
        public virtual string DisableForeignKeyConstraintsString
        {
            get { return null; }
        }

        /// <summary> Only needed if the Dialect does not have SupportsForeignKeyConstraintInAlterTable. </summary>
        public virtual string EnableForeignKeyConstraintsString
        {
            get { return null; }
        }

        #endregion

        #region Native identifier generation

        #region IDENTITY support

        /// <summary>
        /// Does this dialect support identity column key generation?
        /// </summary>
        public virtual bool SupportsIdentityColumns
        {
            get { return false; }
        }

        /// <summary> 
        /// Does the dialect support some form of inserting and selecting
        /// the generated IDENTITY value all in the same statement.
        ///  </summary>
        public virtual bool SupportsInsertSelectIdentity
        {
            get { return false; }
        }

        /// <summary>
        /// Whether this dialect has an identity clause added to the data type or a
        /// completely separate identity data type.
        /// </summary>
        public virtual bool HasDataTypeInIdentityColumn
        {
            get { return true; }
        }



        /// <summary> 
        /// Get the select command to use to retrieve the last generated IDENTITY
        /// value for a particular table 
        /// </summary>
        /// <param name="tableName">The table into which the insert was done </param>
        /// <param name="identityColumn">The PK column. </param>
        /// <param name="type">The <see cref="DatabaseType"/> type code. </param>
        /// <returns> The appropriate select command </returns>
        public virtual string GetIdentitySelectString(string identityColumn, string tableName, DbType type)
        {
            return IdentitySelectString;
        }

        /// <summary> 
        /// Get the select command to use to retrieve the last generated IDENTITY value.
        /// </summary>
        /// <returns> The appropriate select command </returns>
        public virtual string IdentitySelectString
        {
            get { throw new Exception("Dialect does not support identity key generation"); }
        }

        /// <summary> 
        /// The syntax used during DDL to define a column as being an IDENTITY of
        /// a particular type. 
        /// </summary>
        /// <param name="type">The <see cref="DatabaseType"/> type code. </param>
        /// <returns> The appropriate DDL fragment. </returns>
        public virtual string GetIdentityColumnString(DbType type)
        {
            return IdentityColumnString;
        }

        /// <summary>
        /// The keyword used to specify an identity column, if native key generation is supported
        /// </summary>
        public virtual string IdentityColumnString
        {
            get { throw new Exception("Dialect does not support identity key generation"); }
        }

        /// <summary>
        /// Set this to false if no table-level primary key constraint should be generated when an identity column has been specified for the table.
        /// This is used as a work-around for SQLite so it doesn't tell us we have "more than one primary key".
        /// </summary>
        public virtual bool GenerateTablePrimaryKeyConstraintForIdentityColumn
        {
            get { return true; }
        }





        /// <summary>
        /// The keyword used to insert a generated value into an identity column (or null).
        /// Need if the dialect does not support inserts that specify no column values.
        /// </summary>
        public virtual string IdentityInsertString
        {
            get { return null; }
        }

        #endregion

        #region SEQUENCE support

        /// <summary>
        /// Does this dialect support sequences?
        /// </summary>
        public virtual bool SupportsSequences
        {
            get { return false; }
        }

        /// <summary> 
        /// Does this dialect support "pooled" sequences?
        /// </summary>
        /// <returns> True if such "pooled" sequences are supported; false otherwise. </returns>
        /// <remarks>
        /// A pooled sequence is one that has a configurable initial size and increment 
        /// size. It enables NHibernate to be allocated a pool/block/range of IDs,
        /// which can reduce the frequency of round trips to the database during ID
        /// generation.
        /// </remarks>
        /// <seealso cref="GetCreateSequenceStrings(string, int, int)"> </seealso>
        /// <seealso cref="GetCreateSequenceString(string, int, int)"> </seealso>
        public virtual bool SupportsPooledSequences
        {
            get { return false; }
        }

        /// <summary> 
        /// Generate the appropriate select statement to to retreive the next value
        /// of a sequence.
        /// </summary>
        /// <param name="sequenceName">the name of the sequence </param>
        /// <returns> String The "nextval" select string. </returns>
        /// <remarks>This should be a "stand alone" select statement.</remarks>
        public virtual string GetSequenceNextValString(string sequenceName)
        {
            throw new Exception("Dialect does not support sequences");
        }

        /// <summary> 
        /// Typically dialects which support sequences can drop a sequence
        /// with a single command.  
        /// </summary>
        /// <param name="sequenceName">The name of the sequence </param>
        /// <returns> The sequence drop commands </returns>
        /// <remarks>
        /// This is convenience form of <see cref="GetDropSequenceStrings"/>
        /// to help facilitate that.
        /// 
        /// Dialects which support sequences and can drop a sequence in a
        /// single command need *only* override this method.  Dialects
        /// which support sequences but require multiple commands to drop
        /// a sequence should instead override <see cref="GetDropSequenceStrings"/>. 
        /// </remarks>
        public virtual string GetDropSequenceString(string sequenceName)
        {
            throw new Exception("Dialect does not support sequences");
        }

        /// <summary> 
        /// The multiline script used to drop a sequence. 
        /// </summary>
        /// <param name="sequenceName">The name of the sequence </param>
        /// <returns> The sequence drop commands </returns>
        public virtual string[] GetDropSequenceStrings(string sequenceName)
        {
            return new string[] { GetDropSequenceString(sequenceName) };
        }

        /// <summary> 
        /// Generate the select expression fragment that will retrieve the next
        /// value of a sequence as part of another (typically DML) statement.
        /// </summary>
        /// <param name="sequenceName">the name of the sequence </param>
        /// <returns> The "nextval" fragment. </returns>
        /// <remarks>
        /// This differs from <see cref="GetSequenceNextValString"/> in that this
        /// should return an expression usable within another statement.
        /// </remarks>
        public virtual string GetSelectSequenceNextValString(string sequenceName)
        {
            throw new Exception("Dialect does not support sequences");
        }

        /// <summary> 
        /// Typically dialects which support sequences can create a sequence
        /// with a single command.
        /// </summary>
        /// <param name="sequenceName">The name of the sequence </param>
        /// <returns> The sequence creation command </returns>
        /// <remarks>
        /// This is convenience form of <see cref="GetCreateSequenceStrings(string,int,int)"/> to help facilitate that.
        /// Dialects which support sequences and can create a sequence in a
        /// single command need *only* override this method.  Dialects
        /// which support sequences but require multiple commands to create
        /// a sequence should instead override <see cref="GetCreateSequenceStrings(string,int,int)"/>.
        /// </remarks>
        public virtual string GetCreateSequenceString(string sequenceName)
        {
            throw new Exception("Dialect does not support sequences");
        }

        /// <summary> 
        /// An optional multi-line form for databases which <see cref="SupportsPooledSequences"/>. 
        /// </summary>
        /// <param name="sequenceName">The name of the sequence </param>
        /// <param name="initialValue">The initial value to apply to 'create sequence' statement </param>
        /// <param name="incrementSize">The increment value to apply to 'create sequence' statement </param>
        /// <returns> The sequence creation commands </returns>
        public virtual string[] GetCreateSequenceStrings(string sequenceName, int initialValue, int incrementSize)
        {
            return new string[] { GetCreateSequenceString(sequenceName, initialValue, incrementSize) };
        }

        /// <summary> 
        /// Overloaded form of <see cref="GetCreateSequenceString(string)"/>, additionally
        /// taking the initial value and increment size to be applied to the sequence
        /// definition.
        ///  </summary>
        /// <param name="sequenceName">The name of the sequence </param>
        /// <param name="initialValue">The initial value to apply to 'create sequence' statement </param>
        /// <param name="incrementSize">The increment value to apply to 'create sequence' statement </param>
        /// <returns> The sequence creation command </returns>
        /// <remarks>
        /// The default definition is to suffix <see cref="GetCreateSequenceString(string,int,int)"/>
        /// with the string: " start with {initialValue} increment by {incrementSize}" where
        /// {initialValue} and {incrementSize} are replacement placeholders.  Generally
        /// dialects should only need to override this method if different key phrases
        /// are used to apply the allocation information.
        /// </remarks>
        protected virtual string GetCreateSequenceString(string sequenceName, int initialValue, int incrementSize)
        {
            if (SupportsPooledSequences)
            {
                return GetCreateSequenceString(sequenceName) + " start with " + initialValue + " increment by " + incrementSize;
            }
            throw new Exception("Dialect does not support pooled sequences");
        }

        /// <summary> Get the select command used retrieve the names of all sequences.</summary>
        /// <returns> The select command; or null if sequences are not supported. </returns>
        public virtual string QuerySequencesString
        {
            get { return null; }
        }

        #endregion





        #endregion

        #region Miscellaneous support




        /// <summary> The SQL literal value to which this database maps boolean values. </summary>
        /// <param name="value">The boolean value </param>
        /// <returns> The appropriate SQL literal. </returns>
        public virtual string ToBooleanValueString(bool value)
        {
            return value ? "1" : "0";
        }



        #endregion

        #region Limit/offset support

        /// <summary>
        /// Does this Dialect have some kind of <c>LIMIT</c> syntax?
        /// </summary>
        /// <value>False, unless overridden.</value>
        public virtual bool SupportsLimit
        {
            get { return false; }
        }

        /// <summary>
        /// Does this Dialect support an offset?
        /// </summary>
        public virtual bool SupportsLimitOffset
        {
            get { return SupportsLimit; }
        }

        /// <summary>
        /// Can parameters be used for a statement containing a LIMIT?
        /// </summary>
        public virtual bool SupportsVariableLimit
        {
            get { return SupportsLimit; }
        }

        /// <summary> 
        /// Does the <tt>LIMIT</tt> clause take a "maximum" row number instead
        /// of a total number of returned rows?
        /// </summary>
        /// <returns> True if limit is relative from offset; false otherwise. </returns>
        /// <remarks>
        /// This is easiest understood via an example.  Consider you have a table
        /// with 20 rows, but you only want to retrieve rows number 11 through 20.
        /// Generally, a limit with offset would say that the offset = 11 and the
        /// limit = 10 (we only want 10 rows at a time); this is specifying the
        /// total number of returned rows.  Some dialects require that we instead
        /// specify offset = 11 and limit = 20, where 20 is the "last" row we want
        /// relative to offset (i.e. total number of rows = 20 - 11 = 9)
        /// So essentially, is limit relative from offset?  Or is limit absolute?
        /// </remarks>
        public virtual bool UseMaxForLimit
        {
            get { return false; }
        }

        /// <summary>
        /// For limit clauses, indicates whether to use 0 or 1 as the offset that returns the first row.  Should be true if the first row is at offset 1.
        /// </summary>
        public virtual bool OffsetStartsAtOne
        {
            get { return false; }
        }



        /// <summary>
        /// Some databases require that a limit statement contain the maximum row number
        /// instead of the number of rows to retrieve.  This method adjusts source
        /// limit and offset values to account for this.
        /// </summary>
        /// <param name="offset"></param>
        /// <param name="limit"></param>
        /// <returns></returns>
        public int GetLimitValue(int offset, int limit)
        {
            if (limit == int.MaxValue)
                return int.MaxValue;

            if (UseMaxForLimit)
                return GetOffsetValue(offset) + limit;

            return limit;
        }

        /// <summary>
        /// Some databases use limit row offsets that start at one instead of zero.
        /// This method adjusts a desired offset using the OffsetStartsAtOne flag.
        /// </summary>
        /// <param name="offset"></param>
        /// <returns></returns>
        public int GetOffsetValue(int offset)
        {
            if (OffsetStartsAtOne)
                return offset + 1;

            return offset;
        }

        #endregion

        #region Identifier quoting support

        /// <summary>
        /// The opening quote for a quoted identifier.
        /// </summary>
        public virtual char OpenQuote
        {
            get { return '"'; }
        }

        /// <summary>
        /// The closing quote for a quoted identifier.
        /// </summary>
        public virtual char CloseQuote
        {
            get { return '"'; }
        }

        /// <summary>
        /// Checks to see if the name has been quoted.
        /// </summary>
        /// <param name="name">The name to check if it is quoted</param>
        /// <returns>true if name is already quoted.</returns>
        /// <remarks>
        /// The default implementation is to compare the first character
        /// to Dialect.OpenQuote and the last char to Dialect.CloseQuote
        /// </remarks>
        public virtual bool IsQuoted(string name)
        {
            if (string.IsNullOrEmpty(name))
            {
                return false;
            }
            return (name[0] == OpenQuote && name[name.Length - 1] == CloseQuote);
        }

        public virtual string Qualify(string catalog, string schema, string table)
        {
            StringBuilder qualifiedName = new StringBuilder();

            if (!string.IsNullOrEmpty(catalog))
            {
                qualifiedName.Append(catalog).Append(StringHelper.Dot);
            }
            if (!string.IsNullOrEmpty(schema))
            {
                qualifiedName.Append(schema).Append(StringHelper.Dot);
            }
            return qualifiedName.Append(table).ToString();
        }

        /// <summary>
        /// Quotes a name.
        /// </summary>
        /// <param name="name">The string that needs to be Quoted.</param>
        /// <returns>A QuotedName </returns>
        /// <remarks>
        /// <p>
        /// This method assumes that the name is not already Quoted.  So if the name passed
        /// in is <c>"name</c> then it will return <c>"""name"</c>.  It escapes the first char
        /// - the " with "" and encloses the escaped string with OpenQuote and CloseQuote. 
        /// </p>
        /// </remarks>
        protected virtual string Quote(string name)
        {
            string quotedName = name.Replace(OpenQuote.ToString(), new string(OpenQuote, 2));

            // in some dbs the Open and Close Quote are the same chars - if they are 
            // then we don't have to escape the Close Quote char because we already
            // got it.
            if (OpenQuote != CloseQuote)
            {
                quotedName = name.Replace(CloseQuote.ToString(), new string(CloseQuote, 2));
            }

            return OpenQuote + quotedName + CloseQuote;
        }

        /// <summary>
        /// Quotes a name for being used as a aliasname
        /// </summary>
        /// <remarks>Original implementation calls <see cref="QuoteForTableName"/></remarks>
        /// <param name="aliasName">Name of the alias</param>
        /// <returns>A Quoted name in the format of OpenQuote + aliasName + CloseQuote</returns>
        /// <remarks>
        /// <p>
        /// If the aliasName is already enclosed in the OpenQuote and CloseQuote then this 
        /// method will return the aliasName that was passed in without going through any
        /// Quoting process.  So if aliasName is passed in already Quoted make sure that 
        /// you have escaped all of the chars according to your DataBase's specifications.
        /// </p>
        /// </remarks>
        public virtual string QuoteForAliasName(string aliasName)
        {
            return IsQuoted(aliasName) ? aliasName : Quote(aliasName);
        }

        /// <summary>
        /// Quotes a name for being used as a columnname
        /// </summary>
        /// <remarks>Original implementation calls <see cref="QuoteForTableName"/></remarks>
        /// <param name="columnName">Name of the column</param>
        /// <returns>A Quoted name in the format of OpenQuote + columnName + CloseQuote</returns>
        /// <remarks>
        /// <p>
        /// If the columnName is already enclosed in the OpenQuote and CloseQuote then this 
        /// method will return the columnName that was passed in without going through any
        /// Quoting process.  So if columnName is passed in already Quoted make sure that 
        /// you have escaped all of the chars according to your DataBase's specifications.
        /// </p>
        /// </remarks>
        public virtual string QuoteForColumnName(string columnName)
        {
            return IsQuoted(columnName) ? columnName : Quote(columnName);
        }

        /// <summary>
        /// Quotes a name for being used as a tablename
        /// </summary>
        /// <param name="tableName">Name of the table</param>
        /// <returns>A Quoted name in the format of OpenQuote + tableName + CloseQuote</returns>
        /// <remarks>
        /// <p>
        /// If the tableName is already enclosed in the OpenQuote and CloseQuote then this 
        /// method will return the tableName that was passed in without going through any
        /// Quoting process.  So if tableName is passed in already Quoted make sure that 
        /// you have escaped all of the chars according to your DataBase's specifications.
        /// </p>
        /// </remarks>
        public virtual string QuoteForTableName(string tableName)
        {
            return IsQuoted(tableName) ? tableName : Quote(tableName);
        }

        /// <summary>
        /// Quotes a name for being used as a schemaname
        /// </summary>
        /// <param name="schemaName">Name of the schema</param>
        /// <returns>A Quoted name in the format of OpenQuote + schemaName + CloseQuote</returns>
        /// <remarks>
        /// <p>
        /// If the schemaName is already enclosed in the OpenQuote and CloseQuote then this 
        /// method will return the schemaName that was passed in without going through any
        /// Quoting process.  So if schemaName is passed in already Quoted make sure that 
        /// you have escaped all of the chars according to your DataBase's specifications.
        /// </p>
        /// </remarks>
        public virtual string QuoteForSchemaName(string schemaName)
        {
            return IsQuoted(schemaName) ? schemaName : Quote(schemaName);
        }

        /// <summary>
        /// Unquotes and unescapes an already quoted name
        /// </summary>
        /// <param name="quoted">Quoted string</param>
        /// <returns>Unquoted string</returns>
        /// <remarks>
        /// <p>
        /// This method checks the string <c>quoted</c> to see if it is 
        /// quoted.  If the string <c>quoted</c> is already enclosed in the OpenQuote
        /// and CloseQuote then those chars are removed.
        /// </p>
        /// <p>
        /// After the OpenQuote and CloseQuote have been cleaned from the string <c>quoted</c>
        /// then any chars in the string <c>quoted</c> that have been escaped by doubling them
        /// up are changed back to a single version.
        /// </p>
        /// <p>
        /// The following quoted values return these results
        /// "quoted" = quoted
        /// "quote""d" = quote"d
        /// quote""d = quote"d 
        /// </p>
        /// <p>
        /// If this implementation is not sufficient for your Dialect then it needs to be overridden.
        /// MsSql2000Dialect is an example of where UnQuoting rules are different.
        /// </p>
        /// </remarks>
        public virtual string UnQuote(string quoted)
        {
            string unquoted;

            if (IsQuoted(quoted))
            {
                unquoted = quoted.Substring(1, quoted.Length - 2);
            }
            else
            {
                unquoted = quoted;
            }

            unquoted = unquoted.Replace(new string(OpenQuote, 2), OpenQuote.ToString());

            if (OpenQuote != CloseQuote)
            {
                unquoted = unquoted.Replace(new string(CloseQuote, 2), CloseQuote.ToString());
            }

            return unquoted;
        }

        /// <summary>
        /// Unquotes an array of Quoted Names.
        /// </summary>
        /// <param name="quoted">strings to Unquote</param>
        /// <returns>an array of unquoted strings.</returns>
        /// <remarks>
        /// This use UnQuote(string) for each string in the quoted array so
        /// it should not need to be overridden - only UnQuote(string) needs
        /// to be overridden unless this implementation is not sufficient.
        /// </remarks>
        public virtual string[] UnQuote(string[] quoted)
        {
            var unquoted = new string[quoted.Length];

            for (int i = 0; i < quoted.Length; i++)
            {
                unquoted[i] = UnQuote(quoted[i]);
            }

            return unquoted;
        }

        #endregion

        #region Union subclass support

        /// <summary> 
        /// Given a <see cref="DatabaseType"/> type code, determine an appropriate
        /// null value to use in a select clause.
        /// </summary>
        /// <param name="sqlType">The <see cref="DatabaseType"/> type code. </param>
        /// <returns> The appropriate select clause value fragment. </returns>
        /// <remarks>
        /// One thing to consider here is that certain databases might
        /// require proper casting for the nulls here since the select here
        /// will be part of a UNION/UNION ALL.
        /// </remarks>
        public virtual string GetSelectClauseNullString(SqlType sqlType)
        {
            return "null";
        }

        /// <summary> 
        /// Does this dialect support UNION ALL, which is generally a faster variant of UNION? 
        /// True if UNION ALL is supported; false otherwise.
        /// </summary>
        public virtual bool SupportsUnionAll
        {
            get { return false; }
        }

        #endregion

        #region Informational metadata

        /// <summary> 
        /// Does this dialect support empty IN lists?
        /// For example, is [where XYZ in ()] a supported construct?
        /// </summary>
        /// <returns> True if empty in lists are supported; false otherwise. </returns>
        public virtual bool SupportsEmptyInList
        {
            get { return true; }
        }

        /// <summary> 
        /// Are string comparisons implicitly case insensitive.
        /// In other words, does [where 'XYZ' = 'xyz'] resolve to true? 
        /// </summary>
        /// <returns> True if comparisons are case insensitive. </returns>
        public virtual bool AreStringComparisonsCaseInsensitive
        {
            get { return false; }
        }

        /// <summary> 
        /// Is this dialect known to support what ANSI-SQL terms "row value
        /// constructor" syntax; sometimes called tuple syntax.
        /// <p/>
        /// Basically, does it support syntax like
        /// "... where (FIRST_NAME, LAST_NAME) = ('Steve', 'Ebersole') ...". 
        /// </summary>
        /// <returns> 
        /// True if this SQL dialect is known to support "row value
        /// constructor" syntax; false otherwise.
        /// </returns>
        public virtual bool SupportsRowValueConstructorSyntax
        {
            // return false here, as most databases do not properly support this construct...
            get { return false; }
        }

        /// <summary> 
        /// If the dialect supports {@link #supportsRowValueConstructorSyntax() row values},
        /// does it offer such support in IN lists as well?
        /// <p/>
        /// For example, "... where (FIRST_NAME, LAST_NAME) IN ( (?, ?), (?, ?) ) ..." 
        /// </summary>
        /// <returns> 
        /// True if this SQL dialect is known to support "row value
        /// constructor" syntax in the IN list; false otherwise.
        /// </returns>
        public virtual bool SupportsRowValueConstructorSyntaxInInList
        {
            get { return false; }
        }

        /// <summary> 
        /// Should LOBs (both BLOB and CLOB) be bound using stream operations (i.e.
        /// {@link java.sql.PreparedStatement#setBinaryStream}). 
        /// </summary>
        /// <returns> True if BLOBs and CLOBs should be bound using stream operations. </returns>
        public virtual bool UseInputStreamToInsertBlob
        {
            get { return true; }
        }

        /// <summary> 
        /// Does this dialect support parameters within the select clause of
        /// INSERT ... SELECT ... statements? 
        /// </summary>
        /// <returns> True if this is supported; false otherwise. </returns>
        public virtual bool SupportsParametersInInsertSelect
        {
            get { return true; }
        }

        /// <summary> 
        /// Does this dialect require that references to result variables
        /// (i.e, select expression aliases) in an ORDER BY clause be
        /// replaced by column positions (1-origin) as defined by the select clause?
        /// </summary>
        /// <returns> 
        /// true if result variable references in the ORDER BY clause should 
        /// be replaced by column positions; false otherwise. 
        /// </returns>
        public virtual bool ReplaceResultVariableInOrderByClauseWithPosition
        {
            get { return false; }
        }

        /// <summary> 
        /// Does this dialect support asking the result set its positioning
        /// information on forward only cursors.  Specifically, in the case of
        /// scrolling fetches, Hibernate needs to use
        /// {@link java.sql.ResultSet#isAfterLast} and
        /// {@link java.sql.ResultSet#isBeforeFirst}.  Certain drivers do not
        /// allow access to these methods for forward only cursors.
        /// <p/>
        /// NOTE : this is highly driver dependent! 
        /// </summary>
        /// <returns> 
        /// True if methods like {@link java.sql.ResultSet#isAfterLast} and
        /// {@link java.sql.ResultSet#isBeforeFirst} are supported for forward
        /// only cursors; false otherwise.
        /// </returns>
        public virtual bool SupportsResultSetPositionQueryMethodsOnForwardOnlyCursor
        {
            get { return true; }
        }

        /// <summary> 
        /// Does this dialect support definition of cascade delete constraints
        /// which can cause circular chains? 
        /// </summary>
        /// <returns> True if circular cascade delete constraints are supported; false otherwise. </returns>
        public virtual bool SupportsCircularCascadeDeleteConstraints
        {
            get { return true; }
        }

        /// <summary> 
        /// Are subselects supported as the left-hand-side (LHS) of
        /// IN-predicates.
        /// <para/>
        /// In other words, is syntax like "... {subquery} IN (1, 2, 3) ..." supported? 
        /// </summary>
        /// <returns> True if subselects can appear as the LHS of an in-predicate;false otherwise. </returns>
        public virtual bool SupportsSubselectAsInPredicateLHS
        {
            get { return true; }
        }

        /// <summary> 
        /// Expected LOB usage pattern is such that I can perform an insert
        /// via prepared statement with a parameter binding for a LOB value
        /// without crazy casting to JDBC driver implementation-specific classes...
        /// <p/>
        /// Part of the trickiness here is the fact that this is largely
        /// driver dependent.  For example, Oracle (which is notoriously bad with
        /// LOB support in their drivers historically) actually does a pretty good
        /// job with LOB support as of the 10.2.x versions of their drivers... 
        /// </summary>
        /// <returns> 
        /// True if normal LOB usage patterns can be used with this driver;
        /// false if driver-specific hookiness needs to be applied.
        /// </returns>
        public virtual bool SupportsExpectedLobUsagePattern
        {
            get { return true; }
        }

        /// <summary> Does the dialect support propagating changes to LOB
        /// values back to the database?  Talking about mutating the
        /// internal value of the locator as opposed to supplying a new
        /// locator instance...
        /// <p/>
        /// For BLOBs, the internal value might be changed by:
        /// {@link java.sql.Blob#setBinaryStream},
        /// {@link java.sql.Blob#setBytes(long, byte[])},
        /// {@link java.sql.Blob#setBytes(long, byte[], int, int)},
        /// or {@link java.sql.Blob#truncate(long)}.
        /// <p/>
        /// For CLOBs, the internal value might be changed by:
        /// {@link java.sql.Clob#setAsciiStream(long)},
        /// {@link java.sql.Clob#setCharacterStream(long)},
        /// {@link java.sql.Clob#setString(long, String)},
        /// {@link java.sql.Clob#setString(long, String, int, int)},
        /// or {@link java.sql.Clob#truncate(long)}.
        /// <p/>
        /// NOTE : I do not know the correct answer currently for
        /// databases which (1) are not part of the cruise control process
        /// or (2) do not {@link #supportsExpectedLobUsagePattern}. 
        /// </summary>
        /// <returns> True if the changes are propagated back to the database; false otherwise. </returns>
        public virtual bool SupportsLobValueChangePropogation
        {
            get { return true; }
        }

        /// <summary> 
        /// Is it supported to materialize a LOB locator outside the transaction in
        /// which it was created?
        /// <p/>
        /// Again, part of the trickiness here is the fact that this is largely
        /// driver dependent.
        /// <p/>
        /// NOTE: all database I have tested which {@link #supportsExpectedLobUsagePattern()}
        /// also support the ability to materialize a LOB outside the owning transaction... 
        /// </summary>
        /// <returns> True if unbounded materialization is supported; false otherwise. </returns>
        public virtual bool SupportsUnboundedLobLocatorMaterialization
        {
            get { return true; }
        }

        /// <summary> 
        /// Does this dialect support referencing the table being mutated in
        /// a subquery.  The "table being mutated" is the table referenced in
        /// an UPDATE or a DELETE query.  And so can that table then be
        /// referenced in a subquery of said UPDATE/DELETE query.
        /// <p/>
        /// For example, would the following two syntaxes be supported:<ul>
        /// <li>delete from TABLE_A where ID not in ( select ID from TABLE_A )</li>
        /// <li>update TABLE_A set NON_ID = 'something' where ID in ( select ID from TABLE_A)</li>
        /// </ul>
        ///  </summary>
        /// <returns> True if this dialect allows references the mutating table from a subquery. </returns>
        public virtual bool SupportsSubqueryOnMutatingTable
        {
            get { return true; }
        }

        /// <summary> Does the dialect support an exists statement in the select clause? </summary>
        /// <returns> True if exists checks are allowed in the select clause; false otherwise. </returns>
        public virtual bool SupportsExistsInSelect
        {
            get { return true; }
        }

        /// <summary> 
        /// For the underlying database, is READ_COMMITTED isolation implemented by
        /// forcing readers to wait for write locks to be released? 
        /// </summary>
        /// <returns> True if writers block readers to achieve READ_COMMITTED; false otherwise. </returns>
        public virtual bool DoesReadCommittedCauseWritersToBlockReaders
        {
            get { return false; }
        }

        /// <summary> 
        /// For the underlying database, is REPEATABLE_READ isolation implemented by
        /// forcing writers to wait for read locks to be released? 
        /// </summary>
        /// <returns> True if readers block writers to achieve REPEATABLE_READ; false otherwise. </returns>
        public virtual bool DoesRepeatableReadCauseReadersToBlockWriters
        {
            get { return false; }
        }

        /// <summary> 
        /// Does this dialect support using a JDBC bind parameter as an argument
        /// to a function or procedure call? 
        /// </summary>
        /// <returns> True if the database supports accepting bind params as args; false otherwise. </returns>
        public virtual bool SupportsBindAsCallableArgument
        {
            get { return true; }
        }

        /// <summary>
        /// Does this dialect support subselects?
        /// </summary>
        public virtual bool SupportsSubSelects
        {
            get { return true; }
        }

        #endregion

        /// <summary>
        /// Retrieve a set of default Hibernate properties for this database.
        /// </summary>
        public IDictionary<string, string> DefaultProperties
        {
            get { return _properties; }
        }



        public HashSet<string> Keywords
        {
            get { return _sqlKeywords; }
        }

        /// <summary> 
        /// Get the command used to select a GUID from the underlying database.
        /// (Optional operation.)
        ///  </summary>
        /// <returns> The appropriate command. </returns>
        public virtual string SelectGUIDString
        {
            get { throw new NotSupportedException("dialect does not support server side GUIDs generation."); }
        }

        /// <summary> Command used to create a table. </summary>
        public virtual string CreateTableString
        {
            get { return "create table"; }
        }

        /// <summary> 
        /// Slight variation on <see cref="CreateTableString"/>.
        /// The command used to create a multiset table. 
        /// </summary>
        /// <remarks>
        /// Here, we have the command used to create a table when there is no primary key and
        /// duplicate rows are expected.
        /// <p/>
        /// Most databases do not care about the distinction; originally added for
        /// Teradata support which does care.
        /// </remarks>
        public virtual string CreateMultisetTableString
        {
            get { return CreateTableString; }
        }

        /// <summary> Command used to create a temporary table. </summary>
        public virtual string CreateTemporaryTableString
        {
            get { return "create table"; }
        }

        /// <summary> 
        /// Get any fragments needing to be postfixed to the command for
        /// temporary table creation. 
        /// </summary>
        public virtual string CreateTemporaryTablePostfix
        {
            get { return string.Empty; }
        }

        /// <summary> 
        /// Should the value returned by <see cref="CurrentTimestampSelectString"/>
        /// be treated as callable.  Typically this indicates that JDBC escape
        /// syntax is being used...
        /// </summary>
        public virtual bool IsCurrentTimestampSelectStringCallable
        {
            get { throw new NotSupportedException("Database not known to define a current timestamp function"); }
        }

        /// <summary> 
        /// Retrieve the command used to retrieve the current timestammp from the database. 
        /// </summary>
        public virtual string CurrentTimestampSelectString
        {
            get { throw new NotSupportedException("Database not known to define a current timestamp function"); }
        }





        /// <summary>
        /// The keyword used to insert a row without specifying any column values
        /// </summary>
        public virtual string NoColumnsInsertString
        {
            get { return "values ( )"; }
        }

        /// <summary>
        /// The name of the SQL function that transforms a string to lowercase
        /// </summary>
        public virtual string LowercaseFunction
        {
            get { return "lower"; }
        }

        public virtual int MaxAliasLength
        {
            get { return 10; }
        }

        /// <summary>
        /// The syntax used to add a column to a table. Note this is deprecated
        /// </summary>
        public virtual string AddColumnString
        {
            get { throw new NotSupportedException("No add column syntax supported by Dialect"); }
        }

        public virtual string DropForeignKeyString
        {
            get { return " drop constraint "; }
        }

        public virtual string TableTypeString
        {
            get { return String.Empty; } // for differentiation of mysql storage engines
        }

        /// <summary>
        /// The keyword used to specify a nullable column
        /// </summary>
        public virtual string NullColumnString
        {
            get { return String.Empty; }
        }

        /// <summary>
        /// The keyword used to create a primary key constraint
        /// </summary>
        public virtual string PrimaryKeyString
        {
            get { return "primary key"; }
        }

        /// <summary>
        /// Supports splitting batches using GO T-SQL command
        /// </summary>
        /// <remarks>
        /// Batches http://msdn.microsoft.com/en-us/library/ms175502.aspx
        /// </remarks>
        public virtual bool SupportsSqlBatches
        {
            get { return false; }
        }

        public virtual bool IsKnownToken(string currentToken, string nextToken)
        {
            return false;
        }

        protected void RegisterKeyword(string word)
        {
            Keywords.Add(word);
        }



        /// <summary> 
        /// Registers a NHibernate <see cref="IType"/> name for the given <see cref="DatabaseType"/> type code. 
        /// </summary>
        /// <param name="code">The <see cref="DatabaseType"/> typecode </param>
        /// <param name="name">The NHibernate <see cref="DatabaseType"/> name </param>
        private void RegisterHibernateType(DbType code, string name)
        {
            _hibernateTypeNames.Put(code, name);
        }


    }

    [Serializable]
    public class SqlType
    {
        private readonly DbType dbType;
        private readonly int length;
        private readonly byte precision;
        private readonly byte scale;
        private readonly bool lengthDefined;
        private readonly bool precisionDefined;

        public SqlType(DbType dbType)
        {
            this.dbType = dbType;
        }

        public SqlType(DbType dbType, int length)
        {
            this.dbType = dbType;
            this.length = length;
            lengthDefined = true;
        }

        public SqlType(DbType dbType, byte precision, byte scale)
        {
            this.dbType = dbType;
            this.precision = precision;
            this.scale = scale;
            precisionDefined = true;
        }

        public DbType DbType
        {
            get { return dbType; }
        }

        public int Length
        {
            get { return length; }
        }

        public byte Precision
        {
            get { return precision; }
        }

        public byte Scale
        {
            get { return scale; }
        }

        public bool LengthDefined
        {
            get { return lengthDefined; }
        }

        public bool PrecisionDefined
        {
            get { return precisionDefined; }
        }

        #region System.Object Members

        public override int GetHashCode()
        {
            unchecked
            {
                int hashCode;

                if (LengthDefined)
                {
                    hashCode = (DbType.GetHashCode() / 2) + (Length.GetHashCode() / 2);
                }
                else if (PrecisionDefined)
                {
                    hashCode = (DbType.GetHashCode() / 3) + (Precision.GetHashCode() / 3) + (Scale.GetHashCode() / 3);
                }
                else
                {
                    hashCode = DbType.GetHashCode();
                }

                return hashCode;
            }
        }

        public override bool Equals(object obj)
        {
            return obj == this || Equals(obj as SqlType);
        }

        public bool Equals(SqlType rhsSqlType)
        {
            if (rhsSqlType == null)
            {
                return false;
            }

            if (LengthDefined)
            {
                return (DbType.Equals(rhsSqlType.DbType)) && (Length == rhsSqlType.Length);
            }
            if (PrecisionDefined)
            {
                return (DbType.Equals(rhsSqlType.DbType)) && (Precision == rhsSqlType.Precision) && (Scale == rhsSqlType.Scale);
            }
            return (DbType.Equals(rhsSqlType.DbType));
        }

        public override string ToString()
        {
            if (!LengthDefined && !PrecisionDefined)
            {
                // Shortcut
                return DbType.ToString();
            }

            var result = new StringBuilder(DbType.ToString());

            if (LengthDefined)
            {
                result.Append("(Length=").Append(Length).Append(')');
            }

            if (PrecisionDefined)
            {
                result.Append("(Precision=").Append(Precision).Append(", ").Append("Scale=").Append(Scale).Append(')');
            }

            return result.ToString();
        }

        #endregion
    }

    /// <summary>
    /// This class maps a DbType to names.
    /// </summary>
    /// <remarks>
    /// Associations may be marked with a capacity. Calling the <c>Get()</c>
    /// method with a type and actual size n will return the associated
    /// name with smallest capacity >= n, if available and an unmarked
    /// default type otherwise.
    /// Eg, setting
    /// <code>
    ///		Names.Put(DbType,			"TEXT" );
    ///		Names.Put(DbType,	255,	"VARCHAR($l)" );
    ///		Names.Put(DbType,	65534,	"LONGVARCHAR($l)" );
    /// </code>
    /// will give you back the following:
    /// <code>
    ///		Names.Get(DbType)			// --> "TEXT" (default)
    ///		Names.Get(DbType,100)		// --> "VARCHAR(100)" (100 is in [0:255])
    ///		Names.Get(DbType,1000)	// --> "LONGVARCHAR(1000)" (100 is in [256:65534])
    ///		Names.Get(DbType,100000)	// --> "TEXT" (default)
    /// </code>
    /// On the other hand, simply putting
    /// <code>
    ///		Names.Put(DbType, "VARCHAR($l)" );
    /// </code>
    /// would result in
    /// <code>
    ///		Names.Get(DbType)			// --> "VARCHAR($l)" (will cause trouble)
    ///		Names.Get(DbType,100)		// --> "VARCHAR(100)" 
    ///		Names.Get(DbType,1000)	// --> "VARCHAR(1000)"
    ///		Names.Get(DbType,10000)	// --> "VARCHAR(10000)"
    /// </code>
    /// </remarks>
    public class TypeNames
    {
        public const string LengthPlaceHolder = "$l";
        public const string PrecisionPlaceHolder = "$p";
        public const string ScalePlaceHolder = "$s";

        private readonly Dictionary<DbType, SortedList<int, string>> weighted =
            new Dictionary<DbType, SortedList<int, string>>();

        private readonly Dictionary<DbType, string> defaults = new Dictionary<DbType, string>();

        /// <summary>
        /// Get default type name for specified type
        /// </summary>
        /// <param name="typecode">the type key</param>
        /// <returns>the default type name associated with the specified key</returns>
        public string Get(DbType typecode)
        {
            string result;
            if (!defaults.TryGetValue(typecode, out result))
            {
                throw new ArgumentException("Dialect does not support DbType." + typecode, "typecode");
            }
            return result;
        }

        /// <summary>
        /// Get the type name specified type and size
        /// </summary>
        /// <param name="typecode">the type key</param>
        /// <param name="size">the SQL length </param>
        /// <param name="scale">the SQL scale </param>
        /// <param name="precision">the SQL precision </param>
        /// <returns>
        /// The associated name with smallest capacity >= size if available and the
        /// default type name otherwise
        /// </returns>
        public string Get(DbType typecode, int size, int precision, int scale)
        {
            SortedList<int, string> map;
            weighted.TryGetValue(typecode, out map);
            if (map != null && map.Count > 0)
            {
                foreach (KeyValuePair<int, string> entry in map)
                {
                    if (size <= entry.Key)
                    {
                        return Replace(entry.Value, size, precision, scale);
                    }
                }
            }
            //Could not find a specific type for the size, using the default
            return Replace(Get(typecode), size, precision, scale);
        }

        /// <summary>
        /// For types with a simple length, this method returns the definition
        /// for the longest registered type.
        /// </summary>
        /// <param name="typecode"></param>
        /// <returns></returns>
        public string GetLongest(DbType typecode)
        {
            SortedList<int, string> map;
            weighted.TryGetValue(typecode, out map);

            if (map != null && map.Count > 0)
                return Replace(map.Values[map.Count - 1], map.Keys[map.Count - 1], 0, 0);

            return Get(typecode);
        }

        private static string Replace(string type, int size, int precision, int scale)
        {
            type = StringHelper.ReplaceOnce(type, LengthPlaceHolder, size.ToString());
            type = StringHelper.ReplaceOnce(type, ScalePlaceHolder, scale.ToString());
            return StringHelper.ReplaceOnce(type, PrecisionPlaceHolder, precision.ToString());
        }

        /// <summary>
        /// Set a type name for specified type key and capacity
        /// </summary>
        /// <param name="typecode">the type key</param>
        /// <param name="capacity">the (maximum) type size/length</param>
        /// <param name="value">The associated name</param>
        public void Put(DbType typecode, int capacity, string value)
        {
            SortedList<int, string> map;
            if (!weighted.TryGetValue(typecode, out map))
            {
                // add new ordered map
                weighted[typecode] = map = new SortedList<int, string>();
            }
            map[capacity] = value;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="typecode"></param>
        /// <param name="value"></param>
        public void Put(DbType typecode, string value)
        {
            defaults[typecode] = value;
        }
    }
    public static class StringHelper
    {
        /// <summary>
        /// This allows for both CRLF and lone LF line separators.
        /// </summary>
        internal static readonly string[] LineSeparators = { "\r\n", "\n" };

        public const string WhiteSpace = " \n\r\f\t";

        /// <summary></summary>
        public const char Dot = '.';

        /// <summary></summary>
        public const char Underscore = '_';

        /// <summary></summary>
        public const string CommaSpace = ", ";

        /// <summary></summary>
        public const string Comma = ",";

        /// <summary></summary>
        public const string OpenParen = "(";

        /// <summary></summary>
        public const string ClosedParen = ")";

        /// <summary></summary>
        public const char SingleQuote = '\'';

        /// <summary></summary>
        public const string SqlParameter = "?";

        public const int AliasTruncateLength = 10;

        public static string Join(string separator, IEnumerable objects)
        {
            StringBuilder buf = new StringBuilder();
            bool first = true;

            foreach (object obj in objects)
            {
                if (!first)
                {
                    buf.Append(separator);
                }

                first = false;
                buf.Append(obj);
            }

            return buf.ToString();
        }

        internal static string Join<T>(string separator, IEnumerable<T> objects)
        {
            return string.Join(separator, objects);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="str"></param>
        /// <param name="times"></param>
        /// <returns></returns>
        public static string Repeat(string str, int times)
        {
            StringBuilder buf = new StringBuilder(str.Length * times);
            for (int i = 0; i < times; i++)
            {
                buf.Append(str);
            }
            return buf.ToString();
        }

        public static string Replace(string template, string placeholder, string replacement)
        {
            return Replace(template, placeholder, replacement, false);
        }

        public static string Replace(string template, string placeholder, string replacement, bool wholeWords)
        {
            Predicate<string> isWholeWord = c => WhiteSpace.Contains(c) || ClosedParen.Equals(c) || Comma.Equals(c);
            return ReplaceByPredicate(template, placeholder, replacement, wholeWords, isWholeWord);
        }

        private static string ReplaceByPredicate(string template, string placeholder, string replacement, bool useWholeWord, Predicate<string> isWholeWord)
        {
            // sometimes a null value will get passed in here -> SqlWhereStrings are a good example
            if (string.IsNullOrWhiteSpace(template))
            {
                return null;
            }

            int loc = template.IndexOf(placeholder);
            if (loc < 0)
            {
                return template;
            }
            else
            {
                // NH different implementation (NH-1253)
                string replaceWith = replacement;
                if (loc + placeholder.Length < template.Length)
                {
                    string afterPlaceholder = template[loc + placeholder.Length].ToString();
                    //After a token in HQL there can be whitespace, closedparen or comma.. 
                    if (useWholeWord && !isWholeWord(afterPlaceholder))
                    {
                        //If this is not a full token we don't want to touch it
                        replaceWith = placeholder;
                    }
                }

                return new StringBuilder(template.Substring(0, loc))
                    .Append(replaceWith)
                    .Append(ReplaceByPredicate(template.Substring(loc + placeholder.Length), placeholder, replacement, useWholeWord, isWholeWord))
                    .ToString();
            }
        }

        public static string ReplaceWholeWord(this string template, string placeholder, string replacement)
        {
            Predicate<string> isWholeWord = s => !Char.IsLetterOrDigit(s[0]);
            return ReplaceByPredicate(template, placeholder, replacement, true, isWholeWord);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="template"></param>
        /// <param name="placeholder"></param>
        /// <param name="replacement"></param>
        /// <returns></returns>
        public static string ReplaceOnce(string template, string placeholder, string replacement)
        {
            int loc = template.IndexOf(placeholder);
            if (loc < 0)
            {
                return template;
            }
            else
            {
                return new StringBuilder(template.Substring(0, loc))
                    .Append(replacement)
                    .Append(template.Substring(loc + placeholder.Length))
                    .ToString();
            }
        }

        /// <summary>
        /// Just a fa鏰de for calling string.Split()
        /// We don't use our StringTokenizer because string.Split() is
        /// more efficient (but it only works when we don't want to retrieve the delimiters)
        /// </summary>
        /// <param name="separators">separators for the tokens of the list</param>
        /// <param name="list">the string that will be broken into tokens</param>
        /// <returns></returns>
        public static string[] Split(string separators, string list)
        {
            return list.Split(separators.ToCharArray());
        }




        /// <summary>
        /// 
        /// </summary>
        /// <param name="qualifiedName"></param>
        /// <param name="seperator"></param>
        /// <returns></returns>
        public static string Unqualify(string qualifiedName, string seperator)
        {
            return qualifiedName.Substring(qualifiedName.LastIndexOf(seperator) + 1);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="qualifiedName"></param>
        /// <returns></returns>
        public static string Qualifier(string qualifiedName)
        {
            int loc = qualifiedName.LastIndexOf(".");
            if (loc < 0)
            {
                return String.Empty;
            }
            else
            {
                return qualifiedName.Substring(0, loc);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="columns"></param>
        /// <param name="suffix"></param>
        /// <returns></returns>
        public static string[] Suffix(string[] columns, string suffix)
        {
            if (suffix == null)
            {
                return columns;
            }
            string[] qualified = new string[columns.Length];
            for (int i = 0; i < columns.Length; i++)
            {
                qualified[i] = Suffix(columns[i], suffix);
            }
            return qualified;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="name"></param>
        /// <param name="suffix"></param>
        /// <returns></returns>
        public static string Suffix(string name, string suffix)
        {
            return (suffix == null) ?
                   name :
                   name + suffix;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="columns"></param>
        /// <param name="prefix"></param>
        /// <returns></returns>
        public static string[] Prefix(string[] columns, string prefix)
        {
            if (prefix == null)
            {
                return columns;
            }
            string[] qualified = new string[columns.Length];
            for (int i = 0; i < columns.Length; i++)
            {
                qualified[i] = prefix + columns[i];
            }
            return qualified;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="qualifiedName"></param>
        /// <returns></returns>
        public static string Root(string qualifiedName)
        {
            int loc = qualifiedName.IndexOf(".");
            return (loc < 0)
                    ? qualifiedName
                    : qualifiedName.Substring(0, loc);
        }

        /// <summary>
        /// Converts a <see cref="String"/> in the format of "true", "t", "false", or "f" to
        /// a <see cref="Boolean"/>.
        /// </summary>
        /// <param name="value">The string to convert.</param>
        /// <returns>
        /// The <c>value</c> converted to a <see cref="Boolean"/> .
        /// </returns>
        public static bool BooleanValue(string value)
        {
            string trimmed = value.Trim().ToLowerInvariant();
            return trimmed.Equals("true") || trimmed.Equals("t");
        }

        private static string NullSafeToString(object obj)
        {
            return obj == null ? "(null)" : obj.ToString();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="array"></param>
        /// <returns></returns>
        public static string ToString(object[] array)
        {
            int len = array.Length;

            // if there is no value in the array then return no string...
            if (len == 0)
            {
                return String.Empty;
            }

            StringBuilder buf = new StringBuilder(len * 12);
            for (int i = 0; i < len - 1; i++)
            {
                buf.Append(NullSafeToString(array[i])).Append(CommaSpace);
            }
            return buf.Append(NullSafeToString(array[len - 1])).ToString();
        }

        public static string LinesToString(this string[] text)
        {
            if (text == null)
            {
                return null;
            }
            if (text.Length == 1)
            {
                return text[0];
            }
            var sb = new StringBuilder(200);
            Array.ForEach(text, t => sb.AppendLine(t));
            return sb.ToString();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="str"></param>
        /// <param name="placeholders"></param>
        /// <param name="replacements"></param>
        /// <returns></returns>
        public static string[] Multiply(string str, IEnumerator placeholders, IEnumerator replacements)
        {
            string[] result = new string[] { str };
            while (placeholders.MoveNext())
            {
                replacements.MoveNext();
                result = Multiply(result, placeholders.Current as string, replacements.Current as string[]);
            }
            return result;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="strings"></param>
        /// <param name="placeholder"></param>
        /// <param name="replacements"></param>
        /// <returns></returns>
        public static string[] Multiply(string[] strings, string placeholder, string[] replacements)
        {
            string[] results = new string[replacements.Length * strings.Length];
            int n = 0;
            for (int i = 0; i < replacements.Length; i++)
            {
                for (int j = 0; j < strings.Length; j++)
                {
                    results[n++] = ReplaceOnce(strings[j], placeholder, replacements[i]);
                }
            }
            return results;
        }

        /// <summary>
        /// Counts the unquoted instances of the character.
        /// </summary>
        /// <param name="str"></param>
        /// <param name="character"></param>
        /// <returns></returns>
        public static int CountUnquoted(string str, char character)
        {
            if (SingleQuote == character)
            {
                throw new ArgumentOutOfRangeException("character", "Unquoted count of quotes is invalid");
            }

            // Impl note: takes advantage of the fact that an escaped single quote
            // embedded within a quote-block can really be handled as two separate
            // quote-blocks for the purposes of this method...
            int count = 0;
            char[] chars = str.ToCharArray();
            int stringLength = string.IsNullOrEmpty(str) ? 0 : chars.Length;
            bool inQuote = false;
            for (int indx = 0; indx < stringLength; indx++)
            {
                if (inQuote)
                {
                    if (SingleQuote == chars[indx])
                    {
                        inQuote = false;
                    }
                }
                else if (SingleQuote == chars[indx])
                {
                    inQuote = true;
                }
                else if (chars[indx] == character)
                {
                    count++;
                }
            }
            return count;
        }

        public static bool IsEmpty(string str)
        {
            return string.IsNullOrEmpty(str);
        }

        public static bool IsNotEmpty(string str)
        {
            return !IsEmpty(str);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="prefix"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        public static string Qualify(string prefix, string name)
        {
            char first = name[0];

            // Should we check for prefix == string.Empty rather than a length check?
            if (!string.IsNullOrEmpty(prefix) && first != SingleQuote && !char.IsDigit(first))
            {
                return prefix + Dot + name;
            }
            else
            {
                return name;
            }
        }

        public static string[] Qualify(string prefix, string[] names)
        {
            // Should we check for prefix == string.Empty rather than a length check?
            if (!string.IsNullOrEmpty(prefix))
            {
                int len = names.Length;
                string[] qualified = new string[len];
                for (int i = 0; i < len; i++)
                {
                    qualified[i] = names[i] == null ? null : Qualify(prefix, names[i]);
                }
                return qualified;
            }
            else
            {
                return names;
            }
        }

        public static int FirstIndexOfChar(string sqlString, string str, int startIndex)
        {
            return sqlString.IndexOfAny(str.ToCharArray(), startIndex);
        }

        public static string Truncate(string str, int length)
        {
            if (str.Length <= length)
            {
                return str;
            }
            else
            {
                return str.Substring(0, length);
            }
        }

        public static int LastIndexOfLetter(string str)
        {
            for (int i = 0; i < str.Length; i++)
            {
                if (!char.IsLetter(str, i) /*&& !('_'==character)*/)
                {
                    return i - 1;
                }
            }
            return str.Length - 1;
        }




        public static string MoveAndToBeginning(string filter)
        {
            if (filter.Trim().Length > 0)
            {
                filter += " and ";
                if (filter.StartsWith(" and "))
                {
                    filter = filter.Substring(4);
                }
            }
            return filter;
        }

        public static string Unroot(string qualifiedName)
        {
            int loc = qualifiedName.IndexOf(".");
            return (loc < 0) ? qualifiedName : qualifiedName.Substring(loc + 1);
        }

        public static bool EqualsCaseInsensitive(string a, string b)
        {
            return StringComparer.InvariantCultureIgnoreCase.Compare(a, b) == 0;
        }

        public static int IndexOfCaseInsensitive(string source, string value)
        {
            return source.IndexOf(value, StringComparison.InvariantCultureIgnoreCase);
        }

        public static int IndexOfCaseInsensitive(string source, string value, int startIndex)
        {
            return source.IndexOf(value, startIndex, StringComparison.InvariantCultureIgnoreCase);
        }

        public static int IndexOfCaseInsensitive(string source, string value, int startIndex, int count)
        {
            return source.IndexOf(value, startIndex, count, StringComparison.InvariantCultureIgnoreCase);
        }

        public static int LastIndexOfCaseInsensitive(string source, string value)
        {
            return source.LastIndexOf(value, StringComparison.InvariantCultureIgnoreCase);
        }

        public static bool StartsWithCaseInsensitive(string source, string prefix)
        {
            return source.StartsWith(prefix, StringComparison.InvariantCultureIgnoreCase);
        }

        /// <summary>
        /// Returns the interned string equal to <paramref name="str"/> if there is one, or <paramref name="str"/>
        /// otherwise.
        /// </summary>
        /// <param name="str">A <see cref="string" /></param>
        /// <returns>A <see cref="string" /></returns>
        public static string InternedIfPossible(string str)
        {
            if (str == null)
            {
                return null;
            }

            string interned = string.IsInterned(str);
            if (interned != null)
            {
                return interned;
            }

            return str;
        }

        public static string CollectionToString(IEnumerable keys)
        {
            var sb = new StringBuilder();
            foreach (object o in keys)
            {
                sb.Append(o);
                sb.Append(", ");
            }
            if (sb.Length != 0)//remove last ", "
                sb.Remove(sb.Length - 2, 2);
            return sb.ToString();
        }

        public static string ToUpperCase(string str)
        {
            return str == null ? null : str.ToUpperInvariant();
        }

        public static string ToLowerCase(string str)
        {
            return str == null ? null : str.ToLowerInvariant();
        }

        public static bool IsBackticksEnclosed(string identifier)
        {
            return !string.IsNullOrEmpty(identifier) && identifier.StartsWith("`") && identifier.EndsWith("`");
        }

        public static string PurgeBackticksEnclosing(string identifier)
        {
            if (IsBackticksEnclosed(identifier))
            {
                return identifier.Substring(1, identifier.Length - 2);
            }
            return identifier;
        }

        public static string[] ParseFilterParameterName(string filterParameterName)
        {
            int dot = filterParameterName.IndexOf(".");
            if (dot <= 0)
            {
                throw new ArgumentException("Invalid filter-parameter name format; the name should be a property path.", "filterParameterName");
            }
            string filterName = filterParameterName.Substring(0, dot);
            string parameterName = filterParameterName.Substring(dot + 1);
            return new[] { filterName, parameterName };
        }


        /// <summary>
        /// Return the index of the next line separator, starting at startIndex. If will match
        /// the first CRLF or LF line separator. If there is no match, -1 will be returned. When
        /// returning, newLineLength will be set to the number of characters in the matched line
        /// separator (1 if LF was found, 2 if CRLF was found).
        /// </summary>
        public static int IndexOfAnyNewLine(this string str, int startIndex, out int newLineLength)
        {
            newLineLength = 0;
            var matchStartIdx = str.IndexOfAny(new[] { '\r', '\n' }, startIndex);

            if (matchStartIdx == -1)
                return -1;

            if (string.Compare(str, matchStartIdx, "\r\n", 0, 2, StringComparison.OrdinalIgnoreCase) == 0)
                newLineLength = 2;
            else
                newLineLength = 1;

            return matchStartIdx;
        }


        /// <summary>
        /// Check if the given index points to a line separator in the string. Both CRLF and LF
        /// line separators are handled. When returning, newLineLength will be set to the number
        /// of characters matched in the line separator. It will be 2 if a CRLF matched, 1 if LF
        /// matched, and 0 if the index doesn't indicate (the start of) a line separator.
        /// </summary>
        public static bool IsAnyNewLine(this string str, int index, out int newLineLength)
        {
            if (string.Compare(str, index, "\r\n", 0, 2, StringComparison.OrdinalIgnoreCase) == 0)
            {
                newLineLength = 2;
                return true;
            }

            if (index < str.Length && str[index] == '\n')
            {
                newLineLength = 1;
                return true;
            }

            newLineLength = 0;
            return false;
        }
    }
}
