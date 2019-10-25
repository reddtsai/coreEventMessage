using NetMQ;
using NetMQ.Sockets;

namespace eventMessage 
{
    public interface IMessagePublisher 
    {
        void Send (string topic, string message);
        void Send (byte[] topic, byte[] message);
        void Close ();
    }

    public class MessagePublisher : IMessagePublisher 
    {
        PublisherSocket _publisher;

        /// <summary>
        /// create a NetMQ publisher socket
        /// </summary>
        /// <param name="port">socket port</param>
        public MessagePublisher (int port) 
        {
            _publisher = new PublisherSocket ();
            _publisher.Options.SendHighWatermark = 1000;
            _publisher.Bind ($"tcp://*:{port}");
        }

        /// <summary>
        /// publish message to topic
        /// </summary>
        /// <param name="topic">topic</param>
        /// <param name="message">message</param>
        public void Send (string topic, string message) 
        {
            _publisher.SendMoreFrame (topic).SendFrame (message);
        }

        /// <summary>
        /// publish message to topic
        /// </summary>
        /// <param name="topic">topic</param>
        /// <param name="message">message</param>
        public void Send (byte[] topic, byte[] message) 
        {
            _publisher.SendMoreFrame (topic).SendFrame (message);
        }

        /// <summary>
        /// close NetMQ publisher socket
        /// </summary>
        public void Close () 
        {
            _publisher.Close ();
        }
    }
}