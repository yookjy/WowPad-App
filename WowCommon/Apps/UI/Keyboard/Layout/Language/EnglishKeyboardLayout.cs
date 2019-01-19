using Apps.WowPad.Type;

namespace Apps.UI.Keyboard.Layout.Language
{
    partial class EnglishKeyboardLayout : AbstractKeyboardLayout
    {
        public override string LanguageLabelText
        {
            get { return "ENG"; }
        }

        public override string SymbolLabelText
        {
            get { return "ABC"; }
        }

        public override KeyboardLayoutTypes KeyboardLayoutType
        {
            get { return KeyboardLayoutTypes.English; }
        }
    }
}
