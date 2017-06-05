using System.Collections.Generic;

namespace Easy.Data.Common.Interface
{
    public interface IFactory
    {
        List<T> BuildUp<T>(int capacity) where T : new();

        T BuildUp<T>() where T : new();

        List<T> BuildUp<T>(int capacity,params object[] parameters);

        T BuildUp<T>(params object[] parameters);
    }
}
