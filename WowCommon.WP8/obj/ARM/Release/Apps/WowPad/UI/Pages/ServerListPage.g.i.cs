﻿#pragma checksum "D:\Project\C#\WowPad\WowCommon.WP8\..\WowCommon\Apps\WowPad\UI\Pages\ServerListPage.xaml" "{406ea660-64cf-4c82-b6f0-42d48172a799}" "AA1D7EDAA3AE4E5FFC557150F4FF941A"
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
    
    
    public partial class ServerListPage : Microsoft.Phone.Controls.PhoneApplicationPage {
        
        internal System.Windows.Controls.Grid LayoutRoot;
        
        internal Microsoft.Phone.Controls.Pivot txtAppName;
        
        internal Microsoft.Phone.Controls.PivotItem SearchPivot;
        
        internal System.Windows.Controls.Grid SearchContentPanel;
        
        internal System.Windows.Controls.TextBox hostTxtBox;
        
        internal System.Windows.Controls.TextBox portTxtBox;
        
        internal System.Windows.Controls.Button findServerBtn;
        
        internal System.Windows.Controls.ProgressBar findProgressBar;
        
        internal System.Windows.Controls.TextBlock labelProgressBar;
        
        internal System.Windows.Controls.ListBox ItemViewOnPage;
        
        internal Microsoft.Phone.Controls.PivotItem HistoryPivot;
        
        internal System.Windows.Controls.Grid HistoryContentPanel;
        
        internal System.Windows.Controls.ListBox historyItemViewOnPage;
        
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
            System.Windows.Application.LoadComponent(this, new System.Uri("/WowCommon;component/Apps/WowPad/UI/Pages/ServerListPage.xaml", System.UriKind.Relative));
            this.LayoutRoot = ((System.Windows.Controls.Grid)(this.FindName("LayoutRoot")));
            this.txtAppName = ((Microsoft.Phone.Controls.Pivot)(this.FindName("txtAppName")));
            this.SearchPivot = ((Microsoft.Phone.Controls.PivotItem)(this.FindName("SearchPivot")));
            this.SearchContentPanel = ((System.Windows.Controls.Grid)(this.FindName("SearchContentPanel")));
            this.hostTxtBox = ((System.Windows.Controls.TextBox)(this.FindName("hostTxtBox")));
            this.portTxtBox = ((System.Windows.Controls.TextBox)(this.FindName("portTxtBox")));
            this.findServerBtn = ((System.Windows.Controls.Button)(this.FindName("findServerBtn")));
            this.findProgressBar = ((System.Windows.Controls.ProgressBar)(this.FindName("findProgressBar")));
            this.labelProgressBar = ((System.Windows.Controls.TextBlock)(this.FindName("labelProgressBar")));
            this.ItemViewOnPage = ((System.Windows.Controls.ListBox)(this.FindName("ItemViewOnPage")));
            this.HistoryPivot = ((Microsoft.Phone.Controls.PivotItem)(this.FindName("HistoryPivot")));
            this.HistoryContentPanel = ((System.Windows.Controls.Grid)(this.FindName("HistoryContentPanel")));
            this.historyItemViewOnPage = ((System.Windows.Controls.ListBox)(this.FindName("historyItemViewOnPage")));
        }
    }
}

