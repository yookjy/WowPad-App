using Apps.UI.Keyboard.Key;
using Apps.WowPad.Type;

namespace Apps.UI.Keyboard.Interface
{
    public delegate void KeyCapEventHandler(object sender, KeyCapEventArgs e);

    public interface IKeyboardLayout
    {
        IKeyboardLayout SymbolKeyboard { get; set; }

        IKeyboardLayout FunctionKeyboard { get; set; }

        IKeyboardLayout LanguageKeyboard { get; set; }
        
        string LanguageLabelText { get; }

        string SymbolLabelText { get; }

        KeyboardLayoutTypes KeyboardLayoutType { get; }

        KeyboardLayoutTypes OriginalKeyboardLayoutType { get; set; }

        void UpdateKeyboardLayout();

        void UpdateKeyCapLayout(KeyCap keyCap, bool isShift);

        void ResetKeyboardLayout();

        void PrepareKeyboardLayout();

        event KeyCapEventHandler KeyCapPressed;

        event KeyCapEventHandler KeyCapReleased;

        event KeyCapEventHandler KeyCapChecked;

        event KeyCapEventHandler KeyCapUnchecked;

        event KeyCapEventHandler SymbolKeyCapTap;

        event KeyCapEventHandler LanguageKeyCapTap;

        event KeyCapEventHandler FunctionKeyCapChecked;

        event KeyCapEventHandler FunctionKeyCapUnchecked;

        event KeyCapEventHandler KeyboardLayoutReseted;
    }
}
