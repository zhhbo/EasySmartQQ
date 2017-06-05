using System;
using System.Collections.Generic;
using Easy.Data.Common.Interface;

namespace Easy.Data.Common.Partten_Class
{
    public class ObjectFactory : IFactory
    {
        public List<T> BuildUp<T>(int capacity) where T : new()
        {
            List<T> items = new List<T>();
            for (int i = 0; i < capacity; i++)
            {
                items.Add(new T());
            }
            return items;
        }

        public T BuildUp<T>() where T : new()
        {
            return new T();
        }

        public List<T> BuildUp<T>(int capacity, params object[] parameters)
        {
            List<T> items = new List<T>();
            Type objectType = typeof (T);
            for (int i = 0; i < capacity; i++)
            {
                items.Add((T)Activator.CreateInstance(objectType, parameters));
            }
            return items;
        }

        public T BuildUp<T>(params object[] parameters)
        {
            return (T)Activator.CreateInstance(typeof(T), parameters);
        }

        public List<object> BuildUp(Type objType,int capacity, params object[] parameters)
        {
            List<object> items = new List<object>();
            for (int i = 0; i < capacity; i++)
            {
                items.Add(Activator.CreateInstance(objType, parameters));
            }
            return items;
        }

        public object BuildUp(Type objType, params object[] parameters)
        {
            return Activator.CreateInstance(objType, parameters);
        }
    }
}
