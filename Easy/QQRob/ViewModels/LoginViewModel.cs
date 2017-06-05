using System;
using Microsoft.Practices.Unity;
using System.Windows.Input;
using Easy.QQRob.Events;
using Easy.Commands;
using System.Windows.Media.Imaging;
using Easy.QQRob.Services;
using System.Windows;
using Easy;
using Prism.Regions;
using Prism.Events;
using Prism.Logging;
using System.IO;
namespace Easy.QQRob.ViewModels
{
    public class LoginViewModel : ViewModelBase
    {
        public LoginViewModel(IEventAggregator eventAggregator,ILoggerFacade logger,  IRegionManager regionManager,IUnityContainer container) //,ISmartService smartService: IUnityContainer container,base(container)
        {
            EventAggregator = eventAggregator;
            Logger = logger;
            RegionManager = regionManager;
            EventAggregator.GetEvent<LoginStatusChangedEvent>().Subscribe(OnLoginStatusChanged,true);
            EventAggregator.GetEvent<InitComplatedEvent>().Subscribe(OnInitedChanged, true);
           // SmartQQService = smartService;//container.Resolve<ISmartService>(nameof(SmartService));
            LoginCommand = new Command<System.Windows.Controls.Image>(image => {
                var clientManager = ClientManager.GetClientManagerUser(container, new Models.QQState());
                clientManager.Client.Logined += Client_Logined;
               var qrimage = clientManager.Client.GetLoginQrImage();
                if (qrimage != null)
                {
                    System.Drawing.Bitmap bmp = new System.Drawing.Bitmap(qrimage);
                    IntPtr hBitmap = bmp.GetHbitmap();
                    image.Source = System.Windows.Interop.Imaging.CreateBitmapSourceFromHBitmap(hBitmap, IntPtr.Zero, Int32Rect.Empty, BitmapSizeOptions.FromWidthAndHeight(165, 165)); //new BitmapImage(new Uri(tempimage, UriKind.Absolute));
                    clientManager.Client.LoginMain();
                }
                else
                {
                    Message = "网络故障";
                    RaisePropertyChanged(nameof(Message));
                }
            }, null);
        }

        private void Client_Logined(object sender, LoginSucessedEventArgs e)
        {
            // OrmManager.Update(state);
            var state = OrmManager.Get<Models.QQState>(x=>x.QQNum==e.Session.QQNum);
            
            var clientManager = ClientManager.GetClientManagerUser(Container, new Models.QQState());
            clientManager.QQ.QQNum = e.Session.QQNum;
            clientManager.QQ.Logined = true;
            clientManager.QQ.SetCookies(Constract.LoginCookies, e.JsonResult);
            //clientManager.QQ.SetCookies(Constract.QQSession, e.Session);
            if (state != null)
            {
                clientManager.QQ.Id = state.Id;
                OrmManager.Update(clientManager.QQ);
            }
            else
            {
                OrmManager.Insert(clientManager.QQ);
            }
            WorkContext.SetState(Constract.CurrentQQState, clientManager.QQ);
            WorkContext.SetState(Constract.CurrentQQ, clientManager.QQ.QQNum);
            clientManager.Client.DefaultState = clientManager.QQ.State;
            ClientManager.ResetKey();
            //throw new NotImplementedException();
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

        //  public ISmartService SmartQQService { get; }
        private IRegionManager RegionManager;
       public ICommand LoginCommand { get; set; }

        public string Message { get; set; }
       
        public void OnLoginStatusChanged(LoginEventArgs e)
        {
            Message = e.Message;
            RaisePropertyChanged(nameof(Message));
            Logger.Log(e.Message,Prism.Logging.Category.Info,Prism.Logging.Priority.High);
            if (e.Code == "0")
            {
                Message = "登录成功，正在初始化数据请稍后";
            }
        }
        public void OnInitedChanged(InitComplatedEventArgs e)
        {
            Message = e.Message;
            RaisePropertyChanged(nameof(Message));
            Logger.Log(e.Message, Prism.Logging.Category.Info, Prism.Logging.Priority.High);
            if (e.Code == "0")
            {
                Message = "初始化完成";
                //Navigate(new Uri("pack://application:,,,/GreetingPage.xaml"));
                RegionManager.RequestNavigate("ContentRegion", new Uri($"/Easy.QQRob;component/Views/{nameof(Easy.QQRob.Views.ListGroup)}.xaml", UriKind.Relative));
            }

        }
    }
}
