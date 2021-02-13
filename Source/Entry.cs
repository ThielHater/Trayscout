using System;
using System.Collections.Generic;
using System.Linq;

namespace Trayscout
{
    public class Entry : IEquatable<Entry>
    {
        public DateTime Timestamp { get; }
        public int Value { get; }
        public IList<short> Digits { get; }
        public Trend Trend { get; set; }

        public Entry(string line)
        {
            IList<string> split = line.Split('\t').Select(x => x.Trim('"')).ToList();
            Timestamp = DateTime.Parse(split[0]);
            Value = int.Parse(split[2]);
            Digits = IntToArray(Value);
            if (!Enum.TryParse(split[3], out Trend trend))
                trend = Trend.None;
            Trend = trend;
        }

        public Entry(DateTime timestamp, int value, Trend trend)
        {
            Timestamp = timestamp;
            Value = value;
            Digits = IntToArray(Value);
            Trend = trend;
        }

        private IList<short> IntToArray(int value)
        {
            return value.ToString().Select(x => (short)(x - 48)).ToList();
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
