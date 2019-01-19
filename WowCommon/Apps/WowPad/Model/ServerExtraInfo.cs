using Apps.WowPad.Type;
using System;
using System.Collections.Generic;

namespace Apps.WowPad.Model
{
    public class ServerExtraInfo
    {
        public ScreenTypes ScreenType { get; set; }

        public List<KeyboardLayoutTypes> KeyboardList { get; set; }

        public List<Byte[]> MacAddressList { get; set; }
    }
}
