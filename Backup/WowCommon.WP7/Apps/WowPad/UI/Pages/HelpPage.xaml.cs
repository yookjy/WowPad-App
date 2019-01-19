using Apps.WowPad.Resources;
using Apps.WowPad.Util;
using Microsoft.Phone.Controls;
using System;
using System.Windows;
using System.Windows.Navigation;

namespace Apps.WowPad.UI.Pages
{
    public partial class HelpPage : PhoneApplicationPage
    {
        public HelpPage()
        {
            InitializeComponent();

            //타이틀 설정
            txtAppName.Text = string.Format(I18n.GetString("AppName"), " " + VersionUtils.Version);

            string locale = I18n.GetString("ResourceLanguage");
            switch (locale)
            {
                case "ko" :
                    locale = "ko-kr";
                    break;
                case "ja" :
                    locale = "ja-jp";
                    break;
                default:
                    locale = "en-us";
                    break;
            }

            webBrowser.IsScriptEnabled = true;
            webBrowser.Source = new Uri(string.Format(Constant.URL_HOW_TO_USE, locale, (Visibility)Application.Current.Resources["PhoneDarkThemeVisibility"] == Visibility.Visible ? "" : "_light"));
            webBrowser.Navigated += webBrowser_Navigated;
            webBrowser.NavigationFailed += webBrowser_NavigationFailed;
        }

        void webBrowser_NavigationFailed(object sender, NavigationFailedEventArgs e)
        {
            if (NavigationService.CanGoBack)
            {
                if (!NetworkUtils.IsNetworkAvailable)
                {
                    NetworkUtils.ShowWiFiSettingPage(I18n.GetString("AppMessageRequiredDataNetwork"), I18n.GetString("AppMessageNotification"));
                }
                else
                {
                    MessageBox.Show(I18n.GetString("HelpPageLoadFail"));
                }
                NavigationService.GoBack();
            }
        }

        void webBrowser_Navigated(object sender, NavigationEventArgs e)
        {
            loadingGrid.Visibility = System.Windows.Visibility.Collapsed;
        }

        protected override void OnBackKeyPress(System.ComponentModel.CancelEventArgs e)
        {
            webBrowser.Navigated -= webBrowser_Navigated;
            webBrowser.NavigationFailed -= webBrowser_NavigationFailed;
            base.OnBackKeyPress(e);
        }

        protected override Size MeasureOverride(Size availableSize)
        {
            loadingGrid.Width = availableSize.Width;
            loadingGrid.Height = availableSize.Height;

            return base.MeasureOverride(availableSize);
        }
    }
}