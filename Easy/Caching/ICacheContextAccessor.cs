namespace Easy.Caching {
    public interface ICacheContextAccessor:ISingletonDependency {
        IAcquireContext Current { get; set; }
    }
}