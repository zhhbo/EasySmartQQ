using System;
using System.Windows;
using System.Reflection;
using Microsoft.Practices.Unity;
using Prism.Modularity;
using Prism.Unity;
using FirstFloor.ModernUI.Presentation;
using System.Collections.Generic;
using System.Linq;
using Prism.Logging;
using System.IO;
using System.Text;
using Easy.Data.Database.Core;
using Easy.Data.Access.Database;
using Easy.Data.Database.Enum;
using Easy.Data.Access.Connection;
using Easy.Models;
using Easy.Constants;
using Easy.Data;
using Easy.Data.Schema;
namespace Easy
{
    class Bootstrapper : UnityBootstrapper
    {
        private const string MODULES_PATH = @".\modules";
      //  private  LinkGroupCollection linkGroupCollection = null;
        
        protected override DependencyObject CreateShell()
        {
            // Container.RegisterType<IEnumerable<INavigateProvider>, INavigateProvider[]>();
            // MainWindow shell = Container.Resolve<MainWindow>();

            /*  if (linkGroupCollection != null)
              {
                  shell.AddLinkGroups(linkGroupCollection);
              }*/
            CreateSqlCeDb();


            return Container.Resolve<LoginWindow>();
            // return shell;
        }

        private void CreateSqliteDb()
        {
            var _fileName = "easy.db";
            string connectionString = string.Format("Data Source={0}", _fileName);
            var filePath = Path.Combine(Directory.GetCurrentDirectory(), _fileName);
            if (!File.Exists(filePath)) //File.Create(filePath,1,FileOptions.Asynchronous);
            {
               System.Data.SQLite.SQLiteConnection.CreateFile(filePath);
            }
            ConnectionManager.RegisterConnectionType(DatabaseType.SQLite, typeof(SqliteConnection));
            ConnectionManager.SetConnection(DatabaseType.SQLite, connectionString);
            ConnectionManager.InitManager(DatabaseType.SQLite, 1);
           // var logger = Container.Resolve<ILoggerFacade>();
            OrmManager.SetDatabase(new SQLiteDatabase());
        }
        private void CreateSqlCeDb()
        {
            var _fileName = "easy.sdf";
            string connectionString = string.Format("Data Source={0}", _fileName);
            var filePath = Path.Combine(Directory.GetCurrentDirectory(), _fileName);
            if (!File.Exists(filePath)) //File.Create(filePath,1,FileOptions.Asynchronous);
            {
                const string assemblyName = "System.Data.SqlServerCe";
                const string typeName = "System.Data.SqlServerCe.SqlCeEngine";

                var sqlceEngineHandle = Activator.CreateInstance(assemblyName, typeName);
                var engine = sqlceEngineHandle.Unwrap();

                engine.GetType().GetProperty("LocalConnectionString").SetValue(engine, connectionString, null/*index*/);

                //engine.CreateDatabase();
                engine.GetType().GetMethod("CreateDatabase").Invoke(engine, null);

                //engine.Dispose();
                engine.GetType().GetMethod("Dispose").Invoke(engine, null);
            }
            ConnectionManager.RegisterConnectionType(DatabaseType.SqlCe, typeof(SqlceConnection));
            ConnectionManager.SetConnection(DatabaseType.SqlCe, connectionString);
            ConnectionManager.InitManager(DatabaseType.SqlCe, 1);
            var logger = Container.Resolve<ILoggerFacade>();
            OrmManager.SetDatabase(new SqlCeDatabase(logger));
        }
        protected override void InitializeShell()
        {
            base.InitializeShell();

            //Application.Current.MainWindow = (Window)Shell;
            if (OrmManager.HasTable<SettingModel>())
            {
                var setting = OrmManager.GetDefault<SettingModel>();
                if (Theme.GetTheme(setting.Theme) != null)
                    AppearanceManager.Current.ThemeSource = new Uri(Theme.GetTheme(setting.Theme).Path, UriKind.Relative);
                var fontsize = Theme.GetThemeFrontSize(setting.FontSize);
                Application.Current.Resources[AppearanceManager.KeyDefaultFontSize] = fontsize.DefaultFontSize;
                Application.Current.Resources[AppearanceManager.KeyFixedFontSize] = fontsize.FixedFontSize;
                AppearanceManager.Current.AccentColor = Theme.GetThemeColorByName(setting.Color).ColorAlias;


            }
            Application.Current.MainWindow.Show();
        }

        protected override void ConfigureContainer()
        {        
            base.ConfigureContainer();

        }
        
       protected override IModuleCatalog CreateModuleCatalog()
        {
            return new DirectoryModuleCatalog() { ModulePath = MODULES_PATH };
        }
        protected override void InitializeModules()
        {
            var modulePath = Path.Combine(Directory.GetCurrentDirectory(),MODULES_PATH);
            if (!Directory.Exists(modulePath))
            {
                Directory.CreateDirectory(modulePath);
            }
            var directoryCatalog = (DirectoryModuleCatalog)ModuleCatalog;
            if (directoryCatalog.Items.Count > 0)
            {
                var asms = directoryCatalog.Items.Select(x => Assembly.LoadFrom(((ModuleInfo)x).Ref)).ToList();
                asms.Add(typeof(Bootstrapper).Assembly);
                Register(asms);
            }
            else
            {
                Register(new[] { typeof(Bootstrapper).Assembly });
            }
            Container.RegisterType<IEnumerable<INavigateProvider>, INavigateProvider[]>();
            Container.RegisterType<IEnumerable<ICommandInterpreter>, ICommandInterpreter[]>();
            Container.RegisterType<IEnumerable<IDataMigration>, IDataMigration[]>();
            Container.RegisterType<IEnumerable<QQRob.Services.IAutoAnswer>, QQRob.Services.IAutoAnswer[]>();
            // Container.RegisterInstance(Container);
            base.InitializeModules();

            var schemaBuilder = new SchemaBuilder(Container.Resolve<IDataMigrationInterpreter>());
            if (!OrmManager.HasTable<Tables.OrmTablesInfo>())
            {
              
                schemaBuilder.CreateTable("OrmTablesInfo", table => table
                            .Column<int>("Id", column => column.PrimaryKey().Identity())//PrimaryKey().
                            .Column<string>("DataMigrationClass")
                            .Column<int>("Version"));
            }
            if (!OrmManager.HasTable<SettingModel>())
            {
                schemaBuilder.CreateTable("Setting", table => table
                             .Column<int>("Id", column => column.PrimaryKey().Identity())//PrimaryKey().
                             .Column<string>("Theme")
                             .Column<string>("FontSize")
                             .Column<string>("Color"));
                OrmManager.Insert(new SettingModel());
                var setting = new SettingModel();

                AppearanceManager.Current.ThemeSource = new Uri(Theme.GetTheme(setting.Theme).Path, UriKind.Relative);
                var fontsize = Theme.GetThemeFrontSize(setting.FontSize);
                Application.Current.Resources[AppearanceManager.KeyDefaultFontSize] = fontsize.DefaultFontSize;
                Application.Current.Resources[AppearanceManager.KeyFixedFontSize] = fontsize.FixedFontSize;
                AppearanceManager.Current.AccentColor = Theme.GetThemeColorByName(setting.Color).ColorAlias;
            }
         /*       var links = Container.ResolveAll<INavigateProvider>();
            var shell = (this.Shell as MainWindow);
            if (shell != null && links.Count() > 0)
            {
                foreach (var link in links.Where(x => x.Name == NavigateProvider.Main))
                {
                    //  linkGroupCollection.Add(link.GetLinkGroup());

                    shell.AddLinkGroups(new LinkGroupCollection() { link.GetLinkGroup() });//.AddLinkGroups(linkGroupCollection);

                }
                foreach (var link in links.Where(x => x.Name == NavigateProvider.Title))
                {
                    shell.AddTitleLinks(link.GetLinkGroup().Links);
                }
            }*/
            var datemanager = Container.Resolve<IDataMigrationManager>();
              var features=  datemanager.GetFeaturesThatNeedUpdate();
              datemanager.Update(features);
         

        }
        protected override ILoggerFacade CreateLogger()
        {
            var dic = Path.Combine(Directory.GetCurrentDirectory(), "Logs");
            if (!Directory.Exists(dic))
            {
                Directory.CreateDirectory(dic);
            }
            var filePath = Path.Combine(dic, $"{DateTime.Now.Date.ToString("yyyy-MM-dd")}-log.log");
            if (File.Exists(filePath)) File.Delete(filePath);
            var file = new FileStream(filePath, FileMode.OpenOrCreate, FileAccess.Write, FileShare.Read);
            var writer = new StreamWriter(file, Encoding.UTF8) { AutoFlush = true };
            return new TextLogger(writer);
        }
        protected override void ConfigureModuleCatalog()
        {

          //  var directoryCatalog = (DirectoryModuleCatalog)ModuleCatalog;
          //  directoryCatalog.Initialize();
            //linkGroupCollection = new LinkGroupCollection();

        }

        private static bool IsDependency(Type type)
        {
            return typeof(IDependency).IsAssignableFrom(type);
        }
        private static IEnumerable<T> BuildBlueprint<T>(
           IEnumerable<Assembly> features,
           Func<Type, bool> predicate,
           Func<Type, Assembly, T> selector
           )
        {

            // Load types excluding the replaced types
            return features.SelectMany(x => x.GetExportedTypes()
                    .Where(t => t.IsClass && !t.IsAbstract)
                    .Except(new[] { typeof(IModule) })
                    .ToArray()
                    .Where(predicate)
                    .Select(type => selector(type, x))
                .ToArray());
        }
        public class FeatureType
        {
            public string AssemblyName { get; set; }
            public Type Type { get; set; }
        }
        private void Register(IEnumerable< Assembly> assemblies)
        {
            //var assemblies = AppDomain.CurrentDomain.GetAssemblies();
            var features = BuildBlueprint<FeatureType>(assemblies, IsDependency, (t, a) => { return new FeatureType() { Type = t, AssemblyName = a.FullName }; });

           // Container.RegisterTypes(,)
            foreach (var item in features.Where(t => typeof(IDependency).IsAssignableFrom(t.Type)))
            {
               // Container.RegisterType(item.Type);
                foreach (var it in item.Type.GetInterfaces().Except(new[] { typeof(IEventHandler), typeof(IDependency), typeof(ISingletonDependency) }))
                {
                    if (typeof(ISingletonDependency).IsAssignableFrom(it))
                    {
                        Container.RegisterType(it, item.Type ,new PerThreadLifetimeManager());
                        Logger.Log(item.Type.FullName+"->"+it.Name, Category.Debug, Priority.Low);
                    }
                    else if (typeof(IEventHandler).IsAssignableFrom(it))
                    {
                        Container.RegisterType(it, item.Type, item.Type.FullName);//, new TransientLifetimeManager()
                        Logger.Log(item.Type.FullName + "->" + it.Name + "注册名：" + item.Type.FullName, Category.Debug, Priority.Low);
                    }
                    else 
                    {                      
                        Container.RegisterType(it, item.Type,new PerResolveLifetimeManager());
                        Logger.Log(item.Type.FullName + "->" + it.Name , Category.Debug, Priority.Low);
                    }

                }

            }
        }
    }
}