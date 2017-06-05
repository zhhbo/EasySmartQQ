using System.Windows.Controls;
using System.Windows.Input;
using Easy.Commands;
using Microsoft.Practices.Unity;
using Prism.Events;
using Prism.Logging;
using Prism.Mvvm;
using System.Linq;
namespace Easy
{
    public abstract class ViewModelBase : BindableBase
    {
        private ICommand _loadedCommand;
        private ICommand _loadedCommandWithParam;

        public ICommand LoadedCommand
        {
            get { return _loadedCommand; }
            set { SetProperty(ref _loadedCommand, value); }
        }
        public ICommand LoadedCommandWithParam
        {
            get { return _loadedCommandWithParam; }
            set { SetProperty(ref _loadedCommandWithParam, value); }
        }
        [Dependency]
        protected IUnityContainer Container { get; set; }
        [Dependency]
        protected IEventAggregator EventAggregator { get; set; }
        [Dependency]
        protected ILoggerFacade Logger { get; set; }

        protected ViewModelBase()//IUnityContainer container
        {
           // Container = container;
           // EventAggregator = container.Resolve<IEventAggregator>();
           // Logger = container.Resolve<ILoggerFacade>();
            LoadedCommand = new Command(OnLoaded);
            LoadedCommandWithParam = new Command<ContentControl>(OnLoaded);
           /* int i = 0;
            Logger.Log("\t,序号,Name,RegisteredType.Name,LifetimeManager,", Category.Debug, Priority.High);
            foreach (var item in container.Registrations.Distinct().OrderBy(x=>x.RegisteredType.Name))
            {
                i++;
                Logger.Log("\t,"+i.ToString()+","+item.Name+","+item.RegisteredType.Name+","+item.LifetimeManager+",", Category.Debug, Priority.High);
            }*/
        }

        protected virtual void OnLoaded() { }
        protected virtual void OnLoaded(ContentControl param) { }
    }
}
