using NetMQ;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using Xunit;
using Xunit.Abstractions;

namespace eventMessage.UnitTests
{
    public class PubSubTest : IClassFixture<CleanupNetMQ>
    {
        private readonly ITestOutputHelper _testOutputHelper;

        public PubSubTest(ITestOutputHelper testOutputHelper)
        {
            _testOutputHelper = testOutputHelper;
        }

        [Fact]
        public void PubSub()
        {
            var pub = new MessagePublisher(9200);
            var sub = new MessageSubscriber("localhost", 9200); 
            pub.Close();
            sub.Close();
        }

        [Theory]
        [InlineData("topic", "message")]
        [InlineData("redd", "hello")]
        public void PubSubTopic(string topic, string message)
        {
            using(var pub = new MessagePublisher(9201))
            using(var sub = new MessageSubscriber("localhost", 9201))
            {
                var handler = new TestSubscribeHandler();
                sub.SetReceiveHandleEvent(handler);
                sub.Listen();
                sub.Subscribe(Encoding.UTF8.GetBytes(topic));
                Thread.Sleep(200);

                pub.Send(Encoding.UTF8.GetBytes(topic), Encoding.UTF8.GetBytes(message));
                Thread.Sleep(200);          
                
                Assert.Equal(topic, Encoding.UTF8.GetString(handler.Block[0]));
                Assert.Equal(message, Encoding.UTF8.GetString(handler.Block[1]));
            }
        }

        [Fact]
        public void Unsubscribe()
        {
            using(var pub = new MessagePublisher(9202))
            using(var sub = new MessageSubscriber("localhost", 9202))
            {
                var handler = new TestSubscribeHandler();
                sub.SetReceiveHandleEvent(handler);
                sub.Listen();
                sub.Subscribe(Encoding.UTF8.GetBytes("A"));
                Thread.Sleep(200);

                pub.Send(Encoding.UTF8.GetBytes("A"), Encoding.UTF8.GetBytes("hello"));
                Thread.Sleep(200);

                Assert.Equal("A", Encoding.UTF8.GetString(handler.Block[0]));
                Assert.Equal("hello", Encoding.UTF8.GetString(handler.Block[1]));
                handler.Block = null;

                sub.Unsubscribe(Encoding.UTF8.GetBytes("A"));
                Thread.Sleep(200);

                pub.Send(Encoding.UTF8.GetBytes("A"), Encoding.UTF8.GetBytes("hello"));
                Thread.Sleep(200);

                Assert.Null(handler.Block);
            }
        }
    }
}