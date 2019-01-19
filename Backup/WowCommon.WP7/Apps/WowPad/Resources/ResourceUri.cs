using Apps.WowPad.Model;
using Apps.WowPad.Util;
using System;
using System.Windows;

namespace Apps.WowPad.Resources
{
    public class ResourceUri
    {
        public static void Load()
        {
            string dir = (Visibility)Application.Current.Resources["PhoneDarkThemeVisibility"] == Visibility.Visible ? "dark" : "light";
            IMG_APPBAR_TOUCH = string.Format("Images/{0}/Appbar/appbar.cursor.hand.png", dir);
            IMG_APPBAR_MOUSE = string.Format("Images/{0}/Appbar/appbar.cursor.default.png", dir);
            IMG_APPBAR_CONNECT = string.Format("Images/{0}/Appbar/appbar.connect.png", dir);
            IMG_APPBAR_DISCONNECT = string.Format("Images/{0}/Appbar/appbar.disconnect.png", dir);
            IMG_APPBAR_SETTINGS = string.Format("Images/{0}/Appbar/appbar.settings.png", dir);
            IMG_APPBAR_PCSEARCH = string.Format("Images/{0}/Appbar/appbar.magnify.png", dir);
            IMG_APPBAR_POWER = string.Format("Images/{0}/Appbar/appbar.power.png", dir);
            IMG_APPBAR_DELETE = string.Format("Images/{0}/Appbar/appbar.close.png", dir);
            IMG_APPBAR_ABOUT = string.Format("Images/{0}/Appbar/appbar.information.png", dir);

            IMG_TOUCHSCREEN_BACKGROUND = string.Format("Images/{0}/touch_background_{1}.png", dir, DeviceInfo.ScreenHeight * DeviceInfo.ScaleFactor / 100);
            IMG_MOUSE_BACKGROUND = string.Format("Images/{0}/mouse_background_{1}.png", dir, DeviceInfo.ScreenHeight * DeviceInfo.ScaleFactor / 100);

            IMG_AD_PLACEHOLDER = string.Format("Images/{0}/ad_placeholder.png", dir);

            AboutImageUri = new Uri(IMG_APPBAR_ABOUT, UriKind.Relative);
            ConnectImageUri = new Uri(IMG_APPBAR_CONNECT, UriKind.Relative);
            DisconnectImageUri = new Uri(IMG_APPBAR_DISCONNECT, UriKind.Relative);
            TouchscreenImageUri = new Uri(IMG_APPBAR_TOUCH, UriKind.Relative);
            MouseImageUri = new Uri(IMG_APPBAR_MOUSE, UriKind.Relative);
            PowerImageUri = new Uri(IMG_APPBAR_POWER, UriKind.Relative);
            DeleteImageUri = new Uri(IMG_APPBAR_DELETE, UriKind.Relative);
        }

        public static string IMG_APPBAR_TOUCH;

        public static string IMG_APPBAR_MOUSE;

        public static string IMG_APPBAR_CONNECT;

        public static string IMG_APPBAR_DISCONNECT;

        public static string IMG_APPBAR_SETTINGS;

        public static string IMG_APPBAR_PCSEARCH;

        public static string IMG_APPBAR_POWER;

        public static string IMG_APPBAR_DELETE;

        public static string IMG_APPBAR_ABOUT;
        
        public static string IMG_MOUSE_BACKGROUND;

        public static string IMG_TOUCHSCREEN_BACKGROUND;
        
        public static string IMG_AD_PLACEHOLDER;

        public static Uri AboutImageUri;

        public static Uri ConnectImageUri;

        public static Uri DisconnectImageUri;

        public static Uri TouchscreenImageUri;

        public static Uri MouseImageUri;
        
        public static Uri PowerImageUri;

        public static Uri DeleteImageUri;

        public static Uri MainPageUri = new Uri(Constant.PAGE_MAIN, UriKind.Relative);

        public static Uri SettingPageUri = new Uri("/WowCommon;component/Apps/WowPad/UI/Pages/SettingsPage.xaml", UriKind.Relative);

        public static Uri ServerListPageUri = new Uri("/WowCommon;component/Apps/WowPad/UI/Pages/ServerListPage.xaml", UriKind.Relative);

        public static Uri HelpPageUri = new Uri("/WowCommon;component/Apps/WowPad/UI/Pages/HelpPage.xaml", UriKind.Relative);

        public static Uri AboutPageUri = new Uri("/WowCommon;component/Apps/WowPad/UI/Pages/AboutPage.xaml", UriKind.Relative);
    }
}
