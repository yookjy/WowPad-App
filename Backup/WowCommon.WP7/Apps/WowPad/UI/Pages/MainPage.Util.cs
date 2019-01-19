using Apps.UI.Notification;
using Apps.WowPad.Manager;
using Apps.WowPad.Model;
using Apps.WowPad.Resources;
using Apps.WowPad.Type;
using Apps.WowPad.Util;
using Microsoft.Phone.Shell;
using System;
using System.Collections.Generic;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;

namespace Apps.WowPad.UI.Pages
{
    public partial class MainPage
    {
        //앱바 버튼이 연결 버튼인가를 판별
        private bool IsConnectAppBarButton(ApplicationBarIconButton appBarIconBtn)
        {
            return appBarIconBtn.IconUri.OriginalString == ResourceUri.IMG_APPBAR_CONNECT;
        }

        //앱바 버튼이 끊기 버튼인가를 판별
        private bool IsDisconnectAppBarButton(ApplicationBarIconButton appBarIconBtn)
        {
            return appBarIconBtn.IconUri.OriginalString == ResourceUri.IMG_APPBAR_DISCONNECT;
        }

        //앱바 버튼이 터치스크린 버튼인가를 판별
        private bool IsTouchScreenAppBarButton(ApplicationBarIconButton appBarIconBtn)
        {
            return appBarIconBtn.IconUri.OriginalString == ResourceUri.IMG_APPBAR_TOUCH;
        }

        //앱바 버튼에 따른 장치 타입을 조회
        private DeviceTypes GetPointingDeviceType(ApplicationBarIconButton appBarIconBtn)
        {
            return IsTouchScreenAppBarButton(appBarIconBtn) ? DeviceTypes.TouchScreen : DeviceTypes.Mouse;
        }

        private void AddChild(UIElement elem)
        {
            if (!this.LayoutRoot.Children.Contains(elem))
            {
                this.LayoutRoot.Children.Add(elem);
            }
        }

        private void RemoveChild(UIElement elem)
        {
            if (this.LayoutRoot.Children.Contains(elem))
            {
                this.LayoutRoot.Children.Remove(elem);
            }
        }

        private void RemoveChild(UIElement elem, bool condition)
        {
            if (condition)
            {
                RemoveChild(elem);
            }
        }

        //모든 버튼을 숨김 상태로 처리한다.
        private void HideAllChildren()
        {
            //현재 버전에서 불필요한 레이아웃 삭제
            RemoveChild(txtTrial, !VersionUtils.IsTrial);
            RemoveChild(layerGettingStart, !SettingManager.Instance.SettingInfo.GettingStart);
            RemoveChild(layerPpt);
            RemoveChild(layerLeftSlideshow);
            RemoveChild(layerRightSlideshow);

            foreach (UIElement elem in LayoutRoot.Children)
            {
                if ((elem is Image && (elem as Image).Name == "BackgroundImage")
                    || (elem is TextBlock && VersionUtils.IsTrial && (elem as TextBlock).Name == "txtTrial")
                    || (elem is MessageTip)
                    )
                    continue;

                UIUtils.SetVisibility(elem, false);
            }
        }

        //스크린 모드를 설정하고 화면을 초기화한다.
        private void SetScreenMode(ScreenTypes screenType)
        {
            //상태 저장
            this.screenType = screenType;
            System.Diagnostics.Debug.WriteLine(screenType.ToString());
            switch (screenType)
            {
                case ScreenTypes.PowerPoint:
                    SetPPTSlideShowButtonVisibility(false);
                    SetPPTButtonVisibilitiy(true);
                    break;
                case ScreenTypes.PowerPointSlideShow2007:
                    HideAllChildren();
                    SetPPTButtonVisibilitiy(false);
                    SetPPTSlideShowButtonVisibility(true);
                    break;
                case ScreenTypes.PowerPointSlideShow2010:
                    HideAllChildren();
                    SetPPTButtonVisibilitiy(false);
                    SetPPTSlideShowButtonVisibility(true);
                    //UIUtils.SetVisibility(btnPptBpen, false);
                    break;
                case ScreenTypes.PowerPointSlideShow2013:
                    HideAllChildren();
                    SetPPTButtonVisibilitiy(false);
                    SetPPTSlideShowButtonVisibility(true);
                    break;
                default:
                    SetPPTButtonVisibilitiy(false);
                    SetPPTSlideShowButtonVisibility(false);
                    break;
            }
        }

        //파워포인트 관련 버튼의 Visibliltiy를 설정한다.
        private void SetPPTButtonVisibilitiy(bool isVisible)
        {
            if (isVisible)
            {
                AddChild(layerPpt);
                UIUtils.SetVisibility(layerPpt, isVisible);
            }
            else
            {
                RemoveChild(layerPpt);
            }
        }

        //파워포인트 슬라이드쇼 관련 버튼의 Visibliltiy를 설정한다.
        private void SetPPTSlideShowButtonVisibility(bool isVisible)
        {
            //PPT 슬라이드 쇼용 버튼
            UIUtils.SetVisibility(btnPptHand, isVisible);
            UIUtils.SetVisibility(btnPptPointer, isVisible);

            if (PointingControlManager.Instance.DeviceType == DeviceTypes.Mouse)
            {
                UIUtils.SetVisibility(btnPptHand, false);
            }
            else if (PointingControlManager.Instance.DeviceType == DeviceTypes.TouchScreen)
            {
                UIUtils.SetVisibility(btnPptPointer, false);
            }

            switch (this.screenType)
            {
                case ScreenTypes.PowerPointSlideShow2007:
                    UIUtils.SetVisibility(btnPptBpen, true);
                    UIUtils.SetVisibility(btnPptLaser, false);
                    break;
                case ScreenTypes.PowerPointSlideShow2010:
                    UIUtils.SetVisibility(btnPptBpen, false);
                    UIUtils.SetVisibility(btnPptLaser, true);
                    break;
                case ScreenTypes.PowerPointSlideShow2013:
                    UIUtils.SetVisibility(btnPptBpen, false);
                    UIUtils.SetVisibility(btnPptLaser, true);
                    break;
            }

            if (isVisible)
            {
                AddChild(layerLeftSlideshow);
                AddChild(layerRightSlideshow);
                UIUtils.SetVisibility(layerLeftSlideshow, isVisible);
                UIUtils.SetVisibility(layerRightSlideshow, isVisible);
            }
            else
            {
                RemoveChild(layerLeftSlideshow);
                RemoveChild(layerRightSlideshow);
            }
        }

        //터치한 영역이 UIElement와 겹치는가를 판별한다.
        private bool IsButtonArea(FrameworkElement elem, Point pt)
        {
            if (UIUtils.IsVisible(elem) && this.LayoutRoot.Children.Contains(elem))
            {
                GeneralTransform transform = elem.TransformToVisual(LayoutRoot);
                Point absolutePosition = transform.Transform(new Point(0, 0));

                if (absolutePosition.X <= pt.X && pt.X <= (absolutePosition.X + elem.ActualWidth)
                    && absolutePosition.Y <= pt.Y && pt.Y <= (absolutePosition.Y + elem.ActualHeight))
                {
                    return true;
                }
            }
            return false;
        }

        //터치한 영역이 모든 UIElement와 겹치는가를 판별한다.
        private bool IsButtonsArea(Point pt)
        {
            if (//항시 기본 버튼
                IsButtonArea(btnMenu, pt)
                //접속시 기본버튼
                || IsButtonArea(btnWindows, pt)
                || IsButtonArea(btnKeyboard, pt)
                //PPT 모드
                || IsButtonArea(layerPpt, pt)
                //PPT 슬라이드 모드
                || IsButtonArea(layerLeftSlideshow, pt)
                || IsButtonArea(layerRightSlideshow, pt)
                //키보드 영역
                || IsButtonArea(keybdLayer, pt)) return true;
            return false;
        }

        //키보드에 언어를 추가하고 키보드를 로드한다.
        private void SetKeyboardList(List<KeyboardLayoutTypes> keyboardList)
        {
            if (keyboardList == null)
            {
                keyboardList = new List<KeyboardLayoutTypes>();
            }

            Apps.UI.Keyboard.Keyboard keybd = keybdLayer.Children[0] as Apps.UI.Keyboard.Keyboard;
            //키보드 로드
            keybd.LoadKeyboardLayout(keyboardList);
        }

        //바이트배열을 이미지로 변환하여 백그라운드 이미지를 갱신한다.
        private void UpdateBackgroundImage(ImageInfo imageInfo)
        {
            using (MemoryStream ms = new MemoryStream(imageInfo.ImageBytes))
            {
                WriteableBitmap writeableBitmap = BackgroundImage.Source as WriteableBitmap;

                if (!imageInfo.IsXorImage)
                {
                    //최초의 경우
                    if (writeableBitmap == null)
                    {
                        writeableBitmap = (PointingControlManager.Instance.DeviceType == DeviceTypes.Mouse ? backgroundMouseBitmap : backgroundTouchScreenBitmap);
                    }
                    //전체 그리기
                    writeableBitmap = writeableBitmap.FromStream(ms);
                    BackgroundImage.Source = writeableBitmap;
                }
                else
                {
                    WriteableBitmap diff = writeableBitmap.FromStream(ms);

                    if (writeableBitmap.Pixels.Length != diff.Pixels.Length)
                        throw new Exception("images is not the same length !");

                    var diffPixels = diff.Pixels;
                    var referencePixels = writeableBitmap.Pixels;
                    int idx = referencePixels.Length - 1;

                    do //loop unrolling ftw :)
                    {
                        unchecked
                        {
                            referencePixels[idx] ^= diffPixels[idx];
                            referencePixels[idx] ^= (int)(uint)0xFF000000;
                            idx--;
                        }
                    } while (idx >= 0);

                    writeableBitmap.Invalidate();
                }
                isRefreshAll = false;
            }
        }

        //이미지 리소스의 경로로부터 백그라운드 이미지를 갱신한다.
        private void UpdateBackgroundImage(WriteableBitmap writeableBitmap)
        {
            BackgroundImage.Source = writeableBitmap;
        }

        //키보드 슬라이드 애니메이션
        private DoubleAnimationUsingKeyFrames GetKeyboardAnimation(bool isShow)
        {
            EasingDoubleKeyFrame sKeyFrame = new EasingDoubleKeyFrame();
            EasingDoubleKeyFrame eKeyFrame = new EasingDoubleKeyFrame();
            DoubleAnimationUsingKeyFrames animation = new DoubleAnimationUsingKeyFrames();

            sKeyFrame.KeyTime = KeyTime.FromTimeSpan(new TimeSpan(0, 0, 0));
            sKeyFrame.Value = isShow ? keybdLayer.ActualHeight : 0;
            eKeyFrame.KeyTime = KeyTime.FromTimeSpan(TimeSpan.FromMilliseconds(300));
            eKeyFrame.Value = isShow ? 0 : keybdLayer.ActualHeight;

            animation.KeyFrames.Add(sKeyFrame);
            animation.KeyFrames.Add(eKeyFrame);

            Storyboard.SetTarget(animation, keybdLayer);
            Storyboard.SetTargetProperty(animation, new PropertyPath("(UIElement.RenderTransform).(CompositeTransform.TranslateY)"));

            return animation;
        }

        //로딩 화면을 생성한다.
        private Grid CreateLoadingGrid(string txtKey)
        {
            Grid loadingGrd = new Grid() { HorizontalAlignment = HorizontalAlignment.Stretch, Background = new SolidColorBrush(Color.FromArgb(100, 0, 0, 0)) };
            loadingGrd.SetValue(Canvas.ZIndexProperty, 1000);
            ProgressBar loadingBar = new ProgressBar() { IsIndeterminate = true };
            TextBlock loadingTxt = new TextBlock() { VerticalAlignment = VerticalAlignment.Center, HorizontalAlignment = HorizontalAlignment.Center, Margin = new Thickness(0, 24, 0, 0) };

            loadingGrd.Children.Add(loadingBar);
            loadingGrd.Children.Add(loadingTxt);
            this.LayoutRoot.Children.Add(loadingGrd);
            loadingTxt.Text = I18n.GetString(txtKey);

            return loadingGrd;
        }
    }
}
