﻿using Apps.WowPad.Type;

namespace Apps.UI.Keyboard.Layout.Language
{
    partial class JapanessKeyboardLayout : AbstractKeyboardLayout
    {
        public override string LanguageLabelText
        {
            get { return "日"; }
        }

        public override string SymbolLabelText
        {
            get { return "ABC"; }
        }

        public override KeyboardLayoutTypes KeyboardLayoutType
        {
            get { return KeyboardLayoutTypes.Japaness; }
        }

    }
}
