using Apps.WowPad.Sockets;
using Apps.WowPad.Type;
using Microsoft.Phone.Net.NetworkInformation;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Windows;
#if !WP7
using Windows.Networking;
using Windows.Networking.Connectivity;
#else
using Microsoft.Devices;
#endif

namespace Apps.WowPad.Util
{
    public class NetworkUtils
    {
        private static ManualResetEvent clientDone = new ManualResetEvent(false);

        public static string MyIp
        {
            get
            {
                String ipAddressString = string.Empty;
                #if WP7
                #else 
                IReadOnlyList<HostName> hostNames = NetworkInformation.GetHostNames();

                foreach (HostName hostName in hostNames)
                {
                    IPInformation ipif = hostName.IPInformation;
                    String hn = hostName.ToString();

                    //IANA Tpe : WiFi => 71, 셀룰러 => 244
                    if (hostName.Type == HostNameType.Ipv4)
                    {
                        ipAddressString = hostName.RawName;
                        break;
                    }
                }
                #endif                 
                return ipAddressString;
            }
        }

        public static string MulticastIP
        {
            get
            {
                String ipAddressString = string.Empty;
                #if WP7
                ipAddressString = IPAddress.Broadcast.ToString();
                #else 
                IReadOnlyList<HostName> hostNames = NetworkInformation.GetHostNames();

                foreach (HostName hostName in hostNames)
                {
                    IPInformation ipif = hostName.IPInformation;
                    String hn = hostName.ToString();

                    //IANA Tpe : WiFi => 71, 셀룰러 => 244
                    if (hostName.Type == HostNameType.Ipv4 && ipif.NetworkAdapter.IanaInterfaceType == 71)
                    {
                        String ip = hostName.RawName;
                        ipAddressString = ip.Substring(0, ip.LastIndexOf(".") + 1) + "255";
                        break;
                    }
                }
                #endif
                return ipAddressString;
            }
        }

        public static string GetSubnetDirectedBroadcastIP(string ipStr, string subnetStr)
        {
            if (string.IsNullOrEmpty(ipStr) || string.IsNullOrEmpty(subnetStr)) return string.Empty;

            string[] ips = ipStr.Split('.');
            string[] sbs = subnetStr.Split('.');
            string sdb = string.Empty;

            if (ips.Length != sbs.Length) return string.Empty;

            for (int i = 0; i < ips.Length; i++)
            {
                int ipNum = Convert.ToInt16(ips[i]);
                int sbNum = Convert.ToInt16(sbs[i]);

                //IP 유효성 검사
                if (ipNum < 0 || ipNum > 255 || sbNum < 0 || sbNum > 255) return string.Empty;

                //서브넷을 xor 한후 ip와 or 연산함
                sdb += Convert.ToString((ipNum | (sbNum ^ 255)));
                if (i < ips.Length - 1)
                {
                    sdb += ".";
                }
            }
            return sdb;
        }


        public static bool IsWiFiNetwork
        {
            get
            {
                return NetworkInterface.NetworkInterfaceType == NetworkInterfaceType.Ethernet || NetworkInterface.NetworkInterfaceType == NetworkInterfaceType.Wireless80211;
                /*
#if WP7
                if (DeviceNetworkInformation.IsNetworkAvailable && DeviceNetworkInformation.IsWiFiEnabled)
                {
                    if (DeviceNetworkInformation.IsCellularDataEnabled)
                    {
                        clientDone.Reset();
                        bool ret = false;
                        DeviceNetworkInformation.ResolveHostNameAsync(
                            new DnsEndPoint("www.microsoft.com", 80),
                            new NameResolutionCallback(nrr =>
                            {
                                var info = nrr.NetworkInterface;
                                var type = info.InterfaceType;
                                var subType = info.InterfaceSubtype;
                                ret = (type == NetworkInterfaceType.Wireless80211 || type == NetworkInterfaceType.Ethernet);

                                clientDone.Set();
                            }), null);

                        clientDone.WaitOne(2000); 
                        return ret;
                    }
                    else
                    {
                        return true;
                    }
                }
#else
                
                IReadOnlyList<HostName> hostNames = NetworkInformation.GetHostNames();

                foreach (HostName hostName in hostNames)
                {
                    IPInformation ipif = hostName.IPInformation;
                    String hn = hostName.ToString();

                    //IANA Tpe : WiFi => 71, 셀룰러 => 244, 243등
                    if (hostName.Type == HostNameType.Ipv4 && ipif.NetworkAdapter.IanaInterfaceType == 71)
                    {
                        return true;
                    }
                }
#endif
                return false;
    */
            }
        }

        public static bool IsNetworkAvailable
        {
            get
            {
                return DeviceNetworkInformation.IsNetworkAvailable;
            }
        }

        public static bool IsCellularDataEnabled
        {
            get
            {
                return DeviceNetworkInformation.IsCellularDataEnabled;
            }
        }

        public static bool IsCellularDataRoamingEnabled
        {
            get
            {
                return DeviceNetworkInformation.IsCellularDataRoamingEnabled;
            }
        }

        public static bool IsWiFiEnabled
        {
            get
            {
                return DeviceNetworkInformation.IsWiFiEnabled;
            }
        }

        public static void ShowWiFiSettingPage(string message, string title)
        {
            Deployment.Current.Dispatcher.BeginInvoke(() =>
            {
                //if (MessageBox.Show(AppResources.AppMessageRequiredWiFi, AppResources.AppMessageNotification, MessageBoxButton.OKCancel) == MessageBoxResult.OK)
                if (MessageBox.Show(message, title, MessageBoxButton.OKCancel) == MessageBoxResult.OK)
                {
                    new Microsoft.Phone.Tasks.ConnectionSettingsTask().Show();
                }
            });
        }

        public static void WakeOnLan(string wolIp, List<byte[]> macAddressList)
        {
            byte[] packet = null;
            SocketAsyncEventArgs arg = null;

            foreach (byte[] macAddress in macAddressList)
            {
                arg = new SocketAsyncEventArgs();
                packet = new byte[17 * 6];
                //
                // Trailer of 6 times 0xFF.
                //
                for (int i = 0; i < 6; i++)
                    packet[i] = 0xFF;

                //
                // Body of magic packet contains 16 times the MAC address.
                //
                for (int i = 1; i <= 16; i++)
                    for (int j = 0; j < 6; j++)
                        packet[i * 6 + j] = macAddress[j];

                IPEndPoint ipEp = new IPEndPoint(IPAddress.Parse(wolIp), 40000);
                arg.SetBuffer(packet, 0, packet.Length);
                arg.RemoteEndPoint = ipEp;

                UdpSocket wolSocket = new UdpSocket();
                wolSocket.SendToAsync(arg);

                //데이터 사용량 누적
                if (arg.BytesTransferred > 0)
                {
                    AppLoader.CellularDataUtil.SumUsageCellularData(arg.BytesTransferred);
                }
            }
        }

        public static string GetSubnetmask(String ipaddress)
        {
            uint firstOctet = GetFirtsOctet(ipaddress);
            if (firstOctet >= 0 && firstOctet <= 127)
                return "255.0.0.0";
            else if (firstOctet >= 128 && firstOctet <= 191)
                return "255.255.0.0";
            else if (firstOctet >= 192 && firstOctet <= 223)
                return "255.255.255.0";
            else return "0.0.0.0";
        }

        public static uint GetFirtsOctet(string ipAddress)
        {
            System.Net.IPAddress iPAddress = System.Net.IPAddress.Parse(ipAddress);
            byte[] byteIP = iPAddress.GetAddressBytes();
            uint ipInUint = (uint)byteIP[0];     
            return ipInUint;
        }

        public static bool IsPrivateNetwork(string ip)
        {
            //RFC1918 이름  IP 주소 범위                   주소 개수      클래스 내용            최대 사이더 블록 (서브넷 마스크)  호스트 ID 크기
            //24비트 블록    10.0.0.0 – 10.255.255.255      16,777,216     클래스 A 하나          10.0.0.0/8 (255.0.0.0)            24 비트 
            //20비트 블록    172.16.0.0 – 172.31.255.255    1,048,576      16개의 인접 클래스 B   172.16.0.0/12 (255.240.0.0)       20 비트 
            //16비트 블록    192.168.0.0 – 192.168.255.255  65,536         256개의 인접 클래스 C  192.168.0.0/16 (255.255.0.0)      16 비트 
            bool isPrivateIp = false;
            string[] currIp = ip.Split('.');
            if (ip == "0.0.0.0" || ip == "127.0.0.1"
                || currIp[0] == "10"
                || (currIp[0] == "172" && Convert.ToSByte(currIp[1]) >= 16 && Convert.ToSByte(currIp[1]) <= 31)
                || (currIp[0] == "192" && currIp[1] == "168"))
            {
                isPrivateIp = true;
            }

            return isPrivateIp;
        }
    }
}
