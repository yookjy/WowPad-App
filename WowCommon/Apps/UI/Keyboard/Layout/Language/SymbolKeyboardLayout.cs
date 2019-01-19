using Apps.WowPad.Type;

namespace Apps.UI.Keyboard.Layout.Language
{
    partial class SymbolKeyboardLayout : AbstractDoubleCapKeyboardLayout
    {
        public override string LanguageLabelText
        {
            get { return string.Empty; }
        }

        public override string SymbolLabelText
        {
            get { return "&123"; }
        }

        public override KeyboardLayoutTypes KeyboardLayoutType
        {
            get { return KeyboardLayoutTypes.Symbol; }
        }
    }
}
