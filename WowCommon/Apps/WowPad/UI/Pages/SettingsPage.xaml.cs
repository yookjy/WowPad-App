using Apps.WowPad.Manager;
using Apps.WowPad.Model;
using Apps.WowPad.Resources;
using Apps.WowPad.Type;
using Apps.WowPad.Util;
using Microsoft.Phone.Controls;
using System;
using System.Collections.Generic;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Navigation;

namespace Apps.WowPad.UI.Pages
{
    public partial class SettingsPage : PhoneApplicationPage
    {
        private object imgQuality = 0;

        public SettingsPage()
        {
            InitializeComponent();

            //타이틀 설정
            txtSettingPageTitle.Title = string.Format(I18n.GetString("SettingsPageTitle"), " " + VersionUtils.Version);

            List<PickerItem> imgQualitySource = new List<PickerItem>();
            imgQualitySource.Add(new PickerItem() { Name = I18n.GetString("SettingsPagePivotTouchPadBackgroundQualityLow"), Key = ImageQualityTypes.Low });
            imgQualitySource.Add(new PickerItem() { Name = I18n.GetString("SettingsPagePivotTouchPadBackgroundQualityMedium"), Key = ImageQualityTypes.Medium });
            imgQualitySource.Add(new PickerItem() { Name = I18n.GetString("SettingsPagePivotTouchPadBackgroundQualityHigh"), Key = ImageQualityTypes.High });
            imgQualityPicker.ItemsSource = imgQualitySource;

            List<PickerItem> defaultPageSource = new List<PickerItem>();
            defaultPageSource.Add(new PickerItem() { Name = I18n.GetString("ServerListPageTitle"), Key = 0 });
            defaultPageSource.Add(new PickerItem() { Name = I18n.GetString("HistoryListPageTitle"), Key = 1 });
            defaultPagePicker.ItemsSource = defaultPageSource;

            List<PickerItem> dataUsageSource = new List<PickerItem>();
            dataUsageSource.Add(new PickerItem() { Name = I18n.GetString("SettingsPagePivotEtcDataUsageLevel2"), Key = 100 });
            dataUsageSource.Add(new PickerItem() { Name = I18n.GetString("SettingsPagePivotEtcDataUsageLevel3"), Key = 250 });
            dataUsageSource.Add(new PickerItem() { Name = I18n.GetString("SettingsPagePivotEtcDataUsageLevel4"), Key = 500 });
            dataUsagePicker.ItemsSource = dataUsageSource;

            if (VersionUtils.IsFull)
            {
                List<PickerItem> deviceTypeSource = new List<PickerItem>();
                deviceTypeSource.Add(new PickerItem() { Name = I18n.GetString("AppBarButtonTextMouse"), Key = DeviceTypes.Mouse });
                deviceTypeSource.Add(new PickerItem() { Name = I18n.GetString("AppBarButtonTextTouchScreen"), Key = DeviceTypes.TouchScreen });
                deviceTypePicker.ItemsSource = deviceTypeSource;

                if (VersionUtils.IsTrial)
                {
                    //터치패드 설정
                    extendToggleBtn.Header = I18n.GetString("SettingsPagePivotTouchPadNavigationButton") + " (" + I18n.GetString("AppMessageTrialCannotuse") + ")";
                    imgQualityPicker.Header = I18n.GetString("SettingsPagePivotTouchPadBackgroundQuality") + " (" + I18n.GetString("AppMessageTrialCannotuse") + ")";
                    SettingManager.Instance.UpdateNLoad(SettingManager.KEY_USE_EXTEND_BUTTON, false);
                    SettingManager.Instance.UpdateNLoad(SettingManager.KEY_IMAGE_QUALITY, ImageQualityTypes.Low);
                    extendToggleBtn.IsEnabled = false;
                    imgQualityPicker.IsEnabled = false;

                    //연결정보설정
                    autoReconnToggleBtn.Header = I18n.GetString("SettingsPagePivotConnectionAutoConnect") + " (" + I18n.GetString("AppMessageTrialCannotuse") + ")";
                    deviceTypePicker.Header = I18n.GetString("SettingsPagePivotConnectionDeviceType") + " (" + I18n.GetString("AppMessageTrialCannotuse") + ")";
                    SettingManager.Instance.UpdateNLoad(SettingManager.KEY_AUTO_CONNECT, false);
                    SettingManager.Instance.UpdateNLoad(SettingManager.KEY_DEVICE_TYPE, DeviceTypes.Mouse);
                    autoReconnToggleBtn.IsEnabled = false;
                    deviceTypePicker.IsEnabled = false;

                    //기타설정
                    defaultPagePicker.Header = I18n.GetString("SettingsPagePivotEtcDefaultPage") + " (" + I18n.GetString("AppMessageTrialCannotuse") + ")";
                    dataUsagePicker.Header = I18n.GetString("SettingsPagePivotEtcDataUsage") + " (" + I18n.GetString("AppMessageTrialCannotuse") + ")";
                    fullSizeKeybdToggleBtn.Header = I18n.GetString("SettingsPagePivotEtcFullSizeKeyboard") + " (" + I18n.GetString("AppMessageTrialCannotuse") + ")";
                    SettingManager.Instance.UpdateNLoad(SettingManager.KEY_SEARCH_DEFAULT_PAGE, 0);
                    SettingManager.Instance.UpdateNLoad(SettingManager.KEY_CELLULAR_DATA_USAGE, 100);
                    SettingManager.Instance.UpdateNLoad(SettingManager.KEY_FULLSIZE_KEYBOARD, true);
                    defaultPagePicker.IsEnabled = false;
                    dataUsagePicker.IsEnabled = false;
                    fullSizeKeybdToggleBtn.IsEnabled = false;
                }
            }
            else
            {
                if (VersionUtils.IsTouchscreen)
                {
                    UIUtils.SetVisibility(extendToggleBtn, false);
                }
                else
                {
                    UIUtils.SetVisibility(imgQualityPicker, false);
                }

                UIUtils.SetVisibility(deviceTypePicker, false);
            }
        }
        
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            
            if (e.NavigationMode == NavigationMode.New)
            {
                //이미지 품질 기본값 설정
                foreach (PickerItem pi in imgQualityPicker.Items)
                {
                    if ((ImageQualityTypes)pi.Key == SettingManager.Instance.SettingInfo.ImageQualityType)
                    {
                        imgQualityPicker.SelectedItem = pi;
                        //기본 이미지 품질값 
                        imgQuality = pi.Key;
                        break;
                    }
                }
                //장치모드 기본값 설정
                foreach (PickerItem pi in deviceTypePicker.Items)
                {
                    if ((DeviceTypes)pi.Key == SettingManager.Instance.SettingInfo.DeviceType)
                    {
                        deviceTypePicker.SelectedItem = pi;
                        break;
                    }
                }
                //검색 페이지 기본값 설정
                foreach (PickerItem pi in defaultPagePicker.Items)
                {
                    if ((int)pi.Key == SettingManager.Instance.SettingInfo.SearchDefaultPageIndex)
                    {
                        defaultPagePicker.SelectedItem = pi;
                        break;
                    }
                }
                //데이터 사용량 기본값 설정
                foreach (PickerItem pi in dataUsagePicker.Items)
                {
                    if ((int)pi.Key == SettingManager.Instance.SettingInfo.CellularDataUsage)
                    {
                        dataUsagePicker.SelectedItem = pi;
                        break;
                    }
                }
                //포트 기본값 설정
                defaultPortTxtBox.Text = Convert.ToString(SettingManager.Instance.SettingInfo.DefaultPort);
                //자동 접속 기본값 설정
                autoReconnToggleBtn.IsChecked = SettingManager.Instance.SettingInfo.AutoConnect;
                //마우스 브라우저 버튼 기본값 설정
                extendToggleBtn.IsChecked = SettingManager.Instance.SettingInfo.UseExtendButton;
                //키보드 크기 기본값 설정
                fullSizeKeybdToggleBtn.IsChecked = SettingManager.Instance.SettingInfo.FullSizeKeyboard;
            }
        }

        private void defaultPortTxtBox_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (System.Text.RegularExpressions.Regex.IsMatch(e.Key.ToString(), "[0-9]"))
                e.Handled = false;
            else e.Handled = true;
        }

        private void imgQualityPicker_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (imgQualityPicker.SelectedItem != null && e.RemovedItems.Count > 0 && e.AddedItems.Count > 0)
            {
                PickerItem pickerItem = imgQualityPicker.SelectedItem as PickerItem;
                ImageQualityTypes imgQualityType = (ImageQualityTypes)pickerItem.Key;
                //변경된 설정 저장
                SettingManager.Instance.UpdateNLoad(SettingManager.KEY_IMAGE_QUALITY, imgQualityType);
                //변경값 포인팅 매니져로 전달
                PointingControlManager.Instance.ImageQualityType = imgQualityType;
            }
        }

        private void deviceTypePicker_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (deviceTypePicker.SelectedItem != null && e.RemovedItems.Count > 0 && e.AddedItems.Count > 0)
            {
                SettingManager.Instance.UpdateNLoad(SettingManager.KEY_DEVICE_TYPE, ((PickerItem)deviceTypePicker.SelectedItem).Key);
            }
        }

        private void defaultPagePicker_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (defaultPagePicker.SelectedItem != null && e.RemovedItems.Count > 0 && e.AddedItems.Count > 0)
            {
                PickerItem pickerItem = defaultPagePicker.SelectedItem as PickerItem;
                int pageIndex = (int)pickerItem.Key;
                SettingManager.Instance.UpdateNLoad(SettingManager.KEY_SEARCH_DEFAULT_PAGE, pageIndex);
            }
        }

        private void dataUsagePicker_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (dataUsagePicker.SelectedItem != null && e.RemovedItems.Count > 0 && e.AddedItems.Count > 0)
            {
                PickerItem pickerItem = dataUsagePicker.SelectedItem as PickerItem;
                int perDataUsage = (int)pickerItem.Key;
                SettingManager.Instance.UpdateNLoad(SettingManager.KEY_CELLULAR_DATA_USAGE, perDataUsage);
            }
        }

        private void defaultPortTxtBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            String strNum = String.IsNullOrEmpty(defaultPortTxtBox.Text.Trim()) ? Constant.DEFAULT_PORT : defaultPortTxtBox.Text;

            try
            {
                if (Convert.ToInt32(strNum) <= IPEndPoint.MaxPort)
                {
                    SettingManager.Instance.UpdateNLoad(SettingManager.KEY_DEFAULT_PORT, Convert.ToInt32(strNum));
                }
            }
            catch (Exception e1)
            {
                System.Diagnostics.Debug.WriteLine(e1.Message);
            }
        }

        private void extendToggleBtn_Checked(object sender, RoutedEventArgs e)
        {
            SettingManager.Instance.UpdateNLoad(SettingManager.KEY_USE_EXTEND_BUTTON, true);
        }

        private void extendToggleBtn_Unchecked(object sender, RoutedEventArgs e)
        {
            SettingManager.Instance.UpdateNLoad(SettingManager.KEY_USE_EXTEND_BUTTON, false);
        }

        private void autoReconnToggleBtn_Checked(object sender, RoutedEventArgs e)
        {
            SettingManager.Instance.UpdateNLoad(SettingManager.KEY_AUTO_CONNECT, true);
        }

        private void autoReconnToggleBtn_Unchecked(object sender, RoutedEventArgs e)
        {
            SettingManager.Instance.UpdateNLoad(SettingManager.KEY_AUTO_CONNECT, false);
        }

        private void fullSizeKeybdToggleBtn_Checked(object sender, RoutedEventArgs e)
        {
            SettingManager.Instance.UpdateNLoad(SettingManager.KEY_FULLSIZE_KEYBOARD, true);
        }

        private void fullSizeKeybdToggleBtn_Unchecked(object sender, RoutedEventArgs e)
        {
            SettingManager.Instance.UpdateNLoad(SettingManager.KEY_FULLSIZE_KEYBOARD, false);
        }
    }
}