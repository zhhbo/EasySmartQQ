using System;
using System.Text;
using System.IO;
using System.Net;
using System.Web;
using Easy.QQRob.Models;
namespace Easy.QQRob.Services
{
    public static class QQClient
    {
        //网络通信相关
        public static CookieContainer cookies = new CookieContainer();
        public static string Get(string url, string referer = "http://d1.web2.qq.com/proxy.html?v=20151105001&callback=1&id=2", int timeout = 100000, Encoding encode = null, bool NoProxy = false)
        {
            string dat;
            HttpWebResponse res = null;
            HttpWebRequest req;
            try
            {
                req = (HttpWebRequest)WebRequest.Create(url);
                req.CookieContainer = cookies;
                req.AllowAutoRedirect = false;
                req.Timeout = timeout;
                req.Referer = referer;
                req.Accept = "*/*";
                if (NoProxy)
                    req.Proxy = null;
                req.UserAgent = "Mozilla/5.0 (Windows NT 10.0; WOW64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/53.0.2785.104 Safari/537.36 Core/1.53.2372.400 QQBrowser/9.5.10548.400";
                res = (HttpWebResponse)req.GetResponse();
                cookies.Add(res.Cookies);
            }
            catch (HttpException)
            {
                return "";
            }
            catch (WebException)
            {
                return "";
            }
            StreamReader reader;

            reader = new StreamReader(res.GetResponseStream(), encode == null ? Encoding.UTF8 : encode);
            dat = reader.ReadToEnd();

            res.Close();
            req.Abort();

            if (!dat.Equals(""))
            {
               // OrmManager.Insert(new SystemLogData { OriginData=dat.Replace(';',' '), Message= "System" });
            }

            return dat;
        }
        //http://www.itokit.com/2012/0721/74607.html
        public static string Post(string url, string data, string Referer = "http://d1.web2.qq.com/proxy.html?v=20151105001&callback=1&id=2", int timeout = 100000, Encoding encode = null)
        {
            string dat = "";
            HttpWebRequest req;
            try
            {
                req = WebRequest.Create(url) as HttpWebRequest;
                req.CookieContainer = cookies;
                req.ContentType = "application/x-www-form-urlencoded";
                req.Method = "POST";
                req.Proxy = null;
                req.Timeout = timeout;
                req.UserAgent = "Mozilla/5.0 (Windows NT 10.0; WOW64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/53.0.2785.104 Safari/537.36 Core/1.53.2372.400 QQBrowser/9.5.10548.400";
                req.Accept = "*/*";
               
                //req.UserAgent = "Mozilla/5.0 (Windows NT 10.0;%20WOW64; rv:47.0) Gecko/20100101 Firefox/47.0";
                req.ProtocolVersion = HttpVersion.Version11;
                req.Referer = Referer;
                req.Headers.Add("Origin", "");
                byte[] mybyte = Encoding.Default.GetBytes(data);
                req.ContentLength = mybyte.Length;

                Stream stream = req.GetRequestStream();
                stream.Write(mybyte, 0, mybyte.Length);


                HttpWebResponse res = req.GetResponse() as HttpWebResponse;
                cookies.Add(res.Cookies);
                stream.Close();

                StreamReader SR = new StreamReader(res.GetResponseStream(), encode == null ? Encoding.UTF8 : encode);
                dat = SR.ReadToEnd();
                res.Close();
                req.Abort();
            }
            catch (HttpException e)
            {
                OrmManager.Insert(new SystemLogData { OriginData = e.Message, Message = "网络故障" });
                return "";
            }
            catch (WebException e)
            {
                OrmManager.Insert(new SystemLogData { OriginData = e.Message, Message = "网络故障" });
                return "";
            }
            if (!dat.Equals(""))
            {
               // OrmManager.Insert(new SystemLogData { OriginData = dat.Replace(';',' '), Message = "System" });
            }
            return dat;
        }

        public static string PostJsonData(string url, string data,  int timeout = 100000, Encoding encode = null)
        {
            string dat = "";
            HttpWebRequest req;
            try
            {
                req = WebRequest.Create(url) as HttpWebRequest;
                req.ContentType = "application/json;charset=utf-8";
                req.Method = "POST";
                req.Proxy = null;
                req.Timeout = timeout;
               req.UserAgent= "Mozilla/5.0 (Windows NT 10.0; WOW64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/53.0.2785.104 Safari/537.36 Core/1.53.2372.400 QQBrowser/9.5.10548.400";
                req.ProtocolVersion = HttpVersion.Version11;

                byte[] mybyte = Encoding.Default.GetBytes(data);
                req.ContentLength = mybyte.Length;

                Stream stream = req.GetRequestStream();
                stream.Write(mybyte, 0, mybyte.Length);


                HttpWebResponse res = req.GetResponse() as HttpWebResponse;

               stream.Close();

                StreamReader SR = new StreamReader(res.GetResponseStream(), encode == null ? Encoding.UTF8 : encode);
                dat = SR.ReadToEnd();
                res.Close();
                req.Abort();
            }
            catch (HttpException)
            {
                return "";
            }
            catch (WebException)
            {
                return "";
            }
            if (!dat.Equals(""))
            { OrmManager.Insert(new SystemLogData { OriginData = dat.Replace(';', ' '), Message = "System" }); }
            return dat;
        }
        public delegate void AsyncPostAction(string data);
        private class AsyncPostData
        {
            public HttpWebRequest req;
            public AsyncPostAction asyncPostAction;
        }
        public static void AsyncPost(string url, string PostData, AsyncPostAction action, string Referer = "http://d1.web2.qq.com/proxy.html?v=20151105001&callback=1&id=2", int timeout = 100000)
        {
            HttpWebRequest req = WebRequest.Create(url) as HttpWebRequest;
            req.CookieContainer = cookies;
            req.ContentType = "application/x-www-form-urlencoded";
            req.Method = "POST";
            req.Referer = Referer;
            req.UserAgent = "Mozilla/5.0 (Windows NT 10.0; WOW64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/53.0.2785.104 Safari/537.36 Core/1.53.2372.400 QQBrowser/9.5.10548.400";

            //req.UserAgent = "Mozilla/5.0 (Windows NT 10.0;%20WOW64; rv:47.0) Gecko/20100101 Firefox/47.0";
            req.Proxy = null;
            req.ProtocolVersion = HttpVersion.Version11;
            req.ContinueTimeout = timeout;

            ASCIIEncoding encoding = new ASCIIEncoding();
            byte[] data = encoding.GetBytes(PostData);
            Stream stream = req.GetRequestStream();
            stream.Write(data, 0, data.Length);
            stream.Close();

            AsyncPostData dat = new AsyncPostData();
            dat.req = req;
            dat.asyncPostAction = action;
            req.BeginGetResponse(new AsyncCallback(AsyncPostResponesProceed), dat);
        }

        private static void AsyncPostResponesProceed(IAsyncResult ar)
        {
            StreamReader reader = null;
            AsyncPostData dat = ar.AsyncState as AsyncPostData;
            HttpWebRequest req = dat.req;
            HttpWebResponse res = req.GetResponse() as HttpWebResponse;
            reader = new StreamReader(res.GetResponseStream());
            string temp = reader.ReadToEnd();
            res.Close();
            req.Abort();
            dat.asyncPostAction(temp);
        }
    }
}
