using Apps.UI.Keyboard.Key;
using System;
using System.Collections.Generic;
using System.Windows.Media.Imaging;

namespace Apps.UI.Keyboard.Layout.Language
{
    partial class KoreanKeyboardLayout
    {
        public override List<KeyCap[]> GetKeyCapList()
        {
            List<KeyCap[]> keyCapList = new List<KeyCap[]>();

            keyCapList.Add(new KeyCap[]
            {
                new KeyCap() { KeyCapType = KeyCapTypes.Esc, Text = "Esc", KeyCode = 41 },
                new KeyCap() { KeyCapType = KeyCapTypes.Tab, Text = "Tab", KeyCode = 43, WidthRatio = 1.5 },
                new KeyCap() { KeyCapType = KeyCapTypes.LanguageSub, Text = "한자", KeyCode = 145 },
                new KeyCap() { IsHidden = true },
                new KeyCap() { IsHidden = true },
                new KeyCap() { KeyCapType = KeyCapTypes.LanguageSub, Text = "한영", KeyCode = 144 },
                new KeyCap() { KeyCapType = KeyCapTypes.Language, Text = "@IME" },
                new KeyCap() { KeyCapType = KeyCapTypes.Symbol, Text = "&123", WidthRatio = 1.5  },
                new KeyCap() { KeyCapType = KeyCapTypes.Function, Text = "Fn", IsHiddenToolTip = true }
            });

            keyCapList.Add(new KeyCap[]
            {
                new KeyCap() { Text = "ㅂ|q", SubText = "ㅃ|Q", KeyCode = 20 },
                new KeyCap() { Text = "ㅈ|w", SubText = "ㅉ|W", KeyCode = 26 },
                new KeyCap() { Text = "ㄷ|e", SubText = "ㄸ|E", KeyCode = 8 },
                new KeyCap() { Text = "ㄱ|r", SubText = "ㄲ|R", KeyCode = 21 },
                new KeyCap() { Text = "ㅅ|t", SubText = "ㅆ|T", KeyCode = 23 },
                new KeyCap() { Text = "ㅛ|y", SubText = "ㅛ|Y", KeyCode = 28 },
                new KeyCap() { Text = "ㅕ|u", SubText = "ㅕ|U", KeyCode = 24 },
                new KeyCap() { Text = "ㅑ|i", SubText = "ㅑ|I", KeyCode = 12 },
                new KeyCap() { Text = "ㅐ|o", SubText = "ㅒ|O", KeyCode = 18 },
                new KeyCap() { Text = "ㅔ|p", SubText = "ㅖ|P", KeyCode = 19 },
            });

            keyCapList.Add(new KeyCap[]
            {
                new KeyCap() { Text = "ㅁ|a", SubText = "ㅁ|A", KeyCode = 4 },
                new KeyCap() { Text = "ㄴ|s", SubText = "ㄴ|S", KeyCode = 22 },
                new KeyCap() { Text = "ㅇ|d", SubText = "ㅇ|D", KeyCode = 7 },
                new KeyCap() { Text = "ㄹ|f", SubText = "ㄹ|F", KeyCode = 9 },
                new KeyCap() { Text = "ㅎ|g", SubText = "ㅎ|G", KeyCode = 10 },
                new KeyCap() { Text = "ㅗ|h", SubText = "ㅗ|H", KeyCode = 11 },
                new KeyCap() { Text = "ㅓ|j", SubText = "ㅓ|J", KeyCode = 13 },
                new KeyCap() { Text = "ㅏ|k", SubText = "ㅏ|K", KeyCode = 14 },
                new KeyCap() { Text = "ㅣ|l", SubText = "ㅣ|L", KeyCode = 15 }
            });

            keyCapList.Add(new KeyCap[]
            {
                new KeyCap() { KeyCapType = KeyCapTypes.Shift, Text = "@SFT", KeyCode = 225, BKImage = new BitmapImage(new Uri("/Images/" + ThemeDirectory + "/Keyboard/arrow.up.png", UriKind.Relative)), IsHiddenToolTip = true , WidthRatio = 1.5 },
                new KeyCap() { Text = "ㅋ|z", SubText = "ㅋ|Z", KeyCode = 29 },
                new KeyCap() { Text = "ㅌ|x", SubText = "ㅌ|X", KeyCode = 27 },
                new KeyCap() { Text = "ㅊ|c", SubText = "ㅊ|C", KeyCode = 6 },
                new KeyCap() { Text = "ㅍ|v", SubText = "ㅍ|V", KeyCode = 25 },
                new KeyCap() { Text = "ㅠ|b", SubText = "ㅠ|B", KeyCode = 5 },
                new KeyCap() { Text = "ㅜ|n", SubText = "ㅜ|N", KeyCode = 17 },
                new KeyCap() { Text = "ㅡ|m", SubText = "ㅡ|M", KeyCode = 16 },
                new KeyCap() { KeyCapType = KeyCapTypes.Backspace, Text = "@BS", KeyCode = 42, BKImage = new BitmapImage(new Uri("/Images/" + ThemeDirectory + "/Keyboard/clear.reflect.horizontal.png", UriKind.Relative)), IsHiddenToolTip = true, WidthRatio = 1.5 }
            });

            keyCapList.Add(new KeyCap[]
            {
                new KeyCap() { KeyCapType = KeyCapTypes.Control, Text = "Ctrl", KeyCode = 224, IsHiddenToolTip = true, WidthRatio = 1.5  },
                new KeyCap() { KeyCapType = KeyCapTypes.Window, Text = "@WND", KeyCode = 227, BKImage = new BitmapImage(new Uri("/Images/" + ThemeDirectory + "/Keyboard/os.windows.8.png", UriKind.Relative)), IsHiddenToolTip = true },
                new KeyCap() { KeyCapType = KeyCapTypes.Alt, Text = "Alt", KeyCode = 226, IsHiddenToolTip = true },
                new KeyCap() { KeyCapType = KeyCapTypes.Space, Text = "공백", KeyCode = 44, IsHiddenToolTip = true, WidthRatio = 3 },
                new KeyCap() { Text = ",", SubText = "<", KeyCode = 54 },
                new KeyCap() { Text = ".", SubText = ">", KeyCode = 55 },
                new KeyCap() { KeyCapType = KeyCapTypes.Enter, Text = "엔터", KeyCode = 40, IsHiddenToolTip = true, WidthRatio = 1.5 }
            });

            return keyCapList;
        }

        
        
    }
}
