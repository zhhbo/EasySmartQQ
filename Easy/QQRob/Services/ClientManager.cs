using System.Collections.Generic;
using System.Linq;
using System.Threading;
namespace Easy.QQRob.Services
{
    using Microsoft.Practices.Unity;
    using Easy.QQRob.Models;
    public class ClientManager 
    {
        private static readonly Dictionary<long, ClientManager> Manager = new Dictionary<long, ClientManager>();

        public static ClientManager GetClientManagerUser(IUnityContainer container, QQState user)
        {
            if (!Manager.ContainsKey(user.QQNum))
            {
                Manager.Add(user.QQNum, new ClientManager( user,new SmartQQClient(container)));
            }
            return Manager[user.QQNum];
        }
        public static ClientManager GetClientManagerUser(long qq)
        {
            if (!Manager.ContainsKey(qq))
            {
                return null;
            }
            return Manager[qq];
        }
     public static   object _lock = new object();
        public static bool ResetKey()
        {
            try {

                if (Monitor.TryEnter(_lock))
                {
                    List<ClientManager> needToReplace =new List<ClientManager>();// Manager.Where(x => x.Key != x.Value.QQ.QQNum).Select(x => x.Value);
                    List<long> needToRemove = new List<long>();// Manager.Where(x => x.Key != x.Value.QQ.QQNum).Select(x => x.Key);
                    foreach (var m in Manager)
                    {
                        if (m.Key != m.Value.QQ.QQNum&&m.Value.QQ.QQNum!=0)
                        {
                            needToReplace.Add(m.Value);
                            needToRemove.Add(m.Key);
                        }
                    }
                    foreach (var key in needToRemove.Distinct())
                    {
                        Manager.Remove(key);
                    }
                    foreach (var m in needToReplace)
                    {
                        if(Manager.ContainsKey(m.QQ.QQNum))
                            Manager.Remove(m.QQ.QQNum);
                        Manager.Add(m.QQ.QQNum, m);
                    }
                    Monitor.Exit(_lock);
                }
            }
            catch {
                if (Monitor.TryEnter(_lock))
                {
                    Monitor.Exit(_lock);
                }
                else
                {
                    Monitor.Exit(_lock);
                }
                return false;
            }
            
            return true;
        }
        private readonly QQState _user;

        public static void CLoseAll()
        {
            foreach (var c in Manager)
            {
                c.Value.Close();
            }

        }
        public void Close()
        {
            Client.Close();
        }
        private readonly SmartQQClient _client;


        public ClientManager( QQState user,SmartQQClient client)
        {
            _user = user;
            _client = client;

        }

        public SmartQQClient Client
        {
            get { return _client; }
        }
        public QQState QQ
        {
            get { return _user; }
        }
    }
}
