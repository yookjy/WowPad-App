using System.Net.Sockets;

namespace Apps.WowPad.Sockets
{
    public class UdpSocket : System.Net.Sockets.Socket
    {
        public UdpSocket()
            : base(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp)
        {
        }

        private UdpSocket(AddressFamily addressFamily, SocketType socketType, ProtocolType protocolType)
            : base(addressFamily, socketType, protocolType)
        {
        }
    }
}
