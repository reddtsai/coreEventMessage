using NetMQ;
using System.Text;
using System.Threading;
using Xunit;

namespace eventMessage.UnitTests
{
    public class PubSubTest 
    {
        [Fact]
        public void PubSub()
        {
            var pub = new MessagePublisher(9200);
            var sub = new MessageSubscriber("localhost", 9200); 
            pub.Close();
            sub.Close();
        }

        [Theory]
        [InlineData(9201, "topic", "message")]
        [InlineData(9202, "redd", "hello")]
        public void PubSubTopic(int port, string topic, string message)
        {
            using(var pub = new MessagePublisher(port))
            using(var sub = new MessageSubscriber("localhost", port))
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

                pub.Close();
                sub.Close();
            }
        }

        [Fact]
        public void Unsubscribe()
        {
            using(var pub = new MessagePublisher(9203))
            using(var sub = new MessageSubscriber("localhost", 9203))
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