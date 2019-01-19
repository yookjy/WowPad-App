using Apps.WowPad.Type;

namespace Apps.UI.Keyboard.Layout.Language
{
    partial class FunctionKeyboardLayout : AbstractKeyboardLayout
    {
        public override string LanguageLabelText
        {
            get { return this.FunctionKeyboard.LanguageLabelText; }
        }

        public override string SymbolLabelText
        {
            get { return this.SymbolKeyboard.SymbolLabelText; }
        }

        public override KeyboardLayoutTypes KeyboardLayoutType
        {
            get { return KeyboardLayoutTypes.Function; }
        }
    }
}
