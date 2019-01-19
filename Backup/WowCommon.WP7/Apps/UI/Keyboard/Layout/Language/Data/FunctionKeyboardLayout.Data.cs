using Apps.UI.Keyboard.Key;
using System;
using System.Collections.Generic;
using System.Windows.Media.Imaging;

namespace Apps.UI.Keyboard.Layout.Language
{
    partial class FunctionKeyboardLayout
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
                new KeyCap() { KeyCapType = KeyCapTypes.Language, Text = "@IME", IsEnabled = false },
                new KeyCap() { KeyCapType = KeyCapTypes.Symbol, Text = "&123", WidthRatio = 1.5, IsEnabled = false },
                new KeyCap() { KeyCapType = KeyCapTypes.Function, Text = "Fn", IsHiddenToolTip = true }
            });

            keyCapList.Add(new KeyCap[]
            {
                new KeyCap() { Text = "F1", KeyCode = 58 },
                new KeyCap() { Text = "F2", KeyCode = 59 },
                new KeyCap() { Text = "F3", KeyCode = 60 },
                new KeyCap() { Text = "F4", KeyCode = 61 },
                new KeyCap() { Text = "F5", KeyCode = 62 },
                new KeyCap() { Text = "F6", KeyCode = 63 },
                new KeyCap() { Text = "F7", KeyCode = 64 },
                new KeyCap() { Text = "F8", KeyCode = 65 },
                new KeyCap() { Text = "F9", KeyCode = 66 },
                new KeyCap() { Text = "F10", KeyCode = 67 }
            });

            keyCapList.Add(new KeyCap[]
            {
                new KeyCap() { Text = "Menu", KeyCode = 101 },
                new KeyCap() { Text = "Insert", KeyCode = 73 },
                new KeyCap() { Text = "Home", KeyCode = 74 },
                new KeyCap() { Text = "Page\nUp", KeyCode = 75 },
                new KeyCap() { Text = "Print\nScreen", SubText = "SysRq", KeyCode = 70 },
                new KeyCap() { Text = "Pause", SubText = "Break", KeyCode = 72 },
                new KeyCap() { Text = "↑", KeyCode = 82 },
                new KeyCap() { Text = "F11", KeyCode = 68 },
                new KeyCap() { Text = "F12", KeyCode = 69 }
            });

            keyCapList.Add(new KeyCap[]
            {
                new KeyCap() { KeyCapType = KeyCapTypes.Shift, Text = "@SFT", KeyCode = 225, BKImage = new BitmapImage(new Uri("/Images/" + ThemeDirectory + "/Keyboard/arrow.up.png", UriKind.Relative)), IsHiddenToolTip = true , WidthRatio = 1.5 },
                new KeyCap() { Text = "Delete", SubText = "Back\nspace", KeyCode = 76 },
                new KeyCap() { Text = "End", KeyCode = 77 },
                new KeyCap() { Text = "Page\nDown", KeyCode = 78 },
                new KeyCap() { Text = "Scroll\nLock", KeyCode = 71 },
                new KeyCap() { Text = "←", KeyCode = 80 },
                new KeyCap() { Text = "↓", KeyCode = 81 },
                new KeyCap() { Text = "→", KeyCode = 79 },
                new KeyCap() { KeyCapType = KeyCapTypes.Backspace, Text = "@BS", KeyCode = 42, BKImage = new BitmapImage(new Uri("/Images/" + ThemeDirectory + "/Keyboard/clear.reflect.horizontal.png", UriKind.Relative)), IsHiddenToolTip = true, WidthRatio = 1.5 }
            });

            keyCapList.Add(new KeyCap[]
            {
                new KeyCap() { KeyCapType = KeyCapTypes.Control, Text = "Ctrl", KeyCode = 224, IsHiddenToolTip = true, WidthRatio = 1.5  },
                new KeyCap() { KeyCapType = KeyCapTypes.Window, Text = "@WND", KeyCode = 227, BKImage = new BitmapImage(new Uri("/Images/" + ThemeDirectory + "/Keyboard/os.windows.8.png", UriKind.Relative)), IsHiddenToolTip = true },
                new KeyCap() { KeyCapType = KeyCapTypes.Alt, Text = "Alt", KeyCode = 226, IsHiddenToolTip = true },
                new KeyCap() { KeyCapType = KeyCapTypes.Space, Text = "space", KeyCode = 44, IsHiddenToolTip = true, WidthRatio = 3 },
                new KeyCap() { Text = ",", SubText = "<", KeyCode = 54 },
                new KeyCap() { Text = ".", SubText = ">", KeyCode = 55 },
                new KeyCap() { KeyCapType = KeyCapTypes.Enter, Text = "@ENT", KeyCode = 40, BKImage = new BitmapImage(new Uri("/Images/" + ThemeDirectory + "/Keyboard/arrow.corner.up.left.png", UriKind.Relative)), IsHiddenToolTip = true, WidthRatio = 1.5 }
            });
            return keyCapList;
        }
        
    }
}
