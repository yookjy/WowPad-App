using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using System.Reflection;
using Microsoft.Phone.Tasks;
using Microsoft.Phone.Info;
using Apps.WowPad.Resources;
using Apps.WowPad.Model;
using System.Windows.Media;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using Apps.WowPad.Util;

namespace Apps.WowPad.UI.Pages
{
    public partial class AboutPage : PhoneApplicationPage, INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        protected void NotifyPropertyChanged(String propertyName)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        private SolidColorBrush _LinkColor;

        public SolidColorBrush LinkColor
        {
            get
            {
                return _LinkColor;
            }
            set
            {
                if (_LinkColor != value)
                {
                    _LinkColor = value;
                    NotifyPropertyChanged("LinkColor");
                }
            }
        }

        public AboutPage()
        {
            InitializeComponent();

            DataContext = new About();
            /*
             * <toolkit:LongListSelector x:Name="LLSWhatsNew"
                                          GroupHeaderTemplate="{StaticResource LLSWhatsNewGroupHeaderTemplate}"
                                          ItemTemplate="{StaticResource LLSWhatsNewItemTemplate}"
                                          Margin="0,0,0,6"
                                          ItemsSource="{Binding VersionList}">
                </toolkit:LongListSelector>
             */
            DataTemplate dt = this.Resources["LLSWhatsNewGroupHeaderTemplate"] as DataTemplate;
            
            LongListSelector LLSWhatsNew = new LongListSelector()
            {
                Name = "LLSWhatsNew",
                GroupHeaderTemplate = this.Resources["LLSWhatsNewGroupHeaderTemplate"] as DataTemplate,
                ItemTemplate = this.Resources["LLSWhatsNewItemTemplate"] as DataTemplate,
                Margin = new Thickness(0, 0, 0, 6),
                ItemsSource = (DataContext as About).VersionList
            };
            PivotWhatsNew.Content = LLSWhatsNew; 
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
#if !WP7
            (PivotWhatsNew.Content as LongListSelector).IsGroupingEnabled = true;
#endif
            foreach (PivotItem pi in AboutPivot.Items)
            {
                AttachEventTextBlock(pi.Content as Panel);
            }
        }

        private void AttachEventTextBlock(Panel panel)
        {
            if (panel == null) return;

            foreach (UIElement elem in panel.Children)
            {
                if (elem is TextBlock && (elem as TextBlock).Tag as string == "link")
                {
                    elem.Tap += Link_Tap;
                    LinkColor = (SolidColorBrush)Application.Current.Resources["PhoneAccentBrush"];
                }
                else if (elem is Panel)
                {
                    AttachEventTextBlock(elem as Panel);
                }
            }
        }

        void Link_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            string name = (sender as TextBlock).Name;
            TextBlockTap(name);            
        }

        void TextBlockTap(string name)
        {
            if (name == DeveloperContact.Name)
            {
                ShowWebBrowserTask("https://twitter.com/yookjy");
            }
            else if (name == DesignerContact.Name)
            {
                ShowWebBrowserTask("https://www.facebook.com/100001136649926");
            }
            else if (name == RateReview.Name)
            {
                new MarketplaceReviewTask().Show();
            }
            else if (name == VisitHomePage.Name)
            {
                ShowWebBrowserTask("http://apps.velostep.com/wowpad");
            }
            else if (name == Facebook.Name)
            {
                ShowWebBrowserTask(Constant.FACEBOOK_SUPPORT);
            }
            else if (name == Feedback.Name)
            {
                string assName = ("WowPad " + VersionUtils.Version).Trim();
                EmailComposeTask emailComposeTask = new EmailComposeTask();
                emailComposeTask.To = Constant.EMAIL_SUPPORT;
                emailComposeTask.Subject = "Support";
                emailComposeTask.Body =
                    new System.Text.StringBuilder().Append(assName).Append(" ").AppendLine(new AssemblyName(Assembly.Load(assName).FullName).Version.ToString())
                    .AppendLine(System.Globalization.CultureInfo.CurrentCulture.EnglishName)
                    .AppendLine(DeviceStatus.DeviceName)
                    .AppendLine(DeviceStatus.DeviceFirmwareVersion)
                    .AppendLine(Environment.OSVersion.Version.ToString()).ToString();
                emailComposeTask.Show();
            }
            else if (name == ShareApp.Name)
            {
                ShareLinkTask slt = new ShareLinkTask();
                slt.LinkUri = new Uri(string.Format("http://www.windowsphone.com/s?appid={0}", Constant.FULL_VERSION_APP_ID));
                slt.Message = "This app is very useful!";
                slt.Title = "Share Wowpad app!";
                slt.Show();
            }
            else if (name == LibraryCreator1.Name)
            {
                ShowWebBrowserTask("http://phone.codeplex.com/");
            }
            else if (name == LibraryCreator2.Name)
            {
                ShowWebBrowserTask("http://writeablebitmapex.codeplex.com/");
            }
            //else if (name == LibraryCreator3.Name)
            //{
            //    ShowWebBrowserTask("http://james.newtonking.com/json");
            //}
            //else if (name == LibraryCreator4.Name)
            //{
            //    ShowWebBrowserTask("http://www.icsharpcode.net/");
            //}
            else if (name == SpecialPeople1.Name)
            {
            }
            //else if (name == SpecialPeople2.Name)
            //{
            //    ShowWebBrowserTask("https://www.facebook.com/100001136649926");
            //}
            //else if (name == SpecialPeople3.Name)
            //{
            //    ShowWebBrowserTask("http://dribbble.com/gilhyun");
            //}
        }

        private void ShowWebBrowserTask(string url)
        {
            WebBrowserTask wbt = new WebBrowserTask();
            wbt.Uri = new Uri(url, UriKind.Absolute);
            wbt.Show();
        }

        private void ScrollViewer_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            if (e.OriginalSource is TextBlock)
            {
                TextBlockTap((e.OriginalSource as TextBlock).Name);
            }
        }
    }
}