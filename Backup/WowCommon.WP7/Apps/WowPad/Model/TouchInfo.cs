using Apps.WowPad.Type;
using System.Windows.Input;

namespace Apps.WowPad.Model
{
    public class TouchInfo
    {
        public int Id { get; set; }

        public int X { get; set; }

        public int Y { get; set; }

        public TouchActionTypes Action { get; set; }

        public TouchInfo Clone()
        {
            return (TouchInfo)this.MemberwiseClone();
        }
    }
}