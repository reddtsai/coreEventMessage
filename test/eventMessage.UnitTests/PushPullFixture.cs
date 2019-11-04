using System;
using System.Linq;
using System.Text;
using System.Threading;

namespace eventMessage.UnitTests
{
    public class PushPullFixture : IDisposable
    {
        public MessagePuller Pull;
        public MessagePusher Push;
        TestPullHandler Handler;

        public PushPullFixture()
        {
            Pull = new MessagePuller(9100);
            Handler = new TestPullHandler();
            Push = new MessagePusher("localhost", 9100);

            Thread.Sleep(300);
            
            Pull.SetReceiveHandleEvent(Handler);
            Pull.Listen();
        }

        public string GetMessage()
        {
            return Encoding.UTF8.GetString(Handler.Block);
        }

        public int GetLength()
        {
            return Handler.Block.Length;
        }

        public void Dispose()
        {
            Push.Close();
            Pull.Close();
        }
    }

    public class TestPullHandler : IPullHandler
    {
        //public string TestData { get; set; }
        public byte[] Block { get; set; }

        public void OnReceive(byte[] buffer)
        {
            Block = buffer.ToArray();
            //TestData = Encoding.UTF8.GetString(buffer);
        }
    }
}