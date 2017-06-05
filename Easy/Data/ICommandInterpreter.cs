namespace Easy.Data
{
    using Easy.Data.Schema;
    using Data.Database.Enum;
    /// <summary>
    /// This interface can be implemented to provide a data migration behavior
    /// </summary>
    public interface ICommandInterpreter<in T> : ICommandInterpreter
    where T : ISchemaBuilderCommand {
        string[] CreateStatements(T command);
    }

    public interface ICommandInterpreter:IEventHandler  {
        DatabaseType DataProvider { get; }
    }
}
