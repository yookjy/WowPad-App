using Apps.WowPad.Model;
using Apps.WowPad.Type;
using Microsoft.Phone.Shell;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Shapes;

namespace Apps.WowPad.Util
{
    public class UIUtils
    {
        //Relative url을 리턴한다.
        public static Uri GetRelativeUri(string uriString)
        {
            return new Uri(uriString, UriKind.Relative);
        }

        //앱바에 아이콘 버튼을 추가하고 이벤트를 연결한다.
        public static ApplicationBarIconButton CreateAppBarIconButton(string label, Uri imgUri, EventHandler eventHandler)
        {
            ApplicationBarIconButton appBarIconBtn = new ApplicationBarIconButton();
            appBarIconBtn.IconUri = imgUri;
            appBarIconBtn.Text = label;
            appBarIconBtn.Click += eventHandler;
            return appBarIconBtn;
        }

        //앱바에 아이콘 버튼을 추가하고 이벤트를 연결한다.
        public static ApplicationBarIconButton CreateAppBarIconButton(string label, string imgPath, EventHandler eventHandler)
        {
            return UIUtils.CreateAppBarIconButton(label, UIUtils.GetRelativeUri(imgPath), eventHandler);
        }

        //앱바 아이콘 버튼의 텍스트와 이미지를 변경한다.
        public static void UpdateAppBarIconButton(ApplicationBarIconButton appBarIconBtn, string label, Uri imageUri)
        {
            if (appBarIconBtn.Text != label)
            {
                appBarIconBtn.Text = label;
                appBarIconBtn.IconUri = imageUri;
            }
        }

        //앱바 아이콘 버튼의 텍스트와 이미지를 변경한다.
        public static void UpdateAppBarIconButton(ApplicationBarIconButton appBarIconBtn, string label, string imgPath)
        {
            UpdateAppBarIconButton(appBarIconBtn, label, GetRelativeUri(imgPath));
        }

        //앱바에 메뉴아이템을 추가하고 이벤트를 연결한다.
        public static ApplicationBarMenuItem CreateAppBarMenuItem(string label, EventHandler eventHandler)
        {
            ApplicationBarMenuItem appBarMenuItem = new ApplicationBarMenuItem();
            appBarMenuItem.Text = label;
            appBarMenuItem.Click += eventHandler;
            return appBarMenuItem;
        }


        //UIElement의 Sub class들의 Visibility를 변경한다.
        public static void SetVisibility(UIElement elem, bool isVisible)
        {
            if (isVisible && elem.Visibility == Visibility.Collapsed)
            {
                elem.Visibility = System.Windows.Visibility.Visible;
            }
            else if (!isVisible && elem.Visibility == Visibility.Visible)
            {
                elem.Visibility = System.Windows.Visibility.Collapsed;
            }
        }

        //UIElement의 Sub class들의 Visibility를 변경하고 문자열을 설정한다.
        public static void SetVisibility(UIElement elem, bool isVisible, string text)
        {
            SetVisibility(elem, isVisible);

            if (elem is TextBlock && (elem as TextBlock).Text != text)
            {
                (elem as TextBlock).Text = text;
            }
            else if (elem is TextBox && (elem as TextBox).Text != text)
            {
                (elem as TextBox).Text = text;
            }
            else if (elem is ContentControl && ((string)(elem as ContentControl).Content) != text)
            {
                (elem as ContentControl).Content = text;
            }
        }

        //UIElement의 Sub class들의 Visibility를 조회한다.
        public static bool IsVisible(UIElement elem)
        {
            return elem.Visibility == System.Windows.Visibility.Visible;
        }

        //포인터 표시 이미지의 좌표를 설정한다.
        public static void SetPosition(Rectangle rect, TouchInfo ti)
        {
            rect.Margin = new Thickness(ti.X - rect.Width / 2, ti.Y - rect.Height / 2, 0, 0);
        }
    }
}