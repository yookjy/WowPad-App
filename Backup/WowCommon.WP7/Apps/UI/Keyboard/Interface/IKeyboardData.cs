using Apps.UI.Keyboard.Key;
using System.Collections.Generic;

namespace Apps.UI.Keyboard.Interface
{
    interface IKeyboardData
    {
        List<KeyCap[]> GetKeyCapList();
    }
}
