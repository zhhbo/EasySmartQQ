using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Easy.Data.Common.Utility
{
    public static class AttributeHelper
    {
        public static T GetAttribute<T>(Type targetType, bool inherit)
            where T : Attribute
        {
            var attrs = (T[])targetType.GetCustomAttributes(typeof(T), inherit);
            if (attrs != null && attrs.Length > 0)
            {
                return attrs.FirstOrDefault();
            }
            return null;
        }

        public static List<T> GetAttributes<T>(Type targetType, bool inherit)
            where T : Attribute
        {
            var attrs = (T[])targetType.GetCustomAttributes(typeof(T), inherit);
            return attrs.ToList();
        }

        public static List<PropertyInfo> GetProperties(Type targetType)
        {
            return targetType.GetProperties().ToList();
        }

        public static PropertyInfo GetProperty(Type targetType,string propertyName)
        {
            return targetType.GetProperty(propertyName);
        }

        public static T GetPropertyAttribute<T>(PropertyInfo propertyInfo)
            where T : Attribute
        {
            var result = propertyInfo.GetCustomAttribute(typeof(T));
            return result == null ? null : (T)result;
        }
    }
}
