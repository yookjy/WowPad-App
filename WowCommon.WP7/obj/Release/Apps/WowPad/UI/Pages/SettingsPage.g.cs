﻿#pragma checksum "D:\Project\C#\WowPad\WowCommon.WP7\..\WowCommon\Apps\WowPad\UI\Pages\SettingsPage.xaml" "{406ea660-64cf-4c82-b6f0-42d48172a799}" "F64B9499A7B893CB4F1FDC163341B6C3"
//------------------------------------------------------------------------------
// <auto-generated>
//     이 코드는 도구를 사용하여 생성되었습니다.
//     런타임 버전:4.0.30319.34003
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
    
    
    public partial class SettingsPage : Microsoft.Phone.Controls.PhoneApplicationPage {
        
        internal System.Windows.Controls.Grid LayoutRoot;
        
        internal System.Windows.DataTemplate PickerItemTemplate;
        
        internal System.Windows.DataTemplate PickerFullModeItemTemplate;
        
        internal Microsoft.Phone.Controls.Pivot txtSettingPageTitle;
        
        internal Microsoft.Phone.Controls.ToggleSwitch extendToggleBtn;
        
        internal Microsoft.Phone.Controls.ListPicker imgQualityPicker;
        
        internal Microsoft.Phone.Controls.ToggleSwitch autoReconnToggleBtn;
        
        internal Microsoft.Phone.Controls.ListPicker deviceTypePicker;
        
        internal System.Windows.Controls.TextBox defaultPortTxtBox;
        
        internal Microsoft.Phone.Controls.ListPicker defaultPagePicker;
        
        internal Microsoft.Phone.Controls.ListPicker dataUsagePicker;
        
        internal Microsoft.Phone.Controls.ToggleSwitch fullSizeKeybdToggleBtn;
        
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
            System.Windows.Application.LoadComponent(this, new System.Uri("/WowCommon;component/Apps/WowPad/UI/Pages/SettingsPage.xaml", System.UriKind.Relative));
            this.LayoutRoot = ((System.Windows.Controls.Grid)(this.FindName("LayoutRoot")));
            this.PickerItemTemplate = ((System.Windows.DataTemplate)(this.FindName("PickerItemTemplate")));
            this.PickerFullModeItemTemplate = ((System.Windows.DataTemplate)(this.FindName("PickerFullModeItemTemplate")));
            this.txtSettingPageTitle = ((Microsoft.Phone.Controls.Pivot)(this.FindName("txtSettingPageTitle")));
            this.extendToggleBtn = ((Microsoft.Phone.Controls.ToggleSwitch)(this.FindName("extendToggleBtn")));
            this.imgQualityPicker = ((Microsoft.Phone.Controls.ListPicker)(this.FindName("imgQualityPicker")));
            this.autoReconnToggleBtn = ((Microsoft.Phone.Controls.ToggleSwitch)(this.FindName("autoReconnToggleBtn")));
            this.deviceTypePicker = ((Microsoft.Phone.Controls.ListPicker)(this.FindName("deviceTypePicker")));
            this.defaultPortTxtBox = ((System.Windows.Controls.TextBox)(this.FindName("defaultPortTxtBox")));
            this.defaultPagePicker = ((Microsoft.Phone.Controls.ListPicker)(this.FindName("defaultPagePicker")));
            this.dataUsagePicker = ((Microsoft.Phone.Controls.ListPicker)(this.FindName("dataUsagePicker")));
            this.fullSizeKeybdToggleBtn = ((Microsoft.Phone.Controls.ToggleSwitch)(this.FindName("fullSizeKeybdToggleBtn")));
        }
    }
}
