﻿#pragma checksum "D:\Project\C#\WowPad\WowCommon.WP8\..\WowCommon\Apps\WowPad\UI\Pages\HelpPage.xaml" "{406ea660-64cf-4c82-b6f0-42d48172a799}" "D7B7F028E2006F56825DB60272368F7F"
//------------------------------------------------------------------------------
// <auto-generated>
//     이 코드는 도구를 사용하여 생성되었습니다.
//     런타임 버전:4.0.30319.34011
//
//     파일 내용을 변경하면 잘못된 동작이 발생할 수 있으며, 코드를 다시 생성하면
//     이러한 변경 내용이 손실됩니다.
// </auto-generated>
//------------------------------------------------------------------------------

using Microsoft.Phone.Controls;
using System;
using System.Windows;
using System.Windows.Automation;
using System.Windows.Automation.Peers;
using System.Windows.Automation.Provider;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Resources;
using System.Windows.Shapes;
using System.Windows.Threading;


namespace Apps.WowPad.UI.Pages {
    
    
    public partial class HelpPage : Microsoft.Phone.Controls.PhoneApplicationPage {
        
        internal System.Windows.Controls.Grid LayoutRoot;
        
        internal System.Windows.Controls.TextBlock txtAppName;
        
        internal System.Windows.Controls.ScrollViewer ContentPanel;
        
        internal Microsoft.Phone.Controls.WebBrowser webBrowser;
        
        internal System.Windows.Controls.Grid loadingGrid;
        
        internal System.Windows.Controls.ProgressBar loadingProgressBar;
        
        internal System.Windows.Controls.TextBlock loadingLabel;
        
        private bool _contentLoaded;
        
        /// <summary>
        /// InitializeComponent
        /// </summary>
        [System.Diagnostics.DebuggerNonUserCodeAttribute()]
        public void InitializeComponent() {
            if (_contentLoaded) {
                return;
            }
            _contentLoaded = true;
            System.Windows.Application.LoadComponent(this, new System.Uri("/WowCommon;component/Apps/WowPad/UI/Pages/HelpPage.xaml", System.UriKind.Relative));
            this.LayoutRoot = ((System.Windows.Controls.Grid)(this.FindName("LayoutRoot")));
            this.txtAppName = ((System.Windows.Controls.TextBlock)(this.FindName("txtAppName")));
            this.ContentPanel = ((System.Windows.Controls.ScrollViewer)(this.FindName("ContentPanel")));
            this.webBrowser = ((Microsoft.Phone.Controls.WebBrowser)(this.FindName("webBrowser")));
            this.loadingGrid = ((System.Windows.Controls.Grid)(this.FindName("loadingGrid")));
            this.loadingProgressBar = ((System.Windows.Controls.ProgressBar)(this.FindName("loadingProgressBar")));
            this.loadingLabel = ((System.Windows.Controls.TextBlock)(this.FindName("loadingLabel")));
        }
    }
}

