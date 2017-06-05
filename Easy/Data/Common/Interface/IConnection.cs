using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Easy.Data.Common.Interface
{
    public interface IConnection
    {
        bool IsConnected { get; }

        bool IsOccupation { get; }

        DateTime LastUseTime { get; }

        bool Connect();

        bool WaitForClose(bool isWait = false);

        bool Close();
    }
}
