using Apps.UI.Keyboard.Key;
using System;
using System.Collections.Generic;
using System.Windows.Media.Imaging;

namespace Apps.UI.Keyboard.Layout.Language
{
    partial class ChinessKeyboardLayout
    {
        public override List<KeyCap[]> GetKeyCapList()
        {
            List<KeyCap[]> keyCapList = new List<KeyCap[]>();

            keyCapList.Add(new KeyCap[]
            {
                new KeyCap() { KeyCapType = KeyCapTypes.Esc, Text = "Esc", KeyCode = 41 },
                new KeyCap() { KeyCapType = KeyCapTypes.Tab, Text = "Tab", KeyCode = 43, WidthRatio = 1.5 },
                new KeyCap() { IsHidden = true },
                new KeyCap() { IsHidden = true },
                new KeyCap() { IsHidden = true },
                new KeyCap() { IsHidden = true },
                new KeyCap() { KeyCapType = KeyCapTypes.Language, Text = "@IME" },
                new KeyCap() { KeyCapType = KeyCapTypes.Symbol, Text = "&123", WidthRatio = 1.5  },
                new KeyCap() { KeyCapType = KeyCapTypes.Function, Text = "Fn", IsHiddenToolTip = true }
            });

            keyCapList.Add(new KeyCap[]
            {
                new KeyCap() { Text = "q", SubText = "Q", KeyCode = 20 },
                new KeyCap() { Text = "w", SubText = "W", KeyCode = 26 },
                new KeyCap() { Text = "e", SubText = "E", KeyCode = 8 },
                new KeyCap() { Text = "r", SubText = "R", KeyCode = 21 },
                new KeyCap() { Text = "t", SubText = "T", KeyCode = 23 },
                new KeyCap() { Text = "y", SubText = "Y", KeyCode = 28 },
                new KeyCap() { Text = "u", SubText = "U", KeyCode = 24 },
                new KeyCap() { Text = "i", SubText = "I", KeyCode = 12 },
                new KeyCap() { Text = "o", SubText = "O", KeyCode = 18 },
                new KeyCap() { Text = "p", SubText = "P", KeyCode = 19 },
            });

            keyCapList.Add(new KeyCap[]
            {
                new KeyCap() { Text = "a", SubText = "A", KeyCode = 4 },
                new KeyCap() { Text = "s", SubText = "S", KeyCode = 22 },
                new KeyCap() { Text = "d", SubText = "D", KeyCode = 7 },
                new KeyCap() { Text = "f", SubText = "F", KeyCode = 9 },
                new KeyCap() { Text = "g", SubText = "G", KeyCode = 10 },
                new KeyCap() { Text = "h", SubText = "H", KeyCode = 11 },
                new KeyCap() { Text = "j", SubText = "J", KeyCode = 13 },
                new KeyCap() { Text = "k", SubText = "K", KeyCode = 14 },
                new KeyCap() { Text = "l", SubText = "L", KeyCode = 15 }
            });

            keyCapList.Add(new KeyCap[]
            {
                new KeyCap() { KeyCapType = KeyCapTypes.Shift, Text = "@SFT", KeyCode = 225, BKImage = new BitmapImage(new Uri("/Images/" + ThemeDirectory + "/Keyboard/arrow.up.png", UriKind.Relative)), IsHiddenToolTip = true , WidthRatio = 1.5 },
                new KeyCap() { Text = "z", SubText = "Z", KeyCode = 29 },
                new KeyCap() { Text = "x", SubText = "X", KeyCode = 27 },
                new KeyCap() { Text = "c", SubText = "C", KeyCode = 6 },
                new KeyCap() { Text = "v", SubText = "V", KeyCode = 25 },
                new KeyCap() { Text = "b", SubText = "B", KeyCode = 5 },
                new KeyCap() { Text = "n", SubText = "N", KeyCode = 17 },
                new KeyCap() { Text = "m", SubText = "M", KeyCode = 16 },
                new KeyCap() { KeyCapType = KeyCapTypes.Backspace, Text = "@BS", KeyCode = 42, BKImage = new BitmapImage(new Uri("/Images/" + ThemeDirectory + "/Keyboard/clear.reflect.horizontal.png", UriKind.Relative)), IsHiddenToolTip = true, WidthRatio = 1.5 }
            });

            keyCapList.Add(new KeyCap[]
            {
                new KeyCap() { KeyCapType = KeyCapTypes.Control, Text = "Ctrl", KeyCode = 224, IsHiddenToolTip = true, WidthRatio = 1.5  },
                new KeyCap() { KeyCapType = KeyCapTypes.Window, Text = "@WND", KeyCode = 227, BKImage = new BitmapImage(new Uri("/Images/" + ThemeDirectory + "/Keyboard/os.windows.8.png", UriKind.Relative)), IsHiddenToolTip = true },
                new KeyCap() { KeyCapType = KeyCapTypes.Alt, Text = "Alt", KeyCode = 226, IsHiddenToolTip = true },
                new KeyCap() { KeyCapType = KeyCapTypes.Space, Text = "空格", KeyCode = 44, IsHiddenToolTip = true, WidthRatio = 3 },
                new KeyCap() { Text = ",", SubText = "<", KeyCode = 54 },
                new KeyCap() { Text = ".", SubText = ">", KeyCode = 55 },
                new KeyCap() { KeyCapType = KeyCapTypes.Enter, Text = "換行", KeyCode = 40, IsHiddenToolTip = true, WidthRatio = 1.5 }
            });

            return keyCapList;
        }
    }
}
