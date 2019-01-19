using Apps.WowPad.Model;
using System;
using System.Collections.Generic;
using System.IO.IsolatedStorage;

namespace Apps.WowPad.Manager
{
    public class DateTimeComparer : IComparer<ServerInfo>
    {
        public int Compare(ServerInfo x, ServerInfo y)
        {
            return y.LastDateTime.CompareTo(x.LastDateTime);
        }
    }

    public class HistorygManager
    {
        private const String KEY_PC_HISTORIES = "pcHistories";

        private const int MAX_HISTORY_CNT = 20;

        private IsolatedStorageSettings settings;

        private List<ServerInfo> serverInfoList;

        public List<ServerInfo> ServerInfoList 
        {
            get
            {
                return serverInfoList;
            }
        }

        private static HistorygManager instance;

        public static HistorygManager Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new HistorygManager();
                }
                return instance;
            }
        }

        private HistorygManager()
        {
            this.settings = IsolatedStorageSettings.ApplicationSettings;
        }

        public void Load()
        {
            if (IsolatedStorageSettings.ApplicationSettings.Contains(KEY_PC_HISTORIES))
            {
                serverInfoList = IsolatedStorageSettings.ApplicationSettings[KEY_PC_HISTORIES] as List<ServerInfo>;
            }
            else
            {
                serverInfoList = new List<ServerInfo>();
                IsolatedStorageSettings.ApplicationSettings.Add(KEY_PC_HISTORIES, serverInfoList);
            }
            serverInfoList.Sort(new DateTimeComparer());
        }

        public void Add(ServerInfo serverInfo)
        {
            for (int i = 0; i < serverInfoList.Count; i++)
            {
                ServerInfo si = serverInfoList[i];

                foreach (byte[] mac in si.MacAddressList)
                {
                    bool isDiff = false;

                    foreach (byte[] newMac in serverInfo.MacAddressList)
                    {
                        
                        for (int j = 0; j < mac.Length; j++)
                        {
                            //맥과 IP모두 다른경우 다른걸로 취급함
                            if (mac[j] != newMac[j] || si.ServerIP != serverInfo.ServerIP)
                            {
                                isDiff = true;
                                continue;
                            }
                        }

                        if (!isDiff)
                        {
                            serverInfoList.RemoveAt(i);
                            break;
                        }
                    }
                    if (!isDiff)
                    {
                        break;
                    }
                }
            }
            //신규 추가
            this.serverInfoList.Add(serverInfo);
            //기준 갯수를 초과하면 가장 오래된것 부터 삭제 (신규 추가한것은 제외) : 총 20개의 이력을 보유
            for (int i = serverInfoList.Count - 2; i >= MAX_HISTORY_CNT - 1; i--)
            {
                serverInfoList.RemoveAt(i);
            }
            //저장 및 갱신
            Update(serverInfoList);
            Load();
        }

        public void RemoveAt(int index)
        {
            this.serverInfoList.RemoveAt(index);
            Update(serverInfoList);
            Load();
        }

        public void Remove(ServerInfo serverInfo)
        {
            this.serverInfoList.Remove(serverInfo);
            Update(serverInfoList);
            Load();
        }
        
        public void Update(List<ServerInfo> serverInfoList)
        {
            if (!IsolatedStorageSettings.ApplicationSettings.Contains(KEY_PC_HISTORIES))
            {
                IsolatedStorageSettings.ApplicationSettings.Add(KEY_PC_HISTORIES, serverInfoList);
            }
            else
            {
                IsolatedStorageSettings.ApplicationSettings[KEY_PC_HISTORIES] = serverInfoList;
            }
            
            IsolatedStorageSettings.ApplicationSettings.Save();
        }

        
    }
}
