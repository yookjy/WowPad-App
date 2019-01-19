using Apps.UI.Keyboard.Interface;
using Apps.UI.Keyboard.Key;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;

namespace Apps.UI.Keyboard.Layout.Language
{
    abstract partial class AbstractKeyboardLayout
    {
        public event KeyCapEventHandler KeyCapPressed;

        public event KeyCapEventHandler KeyCapReleased;

        public event KeyCapEventHandler KeyCapChecked;

        public event KeyCapEventHandler KeyCapUnchecked;

        public event KeyCapEventHandler LanguageKeyCapTap;

        public event KeyCapEventHandler SymbolKeyCapTap;

        public event KeyCapEventHandler FunctionKeyCapChecked;

        public event KeyCapEventHandler FunctionKeyCapUnchecked;

        public event KeyCapEventHandler KeyboardLayoutReseted;

        void AbstractKeyboardLayout_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "Visibility")
            {
                if (this.Visibility == System.Windows.Visibility.Collapsed)
                {
                    this.ResetKeyboardLayout();
                }
                else
                {
                    this.PrepareKeyboardLayout();
                }
            }
        }

        void AbstractKeyboardLayout_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            UpdateKeyboardLayout();
        }

        protected virtual void OnKeyCapPressed(KeyCapEventArgs e)
        {
            if (KeyCapPressed != null)
            {
                KeyCapPressed(this, e);
            }
        }

        protected virtual void OnKeyCapReleased(KeyCapEventArgs e)
        {
            if (KeyCapReleased != null)
            {
                KeyCapReleased(this, e);
            }
        }

        protected virtual void OnKeyCapChecked(KeyCapEventArgs e)
        {
            if (KeyCapChecked != null)
            {
                KeyCapChecked(this, e);
            }
        }

        protected virtual void OnKeyCapUnchecked(KeyCapEventArgs e)
        {
            if (KeyCapUnchecked != null)
            {
                KeyCapUnchecked(this, e);
            }
        }

        protected virtual void OnSymbolKeyCapTap(KeyCapEventArgs e)
        {
            if (SymbolKeyCapTap != null)
            {
                SymbolKeyCapTap(this, e);
            }
        }

        protected virtual void OnLanguageKeyCapTap(KeyCapEventArgs e)
        {
            if (LanguageKeyCapTap != null)
            {
                LanguageKeyCapTap(this, e);
            }
        }

        protected virtual void OnFunctionKeyCapChecked(KeyCapEventArgs e)
        {
            if (FunctionKeyCapChecked != null)
            {
                FunctionKeyCapChecked(this, e);
            }
        }

        protected virtual void OnFunctionKeyCapUnchecked(KeyCapEventArgs e)
        {
            if (FunctionKeyCapUnchecked != null)
            {
                FunctionKeyCapUnchecked(this, e);
            }
        }

        protected virtual void OnKeyboardLayoutReseted(KeyCapEventArgs e)
        {
            if (KeyboardLayoutReseted != null)
            {
                KeyboardLayoutReseted(this, e);
            }
        }

        void keyCap_MouseEnter(object sender, System.Windows.Input.MouseEventArgs e)
        {
            KeyCap keyCap = sender as KeyCap;
            ContentControl cc = keyCap.LayoutRoot.Children[0] as ContentControl;
            //데이터 셋팅
            KeyCapEventArgs keyCapEventArgs = GetKeyCapEventArgs(keyCap);
            //키데이터 셋팅

            if (cc is Button)
            {
                OnKeyCapPressed(keyCapEventArgs);
            }
            else if (cc is ToggleButton && (cc as ToggleButton).IsChecked == true)
            {
                if (keyCap.KeyCapType == KeyCapTypes.Function)
                {
                    OnFunctionKeyCapChecked(keyCapEventArgs);
                }
                else
                {
                    OnKeyCapChecked(keyCapEventArgs);

                    if (keyCapEventArgs.KeyCapType == KeyCapTypes.Shift)
                    {
                        foreach (Grid grd in this.Children)
                        {
                            foreach (KeyCap kc in grd.Children)
                            {
                                if (kc.KeyCapType == KeyCapTypes.Normal || kc.KeyCapType == KeyCapTypes.Backspace)
                                    UpdateKeyCapLayout(kc, true);
                            }
                        }
                    }
                }
            }
        }

        void keyCap_MouseLeave(object sender, System.Windows.Input.MouseEventArgs e)
        {
            KeyCap keyCap = sender as KeyCap;
            ContentControl cc = keyCap.LayoutRoot.Children[0] as ContentControl;
            //데이터 셋팅
            KeyCapEventArgs keyCapEventArgs = GetKeyCapEventArgs(keyCap);
            //릴리즈 데이터
            keyCapEventArgs.KeyCode = 0;
            //키데이터 셋팅

            if (cc is Button)
            {
                OnKeyCapReleased(keyCapEventArgs);
            }
            else if (cc is ToggleButton && (cc as ToggleButton).IsChecked == false)
            {
                if (keyCap.KeyCapType == KeyCapTypes.Function)
                {
                    OnFunctionKeyCapUnchecked(keyCapEventArgs);
                }
                else
                {
                    OnKeyCapUnchecked(keyCapEventArgs);

                    if (keyCapEventArgs.KeyCapType == KeyCapTypes.Shift)
                    {
                        foreach (Grid grd in this.Children)
                        {
                            foreach (KeyCap kc in grd.Children)
                            {
                                if (kc.KeyCapType == KeyCapTypes.Normal || kc.KeyCapType == KeyCapTypes.Backspace)
                                    UpdateKeyCapLayout(kc, false);
                            }
                        }
                    }
                }
            }
        }
        
        void keyCap_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            KeyCap keyCap = sender as KeyCap;
            ContentControl cc = keyCap.LayoutRoot.Children[0] as ContentControl;
            //데이터 셋팅
            KeyCapEventArgs keyCapEventArgs = GetKeyCapEventArgs(keyCap);
            //키데이터 셋팅

            if (keyCap.KeyCapType == KeyCapTypes.Symbol)
            {
                OnSymbolKeyCapTap(keyCapEventArgs);
            }
            else if (keyCap.KeyCapType == KeyCapTypes.Language)
            {
                OnLanguageKeyCapTap(keyCapEventArgs);
            }
        }

        private KeyCapEventArgs GetKeyCapEventArgs(KeyCap keyCap)
        {
            KeyCapEventArgs keyCapEventArgs = new KeyCapEventArgs()
            {
                ImeKey = keyCap.KeyCapType == KeyCapTypes.Language ? this.LanguageKeyboard.OriginalKeyboardLayoutType : this.OriginalKeyboardLayoutType,
                KeyCapType = keyCap.KeyCapType,
                KeyCode = keyCap.KeyCode
            };

            foreach (Grid grd in this.Children)
            {
                foreach (KeyCap kc in grd.Children)
                {
                    if (kc.LayoutRoot.Children[0] is ToggleButton)
                    {
                        ToggleButton tb = kc.LayoutRoot.Children[0] as ToggleButton;
                        switch (kc.KeyCapType)
                        {
                            case KeyCapTypes.Shift:
                                keyCapEventArgs.IsShiftKey = (bool)tb.IsChecked;
                                break;
                            case KeyCapTypes.Control:
                                keyCapEventArgs.IsControlKey = (bool)tb.IsChecked;
                                break;
                            case KeyCapTypes.Alt:
                                keyCapEventArgs.IsAltKey = (bool)tb.IsChecked;
                                break;
                            case KeyCapTypes.Window:
                                keyCapEventArgs.IsWindowKey = (bool)tb.IsChecked;
                                break;
                            default:
                                break;
                        }
                    }
                }
            }

            return keyCapEventArgs;
        }
    }
}
