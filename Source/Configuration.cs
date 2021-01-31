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
        }

        private string Sha1(string input)
        {
            byte[] hash = new SHA1Managed().ComputeHash(Encoding.UTF8.GetBytes(input));
            return string.Concat(hash.Select(b => b.ToString("x2")));
        }
    }
}
