using Apps.UI.Keyboard.Key;
using System;
using System.Collections.Generic;
using System.Windows.Media.Imaging;

namespace Apps.UI.Keyboard.Layout.Language
{
    partial class SymbolKeyboardLayout
    {
        public override List<KeyCap[]> GetKeyCapList()
        {
            List<KeyCap[]> keyCapList = new List<KeyCap[]>();

            keyCapList.Add(new KeyCap[]
            {
                new KeyCap() { KeyCapType = KeyCapTypes.Esc, Text = "Esc", KeyCode = 41 },
                new KeyCap() { KeyCapType = KeyCapTypes.Tab, Text = "Tab", KeyCode = 43, WidthRatio = 1.5 },
                new KeyCap() { KeyCapType = KeyCapTypes.LanguageSub, Text = "@CMP", KeyCode = 235, BKImage = new BitmapImage(new Uri("/Images/" + ThemeDirectory + "/Keyboard/greek.sigma.uppercase.png", UriKind.Relative)) },
                new KeyCap() { IsHidden = true },
                new KeyCap() { IsHidden = true },
                new KeyCap() { KeyCapType = KeyCapTypes.LanguageSub, Text = "@CAL", KeyCode = 236, BKImage = new BitmapImage(new Uri("/Images/" + ThemeDirectory + "/Keyboard/calculator.png", UriKind.Relative)) },
                new KeyCap() { KeyCapType = KeyCapTypes.Language, Text = "@IME" },
                new KeyCap() { KeyCapType = KeyCapTypes.Symbol, Text = "&123", WidthRatio = 1.5 },
                new KeyCap() { KeyCapType = KeyCapTypes.Function, Text = "Fn", IsHiddenToolTip = true }
            });
            
            keyCapList.Add(new KeyCap[]
            {
                new KeyCap() { Text = "1|!", SubText = "!|1",  KeyCode = 30 },
                new KeyCap() { Text = "2|@", SubText = "@|2", KeyCode = 31 },
                new KeyCap() { Text = "3|#", SubText = "#|3", KeyCode = 32 },
                new KeyCap() { Text = "4|$", SubText = "$|4", KeyCode = 33 },
                new KeyCap() { Text = "5|%", SubText = "%|5", KeyCode = 34 },
                new KeyCap() { Text = "6|^", SubText = "^|6", KeyCode = 35 },
                new KeyCap() { Text = "7|&", SubText = "&|7", KeyCode = 36 },
                new KeyCap() { Text = "8|*", SubText = "*|8", KeyCode = 37 },
                new KeyCap() { Text = "9|(", SubText = "(|9", KeyCode = 38 },
                new KeyCap() { Text = "0|)", SubText = ")|0", KeyCode = 39 },
            });

            keyCapList.Add(new KeyCap[]
            {
                new KeyCap() { Text = "@MST", KeyCode = 237, BKImage = new BitmapImage(new Uri("/Images/" + ThemeDirectory + "/Keyboard/control.stop.png", UriKind.Relative)) },
                new KeyCap() { Text = "@MPV", KeyCode = 238, BKImage = new BitmapImage(new Uri("/Images/" + ThemeDirectory + "/Keyboard/control.rewind.png", UriKind.Relative)) },
                new KeyCap() { Text = "@MPL", KeyCode = 239, BKImage = new BitmapImage(new Uri("/Images/" + ThemeDirectory + "/Keyboard/control.resume.png", UriKind.Relative)) },
                new KeyCap() { Text = "@MNT", KeyCode = 240, BKImage = new BitmapImage(new Uri("/Images/" + ThemeDirectory + "/Keyboard/control.fastforward.png", UriKind.Relative)) },
                new KeyCap() { Text = "`|~", SubText = "~|`",  KeyCode = 53 },
                new KeyCap() { Text = "-|_", SubText = "_|-",  KeyCode = 45 },
                new KeyCap() { Text = "=|+", SubText = "+|=",  KeyCode = 46 },
                new KeyCap() { Text = "[|{", SubText = "{|[",  KeyCode = 47 },
                new KeyCap() { Text = "]|}", SubText = "}|]",  KeyCode = 48 },
            });

            keyCapList.Add(new KeyCap[]
            {
                new KeyCap() { KeyCapType = KeyCapTypes.Shift, Text = "@SFT", KeyCode = 225, BKImage = new BitmapImage(new Uri("/Images/" + ThemeDirectory + "/Keyboard/arrow.up.png", UriKind.Relative)), IsHiddenToolTip = true , WidthRatio = 1.5 },
                new KeyCap() { Text = "@VLM", KeyCode = 127, BKImage = new BitmapImage(new Uri("/Images/" + ThemeDirectory + "/Keyboard/sound.mute.png", UriKind.Relative)) },
                new KeyCap() { Text = "@VLD", KeyCode = 129, BKImage = new BitmapImage(new Uri("/Images/" + ThemeDirectory + "/Keyboard/sound.1.png", UriKind.Relative)) },
                new KeyCap() { Text = "@VLU", KeyCode = 128, BKImage = new BitmapImage(new Uri("/Images/" + ThemeDirectory + "/Keyboard/sound.3.png", UriKind.Relative)) },
                new KeyCap() { Text = "||\\", SubText = "\\||",  KeyCode = 49 },
                new KeyCap() { Text = ";|:", SubText = ":|;",  KeyCode = 51 },
                new KeyCap() { Text = "'|\"", SubText = "\"|'",  KeyCode = 52 },
                new KeyCap() { Text = "/|?", SubText = "?|/",  KeyCode = 56 },
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
