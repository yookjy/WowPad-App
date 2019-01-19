using Apps.UI.Keyboard.Interface;
using Apps.UI.Keyboard.Layout.Language;
using Apps.WowPad.Manager;
using Apps.WowPad.Model;
using Apps.WowPad.Type;
using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;

namespace Apps.UI.Keyboard
{
    public partial class Keyboard : UserControl
    {
        private Dictionary<KeyboardLayoutTypes, int> indexDict = new Dictionary<KeyboardLayoutTypes, int>();

        public IKeyboardLayout currentKeyboardLayout;

        public Keyboard()
        {
            InitializeComponent();
            InitializeKeyboard();
        }

        public void RecreateKeyboard()
        {
            List<KeyboardLayoutTypes> list = new List<KeyboardLayoutTypes>();

            if (indexDict.Count > 2)
            {
                foreach (KeyboardLayoutTypes types in indexDict.Keys)
                {
                    if (types != KeyboardLayoutTypes.Symbol && types != KeyboardLayoutTypes.Function)
                    {
                        list.Add(types);
                    }
                }

                InitializeKeyboard();
                LoadKeyboardLayout(list);
            }
        }

        private void InitializeKeyboard()
        {
            if (indexDict.Count > 0)
            {
                indexDict.Clear();
                this.LayoutRoot.Children.Clear();
            }

            //기본 키보드 등록
            indexDict.Add(KeyboardLayoutTypes.Symbol, 0);
            indexDict.Add(KeyboardLayoutTypes.Function, 1);

            this.LayoutRoot.Children.Add(new SymbolKeyboardLayout());
            this.LayoutRoot.Children.Add(new FunctionKeyboardLayout());

            foreach (UIElement elem in this.LayoutRoot.Children)
            {
                elem.Visibility = System.Windows.Visibility.Collapsed;
                AddEventHandlers(elem as IKeyboardLayout);
            }
        }

        private void AddEventHandlers(IKeyboardLayout keybdLayout)
        {
            keybdLayout.KeyCapPressed += currentKeybdLayout_KeyCapPressed;
            keybdLayout.KeyCapReleased += currentKeybdLayout_KeyCapReleased;

            keybdLayout.KeyCapChecked += keybdLayout_KeyCapChecked;
            keybdLayout.KeyCapUnchecked += keybdLayout_KeyCapUnchecked;

            keybdLayout.LanguageKeyCapTap += currentKeybdLayout_LanguageKeyCapTap;
            keybdLayout.SymbolKeyCapTap += currentKeybdLayout_SymbolKeyCapTap;
            //펑션키 토글 이벤트
            keybdLayout.FunctionKeyCapChecked += currentKeybdLayout_FunctionKeyCapTap;
            keybdLayout.FunctionKeyCapUnchecked += currentKeybdLayout_FunctionKeyCapTap;
            //키보드 초기화 이벤트
            keybdLayout.KeyboardLayoutReseted += keybdLayout_KeyboardLayoutReseted;
        }
        
        public void LoadKeyboardLayout(List<KeyboardLayoutTypes> keyboardLayoutList)
        {
            //현재 키보드 삭제
            currentKeyboardLayout = null;
            //기본 키보드를 제외한 키보드 삭제
            if (this.LayoutRoot.Children.Count > 2)
            {
                for (int last = this.LayoutRoot.Children.Count - 1; last >= 2; last--)
                {
                    IKeyboardLayout kbd = this.LayoutRoot.Children[last] as IKeyboardLayout;
                    this.LayoutRoot.Children.RemoveAt(last);
                    this.indexDict.Remove(kbd.OriginalKeyboardLayoutType);
                }
            }
            
            //새로운 키보드 추가
            IKeyboardLayout keybdLayout = null;
            IKeyboardLayout prevKeybdLayout = null;
            //bool isDefault = false;
            KeyboardLayoutTypes keybdLayoutType = KeyboardLayoutTypes.None;
            foreach (KeyboardLayoutTypes keyboardLayoutType in keyboardLayoutList)
            {
                Type type = Type.GetType("Apps.UI.Keyboard.Layout.Language." + keyboardLayoutType.ToString() + "KeyboardLayout");
                keybdLayoutType = keyboardLayoutType;

                if (type == null)
                {
                    type = Type.GetType("Apps.UI.Keyboard.Layout.Language.EnglishKeyboardLayout");
                    keybdLayoutType = KeyboardLayoutTypes.English;
                }

                keybdLayout = (IKeyboardLayout)Activator.CreateInstance(type);
                if (keybdLayout != null)
                {
                    //현재 키보드 설정
                    if (currentKeyboardLayout == null)
                    {
                        currentKeyboardLayout = keybdLayout;
                        currentKeyboardLayout.SymbolKeyboard = this.LayoutRoot.Children[0] as IKeyboardLayout;
                        currentKeyboardLayout.FunctionKeyboard = this.LayoutRoot.Children[1] as IKeyboardLayout;
                    }
                    else
                    {
                        prevKeybdLayout.LanguageKeyboard = keybdLayout;
                        ((UIElement)keybdLayout).Visibility = System.Windows.Visibility.Collapsed;
                    }

                    //레이아웃에 추가
                    if (!indexDict.ContainsKey(keyboardLayoutType))
                        indexDict.Add(keyboardLayoutType, indexDict.Count);
                    //원본 키보드 레이아웃값 저장
                    keybdLayout.OriginalKeyboardLayoutType = keyboardLayoutType;
                    this.LayoutRoot.Children.Add((UIElement)keybdLayout);
                    //이벤트 추가
                    AddEventHandlers(keybdLayout);
                    //임시 저장
                    prevKeybdLayout = keybdLayout;
                }
                
                //순환구조 생성
                prevKeybdLayout.LanguageKeyboard = currentKeyboardLayout;
            }
            //현재 키보드 준비 (IME, 심볼키캡 설정)
            currentKeyboardLayout.PrepareKeyboardLayout();
        }

        void keybdLayout_KeyboardLayoutReseted(object sender, Key.KeyCapEventArgs e)
        {
            KeyboardControlManager.Instance.KeyRelease(e.ToKeyboardInfo());
        }
        
        void currentKeybdLayout_LanguageKeyCapTap(object sender, Key.KeyCapEventArgs e)
        {
            if (currentKeyboardLayout.LanguageKeyboard == null ||
                currentKeyboardLayout.LanguageKeyboard.KeyboardLayoutType == currentKeyboardLayout.KeyboardLayoutType)
            {
                //언어가 없거나 하나일때는 버튼처리 무시
                return;
            }
            int index = indexDict[currentKeyboardLayout.LanguageKeyboard.OriginalKeyboardLayoutType];
            //키보드 스왑
            IKeyboardLayout tmpLayout = currentKeyboardLayout;
            currentKeyboardLayout = this.LayoutRoot.Children[index] as IKeyboardLayout;
            currentKeyboardLayout.SymbolKeyboard = this.LayoutRoot.Children[0] as IKeyboardLayout;
            currentKeyboardLayout.FunctionKeyboard = this.LayoutRoot.Children[1] as IKeyboardLayout;

            ((AbstractKeyboardLayout)tmpLayout).Visibility = System.Windows.Visibility.Collapsed;
            ((AbstractKeyboardLayout)currentKeyboardLayout).Visibility = System.Windows.Visibility.Visible;

            if (((AbstractKeyboardLayout)currentKeyboardLayout).ActualWidth == 0)
            {
                this.UpdateLayout();
            }
        }

        void currentKeybdLayout_SymbolKeyCapTap(object sender, Key.KeyCapEventArgs e)
        {
            int index = indexDict[currentKeyboardLayout.SymbolKeyboard.KeyboardLayoutType];
            //키보드 스왑
            IKeyboardLayout tmpLayout = currentKeyboardLayout;
            currentKeyboardLayout = currentKeyboardLayout.SymbolKeyboard;
            currentKeyboardLayout.SymbolKeyboard = tmpLayout; ;
            //바뀌기 이전 키보드의 언어 및 펑션키 복사
            if (currentKeyboardLayout.KeyboardLayoutType == KeyboardLayoutTypes.Symbol)
            {
                currentKeyboardLayout.LanguageKeyboard = tmpLayout.LanguageKeyboard;
                currentKeyboardLayout.FunctionKeyboard = tmpLayout.FunctionKeyboard;
            }

            ((AbstractKeyboardLayout)tmpLayout).Visibility = System.Windows.Visibility.Collapsed;
            ((AbstractKeyboardLayout)currentKeyboardLayout).Visibility = System.Windows.Visibility.Visible;

            if (((AbstractKeyboardLayout)currentKeyboardLayout).ActualWidth == 0)
            {
                this.UpdateLayout();
            }
        }

        void currentKeybdLayout_FunctionKeyCapTap(object sender, Key.KeyCapEventArgs e)
        {
            int index = indexDict[currentKeyboardLayout.FunctionKeyboard.KeyboardLayoutType];
            //키보드 스왑
            IKeyboardLayout tmpLayout = currentKeyboardLayout;
            currentKeyboardLayout = currentKeyboardLayout.FunctionKeyboard;
            currentKeyboardLayout.FunctionKeyboard = tmpLayout; ;
            //바뀌기 이전 키보드의 언어 및 심볼키 복사
            if (currentKeyboardLayout.KeyboardLayoutType == KeyboardLayoutTypes.Function)
            {
                currentKeyboardLayout.LanguageKeyboard = tmpLayout.LanguageKeyboard;
                currentKeyboardLayout.SymbolKeyboard = tmpLayout.SymbolKeyboard;
            }
            ((AbstractKeyboardLayout)tmpLayout).Visibility = System.Windows.Visibility.Collapsed;
            ((AbstractKeyboardLayout)currentKeyboardLayout).Visibility = System.Windows.Visibility.Visible;

            if (((AbstractKeyboardLayout)currentKeyboardLayout).ActualWidth == 0)
            {
                this.UpdateLayout();
            }
        }
        
        void currentKeybdLayout_KeyCapPressed(object sender, Key.KeyCapEventArgs e)
        {
            KeyboardControlManager.Instance.KerPress(e.ToKeyboardInfo());
        }

        void currentKeybdLayout_KeyCapReleased(object sender, Key.KeyCapEventArgs e)
        {
            KeyboardControlManager.Instance.KeyRelease(e.ToKeyboardInfo());
        }
        
        void keybdLayout_KeyCapUnchecked(object sender, Key.KeyCapEventArgs e)
        {
            KeyboardControlManager.Instance.KerPress(e.ToKeyboardInfo());
        }

        void keybdLayout_KeyCapChecked(object sender, Key.KeyCapEventArgs e)
        {
            KeyboardControlManager.Instance.KeyRelease(e.ToKeyboardInfo());
        }
    }
}
