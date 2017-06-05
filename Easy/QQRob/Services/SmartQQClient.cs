using Easy.QQRob.Data.SmartQQ;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Net;
using Easy.QQRob.Models;
using Prism.Events;
using Easy.QQRob.Events;
using Prism.Logging;
using Microsoft.Practices.Unity;
using System.Linq;
using Easy.QQRob.Extensions;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
namespace Easy.QQRob.Services
{
    public class SmartQQClient  
    {       /// <summary>
            ///     发送消息的目标类型。
            /// </summary>

        public SmartQQClient(IUnityContainer container)
        {
            Logger = container.Resolve<ILoggerFacade>();
            EventAggregator = container.Resolve<IEventAggregator>();
            Answers = container.ResolveAll<IAutoAnswer>();

            FriendMessageReceived += PrivateMessageProcessor;
            DiscussionMessageReceived += DisscussMessageProcessor;
            GroupMessageReceived += GroupMessageProcessor;

        }

        public IEnumerable<IAutoAnswer> Answers { get; set; }
        public IEventAggregator EventAggregator { get; set; }
        public ILoggerFacade Logger { get; set; }
        public System.Timers.Timer Login_QRStatuTimer = new System.Timers.Timer();

       // public long QQNum;
        public FriendInfo SelfInfo = new FriendInfo();

        public QQSession Session { get; set; } = new QQSession();

        // 线程开关
        private volatile bool _pollStarted;


        /// <summary>
        ///     发送消息的重试次数。
        /// </summary>
        public int RetryTimes { get; set; } = 5;
        public string DefaultState  { get;set; }

        /// <summary>
        ///     客户端当前状态。
        /// </summary>
        public ClientStatus Status { get; private set; } = ClientStatus.Idle;

        #region 网络请求相关
        public CookieContainer Cookies { get; set; }= new CookieContainer();
        /// <summary>
        /// 获取二维码流
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        public System.IO.Stream GetStream(ApiUrl url)
        {
            //string dat;
            HttpWebResponse res = null;
            HttpWebRequest req;
            try
            {
                req = (HttpWebRequest)WebRequest.Create(url.Url);
                req.CookieContainer = Cookies;
                if (!string.IsNullOrEmpty(url.Referer))
                {
                    req.Referer = url.Referer;
                }
                req.UserAgent = ApiUrl.UserAgent;
                req.AuthenticationLevel = System.Net.Security.AuthenticationLevel.None;
                res = (HttpWebResponse)req.GetResponse();
                Cookies.Add(res.Cookies);
            }
            catch (System.Web.HttpException)
            {
                return null;
            }
            catch (WebException e)
            {
                Logger.Error(e);
                return null;
            }

            var result = new System.IO.MemoryStream(res.GetResponseStream().ToBytes());
            res.Close();
            req.Abort();
            return result;
        }
        /// <summary>
        /// 获取信息
        /// </summary>
        /// <param name="url"></param>
        /// <param name="allowAutoRedirect"></param>
        /// <param name="args"></param>
        /// <returns></returns>
        public  string Get(ApiUrl url, bool? allowAutoRedirect, params object[] args)
        {
            string dat;
            HttpWebResponse res = null;
            HttpWebRequest req;
            try
            {
                req = (HttpWebRequest)WebRequest.Create(url.BuildUrl(args));
                req.CookieContainer = Cookies;
                if(allowAutoRedirect.HasValue)
                 req.AllowAutoRedirect = allowAutoRedirect.Value;
                if (!string.IsNullOrEmpty(url.Referer))
                {
                    req.Referer = url.Referer;
                }  
                req.UserAgent =ApiUrl.UserAgent;
                res = (HttpWebResponse)req.GetResponse();
                Cookies.Add(res.Cookies);
            }
            catch (System.Web.HttpException e)
            {
                Logger.Error(e,url.Url);
                return "";
            }
            catch (WebException e)
            {
                Logger.Error(e, url.Url);
                return "";
            }
           System.IO.StreamReader reader;
            
            reader = new System.IO.StreamReader(res.GetResponseStream(),  System.Text. Encoding.GetEncoding(res.CharacterSet));
            dat = reader.ReadToEnd();
            
            res.Close();
            req.Abort();

            if (!dat.Equals(""))
            {

            }

            return dat;
        }
        public string Get(ApiUrl url, params object[] args) => Get(url,null,args);
        public  string Post(ApiUrl url, JObject json, int timeout)
        {
            string dat = "";
            HttpWebRequest req;
            try
            {
                req = WebRequest.Create(url.Url) as HttpWebRequest;
                req.CookieContainer = Cookies;
                req.ContentType = "application/x-www-form-urlencoded; charset=UTF-8";
                req.Method = "POST";
                req.Proxy = null;
                req.KeepAlive = true;
                req.Timeout = timeout;
                req.UserAgent = ApiUrl.UserAgent;
                req.Accept = "*/*";
                req.Headers.Add("Origin", url.Origin);
                req.ProtocolVersion = HttpVersion.Version11;
                if (!string.IsNullOrEmpty(url.Referer))
                {
                    req.Referer = url.Referer;
                }
                byte[] mybyte = System.Text.Encoding.Default.GetBytes("r=" + System.Web.HttpUtility.UrlEncode(json.ToString(Formatting.None)));
                req.ContentLength = mybyte.Length;

                System.IO.Stream stream = req.GetRequestStream();
                stream.Write(mybyte, 0, mybyte.Length);


                HttpWebResponse res = req.GetResponse() as HttpWebResponse;
                Cookies.Add(res.Cookies);
                stream.Close();

                System.IO. StreamReader SR = new System.IO.StreamReader(res.GetResponseStream(), System.Text.Encoding.GetEncoding(res.CharacterSet));
                dat = SR.ReadToEnd();
                res.Close();
                req.Abort();
            }
            catch (System.Web.HttpException e)
            {
                Logger.Error(e, url.Url);
                //OrmManager.Insert(new SystemLogData { OriginData = e.Message, Message = "网络故障" });
                return "";
            }
            catch (WebException e)
            {
                Logger.Error(e, url.Url);
                // OrmManager.Insert(new SystemLogData { OriginData = e.Message, Message = "网络故障" });
                return "";
            }
            if (!dat.Equals(""))
            {
                // OrmManager.Insert(new SystemLogData { OriginData = dat.Replace(';',' '), Message = "System" });
            }
            return dat;
        }
        public string Post(ApiUrl url, JObject json) => Post(url,json,-1);
        public string Post(ApiUrl url,string referer, JObject json, int timeout)
        {
            string dat = "";
            HttpWebRequest req;
            try
            {
                req = WebRequest.Create(url.Url) as HttpWebRequest;
                req.CookieContainer = Cookies;
                req.ContentType = "application/x-www-form-urlencoded; charset=UTF-8";
                req.Method = "POST";
                req.Proxy = null;
                req.KeepAlive = true;
                req.Timeout = timeout;
                
                req.UserAgent = ApiUrl.UserAgent;// "Mozilla/5.0 (Windows NT 10.0; WOW64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/53.0.2785.104 Safari/537.36 Core/1.53.2372.400 QQBrowser/9.5.10548.400";
                req.Accept = "*/*";
                req.Headers.Add("Origin", url.Origin);
                req.ProtocolVersion = HttpVersion.Version11;
                if (!string.IsNullOrEmpty(url.Referer))
                {
                    req.Referer = url.Referer;
                }
                byte[] mybyte = System.Text.Encoding.Default.GetBytes("r=" + System.Web.HttpUtility.UrlEncode(json.ToString(Formatting.None)));
                req.ContentLength = mybyte.Length;

                System.IO.Stream stream = req.GetRequestStream();
                stream.Write(mybyte, 0, mybyte.Length);


                HttpWebResponse res = req.GetResponse() as HttpWebResponse;
                Cookies.Add(res.Cookies);
                stream.Close();

                System.IO.StreamReader SR = new System.IO.StreamReader(res.GetResponseStream(), System.Text.Encoding.GetEncoding(res.CharacterSet));
                dat = SR.ReadToEnd();
                res.Close();
                req.Abort();
            }
            catch (System.Web.HttpException e)
            {
                Logger.Error(e, url.Url);
                //OrmManager.Insert(new SystemLogData { OriginData = e.Message, Message = "网络故障" });
                return "";
            }
            catch (WebException e)
            {
                Logger.Error(e, url.Url);
                // OrmManager.Insert(new SystemLogData { OriginData = e.Message, Message = "网络故障" });
                return "";
            }
            if (!dat.Equals(""))
            {
                // OrmManager.Insert(new SystemLogData { OriginData = dat.Replace(';',' '), Message = "System" });
            }
            return dat;
        }
        public string PostWithRetry(ApiUrl url, JObject json, int retryTimes)
        {
            string dat = "";

            try
            {

               do
                {
                    dat = Post(url, json);
                    retryTimes++;
                } while (retryTimes >= 0 && string.IsNullOrEmpty(dat));

                return dat;

            }
            catch (Exception e)
            {
                Logger.Error(e, url.Url);
               // OrmManager.Insert(new SystemLogData { OriginData = e.Message, Message = "网络故障" });
                return "";
            }

        }

        public  string PostJson(ApiUrl url, JObject json, int timeout = -1)
        {
            string dat = "";
            HttpWebRequest req;
            try
            {
                req = WebRequest.Create(url.Url) as HttpWebRequest;
                req.ContentType = "application/json;charset=utf-8";
                req.Method = "POST";
                req.CookieContainer = Cookies;
                req.Proxy = null;
                req.Timeout = timeout;
                if (!string.IsNullOrEmpty(url.Referer))
                {
                    req.Referer = url.Referer;
                }
                req.UserAgent = ApiUrl.UserAgent;// "Mozilla/5.0 (Windows NT 10.0; WOW64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/53.0.2785.104 Safari/537.36 Core/1.53.2372.400 QQBrowser/9.5.10548.400";
                req.ProtocolVersion = HttpVersion.Version11;
                req.Headers.Add("Origin", url.Origin);
                byte[] mybyte = System.Text.Encoding.UTF8.GetBytes(System.Web.HttpUtility.UrlEncode(json.ToString(Formatting.None)));
                req.ContentLength = mybyte.Length;

                System.IO.Stream stream = req.GetRequestStream();
                stream.Write(mybyte, 0, mybyte.Length);


                HttpWebResponse res = req.GetResponse() as HttpWebResponse;

                stream.Close();

                System.IO.StreamReader SR = new System.IO.StreamReader(res.GetResponseStream(), System.Text.Encoding.GetEncoding(res.CharacterSet));
                dat = SR.ReadToEnd();
                res.Close();
                req.Abort();
            }
            catch (System.Web.HttpException e)
            {
                Logger.Error(e, url.Url);
                return "";
            }
            catch (WebException e)
            {
                Logger.Error(e, url.Url);
                return "";
            }
            if (!dat.Equals(""))
            { OrmManager.Insert(new SystemLogData { OriginData = dat.Replace(';', ' '), Message = "System" }); }
            return dat;
        }
        public string PostData(ApiUrl url, Dictionary<string, object> json, int timeout = -1)
        {
            string dat = "";
            HttpWebRequest req;
            try
            {
                req = WebRequest.Create(url.Url) as HttpWebRequest;
                req.ContentType = "application/x-www-form-urlencoded;charset=UTF-8";
                req.Method = "POST";
                req.CookieContainer = Cookies;
                req.Proxy = null;
                req.Timeout = timeout;
                req.Accept = "application/json, text/javascript, */*; q=0.01";
                req.AllowAutoRedirect = true;
                req.Headers.Add("X-Requested-With","XMLHttpRequest");
                req.KeepAlive = true;
                if (!string.IsNullOrEmpty(url.Referer))
                {
                    req.Referer = url.Referer;
                }
                req.UserAgent = ApiUrl.UserAgent;// "Mozilla/5.0 (Windows NT 10.0; WOW64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/53.0.2785.104 Safari/537.36 Core/1.53.2372.400 QQBrowser/9.5.10548.400";
                req.ProtocolVersion = HttpVersion.Version11;
                req.Headers.Add("Origin", url.Origin);
              var data=  string.Join("&", json.Select(x => string.Format("{0}={1}", x.Key, System.Web.HttpUtility.UrlEncode(x.Value.ToString()))));
                byte[] mybyte = System.Text.Encoding.UTF8.GetBytes(data);
                req.ContentLength = mybyte.Length;
               
                System.IO.Stream stream = req.GetRequestStream();
                stream.Write(mybyte, 0, mybyte.Length);


                HttpWebResponse res = req.GetResponse() as HttpWebResponse;

                stream.Close();

                System.IO.StreamReader SR = new System.IO.StreamReader(res.GetResponseStream(), System.Text.Encoding.GetEncoding(res.CharacterSet));
                dat = SR.ReadToEnd();
                res.Close();
                req.Abort();
            }
            catch (System.Web.HttpException e)
            {
                Logger.Error(e, url.Url);
                return "";
            }
            catch (WebException e)
            {
                Logger.Error(e, url.Url);
                return "";
            }
            if (!dat.Equals(""))
            { OrmManager.Insert(new SystemLogData { OriginData = dat.Replace(';', ' '), Message = "System" }); }
            return dat;
        }
        #endregion
        #region Event
        /// <summary>
        ///     当掉线时被引发。
        /// </summary>
        public event EventHandler ConnectionLost;
        /// <summary>
        /// 登录成功时触发，方便存盘。
        /// </summary>
        public event EventHandler<LoginSucessedEventArgs> Logined;
        /// <summary>
        /// 
        /// </summary>
        public event EventHandler<string> StateChanged;
        /// <summary>
        ///     接收到好友消息时被引发。
        /// </summary>
        public event EventHandler<MessageRecivedEventArgs> FriendMessageReceived;

        /// <summary>
        ///     接收到群消息时被引发。
        /// </summary>
        public event EventHandler<MessageRecivedEventArgs> GroupMessageReceived;

        /// <summary>
        ///     接收到讨论组消息时被引发。
        /// </summary>
        public event EventHandler<MessageRecivedEventArgs> DiscussionMessageReceived;

        /// <summary>
        ///     发出消息时被引发。适合用于在控制台打印发送消息的回显。
        /// </summary>
        public event EventHandler<MessageEchoEventArgs> MessageEcho;
        #endregion

        public IEnumerable<IAutoAnswer> GetAnswers()
        {
            return Answers.OrderByDescending(x => x.Priority);
        }
 #region 登录步骤   
        /// <summary>
        /// 开始登录SmartQQ
        /// </summary>
        public void LoginMain()
        {


            Login_QRStatuTimer.AutoReset = true;
            Login_QRStatuTimer.Elapsed += Login_QRStatuTimer_Elapsed;
            Login_QRStatuTimer.Interval = 1000;
            Login_QRStatuTimer.Start();


        }
        /// <summary>
        /// 每秒检查一次二维码状态
        /// </summary>
        private void Login_QRStatuTimer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            Login2VerifyQrCode();
        }
       
        /// <summary>
        /// 获取登陆二维码
        /// </summary>
        /// <returns></returns>
        public Image GetLoginQrImage(int tryNum=5)
        {
            Logger.Debug("开始获取二维码");
            try
            {
                int i = 1;
                do {
                    var response = GetStream(ApiUrl.GetQrCode);// RequestClient.
                    foreach (Cookie cookie in Cookies.GetAllCookies())
                    {
                        if (cookie.Name != "qrsig") continue;
                        Session.QRsig = cookie.Value;
                        break;
                    }


                    if (response != null)
                    {
                        Logger.Info("二维码已获取");
                        return Image.FromStream(response);
                    }
                    else
                    {
                        Logger.Debug("遇到网络故障，正在进行第{0}次重试",i);
                        System.Threading.Thread.Sleep(500);

                    }
                    i++;

                }
                while (tryNum>=i);

   
                return null;
            }
            catch (Exception) { return null; }

        }
        // 获取二维码
        private void GetQrCode(Action<byte[]> qrCodeDownloadedCallback)
        {
            Logger.Debug("开始获取二维码");
            int i = 5;
            do
            {
                var response = GetStream(ApiUrl.GetQrCode);// RequestClient.
                foreach (Cookie cookie in Cookies.GetAllCookies())
                {
                    if (cookie.Name != "qrsig") continue;
                    Session.QRsig = cookie.Value;
                    break;
                }


                if (response != null)
                {
                    Logger.Info("二维码已获取");
                    qrCodeDownloadedCallback.Invoke(response.ToBytes());
                    return;
                }
                else
                {
                    Logger.Debug("遇到网络故障，正在进行第{0}次重试", i);
                    System.Threading.Thread.Sleep(500);

                }
                i++;

            }
            while (i>0);
           
        }
        /// <summary>
        /// 登录第二步：检查二维码状态
        /// </summary>
        public void Login2VerifyQrCode()
        {
            Logger.Debug("等待扫描二维码");
            string dat;

            System.Threading.Thread.Sleep(1000);
            var response = Get(ApiUrl.VerifyQrCode, Session.QRsig.GetHash33());
            // var result = response.RawText;
            dat = response;//.RawText;
            if (!string.IsNullOrEmpty(dat))
            {
                string[] temp = dat.Split('\'');
                switch (temp[1])
                {
                    case ("65"):
                        //二维码失效                  
                        EventAggregator.GetEvent<LoginStatusChangedEvent>().Publish(new LoginEventArgs("65", "二维码失效，请稍后"));
                        break;
                    case ("66"):
                        //等待扫描
                        EventAggregator.GetEvent<LoginStatusChangedEvent>().Publish(new LoginEventArgs("66", "二维码有效，请扫描"));
                        break;
                    case ("67"):
                        //等待确认
                        EventAggregator.GetEvent<LoginStatusChangedEvent>().Publish(new LoginEventArgs("67", "二维码已扫描，请确认"));

                        break;
                    case ("0"):
                        //已经确认
                        var cookie = Cookies.GetAllCookies()["ptwebqq"];
                        if (cookie != null)
                        {
                            Session.Ptwebqq = cookie.Value;
                            // WorkContext.SetState(Constract.CurrentPtWebQQ, Session.Ptwebqq);
                        }

                        var skeycookie = Cookies.GetAllCookies()["skey"];
                        if (skeycookie != null)
                        {
                            Session.Skey = skeycookie.Value;

                        }
                        EventAggregator.GetEvent<LoginStatusChangedEvent>().Publish(new LoginEventArgs("0", "确认成功，请稍候"));

                        Login_Process(temp[5]);
                        break;

                    default:
                        break;
                }

            }
            else
            {
                EventAggregator.GetEvent<LoginStatusChangedEvent>().Publish(new LoginEventArgs("1000", "网络故障，请查看日志，正在重试"));
            }
        }
        public string VerifyQrCode()
        {
            Logger.Debug("等待扫描二维码");
            string dat;
            // 阻塞直到确认二维码认证成功
            while (true)
            {
                System.Threading.Thread.Sleep(1000);
                var response = Get(ApiUrl.VerifyQrCode, Session.QRsig.GetHash33());
                // var result = response.RawText;
                dat = response;//.RawText;
                if (!string.IsNullOrEmpty(dat))
                {
                    string[] temp = dat.Split('\'');
                    switch (temp[1])
                    {
                        case ("65"):
                            //二维码失效
                            EventAggregator.GetEvent<LoginStatusChangedEvent>().Publish(new LoginEventArgs("65", "二维码失效，请稍后"));
                            break;
                        case ("66"):
                            //等待扫描
                            EventAggregator.GetEvent<LoginStatusChangedEvent>().Publish(new LoginEventArgs("66", "二维码有效，请扫描"));
                            break;
                        case ("67"):
                            //等待确认 
                            EventAggregator.GetEvent<LoginStatusChangedEvent>().Publish(new LoginEventArgs("67", "二维码已扫描，请确认"));

                            break;
                        case ("0"):
                            //已经确认
                            var cookie = Cookies.GetAllCookies()["ptwebqq"];
                            if (cookie != null)
                            {
                                Session.Ptwebqq = cookie.Value;
                                //WorkContext.SetState(Constract.CurrentPtWebQQ, Session.Ptwebqq);
                            }
                            EventAggregator.GetEvent<LoginStatusChangedEvent>().Publish(new LoginEventArgs("0", "确认成功，请稍候"));
                            return temp[5];

                        default:
                            break;
                    }
                }
                else
                {
                    EventAggregator.GetEvent<LoginStatusChangedEvent>().Publish(new LoginEventArgs("1000", "网络故障，请查看日志，正在重试"));
                }
            }

        }
        private object _lock = new object { };
        /// <summary>
        /// 处理扫描二维码并确认后的登录流程
        /// </summary>
        /// <param name="url">获取ptwebqq的跳转地址</param>
        /// 
        public void Login_Process(string url)
        {
            Login_QRStatuTimer.Stop();
            if (System.Threading.Monitor.TryEnter(_lock))
            {
                System.Threading.Monitor.Enter(_lock);
                try
                {
                    Logger.Debug("初始化开始");
                    Login3GetPtwebqq(url);
                    Login4GetVfwebqq();
                    Login5GetPsessionid();

                    Logger.Debug("初始化完成");
                    EventAggregator.GetEvent<InitComplatedEvent>().Publish(new InitComplatedEventArgs("0", "初始化完成"));
                    if (!TryLogin())
                    {
                        Session.Hash = Session.Uin.GetHashByQQNum( Session.Ptwebqq);
                        Logger.Debug("登录失败");
                    }
                    else
                    {
                        Status = ClientStatus.Active;
                        StartMessageLoop();
                        Logger.Debug("登录完成");
                        InitSelfInfo();
                        Login6GetOnlineAndRecentFake();
                       /* foreach (Cookie cookie in Cookies.GetAllCookies())
                        {
                            Cookies.Add(new Cookie(cookie.Name, cookie.Value, "/", "qun.qq.com"));
                            Cookies.Add(new Cookie(cookie.Name, cookie.Value, "/", "w.qq.com"));
                            Cookies.Add(new Cookie(cookie.Name, cookie.Value, "/", "id.qq.com"));
                            Cookies.Add(new Cookie(cookie.Name, cookie.Value, "/", "mail.qq.com"));
                        }*/
                        Logined?.Invoke(this, new LoginSucessedEventArgs(Session, SaveCookies(),true));
                        InitFriendList();
                        InitGroupList();
                        InitDisscussList();

                    }


                }
                catch (Exception e)
                {
                    Logger.Error(e, "初始化出错");
                    EventAggregator.GetEvent<InitComplatedEvent>().Publish(new InitComplatedEventArgs("-1", "初始化出错" + e.Message));

                }

                System.Threading.Monitor.Exit(_lock);
            }
        }
        /// <summary>
        /// 登录第三步：获取ptwebqq值
        /// </summary>
        /// <param name="url">获取ptwebqq的跳转地址</param>
        public void Login3GetPtwebqq(string url)
        {
            Logger.Debug("开始获取ptwebqq");
           Get(ApiUrl.GetPtwebqq, url);
          //  Logger.Debug(dat);
        }

        /// <summary>
        /// 登录第四步：获取vfwebqq的值
        /// </summary>
        public void Login4GetVfwebqq()
        {
            Logger.Debug("开始获取vfwebqq");

            var response = Get(ApiUrl.GetVfwebqq, Session.Ptwebqq);
            if (!string.IsNullOrEmpty(response))
            {
                Session.Vfwebqq = response.Split('\"')[7];
                return;
            }
            else
            {
                EventAggregator.GetEvent<LoginStatusChangedEvent>().Publish(new LoginEventArgs("1000", "网络故障，请查看日志，正在重试"));
                System.Threading.Thread.Sleep(1000);
                Login4GetVfwebqq();
            }
            //WorkContext.SetState(Constract.CurrentVfWebQQ, Session.Vfwebqq);

        }
        /// <summary>
        /// 登录第五步：获取pessionid
        /// </summary>
        public void Login5GetPsessionid()
        {
            Logger.Debug("开始获取uin和psessionid");

            var r = new JObject
            {
                {"ptwebqq", Session.Ptwebqq},
                {"clientid", Session.ClientId},
                {"psessionid", ""},
                {"status", "online"}
            };

            var dat = Post(ApiUrl.GetUinAndPsessionid, r);
            if (!string.IsNullOrEmpty(dat))
            {
                //  var dat = response.RawText;
                Session.Psessionid = dat.Replace(":", ",").Replace("{", "").Replace("}", "").Replace("\"", "").Split(',')[10];

                Session.Uin = long.Parse(dat.Replace(":", ",").Replace("{", "").Replace("}", "").Replace("\"", "").Split(',')[14]);
                Session.QQNum = Session.Uin;
                Session.Hash = Session.QQNum.GetHashByQQNum(Session.Ptwebqq);

                var p_skeycookie = Cookies.GetAllCookies()["p_skey"];
                if (p_skeycookie != null)
                {
                    Session.PSkey = p_skeycookie.Value;
                    Cookies.Add(new Cookie("p_skey", p_skeycookie.Value, "/", "qun.qq.com"));
                    Cookies.Add(new Cookie("p_skey", p_skeycookie.Value, "/", "w.qq.com"));
                    Cookies.Add(new Cookie("p_skey", p_skeycookie.Value, "/", "id.qq.com"));
                     Cookies.Add(new Cookie("p_skey", p_skeycookie.Value, "/", "mail.qq.com"));

                }
                var p_uincookie = Cookies.GetAllCookies()["p_uin"];
                if (p_uincookie != null)
                {
                    Session.PUin = p_uincookie.Value;
                    Cookies.Add(new Cookie("p_uin", p_uincookie.Value, "/", "qun.qq.com"));
                    Cookies.Add(new Cookie("p_uin", p_uincookie.Value, "/", "w.qq.com"));
                     Cookies.Add(new Cookie("p_uin", p_uincookie.Value, "/", "id.qq.com"));
                    Cookies.Add(new Cookie("p_uin", p_uincookie.Value, "/", "mail.qq.com"));
                }
            }
            else
            {
                EventAggregator.GetEvent<LoginStatusChangedEvent>().Publish(new LoginEventArgs("1000", "网络故障，请查看日志，正在重试"));
                System.Threading.Thread.Sleep(1000);
                Login5GetPsessionid();
            }


        }
        /// <summary>
        /// 登录第六步：获取在线成员、近期联系人（仅提交请求，未处理）
        /// </summary>
        public void Login6GetOnlineAndRecentFake()
        {
            Get(ApiUrl.GetFriendStatus, Session.Vfwebqq, Session.Psessionid, GetTimeStamp());
            Logger.Debug("开始获取最近聊天记录列表");
            Post(ApiUrl.GetChatHistoryList,
               new JObject { { "vfwebqq", Session.Vfwebqq }, { "clientid", Session.ClientId }, { "psessionid", "" } });
        }

        #endregion

        #region 消息应答
        /// <summary>
        /// 处理收到的消息
        /// </summary>
        /// <param name="data">收到的消息（JSON）</param>
        public void MessageProcessor(string data)
        {

            PollMessage poll = (PollMessage)JsonConvert.DeserializeObject(data, typeof(PollMessage));

            if (poll.RetCode != 0)
            {
                OrmManager.Insert(new SystemLogData { OriginData = data, Message = poll.ErrMsg, ReCode = poll.RetCode });
                ErrorMessageProcessor(poll, data);
            }
            else if (poll.Result != null && poll.Result.Count > 0)
            {
                OrmManager.Insert(new SystemLogData { OriginData = data, Message = "新消息", ReCode = poll.RetCode });
                for (int i = 0; i < poll.Result.Count; i++)
                {
                    switch (poll.Result[i].PollType)
                    {
                        case "kick_message":// 被踢下线
                            OrmManager.Insert(new MessageLogData { OriginData = data, Message = "kick_message" + poll.Result[i].Value.Reason, ReCode = poll.RetCode });
                            EventAggregator.GetEvent<KickedEvent>().Publish(new KickedEventArgs(poll.Result[i].Value.SendUin, poll.Result[i].Value.Reason));
                            break;
                        case "message":// 好友消息
                            FriendMessageReceived?.Invoke(this,new MessageRecivedEventArgs(poll.Result[i].Value, data));
                           // PrivateMessageProcessor(poll.Result[i].Value, data);
                            break;
                        case "group_message":// 群消息
                            GroupMessageReceived?.Invoke(this, new MessageRecivedEventArgs(poll.Result[i].Value, data));
                            //GroupMessageProcessor(poll.Result[i].Value, data);
                            break;
                        case "discu_message":// 讨论组消息
                            DiscussionMessageReceived?.Invoke(this, new MessageRecivedEventArgs(poll.Result[i].Value, data));
                           // DisscussMessageProcessor(poll.Result[i].Value, data);
                            break;
                        case "input_notify":// 正在输入
                        case "sess_message"://临时会话消息
                        case "shake_message"://窗口震动
                        case "buddies_status_change"://群成员状态变动
                        case "system_message"://系统消息，好友添加
                        case "group_web_message":// 发布了共享文件
                        case "sys_g_msg"://被踢出了群
                        default:
                            Logger.Warn("意外的消息类型：" + poll.Result[i].PollType + "：" + data);
                            EventAggregator.GetEvent<UnHandledMessageTypeEvent>().Publish(new UnHandledMessageTypeEventArgs(poll.Result[i].PollType, poll.Result[i].Value.SendUin.ToString()));

                            break;
                    }
                }
            }
            else
            {
                Logger.Debug("无新消息{0}",data);
               // OrmManager.Insert(new SystemLogData { OriginData = data, Message = "无新消息", ReCode = poll.RetCode });
            }
        }
        /// <summary>
        /// 处理poll包中的消息数组
        /// </summary>
        /// <param name="content">消息数组</param>
        /// <returns></returns>
        public string PollToText(List<object> content)
        {
            string message = "";
            for (int i = 1; i < content.Count; i++)
            {
                if (content[i].ToString().Contains("[\"cface\","))
                    continue;
                else if (content[i].ToString().Contains("\"face\","))
                    message += ("{..[face" + content[i].ToString().Replace("\"face\",", "").Replace("]", "").Replace("[", "").Replace(" ", "").Replace("\r", "").Replace("\n", "") + "]..}");
                else
                    message += content[i].ToString();
            }
            message = message.Replace("\\\\n", Environment.NewLine).Replace("＆", "&");
            return message;
        }

        /// <summary>
        /// 收到私聊和讨论组消息时调用的回复函数
        /// </summary>
        /// <param name="uin"></param>
        /// <param name="message"></param>
        /// <param name="type"></param>
        internal void AnswerMessage(AnswerContext context)
        {
            foreach (var answer in GetAnswers().Where(x => x.MessageType.Contains(context.MessageType) && x.CanAnswer(context)))
            {
                if (context.CanAnswer)
                {
                    try
                    {
                        answer.Answer(context);
                    }
                    catch (Exception e)
                    {
                        Logger.Error(e, "回复过程中遇到错误");
                        continue;
                    }
                }
            }
            var alerts = context.Alerts.Where(x => !string.IsNullOrEmpty(x));

            if (alerts.Count() > 0)
            {
                if (alerts.Count() > 1)
                    context.AnswerBySingle = true;
                if (context.AnswerBySingle)
                {
                    foreach (var alert in alerts)
                    {
                        try
                        {
                            Message((TargetType)context.Type, context.SendToId, alert);//"[QQ:at=" + context.FromUin + "]" +
                        }
                        catch(Exception e)
                        {
                            Logger.Error(e);
                            continue;
                        }
                        }
                    
                }
                else
                {
                    Message((TargetType)context.Type, context.SendToId, string.Join(Environment.NewLine, alerts));
                }
                return;
            }
            var MessageToSendArray = context.Answers.Where(x => !string.IsNullOrEmpty(x));
            if (MessageToSendArray.Count() > 1)
                context.AnswerBySingle = true;
            if (context.AnswerBySingle)
            {
                foreach (var message in MessageToSendArray)
                {
                    try
                    {
                        if (message.Equals("None3"))
                        {
                            Message((TargetType)context.Type, context.SendToId, "这句话仍在等待审核哟～～如果要大量添加语库，可以向管理员申请白名单的～");
                        }
                        else
                        {
                            Message((TargetType)context.Type, context.SendToId, message);
                        }
                    }
                    catch (Exception e)
                    {
                        Logger.Error(e);
                        continue;
                    }
                }
            }
            else
            {
                string MessageToSend = "";
                foreach (var message in MessageToSendArray)
                {
                    if (message.Equals("None3"))
                    {
                        MessageToSend += "这句话仍在等待审核哟～～如果要大量添加语库，可以向管理员申请白名单的～";
                    }
                    else
                    {
                        MessageToSend += message;
                    }
                    MessageToSend += Environment.NewLine;
                }

                if (!MessageToSend.Equals(""))
                {
                    Message((TargetType)context.Type, context.SendToId, MessageToSend);
                }
                else if (context.Type == 0)
                {
                    string SenderName = "";
                    string Gender = "";
                    var friendInfo = OrmManager.Get<FriendInfo>(x => x.Uin == context.FromUin && x.QQNum == context.CurrentQQ);
                    if (friendInfo == null)
                    {
                        InitFriendList();
                        friendInfo = OrmManager.Get<FriendInfo>(x => x.Uin == context.FromUin && x.QQNum == context.CurrentQQ);
                        SenderName = friendInfo.Nick;
                        Gender = friendInfo.Gender;
                    }
                    else
                    {
                        SenderName = friendInfo.Nick;// SmartQQ.FriendList[uin].Nick;
                        Gender = friendInfo.Gender; //SmartQQ.FriendList[uin].Gender;
                    }
                    if (friendInfo == null)
                    {
                        Message((TargetType)context.Type, context.SendToId, "～ 我听不懂你在说什么呢。。。教教我吧～～" + Environment.NewLine + "格式 学习^主人的话^我的回复");

                    }
                    if (Gender == "female")
                        Gender = "姐姐 ";
                    else if (Gender == "male")
                        Gender = "哥哥 ";
                    Message((TargetType)context.Type, context.SendToId, SenderName + Gender + "～ 我听不懂你在说什么呢。。。教教我吧～～" + Environment.NewLine + "格式 学习^主人的话^我的回复");
                }
            }
        }
        #endregion

        #region 消息初处理
        /// <summary>
        /// 私聊消息处理
        /// </summary>
        /// <param name="value">poll包中的value</param>
        public void PrivateMessageProcessor(object sender, MessageRecivedEventArgs value)//PollValue value, string originData = ""
        {
            string message = PollToText(value.Poll.Content);
            EventAggregator.GetEvent<CommonMessageReciveEvent>().Publish(new MessageReciveEventArgs(value.Poll.FromUin, message));
            OrmManager.Insert(new MessageLogData { OriginData =value.OriginData, Message = message, ReCode = 0, FromUin = value.Poll.FromUin, OwnerUin = Session.QQNum });
            string nick = "未知";
            var friendInfo = OrmManager.Get<FriendInfo>(x => x.Uin == value.Poll.FromUin && x.QQNum == Session.QQNum);
            if (friendInfo == null)
            {
                InitFriendList();
                friendInfo = OrmManager.Get<FriendInfo>(x => x.Uin == value.Poll.FromUin && x.QQNum == Session.QQNum);
                if (friendInfo != null)
                {
                    nick = friendInfo.Nick;
                }
            }
            else
            {
                nick = friendInfo.Nick;
            }
            var context = new AnswerContext();
            context.CurrentQQ = Session.QQNum;
            context.MessageType = "message";
            context.Message = message;
            context.FromUin = value.Poll.FromUin;
            //  context.FromRealQQ = InitRealQQ(value.FromUin);
            context.FromNick = nick;
            context.IsManager = friendInfo.Uin == SelfInfo.Uin;
            context.SmartService = this;
            context.SendToId = value.Poll.FromUin;
            context.State = DefaultState;//WorkContext.GetState<QQState>(Constract.CurrentQQState).State;
            context.Type = 0;
            AnswerMessage(context);
            if (context.IsStateChanged)
            {
                StateChanged?.Invoke(this,context.State);
            }

        }
        /// <summary>
        /// 群聊消息处理
        /// </summary>
        /// <param name="value">poll包中的value</param>
        public void GroupMessageProcessor(object sender, MessageRecivedEventArgs value)
        {
            string message = PollToText(value.Poll.Content);
            OrmManager.Insert(new MessageLogData { OriginData = value.OriginData, Message = message, ReCode = 0, FromUin = value.Poll.SendUin, OwnerUin = value.Poll.FromUin });
            EventAggregator.GetEvent<GroupMessageReciveEvent>().Publish(new MessageReciveEventArgs(value.Poll.FromUin, message));
            long gid = value.Poll.FromUin;
            string nick = "未知";
            var groupInfo = OrmManager.Get<GroupInfo>(x => x.GroupId == gid && x.QQNum == Session.QQNum);
            if (groupInfo == null)
            {
                InitGroupList();
                groupInfo = OrmManager.Get<GroupInfo>(x => x.GroupId == gid && x.QQNum == Session.QQNum);
            }
            var memberInfo = OrmManager.Get<MemberInfo>(x => x.GroupId == gid && x.QQNum == Session.QQNum && x.Uin == value.Poll.SendUin);
            if (memberInfo != null)
            {
                nick = memberInfo.Nick;
            }
            else
            {
                InitGroupInfo(gid);
                memberInfo = OrmManager.Get<MemberInfo>(x => x.GroupId == gid && x.QQNum == Session.QQNum && x.Uin == value.Poll.SendUin);
                if (memberInfo != null)
                {
                    nick = memberInfo.Nick;
                }
            }
            if (memberInfo == null)
                return;
            // var realQQ = InitRealQQ(value.SendUin);
            //  if (realQQ.Equals("1000000"))
            //     nick = "系统消息";
            // Program.MainForm.AddAndReNewTextBoxGroupChat(value.from_uin, (GroupList[gid].name + "   " + nick + "  " + Info_RealQQ(value.send_uin) + Environment.NewLine + message), false);
            //RuiRui.AnswerGroupMessage(gid, message, value.SendUin, gno);
            //  else
            //  {
            var context = new AnswerContext();
            context.CurrentQQ = Session.QQNum;
            context.MessageType = "group_message";
            context.Message = message;
            context.FromUin = value.Poll.SendUin;
            //context.FromRealQQ = realQQ;
            context.FromNick = nick;
            // context.FromGroupId = gid;
            context.SendToId = gid;
            context.IsManager = memberInfo.IsManager || memberInfo.Uin == groupInfo.Owner;
            context.SmartService = this;
            context.State = groupInfo.State=="{}"?DefaultState:groupInfo.State;
            context.Type = 1;
            AnswerMessage(context);
            if (context.IsStateChanged)
            {
                if (groupInfo.State == "{}")
                { }
                else
                {
                    StateChanged?.Invoke(this,context.State);
                }
            }
            // }
        }

        /// <summary>
        /// 讨论组消息处理
        /// </summary>
        /// <param name="value">poll包中的value</param>
        public void DisscussMessageProcessor(object sender, MessageRecivedEventArgs value)
        {
            string message = PollToText(value.Poll.Content);
            OrmManager.Insert(new MessageLogData { OriginData =value.OriginData, Message = message, ReCode = 0, FromUin = value.Poll.SendUin, OwnerUin = value.Poll.Did });
            EventAggregator.GetEvent<DiscussionMessageReciveEvent>().Publish(new MessageReciveEventArgs(value.Poll.Did, message));
            string DName = "讨论组";
            string SenderNick = "未知";
            var disscussInfo = OrmManager.Get<DiscussInfo>(x => x.Did == value.Poll.Did && x.QQNum == Session.QQNum);
            var disscussMemberInfo = OrmManager.Get<DiscussMemberInfo>(x => x.Did == value.Poll.Did && x.QQNum == Session.QQNum && x.Uin == value.Poll.SendUin);

            if (disscussInfo == null)
            {
                InitDisscussList();
                disscussInfo = OrmManager.Get<DiscussInfo>(x => x.Did == value.Poll.Did && x.QQNum == Session.QQNum);
            }
            else
            {
                DName += disscussInfo.Name;
                if (disscussMemberInfo != null)
                    SenderNick = disscussMemberInfo.Nick;
                else
                {
                    InitDisscussInfo(value.Poll.Did);
                    disscussMemberInfo = OrmManager.Get<DiscussMemberInfo>(x => x.Did == value.Poll.Did && x.QQNum == Session.QQNum && x.Uin == value.Poll.SendUin);
                    SenderNick = disscussMemberInfo.Nick;
                }
            }
            if (disscussInfo == null)
            { DName = "未知讨论组"; }
            //  var realNum = InitRealQQ(value.SendUin);
            // if (realNum.Equals("1000000"))
            //    SenderNick = "系统消息";
            // Program.MainForm.AddAndReNewTextBoxDiscussChat(value.from_uin, (DName + "   " + SenderNick + "  " + Info_RealQQ(value.send_uin) + Environment.NewLine + message), false);
            //RuiRui.AnswerMessage(value.Did, message, 2);
            //  else
            // {
            var context = new AnswerContext();
            context.CurrentQQ = Session.QQNum;
            context.MessageType = "discu_message";
            context.Message = message;
            context.FromUin = value.Poll.SendUin;
            // context.FromDId = value.Did;
            context.FromNick = SenderNick;
            // context.FromRealQQ = realNum;
            context.IsManager = false;
            context.SmartService = this;
            context.State = disscussInfo.State=="{}"?DefaultState:disscussInfo.State;
            context.SendToId = value.Poll.Did;
            context.Type = 2;
            AnswerMessage(context);
            if (context.IsStateChanged)
            {
                if (disscussInfo.State == "{}")
                { }
                else
                {
                    StateChanged?.Invoke(this,context.State);
                }

            }
            // }
        }
        /// <summary>
        /// 错误信息处理
        /// </summary>
        /// <param name="poll">poll包</param>
       // public  int Count103 = 0;
        public void ErrorMessageProcessor(PollMessage poll, string originData)
        {
            EventAggregator.GetEvent<ErrorEvent>().Publish(new ErrorEventArgs(poll.RetCode));

            if (poll.RetCode == 102)
            {
                Logger.Debug("号被冻结：{0}-{1}-{2}", poll.RetCode, poll.ErrMsg , originData);
  return;
            }
            else if (poll.RetCode == 103)
            {
                Logger.Debug("正常连接没有消息：{0}-{1}-{2}", poll.RetCode, poll.ErrMsg, originData);
 
                if (!TryLogin())
                {
                    Session.Hash = Session.QQNum.GetHashByQQNum( Session.Ptwebqq);

                    Logined?.Invoke(this, new LoginSucessedEventArgs(Session, SaveCookies()));
                }
                else
                {
                    Status = ClientStatus.Active;
                }
                return;
            }
            else if (poll.RetCode == 116)
            {
                Logger.Debug("未知错误：{0}-{1}-{2}", poll.RetCode, poll.ErrMsg, originData);
                Session.Ptwebqq = poll.P;
                Logined?.Invoke(this, new LoginSucessedEventArgs(Session, SaveCookies()));
                return;
            }
            else if (poll.RetCode == 122 || poll.RetCode == 108 || poll.RetCode == 114 || poll.RetCode == 100001)
            {
                Logger.Debug("掉线,正尝试连接：{0}-{1}-{2}", poll.RetCode, poll.ErrMsg, originData);
if (!TryLogin())
                {
                    Session.Hash = Session.QQNum.GetHashByQQNum(Session.Ptwebqq);
                    Logined?.Invoke(this, new LoginSucessedEventArgs(Session, SaveCookies()));
                }
                else
                {
                    Status = ClientStatus.Active;
                }

                return;
            }
            else if (poll.RetCode == 120 || poll.RetCode == 121)
            {
                Logger.Debug("被T：{0}-{1}-{2}", poll.RetCode, poll.ErrMsg, originData);
  InitGroupList();
                return;
            }

            else if (poll.RetCode == 100006 || poll.RetCode == 100003)
            {
                Logger.Debug("群编号有问题：{0}-{1}-{2}", poll.RetCode, poll.ErrMsg, originData);
 InitGroupList(); //_pollStarted = false;
                return;
            }
            else if (poll.RetCode == 100000)
            {
                Logger.Debug("已经正常登录：{0}-{1}-{2}", poll.RetCode, poll.ErrMsg, originData);

                return;
            }
            else if (poll.RetCode == 100012)
            {
                Logger.Debug("修改密码 或登录被限制了：{0}-{1}-{2}", poll.RetCode, poll.ErrMsg, originData);
 _pollStarted = false;
                return;
            }
            Logger.Debug("未分析错误：{0}-{1}-{2}", poll.RetCode, poll.ErrMsg, originData);
        }
        #endregion
        #region 消息发送 登录
        /// <summary>
        ///     发送消息。
        /// </summary>
        /// <param name="type">目标类型。</param>
        /// <param name="id">用于发送的ID。</param>
        /// <param name="content">消息内容。</param>
        public void Message(TargetType type, long id, string content)
        {

            if (Status != ClientStatus.Active)
                throw new InvalidOperationException("尚未登录，无法进行该操作");
            Logger.Debug("开始发送消息，对象类型：" + type);

            string paramName;
            ApiUrl url;

            switch (type)
            {
                case TargetType.Friend:
                    paramName = "to";
                    url = ApiUrl.SendMessageToFriend;
                    break;
                case TargetType.Group:
                    paramName = "group_uin";
                    url = ApiUrl.SendMessageToGroup;
                    break;
                case TargetType.Discussion:
                    paramName = "did";
                    url = ApiUrl.SendMessageToDiscussion;
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(type), type, null);
            }

            var response = PostWithRetry(url, new JObject
            {
                {paramName, id},
                {
                    "content",
                    new JArray
                        {
                            content.TranslateEmoticons(),
                            new JArray {"font", JObject.FromObject(Easy.QQRob.Models.Font.DefaultFont)}
                        }
                        .ToString(Formatting.None)
                },
                {"face", 573},
                {"clientid", Session.ClientId},
                {"msg_id", Session.MessageId++},
                {"psessionid", Session.Psessionid}
            }, RetryTimes);

            var dat = response;
            Logger.Info("消息发送返回信息：" + dat);
            if (!string.IsNullOrEmpty(dat))
            {
                Data.SendResult sendResult = (Data.SendResult)JsonConvert.DeserializeObject(dat, typeof(QQRob.Data.SendResult));

                if (sendResult != null && (sendResult.ErrCode == 0 || sendResult.ErrCode == 100100))
                {
                    Logger.Info("消息发送成功");
                    if (MessageEcho == null) return;
                    MessageEchoEventArgs args;
                    switch (type)
                    {
                        case TargetType.Friend:
                            {
                                args = new MessageEchoEventArgs(null, content);
                                break;
                            }
                        case TargetType.Group:
                            {
                                args = new MessageEchoEventArgs(null, content);
                                break;
                            }
                        case TargetType.Discussion:
                            {
                                args = new MessageEchoEventArgs(null, content);
                                break;
                            }
                        default:
                            throw new ArgumentOutOfRangeException(nameof(type), type, null);
                    }
                    MessageEcho(this, args);
                    var rand = new Random();
                    System.Threading.Thread.Sleep(1000 * rand.Next(0, 3));
                }
                else
                {
                    Logger.Error("消息发送失败:" + dat);
                }
            }
            else
            {
                Logger.Error("消息发送失败");
            }

        }

        /// <summary>
        ///     导出当前cookie集合。
        /// </summary>
        /// <returns>当前cookie集合的JSON字符串。</returns>
        public string SaveCookies()
        {
            if (Status != ClientStatus.Active)
                throw new InvalidOperationException("仅在登录后才能导出cookie");

            return new JObject
            {{Constract.QQSession, JToken.FromObject( Session) },
                {"hash", Session.Hash},
                {"psessionid", Session.Psessionid},
                {"ptwebqq", Session.Ptwebqq},
                {"uin", Session.Uin},
                {"vfwebqq", Session.Vfwebqq},
                 {"skey", Session.Skey},
                {"p_skey", Session.PSkey},
                {
                    "cookies",
                    JArray.FromObject(Cookies.GetAllCookies())
                }
            }.ToString(Formatting.None);
        }

        /// <summary>
        ///     使用cookie连接到SmartQQ。
        /// </summary>
        /// <param name="json">由SaveCookies()导出的JSON字符串。</param>
        public LoginResult Start(string json)
        {
            if (Status != ClientStatus.Idle)
                throw new InvalidOperationException("已在登录或者已经登录，不能重复进行登录操作");
            try
            {
                Logger.Debug("开始通过cookie登录");
                Status = ClientStatus.LoggingIn;
                var dump = JObject.Parse(json);

                if (dump[Constract.QQSession] == null)
                {
                    Session = new QQSession();
                    Session.Hash = dump["hash"].Value<string>();
                    Session.Psessionid = dump["psessionid"].Value<string>();
                    Session.Ptwebqq = dump["ptwebqq"].Value<string>();
                    Session.Uin = dump["uin"].Value<long>();
                    Session.Skey = dump["skey"].Value<string>();
                    Session.PSkey = dump["p_skey"].Value<string>();
                    Session.QQNum = Session.Uin;
                    Session.Vfwebqq = dump["vfwebqq"].Value<string>();
                }
                else
                {
                    Session = dump[Constract.QQSession].ToObject<QQSession>();
                }

                foreach (var cookie in dump["cookies"].Value<JArray>().ToObject<List<Cookie>>())
                    Cookies.Add(cookie);

                if (TryLogin())
                {
                    Status = ClientStatus.Active;
                    StartMessageLoop();
                    Logined?.Invoke(this, new LoginSucessedEventArgs(Session, SaveCookies(),true));
                    return LoginResult.Succeeded;
                }
                Status = ClientStatus.Idle;
                Logger.Error( "登录失败，Cookie过期");
                return LoginResult.CookieExpired;
            }
            catch (Exception ex)
            {
                Status = ClientStatus.Idle;
                Logger.Error(ex, "登录失败，抛出异常：");
                return LoginResult.Failed;
            }
        }

        /// <summary>
        ///     连接到SmartQQ。
        /// </summary>
        /// <param name="qrCodeDownloadedCallback">二维码已下载时的回调函数。回调函数的参数为二维码图像的字节数组。</param>
        public LoginResult Start(Action<byte[]> qrCodeDownloadedCallback)
        {
            if (Status != ClientStatus.Idle)
                throw new InvalidOperationException("已在登录或者已经登录，不能重复进行登录操作");
            var result = Login(qrCodeDownloadedCallback);
            if (result != LoginResult.Succeeded)
            {
                Status = ClientStatus.Idle;
                return result;
            }
            Status = ClientStatus.Active;
            StartMessageLoop();
            Logined?.Invoke(this, new LoginSucessedEventArgs(Session, SaveCookies(),true));
            return result;
        }
        // 登录
        private LoginResult Login(Action<byte[]> qrCodeDownloadedCallback)
        {
            try
            {
                Status = ClientStatus.LoggingIn;
                GetQrCode(qrCodeDownloadedCallback);
                var url = VerifyQrCode();
                Login3GetPtwebqq(url);
                Login4GetVfwebqq();
                Login5GetPsessionid();

                if (!TryLogin())
                {
                    Session.Hash = Session.Uin.GetHashByQQNum( Session.Ptwebqq);
                }
                return LoginResult.Succeeded;
            }
            catch (TimeoutException)
            {
                return LoginResult.QrCodeExpired;
            }
            catch (Exception ex)
            {
                Logger.Error("登录失败，抛出异常：" + ex);
                return LoginResult.Failed;
            }
        }



        // 解决103错误码
        private bool TryLogin()
        {
            Logger.Debug("开始向服务器发送测试连接请求");

            var result = Get(ApiUrl.TestLogin, Session.Vfwebqq, Session.ClientId, Session.Psessionid, GetTimeStamp());
            Logger.Debug("请求结果：{0}",result);
            return !string.IsNullOrEmpty(result) &&
                   JObject.Parse(result)["retcode"].Value<int?>() == 0;
        }
        public bool TryLogin(QQState state)
        {

            var cookie = state.GetCookies<string>(Constract.LoginCookies);
            if (cookie != null)
            {
                return Start(cookie) == LoginResult.Succeeded;
            }

            return false;

        }


        // 开始消息轮询
        private void StartMessageLoop()
        {
            _pollStarted = true;
            new System.Threading.Thread(() =>
            {
                while (true)
                {
                    if (!_pollStarted) return;
                    if (
                        FriendMessageReceived == null && GroupMessageReceived == null &&
                        DiscussionMessageReceived == null) continue;
                    try
                    {
                        PollMessage();
                    }
                    catch (Exception ex)
                    {
                      Logger.Error(ex, $"{Session.QQNum}接收消息线程出现错误：");
                        // 自动掉线
                        if (TryLogin())
                            continue;
                        else
                        {
                            Logger.Error(ex, $"{Session.QQNum}错误无法解决，系统已经选择退出：");
                            Close();
                            ConnectionLost?.Invoke(this, EventArgs.Empty);
                        }
                    }
                }
            })
            { IsBackground = true }.Start();
        }

        // 拉取消息
        private void PollMessage()
        {
            Logger.Debug(DateTime.Now.ToLongTimeString() + " 开始接收消息");

            var r = new JObject
            {
                {"ptwebqq", Session.Ptwebqq},
                {"clientid", Session.ClientId},
                {"psessionid", Session.Psessionid},
                {"key", ""}
            };

            var data = Post(ApiUrl.PollMessage, r, 120000);
            if (!string.IsNullOrEmpty(data))
            {
                MessageProcessor(data);
            }
        }

        /// <summary>
        ///     停止通讯。
        /// </summary>
        public void Close()
        {
            if (Status == ClientStatus.Idle)
                throw new InvalidOperationException("尚未登录，无法进行该操作");
            _pollStarted = false;

            var state = OrmManager.Get<QQState>(x => x.QQNum == Session.QQNum);
            if (state != null)
            {
                state.Logined = false;
                OrmManager.Update<QQState>(state);
            }
            OrmManager.Clear<FriendInfo>();
            OrmManager.Clear<GroupInfo>();

            OrmManager.Clear<DiscussInfo>();
            OrmManager.Clear<MemberInfo>();

            OrmManager.Clear<DiscussMemberInfo>();
        }

        #endregion
        #region 好友信息
        /// <summary>
        /// 获取好友列表并保存
        /// </summary>
        public void InitFriendList()
        {
          // InitFriendList2();
            Logger.Debug("开始拉取好友列表");
            var dat = Post(ApiUrl.GetFriendList,
                new JObject { { "vfwebqq", Session.Vfwebqq }, { "hash", Session.Hash } });
            Friend friend = (Friend)JsonConvert.DeserializeObject(dat, typeof(Friend));
            if (friend.RetCode == 0)
            {
                for (int i = 0; i < friend.Result.BasicInfos.Count; i++)
                {
                    Logger.Debug("开始拉取好友基本资料：" + friend.Result.BasicInfos[i].Uin);
                   // var basicinfo = friend.Result.BasicInfos[i];
                   // var alias = Categories + "：" + MarkName ?? basicinfo.Nick;
                    var friendInfo = OrmManager.Get<FriendInfo>(x => x.Uin == friend.Result.BasicInfos[i].Uin && x.QQNum == Session.QQNum);
                    if (friendInfo == null)
                    {
                        friendInfo = new FriendInfo();
                        friendInfo.Face = friend.Result.BasicInfos[i].Face;
                        friendInfo.Nick = friend.Result.BasicInfos[i].Nick;
                        friendInfo.Uin = friend.Result.BasicInfos[i].Uin;
                        friendInfo.QQNum = Session.QQNum;
                        OrmManager.Insert(friendInfo);
                        friendInfo = OrmManager.Get<FriendInfo>(x => x.Uin == friend.Result.BasicInfos[i].Uin && x.QQNum == Session.QQNum);
                    }
                    else
                    {
                        friendInfo.Face = friend.Result.BasicInfos[i].Face;
                        friendInfo.Nick = friend.Result.BasicInfos[i].Nick;
                        friendInfo.Uin = friend.Result.BasicInfos[i].Uin;
                        friendInfo.QQNum = Session.QQNum;
                        OrmManager.Update(friendInfo);
                    }
                    var needUpdate = false;
                    if (friend.Result.MarkNames != null)
                    {
                        needUpdate = true;
                        var card = friend.Result.MarkNames.FirstOrDefault(x => x.Uin == friendInfo.Uin);
                        if (card != null)
                        { friendInfo.MarkName = card.MarkName; }
                    }
                    if (friend.Result.MyCategories != null&&friend.Result.Friends!=null)
                    {
                        needUpdate = true;
                        var friendC = friend.Result.Friends.FirstOrDefault(x => x.Uin == friendInfo.Uin);
                        var card = friend.Result.MyCategories.FirstOrDefault(x => x.Index == friendC.Categories);
                        if (card != null)
                        { friendInfo.Categories = card.Name; }
                        else
                        {
                            if (friendC.Categories == 0)
                            {
                                friendInfo.Categories = "我的好友";
                            }
                        }
                    }
                    if (friend.Result.VipInfo != null)
                    {
                        needUpdate = true;
                        var vip = friend.Result.VipInfo.FirstOrDefault(x => x.U == friendInfo.Uin);
                        if (vip != null)
                        {
                            friendInfo.VipInfo = vip.VipLevel;
                           
                        }
                    }
                    if (needUpdate)
                    {
                        OrmManager.Update(friendInfo);
                    }
                    //InitFriendInfo(friendInfo.Uin);
                }
                Logger.Debug("获取好友完成");
            }
            else
            {
                Logger.Debug("初始化朋友信息出错");
                EventAggregator.GetEvent<ErrorEvent>().Publish(new ErrorEventArgs(friend.RetCode, "初始化朋友信息出错"));
            }
        }
        /// <summary>
        /// 获取好友的详细信息
        /// </summary>
        /// <param name="uin"></param>
        public void InitFriendInfo(long uin)
        {

            Logger.Debug("开始获取好友信息");
            var dat = Get(ApiUrl.GetFriendInfo, uin, Session.Vfwebqq, Session.Psessionid);

            FriendDetail friendData = (FriendDetail)JsonConvert.DeserializeObject(dat, typeof(FriendDetail));
            if (friendData.RetCode == 0)
            {
                var info = OrmManager.Get<FriendInfo>(x => x.Uin == uin && x.QQNum == Session.QQNum);
                if (info == null)
                {
                    info = Unitities.GetFriendInfo(friendData);
                    info.QQNum = Session.QQNum;
                    info.Uin = uin;
                    OrmManager.Insert(info);
                }
                else
                {
                    info.MapFromFriendDetail(friendData);
                    OrmManager.Update(info);
                }
                Logger.Debug("获取朋友信息完成");
            }
            else
            {
                Logger.Debug("初始化朋友信息出错");
                EventAggregator.GetEvent<ErrorEvent>().Publish(new ErrorEventArgs(friendData.RetCode, "初始化朋友信息出错:uin:" + uin));
            }

        }
        /// <summary>
        /// 获取自己的信息
        /// </summary>
        public void InitSelfInfo()
        {
            Logger.Debug("初始化个人信息");
            string dat = Get(ApiUrl.GetAccountInfo);
            FriendDetail selfDetail = (FriendDetail)JsonConvert.DeserializeObject(dat, typeof(FriendDetail));
            if (selfDetail.RetCode == 0)
            {
                var info = OrmManager.Get<FriendInfo>(x => x.Uin == Session.QQNum && x.QQNum == Session.QQNum);
                if (info == null)
                {
                    info = Unitities.GetFriendInfo(selfDetail);
                    info.QQNum = Session.QQNum;
                    info.Uin = Session.QQNum;
                    OrmManager.Insert(info);
                }
                else
                {
                    info.MapFromFriendDetail(selfDetail);
                    OrmManager.Update(info);
                }
                SelfInfo.MapFromFriendDetail(selfDetail);
                SelfInfo.QQNum = Session.QQNum;
                SelfInfo.Uin = Session.QQNum;
                OrmManager.Update(info);
                Logger.Debug("初始化个人信息完成");
            }
            else
            {
                Logger.Debug("初始化个人信息出错");
                EventAggregator.GetEvent<ErrorEvent>().Publish(new ErrorEventArgs(selfDetail.RetCode, "初始化个人信息出错"));
            }
        }
        #endregion
        #region 群组
        /// <summary>
        /// T 人
        /// </summary>
        public void KickGroupMember(long groupId, long uid)
        {

            Logger.Debug("从{0}中踢出{1}", groupId, uid);
            var dat = PostData(ApiUrl.KickGroupMember,
                 new Dictionary<string, object> { { "gc", groupId }, { "ul", uid }, { "flag", "0" }, { "bkn", Session.Skey.GetBkn() } });
            Logger.Debug("返回结果：{0}", dat);
        }
        /// <summary>
        /// 获取群详细信息
        /// </summary>
        public void InitGroupInfo2(long gc)
        {

            Logger.Debug("开始获取群资料{0}",gc);


            var dat = PostData(ApiUrl.GetGroupInfo2,
                new Dictionary<string, object> { { "gc", gc }, { "st", 0 }, { "end", 20 }, { "sort", 0 }, { "bkn", Session.Skey.GetBkn() } });

            Logger.Debug("{1}返回结果{0}", dat,gc);
        }
        /// <summary>
        ///         online,offline,away,hidden ,  busy ,callme , silent
        /// </summary>
        /// <param name="state">online,offline,away,hidden ,  busy ,callme , silent</param>
        public void ChangeState(string state)
        {
            Logger.Debug("更改状态为{0}",state );
            var dat = Get(ApiUrl.ChangeState,state.ToLower(), Session.ClientId, Session.Psessionid,GetTimeStamp() );

            Logger.Debug("{1}返回结果{0}", dat);
        }
        /// <summary>
        /// 设置群名片
        /// </summary>
        /// <param name="groupId">群号</param>
        /// <param name="qq">qq号</param>
        /// <param name="card">名片</param>
        public void SetGroupMemberCard(long groupId,long qq,string card)
        {
            Logger.Debug("开始设置名片{0}：{1},{2}", groupId,qq,card);


            var dat = PostData(ApiUrl.SetGroupMemberCard,
                new Dictionary<string, object> { { "gc", groupId }, { "u", qq }, { "name", card },  { "bkn", Session.Skey.GetBkn() } });

            Logger.Debug("{1}返回结果{0}", dat, groupId);
        }
        /// <summary>
        /// 设置群管理员
        /// </summary>
        /// <param name="groupId">群号</param>
        /// <param name="qq">qq号</param>
        /// <param name="set">true设置,false取消</param>
        public void SetGroupAdmin(long groupId, long qq,bool set)
        {


            Logger.Debug("开始设置管理员{0}：{1}", groupId, qq);


            var dat = PostData(ApiUrl.SetGroupMemberCard,
                new Dictionary<string, object> { { "src", "qinfo_v2" }, { "gc", groupId }, { "u", qq }, { "op", set?1:0 }, { "bkn", Session.Skey.GetBkn() } });

            Logger.Debug("{1}返回结果{0}", dat, groupId);
        }
        /// <summary>
        /// 群签到
        /// </summary>
        /// <param name="groupId">群号</param>
        public void GroupSign(long groupId)
        {

            if (string.IsNullOrEmpty(Session.Skey))
                return;
            Logger.Debug("开始签到{0", groupId);


            var dat = Post(ApiUrl.GroupSign, ApiUrl.GroupSign.BuildReferer(groupId),
                new JObject { { "gc", groupId }, { "is_sign", 0 }, { "bkn", Session.Skey.GetBkn() } },-1);

            Logger.Debug("{1}返回结果{0}", dat, groupId);
        }
        /// <summary>
        /// 邀请好友加入群聊
        /// </summary>
        /// <param name="groupId">群号</param>
        /// <param name="qq">qq号数组</param>
        public void GroupInviteFriend(long groupId, long[] qq)
        {
            Logger.Debug("开始邀请朋友加入{0}：{1}", groupId,string.Join("|",qq));


            var dat = PostData(ApiUrl.GroupInviteFriend,
                new Dictionary<string, object> { { "gc", groupId }, { "ul", string.Join("|", qq) },  { "bkn", Session.Skey.GetBkn() } });

            Logger.Debug("{1}返回结果{0}", dat, groupId);
        }
        /// <summary>
        /// 获取群真实列表
        /// </summary>
        public void InitGroupList2()
        {
            Logger.Debug("开始群资料");


            var dat = PostData(ApiUrl.GetGroupList2,
                new Dictionary<string,object> { { "bkn", Session.Skey.GetBkn() } });

            Logger.Debug("返回结果{0}", dat);
            Data.Qun.QQJoinedQun group = (Data.Qun.QQJoinedQun)JsonConvert.DeserializeObject(dat, typeof(Data.Qun.QQJoinedQun));

            if (group.ErrorCode == 0)
            {
                foreach (var qun in group.Created)
                {
                    InitGroupInfo2(qun.GroupId);
                }
                foreach (var qun in group.Joined)
                {
                    InitGroupInfo2(qun.GroupId);
                }
                foreach (var qun in group.Managed)
                {
                    InitGroupInfo2(qun.GroupId);
                }

            }
        }
        /// <summary>
        /// 获取好友真实列表
        /// </summary>
        public void InitFriendList2()
        {
            Logger.Debug("开始好友资料");


            var dat = PostData(ApiUrl.GetFriendList2,
                new Dictionary<string, object> {  { "bkn", Session.Skey.GetBkn() } });

            Logger.Debug("返回结果{0}", dat);

            Data.Qun.JsonFriend friends = (Data.Qun.JsonFriend)JsonConvert.DeserializeObject(dat, typeof(Data.Qun.JsonFriend));

            if (friends.ErrorCode == 0)
            {
                foreach (var cat in friends.Result)
                {
                    foreach (var friend in cat.Value.Friends)
                    {
                        friend.Alias = cat.Value.Name + "：" + friend.Name;
                    }
                }
            }
        }
        /// <summary>
        /// 获取群列表并保存
        /// </summary>
        public void InitGroupList()
        {
           InitGroupList2();
             Logger.Debug("开始获取群资料");

            var dat = Post(ApiUrl.GetGroupList,
                new JObject { { "vfwebqq", Session.Vfwebqq }, { "hash", Session.Hash } });
            Logger.Debug(dat);
            GroupMessageModel group = (GroupMessageModel)JsonConvert.DeserializeObject(dat, typeof(GroupMessageModel));
            if (group.RetCode == 0)
            {
                var localGroupInfos = OrmManager.Fetch<GroupInfo>(x => x.QQNum == Session.QQNum).Select(x=>x.GroupId).ToList();
                var serverGroupInfos = group.Result.GroupNames.Select(x => x.Gid);
                localGroupInfos.RemoveAll(x => serverGroupInfos.Contains(x));
                foreach (var id in localGroupInfos)
                {
                    var groupInfo = OrmManager.Get<GroupInfo>(x => x.QQNum == Session.QQNum && x.GroupId ==id);
                    groupInfo.StillIn = false;
                    OrmManager.Update(groupInfo);
                }
                for (int i = 0; i < group.Result.GroupNames.Count; i++)
                { Logger.Debug("开始拉取群资料："+group.Result.GroupNames[i].Gid);
                    var groupInfo = OrmManager.Get<GroupInfo>(x => x.QQNum == Session.QQNum && x.GroupId == group.Result.GroupNames[i].Gid);
                    if (groupInfo == null)
                    {
                        groupInfo = Unitities.GetGroupInfoFromGroupName(group.Result.GroupNames[i],Session.QQNum);

                        OrmManager.Insert(groupInfo);
                    }
                    else
                    {
                        groupInfo.MapFromGroupName(group.Result.GroupNames[i]);
                        OrmManager.Update(groupInfo);
                    }
                }
                Logger.Debug("完成群资料提取");
            }
            else
            {
                Logger.Debug("初始化群信息出错");
                EventAggregator.GetEvent<ErrorEvent>().Publish(new ErrorEventArgs(group.RetCode,"初始化群信息出错"));
            }
        }
        /// <summary>
        /// 获取群的详细信息
        /// </summary>
        /// <param name="gid"></param>
        public void InitGroupInfo(long gid)
        {

            var dbGoupInfo = OrmManager.Get<GroupInfo>(x => x.GroupId == gid && x.QQNum == Session.QQNum);
            if (dbGoupInfo == null)
                return;
           // InitGroupInfo2(gid);
            long gcode = dbGoupInfo.Code;
            var dat = Get(ApiUrl.GetGroupInfo, gcode, Session.Vfwebqq);
            Logger.Debug("开始拉取群详细信息"+gid);
            GroupDetailInfo groupInfo = (GroupDetailInfo)JsonConvert.DeserializeObject(dat, typeof(GroupDetailInfo));
            if (groupInfo.RetCode == 0)
            {
                dbGoupInfo.MapFromGroupDetail(groupInfo.Result);
                OrmManager.Update(dbGoupInfo);
                Logger.Debug("拉取群成员");
                for (int i = 0; i < groupInfo.Result.Members.Count; i++)
                {
                    var memberInfo = OrmManager.Get<MemberInfo>(x => x.QQNum == Session.QQNum && x.GroupId == gid && x.Uin == groupInfo.Result.Members[i].Uin);
                    if (memberInfo == null)
                    {
                        memberInfo = Unitities.GetMemberInfoFromGroupDetail(groupInfo.Result.Members[i],Session.QQNum);
                        memberInfo.GroupId = gid;
                        OrmManager.Insert(memberInfo);
                    }

                    else
                    {
                        memberInfo.MapFromMemberInfoDetail(groupInfo.Result.Members[i]);
                        OrmManager.Update(memberInfo);
                    }
                    var needUpdate = false;
                    memberInfo = OrmManager.Get<MemberInfo>(x => x.QQNum == Session.QQNum && x.GroupId == gid && x.Uin == groupInfo.Result.Members[i].Uin);
                    if (groupInfo.Result.Cards != null)
                    {
                        needUpdate = true;
                        var card = groupInfo.Result.Cards.FirstOrDefault(x => x.MUin == memberInfo.Uin);
                        if (card != null)
                        { memberInfo.Card = card.Card; }
                    }
                    var managerinfo = groupInfo.Result.GroupDetail.Members.FirstOrDefault(x => x.MemberUin == memberInfo.Uin);
                    if (managerinfo != null)
                    {
                        needUpdate = true;
                        if (managerinfo.MemberFlag % 2 == 1)
                        {

                            memberInfo.IsManager = true;
                        }
                        else
                        {
                            memberInfo.IsManager = false;
                        }
                    }
                    var vipInfo = groupInfo.Result.VipInfo.FirstOrDefault(x => x.Uin == memberInfo.Uin);
                    if (vipInfo != null)
                    {
                        needUpdate = true;
                        memberInfo.IsVip = vipInfo.IsVip;
                        memberInfo.VipLevel = vipInfo.VipLevel;
                    }
                    var statusInfo = groupInfo.Result.Status.FirstOrDefault(x => x.Uin == memberInfo.Uin);
                    if (statusInfo != null)
                    {
                        needUpdate = true;
                        memberInfo.Status = statusInfo.Status;
                        memberInfo.ClientType = statusInfo.ClientType;
                    }
                    if (needUpdate)
                    { OrmManager.Update(memberInfo); }
                }
                Logger.Debug("完成群成员拉取");
                Logger.Debug("完成群详细信息拉取");
            }
            else
            {
                Logger.Debug("拉取群信息出错");
                EventAggregator.GetEvent<ErrorEvent>().Publish(new ErrorEventArgs(groupInfo.RetCode, "初始化群信息出错:gid:"+gid));
            }
           
        }
        #endregion
#region 讨论组

        /// <summary>
        /// 获取讨论组列表并保存
        /// </summary>
        public  void InitDisscussList()
        {

            Logger.Debug("开始拉取讨论组");
            var dat = Get(ApiUrl.GetDiscussionList, Session.Psessionid, Session.Vfwebqq, GetTimeStamp());
            Disscuss disscuss = (Disscuss)JsonConvert.DeserializeObject(dat, typeof(Disscuss));
            if (disscuss.RetCode == 0)
            {
                var localDisscussInfos = OrmManager.Fetch<DiscussInfo>(x => x.QQNum == Session.QQNum).Select(x => x.Did).ToList();
                var serverDisscussInfos = disscuss.Result.DNameList.Select(x => x.Did);
                localDisscussInfos.RemoveAll(x => serverDisscussInfos.Contains(x));
                foreach (var id in localDisscussInfos)
                {
                    var disscussInfo = OrmManager.Get<DiscussInfo>(x => x.QQNum == Session.QQNum && x.Did == id);
                    disscussInfo.StillIn = false;
                    OrmManager.Update(disscussInfo);
                }
                
                for (int i = 0; i < disscuss.Result.DNameList.Count; i++)
                { Logger.Debug("开始更新讨论组信息："+disscuss.Result.DNameList[i].Did);
                    var disscussInfo = OrmManager.Get<DiscussInfo>(x => x.QQNum == Session.QQNum && x.Did == disscuss.Result.DNameList[i].Did);
                    if (disscussInfo == null)
                    {
                        disscussInfo = new DiscussInfo();
                        disscussInfo.Did = disscuss.Result.DNameList[i].Did;
                        disscussInfo.QQNum = Session.QQNum;
                        disscussInfo.Name = disscuss.Result.DNameList[i].Name;
                        OrmManager.Insert(disscussInfo);
                    }
                    else
                    {
                        disscussInfo.Name = disscuss.Result.DNameList[i].Name;
                        OrmManager.Update(disscussInfo);

                    }

                    InitDisscussInfo(disscuss.Result.DNameList[i].Did);
                }
                Logger.Debug("完成讨论组信息更新");
            }
            else
            {
                Logger.Debug("拉取讨论组信息出错");
                EventAggregator.GetEvent<ErrorEvent>().Publish(new ErrorEventArgs(disscuss.RetCode, "初始化讨论组信息出错" ));
            }
        }
        /// <summary>
        /// 获取讨论组详细信息
        /// </summary>
        /// <param name="did"></param>
        public  void InitDisscussInfo(long did)
        {

            Logger.Debug("开始获取讨论组详细信息："+did);
            var dat = Get(ApiUrl.GetDiscussionInfo, did, Session.Vfwebqq, Session.Psessionid);
            DisscussInfo inf = (DisscussInfo)JsonConvert.DeserializeObject(dat, typeof(DisscussInfo));
            if (inf.RetCode == 0)
            {
                for (int i = 0; i < inf.Result.Info.MemberList.Count; i++)
                {
                    var disscussMemberInfo = OrmManager.Get<DiscussMemberInfo>(x => x.QQNum == Session.QQNum && x.Did == did && x.Uin == inf.Result.Info.MemberList[i].Uin);
                    if (disscussMemberInfo == null)
                    {
                        disscussMemberInfo = new DiscussMemberInfo();
                        disscussMemberInfo.Uin = inf.Result.Info.MemberList[i].Uin;
                        disscussMemberInfo.Did = did;
                        disscussMemberInfo.QQNum = Session.QQNum;
                        disscussMemberInfo.RealQQ = inf.Result.Info.MemberList[i].RealUin;
                        OrmManager.Insert(disscussMemberInfo);
                    }
                    else
                    {
                        disscussMemberInfo.RealQQ = inf.Result.Info.MemberList[i].RealUin;
                        OrmManager.Update(disscussMemberInfo);
                    }
                    disscussMemberInfo = OrmManager.Get<DiscussMemberInfo>(x => x.QQNum == Session.QQNum && x.Did == did && x.Uin == inf.Result.Info.MemberList[i].Uin);
                    var memberInfo = inf.Result.Members.FirstOrDefault(x => x.Uin == disscussMemberInfo.Uin);
                    var needUpdate = false;
                    if (memberInfo != null)
                    {
                        needUpdate = true;
                        disscussMemberInfo.Nick = memberInfo.Nick;

                    }
                    var statusInfo = inf.Result.Status.FirstOrDefault(x => x.Uin == disscussMemberInfo.Uin);
                    if (statusInfo != null)
                    {
                        needUpdate = true;
                        disscussMemberInfo.Status = statusInfo.Status;
                        disscussMemberInfo.ClientType = statusInfo.ClientType;
                    }
                    if (needUpdate)
                    {
                        OrmManager.Update(disscussMemberInfo);

                    }

                }
                Logger.Debug("完成讨论组信息拉取："+did);
            }
            else
            {
                Logger.Debug("拉取讨论组出错："+did);
                EventAggregator.GetEvent<ErrorEvent>().Publish(new ErrorEventArgs(inf.RetCode, "初始化讨论组信息出错:did:" + did));
            }
        }
        #endregion

#region 辅助凼数

        /// <summary>
        /// 根据uin获取真实QQ号
        /// </summary>
        /// <param name="uin"></param>
        /// <returns></returns>
        public long InitRealQQ(long uin)
        {    

            var friend=     OrmManager.Get<FriendInfo>(x=>x.Uin==uin);
            var member = OrmManager.Get<MemberInfo>(x => x.Uin == uin);
            if (member != null)
            {
             //  return ;
            }
            if (friend != null && (friend.RealQQ==0))
            {
                InitFriendInfo(uin);
                return friend.RealQQ;
            }
            else if(friend==null)
            {
                InitFriendList();
                friend = OrmManager.Get<FriendInfo>(x => x.Uin == uin);
                if (friend != null)
                    return friend.RealQQ;
            }
            var discussmember = OrmManager.Get<DiscussMemberInfo>(x => x.Uin == uin);
            if (discussmember != null)
            {
                return discussmember.RealQQ;
            }
            return 0;
        }
        /// <summary>
        /// 获取时间戳
        /// </summary>
        /// <param name="type">1，10位；2，13位。</param>
        /// <returns></returns>
        public  string GetTimeStamp(int type = 1)
        {
            if (type == 1)
            {
                DateTime dt1970 = new DateTime(1970, 1, 1, 0, 0, 0, 0);
                return ((DateTime.UtcNow.Ticks - dt1970.Ticks) / 10000).ToString();
            }
            else if (type == 2)
            {
                TimeSpan ts = DateTime.UtcNow - new DateTime(1970, 1, 1, 0, 0, 0, 0);
                return Convert.ToInt64(ts.TotalSeconds).ToString();
            }
            else return "ERROR";
        }
#endregion
    }
}

