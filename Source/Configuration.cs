using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace Trayscout
{
    public class Configuration
    {
        public string BaseUrl { get; }
        public string ApiSecretHash { get; }
        public int UpdateInterval { get; }
        public int High { get; }
        public int Low { get; }
        public bool UseColor { get; }
        public bool UseAlarm { get; }
        public int AlarmInterval { get; }
        public int TimeRange { get; }
        public BaseStyle Style { get; }

        private IDictionary<StyleKey, Type> _styles = new Dictionary<StyleKey, Type>
        {
            { StyleKey.Light, typeof(LightStyle) },
            { StyleKey.AndroidAPS, typeof(AndroidApsStyle) },
            { StyleKey.xDrip, typeof(XDripStyle) },
            { StyleKey.Dexcom, typeof(DexcomStyle) }
        };

        public Configuration(Ini ini)
        {
            BaseUrl = ini.ReadString("Config", "BaseUrl");
            if (!BaseUrl.EndsWith("/"))
                BaseUrl += "/";
            if (!BaseUrl.EndsWith("api/v1/"))
                BaseUrl += "api/v1/";
            ApiSecretHash = Sha1(ini.ReadString("Config", "APISecret"));
            UpdateInterval = ini.ReadInt("Config", "UpdateInterval");
            High = ini.ReadInt("Config", "High");
            Low = ini.ReadInt("Config", "Low");
            UseColor = ini.ReadBool("Config", "UseColor");
            UseAlarm = ini.ReadBool("Config", "UseAlarm");
            AlarmInterval = ini.ReadInt("Config", "AlarmInterval");
            TimeRange = Math.Min(ini.ReadInt("Config", "TimeRange"), 6);
            StyleKey styleKey = ini.ReadEnum<StyleKey>("Config", "Style");
            Style = (BaseStyle)Activator.CreateInstance(_styles[styleKey]);
        }

        private string Sha1(string input)
        {
            byte[] hash = new SHA1Managed().ComputeHash(Encoding.UTF8.GetBytes(input));
            return string.Concat(hash.Select(b => b.ToString("x2")));
        }
    }
}
