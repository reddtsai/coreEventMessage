using System;
using System.Threading;
using System.Threading.Tasks;
using NetMQ;
using NetMQ.Sockets;

namespace eventMessage 
{
    public interface IMessagePuller : IDisposable
    {
        void SetReceiveHandleEvent(IPullHandler pullHandler);
        void Listen();
        void Close();
    }

    public class MessagePuller : IMessagePuller 
    {
        PullSocket _puller;
        IPullHandler _pullHandler;
        Task _receiveTask;
        CancellationTokenSource _cancellationTokenSource;

        /// <summary>
        /// create a socket server to pull message
        /// </summary>
        /// <param name="port">socket port</param>
        public MessagePuller(int port) 
        {
            _puller = new PullSocket();
            _puller.Bind($"tcp://*:{port}");
        }

        /// <summary>
        /// set up custom event processor to handle receive event from pull socket
        /// </summary>
        /// <param name="pullHandler">custom receive handler</param>
        public void SetReceiveHandleEvent(IPullHandler pullHandler)
        {
            _pullHandler = pullHandler;
        }

        /// <summary>
        /// listen on pull socket to receive message
        /// </summary>   
        public void Listen() 
        {
            if (_pullHandler is null) 
            {
                throw new ArgumentNullException(nameof(_pullHandler));
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
            byte[] buf;

            while(!ct.IsCancellationRequested)
            {
                if(_puller.TryReceiveFrameBytes(out buf))
                {
                    _pullHandler.OnReceive(buf);
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
            _puller.Close();
        }

        public void Dispose()
        {
            Close();
            GC.SuppressFinalize(this);
        }
    }
}