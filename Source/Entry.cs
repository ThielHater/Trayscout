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
            Digits = split[2].Select(x => short.Parse(x.ToString())).ToList();
            if (!Enum.TryParse(split[3], out Trend trend))
                trend = Trend.None;
            Trend = trend;
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
