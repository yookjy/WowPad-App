using Apps.UI.Keyboard.Interface;
using Apps.UI.Keyboard.Key;
using Apps.WowPad.Type;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;

namespace Apps.UI.Keyboard.Layout.Language
{
    abstract partial class AbstractKeyboardLayout : StackPanel, IKeyboardData, IKeyboardLayout, INotifyPropertyChanged
    {
        abstract public List<KeyCap[]> GetKeyCapList();

        abstract public string LanguageLabelText { get; }

        abstract public string SymbolLabelText { get; }

        abstract public KeyboardLayoutTypes KeyboardLayoutType { get; }

        public KeyboardLayoutTypes OriginalKeyboardLayoutType { get; set; }

        private IKeyboardLayout symbolKeyboard;

        private IKeyboardLayout languageKeyboard;

        public event PropertyChangedEventHandler PropertyChanged;

        virtual protected void OnPropertyChanged(PropertyChangedEventArgs e)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, e);
            }
        }

        new public Visibility Visibility
        {
            get
            {
                return base.Visibility;
            }

            set
            {
                base.Visibility = value;
                OnPropertyChanged(new PropertyChangedEventArgs("Visibility"));
            }
        }

        public IKeyboardLayout SymbolKeyboard 
        {
            get
            {
                return this.symbolKeyboard;
            }
            set
            {
                this.symbolKeyboard = value;
            }
        }

        public IKeyboardLayout FunctionKeyboard { get; set; }

        public IKeyboardLayout LanguageKeyboard
        {
            get
            {
                return this.languageKeyboard;
            }
            set
            {
                this.languageKeyboard = value;
            }
        }

        public string ThemeDirectory { get; set; }
                        
        public AbstractKeyboardLayout()
        {
            //테마 색상폴더 지정
            this.ThemeDirectory = (Visibility)Application.Current.Resources["PhoneDarkThemeVisibility"] == Visibility.Visible ? "dark" : "light";
            //키캡 배치
            AllocateKeyCap();
            //이벤트 핸들러는 AbstractKeyboardLayout.Event.cs
            this.SizeChanged += AbstractKeyboardLayout_SizeChanged;
            this.PropertyChanged += AbstractKeyboardLayout_PropertyChanged;
        }

        public void AllocateKeyCap()
        {
            List<KeyCap[]> keyCapList = GetKeyCapList();

            foreach (KeyCap[] row in keyCapList)
            {
                Grid gr = new Grid();
                int col = 0;
                foreach (KeyCap keyCap in row)
                {
                    gr.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(1, GridUnitType.Auto) });
                    gr.Children.Add(keyCap);
                    
                    keyCap.SetValue(Grid.ColumnProperty, col++);
                    //키캡 이벤트 등록
                    keyCap.MouseEnter += keyCap_MouseEnter; ;
                    keyCap.MouseLeave += keyCap_MouseLeave;
                    //keyCap.MouseLeftButtonDown += keyCap_MouseLeftButtonDown;
                    //keyCap.MouseLeftButtonUp += keyCap_MouseLeftButtonUp;
                    keyCap.Tap += keyCap_Tap;
                    switch (keyCap.KeyCapType)
                    {
                        case KeyCapTypes.Language:
                            keyCap.ActivateKeyCap(false, ThemeDirectory);
                            break;
                        case KeyCapTypes.Symbol:
                            keyCap.ActivateKeyCap(false, ThemeDirectory);
                            break;
                        case KeyCapTypes.Function:
                            keyCap.ActivateKeyCap(true, ThemeDirectory);
                            break;
                        case KeyCapTypes.Shift:
                            keyCap.ActivateKeyCap(true, ThemeDirectory);
                            break;
                        case KeyCapTypes.Control:
                        case KeyCapTypes.Alt:
                        case KeyCapTypes.Window:
                            keyCap.ActivateKeyCap(true, ThemeDirectory);
                            break;
                        default:
                            keyCap.ActivateKeyCap(false, ThemeDirectory);
                            break;
                    }
                }
                this.Children.Add(gr);
            }
        }

        virtual public void UpdateKeyboardLayout()
        {
            //사이즈 및 Locale, 심벌 버튼 레이블 변경
            double height = (this.ActualHeight) / this.Children.Count;
            foreach (Grid grdRow in this.Children)
            {
                double sumWidth = 0;
                double width = this.ActualWidth / 10;

                foreach (KeyCap keyCap in grdRow.Children)
                {
                    keyCap.Width = width * keyCap.WidthRatio;
                    keyCap.Height = height;
                    sumWidth += keyCap.Width;

                    UpdateKeyCapLayout(keyCap, false);
                }

                if (sumWidth < this.ActualWidth)
                {
                    Thickness margin = grdRow.Margin;
                    margin.Left += ((this.ActualWidth - sumWidth) / 2);
                    if (margin.Left * 2 + sumWidth == this.ActualWidth) 
                        grdRow.Margin = margin;
                }
            }
        }

        virtual public void UpdateKeyCapLayout(KeyCap keyCap, bool isShift)
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
                    grd.Children.RemoveAt(0);
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

                int len = label.Length;
                //업데이트할 텍스트가 없으면 리턴
                if (len == 0) return;

                if (grd.Children.Count == 0)
                {
                    grd.Children.Add(new TextBlock());
                }
                else if (!(grd.Children[0] is TextBlock))
                {
                    grd.Children.RemoveAt(0);
                    grd.Children.Add(new TextBlock());
                }
                TextBlock tbUp = grd.Children[0] as TextBlock;

                button.FontWeight = FontWeights.SemiBold;
                //글짜수에 따르 크기 변경
                //button.FontSize = keyCap.GetFontSize(label) * (Apps.WowPad.Manager.SettingManager.Instance.SettingInfo.FullSizeKeyboard ? 1 : 0.9);
                button.FontSize = keyCap.GetFontSize(label);

                tbUp.VerticalAlignment = System.Windows.VerticalAlignment.Center;
                tbUp.HorizontalAlignment = System.Windows.HorizontalAlignment.Center;
                tbUp.Text = label;

                Grid.SetRowSpan(tbUp, 2);
                Grid.SetColumnSpan(tbUp, 2);
            }
        }

        virtual public void ResetKeyboardLayout()
        {
            bool isReseted = false;
            foreach (Grid grdRow in this.Children)
            {
                foreach (KeyCap keyCap in grdRow.Children)
                {
                    UpdateKeyCapLayout(keyCap, false);

                    if (keyCap.LayoutRoot.Children[0] is ToggleButton)
                    {
                        if (!isReseted && keyCap.KeyCapType != KeyCapTypes.Function)
                            isReseted = (bool)(keyCap.LayoutRoot.Children[0] as ToggleButton).IsChecked;

                        (keyCap.LayoutRoot.Children[0] as ToggleButton).IsChecked = false;
                    }
                }
            }

            if (isReseted)
            {
                KeyCapEventArgs args = new KeyCapEventArgs();
                args.KeyCapType = KeyCapTypes.Esc;
                args.KeyCode = 41;  //ESC코드
                OnKeyboardLayoutReseted(args);
            }
        }

        virtual public void PrepareKeyboardLayout()
        {
            int cnt = 0;
            foreach (Grid grdRow in this.Children)
            {
                foreach (KeyCap keyCap in grdRow.Children)
                {
                    if (keyCap.KeyCapType == KeyCapTypes.Language)
                    {
                        switch (this.KeyboardLayoutType)
                        {
                            case KeyboardLayoutTypes.Symbol:
                                keyCap.Text = this.SymbolKeyboard.LanguageKeyboard.LanguageLabelText;
                                break;
                            case KeyboardLayoutTypes.Function:
                                keyCap.Text = this.FunctionKeyboard.LanguageKeyboard.LanguageLabelText;
                                break;
                            default:
                                keyCap.Text = this.LanguageKeyboard.LanguageLabelText;
                                break;
                        }
                        UpdateKeyCapLayout(keyCap, false);
                        cnt++;
                    }
                    else if (keyCap.KeyCapType == KeyCapTypes.Symbol)
                    {
                        switch (this.KeyboardLayoutType)
                        {
                            case KeyboardLayoutTypes.Symbol:
                                keyCap.Text = this.SymbolKeyboard.SymbolLabelText;
                                break;
                            case KeyboardLayoutTypes.Function:
                                keyCap.Text = this.FunctionKeyboard.SymbolKeyboard.SymbolLabelText;
                                break;
                            default:
                                keyCap.Text = this.SymbolKeyboard.SymbolLabelText;
                                break;
                        }
                        UpdateKeyCapLayout(keyCap, false);
                        cnt++;
                    }
                    if (cnt == 2) return;
                }
            }
        }
    }
}
