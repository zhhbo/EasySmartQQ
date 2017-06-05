using Easy.Data.Database.Enum;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlTypes;
namespace Easy.Data.Database.Core
{
    public interface IDatabase
    {
        bool CanUse { get; }
        DatabaseType DatabaseType { get; }
        void Close();
        object Query(string strSql);
        bool ExecuteSql(string strSql);
        void BeginTransaction();
        void CommitTransaction();
        void RollbackTransaction();
        long GetNextId(string idFieldName, string tableName);
        string GetFieldValue(object val, Type fieldType);

        string AddColumnString { get; }
        bool AreStringComparisonsCaseInsensitive { get; }
        string CascadeConstraintsString { get; }
        char CloseQuote { get; }
        string CreateMultisetTableString { get; }
        string CreateTableString { get; }
        string CreateTemporaryTablePostfix { get; }
        string CreateTemporaryTableString { get; }
        string CurrentTimestampSelectString { get; }

        IDictionary<string, string> DefaultProperties { get; }
        string DisableForeignKeyConstraintsString { get; }
        bool DoesReadCommittedCauseWritersToBlockReaders { get; }
        bool DoesRepeatableReadCauseReadersToBlockWriters { get; }
        bool DropConstraints { get; }
        string DropForeignKeyString { get; }
        string EnableForeignKeyConstraintsString { get; }
        string ForUpdateNowaitString { get; }
        bool ForUpdateOfColumns { get; }
        string ForUpdateString { get; }
        bool GenerateTablePrimaryKeyConstraintForIdentityColumn { get; }
        bool HasDataTypeInIdentityColumn { get; }
        bool HasSelfReferentialForeignKeyBug { get; }
        string IdentityColumnString { get; }
        string IdentityInsertString { get; }
        string IdentitySelectString { get; }
        bool IsCurrentTimestampSelectStringCallable { get; }
        HashSet<string> Keywords { get; }
        string LowercaseFunction { get; }
        int MaxAliasLength { get; }
        string NoColumnsInsertString { get; }
        string NullColumnString { get; }
        bool OffsetStartsAtOne { get; }
        char OpenQuote { get; }
        string PrimaryKeyString { get; }
        bool QualifyIndexName { get; }
        string QuerySequencesString { get; }
        bool ReplaceResultVariableInOrderByClauseWithPosition { get; }
        string SelectGUIDString { get; }
        bool SupportsBindAsCallableArgument { get; }
        bool SupportsCascadeDelete { get; }
        bool SupportsCircularCascadeDeleteConstraints { get; }
        bool SupportsColumnCheck { get; }
        bool SupportsCommentOn { get; }
        bool SupportsCurrentTimestampSelection { get; }
        bool SupportsEmptyInList { get; }
        bool SupportsExistsInSelect { get; }
        bool SupportsExpectedLobUsagePattern { get; }
        bool SupportsForeignKeyConstraintInAlterTable { get; }
        bool SupportsIdentityColumns { get; }
        bool SupportsIfExistsAfterTableName { get; }
        bool SupportsIfExistsBeforeTableName { get; }
        bool SupportsInsertSelectIdentity { get; }
        bool SupportsLimit { get; }
        bool SupportsLimitOffset { get; }
        bool SupportsLobValueChangePropogation { get; }
        bool SupportsNotNullUnique { get; }
        bool SupportsOuterJoinForUpdate { get; }
        bool SupportsParametersInInsertSelect { get; }
        bool SupportsPooledSequences { get; }
        bool SupportsResultSetPositionQueryMethodsOnForwardOnlyCursor { get; }
        bool SupportsRowValueConstructorSyntax { get; }
        bool SupportsRowValueConstructorSyntaxInInList { get; }
        bool SupportsSequences { get; }
        bool SupportsSqlBatches { get; }
        bool SupportsSubqueryOnMutatingTable { get; }
        bool SupportsSubselectAsInPredicateLHS { get; }
        bool SupportsSubSelects { get; }
        bool SupportsTableCheck { get; }
        bool SupportsTemporaryTables { get; }
        bool SupportsUnboundedLobLocatorMaterialization { get; }
        bool SupportsUnionAll { get; }
        bool SupportsUnique { get; }
        bool SupportsUniqueConstraintInCreateAlterTable { get; }
        bool SupportsVariableLimit { get; }
        string TableTypeString { get; }
        long TimestampResolutionInTicks { get; }
        bool UseInputStreamToInsertBlob { get; }
        bool UseMaxForLimit { get; }
        string GenerateTemporaryTableName(string baseTableName);
        string GetAddForeignKeyConstraintString(string constraintName, string[] foreignKey, string referencedTable, string[] primaryKey, bool referencesPrimaryKey);
        string GetAddPrimaryKeyConstraintString(string constraintName);
        string GetColumnComment(string comment);
        string GetCreateSequenceString(string sequenceName);
        string[] GetCreateSequenceStrings(string sequenceName, int initialValue, int incrementSize);
        string GetDropForeignKeyConstraintString(string constraintName);
        string GetDropIndexConstraintString(string constraintName);
        string GetDropPrimaryKeyConstraintString(string constraintName);
        string GetDropSequenceString(string sequenceName);
        string[] GetDropSequenceStrings(string sequenceName);
        string GetDropTableString(string tableName);

        string GetForUpdateNowaitString(string aliases);
        string GetForUpdateString(string aliases);
        string GetIdentityColumnString(DbType type);
        string GetIdentitySelectString(string identityColumn, string tableName, DbType type);
        int GetLimitValue(int offset, int limit);
        string GetLongestTypeName(DbType dbType);

        int GetOffsetValue(int offset);
        string GetSelectClauseNullString(SqlType sqlType);
        string GetSelectSequenceNextValString(string sequenceName);
        string GetSequenceNextValString(string sequenceName);
        string GetTableComment(string comment);
        string GetTypeName(SqlType sqlType);
        string GetTypeName(SqlType sqlType, int length, int precision, int scale);
        bool IsKnownToken(string currentToken, string nextToken);
        bool IsQuoted(string name);
        bool? PerformTemporaryTableDDLInIsolation();
        string Qualify(string catalog, string schema, string table);

        string QuoteForAliasName(string aliasName);
        string QuoteForColumnName(string columnName);
        string QuoteForSchemaName(string schemaName);
        string QuoteForTableName(string tableName);

        string ToBooleanValueString(bool value);
        string UnQuote(string quoted);
        string[] UnQuote(string[] quoted);
    }
}
