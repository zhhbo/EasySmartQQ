using System.Windows;

namespace Easy
{
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            Bootstrapper bootstrapper = new Bootstrapper();
            bootstrapper.Run(true);
        }
        protected override void OnExit(ExitEventArgs e)
        {
            base.OnExit(e);
           
        }
        
    }
}