using Apps.UI.Notification;
using Apps.WowPad.Manager;

namespace Apps.WowPad.Util
{
    public class CellularDataUtils
    {
        private long usageCellularData;

        private long lastNotiDataUsage;

        public void SumUsageCellularData(int usage)
        {
            if (NetworkUtils.IsNetworkAvailable && !NetworkUtils.IsWiFiNetwork)
            //if (NetworkUtils.IsNetworkAvailable)
            {
                long tmp = usageCellularData;
                usageCellularData += usage;

                if (tmp != usageCellularData)
                {
                    int perUsage = SettingManager.Instance.SettingInfo.CellularDataUsage;
                    //int perUsage = 1;

                    if (GetMegaBytes(usageCellularData) / perUsage > 0 && (GetMegaBytes(lastNotiDataUsage) / perUsage < GetMegaBytes(usageCellularData) / perUsage))
                    {
                        UsageMB(usageCellularData);
                    }
                }
            }
        }

        private int GetMegaBytes(long value)
        {
            return (int)value / 1024 / 1024; 
        }

        public event CellularDataEventHandler NotifyUsagePerMegaBytes;

        virtual protected void UsageMB(long usageCellularData)
        {
            if (NotifyUsagePerMegaBytes != null)
            {
                NotifyUsagePerMegaBytes(this, new CellularDataEventArgs(usageCellularData));
                lastNotiDataUsage = usageCellularData;
            }
        }
    }
}
