using Apps.UI;
using Apps.WowPad.Manager;
using Apps.WowPad.Model;
using Apps.WowPad.Resources;
using Apps.WowPad.Type;
using Apps.WowPad.Util;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using Microsoft.Phone.Tasks;
using System;
using System.ComponentModel;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Animation;
using System.Windows.Navigation;

namespace Apps.WowPad.UI.Pages
{
    public partial class MainPage
    {
        //데이터 사용량 알리미
        void CellularDataUtil_NotifyUsagePerMegaBytes(object sender, Apps.UI.Notification.CellularDataEventArgs e)
        {
            if (NetworkUtils.IsNetworkAvailable && !NetworkUtils.IsWiFiNetwork)
            {
                Deployment.Current.Dispatcher.BeginInvoke(() =>
                {
                    double gb = e.usage / 1024 / 1024 / 1024;
                    double nUsage = gb > 0 ? gb : e.usage / 1024 / 1024;
                    string usage = (gb > 0 ? nUsage.ToString("F1") + "GB" : nUsage.ToString("F0") + "MB");
                    msgTip.FadeInOut(string.Format(I18n.GetString("AppMessageUsage"), usage), 2000);
                });
            }
        }

        //셀룰러 접속시 데이터 폭탄 경고
        private void PhoneApplicationPage_Loaded(object sender, RoutedEventArgs e)
        {
            if (isLoaded && NetworkUtils.IsNetworkAvailable && !NetworkUtils.IsWiFiNetwork)
            {
                msgTip.FadeInOut(I18n.GetString("AppMessageCellularChargeInfomation"), 2000);
            }
        }

        //메인화면에서 Back버튼 클릭했을때 이벤트 핸들러 재정의
        protected override void OnBackKeyPress(System.ComponentModel.CancelEventArgs e)
        {
            if (!msgTip.IsVisiable || string.IsNullOrEmpty(msgTip.HoldKey) || msgTip.HoldKey != "exit")
            {
                msgTip.Show(string.Format(I18n.GetString("AppMessageExit"), " " +  VersionUtils.Version));
                msgTip.FadeOut(2000, "exit");
                e.Cancel = true;
            }

            base.OnBackKeyPress(e);
        }

        #region Navigation Events

        //메인 페이지에 진입했을때 발생되는 이벤트 재정의
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            isLoaded = false;
            base.OnNavigatedTo(e);

            if (e.NavigationMode == NavigationMode.Back)
            {
                if (AppStateUtils.ContainsRecoveryType(RecoveryTypes.Connection))
                {
                    ConnectionManager.Instance.ConnectServer(
                        PointingControlManager.Instance.DeviceType,
                        new CallbackHandler((object param) =>
                        {
                            CallbackConnectServer(param);
                            //복구 모드 삭제
                            AppStateUtils.ClearRecoveryTypes();
                        }));
                }
                else
                {
                    ServerExtraInfo seInfo = (ServerExtraInfo)AppStateUtils.Get(Constant.KEY_SERVER_EXTRA_INFO);
                    if (seInfo != null)
                    {
                        //접속후 돌아온 경우
                        txtInfomation.Text = I18n.GetString("ServerConnectPageConnecting");
                        CallbackConnectServer(seInfo);
                        AppStateUtils.Remove(Constant.KEY_SERVER_EXTRA_INFO);
                    }
                    else
                    {
                        if (!ConnectionManager.Instance.IsConnected)
                        {
                            UpdateUIElements();
                        }
                        else
                        {
                            //접속이 되어 있으면 접속된 정보를 저장
                            if (SettingManager.Instance.SettingInfo.AutoConnect)
                            {
                                ConnectionManager.Instance.SaveLastConnectedServer(PointingControlManager.Instance.DeviceType);
                            }
                            //버튼 복구
                            UIUtils.SetVisibility(btnMenu, true);
                            UIUtils.SetVisibility(btnKeyboard, true);
                        }
                    }
                }
            }
            else
            {
                isLoaded = true;
            }
        }
        #endregion

        #region
        //로딩팝업 이벤트 처리
        void loadWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
        }

        void loadWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            LoadingArgument loadingArg = e.Argument as LoadingArgument;
            BackgroundWorker worker = sender as BackgroundWorker;

            Deployment.Current.Dispatcher.BeginInvoke(() =>
            {
                switch (loadingArg.LoadingType)
                {
                    case LoadingTypes.Keyboard:
                        if (loadingArg.LoadingGrid != null)
                        {

                            Apps.UI.Keyboard.Keyboard keybd = keybdLayer.Children[0] as Apps.UI.Keyboard.Keyboard;
                            keybd.RecreateKeyboard();
                            //로딩제거
                            this.LayoutRoot.Children.Remove(loadingArg.LoadingGrid);
                        }

                        UIUtils.SetVisibility(keybdLayer, true);
                        Storyboard sb = new Storyboard();
                        sb.Children.Add(GetKeyboardAnimation(true));
                        sb.Completed += new EventHandler((object obj, EventArgs ev) =>
                        {
                            UIUtils.SetVisibility(btnKeyboard, false);
                        });
                        sb.Begin();
                        break;
                    case LoadingTypes.Connection:
                        ServerExtraInfo srvExtraInfo = (ServerExtraInfo)loadingArg.Result;
                        //UI 업데이트
                        this.UpdateUIElements();
                        //키보드 리스트 적용
                        SetKeyboardList(srvExtraInfo.KeyboardList);
                        //로딩제거
                        this.LayoutRoot.Children.Remove(loadingArg.LoadingGrid);
                        break;
                }
            });
        }
        #endregion

        #region MainPage Button Events
        //백그라운드 모드를 변경한다.
        private void btnScreen_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            //모드 토글
            PointingControlManager.Instance.IsRealTimeScreen = !PointingControlManager.Instance.IsRealTimeScreen;
            //모드에 따라 백스크린 호출
            if (PointingControlManager.Instance.IsRealTimeScreen)
            {
                PointingControlManager.Instance.UpdateBackgroundImage(true, new CallbackHandler((object imageInfo) =>
                {
                    Deployment.Current.Dispatcher.BeginInvoke(() =>
                    {
                        try
                        {
                            this.UpdateBackgroundImage((ImageInfo)imageInfo);
                        }
                        catch (Exception ex)
                        {
                            System.Diagnostics.Debug.WriteLine(ex.Message);
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
        //어플리케이션바를 표시한다.
        private void btnSettings_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            UIUtils.SetVisibility(btnMenu, false);
            UIUtils.SetVisibility(btnKeyboard, false);
            ApplicationBar.IsVisible = true;
        }

        //키보드를 화면에 표시한다.
        private void btnKeyboard_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            //체험판 체크
            if (VersionUtils.CheckTrialVersion())
            {
                MessageBox.Show(I18n.GetString("AppMessageTrialCannotuse") + " " + I18n.GetString("AppMessageTrialBuyInfo"));
                return;
            }

            if (!loadWorker.IsBusy)
            {
                Thickness margin = keybdLayer.Margin;
                if (SettingManager.Instance.SettingInfo.FullSizeKeyboard)
                {
                    keybdLayer.Margin = new Thickness(0, this.ActualHeight * 0.3, 0, 0);
                }
                else
                {
                    keybdLayer.Margin = new Thickness(50, this.ActualHeight * 0.5, 50, 0);
                }

                //키보드 사이즈가 바뀐경우 재생성
                if (margin.Top != 0 && margin.Top != keybdLayer.Margin.Top)
                {
                    Grid loadingGrd = CreateLoadingGrid("LoadingKeyboardSize");
                    loadWorker.RunWorkerAsync(new LoadingArgument() { LoadingGrid = loadingGrd, LoadingType = LoadingTypes.Keyboard });
                }
                else
                {
                    loadWorker.RunWorkerAsync(new LoadingArgument() { LoadingType = LoadingTypes.Keyboard });
                }
            }
        }

        //가상키 버튼들에 대한 처리
        private void virtualButton_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            if (sender is ImageButton)
            {
                ImageButton button = sender as ImageButton;
                ButtonTypes buttonType = ButtonTypes.None;
                
                //체험판 체크
                if (VersionUtils.CheckTrialVersion(button.Name != "btnWindows"
                    && button.Name != "btnPptShowSlideFirstPage"
                    && button.Name != "btnPptShowSlideCurrentPage"
                    && button.Name != "btnPptCloseSlide"))
                {
                    MessageBox.Show(I18n.GetString("AppMessageTrialCannotuse") + " " + I18n.GetString("AppMessageTrialBuyInfo"));
                    return;
                }

                switch (button.Name)
                {
                    case "btnWindows":
                        buttonType = ButtonTypes.Windows;
                        break;
                    case "btnPptShowSlideFirstPage":
                        buttonType = ButtonTypes.OpenSlideShowFirstPage;
                        break;
                    case "btnPptShowSlideCurrentPage":
                        buttonType = ButtonTypes.OpenSlideShowCurrentPage;
                        break;
                    case "btnPptPointer":
                        buttonType = ButtonTypes.ShowPointer;
                        break;
                    case "btnPptHand":
                        buttonType = ButtonTypes.ShowPointer;
                        break;
                    case "btnPptBpen":
                        buttonType = ButtonTypes.BallPointPen;
                        break;
                    case "btnPptLaser":
                        buttonType = ButtonTypes.Laser;
                        break;
                    case "btnPptIpen":
                        buttonType = ButtonTypes.InkPen;
                        break;
                    case "btnPptHpen":
                        buttonType = ButtonTypes.Highlighter;
                        break;
                    case "btnPptPrev":
                        buttonType = ButtonTypes.PreviousSlide;
                        break;
                    case "btnPptCloseSlide":
                        buttonType = ButtonTypes.CloseSlideShow;
                        break;
                    case "btnPptEraser":
                        buttonType = ButtonTypes.Eraser;
                        break;
                    case "btnPptRemover":
                        buttonType = ButtonTypes.PageEraser;
                        break;
                    case "btnPptToggleInk":
                        buttonType = ButtonTypes.InkToggle;
                        break;
                    case "btnPptNext":
                        buttonType = ButtonTypes.NextSlide;
                        break;
                }
                
                if (buttonType != ButtonTypes.None)
                    PointingControlManager.Instance.PressedVirtualButton(buttonType);
            }
        }

        #endregion

        #region ApplicationBtn Click Events
        //앱바의 검색 버튼 클릭시 pc검색 페이지로 이동한다.
        void searchAppbarIconBtn_Click(object sender, EventArgs e)
        {
            ApplicationBar.IsVisible = false;
            this.NavigationService.Navigate(ResourceUri.ServerListPageUri);
        }

        //앱바의 셋팅 버튼 클릭시 셋팅 페이지로 이동한다.
        void settingsAppbarIconBtn_Click(object sender, EventArgs e)
        {
            ApplicationBar.IsVisible = false;
            this.NavigationService.Navigate(ResourceUri.SettingPageUri);
        }

        //앱바의 마우스/터치스크린 버튼 클릭시 해당 모드로 전환처리를 한다.
        void modeAppbarIconBtn_Click(object sender, EventArgs e)
        {
            DeviceTypes deviceType = GetPointingDeviceType(sender as ApplicationBarIconButton);
            PointingControlManager.Instance.DeviceType = deviceType;
            ApplicationBar.IsVisible = false;
        }

        //앱바의 어바웃 버튼 클릭시 처리를 한다. => 어바웃페이지 변경
        void aboutAppbarIconBtn_Click(object sender, EventArgs e)
        {
            //new MarketplaceReviewTask().Show();
            this.NavigationService.Navigate(ResourceUri.AboutPageUri);
        }

        //앱바의 연결/끊기 버튼 클릭시 접속/접속 해제 처리를 한다.
        void connectAppbarIconBtn_Click(object sender, EventArgs e)
        {
            Boolean isConnect = IsConnectAppBarButton(sender as ApplicationBarIconButton);
            UpdateConnectionStatus(isConnect);
            ApplicationBar.IsVisible = false;
        }

        //도움말 페이지로 이동한다.
        void miHelp_Click(object sender, EventArgs e)
        {
            this.NavigationService.Navigate(ResourceUri.HelpPageUri);
        }

        //이메일 페이지로 이동한다.
        void miEmail_Click(object sender, EventArgs e)
        {
            EmailComposeTask emailComposeTask = new EmailComposeTask();
            emailComposeTask.To = Constant.EMAIL_SUPPORT;
            emailComposeTask.Show();
        }

        //정식버전 구매 페이지로 이동한다.
        void miBuyFull_Click(object sender, EventArgs e)
        {
            if (VersionUtils.IsTrial)
            {
                new MarketplaceDetailTask().Show();
            }
            else
            {
                MarketplaceDetailTask marketplaceDetailTask = new MarketplaceDetailTask();
                marketplaceDetailTask.ContentIdentifier = Constant.FULL_VERSION_APP_ID;
                marketplaceDetailTask.ContentType = MarketplaceContentType.Applications;
                marketplaceDetailTask.Show();
            }
        }

        //MK버전 구매 페이지로 이동한다.
        void miBuyMK_Click(object sender, EventArgs e)
        {
            MarketplaceDetailTask marketplaceDetailTask = new MarketplaceDetailTask();
            marketplaceDetailTask.ContentIdentifier = Constant.MK_VERSION_APP_ID;
            marketplaceDetailTask.ContentType = MarketplaceContentType.Applications;
            marketplaceDetailTask.Show();
        }

        //TK버전 구매 페이지로 이동한다.
        void miBuyTK_Click(object sender, EventArgs e)
        {
            MarketplaceDetailTask marketplaceDetailTask = new MarketplaceDetailTask();
            marketplaceDetailTask.ContentIdentifier = Constant.TK_VERSION_APP_ID;
            marketplaceDetailTask.ContentType = MarketplaceContentType.Applications;
            marketplaceDetailTask.Show();
        }
        #endregion 

        #region MainPage View Events
        //화면이 회전할때 페이지 사이즈 정보를 재설정한다.
        private void PhoneApplicationPage_OrientationChanged_1(object sender, OrientationChangedEventArgs e)
        {
            DeviceInfo.Load();
            ResourceUri.Load();
        }
        
        //터치 이벤트를 모두 처리한다.
        void Touch_FrameReported(object sender, TouchFrameEventArgs e)
        {
            //메인페이지가 아닌곳에서 발생되면 무시
            if (this.NavigationService.CurrentSource.OriginalString != Constant.PAGE_MAIN 
                || (layerGettingStart.Visibility == System.Windows.Visibility.Visible && LayoutRoot.Children.Contains(layerGettingStart)))
            {
                return;
            }

            //이벤트 시작
            TouchPoint pTouchInfo = e.GetPrimaryTouchPoint(LayoutRoot);

            //버튼이 눌린 경우 이벤트 무시
            if (IsButtonsArea(pTouchInfo.Position)) return;
            
            //키보드 밖을 터치하거나 움직이면 키보드 숨김
            if (pTouchInfo.Action == TouchAction.Down || pTouchInfo.Action == TouchAction.Move)
            {
                if (UIUtils.IsVisible(keybdLayer) && !isAnimating)
                {
                    isAnimating = true;
                    Storyboard sb = new Storyboard();
                    sb.Children.Add(GetKeyboardAnimation(false));
                    sb.Completed += new EventHandler((object obj, EventArgs ev) =>
                    {
                        UIUtils.SetVisibility(keybdLayer, false);
                        isAnimating = false;
                    });
                    sb.Begin();
                }
            }

            if (pTouchInfo.Action == TouchAction.Down)
            {
                //화면을 터치하면 앱바 숨김
                if (ApplicationBar.IsVisible)
                {
                    ApplicationBar.IsVisible = false;
                }
            }
            else if (pTouchInfo.Action == TouchAction.Move)
            {
                //움직임 있으면 기본 버튼 숨김 숨김
                if (screenType == ScreenTypes.PowerPointSlideShow2007
                    || screenType == ScreenTypes.PowerPointSlideShow2010
                    || screenType == ScreenTypes.PowerPointSlideShow2013)
                {
                    UIUtils.SetVisibility(layerLeftSlideshow, false);
                    UIUtils.SetVisibility(layerRightSlideshow, false);
                }
                else
                {
                    UIUtils.SetVisibility(btnWindows, false);
                    UIUtils.SetVisibility(btnKeyboard, false);
                    UIUtils.SetVisibility(btnMenu, false);
                }
            }
            else if (pTouchInfo.Action == TouchAction.Up)
            {
                if (!ApplicationBar.IsVisible)
                {
                    //앱바가 숨김상태이고 연결된 상태에서 터치가 않되고 있다면 기본 버튼 표시
                    if (ConnectionManager.Instance.IsConnected)
                    {
                        if (screenType == ScreenTypes.PowerPointSlideShow2007
                            || screenType == ScreenTypes.PowerPointSlideShow2010
                            || screenType == ScreenTypes.PowerPointSlideShow2013)
                        {
                            UIUtils.SetVisibility(layerLeftSlideshow, true);
                            UIUtils.SetVisibility(layerRightSlideshow, true);
                        }
                        else
                        {
                            //UIUtils.SetVisibility(btnScreen, true);
                            UIUtils.SetVisibility(btnWindows, true);
                            UIUtils.SetVisibility(btnKeyboard, true);
                        }
                    }
                    else
                    {
                        //연결이 끊긴 상태이면 윈도우, 키보드 버튼 숨김
                        //UIUtils.SetVisibility(btnScreen, false);
                        UIUtils.SetVisibility(btnWindows, false);
                        UIUtils.SetVisibility(btnKeyboard, false);
                    }

                    //스크린 타입이 없으면 항시 보임
                    if (screenType != ScreenTypes.PowerPointSlideShow2007
                        && screenType != ScreenTypes.PowerPointSlideShow2010
                        && screenType != ScreenTypes.PowerPointSlideShow2013)
                    {
                        UIUtils.SetVisibility(btnMenu, true);
                    }
                }
            }

            if (ConnectionManager.Instance.IsConnected)
            {
                //버튼을 제외한 영역 부터 터치 좌표 구함.
                TouchPointCollection tpCols = e.GetTouchPoints(this.LayoutRoot);

                for (int i = 0; i < 5; i++)
                {
                    if (tpCols.Count > i)
                    {
                        //ID는 1부터로 서버에서 정의함
                        touchInfos[i].Id = tpCols[i].TouchDevice.Id + 1;
                        touchInfos[i].X = (int)tpCols[i].Position.X;
                        touchInfos[i].Y = (int)tpCols[i].Position.Y;

                        switch (tpCols[i].Action)
                        {
                            case TouchAction.Down:
                                touchInfos[i].Action = TouchActionTypes.Begin;
                                if (PointingControlManager.Instance.DeviceType == DeviceTypes.Mouse)
                                {
                                    //메인 핑거 저장
                                    if (primaryPointer == null && tpCols.Count == 1)
                                    {
                                        primaryPointer = touchInfos[i].Clone();
                                    }

                                    //클릭 버튼 ID 저장
                                    if (primaryPointer != null && tpCols.Count == 2)
                                    {
                                        if (primaryPointer.Id != touchInfos[i].Id)
                                        {
                                            secondaryPointer = touchInfos[i].Clone();
                                        }
                                    }
                                }
                                break;
                            case TouchAction.Move:
                                touchInfos[i].Action = TouchActionTypes.Move;
                                if (PointingControlManager.Instance.DeviceType == DeviceTypes.Mouse)
                                {
                                    if (primaryPointer != null && primaryPointer.Id == touchInfos[i].Id)
                                    {
                                        //오차 범위 수평, 수직3 이외로 움직일때만 이동 문구 표시
                                        if ((Math.Abs(primaryPointer.X - touchInfos[i].X) > 3 || Math.Abs(primaryPointer.Y - touchInfos[i].Y) > 3))
                                        {
                                            UIUtils.SetVisibility(txtInfomation, true, I18n.GetString("MainPageMouseMovePointer"));
                                        }
                                        //메인 핑거 위치 이동
                                        primaryPointer = touchInfos[i].Clone();
                                    }
                                    else if (secondaryPointer != null && secondaryPointer.Id == touchInfos[i].Id)
                                    {
                                        //클릭 버튼 위치 이동
                                        secondaryPointer = touchInfos[i].Clone();
                                    }
                                }
                                break;
                            case TouchAction.Up:
                                touchInfos[i].Action = TouchActionTypes.End;
                                 if (PointingControlManager.Instance.DeviceType == DeviceTypes.Mouse)
                                {
                                    if (secondaryPointer != null && secondaryPointer.Id == touchInfos[i].Id)
                                    {
                                        //클릭 버튼 초기화
                                        UIUtils.SetVisibility(txtInfomation, false);
                                        secondaryPointer = null;
                                    }
                                    else if (primaryPointer != null && primaryPointer.Id == touchInfos[i].Id)
                                    {
                                        //메인핑거 초기화 및 클릭버튼 초기화
                                        UIUtils.SetVisibility(txtInfomation, false);
                                        primaryPointer = null;
                                        secondaryPointer = null;
                                    }
                                }
                                break;                               
                        }
                    }
                    else
                    {
                        touchInfos[i].Id = 0;
                        touchInfos[i].X = 0;
                        touchInfos[i].Y = 0;
                        touchInfos[i].Action = TouchActionTypes.None;
                    }
                }

                if (PointingControlManager.Instance.DeviceType != DeviceTypes.Mouse)
                {
                    //마우스 이외의 모드이면 숨김
                    UIUtils.SetVisibility(txtInfomation, false);
                }

                //마우스 모드 버튼 클릭 화면 표시
                if (secondaryPointer != null && 
                    (secondaryPointer.Action == TouchActionTypes.Begin || secondaryPointer.Action == TouchActionTypes.End))
                {
                    if (secondaryPointer.X < primaryPointer.X && secondaryPointer.Y != primaryPointer.Y)
                    {
                        if (secondaryPointer.Y < primaryPointer.Y && SettingManager.Instance.SettingInfo.UseExtendButton)
                        {
                            //브라우저 뒤로
                            UIUtils.SetVisibility(txtInfomation, true, I18n.GetString("MainPageMouseBackClick"));
                        }
                        else
                        {
                            //마우스 우측 버튼
                            UIUtils.SetVisibility(txtInfomation, true, I18n.GetString("MainPageMouseLeftClick"));
                        }
                    }
                    else if (secondaryPointer.X > primaryPointer.X && secondaryPointer.Y != primaryPointer.Y)
                    {
                        if (secondaryPointer.Y < primaryPointer.Y && SettingManager.Instance.SettingInfo.UseExtendButton)
                        {
                            //브라우저 앞으로
                            UIUtils.SetVisibility(txtInfomation, true, I18n.GetString("MainPageMouseForwardClick"));
                        }
                        else
                        {
                            //마우스 오른족 버튼
                            UIUtils.SetVisibility(txtInfomation, true, I18n.GetString("MainPageMouseRightClick"));
                        }
                    }
                }

                //마우스 / 터치 스크린 좌표 이동
                PointingControlManager.Instance.MoveTouch(touchInfos);
            }
        }

        #endregion
        
        //포인팅 장치 타입을 변경했을때 화면 UI 갱신 처리
        void pointerCtrlMgr_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            switch (e.PropertyName)
            {
                case "DeviceType":
                    UpdateUIElements();
                    break;
            }
        }       

        //백그라운드 이미지 수신이 실패한경우에 대한 처리
        void Instance_ImageReceiveFailed(object sender, ImageReceiveFailedEventArgs e)
        {
            if (ConnectionManager.Instance.IsConnected && PointingControlManager.Instance.IsRealTimeScreen)
            {
                Deployment.Current.Dispatcher.BeginInvoke(() =>
                {
                    if (e.NeedReconnect)
                    {
                        Thread.Sleep(300);
                        isRefreshAll = true;
                    }

                    //이미지 재전송 요청
                    PointingControlManager.Instance.UpdateBackgroundImage(isRefreshAll, new CallbackHandler((object imageInfo) =>
                    {
                        //콜백으로 시간차가 있으므로 다시 체크해야한다.
                        if (ConnectionManager.Instance.IsConnected && PointingControlManager.Instance.IsRealTimeScreen)
                        {
                            Deployment.Current.Dispatcher.BeginInvoke(() =>
                            {
                                try
                                {
                                    this.UpdateBackgroundImage((ImageInfo)imageInfo);
                                }
                                catch (Exception e2)
                                {
                                    System.Diagnostics.Debug.WriteLine(e2.Message);
                                    isRefreshAll = true;
                                }
                            });
                        }
                    }));
                });
            }
        }

        #region
        //시작하기 도움말 이벤트 핸들러
        private void lnkHelp_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            this.NavigationService.Navigate(ResourceUri.HelpPageUri);
        }

        private void lnkShareDownload_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            ShareLinkTask slt = new ShareLinkTask();
            slt.LinkUri = new Uri("http://apps.velostep.com/wowpad/download/lastest/WowPad_Setup.exe");
            slt.Message = "Direct download link";
            slt.Title = "WowPad PC Server";
            slt.Show();
        }

        private void lnkHide_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            //광고 컨트롤 추가
            VersionUtils.DecideAdControl(this.LayoutRoot, this.txtInfomation);
            this.LayoutRoot.Children.Remove(layerGettingStart);
        }

        private void lnkClose_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            //광고 컨트롤 추가
            VersionUtils.DecideAdControl(this.LayoutRoot, this.txtInfomation);
            SettingManager.Instance.UpdateNLoad(SettingManager.KEY_GETTING_START, false);
            this.LayoutRoot.Children.Remove(layerGettingStart);
        }
        #endregion
    }
}
