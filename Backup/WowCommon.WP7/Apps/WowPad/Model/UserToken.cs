using System;

namespace Apps.WowPad.Model
{
    class UserToken
    {
        public UserToken()
        {
        }

        public UserToken(Object callback)
        {
            this.callback = callback;
        }
        
        public UserToken(PacketInfo packetInfo)
        {
            this.PacketInfo = packetInfo;
        }

        public UserToken(PacketInfo packetInfo, Object callback)
        {
            this.PacketInfo = packetInfo;
            this.callback = callback;
        }

        public PacketInfo PacketInfo { get; set; }
        
        public Object callback { get; set; }
    }
}
