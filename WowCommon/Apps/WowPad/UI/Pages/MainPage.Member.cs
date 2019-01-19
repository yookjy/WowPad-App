using Apps.UI.Notification;
using Apps.WowPad.Model;
using Apps.WowPad.Type;
using Microsoft.Phone.Shell;
using System.ComponentModel;
using System.Threading;
using System.Windows.Controls;
using System.Windows.Media.Imaging;

namespace Apps.WowPad.UI.Pages
{
    public enum LoadingTypes
    {
        Connection,
        Keyboard
    }

    public class LoadingArgument
    {
        public Grid LoadingGrid { get; set; }
        public LoadingTypes LoadingType { get; set; }
        public object Result { get; set; }
    }

    public partial class MainPage
    {
        private TouchInfo[] touchInfos;

        private WriteableBitmap backgroundTouchScreenBitmap;

        private WriteableBitmap backgroundMouseBitmap;

        private ScreenTypes screenType;
        
        private bool isAnimating;

        private bool isLoaded;

        private TouchInfo primaryPointer;

        private TouchInfo secondaryPointer;

        private volatile bool isRefreshAll;

        private MessageTip msgTip;

        private static ManualResetEvent connCheckDone = new ManualResetEvent(false);

        private BackgroundWorker loadWorker;

        //앱바 어바웃 인덱스
        private int nPosAppBarAbout;
        //앱바 접속/끊기 인덱스
        private int nPosAppBarConnect;
        //앱바 셋팅 인덱스
        private int nPosAppBarSetting;
        //앱바 pc검색 인덱스
        private int nPosAppBarSearchPC;

        private ApplicationBarIconButton searchAppBarIconButton;

        private ApplicationBarIconButton deviceAppBarIconButton; 
    }
}
