using Apps.WowPad.Manager;
using Apps.WowPad.Model;
using Apps.WowPad.Resources;
using Apps.WowPad.Type;
using Apps.WowPad.Util;
using Microsoft.Phone.Marketplace;
using Microsoft.Phone.Shell;
using System.Globalization;
using System.Resources;

namespace Apps.WowPad
{
    public class AppLoader
    {
        public static CellularDataUtils CellularDataUtil;

        public static void Initialize(VersionTypes version, ResourceManager resourceManager, CultureInfo cultureInfo)
        {
            //언어리소스 로드
            I18n.Load(resourceManager, cultureInfo);
            //버전 설정
            VersionUtils.VersionType = version;
            //버전 처리
            if (!VersionUtils.IsAdvertising)
            {
                //광고 버전이 아니면 체험판 확인
                VersionUtils.IsTrial = new LicenseInformation().IsTrial();
                //VersionUtils.IsTrial = true;
            }
            else
            {
                System.Threading.Thread.CurrentThread.CurrentCulture = new System.Globalization.CultureInfo("en-US");
            }

            //기본설정 로드
            SettingManager.Instance.Load();
            //시작 디바이스 설정
            if (!VersionUtils.IsFull)
            {
                SettingManager.Instance.UpdateNLoad(SettingManager.KEY_DEVICE_TYPE, VersionUtils.IsTouchscreen ? DeviceTypes.TouchScreen : DeviceTypes.Mouse);
            }
            //히스토리 로드
            HistorygManager.Instance.Load();
            //프로퍼티 설정
            KeyboardControlManager.Instance.ConnectionInfo = ConnectionManager.Instance.ConnectionInfo;
            PointingControlManager.Instance.ConnectionInfo = ConnectionManager.Instance.ConnectionInfo;
            PointingControlManager.Instance.SettingInfo = SettingManager.Instance.SettingInfo;

            if (CellularDataUtil == null)
            {
                CellularDataUtil = new CellularDataUtils();
            }
        }

        public static void Launching(LaunchingEventArgs e)
        {
        }

        public static void Activated(ActivatedEventArgs e)
        {
            //DeviceInfo.Load();
            //테마 변경을 적용하기 위해 다시 들어오는 경우 로딩
            ResourceUri.Load();
        }

        public static void Deactivated(DeactivatedEventArgs e)
        {
            ConnectionInfo connInfo = ConnectionManager.Instance.ConnectionInfo;
            //연결 상태였다면 리커버리 모드 셋팅
            if (connInfo.CurrentServer != null && connInfo.CurrentServer.IsConnected)
            {
                //소켓을 끊김 상태로 변경
                connInfo.CurrentServer.IsConnected = false;
                //복구 모드 설정
                AppStateUtils.AddRecoveryType(Apps.WowPad.Type.RecoveryTypes.Connection);
                //터치스크린이었다면 화면 복구 설정
                if (PointingControlManager.Instance.DeviceType == Apps.WowPad.Type.DeviceTypes.TouchScreen)
                {
                    AppStateUtils.AddRecoveryType(Apps.WowPad.Type.RecoveryTypes.BackgroundImage);
                }
            }
        }

    }
}
