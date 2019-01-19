using Apps.UI;
using Apps.WowPad.Model;
using Apps.WowPad.Resources;
using Apps.WowPad.Type;
using Microsoft.Advertising.Mobile.UI;
using Microsoft.Phone.Shell;
using Microsoft.Phone.Tasks;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace Apps.WowPad.Util
{
    public class VersionUtils
    {
        public static VersionTypes VersionType { get; set; }

        public static string Version
        {
            get
            {
                if (VersionType == VersionTypes.Full || VersionType == VersionTypes.None)
                {
                    return string.Empty;
                }
                else
                {
                    return VersionType.ToString();
                }
            }
        }

        public static bool IsTrial { get; set; }

        public static bool IsFull
        {
            get
            {
                return VersionType == VersionTypes.Full;
            }
        }

        public static bool IsAdvertising
        {
            get
            {
                return VersionType == VersionTypes.AMK 
                    || VersionType == VersionTypes.AMP 
                    || VersionType == VersionTypes.ATK 
                    || VersionType == VersionTypes.ATP;
            }
        }

        public static bool IsTouchscreen
        {
            get
            {
                return VersionType == VersionTypes.Full
                    || VersionType == VersionTypes.ATK
                    || VersionType == VersionTypes.ATP
                    || VersionType == VersionTypes.TK
                    || VersionType == VersionTypes.TP;
            }
        }

        public static bool IsMouse
        {
            get
            {
                return VersionType == VersionTypes.Full
                    || VersionType == VersionTypes.AMK
                    || VersionType == VersionTypes.AMP
                    || VersionType == VersionTypes.MK
                    || VersionType == VersionTypes.MP;
            }
        }

        public static bool IsPresentation
        {
            get
            {
                return VersionType == VersionTypes.Full
                    || VersionType == VersionTypes.AMP
                    || VersionType == VersionTypes.ATP
                    || VersionType == VersionTypes.MP
                    || VersionType == VersionTypes.TP;
            }
        }

        public static bool IsKeyboard
        {
            get
            {
                return VersionType == VersionTypes.Full
                    || VersionType == VersionTypes.AMK
                    || VersionType == VersionTypes.ATK
                    || VersionType == VersionTypes.MK
                    || VersionType == VersionTypes.TK;
            }
        }
        public static void DecideAdControl(Grid LayoutRoot, TextBlock txtInfomation)
        {
            if (IsAdvertising)
            {
                //원할한 광고 수신을 위해 지역 고정
                string appId = VersionType == VersionTypes.AMK ? Constant.MS_MK_ADVERTISING_APPID : Constant.MS_TK_ADVERTISING_APPID;
                string unitId = VersionType == VersionTypes.AMK ? Constant.MS_MK_ADVERTISING_AD_UNIT_ID : Constant.MS_TK_ADVERTISING_AD_UNIT_ID;

                double width = 480;
                double height = 80;
                double txtTopMargin = 60;
                HorizontalAlignment hAlign = HorizontalAlignment.Left;
                VerticalAlignment vAlign = VerticalAlignment.Top;
                Thickness margin = new Thickness(0);

                //마우스 안내 문구 위치 이동
                Thickness txtMargin = txtInfomation.Margin;
                txtMargin.Top += txtTopMargin;
                txtInfomation.Margin = txtMargin;

                Grid adGrd = new Grid();
                adGrd.VerticalAlignment = System.Windows.VerticalAlignment.Top;
                adGrd.HorizontalAlignment = System.Windows.HorizontalAlignment.Stretch;
                
                //my ad
                ImageButton myAdCtrl = new ImageButton();
                myAdCtrl.Width = width;
                myAdCtrl.Height = height;
                myAdCtrl.HorizontalAlignment = hAlign;
                myAdCtrl.VerticalAlignment = vAlign;
                myAdCtrl.Margin = margin;
                BitmapImage tn = new BitmapImage();
                tn.SetSource(Application.GetResourceStream(UIUtils.GetRelativeUri(ResourceUri.IMG_AD_PLACEHOLDER)).Stream);
                myAdCtrl.Image = tn;

                //AdControl msAdCtrl = new AdControl("test_client", "Image480_80", true);
                AdControl msAdCtrl = new AdControl(appId, unitId, true);
                AdDuplex.AdControl adDuplexCtrl = new AdDuplex.AdControl() { AppId = Constant.ADDUPLEX_ID };
                //adDuplexCtrl.IsTest = true;

                myAdCtrl.Tap += (s, e) =>
                {
                    //페이지 이동
                    WebBrowserTask wbt = new WebBrowserTask();
                    wbt.Uri = new Uri(string.Format("http://windowsphone.com/s?appId={0}", Constant.FULL_VERSION_APP_ID), UriKind.Absolute);
                    wbt.Show();

                    //광고바 숨김
                    LayoutRoot.Children.Remove(adGrd);
                    txtMargin.Top -= txtTopMargin;
                    txtInfomation.Margin = txtMargin;
                };

                //adduplex
                adDuplexCtrl.HorizontalAlignment = hAlign;
                adDuplexCtrl.VerticalAlignment = vAlign;
                adDuplexCtrl.Margin = margin;
                adDuplexCtrl.Width = width;
                adDuplexCtrl.Height = height;
                adDuplexCtrl.AdClick += (s, e) =>
                {
                    LayoutRoot.Children.Remove(adGrd);
                    txtMargin.Top -= txtTopMargin;
                    txtInfomation.Margin = txtMargin;
                };
                adDuplexCtrl.AdLoadingError += (s, e) =>
                {
                    if (e.Error.Message != "Ad control is hidden by other control")
                    {
                        Deployment.Current.Dispatcher.BeginInvoke(() =>
                        {
                            adGrd.Children.Remove(adDuplexCtrl);
                            //다음광고 호출
                            adGrd.Children.Add(myAdCtrl);
                        });
                    }
                };

                //ms ad
                msAdCtrl.HorizontalAlignment = hAlign;
                msAdCtrl.VerticalAlignment = vAlign;
                msAdCtrl.Margin = margin;
                msAdCtrl.Height = height;
                msAdCtrl.Width = width;
                msAdCtrl.IsAutoCollapseEnabled = false;
                msAdCtrl.ErrorOccurred += (s, e) =>
                {
                    //다음광고 호출
                    Deployment.Current.Dispatcher.BeginInvoke(() =>
                    {
                        adGrd.Children.Remove(msAdCtrl);
                        adGrd.Children.Add(adDuplexCtrl);
                    });

                    if (System.Diagnostics.Debugger.IsAttached)
                    {
                        MessageBox.Show("MS Advertising : " + e.Error.Message);
                    }
                };
                msAdCtrl.IsEngagedChanged += (s, e) =>
                {
                    LayoutRoot.Children.Remove(adGrd);
                    txtMargin.Top -= txtTopMargin;
                    txtInfomation.Margin = txtMargin;
                };

                TextBlock adTxt = new TextBlock();
                adTxt.Margin = new Thickness(485, 0, 0, 0);
                adTxt.HorizontalAlignment = System.Windows.HorizontalAlignment.Left;
                adTxt.VerticalAlignment = System.Windows.VerticalAlignment.Center;
                adTxt.Text = I18n.GetString("AdClickMessage");
                adTxt.TextWrapping = TextWrapping.Wrap;

                adGrd.Children.Add(msAdCtrl);
                adGrd.Children.Add(adTxt);
                LayoutRoot.Children.Add(adGrd);
            }
        }

        public static bool CheckTrialVersion(bool condition)
        {
            return IsTrial && condition;
        }

        public static bool CheckTrialVersion()
        {
            return CheckTrialVersion(true);
        }
    }
}