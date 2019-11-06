using NetMQ;
using System;

namespace eventMessage.UnitTests
{
    public class PullFixture : IDisposable
    {
        MessagePuller Pull;
        TestPullHandler Handler;

        public PullFixture()
        {
            Pull = new MessagePuller(9101);
            Handler = new TestPullHandler();
            Pull.SetReceiveHandleEvent(Handler);
            Pull.Listen();
        }

        public byte[] GetBytes()
        {
            return Handler.Block;
        }

        public void Dispose()
        {
            Pull.Close();
        }
    }
}