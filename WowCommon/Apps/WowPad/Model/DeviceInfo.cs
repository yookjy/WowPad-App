using Microsoft.Phone.Controls;
using Microsoft.Phone.Info;
using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Interop;

namespace Apps.WowPad.Model
{
    public class DeviceInfo
    {
        private static PageOrientation pageOrientation;

        private static int actualWidth;

        private static int actualHeight;

        private static string name;

        private static int battery;

        public static void Load(double width, double height)
        {
            if (Application.Current.RootVisual == null)
            {
                pageOrientation = PageOrientation.Landscape;
            }
            else
            {
                pageOrientation = ((PhoneApplicationFrame)Application.Current.RootVisual).Orientation;
            }

            actualWidth = (int)width;
            actualHeight = (int)height;

            name = Model + "\0";
#if !WP7
            battery = Windows.Phone.Devices.Power.Battery.GetDefault().RemainingChargePercent;
#else
            battery = 101;
#endif
        }

        public static void Load(FrameworkElement elem)
        {
            Load(elem.ActualHeight, elem.ActualWidth);  //세로 모드인척...
        }

        public static void Load()
        {
            Load(Application.Current.Host.Content.ActualWidth, Application.Current.Host.Content.ActualHeight);
        }

        public static int ScreenWidth
        {
            get
            {
                if (pageOrientation == PageOrientation.Portrait
                    || pageOrientation == PageOrientation.PortraitDown
                    || pageOrientation == PageOrientation.PortraitUp)
                {
                    return actualWidth;
                }
                else
                {
                    return actualHeight;
                }
            }
        }

        public static int ScreenHeight
        {
            get
            {
                if (pageOrientation == PageOrientation.Landscape
                    || pageOrientation == PageOrientation.LandscapeLeft
                    || pageOrientation == PageOrientation.LandscapeRight)
                {
                    return actualWidth;
                }
                else
                {
                    return actualHeight;
                }
            }
        }

        public static int ScaleFactor
        {
            get
            {
#if !WP7
                return Application.Current.Host.Content.ScaleFactor;
#else
                return 100;
#endif
            }
        }

        public static int Battery
        {
            get
            {
                return battery;
            }
            set
            {
                battery = value;
            }
        }

        public static String Name
        {
            //return Microsoft.Phone.Info.DeviceStatus.DeviceName;
            get
            {
                return name;
            }
        }

        public static int OsMajorVersion
        {
            get
            {
                return Environment.OSVersion.Version.Major;
            }
        }

        public static int OsMinorVersion
        {
            get
            {
                return Environment.OSVersion.Version.Minor;
            }
        }

        private static string Model
        {
            get
            {
                /*
                string manufacturer = DeviceStatus.DeviceManufacturer;
                string model = string.Empty;
                if (manufacturer.Equals("NOKIA"))
                {
                    model = manufacturer + " ";
                    string name = DeviceStatus.DeviceName.Substring(0, 6);
                    switch (name)
                    {
                        case "RM-808":
                        case "RM-823":
                            model += "Lumia 900";
                            break;
                        case "RM-923":
                            model += "Lumia 505";
                            break;
                        case "RM-889":
                        case "RM-898":
                            model += "Lumia 510";
                            break;
                        case "RM-913":
                            model += "Lumia 520T";
                            break;
                        case "RM-914":
                        case "RM-915":
                            model += "Lumia 520";
                            break;
                        case "RM-917":
                            model += "Lumia 521";
                            break;
                        case "RM-835":
                        case "RM-849":
                            model += "Lumia 610";
                            break;
                        case "RM-836":
                            model += "Lumia 610C";
                            break;
                        case "RM-846":
                            model += "Lumia 620";
                            break;
                        case "RM-941":
                        case "RM-942":
                        case "RM-943":
                            model += "Lumia 625";
                            break;
                        case "RM-803":
                        case "RM-809":
                            model += "Lumia 710";
                            break;
                        case "RM-885":
                            model += "Lumia 720";
                            break;
                        case "RM-887":
                            model += "Lumia 720T";
                            break;
                        case "RM-878":
                            model += "Lumia 810";
                            break;
                        case "RM-824":
                        case "RM-825":
                        case "RM-826":
                            model += "Lumia 820";
                            break;
                        case "RM-845":
                            model += "Lumia 822";
                            break;
                        case "RM-820":
                        case "RM-821":
                        case "RM-822":
                            model += "Lumia 920";
                            break;
                        case "RM-860":
                            model += "Lumia 928";
                            break;
                        case "RM-867":
                            model += "Lumia 920T";
                            break;
                        case "RM-892":
                        case "RM-893":
                        case "RM-910":
                            model += "Lumia 925";
                            break;
                        case "RM-875":
                        case "RM-876":
                        case "RM-877":
                            model += "Lumia 1020";
                            break;
                        case "RM-994":
                        case "RM-995":
                        case "RM-996":
                            model += "Lumia 1320";
                            break;
                        case "RM-937":
                        case "RM-938":
                        case "RM-939":
                            model += "Lumia 1520";
                            break;
                        default:
                            model += "Lumia Series";
                            break;
                        //- Huawei H889L
                        //- Samsung SCH-I930
                    }
                }
                else if (manufacturer.Equals("HTC"))
                {
                    string partModel = DeviceStatus.DeviceName;
                    model = manufacturer + " ";
                    if (partModel.ToUpper().Contains("8X") || partModel.ToUpper().Contains("ACCORD") || partModel.ToUpper().Contains("6990"))
                    {
                        model += "8X";
                    }
                    else if (partModel.ToUpper().Contains("8S") || partModel.ToUpper().Contains("RIO"))
                    {
                        model += "8S";
                    }
                    else
                    {
                        model += partModel;
                    }
                }
                else if (manufacturer.Equals("Huawei"))
                {
                    //Huawei Ascend W1 (untested)
                    model = DeviceStatus.DeviceName;
                }
                else if (manufacturer.Equals("Samsung"))
                {
                    //Samsung Ativ S, Samsung Ativ Odyssey (untested)
                    model = DeviceStatus.DeviceName;
                }
                else
                {
                    model = "Windows Phone " + Environment.OSVersion.Version.Major;
                }

                model = model.Trim();

                return model;
                 **/
                string name = NetHelper.DeviceName.PhoneName;
                string peerName = Windows.Networking.Proximity.PeerFinder.DisplayName;

                if (!string.IsNullOrEmpty(peerName) && peerName.Length <= name.Length)
                {
                    name = peerName + name.Substring(peerName.Length);
                }
                return name;
            }
        }
    }
}
