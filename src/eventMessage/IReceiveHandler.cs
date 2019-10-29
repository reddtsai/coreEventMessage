using System.Collections.Generic;

namespace eventMessage
{ 
    public interface IReceiveHandler<in T>
    {
        void OnReceive(T buffer);
    }

    public interface IPullHandler : IReceiveHandler<byte[]>
    {
    }

    public interface ISubscribeHandler : IReceiveHandler<List<byte[]>>
    {
    }
}