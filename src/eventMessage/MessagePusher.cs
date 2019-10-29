using NetMQ;
using NetMQ.Sockets;
using System;

namespace eventMessage
{
    public interface IMessagePusher : IDisposable
    {
        void Send(byte[] message);
        void Close();
    }

    public class MessagePusher : IMessagePusher
    {
        PushSocket _pusher;

        /// <summary>
        /// create a socket client to push message
        /// </summary>
        /// <param name="ip">socket server ip</param>
        /// <param name="port">socket server port</param>
        public MessagePusher(string ip, int port)
        {
            _pusher = new PushSocket();
            _pusher.Connect($"tcp://{ip}:{port}");  
        }

        /// <summary>
        /// push message
        /// </summary>
        /// <param name="message">message</param>
        public void Send(byte[] message) 
        {
            _pusher.SendFrame(message);
        }

        /// <summary>
        /// close socket
        /// </summary>
        public void Close() 
        {
            _pusher.Close();
        }

        public void Dispose()
        {
            Close();
            GC.SuppressFinalize(this);
        }
    }
}