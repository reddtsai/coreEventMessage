using System.Text;
using System.Threading;
using Xunit;

namespace eventMessage.UnitTests
{
    public class PushPullTests : IClassFixture<PushPullFixture>
    {        
        PushPullFixture _fixture;

        public PushPullTests()
        {
            _fixture = new PushPullFixture();
        }

        [Fact]
        public void PushPullText()
        {      
            _fixture.Push.Send(Encoding.UTF8.GetBytes("hello"));
            Thread.Sleep(1000);
            var test = _fixture.GetMessage();
            Assert.Equal("hello", test);
        }

        [Fact]
        public void PushPullBytes()
        {      
            _fixture.Push.Send(new byte[100]);
            Thread.Sleep(1000);
            var test = _fixture.GetLength();
            Assert.Equal(100, test);
        }
    }
}