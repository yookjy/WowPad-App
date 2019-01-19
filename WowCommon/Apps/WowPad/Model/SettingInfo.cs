using Apps.WowPad.Type;
using System;

namespace Apps.WowPad.Model
{
    public class SettingInfo
    {
        public Boolean GettingStart { get; set; }
        
        public DeviceTypes DeviceType { get; set; }

        public ImageQualityTypes ImageQualityType { get; set; }

        public Boolean UseExtendButton { get; set; }

        public Boolean FullSizeKeyboard { get; set; }

        public int DefaultPort { get; set; }

        public Boolean AutoConnect { get; set; }

        public string ServerName { get; set; }

        public string ServerIP { get; set; }

        public int TcpPort { get; set; }

        public int UdpPort { get; set; }

        public int AccessCode { get; set; }

        public int SearchDefaultPageIndex { get; set; }

        public int CellularDataUsage { get; set; }

        public ServerInfo ServerInfo
        {
            get
            {
                ServerInfo serverInfo = new ServerInfo();
                serverInfo.ServerName = this.ServerName;
                serverInfo.ServerIP = this.ServerIP;
                serverInfo.TcpPort = this.TcpPort;
                serverInfo.UdpPort = this.UdpPort;
                serverInfo.AccessCode = this.AccessCode;

                return serverInfo;
            }
        }
    }
}
