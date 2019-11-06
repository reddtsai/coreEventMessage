using NetMQ;
using NetMQ.Sockets;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace eventMessage
{
    public interface IMessageSubscriber : IDisposable
    {
        void Subscribe(byte[] topic);
        void Unsubscribe(byte[] topic);
        void SetReceiveHandleEvent(ISubscribeHandler subscribeHandler);
        void Listen();
        void Close();
    }

    public class MessageSubscriber : IMessageSubscriber
    {
        SubscriberSocket _Subscriber;
        ISubscribeHandler _subscribeHandler;
        Task _receiveTask;
        CancellationTokenSource _cancellationTokenSource;

        /// <summary>
        /// create a socket client to subscribe message
        /// </summary>
        /// <param name="ip">socket server ip</param>
        /// <param name="port">socket server port</param>
        public MessageSubscriber(string ip, int port)
        {
            _Subscriber = new SubscriberSocket();  
            _Subscriber.Connect($"tcp://{ip}:{port}");        
        }

        /// <summary>
        /// subscribe a topic
        /// </summary>
        /// <param name="topic">topic</param>
        public void Subscribe(byte[] topic)
        {
            _Subscriber.Subscribe(topic);
        }

        /// <summary>
        /// unsubscribe a topic
        /// </summary>
        /// <param name="topic">topic</param>
        public void Unsubscribe(byte[] topic)
        {
            _Subscriber.Unsubscribe(topic);
        }

        /// <summary>
        /// set up custom event processor to handle receive event from subscribe socket
        /// </summary>
        /// <param name="subscribeHandler">custom receive handler</param>
        public void SetReceiveHandleEvent(ISubscribeHandler subscribeHandler)
        {
            _subscribeHandler = subscribeHandler;
        }

        /// <summary>
        /// listen on pull socket to receive message
        /// </summary>   
        public void Listen() 
        {
            if (_subscribeHandler is null) 
            {
                throw new ArgumentNullException(nameof(_subscribeHandler));
            }

            _cancellationTokenSource = new CancellationTokenSource();
            var cancelToken = _cancellationTokenSource.Token;
            _receiveTask = Task.Factory.StartNew(
                Receive,
                cancelToken);
        }

        private void Receive() 
        {
            var ct = _cancellationTokenSource.Token;
            //var ts = TimeSpan.FromSeconds(5);
            List<byte[]> buf = new List<byte[]>(2);
            
            while(!ct.IsCancellationRequested)
            {
                if(_Subscriber.TryReceiveMultipartBytes(ref buf, 2))
                {
                    _subscribeHandler.OnReceive(buf);
                }
            }
        }

        /// <summary>
        /// close socket
        /// </summary>
        public void Close()
        {
            _cancellationTokenSource?.Cancel();
            _receiveTask?.Wait();
            _Subscriber.Close();
        }

        public void Dispose()
        {
            Close();
            GC.SuppressFinalize(this);
        }
    }
}