using System.Collections.Generic;
using System.Linq;

namespace eventMessage.UnitTests
{
    public class TestPullHandler : IPullHandler
    {
        public byte[] Block { get; set; }

        public void OnReceive(byte[] buffer)
        {
            Block = buffer.ToArray();
        }
    }

    public class TestSubscribeHandler : ISubscribeHandler
    {
        public List<byte[]> Block { get; set; }

        public void OnReceive(List<byte[]> buffer)
        {
            Block = buffer;
        }
    }
}