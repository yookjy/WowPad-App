using Apps.WowPad.Model;
using Apps.WowPad.Resources;
using Apps.WowPad.Sockets;
using Apps.WowPad.Type;
using Apps.WowPad.Util;
using System;
using System.ComponentModel;
using System.Net.Sockets;
using System.Threading;
using System.Windows;

namespace Apps.WowPad.Manager
{
    public class ConnectionManager
    {
        private static ConnectionManager instance;

        public static ConnectionManager Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new ConnectionManager();
                }
                return instance;
            }
        }

        private ConnectionManager() 
        {
            packetInfo = new PacketInfo();
            ConnectionInfo = new ConnectionInfo();
        }

        public ConnectionInfo ConnectionInfo { get; set; }

        private static ManualResetEvent _clientDone = new ManualResetEvent(false);

        private PacketInfo packetInfo;

        public Boolean IsConnected
        {
            get
            {
                return this.ConnectionInfo != null && this.ConnectionInfo.CurrentServer != null && this.ConnectionInfo.CurrentServer.IsConnected;
            }
        }

        void ConnectionManager_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            MessageBox.Show(e.PropertyName);
        }

        public Object ConnectServer(DeviceTypes deviceType, CallbackHandler connectResult)
        {
            if (!NetworkUtils.IsNetworkAvailable)
            {
                NetworkUtils.ShowWiFiSettingPage(I18n.GetString("AppMessageRequiredDataNetwork"), I18n.GetString("AppMessageNotification"));
                return SocketError.NotConnected;
            }

            Object result = SocketError.TimedOut;
            ConnectionInfo.TcpSocket = new TcpSocket();
            ConnectionInfo.TcpSocket.NoDelay = true;

            packetInfo.Clear();
            packetInfo.PacketType = PacketTypes.Authentication;
            packetInfo.DeviceType = deviceType;
            packetInfo.AccessCode = ConnectionInfo.CurrentServer.AccessCode;
            
            SocketAsyncEventArgs tcpSocketArgs = new SocketAsyncEventArgs();
            tcpSocketArgs.RemoteEndPoint = ConnectionInfo.CurrentServer.TcpIPEndPoint;
            tcpSocketArgs.UserToken = new UserToken(packetInfo, connectResult);
            tcpSocketArgs.Completed += new EventHandler<SocketAsyncEventArgs>((object s, SocketAsyncEventArgs e) =>
            {
                result = e.SocketError;
                // Signal that the request is complete, unblocking the UI thread
                _clientDone.Set();

                if (e.SocketError == SocketError.Success)
                {
                    //데이터 사용량 누적
                    if (e.BytesTransferred > 0)
                    {
                        AppLoader.CellularDataUtil.SumUsageCellularData(tcpSocketArgs.BytesTransferred);
                    }

                    UserToken userToken = null;
                    switch (e.LastOperation)
                    {
                        case SocketAsyncOperation.Connect:
                            if (e.ConnectSocket != null)
                            {
                                if (e.ConnectSocket.Connected)
                                {
                                    byte[] packet = PacketUtils.MakeClientPacket(e.UserToken);
                                    e.SetBuffer(packet, 0, packet.Length);
                                    e.ConnectSocket.SendAsync(e);
                                }
                            }
                            else
                            {
                                userToken = e.UserToken as UserToken;
                                if (userToken.callback != null)
                                {
                                    ConnectionInfo.CurrentServer.IsConnected = false;
                                    ((CallbackHandler)userToken.callback)(null);
                                }
                            }
                            break;
                        case SocketAsyncOperation.Send:
                            userToken = e.UserToken as UserToken;
                            if (userToken.PacketInfo.PacketType == PacketTypes.Authentication)
                            {
                                byte[] packet = new byte[1024];
                                e.SetBuffer(packet, 0, packet.Length);
                                e.ConnectSocket.ReceiveAsync(e);
                            }
                            break;
                        case SocketAsyncOperation.Receive:
                            userToken = e.UserToken as UserToken;
                            ServerExtraInfo serverExtraInfo = (ServerExtraInfo)PacketUtils.ResolveServerPacket(PacketTypes.Authentication, e);

                            ConnectionInfo.CurrentServer.IsConnected = serverExtraInfo != null;
                            //결과 UI반영
                            if (userToken.callback != null)
                            {
                                ((CallbackHandler)userToken.callback)(serverExtraInfo);
                            }
                            //System.Diagnostics.Debug.WriteLine("접속 소켓 실행 결과 : " + result.ToString());
                            break;
                    }
                }
            });
            _clientDone.Reset();
            ConnectionInfo.TcpSocket.ConnectAsync(tcpSocketArgs);
            _clientDone.WaitOne(5000);

            //에러가 타임 아웃인 경우 강제적으로 콜백을 호출한다.
            if ((SocketError)result == SocketError.TimedOut)
            {
                connectResult(SocketError.TimedOut);
            }

            return result;
        }

        public void DisconnectServer(CallbackHandler callbackHandler)
        {
            ConnectionInfo.TcpSocket.Close();
            ConnectionInfo.CurrentServer.IsConnected = false;
            if (callbackHandler != null)
            {
                callbackHandler(System.Net.Sockets.SocketError.NotConnected);
            }
        }

        public Object CheckConnection(SettingInfo settingInfo, CallbackHandler checkConnectionResult)
        {
            if (!NetworkUtils.IsNetworkAvailable)
            {
                NetworkUtils.ShowWiFiSettingPage(I18n.GetString("AppMessageRequiredDataNetwork"), I18n.GetString("AppMessageNotification"));
                return SocketError.NotConnected;
            }

            Object result = SocketError.TimedOut;

            packetInfo.Clear();
            packetInfo.PacketType = PacketTypes.CheckConnection;
            packetInfo.DeviceType = settingInfo.DeviceType;
            packetInfo.AccessCode = ConnectionInfo.CurrentServer.AccessCode;
            
            SocketAsyncEventArgs tcpSocketArgs = new SocketAsyncEventArgs();
            tcpSocketArgs.RemoteEndPoint = ConnectionInfo.CurrentServer.TcpIPEndPoint;
            tcpSocketArgs.UserToken = new UserToken(packetInfo, checkConnectionResult);
            byte[] packet = PacketUtils.MakeClientPacket(packetInfo);
            tcpSocketArgs.SetBuffer(packet, 0, packet.Length);
            tcpSocketArgs.Completed += new EventHandler<SocketAsyncEventArgs>((object s, SocketAsyncEventArgs e) =>
            {
                result = e.SocketError;

                if (e.SocketError == SocketError.Success)
                {
                    //데이터 사용량 누적
                    if (e.BytesTransferred > 0)
                    {
                        AppLoader.CellularDataUtil.SumUsageCellularData(tcpSocketArgs.BytesTransferred);
                    }

                    UserToken userToken = null;
                    switch (e.LastOperation)
                    {
                        case SocketAsyncOperation.Send:
                            userToken = e.UserToken as UserToken;
                            //if (e.Buffer.Length == 0)
                            //{
                            //    System.Diagnostics.Debug.WriteLine("뭥미");
                            //}
                            packet = new byte[1024];
                            e.SetBuffer(packet, 0, packet.Length);
                            ConnectionInfo.TcpSocket.ReceiveAsync(e);
                            break;
                        case SocketAsyncOperation.Receive:
                            userToken = e.UserToken as UserToken;
                            ServerExtraInfo serverStatusInfo = (ServerExtraInfo)PacketUtils.ResolveServerPacket(PacketTypes.CheckConnection, e);

                            if (serverStatusInfo == null)
                            {
                                if (ConnectionInfo.TcpSocket.Connected)
                                {
                                    //끊김 확인용
                                    e.SetBuffer(0, 0);
                                    ConnectionInfo.TcpSocket.SendAsync(e);
                                }
                                else
                                {
                                    result = SocketError.ConnectionAborted;
                                    _clientDone.Set();
                                }
                            }
                            else
                            {
                                _clientDone.Set();
                                //결과 UI반영
                                if (userToken.callback != null)
                                {
                                    ((CallbackHandler)userToken.callback)(serverStatusInfo);
                                }
                            }
                            break;
                    }
                }
                else
                {
                    _clientDone.Set();
                    System.Diagnostics.Debug.WriteLine("끊김 재확인 => 소켓 에러 : " + result);
                }
            });

            if (IsConnected)
            {
                _clientDone.Reset();
                ConnectionInfo.TcpSocket.NoDelay = true;
                ConnectionInfo.TcpSocket.SendAsync(tcpSocketArgs);
                _clientDone.WaitOne(3000);
            }

            if ((SocketError)result != SocketError.Success)
            {
                System.Diagnostics.Debug.WriteLine("커넥션 체크 중 소켓 에러 : " + (SocketError)result);
                if (ConnectionInfo.CurrentServer.IsConnected)
                {
                    DisconnectServer(checkConnectionResult);
                }
            }
            return result;
        }

        public void SaveLastConnectedServer(DeviceTypes deviceType)
        {
            SettingManager.Set(SettingManager.KEY_SERVER_NAME, ConnectionInfo.CurrentServer.ServerName);
            SettingManager.Set(SettingManager.KEY_SERVER_IP, ConnectionInfo.CurrentServer.ServerIP);
            SettingManager.Set(SettingManager.KEY_SERVER_TCP_PORT, ConnectionInfo.CurrentServer.TcpPort);
            SettingManager.Set(SettingManager.KEY_SERVER_UDP_PORT, ConnectionInfo.CurrentServer.UdpPort);
            SettingManager.Set(SettingManager.KEY_SERVER_ACCESS_CODE, ConnectionInfo.CurrentServer.AccessCode);
            SettingManager.Update();

            packetInfo.Clear();
            packetInfo.PacketType = PacketTypes.AutoConnect;
            packetInfo.DeviceType = deviceType;
            packetInfo.AccessCode = ConnectionInfo.CurrentServer.AccessCode;

            SocketAsyncEventArgs tcpSocketArgs = new SocketAsyncEventArgs();
            tcpSocketArgs.RemoteEndPoint = ConnectionInfo.CurrentServer.TcpIPEndPoint;

            //기존 패킷에 패킷타입만 바꿔서, 패킷 새로 생성
            byte[] packet = PacketUtils.MakeClientPacket(packetInfo);
            tcpSocketArgs.SetBuffer(packet, 0, packet.Length);
            ConnectionInfo.TcpSocket.SendAsync(tcpSocketArgs);
        }        
    }
}
