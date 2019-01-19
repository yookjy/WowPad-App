using Apps.WowPad.Model;
using Apps.WowPad.Resources;
using Apps.WowPad.Sockets;
using Apps.WowPad.Type;
using Apps.WowPad.Util;
using System;
using System.ComponentModel;
using System.Net;
using System.Net.Sockets;
using System.Threading;

namespace Apps.WowPad.Manager
{
    public class ImageReceiveFailedEventArgs : EventArgs
    {
        public bool NeedReconnect { get; set; }
    }

    public class PointingControlManager : INotifyPropertyChanged
    {
        public delegate void ChangedDeviceTypeHandler();

        public event PropertyChangedEventHandler PropertyChanged;

        public event EventHandler<ImageReceiveFailedEventArgs> ImageReceiveFailed;

        private const int PACKET_FRAME_SIZE = 24;
        
        private static ManualResetEvent _clientDone = new ManualResetEvent(false);

        private DeviceTypes deviceType;

        private ImageQualityTypes imageQualityType;

        private byte[] currImgBytes;

        private int totalSize;

        private int lastReceiveSeq;

        //안전 보조장치 : 이미지를 요청할때 최초에 요청된 이미지가 전체였다면, 이미지 받기가 끝나야 XOR로 요청할 수있다.
        private bool isRefreshAll; 

        private UdpSocket pointingUdpSocket;

        private UdpSocket imageUdpSocket;

        private PacketInfo movePacket;

        private PacketInfo imagePacket;

        //스크린샷 화면 여부
        public bool IsRealTimeScreen { get; set; }

        public ConnectionInfo ConnectionInfo { get; set; }       

        public DeviceTypes DeviceType
        {
            get
            {
                return this.deviceType;
            }
            set
            {
                //화면 백스크린 여부
                IsRealTimeScreen = (value == DeviceTypes.TouchScreen);

                DeviceTypes prevType = this.deviceType;
                this.deviceType = value;

                //값이 다를때만 설정
                if (prevType != value)
                {
                    OnChangeDeviceType(value);
                }
            }
        }

        public ImageQualityTypes ImageQualityType
        {
            get
            {
                return this.imageQualityType;
            }
            set
            {
                ImageQualityTypes prevType = this.imageQualityType;
                this.imageQualityType = value;

                //값이 다를때만 설정
                if (prevType != value)
                {
                    OnChangeImageQualityType(value);
                }
            }
        }

        public SettingInfo SettingInfo { get; set; }

        private static PointingControlManager instance;

        public static PointingControlManager Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new PointingControlManager();
                }
                return instance;
            }
        }

        private PointingControlManager()
        {
            this.currImgBytes = null;
            this.totalSize = 0;
            this.movePacket = new PacketInfo();
            this.imagePacket = new PacketInfo();
        }

        private void OnChangeDeviceType(DeviceTypes deviceType)
        {
            if (ConnectionInfo.CurrentServer != null)
            {
                PacketInfo packetInfo = new PacketInfo();
                packetInfo.PacketType = PacketTypes.DeviceMode;
                packetInfo.DeviceType = deviceType;
                packetInfo.AccessCode = ConnectionInfo.CurrentServer.AccessCode;

                byte[] packet = packetInfo.CachedPacket;
                SocketAsyncEventArgs socketArg = new SocketAsyncEventArgs();
                socketArg.SetBuffer(packet, 0, packet.Length);
                ConnectionInfo.TcpSocket.NoDelay = true;
                ConnectionInfo.TcpSocket.SendAsync(socketArg);

                //데이터 사용량 누적
                if (socketArg.BytesTransferred > 0)
                {
                    AppLoader.CellularDataUtil.SumUsageCellularData(socketArg.BytesTransferred);
                }
            }

            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs("DeviceType"));
            }
        }

        private void OnChangeImageQualityType(ImageQualityTypes imageQualityType)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs("ImageQualityType"));
            }
        }

        private void OnImageReceiveFailed(bool needReconnect)
        {
            if (ImageReceiveFailed != null)
            {
                ImageReceiveFailed(this, new ImageReceiveFailedEventArgs() {NeedReconnect = needReconnect});
            }
        }

        public Boolean PressedVirtualButton(ButtonTypes button)
        {
            Boolean isRequested = false;

            if (ConnectionInfo.CurrentServer.IsConnected)
            {
                byte[] packet = null;
                SocketAsyncEventArgs socketArg = new SocketAsyncEventArgs();
                PacketInfo packetInfo = new PacketInfo();
                packetInfo.AccessCode = ConnectionInfo.CurrentServer.AccessCode;
                packetInfo.PacketType = PacketTypes.VirtualButton;
                packetInfo.ButtonType = button;
                packetInfo.DeviceType = DeviceType;

                packet = PacketUtils.MakeClientPacket(packetInfo);
                socketArg.SetBuffer(packet, 0, packet.Length);
                ConnectionInfo.TcpSocket.NoDelay = true;
                isRequested = ConnectionInfo.TcpSocket.SendAsync(socketArg);

                //데이터 사용량 누적
                if (socketArg.BytesTransferred > 0)
                {
                    AppLoader.CellularDataUtil.SumUsageCellularData(socketArg.BytesTransferred);
                }
            }

            return isRequested;
        }
     
        public Boolean MoveTouch(TouchInfo[] touchInfos)
        {
            Boolean isSuccess = false;
            if (ConnectionInfo.CurrentServer.IsConnected)
            {
                byte[] packet = null;

                if (pointingUdpSocket == null)
                {
                    pointingUdpSocket = new UdpSocket();
                }

                SocketAsyncEventArgs udpSocketArg = new SocketAsyncEventArgs();
                movePacket.Clear();
                movePacket.DeviceType = DeviceType;
                movePacket.ButtonType = SettingInfo.UseExtendButton ? ButtonTypes.Navigation : ButtonTypes.None;
                movePacket.AccessCode = ConnectionInfo.CurrentServer.AccessCode;
                movePacket.PacketType = PacketTypes.Coordinates;
                movePacket.TouchInfos = touchInfos;

                packet = PacketUtils.MakeClientPacket(movePacket);
                //패킷 전송
                udpSocketArg.RemoteEndPoint = ConnectionInfo.CurrentServer.UdpIPEndPoint;
                udpSocketArg.SetBuffer(packet, 0, packet.Length);
                pointingUdpSocket.SendToAsync(udpSocketArg);
                
                //데이터 사용량 누적
                if (udpSocketArg.BytesTransferred > 0)
                {
                    AppLoader.CellularDataUtil.SumUsageCellularData(udpSocketArg.BytesTransferred);
                }    
            }

            return isSuccess;
        }

        public Boolean UpdateBackgroundImage(bool ignoreXorFirstReceive, CallbackHandler requestImageCallbackHandler)
        {
            Boolean ret = false;
            isRefreshAll = ignoreXorFirstReceive;

            if (ConnectionInfo.CurrentServer != null && ConnectionInfo.CurrentServer.IsConnected)
            {
                //변수 초기화
                this.currImgBytes = null;
                this.totalSize = 0;
                this.lastReceiveSeq = 0;
                                
                imagePacket.Clear();
                imagePacket.PacketType = PacketTypes.RequestImage;
                imagePacket.DeviceType = DeviceType;
                imagePacket.ButtonType = ButtonTypes.None;
                imagePacket.AccessCode = ConnectionInfo.CurrentServer.AccessCode;
                imagePacket.Seq = lastReceiveSeq;
                imagePacket.Flag = ignoreXorFirstReceive ? 1 : 0;
                imagePacket.ImageQualityType = ImageQualityType;
                byte[] packet = imagePacket.CachedPacket;

                SocketAsyncEventArgs args = new SocketAsyncEventArgs();
                args.UserToken = new UserToken(imagePacket, requestImageCallbackHandler);
                args.SetBuffer(packet, 0, packet.Length);
                args.Completed += args_Completed;
                args.RemoteEndPoint = ConnectionInfo.CurrentServer.UdpIPEndPoint;

                imageUdpSocket = new UdpSocket();
                imageUdpSocket.ReceiveBufferSize = Constant.UDP_BUFFER_SIZE;
                imageUdpSocket.SendToAsync(args);

                ret = true;
            }
            return ret;
        }
        
        void args_Completed(object sender, SocketAsyncEventArgs e)
        {
            if (ConnectionInfo.CurrentServer != null && ConnectionInfo.CurrentServer.IsConnected)
            {
                SocketAsyncEventArgs args = null;
                UserToken userToken = null;
                PacketInfo packetInfo = null;

                if (e.SocketError != SocketError.Success)
                {
                    return;
                }

                //데이터 사용량 누적
                if (e.BytesTransferred > 0)
                {
                    AppLoader.CellularDataUtil.SumUsageCellularData(e.BytesTransferred);
                }

                //이미지를 받다가 커넥션이 끊길때 에러 방지를 위해
                if (ConnectionInfo.TcpSocket.Connected && ConnectionInfo.CurrentServer != null
                    && ConnectionInfo.CurrentServer.IsConnected
                    && this.IsRealTimeScreen)
                {
                    switch (e.LastOperation)
                    {
                        case SocketAsyncOperation.SendTo:
                            if (imageUdpSocket != null)
                            {
                                userToken = e.UserToken as UserToken;
                                packetInfo = userToken.PacketInfo;

                                byte[] buff = new byte[Constant.UDP_BUFFER_SIZE];
                                UserToken ut = new UserToken(packetInfo, userToken.callback);

                                args = new SocketAsyncEventArgs();
                                args.UserToken = ut;
                                args.RemoteEndPoint =  new IPEndPoint(IPAddress.Parse(ConnectionInfo.CurrentServer.ServerIP), ConnectionInfo.CurrentServer.UdpPort); ;
                                args.SetBuffer(buff, 0, Constant.UDP_BUFFER_SIZE);
                                args.Completed += args_Completed;
                                
                                _clientDone.Reset();
                                imageUdpSocket.ReceiveFromAsync(args);
                                bool ret = _clientDone.WaitOne(1000);    //1초 동안 수신을 대기함.

                                if (!ret || ImageQualityType != packetInfo.ImageQualityType)
                                {
                                    //실패한 소켓 수신객체의 콜백 삭제
                                    args.Completed -= args_Completed;
                                    //이미지 수신 실패 이벤트 발생
                                    OnImageReceiveFailed(ImageQualityType != packetInfo.ImageQualityType);
                                }
                            }
                            break;
                        case SocketAsyncOperation.ReceiveFrom:
                            if (e.BytesTransferred > PACKET_FRAME_SIZE)
                            {
                                byte[] rcvbuff = e.Buffer;
                                int header = BitConverter.ToInt32(rcvbuff, 0);
                                int totSize = BitConverter.ToInt32(rcvbuff, 4);
                                int packetSize = BitConverter.ToInt32(rcvbuff, 8);
                                int seq = BitConverter.ToInt32(rcvbuff, 12);
                                bool isXor = Convert.ToBoolean(BitConverter.ToInt32(rcvbuff, 16));
                                int footer = BitConverter.ToInt32(rcvbuff, 20 + packetSize);

                                if (totSize == 0 || packetSize == 0)
                                {
                                    System.Diagnostics.Debug.WriteLine("패킷이 0인 경우 => 발생하면 안되는데...");
                                    Thread.Sleep(300);
                                    break;
                                }

                                if (totalSize != totSize && totSize > 0 && totalSize > 0)
                                {
                                    System.Diagnostics.Debug.WriteLine("수신 실패이후 다시 시작전에 받아진 데이터 .... 무시하고 넘어감");
                                    break;
                                }
                                    //System.Diagnostics.Debug.WriteLine(seq + "번 이미지 수신");

                                if (header == Constant.PACKET_VELOSTEP_HEADER   //헤더 체크
                                    && totalSize < 10 * 1024 * 1024 // 10MB미만만 허용
                                    && packetSize > 0 && seq >= 0
                                    && footer == Constant.PACKET_VELOSTEP_FOOTER)
                                {
                                    if (seq == 0 || currImgBytes == null)
                                    {
                                        totalSize = totSize;
                                        currImgBytes = new byte[totalSize];
                                    }

                                    //이미지 데이터 복사
                                    Buffer.BlockCopy(e.Buffer, 20, currImgBytes, (Constant.UDP_BUFFER_SIZE - PACKET_FRAME_SIZE) * seq, packetSize);
                                    //System.Diagnostics.Debug.WriteLine("전송된길이:{0}, 시퀀스:{1}, 패킷길이:{2}, 현재쓸포인터{3}", new object[] { e.BytesTransferred, seq, packetSize, (Constant.UDP_BUFFER_SIZE - PACKET_FRAME_SIZE) * seq });

                                    if (seq * (Constant.UDP_BUFFER_SIZE - PACKET_FRAME_SIZE) + packetSize >= totalSize)
                                    {
                                        //완료
                                        seq = 0;
                                        totalSize = 0;
                                        isRefreshAll = false;
                                        
                                        userToken = e.UserToken as UserToken;
                                        //이미지 화면 표시 요청
                                        ((CallbackHandler)userToken.callback)(new ImageInfo() { IsXorImage = isXor, ImageBytes = currImgBytes });
                                    }
                                    else
                                    {
                                        seq++;
                                    }

                                    //이전 토큰
                                    userToken = e.UserToken as UserToken;
                                    //신규 패킷
                                    packetInfo = new PacketInfo();
                                    packetInfo.PacketType = PacketTypes.RequestImage;
                                    packetInfo.DeviceType = DeviceType;
                                    packetInfo.ButtonType = ButtonTypes.None;
                                    packetInfo.AccessCode = ConnectionInfo.CurrentServer.AccessCode;
                                    //seq가 0인 경우만 이미지 품질 재적용
                                    packetInfo.ImageQualityType = seq == 0 ? ImageQualityType : userToken.PacketInfo.ImageQualityType;
                                    packetInfo.Seq = seq;
                                    packetInfo.Flag = isRefreshAll ? 1 : 0;    //xor auto
                                    byte[] packet = packetInfo.CachedPacket;

                                    args = new SocketAsyncEventArgs();
                                    args.UserToken = new UserToken(packetInfo, userToken.callback);
                                    args.SetBuffer(packet, 0, packet.Length);
                                    args.Completed += args_Completed;
                                    args.RemoteEndPoint = e.RemoteEndPoint;

                                    //이벤트 시그널
                                    _clientDone.Set();
                                    //다음 받기 요청
                                    imageUdpSocket.SendToAsync(args);
                                    lastReceiveSeq = seq;

                                    //if (seq > 0)
                                    //{
                                    //    //System.Diagnostics.Debug.WriteLine(lastReceiveSeq + "번 이미지 요청.");
                                    //}

                                    //if (seq == 0)
                                    //{
                                    //    System.Diagnostics.Debug.WriteLine("다시시작.......................");
                                    //    System.Diagnostics.Debug.WriteLine(stw.ElapsedMilliseconds);
                                    //}
                                    //else
                                    //{
                                    //    System.Diagnostics.Debug.WriteLine(stw.ElapsedMilliseconds);
                                    //}
                                }
                                else
                                {
                                    System.Diagnostics.Debug.WriteLine("패킷이 잘못된 경우");
                                }
                            }
                            else
                            {
                                System.Diagnostics.Debug.WriteLine("화면 변경 없음");
                            }
                            break;
                    }
                }
            }
        }
    }
}
