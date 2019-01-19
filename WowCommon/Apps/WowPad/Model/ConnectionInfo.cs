using System.Net.Sockets;

namespace Apps.WowPad.Model
{
    public class ConnectionInfo
    {
        public Socket TcpSocket { get; set; }

        public ServerInfo CurrentServer { get; set; }
    }
}
