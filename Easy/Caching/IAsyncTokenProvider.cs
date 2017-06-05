using System;

namespace Easy.Caching {
    public interface IAsyncTokenProvider:ISingletonDependency {
        IVolatileToken GetToken(Action<Action<IVolatileToken>> task);
    }
}