using System;
using System.Data.SqlServerCe;
using Easy.Data.Database.Core;
namespace Easy.Data.Access.Connection
{
    public class SqlceConnection : DbConnectionBase
    {
        private SqlCeConnection _connection;

        public SqlceConnection()
        {
            _connection = new SqlCeConnection();
        }

        public override bool Close()
        {
            if (_isConnected)
            {
                if (_connection != null)
                {
                    _connection.Close();
                }
            }
            _isConnected = false;
            return true;
        }

        public override bool Connect()
        {
            if(string.IsNullOrEmpty(ConnectionStr))
            {
                ErrorMsg = "not register connection string!";
                return false;
            }
            _connection.ConnectionString = ConnectionStr;
            try
            {
                _connection.Open();
                _isConnected = true;
            }
            catch(Exception exp)
            {
                ErrorMsg = exp.Message;
                _isConnected = false;
            }
            return _isConnected;
        }

        public override void Dispose()
        {
            _isConnected = _isOccupation = false;
            Close();
            if (_connection != null)
            {
                _connection.Dispose();
            }
            _connection = null;
        }

        public override T GetRealConnection<T>()
        {
            _isOccupation = true;
            _lastUseTime = DateTime.Now;
            return _connection as T;
        }

        public override bool WaitForClose(bool isWait = false)
        {
            return Close();
        }
    }
}
