using Apps.WowPad.Model;
using Apps.WowPad.Type;
using System;
using System.IO.IsolatedStorage;

namespace Apps.WowPad.Manager
{
    public class SettingManager
    {
        public const String KEY_GETTING_START = "gettingStart";
        public const String KEY_DEVICE_TYPE = "deviceType";
        public const String KEY_IMAGE_QUALITY = "imageQuality";
        public const String KEY_USE_EXTEND_BUTTON = "useExtendButton";
        public const String KEY_DEFAULT_PORT = "defaultPort";
        public const String KEY_AUTO_CONNECT = "autoConnect";
        public const String KEY_SERVER_IP = "serverIP";
        public const String KEY_SERVER_NAME = "serverName";
        public const String KEY_SERVER_TCP_PORT = "serverTcpPort";
        public const String KEY_SERVER_UDP_PORT = "serverUdpPort";
        public const String KEY_SERVER_ACCESS_CODE = "serverAccessCode";
        public const String KEY_SEARCH_DEFAULT_PAGE = "searchDefaultPage";
        public const String KEY_CELLULAR_DATA_USAGE = "cellularDataUsage";
        public const String KEY_FULLSIZE_KEYBOARD = "fullSizeKeyboard";

        private IsolatedStorageSettings settings; 

        public SettingInfo SettingInfo { get; set; }

        private static SettingManager instance;

        public static SettingManager Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new SettingManager();
                }
                return instance;
            }
        }
        
        private SettingManager()
        {
            this.settings = IsolatedStorageSettings.ApplicationSettings;
            this.SettingInfo = new SettingInfo();
        }

        public void Load()
        {
            Boolean isFirst = true;
            //디폴트값 설정
            this.SettingInfo.GettingStart = true;
            this.SettingInfo.DeviceType = DeviceTypes.Mouse;
            this.SettingInfo.ImageQualityType = ImageQualityTypes.Medium;
            this.SettingInfo.UseExtendButton = true;
            this.SettingInfo.DefaultPort = 9000;
            this.SettingInfo.AutoConnect = true;
            this.SettingInfo.ServerIP = "";
            this.SettingInfo.TcpPort = 0;
            this.SettingInfo.UdpPort = 0;
            this.SettingInfo.AccessCode = 0;
            this.SettingInfo.SearchDefaultPageIndex = 0;
            this.SettingInfo.CellularDataUsage = 100;
            this.SettingInfo.FullSizeKeyboard = true;
                        
            //저장값 불러오기
            foreach (String key in settings.Keys)
            {
                switch(key)
                {
                    case KEY_GETTING_START:
                        isFirst = false;
                        this.SettingInfo.GettingStart = (Boolean)this.settings[key];
                        break;
                    case KEY_DEVICE_TYPE:
                        isFirst = false;
                        this.SettingInfo.DeviceType = (DeviceTypes)this.settings[key];
                        break;
                    case KEY_IMAGE_QUALITY:
                        isFirst = false;
                        this.SettingInfo.ImageQualityType = (ImageQualityTypes)this.settings[key];
                        break;
                    case KEY_USE_EXTEND_BUTTON:
                        isFirst = false;
                        this.SettingInfo.UseExtendButton = (Boolean)this.settings[key];
                        break;
                    case KEY_DEFAULT_PORT:
                        isFirst = false;
                        this.SettingInfo.DefaultPort = (int)this.settings[key];
                        break;
                    case KEY_AUTO_CONNECT:
                        isFirst = false;
                        this.SettingInfo.AutoConnect = (Boolean)this.settings[key];
                        break;
                    case KEY_SERVER_NAME:
                        isFirst = false;
                        this.SettingInfo.ServerName = (String)this.settings[key];
                        break;
                    case KEY_SERVER_IP:
                        isFirst = false;
                        this.SettingInfo.ServerIP = (String)this.settings[key];
                        break;
                    case KEY_SERVER_TCP_PORT:
                        isFirst = false;
                        this.SettingInfo.TcpPort = (int)this.settings[key];
                        break;
                    case KEY_SERVER_UDP_PORT:
                        isFirst = false;
                        this.SettingInfo.UdpPort = (int)this.settings[key];
                        break;
                    case KEY_SERVER_ACCESS_CODE:
                        isFirst = false;
                        this.SettingInfo.AccessCode = (int)this.settings[key];
                        break;
                    case KEY_SEARCH_DEFAULT_PAGE:
                        isFirst = false;
                        this.SettingInfo.SearchDefaultPageIndex= (int)this.settings[key];
                        break;
                    case KEY_CELLULAR_DATA_USAGE:
                        isFirst = false;
                        this.SettingInfo.CellularDataUsage = (int)this.settings[key];
                        break;
                    case KEY_FULLSIZE_KEYBOARD:
                        isFirst = false;
                        this.SettingInfo.FullSizeKeyboard = (Boolean)this.settings[key];
                        break;
                }
            }

            //최초 한번
            if (isFirst)
            { 
                Save(); 
            }
        }

        public void Save()
        {
            SettingManager.Update(KEY_GETTING_START, this.SettingInfo.GettingStart);
            SettingManager.Update(KEY_DEVICE_TYPE, this.SettingInfo.DeviceType);
            SettingManager.Update(KEY_IMAGE_QUALITY, this.SettingInfo.ImageQualityType);
            SettingManager.Update(KEY_USE_EXTEND_BUTTON, this.SettingInfo.UseExtendButton);
            SettingManager.Update(KEY_DEFAULT_PORT, this.SettingInfo.DefaultPort);
            SettingManager.Update(KEY_AUTO_CONNECT, this.SettingInfo.AutoConnect);
            SettingManager.Update(KEY_SERVER_NAME, this.SettingInfo.ServerName);
            SettingManager.Update(KEY_SERVER_IP, this.SettingInfo.ServerIP);
            SettingManager.Update(KEY_SERVER_TCP_PORT, this.SettingInfo.TcpPort);
            SettingManager.Update(KEY_SERVER_UDP_PORT, this.SettingInfo.UdpPort);
            SettingManager.Update(KEY_SERVER_ACCESS_CODE, this.SettingInfo.AccessCode);
            SettingManager.Update(KEY_SEARCH_DEFAULT_PAGE, this.SettingInfo.SearchDefaultPageIndex);
            SettingManager.Update(KEY_CELLULAR_DATA_USAGE, this.SettingInfo.CellularDataUsage);
            SettingManager.Update(KEY_FULLSIZE_KEYBOARD, this.SettingInfo.FullSizeKeyboard);
            IsolatedStorageSettings.ApplicationSettings.Save();
        }

        public static void Update(string key, object value)
        {
            if (!IsolatedStorageSettings.ApplicationSettings.Contains(key))
            {
                IsolatedStorageSettings.ApplicationSettings.Add(key, value);
            }
            else
            {
                IsolatedStorageSettings.ApplicationSettings[key] = value;
            }
            IsolatedStorageSettings.ApplicationSettings.Save();
        }

        public static void Set(string key, object value)
        {
            if (!IsolatedStorageSettings.ApplicationSettings.Contains(key))
            {
                IsolatedStorageSettings.ApplicationSettings.Add(key, value);
            }
            else
            {
                IsolatedStorageSettings.ApplicationSettings[key] = value;
            }
        }

        public static void Update()
        {
            IsolatedStorageSettings.ApplicationSettings.Save();
        }

        public void UpdateNLoad(String key, Object value)
        {
            //저장
            SettingManager.Update(key, value);
            //로드
            switch (key)
            {
                case KEY_GETTING_START:
                    this.SettingInfo.GettingStart = (Boolean)this.settings[key];
                    break;
                case KEY_DEVICE_TYPE:
                    this.SettingInfo.DeviceType = (DeviceTypes)this.settings[key];
                    break;
                case KEY_IMAGE_QUALITY:
                    this.SettingInfo.ImageQualityType = (ImageQualityTypes)this.settings[key];
                    break;
                case KEY_USE_EXTEND_BUTTON:
                    this.SettingInfo.UseExtendButton = (Boolean)this.settings[key];
                    break;
                case KEY_DEFAULT_PORT:
                    this.SettingInfo.DefaultPort = (int)this.settings[key];
                    break;
                case KEY_AUTO_CONNECT:
                    this.SettingInfo.AutoConnect = (Boolean)this.settings[key];
                    break;
                case KEY_SERVER_NAME:
                    this.SettingInfo.ServerName = (string)this.settings[key];
                    break;
                case KEY_SERVER_IP:
                    this.SettingInfo.ServerIP = (string)this.settings[key];
                    break;
                case KEY_SERVER_TCP_PORT:
                    this.SettingInfo.TcpPort = (int)this.settings[key];
                    break;
                case KEY_SERVER_UDP_PORT:
                    this.SettingInfo.UdpPort = (int)this.settings[key];
                    break;
                case KEY_SERVER_ACCESS_CODE:
                    this.SettingInfo.AccessCode = (int)this.settings[key];
                    break;
                case KEY_SEARCH_DEFAULT_PAGE:
                    this.SettingInfo.SearchDefaultPageIndex = (int)this.settings[key];
                    break;
                case KEY_CELLULAR_DATA_USAGE:
                    this.SettingInfo.CellularDataUsage = (int)this.settings[key];
                    break;
                case KEY_FULLSIZE_KEYBOARD:
                    this.SettingInfo.FullSizeKeyboard = (Boolean)this.settings[key];
                    break;
            }
        }
    }
}
