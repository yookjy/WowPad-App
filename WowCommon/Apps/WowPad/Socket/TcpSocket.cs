using System.Net.Sockets;

namespace Apps.WowPad.Sockets
{
    public class TcpSocket : System.Net.Sockets.Socket
    {
        public TcpSocket()
            : base(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp)
        {
        }

        private TcpSocket(AddressFamily addressFamily, SocketType socketType, ProtocolType protocolType)
            : base(addressFamily, socketType, protocolType)
        {
        }
    }
}
