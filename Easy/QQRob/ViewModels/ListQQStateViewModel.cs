using System.Collections.Generic;
using Microsoft.Practices.Unity;
using Easy.QQRob.Services;
using System.Windows.Input;
using System.Windows;
using Easy.Commands;
using Easy.QQRob.Models;
using Prism.Logging;
using System.ComponentModel;
namespace Easy.QQRob.ViewModels
{
    public class ListQQStateViewModel : ViewModelBase
    {
        public ListQQStateViewModel(IUnityContainer container) //ISmartService smartService: base(container)IUnityContainer container
        {
            Container = container;
            Logger = container.Resolve<ILoggerFacade>();
               States =  OrmManager.Fetch<QQState>(x=>x.Id>0);
            SelectedCommand = new Command(() =>{
                Logger.Log(SelectedState.QQNum.ToString(),Prism.Logging.Category.Debug, Prism.Logging.Priority.High);
            }, ()=>SelectedState!=null);
            RefreshCommand = new Command(() => {

                States = OrmManager.Fetch<QQState>(x => x.Id > 0);
                RaisePropertyChanged(nameof(States));
                // Logger.Log(SelectedGroup.Name, Prism.Logging.Category.Debug, Prism.Logging.Priority.High);
            });
            TryLoginSelectedCommand = new Command(() => {
                Message = "开始登录";
                RaisePropertyChanged(nameof(Message));
                // var state = WorkContext.GetState<QQState>(Constract.CurrentQQState);
                // state?.GetClient().InitFriendList();
                var clientManager = ClientManager.GetClientManagerUser(container, SelectedState);
                WorkContext.SetState(Constract.CurrentQQState, SelectedState);
                WorkContext.SetState(Constract.CurrentQQ, SelectedState.QQNum);
                clientManager.Client.Logined += Client_Logined;
                if (clientManager.Client.TryLogin(SelectedState))
                {
                    Message = "登录成功";
                    RaisePropertyChanged(nameof(Message));
                }
                else
                {
                    Message = "登录失败";
                    RaisePropertyChanged(nameof(Message));
                }
               // States = OrmManager.Fetch<QQState>(x => x.Id > 0);
            
                Logger.Log(SelectedState.QQNum.ToString(), Prism.Logging.Category.Debug, Prism.Logging.Priority.High);
            }, () => SelectedState != null);
        }

        private void Client_Logined(object sender, LoginSucessedEventArgs e)
        {
            var state= WorkContext.GetState<QQState>(Constract.CurrentQQState);
            state.Logined = true;
            state.SetCookies(Constract.LoginCookies,e.JsonResult);
           // state.SetCookies(Constract.QQSession, e.Session);
            OrmManager.Update(state);
            var clientManager = ClientManager.GetClientManagerUser(Container, state);
            clientManager.QQ.Logined = true;
            clientManager.QQ.State = state.State;
            clientManager.Client.DefaultState = state.State;
            if (e.ShowMainWindow)
            {
                App.Current.Dispatcher.Invoke(() =>
                {
                    new MainWindow(Container).Show();
                    Logger.Debug("MainWindow has been created and displayed.");
                    // 3.Closes the SignIn window.
                    (Application.Current.Resources[LoginWindow.Key] as LoginWindow)?.Close();
                    Logger.Debug("LoginWindow has been closed.");
                });
            }


        }

        public  IEnumerable<QQState> States { get; set; }
        public QQState SelectedState { get; set; }
        public ICommand SelectedCommand { get; set; }
        public ICommand RefreshCommand { get; set; }
        public ICommand TryLoginSelectedCommand { get; set; }
        public string Message { get; set; }
       // public IUnityContainer Container { get; set; }
    }
}
