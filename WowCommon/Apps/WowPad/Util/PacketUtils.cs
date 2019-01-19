using Apps.WowPad.Model;
using Apps.WowPad.Resources;
using Apps.WowPad.Type;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;


namespace Apps.WowPad.Util
{
    public class PacketUtils
    {
        private static byte[] packetHeader;

        private static byte[] packetFooter;

        private static byte[] deviceName;

        private static byte[] os;

        private static byte[] osMajorVersion;

        private static byte[] osMinorVersion;

        private static byte[] zeroBytes;

        private static byte[] zero16Bytes;
        
        private static byte[] PacketHeader
        {
            get
            {
                if (packetHeader == null)
                {
                    packetHeader = BitConverter.GetBytes(Constant.PACKET_VELOSTEP_HEADER);
                }
                return packetHeader;
            }
        }

        private static byte[] PacketFooter
        {
            get
            {
                if (packetFooter == null)
                {
                    packetFooter = BitConverter.GetBytes(Constant.PACKET_VELOSTEP_FOOTER);
                }
                return packetFooter;
            }
        }

        private static byte[] DeviceName
        {
            get
            {
                if (deviceName == null)
                {
                    deviceName = Encoding.UTF8.GetBytes(DeviceInfo.Name);
                }
                return deviceName;
            }
        }

        private static byte[] Os
        {
            get
            {
                if (os == null)
                {
                    os = BitConverter.GetBytes(Constant.WINDOWS_PHONE); //윈도우폰 => 3, 안드로이드폰 => 2, 아이폰 => 1
                }
                return os;
            }
        }

        private static byte[] OsMajorVersion
        {
            get
            {
                if (osMajorVersion == null)
                {
                    osMajorVersion = BitConverter.GetBytes(DeviceInfo.OsMajorVersion);
                }
                return osMajorVersion;
            }
        }

        private static byte[] OsMiorVersion
        {
            get
            {
                if (osMinorVersion == null)
                {
                    osMinorVersion = BitConverter.GetBytes(DeviceInfo.OsMinorVersion);
                }
                return osMinorVersion;
            }
        }

        public static byte[] ZeroBytes
        {
            get
            {
                if (zeroBytes == null)
                {
                    zeroBytes = BitConverter.GetBytes(0);
                }
                return zeroBytes;
            }
        }

        public static byte[] Zero16Bytes
        {
            get
            {
                if (zero16Bytes == null)
                {
                    zero16Bytes = new byte[16];
                    Buffer.BlockCopy(ZeroBytes, 0, zero16Bytes, Constant.INT_SIZE * 0, Constant.INT_SIZE);
                    Buffer.BlockCopy(ZeroBytes, 0, zero16Bytes, Constant.INT_SIZE * 1, Constant.INT_SIZE);
                    Buffer.BlockCopy(ZeroBytes, 0, zero16Bytes, Constant.INT_SIZE * 2, Constant.INT_SIZE);
                    Buffer.BlockCopy(ZeroBytes, 0, zero16Bytes, Constant.INT_SIZE * 3, Constant.INT_SIZE);
                }
                return zero16Bytes;
            }
        }

        public static byte[] MakeClientPacket(object userToken)
        {
            return MakeClientPacket(((UserToken)userToken).PacketInfo);
        }
        
        public static byte[] MakeClientPacket(PacketInfo packetInfo)
        {
            byte[] packet = null;
            int offset = 0;
            int packetSize = 0;
            
            switch (packetInfo.PacketType)
            {
                case PacketTypes.FindServer:
                    //"0"
                    packetSize = 4 * Constant.INT_SIZE;
                    packet = new byte[packetSize];
                    Buffer.BlockCopy(PacketHeader, 0, packet, 0, Constant.INT_SIZE);
                    Buffer.BlockCopy(BitConverter.GetBytes(packet.Length), 0, packet, offset += Constant.INT_SIZE, Constant.INT_SIZE);
                    Buffer.BlockCopy(BitConverter.GetBytes((int)packetInfo.PacketType), 0, packet, offset += Constant.INT_SIZE, Constant.INT_SIZE);
                    Buffer.BlockCopy(PacketFooter, 0, packet, offset += Constant.INT_SIZE, Constant.INT_SIZE);
                    break;
                case PacketTypes.Coordinates:
                    //"1"
                    packetSize = 33 * Constant.INT_SIZE;
                    packet = new byte[packetSize];
                    SetCommonPacket(packetInfo, ref packet, ref offset);
                    Buffer.BlockCopy(BitConverter.GetBytes(Convert.ToInt32(packetInfo.ButtonType == ButtonTypes.Navigation)), 0, packet, offset += Constant.INT_SIZE, Constant.INT_SIZE);
                    //좌표 정보
                    for (int i = 0; i < 5; i++)
                    {
                        if (packetInfo.TouchInfos[i].Id == 0)
                        {
                            //Buffer.BlockCopy(ZeroBytes, 0, packet, offset += Constant.INT_SIZE, Constant.INT_SIZE);
                            //Buffer.BlockCopy(ZeroBytes, 0, packet, offset += Constant.INT_SIZE, Constant.INT_SIZE);
                            //Buffer.BlockCopy(ZeroBytes, 0, packet, offset += Constant.INT_SIZE, Constant.INT_SIZE);
                            //Buffer.BlockCopy(ZeroBytes, 0, packet, offset += Constant.INT_SIZE, Constant.INT_SIZE);
                            Buffer.BlockCopy(Zero16Bytes, 0, packet, offset += Constant.INT_SIZE, Constant.INT_SIZE * 4);
                            offset += Constant.INT_SIZE * 3;
                        }
                        else
                        {
                            Buffer.BlockCopy(BitConverter.GetBytes(packetInfo.TouchInfos[i].Id), 0, packet, offset += Constant.INT_SIZE, Constant.INT_SIZE);
                            Buffer.BlockCopy(BitConverter.GetBytes((int)packetInfo.TouchInfos[i].Action), 0, packet, offset += Constant.INT_SIZE, Constant.INT_SIZE);
                            Buffer.BlockCopy(BitConverter.GetBytes(packetInfo.TouchInfos[i].X), 0, packet, offset += Constant.INT_SIZE, Constant.INT_SIZE);
                            Buffer.BlockCopy(BitConverter.GetBytes(packetInfo.TouchInfos[i].Y), 0, packet, offset += Constant.INT_SIZE, Constant.INT_SIZE);
                        }
                    }
                    Buffer.BlockCopy(PacketFooter, 0, packet, offset += Constant.INT_SIZE, Constant.INT_SIZE);
                    break;
                case PacketTypes.Keyboard:
                    //"1"
                    packetSize = 13 * Constant.INT_SIZE + 7 * Constant.BYTE_SIZE;
                    packet = new byte[packetSize];
                    SetCommonPacket(packetInfo, ref packet, ref offset);
                    //키 정보
                    Buffer.BlockCopy(BitConverter.GetBytes((int)packetInfo.KeyboardInfo.ImeKey), 0, packet, offset += Constant.INT_SIZE, Constant.INT_SIZE);
                    packet[offset += Constant.INT_SIZE] = packetInfo.KeyboardInfo.ShiftFlags;
                    Buffer.BlockCopy(packetInfo.KeyboardInfo.KeyCodes, 0, packet, offset += Constant.BYTE_SIZE, Constant.BYTE_SIZE * packetInfo.KeyboardInfo.KeyCodes.Length);
                    Buffer.BlockCopy(PacketFooter, 0, packet, offset += (Constant.BYTE_SIZE * packetInfo.KeyboardInfo.KeyCodes.Length), Constant.INT_SIZE);
                    break;
                default:
                    packetSize = 15 * Constant.INT_SIZE + DeviceName.Length;
                    packet = new byte[packetSize];
                    int extraData = (int)packetInfo.ImageQualityType;
                    if (packetInfo.PacketType == PacketTypes.VirtualButton)
                        extraData = (int)packetInfo.ButtonType;

                    SetCommonPacket(packetInfo, ref packet, ref offset);
                    Buffer.BlockCopy(BitConverter.GetBytes(extraData), 0, packet, offset += Constant.INT_SIZE, Constant.INT_SIZE);
                    Buffer.BlockCopy(BitConverter.GetBytes(packetInfo.Seq), 0, packet, offset += Constant.INT_SIZE, Constant.INT_SIZE);    //sequence
                    Buffer.BlockCopy(BitConverter.GetBytes(packetInfo.Flag), 0, packet, offset += Constant.INT_SIZE, Constant.INT_SIZE);    //flag
                    Buffer.BlockCopy(DeviceName, 0, packet, offset += Constant.INT_SIZE, DeviceName.Length);
                    Buffer.BlockCopy(PacketFooter, 0, packet, offset += DeviceName.Length, Constant.INT_SIZE);
                    break;
            }
            return packet;
        }

        public static void SetCommonPacket(PacketInfo packetInfo, ref byte[] packet, ref int offset)
        {
            Buffer.BlockCopy(PacketHeader, 0, packet, 0, Constant.INT_SIZE);
            Buffer.BlockCopy(BitConverter.GetBytes(packet.Length), 0, packet, offset += Constant.INT_SIZE, Constant.INT_SIZE);
            Buffer.BlockCopy(BitConverter.GetBytes((int)packetInfo.PacketType), 0, packet, offset += Constant.INT_SIZE, Constant.INT_SIZE);
            Buffer.BlockCopy(BitConverter.GetBytes(packetInfo.AccessCode), 0, packet, offset += Constant.INT_SIZE, Constant.INT_SIZE);
            Buffer.BlockCopy(Os, 0, packet, offset += Constant.INT_SIZE, Constant.INT_SIZE); 
            Buffer.BlockCopy(OsMajorVersion, 0, packet, offset += Constant.INT_SIZE, Constant.INT_SIZE);
            Buffer.BlockCopy(OsMiorVersion, 0, packet, offset += Constant.INT_SIZE, Constant.INT_SIZE);
            Buffer.BlockCopy(BitConverter.GetBytes(DeviceInfo.ScreenWidth), 0, packet, offset += Constant.INT_SIZE, Constant.INT_SIZE);
            Buffer.BlockCopy(BitConverter.GetBytes(DeviceInfo.ScreenHeight), 0, packet, offset += Constant.INT_SIZE, Constant.INT_SIZE);
            Buffer.BlockCopy(BitConverter.GetBytes(DeviceInfo.Battery), 0, packet, offset += Constant.INT_SIZE, Constant.INT_SIZE);
            Buffer.BlockCopy(BitConverter.GetBytes((int)packetInfo.DeviceType), 0, packet, offset += Constant.INT_SIZE, Constant.INT_SIZE);
        }
        
        public static Object ResolveServerPacket(PacketTypes packetType, Object receivedObj)
        {
            SocketAsyncEventArgs args = receivedObj as SocketAsyncEventArgs;
            Object retObj = null;  
            
            int offset = 0;
            int vsHeader = BitConverter.ToInt32(args.Buffer, offset);
            int pSize = BitConverter.ToInt32(args.Buffer, offset += Constant.INT_SIZE);
            //패킷 사이즈가 템플렛(헤더+사이즈+풋터) 보다 작으면 null 리턴
            if (pSize < Constant.INT_SIZE * 3) return retObj;
            int vsFooter = BitConverter.ToInt32(args.Buffer, pSize - Constant.INT_SIZE);

            Byte[] packet = new Byte[pSize - Constant.INT_SIZE * 3];
            Buffer.BlockCopy(args.Buffer, offset += Constant.INT_SIZE, packet, 0, packet.Length);

            if (vsHeader == Constant.PACKET_VELOSTEP_HEADER && vsFooter == Constant.PACKET_VELOSTEP_FOOTER
                && pSize == args.BytesTransferred)
            {
                offset = 0;
                switch (packetType)
                {
                    case PacketTypes.FindServer:
                        ServerInfo serverInfo = new ServerInfo();
                        IPEndPoint remoteEndPoint = args.RemoteEndPoint as IPEndPoint;

                        serverInfo.ServerIP = remoteEndPoint.Address.ToString();
                        serverInfo.UdpPort = remoteEndPoint.Port;

                        serverInfo.TcpPort = BitConverter.ToInt32(packet, offset);
                        serverInfo.ServerName = Encoding.UTF8.GetString(packet, offset += Constant.INT_SIZE, packet.Length - Constant.INT_SIZE);
                        retObj = serverInfo;
                        break;
                    case PacketTypes.Coordinates:
                        break;
                    case PacketTypes.Authentication:
                        ServerExtraInfo serverExtraInfo = null;
                        //결과 bool로 파싱하지만 실제 길이는 INT_SIZE임.
                        if (BitConverter.ToBoolean(packet, offset))
                        {
                            byte[] macAddr = null;
                            int keybdCount = BitConverter.ToInt32(packet, offset += Constant.INT_SIZE);
                            int macAddrCount = BitConverter.ToInt32(packet, offset += Constant.INT_SIZE);

                            List<KeyboardLayoutTypes> keyboardList = new List<KeyboardLayoutTypes>();
                            List<Byte[]> macAddrList = new List<Byte[]>();

                            //키보드 리스트
                            for (int i = 0; i < keybdCount; i++)
                            {
                                keyboardList.Add((KeyboardLayoutTypes)BitConverter.ToInt32(packet, offset += Constant.INT_SIZE));
                            }

                            offset += Constant.INT_SIZE;
                            //맥 주소 리스트
                            for (int i = 0; i < macAddrCount; i++)
                            {
                                macAddr = new byte[6];
                                Buffer.BlockCopy(packet, offset, macAddr, 0, macAddr.Length);
                                macAddrList.Add(macAddr);
                                offset += macAddr.Length;

                                //for (int j=0; j<6; j++)
                                //    System.Diagnostics.Debug.WriteLine(string.Format("{0:X}-", macAddr[j]));
                            }

                            if (keybdCount > 0 || macAddrCount > 0)
                            {
                                serverExtraInfo = new ServerExtraInfo();
                                serverExtraInfo.KeyboardList = keyboardList;
                                serverExtraInfo.MacAddressList = macAddrList;
                            }
                        }
                        retObj = serverExtraInfo;
                        break;
                    case PacketTypes.RequestImage:
                        break;
                    case PacketTypes.DeviceMode:
                        break;
                    case PacketTypes.VirtualButton:
                        break;
                    case PacketTypes.AutoConnect:
                        break;
                    case PacketTypes.CheckConnection:
                        ServerExtraInfo serverStatusInfo = null;
                        int nRet = BitConverter.ToInt32(packet, offset);
                        if (nRet == 1)
                        {
                            serverStatusInfo = new ServerExtraInfo();
                            serverStatusInfo.ScreenType = (ScreenTypes)BitConverter.ToInt32(packet, offset += Constant.INT_SIZE);
                            BitConverter.ToInt32(packet, offset += Constant.INT_SIZE);
                            BitConverter.ToInt32(packet, offset += Constant.INT_SIZE);
                            BitConverter.ToInt32(packet, offset += Constant.INT_SIZE);
                        }
                        retObj = serverStatusInfo;
                        break;
                    case PacketTypes.Keyboard:
                        break;
                }
            }

            return retObj;
        }
    }
}