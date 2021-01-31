using System;
using System.Collections.Generic;
using System.Linq;

namespace Trayscout
{
    public class Entry
    {
        public DateTime Timestamp { get; }
        public int Value { get; }
        public IList<short> Digits { get; }
        public Direction Direction { get; set; }

        public Entry(string line)
        {
            IList<string> split = line.Split('\t').Select(x => x.Trim('"')).ToList();
            if (split.Any())
            {
                Timestamp = DateTime.Parse(split[0]);
                Value = int.Parse(split[2]);
                Digits = split[2].Select(x => short.Parse(x.ToString())).ToList();
                if (!Enum.TryParse(split[3], out Direction direction))
                    direction = Direction.None;
                Direction = direction;
            }
        }
    }
}
