using Apps.WowPad.Model;
using Apps.WowPad.Type;
using System;

namespace Apps.UI.Keyboard.Key
{
    public class KeyCapEventArgs : EventArgs
    {
        public Boolean IsControlKey { get; set; }

        public Boolean IsAltKey { get; set; }

        public Boolean IsWindowKey { get; set; }

        public Boolean IsShiftKey { get; set; }

        public KeyboardLayoutTypes ImeKey { get; set; }

        public KeyCapTypes KeyCapType { get; set; }

        public byte KeyCode { get; set; }

        public KeyboardInfo ToKeyboardInfo()
        {
            KeyboardInfo ki = new KeyboardInfo();
            ki.AltKey = IsAltKey;
            ki.ControlKey = IsControlKey;
            ki.WindowKey = IsWindowKey;
            ki.ShiftKey = IsShiftKey;
            ki.ImeKey = ImeKey;
            ki.KeyCode1 = KeyCode;
            return ki;
        }
    }
}
