namespace Easy.Caching {
    public interface IVolatileToken {
        bool IsCurrent { get; }
    }
}