using System;

namespace Easy.Data.Sql
{
    public class QueryExpression
    {
        public string SqlExpress { get; set; }

        public string PropertyName { get; private set; }

        public void Eq(string propertyName,object fieldValue,Type fieldType)
        {
            PropertyName = propertyName;
            SqlExpress = "{0}" + string.Format(" = {0}", OrmManager.CurrentDatabase().GetFieldValue(fieldValue, fieldType));
        }

        public void NotEq(string propertyName, object fieldValue, Type fieldType)
        {
            PropertyName = propertyName;
            SqlExpress = "{0}" + string.Format(" <> {0}", OrmManager.CurrentDatabase().GetFieldValue(fieldValue, fieldType));
        }

        public void Like(string propertyName, object fieldValue, Type fieldType)
        {
            PropertyName = propertyName;
            SqlExpress = "{0}" + string.Format(" like {0}", OrmManager.CurrentDatabase().GetFieldValue(fieldValue, fieldType));
        }

        public void Smaller(string propertyName, object fieldValue, Type fieldType)
        {
            PropertyName = propertyName;
            SqlExpress = "{0}" + string.Format(" < {0}", OrmManager.CurrentDatabase().GetFieldValue(fieldValue, fieldType));
        }

        public void Bigger(string propertyName, object fieldValue, Type fieldType)
        {
            PropertyName = propertyName;
            SqlExpress = "{0}" + string.Format(" > {0}", OrmManager.CurrentDatabase().GetFieldValue(fieldValue, fieldType));
        }

        public void NoSmaller(string propertyName, object fieldValue, Type fieldType)
        {
            PropertyName = propertyName;
            SqlExpress = "{0}" + string.Format(" >= {0}", OrmManager.CurrentDatabase().GetFieldValue(fieldValue, fieldType));
        }

        public void NoBigger(string propertyName, object fieldValue, Type fieldType)
        {
            PropertyName = propertyName;
            SqlExpress = "{0}" + string.Format(" <= {0}", OrmManager.CurrentDatabase().GetFieldValue(fieldValue, fieldType));
        }
    }
}
