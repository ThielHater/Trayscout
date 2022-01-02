using System;
using System.Globalization;
using System.Runtime.InteropServices;
using System.Text;

namespace Trayscout
{
    public class Ini
    {
        [DllImport("kernel32", CharSet = CharSet.Unicode)]
        static extern int GetPrivateProfileString(string Section, string Key, string Default, StringBuilder RetVal, int Size, string FilePath);

        private string _filePath { get; }

        public Ini(string filePath)
        {
            _filePath = filePath;
        }

        public string ReadString(string section, string key)
        {
            StringBuilder sb = new StringBuilder(255);
            GetPrivateProfileString(section, key, "", sb, 255, _filePath);
            string value = sb.ToString();
            if (string.IsNullOrWhiteSpace(value))
                throw new Exception(key + " is not configured.");
            return value;
        }

        public int ReadInt(string section, string key)
        {
            string value = ReadString(section, key);
            if (!int.TryParse(value, out int intValue))
                throw new Exception(key + " is not an integer.");
            return intValue;
        }

        public float ReadFloat(string section, string key)
        {
            string value = ReadString(section, key);
            if (!float.TryParse(value, NumberStyles.AllowDecimalPoint, CultureInfo.InvariantCulture, out float floatValue))
                throw new Exception(key + " is not a floating point number.");
            return floatValue;
        }

        public bool ReadBool(string section, string key)
        {
            string value = ReadString(section, key);
            bool boolValue = value == "1";
            return boolValue;
        }

        public T ReadEnum<T>(string section, string key) where T : struct
        {
            string value = ReadString(section, key);
            if (!Enum.TryParse(value, out T enumValue))
                throw new Exception(key + " is not valid.");
            return enumValue;
        }
    }
}
