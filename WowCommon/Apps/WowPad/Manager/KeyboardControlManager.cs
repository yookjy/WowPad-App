using Apps.WowPad.Model;
using Apps.WowPad.Sockets;
using Apps.WowPad.Type;
using Apps.WowPad.Util;
using System;
using System.Net.Sockets;

namespace Apps.WowPad.Manager
{
    public class KeyboardControlManager
    {
        private static KeyboardControlManager instance;

        public static KeyboardControlManager Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new KeyboardControlManager();
                }
                return instance;
            }
        }

        public ConnectionInfo ConnectionInfo { get; set; }
        
        public Boolean KerPress(KeyboardInfo keyboardInfo)
        {
            Boolean isRequested = false;
            
            if (ConnectionInfo != null && ConnectionInfo.CurrentServer.IsConnected)
            {
                byte[] packet = null;
                SocketAsyncEventArgs socketArg = new SocketAsyncEventArgs();
                PacketInfo packetInfo = new PacketInfo();
                packetInfo.AccessCode = ConnectionInfo.CurrentServer.AccessCode;

                packetInfo.PacketType = PacketTypes.Keyboard;
                packetInfo.DeviceType = DeviceTypes.Keyboard;
                packetInfo.KeyboardInfo = keyboardInfo;

                packet = PacketUtils.MakeClientPacket(packetInfo);
                socketArg.SetBuffer(packet, 0, packet.Length);
                socketArg.RemoteEndPoint = ConnectionInfo.CurrentServer.UdpIPEndPoint;

                UdpSocket udpSocket = new UdpSocket();
                isRequested = udpSocket.SendToAsync(socketArg);

                //데이터 사용량 누적
                if (socketArg.BytesTransferred > 0)
                {
                    AppLoader.CellularDataUtil.SumUsageCellularData(socketArg.BytesTransferred);
                }
            }

            return isRequested;
        }

        public Boolean KeyRelease(KeyboardInfo keyboardInfo)
        {
            Boolean isRequested = false;

            if (ConnectionInfo != null && ConnectionInfo.CurrentServer.IsConnected)
            {
                byte[] packet = null;
                SocketAsyncEventArgs socketArg = new SocketAsyncEventArgs();
                PacketInfo packetInfo = new PacketInfo();
                packetInfo.AccessCode = ConnectionInfo.CurrentServer.AccessCode;

                packetInfo.PacketType = PacketTypes.Keyboard;
                packetInfo.DeviceType = DeviceTypes.Keyboard;
                packetInfo.KeyboardInfo = keyboardInfo;

                packet = PacketUtils.MakeClientPacket(packetInfo);
                socketArg.SetBuffer(packet, 0, packet.Length);
                socketArg.RemoteEndPoint = ConnectionInfo.CurrentServer.UdpIPEndPoint;

                UdpSocket udpSocket = new UdpSocket();
                isRequested = udpSocket.SendToAsync(socketArg);

                //데이터 사용량 누적
                if (socketArg.BytesTransferred > 0)
                {
                    AppLoader.CellularDataUtil.SumUsageCellularData(socketArg.BytesTransferred);
                }
            }

            return isRequested;
        }
    }
}
