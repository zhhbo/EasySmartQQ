using System;
using Easy.Data.Common.Interface;

namespace Easy.Data.Database.Core
{
    public abstract class DbConnectionBase: IObjectId, IConnection, IDisposable
    {
        private readonly string _objectId;

        public string ObjectId
        {
            get { return _objectId; }
        }

        protected volatile bool _isConnected;

        public bool IsConnected
        {
            get { return _isConnected; }
        }

        protected volatile bool _isOccupation;

        public bool IsOccupation
        {
            get { return _isOccupation; }
        }

        protected DateTime _lastUseTime;

        public DateTime LastUseTime
        {
            get { return _lastUseTime; }
        }

        public string ConnectionStr { get; set; }

        public string ErrorMsg { get; set; }

        protected DbConnectionBase()
        {
            _isConnected = _isOccupation = false;
            _lastUseTime = DateTime.Now;
            _objectId = Guid.NewGuid().ToString();
        }

        public abstract T GetRealConnection<T>() where T : class;

        public abstract bool Connect();

        public abstract bool WaitForClose(bool isWait = false);

        public abstract bool Close();

        public abstract void Dispose();

        public void EndUse()
        {
            _isOccupation = false;
        }
    }
}
