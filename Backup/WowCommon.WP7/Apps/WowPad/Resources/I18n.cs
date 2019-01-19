using Apps.WowPad.Model;
using System;
using System.Globalization;
using System.Resources;
using System.Windows;

namespace Apps.WowPad.Resources
{
    public class I18n
    {
        private static ResourceManager rscMgr;

        private static CultureInfo culture;

        public static void Load(ResourceManager resourceManager, CultureInfo cultureInfo)
        {
            rscMgr = resourceManager;
            culture = cultureInfo;
        }

        public static string GetString(string key)
        {
            return rscMgr.GetString(key, culture);
        }
    }
}
