using Apps.WowPad.Type;
using System;

namespace Apps.WowPad.Model
{
    public class KeyboardInfo
    {
        const Byte KBD_LCONTROL_BIT = 1;
        const Byte KBD_LSHIFT_BIT = 2;
        const Byte KBD_LALT_BIT = 4;
        const Byte KBD_LGUI_BIT = 8;
        //사용하지 않음
        const Byte KBD_RCONTROL_BIT = 16;
        const Byte KBD_RSHIFT_BIT = 32;
        const Byte KBD_RALT_BIT = 64;
        const Byte KBD_RGUI_BIT = 128;

        public Byte KeyCode1 { get; set; }

        public Byte KeyCode2 { get; set; }

        public Byte KeyCode3 { get; set; }

        public Byte KeyCode4 { get; set; }

        public Byte KeyCode5 { get; set; }

        public Byte KeyCode6 { get; set; }

        public Boolean ControlKey { get; set; }

        public Boolean ShiftKey { get; set; }

        public Boolean AltKey { get; set; }

        public Boolean WindowKey { get; set; }

        public KeyboardLayoutTypes ImeKey { get; set; }

        public Byte[] KeyCodes
        {
            get
            {
                Byte[] bytes = new Byte[6];
                bytes[0] = KeyCode1;
                bytes[1] = KeyCode2;
                bytes[2] = KeyCode3;
                bytes[3] = KeyCode4;
                bytes[4] = KeyCode5;
                bytes[5] = KeyCode6;

                return bytes;
            }
        }

        public Byte ShiftFlags
        {
            get
            {
                Byte shiftFlags = 0;
                if (ControlKey)
                    shiftFlags |= KBD_LCONTROL_BIT;
                if (ShiftKey)
                    shiftFlags |= KBD_LSHIFT_BIT;
                if (AltKey)
                    shiftFlags |= KBD_LALT_BIT;
                if (WindowKey)
                    shiftFlags |= KBD_LGUI_BIT;

                return shiftFlags;
            }
        }
    }
}
