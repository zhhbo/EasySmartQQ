using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Easy;
using Easy.Data.Schema;
using Microsoft.Practices.Unity;
using Prism.Logging;
namespace  Easy.Data

{
    /// <summary>
    /// Reponsible for maintaining the knowledge of data migration in a per tenant table
    /// </summary>
    public class DataMigrationManager : IDataMigrationManager
    {
        private readonly IEnumerable<IDataMigration> _dataMigrations;
        private readonly IDataMigrationInterpreter _interpreter;
        private readonly ILoggerFacade Logger;
        private readonly List<string> _processedFeatures;

        public DataMigrationManager(
            IUnityContainer container,
           IEnumerable<IDataMigration> dataMigrations,
           IDataMigrationInterpreter interpreter,
           ILoggerFacade logger
            ) {
            _dataMigrations = dataMigrations; //container.ResolveAll<IDataMigration>(); //FindAllDataMigrationClass();
            _interpreter = interpreter; //new Interpreters.DefaultDataMigrationInterpreter();
            Logger = logger;
            _processedFeatures = new List<string>();
        }
        private IEnumerable<IDataMigration> FindAllDataMigrationClass()
        {
            return AppDomain.CurrentDomain.GetAssemblies()
                                 .SelectMany(a => a.GetTypes().Where(t => t.GetInterfaces().Contains(typeof(IDataMigration))))
                                 .Select(x => x as IDataMigration);
            ;

        }

        public IEnumerable<string> GetFeaturesThatNeedUpdate() {
            var currentVersions = OrmManager.GetAll<Tables.OrmTablesInfo>().ToDictionary(r => r.DataMigrationClass);

            var outOfDateMigrations = _dataMigrations.Where(dataMigration => {
                Tables.OrmTablesInfo record;
                if (currentVersions.TryGetValue(dataMigration.GetType().FullName, out record))
                    return CreateUpgradeLookupTable(dataMigration).ContainsKey(record.Version);

                return (GetCreateMethod(dataMigration) != null);
            });

            return outOfDateMigrations.Select(m =>m.GetType().Assembly.FullName).ToList();
        }

        public void Update(IEnumerable<string> features) {
            foreach (var feature in features) {
                if (!_processedFeatures.Contains(feature)) {
                    Update(feature);
                }
            }
        }

        public void Update(string feature) {
            if (_processedFeatures.Contains(feature)) {
                return;
            }

            _processedFeatures.Add(feature);


            var migrations = GetDataMigrations(feature);

            // apply update methods to each migration class for the module
            foreach (var migration in migrations) {

                var tempMigration = migration;

                // get current version for this migration
                var dataMigrationRecord = GetDataMigrationRecord(tempMigration);

                var current = 0;
                if (dataMigrationRecord != null) {
                    current = dataMigrationRecord.Version;
                }

                try {
                    // do we need to call Create() ?
                    if (current == 0) {
                        // try to resolve a Create method

                        var createMethod = GetCreateMethod(migration);
                        if (createMethod != null) {
                            current = (int)createMethod.Invoke(migration, new object[0]);
                        }
                    }

                    var lookupTable = CreateUpgradeLookupTable(migration);

                    while (lookupTable.ContainsKey(current)) {
                        try {
                           // Logger.Information("Applying migration for {0} from version {1}.", feature, current);
                            current = (int)lookupTable[current].Invoke(migration, new object[0]);
                        }
                        catch  {

                            throw;
                        }
                    }

                    // if current is 0, it means no upgrade/create method was found or succeeded 
                    if (current == 0) {
                        continue;
                    }
                    if (dataMigrationRecord == null) {
                        OrmManager.Insert(new Tables.OrmTablesInfo { Version = current, DataMigrationClass = migration.GetType().FullName });
                    }
                    else {
                        dataMigrationRecord.Version = current;
                        OrmManager.Update(dataMigrationRecord);
                    }
                }
                catch (Exception ex) {
                    Logger.Log(ex.Message,Category.Exception,Priority.High);
                    //throw new Exception(string.Format("Error while running migration version {0} for {1}.", current, feature), ex);
                }

            }
        }

        public void Uninstall(string feature) {
           // Logger.Information("Uninstalling feature: {0}.", feature);

            var migrations = GetDataMigrations(feature);

            // apply update methods to each migration class for the module
            foreach (var migration in migrations) {
                // copy the object for the Linq query
                var tempMigration = migration;

                // get current version for this migration
                var dataMigrationRecord = GetDataMigrationRecord(tempMigration);

                var uninstallMethod = GetUninstallMethod(migration);
                if (uninstallMethod != null) {
                    uninstallMethod.Invoke(migration, new object[0]);
                }

                if (dataMigrationRecord == null) {
                    continue;
                }

                OrmManager.Delete(dataMigrationRecord);
                //_dataMigrationRepository.Flush();
            }

        }

        private Tables.OrmTablesInfo GetDataMigrationRecord(IDataMigration tempMigration) {
            return OrmManager.Read<Tables.OrmTablesInfo>(" DataMigrationClass='" + tempMigration.GetType().FullName + "'").FirstOrDefault();
        }

        /// <summary>
        /// Returns all the available IDataMigration instances for a specific module, and inject necessary builders
        /// </summary>
        private IEnumerable<IDataMigration> GetDataMigrations(string feature) {
            var migrations = _dataMigrations
                    .Where(dm => String.Equals(dm.GetType().Assembly.FullName, feature, StringComparison.OrdinalIgnoreCase))
                    .ToList();

            foreach (var migration in migrations.OfType<DataMigrationImpl>()) {
                migration.SchemaBuilder = new SchemaBuilder(_interpreter);
           }

            return migrations;
        }

        /// <summary>
        /// Whether a feature has already been installed, i.e. one of its Data Migration class has already been processed
        /// </summary>
        public bool IsFeatureAlreadyInstalled(string feature) {
            return GetDataMigrations(feature).Any(dataMigration => GetDataMigrationRecord(dataMigration) != null);
        }

        /// <summary>
        /// Create a list of all available Update methods from a data migration class, indexed by the version number
        /// </summary>
        private static Dictionary<int, MethodInfo> CreateUpgradeLookupTable(IDataMigration dataMigration) {
            return dataMigration
                .GetType()
                .GetMethods(BindingFlags.Public | BindingFlags.Instance)
                .Select(GetUpdateMethod)
                .Where(tuple => tuple != null)
                .ToDictionary(tuple => tuple.Item1, tuple => tuple.Item2);
        }

        private static Tuple<int, MethodInfo> GetUpdateMethod(MethodInfo mi) {
            const string updatefromPrefix = "UpdateFrom";

            if (mi.Name.StartsWith(updatefromPrefix)) {
                var version = mi.Name.Substring(updatefromPrefix.Length);
                int versionValue;
                if (int.TryParse(version, out versionValue)) {
                    return new Tuple<int, MethodInfo>(versionValue, mi);
                }
            }

            return null;
        }

        /// <summary>
        /// Returns the Create method from a data migration class if it's found
        /// </summary>
        private static MethodInfo GetCreateMethod(IDataMigration dataMigration) {
            var methodInfo = dataMigration.GetType().GetMethod("Create", BindingFlags.Public | BindingFlags.Instance);
            if (methodInfo != null && methodInfo.ReturnType == typeof(int)) {
                return methodInfo;
            }

            return null;
        }

        /// <summary>
        /// Returns the Uninstall method from a data migration class if it's found
        /// </summary>
        private static MethodInfo GetUninstallMethod(IDataMigration dataMigration) {
            var methodInfo = dataMigration.GetType().GetMethod("Uninstall", BindingFlags.Public | BindingFlags.Instance);
            if (methodInfo != null && methodInfo.ReturnType == typeof(void)) {
                return methodInfo;
            }

            return null;
        }

    }
}
