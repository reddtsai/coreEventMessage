using NetMQ;
using System;

namespace eventMessage.UnitTests
{
    public class PullFixture : CleanupNetMQ
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

        public override void Dispose() 
        {
            Pull.Close();
            base.Dispose();
        }
    }
}