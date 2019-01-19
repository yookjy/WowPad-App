using Apps.WowPad.Manager;
using Apps.WowPad.Model;
using Apps.WowPad.Resources;
using Apps.WowPad.Type;
using Apps.WowPad.Util;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using System;
using System.ComponentModel;
using System.Threading;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;

namespace Apps.WowPad.UI.Pages
{
    public partial class MainPage : PhoneApplicationPage
    {
        // 생성자
        public MainPage()
        {
            InitializeComponent();

            //기본 설정 로드
            DeviceInfo.Load();
            ResourceUri.Load();

            // ApplicationBar를 지역화하는 샘플 코드
            BuildLocalizedApplicationBar();

            //터치 재사용 객체 생성
            touchInfos = new TouchInfo[5];
            for (int i = 0; i < touchInfos.Length; i++)
            {
                touchInfos[i] = new TouchInfo();
            }

            Touch.FrameReported += new TouchFrameEventHandler(Touch_FrameReported);
            PointingControlManager.Instance.PropertyChanged += pointerCtrlMgr_PropertyChanged;
            PointingControlManager.Instance.ImageReceiveFailed += Instance_ImageReceiveFailed;
            PointingControlManager.Instance.DeviceType = SettingManager.Instance.SettingInfo.DeviceType;
            PointingControlManager.Instance.ImageQualityType = SettingManager.Instance.SettingInfo.ImageQualityType;

            //접속 체크 쓰레드 함수 생성 및 시작
            new Thread(CheckConnectionThreadFn).Start();

            //메세지팁 생성
            msgTip = new Apps.UI.Notification.MessageTip();
            msgTip.MaxWidth = DeviceInfo.ScreenWidth * 0.75;
            msgTip.Margin = new Thickness(15);
            msgTip.VerticalAlignment = System.Windows.VerticalAlignment.Bottom;
            LayoutRoot.Children.Add(msgTip);

            //로딩 팝업을 위한 백그라운드 워커
            loadWorker = new BackgroundWorker();
            loadWorker.DoWork += loadWorker_DoWork;
            loadWorker.RunWorkerCompleted += loadWorker_RunWorkerCompleted;
            
            //데이터 용량 알림 이벤트 핸들러 등록
            AppLoader.CellularDataUtil.NotifyUsagePerMegaBytes += CellularDataUtil_NotifyUsagePerMegaBytes;

            //기본 키보드 생성
            Apps.UI.Keyboard.Keyboard keybd = new Apps.UI.Keyboard.Keyboard();
            keybd.Margin = new Thickness(10);
            keybdLayer.Children.Add(keybd);
                                    
            //화면상의 모든 객체를 숨김으로 처리
            HideAllChildren();
            //기본 셋팅 버튼만 표시
            UIUtils.SetVisibility(btnMenu,true);
            //앱바 및 버튼 상태 업데이트
            UpdateUIElements();

            bool isShowGettingStart = SettingManager.Instance.SettingInfo.GettingStart;
            //자동 재접속
            if (NetworkUtils.IsNetworkAvailable && SettingManager.Instance.SettingInfo.AutoConnect)
            {
                ServerInfo serverInfo = SettingManager.Instance.SettingInfo.ServerInfo;
                if (!string.IsNullOrEmpty(serverInfo.ServerIP) && serverInfo.AccessCode >= 1000
                    && serverInfo.TcpPort > 0 && serverInfo.UdpPort > 0)
                {
                    isShowGettingStart = false;
                    ConnectionManager.Instance.ConnectionInfo.CurrentServer = serverInfo;
                    //접속 요청
                    txtInfomation.Text = I18n.GetString("ServerConnectPageConnecting");
                    System.Net.Sockets.SocketError result = (System.Net.Sockets.SocketError)ConnectionManager.Instance.ConnectServer(
                        SettingManager.Instance.SettingInfo.DeviceType,

                        new CallbackHandler(CallbackConnectServer));
                }
            }

            //마우스 배경화면
            backgroundTouchScreenBitmap = new WriteableBitmap(0, 0).FromContent(ResourceUri.IMG_TOUCHSCREEN_BACKGROUND);
            backgroundMouseBitmap = new WriteableBitmap(0, 0).FromContent(ResourceUri.IMG_MOUSE_BACKGROUND);
            UpdateBackgroundImage(SettingManager.Instance.SettingInfo.DeviceType == DeviceTypes.Mouse ? backgroundMouseBitmap : backgroundTouchScreenBitmap);          

            //시작하기
            UIUtils.SetVisibility(layerGettingStart, isShowGettingStart);

            if (!isShowGettingStart)
            {
                //광고 컨트롤 추가
                VersionUtils.DecideAdControl(this.LayoutRoot, this.txtInfomation);
            }
        }
                        
        public void CallbackConnectServer(object result)
        {
            if (ConnectionManager.Instance.IsConnected)
            {
                Deployment.Current.Dispatcher.BeginInvoke(() =>
                {
                    loadWorker.RunWorkerAsync(new LoadingArgument() { LoadingGrid = CreateLoadingGrid("ServerConnectPageConnecting"), LoadingType = LoadingTypes.Connection, Result = result });
                });

                //접속이 성공하고, 자동접속 설정이 On이면 자동접속을 위한 셋팅
                if (SettingManager.Instance.SettingInfo.AutoConnect)
                {
                    //접속된 서버 저장
                    ConnectionManager.Instance.SaveLastConnectedServer(PointingControlManager.Instance.DeviceType);
                }

                ServerExtraInfo srvExtraInfo = (ServerExtraInfo)result;
                //맥어드레스 저장
                ConnectionManager.Instance.ConnectionInfo.CurrentServer.MacAddressList = srvExtraInfo.MacAddressList;
                ConnectionManager.Instance.ConnectionInfo.CurrentServer.LastDateTime = DateTime.Now;
                //접속 이력 저장
                HistorygManager.Instance.Add(ConnectionManager.Instance.ConnectionInfo.CurrentServer);
            }
            else
            {
                Deployment.Current.Dispatcher.BeginInvoke(() =>
                {
                    MessageBox.Show(I18n.GetString("AppMessageNotAbleConnectPC"));
                    NavigationService.Navigate(ResourceUri.ServerListPageUri);
                });
            }
        }

        private void UpdateUIElements()
        {
            //버튼 업데이트
            //UIUtils.SetVisibility(btnScreen, ConnectionManager.Instance.IsConnected);
            UIUtils.SetVisibility(btnWindows, ConnectionManager.Instance.IsConnected);
            UIUtils.SetVisibility(btnKeyboard, ConnectionManager.Instance.IsConnected);
            UIUtils.SetVisibility(btnMenu, true);
            UIUtils.SetVisibility(txtInfomation, true, 
                ConnectionManager.Instance.IsConnected ?
                (string.IsNullOrEmpty(txtInfomation.Text)
                || (txtInfomation.Text == I18n.GetString("MainPageDisconnected"))
                || (txtInfomation.Text == I18n.GetString("ServerConnectPageConnecting")) ? I18n.GetString("MainPageConnected") : string.Empty)
                : I18n.GetString("MainPageDisconnected"));

            //앱바 업데이트
            ApplicationBarIconButton searchBtn = ApplicationBar.Buttons[nPosAppBarSearchPC] as ApplicationBarIconButton;
            ApplicationBarIconButton connectionBtn = ApplicationBar.Buttons[nPosAppBarConnect] as ApplicationBarIconButton;

            if (VersionUtils.VersionType == VersionTypes.Full)
            {
                if (ConnectionManager.Instance.IsConnected)
                {
                    //마우스/터치스크린 버튼 활성/비활성 및 텍스트 변경
                    if (PointingControlManager.Instance.DeviceType == DeviceTypes.Mouse)
                    {
                        UIUtils.UpdateAppBarIconButton(deviceAppBarIconButton, I18n.GetString("AppBarButtonTextTouchScreen"), ResourceUri.TouchscreenImageUri);
                    }
                    else
                    {
                        UIUtils.UpdateAppBarIconButton(deviceAppBarIconButton, I18n.GetString("AppBarButtonTextMouse"), ResourceUri.MouseImageUri);
                    }
                    ApplicationBar.Buttons[nPosAppBarSearchPC] = deviceAppBarIconButton;
                }
                else
                {
                    ApplicationBar.Buttons[nPosAppBarSearchPC] = searchAppBarIconButton;
                }
            }
            else
            {
                //PC찾기 버튼 활성/비활성화
                searchBtn.IsEnabled = !ConnectionManager.Instance.IsConnected;
            }
            
            //접속 버튼 활성/비활성화 및 텍스트 변경
            connectionBtn.IsEnabled = true;
            if (ConnectionManager.Instance.IsConnected)
            {
                UIUtils.UpdateAppBarIconButton(connectionBtn, I18n.GetString("AppBarButtonTextDisconnect"), ResourceUri.DisconnectImageUri);
            }
            else
            {
                UIUtils.UpdateAppBarIconButton(connectionBtn, I18n.GetString("AppBarButtonTextConnect"), ResourceUri.ConnectImageUri);

                if (ConnectionManager.Instance.ConnectionInfo.CurrentServer == null)
                {
                    connectionBtn.IsEnabled = false;
                }
            }
            
            //장치타입에 따라 배경화면 변경
            if (PointingControlManager.Instance.IsRealTimeScreen)
            {
                //PointingControlManager.Instance.UpdateBackgroundImage는 connect된 상태에서만 작동된다.
                PointingControlManager.Instance.UpdateBackgroundImage(true, new CallbackHandler((object imageInfo) =>
                {
                    Deployment.Current.Dispatcher.BeginInvoke(() =>
                    {
                        try
                        {
                            this.UpdateBackgroundImage((ImageInfo)imageInfo);
                        }
                        catch (Exception e)
                        {
                            System.Diagnostics.Debug.WriteLine(e.Message);
                            isRefreshAll = true;
                        }
                    });
                }));
            }
            else
            {
                UpdateBackgroundImage(PointingControlManager.Instance.DeviceType == DeviceTypes.Mouse ? backgroundMouseBitmap : backgroundTouchScreenBitmap);
            }
        }

        private void UpdateConnectionStatus(Boolean isConnect)
        {
            if (isConnect)
            {
                //앱바에서 재접속시 
                System.Net.Sockets.SocketError result = (System.Net.Sockets.SocketError)ConnectionManager.Instance.ConnectServer(
                    PointingControlManager.Instance.DeviceType,

                    new CallbackHandler(CallbackConnectServer));

                if (result != System.Net.Sockets.SocketError.NotConnected)
                {
                    txtInfomation.Text = I18n.GetString("ServerConnectPageConnecting");
                }
            }
            else
            {
                ConnectionManager.Instance.DisconnectServer(new CallbackHandler((object param) =>
                {
                    Deployment.Current.Dispatcher.BeginInvoke(() =>
                    {
                        UpdateDisconnectUI();
                    });
                }));
            }
        }

        private void UpdateDisconnectUI()
        {
            SetScreenMode(ScreenTypes.None);
            UpdateUIElements();
            UpdateBackgroundImage(PointingControlManager.Instance.DeviceType == DeviceTypes.Mouse ? backgroundMouseBitmap : backgroundTouchScreenBitmap);
            UIUtils.SetVisibility(keybdLayer, false);
        }

        private void CheckConnectionThreadFn()
        {
            int batterySwitch = 1;
            int checkTime = 5000;   //초기 5초 셋팅
            while (true)
            {
                if (!NetworkUtils.IsNetworkAvailable)
                {
                    if (ConnectionManager.Instance.IsConnected)
                    {
                        //접속중 WiFi가 연결이 끊긴경우 접속 해제 처리
                        UpdateConnectionStatus(false);

                    }
                    else if (AppStateUtils.ContainsRecoveryType(RecoveryTypes.Connection))
                    {
                        //접속중 WiFi가 연결이 끊긴경우 접속 해제 처리
                        UpdateConnectionStatus(false);
                        AppStateUtils.ClearRecoveryTypes();
                    }
                }
                else
                {
                    //2. 커넥션 체크 (접속되었을때 체크시작, 접속해지후 체크종료)
                    if (ConnectionManager.Instance.IsConnected)
                    {
                        checkTime = 3000;   //체크 간격을 3초로 줄임
                        //쓰레드 대기 준비
                        connCheckDone.Reset();
                        //체크결과 요청
                        System.Net.Sockets.SocketError socketError = (System.Net.Sockets.SocketError)ConnectionManager.Instance.CheckConnection(SettingManager.Instance.SettingInfo,

                            new CallbackHandler((object param) =>
                            {
                                Deployment.Current.Dispatcher.BeginInvoke(() =>
                                {
                                    //쓰레드 재계
                                    connCheckDone.Set();

                                    if (ConnectionManager.Instance.IsConnected)
                                    {
                                        ServerExtraInfo serverStatusInfo = param as ServerExtraInfo;
                                        if (VersionUtils.IsPresentation)
                                        {
                                            SetScreenMode(serverStatusInfo.ScreenType);
                                        }
                                    }
                                    else
                                    {
                                        UpdateDisconnectUI();

                                        if (MessageBox.Show(I18n.GetString("AppMessageDisconnectedFromPC"), I18n.GetString("AppMessageConfirm"), MessageBoxButton.OKCancel) == MessageBoxResult.OK)
                                        {
                                            NavigationService.Navigate(ResourceUri.ServerListPageUri);
                                        }
                                    }
                                });
                            }));
                        //쓰레드 대기
                        if (!connCheckDone.WaitOne(10000))
                        {
                            //Deployment.Current.Dispatcher.BeginInvoke(() =>
                            //    {
                            //        MessageBox.Show("타임아웃 발생");
                            //    });
                        }
                    }
                    else
                    {
                        checkTime = 10000;  //체크 간격을 10초로 늘림
                    }
                }
                //위치가 위로가면 블록킹 걸리는 경우가 있는것 같다.
                Thread.Sleep(checkTime);
                //배터리는 대략 1 ~ 2분에 한번 체크
                if (batterySwitch == 20)
                {
#if !WP7
                    DeviceInfo.Battery = Windows.Phone.Devices.Power.Battery.GetDefault().RemainingChargePercent;
#endif
                    batterySwitch = 1;
                }
                else
                {
                    batterySwitch++;
                }
            }
        }

        // 지역화된 ApplicationBar를 빌드하는 샘플 코드
        private void BuildLocalizedApplicationBar()
        {
            //페이지의 ApplicationBar를 ApplicationBar의 새 인스턴스로 설정합니다.
            ApplicationBar = new ApplicationBar();
            ApplicationBar.Mode = ApplicationBarMode.Default;
            ApplicationBar.Opacity = 0.9;
            ApplicationBar.IsVisible = false;
            ApplicationBar.IsMenuEnabled = true;

            // 새 단추를 만들고 텍스트 값을 AppResources의 지역화된 문자열로 설정합니다.
            deviceAppBarIconButton = UIUtils.CreateAppBarIconButton(I18n.GetString("AppBarButtonTextMouse"), ResourceUri.MouseImageUri, modeAppbarIconBtn_Click);
            searchAppBarIconButton = UIUtils.CreateAppBarIconButton(I18n.GetString("AppBarButtonTextSearchPC"), ResourceUri.IMG_APPBAR_PCSEARCH, searchAppbarIconBtn_Click);

            nPosAppBarAbout = ApplicationBar.Buttons.Add(UIUtils.CreateAppBarIconButton(I18n.GetString("ApplicationAbout"), ResourceUri.AboutImageUri, aboutAppbarIconBtn_Click)) - 1;
            nPosAppBarConnect = ApplicationBar.Buttons.Add(UIUtils.CreateAppBarIconButton(I18n.GetString("AppBarButtonTextConnect"), ResourceUri.ConnectImageUri, connectAppbarIconBtn_Click)) - 1;
            nPosAppBarSetting = ApplicationBar.Buttons.Add(UIUtils.CreateAppBarIconButton(I18n.GetString("AppBarButtonTextSettings"), ResourceUri.IMG_APPBAR_SETTINGS, settingsAppbarIconBtn_Click)) - 1;
            nPosAppBarSearchPC = ApplicationBar.Buttons.Add(searchAppBarIconButton) - 1;
            
            // AppResources의 지역화된 문자열을 사용하여 새 메뉴 항목을 만듭니다.
            ApplicationBar.MenuItems.Add(UIUtils.CreateAppBarMenuItem(I18n.GetString("AppBarMenuTextHelp"), miHelp_Click));
            //ApplicationBar.MenuItems.Add(UIUtils.CreateAppBarMenuItem(I18n.GetString("AppBarMenuTextEmail"), miEmail_Click));
            if (VersionUtils.IsTrial || VersionUtils.IsAdvertising)
            {
                ApplicationBar.MenuItems.Add(UIUtils.CreateAppBarMenuItem(I18n.GetString("AppMessageTrialBuyFullversion"), miBuyFull_Click));
                ApplicationBar.MenuItems.Add(UIUtils.CreateAppBarMenuItem(I18n.GetString("AppMessageBuyMKVersion"), miBuyMK_Click));
                ApplicationBar.MenuItems.Add(UIUtils.CreateAppBarMenuItem(I18n.GetString("AppMessageBuyTKVersion"), miBuyTK_Click));
            }
            else if (!VersionUtils.IsFull && !VersionUtils.IsAdvertising)
            {
                ApplicationBar.MenuItems.Add(UIUtils.CreateAppBarMenuItem(I18n.GetString("AppMessageTrialBuyFullversion"), miBuyFull_Click));
            }
        }

    }
}