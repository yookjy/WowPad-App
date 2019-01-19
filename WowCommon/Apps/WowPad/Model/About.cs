using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Apps.WowPad.Resources;
using Apps.WowPad.Util;

namespace Apps.WowPad.Model
{
    public class About
    {
        public About()
        {
            AddVersions();
        }

        private void AddVersions()
        {
            VersionList = new ObservableCollection<Version<VersionContent>>();

            Version<VersionContent> version276 = new Version<VersionContent>("2.7.6");
            version276.Add(new VersionContent(VersionContentType.MOD, I18n.GetString("AboutUpdateHistory27601")));

            Version<VersionContent> version275 = new Version<VersionContent>("2.7.5");
            version275.Add(new VersionContent(VersionContentType.NEW, I18n.GetString("AboutUpdateHistory27501")));
            
            Version<VersionContent> version270 = new Version<VersionContent>("2.7.0");
            version270.Add(new VersionContent(VersionContentType.NEW, I18n.GetString("AboutUpdateHistory27001")));

            Version<VersionContent> version265 = new Version<VersionContent>("2.6.5");
            version265.Add(new VersionContent(VersionContentType.NEW, I18n.GetString("AboutUpdateHistory26501")));
            version265.Add(new VersionContent(VersionContentType.NEW, I18n.GetString("AboutUpdateHistory26502")));
            version265.Add(new VersionContent(VersionContentType.FIX, I18n.GetString("AboutUpdateHistory26503")));
            version265.Add(new VersionContent(VersionContentType.MOD, I18n.GetString("AboutUpdateHistory26504")));
            version265.Add(new VersionContent(VersionContentType.IMP, I18n.GetString("AboutUpdateHistory26505")));

            Version<VersionContent> version250 = new Version<VersionContent>("2.5.0");
            version250.Add(new VersionContent(VersionContentType.NEW, I18n.GetString("AboutUpdateHistory25001")));
            version250.Add(new VersionContent(VersionContentType.NEW, I18n.GetString("AboutUpdateHistory25002")));
            version250.Add(new VersionContent(VersionContentType.NEW, I18n.GetString("AboutUpdateHistory25003")));
            version250.Add(new VersionContent(VersionContentType.NEW, I18n.GetString("AboutUpdateHistory25004")));
            version250.Add(new VersionContent(VersionContentType.NEW, I18n.GetString("AboutUpdateHistory25005")));
            version250.Add(new VersionContent(VersionContentType.NEW, I18n.GetString("AboutUpdateHistory25007")));
            version250.Add(new VersionContent(VersionContentType.MOD, I18n.GetString("AboutUpdateHistory25006")));
            version250.Add(new VersionContent(VersionContentType.MOD, I18n.GetString("AboutUpdateHistory25008")));


            Version<VersionContent> version200 = new Version<VersionContent>("2.0.0");
            version200.Add(new VersionContent(VersionContentType.NEW, I18n.GetString("AboutUpdateHistory20001")));

            VersionList.Add(version276);
            VersionList.Add(version275);
            VersionList.Add(version270);
            VersionList.Add(version265);
            VersionList.Add(version250);
            VersionList.Add(version200);
        }

        public ObservableCollection<Version<VersionContent>> VersionList { get; private set; }

        public string CurrentVersion
        {
            get
            {
                string assName = ("WowPad " + VersionUtils.Version).Trim();
                Version version = new AssemblyName(Assembly.Load(assName).FullName).Version;
                return string.Format("{0}.{1}.{2}", version.Major, version.Minor, version.Build);

            }
        }

        public BitmapImage RateReview
        {
            get
            {
                return GetImage("appbar.star.png");
            }
        }

        public BitmapImage Facebook
        {
            get
            {
                return GetImage("appbar.social.facebook.variant.png");
            }
        }

        public BitmapImage Feedback
        {
            get
            {
                return GetImage("appbar.customerservice.png");
            }
        }

        public BitmapImage ShareApp
        {
            get
            {
                return GetImage("appbar.share.png");
            }
        }

        public BitmapImage VisitHomePage
        {
            get
            {
                return GetImage("appbar.home.empty.png");
            }
        }

        private BitmapImage GetImage(string name)
        {
            BitmapImage bi = new BitmapImage();
            bi.UriSource = new Uri(GetFullPath(name), UriKind.Relative);
            return bi;;
        }

        public string GetFullPath(string path)
        {
            string fullPath = "/Images/{0}/Appbar/{1}";
            fullPath = string.Format(fullPath, (Visibility)Application.Current.Resources["PhoneDarkThemeVisibility"] == Visibility.Visible ? "dark" : "light", path);
            return fullPath;
        }

    }
}
