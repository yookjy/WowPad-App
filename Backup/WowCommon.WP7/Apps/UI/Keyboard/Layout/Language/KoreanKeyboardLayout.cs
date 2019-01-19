using Apps.WowPad.Type;

namespace Apps.UI.Keyboard.Layout.Language
{
    partial class KoreanKeyboardLayout : AbstractDoubleCapKeyboardLayout
    {
        public override string LanguageLabelText
        {
            get { return "한"; }
        }

        public override string SymbolLabelText
        {
            get { return "ㄱㄴㄷ"; }
        }

        public override KeyboardLayoutTypes KeyboardLayoutType
        {
            get { return KeyboardLayoutTypes.Korean; }
        }
    }
}
