using Apps.WowPad.Manager;
using Apps.WowPad.Resources;
using Apps.WowPad.Util;
using Microsoft.Phone.Controls;
using System;
using System.ComponentModel;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Navigation;

namespace Apps.WowPad.UI.Pages
{
    public partial class ServerConnectPage : PhoneApplicationPage
    {
        private BackgroundWorker bgWorker;

        public ServerConnectPage()
        {
            InitializeComponent();

            //타이틀 설정
            txtAppName.Text = string.Format(I18n.GetString("AppName"), " " + VersionUtils.Version);

            InputScope scope = new InputScope();
            InputScopeName name = new InputScopeName();

            name.NameValue = InputScopeNameValue.Digits;  //<--Here
            scope.Names.Add(name);

            accessCodeTxtBox.InputScope = scope;
            connectServerBtn.IsEnabled = false;
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            //프로그레스바 숨김
            UIUtils.SetVisibility(connectProgressBar, false);
            UIUtils.SetVisibility(labelProgressBar, false);
            
            accessCodeTxtBox.UpdateLayout();
            title.Text = NavigationContext.QueryString["serverName"];

            //포커스 이동 속도가 너무 빠르면 키보드가 텍스트박스를 덮는 현상이 생기므로 시간차 공격!
            bgWorker = new BackgroundWorker();
            bgWorker.DoWork += bk_DoWork;
            bgWorker.RunWorkerCompleted += bk_RunWorkerCompleted;
            bgWorker.RunWorkerAsync();
        }

        void bk_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            Deployment.Current.Dispatcher.BeginInvoke(() =>
            {
                accessCodeTxtBox.Focus();
            });
        }

        void bk_DoWork(object sender, DoWorkEventArgs e)
        {
            Thread.Sleep(200);
        }

        //뒤로 [하드웨어버튼} 눌렸을때 처리 
        protected override void OnBackKeyPress(System.ComponentModel.CancelEventArgs e)
        {
            if (!ConnectionManager.Instance.IsConnected)
            {
                ConnectionManager.Instance.ConnectionInfo.CurrentServer = null;
            }
            base.OnBackKeyPress(e);
        }

        private void accessCodeTxtBox_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (System.Text.RegularExpressions.Regex.IsMatch(e.Key.ToString(), "[0-9]"))
                e.Handled = false;
            else e.Handled = true;
        }

        private void accessCodeTxtBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            //검색버튼 활성 비활성화
            connectServerBtn.IsEnabled = accessCodeTxtBox.Text.Trim().Length == 4;
        }

        private void connectServerBtn_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            ConnectionManager.Instance.ConnectionInfo.CurrentServer.AccessCode = Convert.ToInt16(accessCodeTxtBox.Text.Trim());
            ConnectionManager.Instance.ConnectServer(
                SettingManager.Instance.SettingInfo.DeviceType, 
                new CallbackHandler((object serverExtraInfo) =>
                {
                    Deployment.Current.Dispatcher.BeginInvoke(() =>
                    {
                        if (ConnectionManager.Instance.IsConnected)
                        {
                            AppStateUtils.Set(Constant.KEY_SERVER_EXTRA_INFO, serverExtraInfo);
                            this.NavigationService.RemoveBackEntry();
                            this.NavigationService.GoBack();
                        }
                        else
                        {
                            //프로그레스바 숨김
                            UIUtils.SetVisibility(connectProgressBar, false);
                            UIUtils.SetVisibility(labelProgressBar, false);

                            MessageBox.Show(I18n.GetString("AppMessageAccessCodeIncorrect"), I18n.GetString("AppMessageNotification"), MessageBoxButton.OK);
                            accessCodeTxtBox.Text = string.Empty;
                            //역시 시간차 공격..
                            bgWorker.RunWorkerAsync();
                        }
                    });
                }));
            //프로그레스바 표시
            UIUtils.SetVisibility(connectProgressBar, true);
            UIUtils.SetVisibility(labelProgressBar, true);
        }
    }
}