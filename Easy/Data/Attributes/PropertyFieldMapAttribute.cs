using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Easy.Data.Attributes
{
    public class PropertyFieldMapAttribute : Attribute
    {
        private string _propertyName;
        public string PropertyName { get { return _propertyName; } }
        private Type _propertyType;
        public Type PropertyType { get { return _propertyType; } }
        private string _fieldName;
        public string FieldName { get { return _fieldName; } }
        private Type _fieldType;
        public Type FieldType { get { return _fieldType; } }

        public PropertyFieldMapAttribute(string propertyName, Type propertyType, string fieldName, Type fieldType)
        {
            _propertyName = propertyName;
            _propertyType = propertyType;
            _fieldName = fieldName;
            _fieldType = fieldType;
        }
    }
}
