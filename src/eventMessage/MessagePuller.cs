using System;
using System.Threading;
using System.Threading.Tasks;
using NetMQ;
using NetMQ.Sockets;

namespace eventMessage 
{
    public interface IMessagePuller 
    {
        void Listen ();
        void Close();
    }

    public class MessagePuller : IMessagePuller 
    {
        PullSocket _puller;
        IReceiveHandler _receiveHandler;
        Task _receiveTask;
        CancellationTokenSource _cancellationTokenSource;

        /// <summary>
        /// create a NetMQ pull socket
        /// </summary>
        /// <param name="port">socket port</param>
        public MessagePuller (int port) 
        {
            _puller = new PullSocket ();
            _puller.Bind ($"tcp://*:{port}");
        }

        /// <summary>
        /// set up custom event processor to handle receive event from pull socket
        /// </summary>
        /// <param name="port">socket port</param>
        public void SetReceiveHandleEvent(IReceiveHandler receiveHandler)
        {
            _receiveHandler = receiveHandler;
        }

        /// <summary>
        /// listen NetMQ pull socket
        /// </summary>   
        public void Listen () 
        {
            if (_receiveHandler is null) {
                throw new ArgumentNullException (nameof (_receiveHandler));
            }

            _cancellationTokenSource = new CancellationTokenSource ();
            var cancelToken = _cancellationTokenSource.Token;
            var task = Task.Factory.StartNew (
                (t) => Receive(t),
                cancelToken,
                cancelToken);
        }

        private void Receive (object o) 
        {
            var ct = (CancellationToken)o;

            bool more;
            byte[] msgBuf;

            while(!ct.IsCancellationRequested)
            {
                _puller.TryReceiveFrameBytes(out msgBuf, out more);
                if (!more && msgBuf is byte[] && msgBuf.Length > 0)
                {
                    _receiveHandler.Oneceive(msgBuf);
                }   
            }
        }

        /// <summary>
        /// close NetMQ pull socket
        /// </summary>
        public void Close()
        {
            _cancellationTokenSource.Cancel();
            _receiveTask.Wait();
            _puller.Close();
        }
    }
}