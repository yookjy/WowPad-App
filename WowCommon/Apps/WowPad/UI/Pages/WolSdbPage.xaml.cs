using Apps.WowPad.Resources;
using Apps.WowPad.Util;
using Microsoft.Phone.Controls;
using System.ComponentModel;
using System.Net;
using System.Text.RegularExpressions;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;

namespace Apps.WowPad.UI.Pages
{
    public partial class WolSdbPage : PhoneApplicationPage
    {
        private BackgroundWorker bgWorker;
        private string serverIP;
        private bool buttonTapped;

        public WolSdbPage()
        {
            InitializeComponent();
            //타이틀 설정
            txtAppName.Text = string.Format(I18n.GetString("AppName"), " " + VersionUtils.Version);
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            serverIP = NavigationContext.QueryString["serverIP"];
            subnetLabel.Text = string.Format(I18n.GetString("WOLPageSubnetMask"), NavigationContext.QueryString["serverName"]);
            subnetTxtBox.Text = NavigationContext.QueryString["subnet"];
            if (string.IsNullOrEmpty(subnetTxtBox.Text.Trim()))
            {
                subnetTxtBox.Text = NetworkUtils.GetSubnetmask(serverIP);
            }

            if (subnetTxtBox.Text == IPAddress.Any.ToString())
            {
                wolBtn.IsEnabled = false;
            }

            //포커스 이동 속도가 너무 빠르면 키보드가 텍스트박스를 덮는 현상이 생기므로 시간차 공격!
            bgWorker = new BackgroundWorker();
            bgWorker.DoWork += bk_DoWork;
            bgWorker.RunWorkerCompleted += bk_RunWorkerCompleted;
            bgWorker.RunWorkerAsync();
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            base.OnNavigatedFrom(e);

            if (e.IsNavigationInitiator && buttonTapped)
            {
                ServerListPage currPage = e.Content as ServerListPage;
                string wolIp = NetworkUtils.GetSubnetDirectedBroadcastIP(serverIP, subnetTxtBox.Text.Trim());
                currPage.WakeOnLan(wolIp);
                currPage.SaveSubnetMask(subnetTxtBox.Text.Trim());
            }
        }

        void bk_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            Deployment.Current.Dispatcher.BeginInvoke(() =>
            {
                subnetTxtBox.Focus();

                if (subnetTxtBox.Text == IPAddress.Any.ToString())
                {
                    subnetTxtBox.SelectAll();
                }
                else
                {
                    subnetTxtBox.Select(subnetTxtBox.Text.Length, 0);
                }
            });
        }

        void bk_DoWork(object sender, DoWorkEventArgs e)
        {
            Thread.Sleep(200);
        }

        private void subnetTxtBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            string ipStr = subnetTxtBox.Text.Trim();
            bool isEnabled = false;
            //검색버튼 활성 비활성화
            //if (System.Text.RegularExpressions.Regex.IsMatch(subnetTxtBox.Text.Trim(), "\\b(25[0-5]|2[0-4][0-9]|[01]?[0-9][0-9]?)\\.(25[0-5]|2[0-4][0-9]|[01]?[0-9][0-9]?)\\.(25[0-5]|2[0-4][0-9]|[01]?[0-9][0-9]?)\\.(25[0-5]|2[0-4][0-9]|[01]?[0-9][0-9]?)\\b"))
            if (Regex.IsMatch(ipStr, "\\b0*(2(5[0-5]|[0-4]\\d)|1?\\d{1,2})(\\.0*(2(5[0-5]|[0-4]\\d)|1?\\d{1,2})){3}\\b") 
                && Regex.Matches(ipStr, "\\.").Count == 3 && (ipStr != IPAddress.Any.ToString()) && (ipStr != IPAddress.Broadcast.ToString()))
            {
                isEnabled = true;
            }

            wolBtn.IsEnabled = isEnabled;
        }

        private void connectServerBtn_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            buttonTapped = true;
            this.NavigationService.GoBack();
        }

    }
}