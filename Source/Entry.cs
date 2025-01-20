using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace Trayscout
{
    public class Entry : IEquatable<Entry>
    {
        public DateTime Timestamp { get; }
        public float Value { get; }
        public IList<short> Digits { get; }
        public Trend Trend { get; set; }

        public Entry(string line, Unit unit)
        {
            try
            {
                IList<string> split = line.Split('\t').Select(x => x.Trim('"')).ToList();
                try
                {
                    Timestamp = DateTime.Parse(split[0]);
                }
                catch
                {
                    long unixTimestamp = long.Parse(split[1]) / 1000;
                    DateTime epoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
                    Timestamp = epoch.AddSeconds(unixTimestamp);
                }
                int value = int.Parse(split[2]);
                // Nightscout always uses mg/dl even if DISPLAY_UNITS is set to mmol/L
                if (unit == Unit.mgdl)
                    Value = value;
                if (unit == Unit.mmolL)
                    Value = value * 0.0555f;
                Digits = GetDigits(Value, unit);
                if (!Enum.TryParse(split[3], out Trend trend))
                    trend = Trend.None;
                Trend = trend;
            }
            catch (Exception ex)
            {
                throw new Exception("Parsing entry '" + line + "' failed.", ex);
            }
        }

        public Entry(DateTime timestamp, float value, Unit unit, Trend trend)
        {
            Timestamp = timestamp;
            Value = value;
            Digits = GetDigits(Value, unit);
            Trend = trend;
        }

        private IList<short> GetDigits(float value, Unit unit)
        {
            string stringValue = value.ToString(unit == Unit.mmolL ? "F1" : "F0", CultureInfo.InvariantCulture);
            IList<short> digits = stringValue.Select(x => x == '.' ? (short)10 : (short)(x - 48)).ToList();
            return digits;
        }

        public bool Equals(Entry other)
        {
            if (other == null)
                return false;
            else
                return other.Timestamp == Timestamp && other.Value == Value;
        }

        public override int GetHashCode()
        {
            return Timestamp.GetHashCode();
        }
    }
}
