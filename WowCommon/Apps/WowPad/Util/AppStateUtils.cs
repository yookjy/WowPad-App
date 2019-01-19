using Apps.WowPad.Resources;
using Apps.WowPad.Type;
using Microsoft.Phone.Shell;
using System.Collections.Generic;

namespace Apps.WowPad.Util
{
    public class AppStateUtils
    {
        public static object Get(string key)
        {
            if (PhoneApplicationService.Current.State.ContainsKey(key))
            {
                return PhoneApplicationService.Current.State[key];
            }
            return null;
        }

        public static void Remove(string key)
        {
            if (PhoneApplicationService.Current.State.ContainsKey(key))
            {
                PhoneApplicationService.Current.State.Remove(key);
            }
        }

        public static void Set(string key, object value)
        {
            if (PhoneApplicationService.Current.State.ContainsKey(key))
            {
                PhoneApplicationService.Current.State[key] = value;
            }
            else
            {
                PhoneApplicationService.Current.State.Add(key, value);
            }
        }

        //리커버리 모드를 추가한다.
        public static void AddRecoveryType(RecoveryTypes recoveryType)
        {
            string key = Constant.KEY_RECOVERY_MODE;
            List<RecoveryTypes> recoveryList = null;

            if (PhoneApplicationService.Current.State.ContainsKey(key))
            {
                recoveryList = PhoneApplicationService.Current.State[key] as List<RecoveryTypes>;
            }
            else
            {
                recoveryList = new List<RecoveryTypes>();
                PhoneApplicationService.Current.State[key] = recoveryList;
            }

            recoveryList.Add(recoveryType);
        }

        //설정된 리커버리 모드를 조회한다.
        public static List<RecoveryTypes> GetRecoveryTypes()
        {
            string key = Constant.KEY_RECOVERY_MODE;
            List<RecoveryTypes> recoveryList = null;

            if (PhoneApplicationService.Current.State.ContainsKey(key))
            {
                recoveryList = PhoneApplicationService.Current.State[key] as List<RecoveryTypes>;
            }

            return recoveryList;
        }

        //설정된 리커버리 모드를 초기화 한다.
        public static bool ClearRecoveryTypes()
        {
            bool result = false;
            string key = Constant.KEY_RECOVERY_MODE;

            if (PhoneApplicationService.Current.State.ContainsKey(key))
            {
                result = PhoneApplicationService.Current.State.Remove(key);
            }
            return result;
        }

        public static bool ContainsRecoveryType(RecoveryTypes recoveryType)
        {
            bool result = false;
            List<RecoveryTypes> recoveryList = GetRecoveryTypes();
            
            if (recoveryList != null)
            {
                result = recoveryList.Contains(recoveryType);
            }
            return result;
        }
    }
}