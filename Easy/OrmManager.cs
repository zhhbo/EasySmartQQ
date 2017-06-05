using Easy.Data.Database.Core;
using Easy.Data.Factory;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq.Expressions;
using System.Linq;
using Easy.Data.Database.Enum;
using Easy.Data.Sql;
using Easy.Data;
namespace Easy
{
    public static class OrmManager
    {
        private static IDatabase _iDatabase;
        private static DatabaseType _dataProvider;
        public static IDatabase SetDatabase(IDatabase iDatabase)
        {
            var oldDatabase = _iDatabase;
            _iDatabase = iDatabase;
            _dataProvider = iDatabase.DatabaseType;
            return oldDatabase;
        }
        public static DatabaseType DataProvider {
            get { return _dataProvider; }
            
        }

        public static IDatabase CurrentDatabase()
        {
            return _iDatabase;
        }/// <summary>
        /// 满足条件的结果
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="whereCondition">条件</param>
        /// <returns></returns>
        public static List<T> Read<T>(string whereCondition)   where T : class, new()
        {
            Type objectType = typeof(T);
            CheckBeforeExe(objectType);
            return DoQuery<T>(objectType, whereCondition);
        }
        #region query
        /// <summary>
        /// 获取满足条件的对象
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="predicate"></param>
        /// <returns></returns>
        public static T Get<T>(Expression<Func<T, bool>> predicate) where T : class, new()
        {
            return Fetch(predicate).FirstOrDefault();
    }
        public static T GetDefault<T>() where T : class, new()
        {
            return GetAll<T>().FirstOrDefault();
        }
        /// <summary>
        /// 满足条件的对象个数
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="predicate">条件</param>
        /// <returns></returns>
        public static int Count<T>(Expression<Func<T, bool>> predicate) where T : class, new()
        {
            return Fetch(predicate).Count();
        }
        public static IQueryable<T> Fetch<T>(Expression<Func<T, bool>> predicate) where T : class, new()
        {
            return GetAll<T>().AsQueryable().Where(predicate);
        }
        /// <summary>
        /// 筛选满足条件的对象
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="predicate">条件</param>
        /// <param name="order">排序</param>
        /// <returns></returns>
        public static IQueryable<T> Fetch<T>(Expression<Func<T, bool>> predicate, Action<Orderable<T>> order) where T : class, new()
        {
            var orderable = new Orderable<T>(Fetch(predicate));
            order(orderable);
            return orderable.Queryable;
        }
        /// <summary>
        /// 执行筛选
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="predicate">条件</param>
        /// <param name="order">排序</param>
        /// <param name="skip">跳过</param>
        /// <param name="count">take</param>
        /// <returns></returns>
        public static IQueryable<T> Fetch<T>(Expression<Func<T, bool>> predicate, Action<Orderable<T>> order, int skip,
                                          int count) where T : class, new()
        {
            return Fetch(predicate, order).Skip(skip).Take(count);
        }
        /// <summary>
        /// 是否存在表，不存在则可以采取创建操作
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static bool HasTable<T>()
            where T : class, new()
        {
            Type objectType = typeof(T);
            CheckBeforeExe(objectType);
                  var table = OrmMapCache.Instance.OrmMap[objectType];
            string strSql = string.Format("select * from {0}", table.TableName);
            try
            {
                DataTable dt = _iDatabase.Query(strSql) as DataTable;
            }
            catch
            {
                return false;
            }
            return true;
        }
        /// <summary>
        /// 读取满足条件的对象
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="queryCondition">条件</param>
        /// <returns></returns>
        public static List<T> Read<T>(QueryCondition queryCondition)
            where T : class, new()
        {
            Type objectType = typeof(T);
            CheckBeforeExe(objectType);
            return DoQuery<T>(objectType, queryCondition);
        }

        public static List<T> Read<T>(SqlString sqlQueryStr)
            where T : class, new()
        {
            Type objectType = typeof(T);
            CheckBeforeExe(objectType);
            return DoQuery<T>(objectType, sqlQueryStr);
        }
/// <summary>
/// 所有对象个数
/// </summary>
/// <typeparam name="T"></typeparam>
/// <returns></returns>
        public static int Count<T>()
            where T : class, new()
        {
            return GetAll<T>().Count;
        }
        /// <summary>
        /// 获取所有对象
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static List<T> GetAll<T>()
            where T : class, new()
        {
            Type objectType = typeof(T);
            CheckBeforeExe(objectType);
            var tableAttr = OrmMapCache.Instance.OrmMap[objectType];
            return Read<T>(new SqlQueryString("select * from " + tableAttr.TableName));

        }
/// <summary>
/// 执行查询
/// </summary>
/// <typeparam name="T"></typeparam>
/// <param name="objectType"></param>
/// <param name="queryCondition"></param>
/// <returns></returns>
        private static List<T> DoQuery<T>(Type objectType, object queryCondition)
             where T : class, new()
        {
            var tableAttr = OrmMapCache.Instance.OrmMap[objectType];
            IDataFactory factory = new SelectTransformFactory(null, tableAttr, queryCondition);
            factory.Database = _iDatabase;
            factory = new SelectExecuteFactory(factory, tableAttr);
            factory = new DataToObjectFactory<T>(factory, tableAttr);
            return factory.GetResult() as List<T>;
        }

        #endregion

        #region insert
        /// <summary>
        /// 插入
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static bool Insert<T>(T obj)
        {
            Type objectType = typeof(T);
            CheckBeforeExe(objectType);
            return DoInsert<T>(objectType, obj);
        }

        public static bool Insert<T>(List<T> objs)
        {
            Type objectType = typeof(T);
            CheckBeforeExe(objectType);
            return DoInsert<T>(objectType, objs);
        }

        public static bool Insert<T>(SqlString sqlInsertStr)
        {
            Type objectType = typeof(T);
            CheckBeforeExe(objectType);
            return DoInsert<T>(objectType, sqlInsertStr);
        }

        private static bool DoInsert<T>(Type objectType, object parameter)
        {
            var tableAttr = OrmMapCache.Instance.OrmMap[objectType];
            IDataFactory factory = new InsertTransformFactory(null, tableAttr, parameter);
            factory.Database = _iDatabase;
            factory = new InsertExecuteFactory(factory, tableAttr);
            return (bool)factory.GetResult();
        }
        #endregion

        #region update
        public static bool Update<T>(T obj)
        {
            Type objectType = typeof(T);
            CheckBeforeExe(objectType);
            return DoUpdate<T>(objectType, obj);
        }

        public static bool Update<T>(List<T> objs)
        {
            Type objectType = typeof(T);
            CheckBeforeExe(objectType);
            return DoUpdate<T>(objectType, objs);
        }

        public static bool Update<T>(SqlString sqlUpdateStr)
        {
            Type objectType = typeof(T);
            CheckBeforeExe(objectType);
            return DoUpdate<T>(objectType, sqlUpdateStr);
        }

        private static bool DoUpdate<T>(Type objectType, object parameter)
        {
            var tableAttr = OrmMapCache.Instance.OrmMap[objectType];
            IDataFactory factory = new UpdateTransformFactory(null, tableAttr, parameter);
            factory.Database = _iDatabase;
            factory = new UpdateExecuteFactory(factory, tableAttr);
            return (bool)factory.GetResult();
        }
        #endregion

        #region delete
        public static bool Clear<T>( )
        {
            Type objectType = typeof(T);
            CheckBeforeExe(objectType);
            return DoClear<T>(objectType);
        }
        public static bool Delete<T>(T obj)
        {
            Type objectType = typeof(T);
            CheckBeforeExe(objectType);
            return DoDelete<T>(objectType, obj);
        }

        public static bool Delete<T>(List<T> objs)
        {
            Type objectType = typeof(T);
            CheckBeforeExe(objectType);
            return DoDelete<T>(objectType, objs);
        }

        public static bool Delete<T>(SqlString sqlDeleteStr)
        {
            Type objectType = typeof(T);
            CheckBeforeExe(objectType);
            return DoDelete<T>(objectType, sqlDeleteStr);
        }

        private static bool DoDelete<T>(Type objectType, object parameter)
        {
            var tableAttr = OrmMapCache.Instance.OrmMap[objectType];
            IDataFactory factory = new DeleteTransformFactory(null, tableAttr, parameter);
            factory.Database = _iDatabase;
            factory = new DeleteExecuteFactory(factory, tableAttr);
            return (bool)factory.GetResult();
        }
        private static bool DoClear<T>(Type objectType)
        {
            var tableAttr = OrmMapCache.Instance.OrmMap[objectType];
            IDataFactory factory = new DeleteTransformFactory(null, tableAttr, new SqlQueryString("DELETE FROM " + tableAttr.TableName));
            factory.Database = _iDatabase;
            factory = new DeleteExecuteFactory(factory, tableAttr);
            return (bool)factory.GetResult();
        }
        #endregion

        private static void CheckBeforeExe(Type objectType)
        {
            if (!MapCollector.Instance.ContainsType(objectType))
            {
                MapCollector.Instance.CollectMapInfo(objectType);
            }
            if (_iDatabase == null)
            {
                throw new Exception("not set database to orm frame!");
            }
            if (!_iDatabase.CanUse)
            {
                throw new Exception("current database can not be used!");
            }
        }
    }
}
