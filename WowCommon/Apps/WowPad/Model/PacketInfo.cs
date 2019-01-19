using Apps.WowPad.Type;
using Apps.WowPad.Util;

namespace Apps.WowPad.Model
{
    public class PacketInfo
    {
        private byte[] packet;
        
        public PacketTypes PacketType { get; set; }
        
        public DeviceTypes DeviceType { get; set; }

        public ButtonTypes ButtonType { get; set; }

        public ImageQualityTypes ImageQualityType { get; set; }
        
        public TouchInfo[] TouchInfos { get; set; }

        public KeyboardInfo KeyboardInfo { get; set; }
        
        public int AccessCode { get; set; }

        public int Seq { get; set; }

        public int Flag { get; set; }

        public byte[] CachedPacket
        {
            get
            {
                if (this.packet == null)
                {
                    this.packet = PacketUtils.MakeClientPacket(this);
                }
                return this.packet;
            }
        }

        public void Clear()
        {
            this.packet = null;
            this.PacketType = PacketTypes.FindServer;
            this.DeviceType = DeviceTypes.None;
            this.ButtonType = ButtonTypes.None;
            this.ImageQualityType = ImageQualityTypes.Low;
            this.KeyboardInfo = null;
            this.AccessCode = 0;
            this.Seq = 0;
            this.Flag = 0;
        }
    }
}