using Apps.WowPad.Manager;
using Apps.WowPad.Model;
using Apps.WowPad.Resources;
using Apps.WowPad.Sockets;
using Apps.WowPad.Type;
using Apps.WowPad.Util;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Windows;
using System.Windows.Controls;

namespace Apps.WowPad.UI.Pages
{
    public partial class ServerListPage : PhoneApplicationPage
    {
        private const int SEARCH_TIMEOUT = 2000;

        private BackgroundWorker bw, bgWorker;

        private ObservableCollection<ServerInfo> serverInfoList;

        private List<ServerInfo> historyList;

        private UdpSocket udpSocket;

        private string defaultHostName;

        private string currentHostname;

        public ServerListPage()
        {
            InitializeComponent();

            //타이틀 설정
            txtAppName.Title = string.Format(I18n.GetString("AppName"), " " + VersionUtils.Version);

            //취소가 가능 하도록 상태로 설정
            bw = new BackgroundWorker();
            bw.WorkerSupportsCancellation = true;
            bw.DoWork += bw_DoWork;
            bw.RunWorkerCompleted += bw_RunWorkerCompleted;

            udpSocket = new UdpSocket();
            //검색결과 리스트 생성
            serverInfoList = new ObservableCollection<ServerInfo>();
            serverInfoList.CollectionChanged += serverInfoList_CollectionChanged;
                        
            //프로그레스바 숨김
            UIUtils.SetVisibility(labelProgressBar, false);
            UIUtils.SetVisibility(findProgressBar, false);
            
            //기본포트 설정
            portTxtBox.Text = Convert.ToString(SettingManager.Instance.SettingInfo.DefaultPort);

            //앱바 설정
            BuildLocalizedApplicationBar();
        }

        protected override void OnNavigatedTo(System.Windows.Navigation.NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            (SearchPivot.Parent as Pivot).SelectedIndex = SettingManager.Instance.SettingInfo.SearchDefaultPageIndex;
        }

        void bk_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            Deployment.Current.Dispatcher.BeginInvoke(() =>
            {
                hostTxtBox.Focus();
            });
        }

        void bk_DoWork(object sender, DoWorkEventArgs e)
        {
            Thread.Sleep(200);
        }

        //뒤로 [하드웨어버튼} 눌렸을때 처리 
        protected override void OnBackKeyPress(System.ComponentModel.CancelEventArgs e)
        {
            base.OnBackKeyPress(e);
            if (!ConnectionManager.Instance.IsConnected)
            {
                ConnectionManager.Instance.ConnectionInfo.CurrentServer = null;
                if (findServerBtn.IsEnabled //버튼이 활성화 상태
                && (SearchPivot.Parent as Pivot).SelectedIndex == 0 //현재 PC검색 탭인 경우
                && (ItemViewOnPage.Items.Count > 0) //리스트가 비어 있지 않다.
                && (ItemViewOnPage.SelectedIndex == -1)) //리스트가 선택되지 않았다.)
                {
                    e.Cancel = MessageBox.Show(I18n.GetString("ServerListPageBack"), I18n.GetString("AppMessageConfirm"), MessageBoxButton.OKCancel) == MessageBoxResult.Cancel;
                }
            }
        }

        void serverInfoList_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            Deployment.Current.Dispatcher.BeginInvoke(() =>
            {
                //결과 렌더링
                ItemViewOnPage.DataContext = from ServerInfo in serverInfoList select ServerInfo;
                
                if (currentHostname.Contains("."))
                {
                    string host = currentHostname.Substring(currentHostname.LastIndexOf(".") + 1);
                    if (host != "255" && serverInfoList.Count == 1)
                    {
                        ItemViewOnPage.SelectedIndex = 0;
                        TapSearchResult();
                    }
                }
            });
        }

        void bw_DoWork(object sender, DoWorkEventArgs e)
        {
            for (int i = 1; i <= 10; i++)
            {
                if ((bw.CancellationPending == true))
                {
                    e.Cancel = true;
                    break;
                }
                else
                {
                    // Perform a time consuming operation and report progress.
                    System.Threading.Thread.Sleep(300);
                }
            }
        }

        void bw_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            SearchComplete();
        }

        private void SearchComplete()
        {
            UIUtils.SetVisibility(labelProgressBar, false);
            UIUtils.SetVisibility(findProgressBar, false);

            findServerBtn.IsEnabled = true;
            portTxtBox.IsEnabled = true;
            ItemViewOnPage.IsEnabled = true;
        }
        
        private void portTxtBox_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (System.Text.RegularExpressions.Regex.IsMatch(e.Key.ToString(), "[0-9]"))
                e.Handled = false;
            else e.Handled = true;
        }

        private void portTxtBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            Boolean isEnabled = !string.IsNullOrEmpty(portTxtBox.Text.Trim());
            
            if (isEnabled)
            {
                int port = Convert.ToInt32(portTxtBox.Text.Trim());
                //멀티캐스트는 1024보다 큰 포트이어야 한다.
                if (port < 1024 || port > IPEndPoint.MaxPort)   
                {
                    isEnabled = false;
                    portTxtBox.Focus();
                }
            }

            //검색버튼 활성 비활성화
            findServerBtn.IsEnabled = isEnabled;
        }

        private void findServerBtn_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            findServer();
        }

        private void findServer()
        {
            //UDP소켓 생성
            int port = Convert.ToInt32(portTxtBox.Text.Trim());
            currentHostname = hostTxtBox.Text.Trim();

            if (!NetworkUtils.IsNetworkAvailable)
            {
                NetworkUtils.ShowWiFiSettingPage(I18n.GetString("AppMessageRequiredDataNetwork"), I18n.GetString("AppMessageNotification"));
                return;
            }

            ////서버 리스트 비우기
            serverInfoList.Clear();

            PacketInfo packetInfo = new PacketInfo();
            packetInfo.PacketType = PacketTypes.FindServer;
            packetInfo.DeviceType = DeviceTypes.None;
            byte[] buffer = PacketUtils.MakeClientPacket(packetInfo);

            DnsEndPoint dnsEp = new DnsEndPoint(currentHostname, port);
            udpSocket = new UdpSocket();
            SocketAsyncEventArgs udpSockArgs = new SocketAsyncEventArgs();
            udpSockArgs.RemoteEndPoint = dnsEp;
            udpSockArgs.SetBuffer(buffer, 0, buffer.Length);
            udpSockArgs.Completed += udpSockArgs_Completed;

            //PC검색 패킷 발송
            udpSocket.SendToAsync(udpSockArgs);
        }

        void udpSockArgs_Completed(object sender, SocketAsyncEventArgs e)
        {
            //소켓 결과가 성공이면 받을 준비
            if (e.SocketError == SocketError.Success)
            {
                //데이터 사용량 누적
                if (e.BytesTransferred > 0)
                {
                    AppLoader.CellularDataUtil.SumUsageCellularData(e.BytesTransferred);
                }

                switch (e.LastOperation)
                {
                    case SocketAsyncOperation.SendTo:
                        Deployment.Current.Dispatcher.BeginInvoke(() =>
                        {
                            //입력 비활성화
                            findServerBtn.IsEnabled = false;
                            portTxtBox.IsEnabled = false;
                            ItemViewOnPage.IsEnabled = false;
                            //진행상태 표시
                            UIUtils.SetVisibility(labelProgressBar, true);
                            UIUtils.SetVisibility(findProgressBar, true);
                            try
                            {
                                //검색 로딩창 표시
                                bw.RunWorkerAsync();
                            }
                            catch (InvalidOperationException ex)
                            {
                                System.Diagnostics.Debug.WriteLine(ex.Message);
                            }

                            int port = ((DnsEndPoint)e.RemoteEndPoint).Port;
                            byte[] buffer = new byte[128];
                            e.SetBuffer(buffer, 0, buffer.Length);
                            e.RemoteEndPoint = new IPEndPoint(IPAddress.Any, port);
                            udpSocket.ReceiveFromAsync(e);
                        });
                        break;
                    case SocketAsyncOperation.ReceiveFrom:
                        ServerInfo serverInfo = PacketUtils.ResolveServerPacket(PacketTypes.FindServer, e) as ServerInfo;
                        if (serverInfo != null)
                        {
                            Deployment.Current.Dispatcher.BeginInvoke(() =>
                            {
                                //수신된 서버 추가
                                serverInfoList.Add(serverInfo);
                                if (currentHostname.Contains("."))
                                {
                                    string host = currentHostname.Substring(currentHostname.LastIndexOf(".") + 1);
                                    if (host == "255")
                                    {
                                        //나머지 수신 대기
                                        udpSocket.ReceiveFromAsync(e);
                                    }
                                }

                                try
                                {
                                    //수신이 되면 bw 취소
                                    if (bw.WorkerSupportsCancellation)
                                    {
                                        bw.CancelAsync();
                                    }
                                }
                                catch (InvalidOperationException ex)
                                {
                                    System.Diagnostics.Debug.WriteLine(ex.Message);
                                }
                            });
                        }
                        break;
                }
            }
            else
            {
                Deployment.Current.Dispatcher.BeginInvoke(() =>
                {
                    switch (e.SocketError)
                    {
                        case SocketError.HostNotFound:
                            MessageBox.Show(I18n.GetString("AppMessageNotFoundHost"), I18n.GetString("AppMessageNotification"), MessageBoxButton.OK);
                            break;
                        case SocketError.HostUnreachable:
                        case SocketError.NetworkUnreachable:
                        case SocketError.AccessDenied:
                            MessageBox.Show(I18n.GetString("AppMessageUnreachableHost"), I18n.GetString("AppMessageNotification"), MessageBoxButton.OK);
                            break;
                        default:
                            break;
                    }
                    hostTxtBox.Focus();
                });
            }
        }

        private void ItemViewOnPage_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            if (ItemViewOnPage.SelectedIndex == -1)
            {
                return;
            }

            TapSearchResult();
        }

        private void TapSearchResult()
        {
            //현재 접속되어 있는 서버가 있다면 종료시킴.
            if (ConnectionManager.Instance.IsConnected)
            {
                ConnectionManager.Instance.DisconnectServer(null);
            }

            //선택된 서버 정보를 현재 서버로 변경
            ServerInfo serverInfo = (ServerInfo)ItemViewOnPage.SelectedItem;
            ConnectionManager.Instance.ConnectionInfo.CurrentServer = serverInfo;
            //접속코드 입력 화면으로 이동
            this.NavigationService.Navigate(new Uri(string.Format(Constant.PAGE_SERVER_CONNECT, serverInfo.ServerName), UriKind.Relative));
        }

        private void historyItemViewOnPage_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            if (historyItemViewOnPage.SelectedIndex == -1)
            {
                return;
            }

            ApplicationBar.IsVisible = true;
            EnableApplicationBarPowerButton(true);
        }

        private void BuildLocalizedApplicationBar()
        {
            //페이지의 ApplicationBar를 ApplicationBar의 새 인스턴스로 설정합니다.
            ApplicationBar = new ApplicationBar();
            ApplicationBar.Mode = ApplicationBarMode.Default;
            ApplicationBar.Opacity = 0.9;
            ApplicationBar.IsVisible = false;
            ApplicationBar.IsMenuEnabled = false;

            // 새 단추를 만들고 텍스트 값을 AppResources의 지역화된 문자열로 설정합니다.
            ApplicationBar.Buttons.Add(UIUtils.CreateAppBarIconButton(I18n.GetString("AppBarButtonTextDelete"), ResourceUri.DeleteImageUri, deleteAppbarIconBtn_Click));
            ApplicationBar.Buttons.Add(UIUtils.CreateAppBarIconButton(I18n.GetString("AppBarButtonTextConnect"), ResourceUri.ConnectImageUri, connectAppbarIconBtn_Click));
            ApplicationBar.Buttons.Add(UIUtils.CreateAppBarIconButton(I18n.GetString("AppBarButtonTextPower"), ResourceUri.PowerImageUri, powerAppbarIconBtn_Click));
        }

        private void deleteAppbarIconBtn_Click(object sender, EventArgs e)
        {
            ServerInfo serverInfo = historyItemViewOnPage.SelectedItem as ServerInfo;

            if (serverInfo != null)
            {
                if (MessageBox.Show(string.Format(I18n.GetString("HistoryPageRemoveItem"), serverInfo.ServerName),
                    I18n.GetString("AppMessageConfirm"), MessageBoxButton.OKCancel) == MessageBoxResult.OK)
                {
                    ApplicationBar.IsVisible = false;
                    HistorygManager.Instance.Remove(serverInfo);
                    LoadHistoryList();
                }
            }
        }

        private void powerAppbarIconBtn_Click(object sender, EventArgs e)
        {
            //체험판 체크
            if (VersionUtils.CheckTrialVersion())
            {
                MessageBox.Show(I18n.GetString("AppMessageTrialCannotuse") + " " + I18n.GetString("AppMessageTrialBuyInfo"));
                return;
            }
            //네트워크 연결체크
            if (!NetworkUtils.IsNetworkAvailable)
            {
                NetworkUtils.ShowWiFiSettingPage(I18n.GetString("AppMessageRequiredDataNetwork"), I18n.GetString("AppMessageNotification"));
                return;
            }
    
            ServerInfo serverInfo = (ServerInfo)historyItemViewOnPage.SelectedItem;
#if !WP7
            if (NetworkUtils.IsPrivateNetwork(serverInfo.ServerIP))
            {
                string wolIp = serverInfo.ServerIP.Substring(0, serverInfo.ServerIP.LastIndexOf(".") + 1);

                if (NetworkUtils.IsPrivateNetwork(NetworkUtils.MyIp) && NetworkUtils.MulticastIP.Contains(wolIp))
                {
                    //내부 IP의 경우 직접 브로드 캐스트
                    WakeOnLan(wolIp += "255");
                }
                else
                {
                    MessageBox.Show(I18n.GetString("HistoryPageWOLCannotRequest"), I18n.GetString("AppMessageNotification"), MessageBoxButton.OK);
                }
            }
            else
            {
#endif
                //Subnet directed Broadcast
                //접속코드 입력 화면으로 이동
                this.NavigationService.Navigate(new Uri(string.Format(Constant.PAGE_WOL_SUBNETMASK, serverInfo.ServerName, serverInfo.ServerIP, serverInfo.SubnetMask), UriKind.Relative));
#if !WP7            
            }
#endif
        }

        public void WakeOnLan(string wolIp)
        {
            if (MessageBox.Show(I18n.GetString("HistoryPageWOLConfirm"), I18n.GetString("AppMessageNotification"), MessageBoxButton.OKCancel) == MessageBoxResult.OK)
            {
                ServerInfo serverInfo = (ServerInfo)historyItemViewOnPage.SelectedItem;
                NetworkUtils.WakeOnLan(wolIp, serverInfo.MacAddressList);
                MessageBox.Show(I18n.GetString("HistoryPageWOLRequest"), I18n.GetString("AppMessageNotification"), MessageBoxButton.OK);
                //Power on request. Please check with your PC
                EnableApplicationBarPowerButton(false);
            }
        }

        public void SaveSubnetMask(string subnetmask)
        {
            try
            {
                //IP확인
                IPAddress.Parse(subnetmask);
                //subnet mask 저장
                ServerInfo serverInfo = historyItemViewOnPage.SelectedItem as ServerInfo;
                serverInfo.SubnetMask = subnetmask;
                HistorygManager.Instance.Update(historyList);
            }
            catch (Exception e)
            {
                System.Diagnostics.Debug.WriteLine(e.Message);
            }
        }

        private void connectAppbarIconBtn_Click(object sender, EventArgs e)
        {
            //네트워크 연결체크
            if (!NetworkUtils.IsNetworkAvailable)
            {
                NetworkUtils.ShowWiFiSettingPage(I18n.GetString("AppMessageRequiredDataNetwork"), I18n.GetString("AppMessageNotification"));
                return;
            }

            //현재 접속되어 있는 서버가 있다면 종료시킴.
            if (ConnectionManager.Instance.IsConnected)
            {
                ConnectionManager.Instance.DisconnectServer(null);
            }

            //선택된 서버 정보를 현재 서버로 변경
            ServerInfo serverInfo = (ServerInfo)historyItemViewOnPage.SelectedItem;
            ConnectionManager.Instance.ConnectionInfo.CurrentServer = serverInfo;

            ConnectionManager.Instance.ConnectServer(
                SettingManager.Instance.SettingInfo.DeviceType,
                new CallbackHandler((object serverExtraInfo) =>
                {
                    Deployment.Current.Dispatcher.BeginInvoke(() =>
                    {

                        if (serverExtraInfo is ServerExtraInfo)
                        {
                            ServerExtraInfo svrExtInfo = serverExtraInfo as ServerExtraInfo;
                            AppStateUtils.Set(Constant.KEY_SERVER_EXTRA_INFO, svrExtInfo);
                            this.NavigationService.GoBack();
                        }
                        else if (serverExtraInfo is SocketError)
                        {
                            if ((SocketError)serverExtraInfo == SocketError.TimedOut)
                            {
                                MessageBox.Show(I18n.GetString("AppMessageNotAbleConnectPC"));
                                (SearchPivot.Parent as Pivot).SelectedIndex = 0;
                            }
                        }
                        else
                        {
                            MessageBox.Show(I18n.GetString("AppMessageAccessCodeIncorrect"));
                            //접속코드 입력 화면으로 이동
                            this.NavigationService.Navigate(new Uri(string.Format(Constant.PAGE_SERVER_CONNECT, serverInfo.ServerName), UriKind.Relative));
                        }
                    });
                }));
        }

        private void Pivot_LoadingPivotItem_1(object sender, PivotItemEventArgs e)
        {
            if (ApplicationBar != null)
            {
                ApplicationBar.IsVisible = false;
            }

            if (e.Item.Name == "HistoryPivot")
            {
                LoadHistoryList();
            }
            else if (e.Item.Name == "SearchPivot")
            {
                //호스트 내용이 비었으면 기본호스트 설정
                if (string.IsNullOrEmpty(hostTxtBox.Text.Trim()))
                {
                    defaultHostName = NetworkUtils.MulticastIP;
                    hostTxtBox.Text = defaultHostName;
                }

                if (NetworkUtils.IsNetworkAvailable && string.IsNullOrEmpty(hostTxtBox.Text.Trim()))
                {
                    findServerBtn.IsEnabled = false;

                    //포커스 이동 속도가 너무 빠르면 키보드가 텍스트박스를 덮는 현상이 생기므로 시간차 공격!
                    bgWorker = new BackgroundWorker();
                    bgWorker.DoWork += bk_DoWork;
                    bgWorker.RunWorkerCompleted += bk_RunWorkerCompleted;
                    bgWorker.RunWorkerAsync();
                }
            }
        }

        private void EnableApplicationBarPowerButton(bool isEnabled)
        {
            foreach (ApplicationBarIconButton appBarIconBtn in ApplicationBar.Buttons)
            {
                //PC 검색 버튼
                if (appBarIconBtn.IconUri.OriginalString == ResourceUri.IMG_APPBAR_POWER)
                {
                    appBarIconBtn.IsEnabled = isEnabled;
                    break;
                }
            }
        }

        private void LoadHistoryList()
        {
            //히스토리 로드
            historyList = HistorygManager.Instance.ServerInfoList;
            //리스트 로드
            //historyItemViewOnPage.DataContext = from ServerInfo in historyList orderby ServerInfo.LastDateTime descending select ServerInfo;
            historyItemViewOnPage.DataContext = from ServerInfo in historyList select ServerInfo;
        }

        private void LayoutRoot_Loaded(object sender, RoutedEventArgs e)
        {
            if (findServerBtn.IsEnabled //버튼이 활성화 상태
                && (SearchPivot.Parent as Pivot).SelectedIndex == 0 //현재 PC검색 탭인 경우
                && (ItemViewOnPage.SelectedIndex == -1))
            {
                findServer();
            }
        }

        private void hostTxtBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            string txtHost = hostTxtBox.Text.Trim();
            findServerBtn.IsEnabled = (!string.IsNullOrEmpty(txtHost) && txtHost.Substring(txtHost.Length - 1) != ".");
        }

    }
}