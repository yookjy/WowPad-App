using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Media;

namespace Apps.WowPad.Model
{
    public enum VersionContentType
    {
        NEW,
        FIX,
        MOD,
        IMP,
        DEL
    }

    public class VersionContent
    {
        public VersionContent(VersionContentType type, string content)
        {
            Color color = Colors.Transparent;
            switch (type)
            {
                case VersionContentType.NEW:
                    color = ConvertColor(0xFF1BA1E2);//Cyan
                    break;
                case VersionContentType.FIX:
                    color = ConvertColor(0xFF60A917);//Green
                    break;
                case VersionContentType.MOD:
                    color = ConvertColor(0xFF00ABA9); // Teal
                    break;
                case VersionContentType.IMP:
                    color = ConvertColor(0xFFF0A30A); //Amber
                    break;
                case VersionContentType.DEL:
                    color = ConvertColor(0xFFE51400); //Red
                    break;
            }

            Type = type;
            Content = content.Replace("\\n", "\n");
            Background = new SolidColorBrush(color);
            
        }

        public Color ConvertColor(uint uintCol)
        {
            byte A = (byte)((uintCol & 0xFF000000) >> 24);
            byte R = (byte)((uintCol & 0x00FF0000) >> 16);
            byte G = (byte)((uintCol & 0x0000FF00) >> 8);
            byte B = (byte)((uintCol & 0x000000FF) >> 0);
            return Color.FromArgb(A, R, G, B); ;
        }

        public SolidColorBrush Background { get; set; }

        public VersionContentType Type { get; set; }

        public string Content { get; set; }
    }
}
