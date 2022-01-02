using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;

namespace Trayscout
{
    public abstract class DotStyle : BaseStyle
    {
        protected override void DrawEntries(Graphics g, int width, int height, IList<Entry> entries, float low, float high, DateTime minX, DateTime maxX, int minY, int maxY)
        {
            g.SmoothingMode = SmoothingMode.HighQuality;

            for (int i = 0; i < entries.Count; i++)
            {
                int x = (int)IntervalScale(minX.Ticks, maxX.Ticks, entries[i].Timestamp.Ticks, 0, width - 1);
                int y = (int)IntervalScale(minY, maxY, entries[i].Value, 0, height - 1);

                Color color = GetGraphColor(low, high, entries[i].Value);
                g.FillEllipse(new SolidBrush(color), x - Radius, y - Radius, Radius * 2, Radius * 2);
            }

            g.SmoothingMode = SmoothingMode.None;
        }
    }
}
