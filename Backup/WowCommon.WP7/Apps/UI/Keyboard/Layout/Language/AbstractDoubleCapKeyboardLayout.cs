using Apps.UI.Keyboard.Key;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace Apps.UI.Keyboard.Layout.Language
{
    abstract partial class AbstractDoubleCapKeyboardLayout : AbstractKeyboardLayout
    {
        public override void UpdateKeyCapLayout(KeyCap keyCap, bool isShift)
        {
            ContentControl button = keyCap.LayoutRoot.Children[0] as ContentControl;
            Grid grd = button.Content as Grid;
            button.Padding = new Thickness();
            grd.Width = keyCap.Width - keyCap.LayoutRoot.Margin.Left - keyCap.LayoutRoot.Margin.Right;
            grd.Height = keyCap.Height - keyCap.LayoutRoot.Margin.Top - keyCap.LayoutRoot.Margin.Bottom;

            if ((keyCap.BKImage != null && !isShift)
                || (keyCap.SubBKImage != null && isShift))
            {
                Image img = null;
                if (grd.Children.Count == 0)
                {
                    grd.Children.Add(new Image());
                }
                else if (!(grd.Children[0] is Image))
                {
                    grd.Children.Clear();
                    grd.Children.Add(new Image());
                }

                img = grd.Children[0] as Image;
                img.Source = !isShift ? keyCap.BKImage : keyCap.SubBKImage;
                img.Width = grd.Width;
                img.Height = grd.Height;

                Grid.SetRowSpan(img, 2);
                Grid.SetColumnSpan(img, 2);
            }
            else
            {
                //사용될 텍스트
                string label = !isShift ? keyCap.Text : keyCap.SubText;
                //업데이트할 텍스트가 없으면 리턴
                if (label.Length == 0) return;

                if (grd.Children.Count == 0)
                {
                    grd.Children.Add(new TextBlock());
                }
                if (!(grd.Children[0] is TextBlock))
                {
                    grd.Children.RemoveAt(0);
                    grd.Children.Add(new TextBlock());
                }
                TextBlock tbUp = grd.Children[0] as TextBlock;
                Border bd = new Border();
                Grid.SetRow(tbUp, 0);
                Grid.SetColumn(tbUp, 0);

                button.FontWeight = FontWeights.SemiBold;
                //글짜수에 따르 크기 변경
                button.FontSize = keyCap.GetFontSize(label) * (Apps.WowPad.Manager.SettingManager.Instance.SettingInfo.FullSizeKeyboard ? 1 : 0.9);

                string[] labels = label.Split("|".ToArray());

                if (labels.Length < 2)
                {
                    tbUp.VerticalAlignment = System.Windows.VerticalAlignment.Center;
                    tbUp.HorizontalAlignment = System.Windows.HorizontalAlignment.Center;
                    Grid.SetRowSpan(tbUp, 2);
                    Grid.SetColumnSpan(tbUp, 2);
                    tbUp.Text = label;
                }
                else
                {
                    string txtUp = string.Empty;
                    string txtDn = string.Empty;

                    if (labels.Length == 2)
                    {
                        txtUp = labels[0];
                        txtDn = labels[1];
                    }
                    else
                    {
                        txtUp = (string.IsNullOrEmpty(labels[0])) ? "|" : labels[0];
                        txtDn = (string.IsNullOrEmpty(labels[0])) ? labels[2] : "|";
                    }

                    if (grd.Children.Count == 1)
                    {
                        grd.Children.Add(new TextBlock());
                    }
                    else if (!(grd.Children[1] is TextBlock))
                    {
                        grd.Children.RemoveAt(1);
                        grd.Children.Add(new TextBlock());
                    }

                    TextBlock tbDn = grd.Children[1] as TextBlock;
                    Grid.SetRow(tbDn, 1);
                    Grid.SetColumn(tbDn, 1);

                    tbUp.HorizontalAlignment = System.Windows.HorizontalAlignment.Center;
                    tbUp.VerticalAlignment = System.Windows.VerticalAlignment.Top;
                    tbUp.FontSize = (Apps.WowPad.Manager.SettingManager.Instance.SettingInfo.FullSizeKeyboard ? 1.1 : 0.9) *(double)Application.Current.Resources["PhoneFontSizeMedium"];
                    tbUp.Text = txtUp;

                    tbDn.HorizontalAlignment = System.Windows.HorizontalAlignment.Center;
                    tbDn.VerticalAlignment = System.Windows.VerticalAlignment.Top;
                    tbDn.Margin = new Thickness(0, -10, 0, 0);
                    tbDn.FontSize = (Apps.WowPad.Manager.SettingManager.Instance.SettingInfo.FullSizeKeyboard ? 0.8 : 0.6) * (double)Application.Current.Resources["PhoneFontSizeMedium"]; ;
                    tbDn.Foreground = new SolidColorBrush(Color.FromArgb(255, 180, 180, 180));
                    tbDn.Text = txtDn;
                }
            }
        }
    }
}
