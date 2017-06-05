namespace Easy
{
    public interface IDependency
    {
    }
    public interface ISingletonDependency : IDependency
    { }
    public interface IEventHandler : IDependency
    { }
}
