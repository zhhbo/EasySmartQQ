namespace Easy.Data.Common.Interface
{
    public interface IUniqueIdentity<T>
    {
        T ObjectId { get; }
    }
}
