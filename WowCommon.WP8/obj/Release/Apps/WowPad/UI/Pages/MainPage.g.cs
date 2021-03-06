﻿#pragma checksum "D:\Project\C#\WowPad\WowCommon.WP8\..\WowCommon\Apps\WowPad\UI\Pages\MainPage.xaml" "{406ea660-64cf-4c82-b6f0-42d48172a799}" "6D0FF14121EBE3C4A9D59F7648A20C67"
//------------------------------------------------------------------------------
// <auto-generated>
//     이 코드는 도구를 사용하여 생성되었습니다.
//     런타임 버전:4.0.30319.34003
//
//     파일 내용을 변경하면 잘못된 동작이 발생할 수 있으며, 코드를 다시 생성하면
//     이러한 변경 내용이 손실됩니다.
// </auto-generated>
//------------------------------------------------------------------------------

using Apps.UI;
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
    
    
    public partial class MainPage : Microsoft.Phone.Controls.PhoneApplicationPage {
        
        internal System.Windows.Media.Animation.Storyboard ShowKeyboard;
        
        internal System.Windows.Media.Animation.Storyboard HideKeyboard;
        
        internal System.Windows.Controls.Grid LayoutRoot;
        
        internal System.Windows.Controls.Image BackgroundImage;
        
        internal System.Windows.Controls.TextBlock txtInfomation;
        
        internal Apps.UI.ImageButton btnKeyboard;
        
        internal Apps.UI.ImageButton btnWindows;
        
        internal Apps.UI.ImageButton btnMenu;
        
        internal System.Windows.Controls.Grid keybdLayer;
        
        internal System.Windows.Controls.Grid layerPpt;
        
        internal Apps.UI.ImageButton btnPptShowSlideFirstPage;
        
        internal Apps.UI.ImageButton btnPptShowSlideCurrentPage;
        
        internal System.Windows.Controls.Grid layerLeftSlideshow;
        
        internal Apps.UI.ImageButton btnPptPointer;
        
        internal Apps.UI.ImageButton btnPptHand;
        
        internal Apps.UI.ImageButton btnPptBpen;
        
        internal Apps.UI.ImageButton btnPptLaser;
        
        internal Apps.UI.ImageButton btnPptIpen;
        
        internal Apps.UI.ImageButton btnPptHpen;
        
        internal Apps.UI.ImageButton btnPptPrev;
        
        internal System.Windows.Controls.Grid layerRightSlideshow;
        
        internal Apps.UI.ImageButton btnPptCloseSlide;
        
        internal Apps.UI.ImageButton btnPptEraser;
        
        internal Apps.UI.ImageButton btnPptRemover;
        
        internal Apps.UI.ImageButton btnPptToggleInk;
        
        internal Apps.UI.ImageButton btnPptNext;
        
        internal System.Windows.Controls.TextBlock txtTrial;
        
        internal System.Windows.Controls.StackPanel layerGettingStart;
        
        internal System.Windows.Controls.HyperlinkButton lnkShareDownload;
        
        internal System.Windows.Controls.HyperlinkButton lnkHelp;
        
        internal System.Windows.Controls.HyperlinkButton lnkClose;
        
        internal System.Windows.Controls.HyperlinkButton lnkHide;
        
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
            System.Windows.Application.LoadComponent(this, new System.Uri("/WowCommon;component/Apps/WowPad/UI/Pages/MainPage.xaml", System.UriKind.Relative));
            this.ShowKeyboard = ((System.Windows.Media.Animation.Storyboard)(this.FindName("ShowKeyboard")));
            this.HideKeyboard = ((System.Windows.Media.Animation.Storyboard)(this.FindName("HideKeyboard")));
            this.LayoutRoot = ((System.Windows.Controls.Grid)(this.FindName("LayoutRoot")));
            this.BackgroundImage = ((System.Windows.Controls.Image)(this.FindName("BackgroundImage")));
            this.txtInfomation = ((System.Windows.Controls.TextBlock)(this.FindName("txtInfomation")));
            this.btnKeyboard = ((Apps.UI.ImageButton)(this.FindName("btnKeyboard")));
            this.btnWindows = ((Apps.UI.ImageButton)(this.FindName("btnWindows")));
            this.btnMenu = ((Apps.UI.ImageButton)(this.FindName("btnMenu")));
            this.keybdLayer = ((System.Windows.Controls.Grid)(this.FindName("keybdLayer")));
            this.layerPpt = ((System.Windows.Controls.Grid)(this.FindName("layerPpt")));
            this.btnPptShowSlideFirstPage = ((Apps.UI.ImageButton)(this.FindName("btnPptShowSlideFirstPage")));
            this.btnPptShowSlideCurrentPage = ((Apps.UI.ImageButton)(this.FindName("btnPptShowSlideCurrentPage")));
            this.layerLeftSlideshow = ((System.Windows.Controls.Grid)(this.FindName("layerLeftSlideshow")));
            this.btnPptPointer = ((Apps.UI.ImageButton)(this.FindName("btnPptPointer")));
            this.btnPptHand = ((Apps.UI.ImageButton)(this.FindName("btnPptHand")));
            this.btnPptBpen = ((Apps.UI.ImageButton)(this.FindName("btnPptBpen")));
            this.btnPptLaser = ((Apps.UI.ImageButton)(this.FindName("btnPptLaser")));
            this.btnPptIpen = ((Apps.UI.ImageButton)(this.FindName("btnPptIpen")));
            this.btnPptHpen = ((Apps.UI.ImageButton)(this.FindName("btnPptHpen")));
            this.btnPptPrev = ((Apps.UI.ImageButton)(this.FindName("btnPptPrev")));
            this.layerRightSlideshow = ((System.Windows.Controls.Grid)(this.FindName("layerRightSlideshow")));
            this.btnPptCloseSlide = ((Apps.UI.ImageButton)(this.FindName("btnPptCloseSlide")));
            this.btnPptEraser = ((Apps.UI.ImageButton)(this.FindName("btnPptEraser")));
            this.btnPptRemover = ((Apps.UI.ImageButton)(this.FindName("btnPptRemover")));
            this.btnPptToggleInk = ((Apps.UI.ImageButton)(this.FindName("btnPptToggleInk")));
            this.btnPptNext = ((Apps.UI.ImageButton)(this.FindName("btnPptNext")));
            this.txtTrial = ((System.Windows.Controls.TextBlock)(this.FindName("txtTrial")));
            this.layerGettingStart = ((System.Windows.Controls.StackPanel)(this.FindName("layerGettingStart")));
            this.lnkShareDownload = ((System.Windows.Controls.HyperlinkButton)(this.FindName("lnkShareDownload")));
            this.lnkHelp = ((System.Windows.Controls.HyperlinkButton)(this.FindName("lnkHelp")));
            this.lnkClose = ((System.Windows.Controls.HyperlinkButton)(this.FindName("lnkClose")));
            this.lnkHide = ((System.Windows.Controls.HyperlinkButton)(this.FindName("lnkHide")));
        }
    }
}

