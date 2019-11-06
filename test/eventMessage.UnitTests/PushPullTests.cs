using System.Text;
using System.Threading;
using Xunit;

namespace eventMessage.UnitTests
{
    public class PushPullTests : IClassFixture<PullFixture>
    {        
        PullFixture _fixture;

        public PushPullTests(PullFixture fixture)
        {
            _fixture = fixture;
        }

        [Fact]
        public void PushPull()
        {
            var pull = new MessagePuller(9100);
            pull.SetReceiveHandleEvent(new TestPullHandler());
            pull.Listen();

            var Push = new MessagePusher("localhost", 9100);

            pull.Close();
            Push.Close();
        }

        [Fact]
        public void PushPullText()
        {      
            using(var push = new MessagePusher("localhost", 9101))
            {
                push.Send(Encoding.UTF8.GetBytes("hello"));
                Thread.Sleep(500);
                Assert.Equal("hello", Encoding.UTF8.GetString(_fixture.GetBytes()));
            }
        }

        [Fact]
        public void PushPullBytes()
        {      
            using(var push = new MessagePusher("localhost", 9101))
            {
                push.Send(new byte[100]);
                Thread.Sleep(500);
                Assert.Equal(100, _fixture.GetBytes().Length);
            }
        }
    }
}