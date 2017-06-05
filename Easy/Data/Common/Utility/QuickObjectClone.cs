using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace Easy.Data.Common.Utility
{
    /// <summary>
    /// 快速对象复制
    /// </summary>
    public class QuickObjectClone<T>
    {
        private static readonly Dictionary<Type, object> _cache = new Dictionary<Type, object>();

        public static T Clone(T orginalObj)
        {
            Type objType = typeof (T);
            if (!_cache.ContainsKey(objType))
            {
                ParameterExpression parameterExpression = Expression.Parameter(objType, "p");
                List<MemberBinding> memberBindingList = (from item in objType.GetProperties() 
                                                         let property = Expression.Property(parameterExpression, objType.GetProperty(item.Name)) 
                                                         select Expression.Bind(item, property)).Cast<MemberBinding>().ToList();
                MemberInitExpression memberInitExpression = Expression.MemberInit(Expression.New(objType), memberBindingList.ToArray());
                Expression<Func<T, T>> lambda = Expression.Lambda<Func<T, T>>(memberInitExpression, new ParameterExpression[] { parameterExpression });
                Func<T, T> func = lambda.Compile();

                _cache[objType] = func;
            }
            return ((Func<T, T>)_cache[objType])(orginalObj);
        }
    }
}
