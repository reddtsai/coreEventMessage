using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace eventMessage.UnitTests
{
    public class CleanupNetMQ : IDisposable
    {
        public virtual void Dispose()
        {
            Thread.Sleep(1000);
            NetMQ.NetMQConfig.Cleanup();
        }
    }
}
