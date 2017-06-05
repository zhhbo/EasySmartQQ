using System;

namespace Easy.Data.Attributes
{
    public class PrimaryKeyAttribute : PropertyFieldMapAttribute
    {
        public PrimaryKeyAttribute(string propertyName, Type propertyType, string fieldName, Type fieldType) 
            : base(propertyName, propertyType, fieldName, fieldType)
        {
        }
    }
}
