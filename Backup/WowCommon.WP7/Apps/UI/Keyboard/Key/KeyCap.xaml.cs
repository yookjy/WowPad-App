using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace Apps.UI.Keyboard.Key
{
    public partial class KeyCap : UserControl
    {
        //공통 사항
        public KeyCapTypes KeyCapType { get; set; }

        public bool IsHidden { get; set; }

        public bool IsHiddenToolTip { get; set; }

        //메인 정보
        public byte KeyCode { get; set; }

        public string Text { get; set; }

        public BitmapImage BKImage { get; set; }

        public string SubText { get; set; }

        public BitmapImage SubBKImage { get; set; }

        //키 크기 배율
        public double WidthRatio { get; set; }


        public KeyCap()
        {
            InitializeComponent();
            //기본값 설정
            this.WidthRatio = 1;
            this.Text = string.Empty;
            this.SubText = string.Empty;
        }

        public double GetFontSize(string text)
        {
            int length = text.Length;
            double fontRatio = 1;
            if (this.WidthRatio == 1)
            {
                if (length > 10) fontRatio = 0.6;
                else if (length > 3) fontRatio = 0.7;
                else if (length > 2) fontRatio = 0.8;
                else if (length > 1) fontRatio = 1;
                else fontRatio = 1.1;
            }
            else
            {
                fontRatio = 1.1;
            }
            return fontRatio * (double)Application.Current.Resources["PhoneFontSizeMedium"];
        }

        public void ActivateKeyCap(bool IsToggleButton, string themeDir)
        {
            ContentControl button = null;
            if (IsToggleButton)
            {
                button = new ToggleButton();
                button.Style = (Style)this.Resources["ToggleKeyCapStyle"];
                button.SetValue(Button.ClickModeProperty, ClickMode.Hover);
            }
            else
            {
                button = new Button();
                button.Style = (Style)this.Resources["KeyCapStyle"];
                button.SetValue(Button.ClickModeProperty, ClickMode.Hover);
            }

            if (this.LayoutRoot.Children.Count > 0)
            {
                this.LayoutRoot.Children.Clear();
            }

            Grid grd = new Grid();
            grd.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(2, GridUnitType.Star) });
            grd.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(1, GridUnitType.Star) });
            grd.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(2, GridUnitType.Star) });
            grd.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(1, GridUnitType.Star) });
            
            button.BorderThickness = new Thickness();
            button.Content = grd;
                     
            this.LayoutRoot.Children.Add(button);
            
            if (this.IsHidden)
            {
                this.LayoutRoot.Style = (Style)Application.Current.Resources["Transparent"];
            }
            else
            {
                Style style = (Style)this.Resources["GridStyle"];
                this.LayoutRoot.Style = style;
                if (themeDir != "dark")
                {
                    this.LayoutRoot.Background = new SolidColorBrush(Colors.White);
                }
            }

            this.IsEnabled = (!this.IsHidden && this.IsEnabled);
        }


        protected override void OnMouseEnter(System.Windows.Input.MouseEventArgs e)
        {
            base.OnMouseEnter(e);

            if (!this.IsHiddenToolTip)
            {
                //눌렸을때 툴팁표시
                ContentControl btn = this.LayoutRoot.Children[0] as ContentControl;
                Canvas tooltip = new Canvas();
                Grid grd = new Grid();
                Border bd = new Border();
                Grid btnContent = btn.Content as Grid;
                Grid ttGrd = new Grid();
                ttGrd.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(2, GridUnitType.Star) });
                ttGrd.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(1, GridUnitType.Star) });
                ttGrd.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(2, GridUnitType.Star) });
                ttGrd.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(1, GridUnitType.Star) });

                if (btnContent.Children[0] is Image)
                {
                    Image img = new Image();
                    Image btnImage = btnContent.Children[0] as Image;
                    img.Source = btnImage.Source;
                    img.Width = btnImage.Width * 1.4;
                    img.Height = btnImage.Height * 1.4;
                    ttGrd.Children.Add(img);
                }
                else
                {
                    TextBlock tbUp = new TextBlock();
                    TextBlock btnTbUp = btnContent.Children[0] as TextBlock;

                    tbUp.FontSize = btnTbUp.FontSize * 1.4;
                    tbUp.FontWeight = FontWeights.Bold;
                    tbUp.Text = btnTbUp.Text;
                    ttGrd.Children.Add(tbUp);
                }

                Grid.SetRow(ttGrd.Children[0] as FrameworkElement, 0);
                Grid.SetColumn(ttGrd.Children[0] as FrameworkElement, 0);

                if (btnContent.Children.Count == 2)
                {
                    (ttGrd.Children[0] as TextBlock).HorizontalAlignment = System.Windows.HorizontalAlignment.Left;
                    (ttGrd.Children[0] as TextBlock).Padding = new Thickness(10, 0, 0, 0);

                    TextBlock tbDn = new TextBlock();
                    TextBlock btnTbDn = btnContent.Children[1] as TextBlock;
                    tbDn.HorizontalAlignment = System.Windows.HorizontalAlignment.Right;
                    tbDn.Margin = new Thickness(0, -10, 0, 0);
                    tbDn.Padding = new Thickness(0, 0, 10, 0);
                    tbDn.FontSize = btnTbDn.FontSize * 1.4;
                    tbDn.FontWeight = FontWeights.Bold;
                    tbDn.Foreground = new SolidColorBrush(Color.FromArgb(255, 220, 220, 220));
                    tbDn.Text = btnTbDn.Text;
                    ttGrd.Children.Add(tbDn);
                    
                    Grid.SetRow(tbDn, 1);
                    Grid.SetColumn(tbDn, 1);
                }
                else
                {
                    ttGrd.VerticalAlignment = System.Windows.VerticalAlignment.Center;
                    ttGrd.HorizontalAlignment = System.Windows.HorizontalAlignment.Center;

                    Grid.SetRowSpan(ttGrd.Children[0] as FrameworkElement, 2);
                    Grid.SetColumnSpan(ttGrd.Children[0] as FrameworkElement, 2);
                }

                bd.BorderThickness = (Thickness)Application.Current.Resources["PhoneBorderThickness"];
                bd.BorderBrush = (Brush)Application.Current.Resources["PhoneChromeBrush"];

                grd.Width = btn.ActualWidth * 1.2;
                grd.Height = btn.ActualHeight * 1.4;
                grd.Background = (Brush)Application.Current.Resources["PhoneAccentBrush"];

                tooltip.HorizontalAlignment = System.Windows.HorizontalAlignment.Left;
                tooltip.VerticalAlignment = System.Windows.VerticalAlignment.Bottom;
                tooltip.Width = btn.ActualWidth * 1.2;
                tooltip.Height = btn.ActualHeight * 1.4;
                tooltip.Margin = new Thickness((btn.ActualWidth - tooltip.Width) / 2 - bd.BorderThickness.Left,
                    -this.LayoutRoot.Margin.Top - tooltip.Height - bd.BorderThickness.Top * 2,
                    0,
                    btn.ActualHeight + bd.BorderThickness.Bottom * 2 + this.LayoutRoot.Margin.Bottom);

                grd.Children.Add(ttGrd);
                bd.Child = grd;
                tooltip.Children.Add(bd);
                this.LayoutRoot.Children.Add(tooltip);
            }
        }

        protected override void OnMouseLeave(System.Windows.Input.MouseEventArgs e)
        {
            base.OnMouseLeave(e);

            if (!this.IsHiddenToolTip)
            {
                //뗐을때 툴팁 제거
                if (this.LayoutRoot.Children.Count() > 1)
                {
                    this.LayoutRoot.Children.RemoveAt(1);
                }
            }
        }

        protected override void OnMouseLeftButtonDown(System.Windows.Input.MouseButtonEventArgs e)
        {
            base.OnMouseLeftButtonDown(e);
            
        }

        protected override void OnMouseLeftButtonUp(System.Windows.Input.MouseButtonEventArgs e)
        {
            base.OnMouseLeftButtonUp(e);
        }
    }
}
