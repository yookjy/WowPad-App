using System;
using System.Collections.Generic;
using System.Net;
using System.Runtime.Serialization;

namespace Apps.WowPad.Model
{
    [DataContract]
    public class ServerInfo
    {
        [DataMember]
        public String ServerName { get; set; }

        [DataMember]
        public int TcpPort { get; set; }

        [DataMember]
        public int UdpPort { get; set; }

        [DataMember]
        public String ServerIP { get; set; }

        [DataMember]
        public String SubnetMask { get; set; }

        [DataMember]
        public int AccessCode { get; set; }

        [DataMember]
        public int SearchDefaultPageIndex { get; set; }

        public Boolean IsConnected { get; set; }

        [DataMember]
        public List<Byte[]> MacAddressList { get; set; }

        [DataMember]
        public DateTime LastDateTime { get; set; }

        private DnsEndPoint tcpIPEndPoint;

        private DnsEndPoint udpIPEndPoint;

        public DnsEndPoint TcpIPEndPoint
        {
            get
            {
                if ((this.tcpIPEndPoint == null || !this.IsConnected)
                    && !String.IsNullOrEmpty(ServerIP) 
                    && !String.IsNullOrWhiteSpace(ServerIP) 
                    && TcpPort > 0)
                {
                    this.tcpIPEndPoint = new DnsEndPoint(ServerIP, TcpPort);
                }
                
                return this.tcpIPEndPoint;
            }
            set
            {
                this.tcpIPEndPoint = value;
            }
        }

        public DnsEndPoint UdpIPEndPoint
        {
            get
            {
                if ((this.udpIPEndPoint == null || !this.IsConnected)
                    && !String.IsNullOrEmpty(ServerIP) 
                    && !String.IsNullOrWhiteSpace(ServerIP) 
                    && UdpPort > 0)
                {
                    this.udpIPEndPoint = new DnsEndPoint(ServerIP, UdpPort);
                }

                return this.udpIPEndPoint;
            }
            set
            {
                this.udpIPEndPoint = value;
            }
        }
    }
}