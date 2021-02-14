using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Text;
using System.Linq;

namespace Trayscout
{
    public abstract class BaseStyle
    {
        public Color HighColor { get; protected set; }
        public Color LowColor { get; protected set; }
        public Color NormalColor { get; protected set; }
        public Color BackgroundColor { get; protected set; }
        public Color GridLinesColor { get; protected set; }
        public Color LabelColor { get; protected set; }
        public int Radius { get; protected set; }

        public Image DrawDiagram(Configuration config, IList<Entry> entries)
        {
            return DrawDiagram(config.Width, config.Height, config.FontFamily, config.FontSize, config.High, config.Low, config.TimeRange, entries);
        }

        public Image DrawDiagram(int width, int height, string fontFamily, int fontSize, int high, int low, int timeRange, IList<Entry> entries)
        {
            Bitmap bmp = new Bitmap(width, height);
            Graphics g = Graphics.FromImage(bmp);

            DateTime maxX = entries.Max(x => x.Timestamp);
            DateTime minX = maxX.AddHours(-timeRange);
            int minY = 0;
            int maxY = Math.Max(entries.Max(x => x.Value) + (int)(height * 0.1f), 400);
            int highY = (int)IntervalScale(minY, maxY, high, 0, height - 1);
            int lowY = (int)IntervalScale(minY, maxY, low, 0, height - 1);
            int alpha = (int)(256 * 0.2f);

            int stepX = timeRange <= 3 ? 30 : 60;
            int stepY = 100;

            DrawBackground(g, width, height);
            DrawGridLines(g, width, height, minX, maxX, stepX, minY, maxY, stepY);
            DrawAreas(g, width, height, lowY, highY, alpha);
            DrawAxisLabels(g, width, height, fontFamily, fontSize, minX, maxX, stepX, minY, maxY, stepY, bmp);
            DrawEntries(g, width, height, entries, low, high, minX, maxX, minY, maxY);
            DrawOuterBorder(g, width, height);

            bmp.RotateFlip(RotateFlipType.RotateNoneFlipY);

            return bmp;
        }

        protected virtual void DrawBackground(Graphics g, int width, int height)
        {
            g.FillRectangle(new SolidBrush(BackgroundColor), 0, 0, width - 1, height - 1);
        }

        protected virtual void DrawGridLines(Graphics g, int width, int height, DateTime minX, DateTime maxX, int stepX, int minY, int maxY, int stepY)
        {
            DateTime helpX = minX.AddMinutes(stepX);
            while (helpX < maxX)
            {
                int x = (int)IntervalScale(minX.Ticks, maxX.Ticks, helpX.Ticks, 0, width - 1);
                g.DrawLine(new Pen(GridLinesColor), x, 0, x, height - 1);
                helpX = helpX.AddMinutes(stepX);
            }

            int helpY = 0 + stepY;
            while (helpY < maxY)
            {
                int y = (int)IntervalScale(minY, maxY, helpY, 0, height - 1);
                g.DrawLine(new Pen(GridLinesColor), 0, y, width - 1, y);
                helpY += stepY;
            }
        }

        protected abstract void DrawAreas(Graphics g, int width, int height, int lowY, int highY, int alpha);

        protected virtual void DrawAxisLabels(Graphics g, int width, int height, string fontFamily, int fontSize, DateTime minX, DateTime maxX, int stepX, int minY, int maxY, int stepY, Bitmap bmp)
        {
            g.TextRenderingHint = TextRenderingHint.ClearTypeGridFit;
            double fontScale = fontSize / 8.0f;

            // flipping because DrawString() does not work as intended (https://stackoverflow.com/a/1486019)
            bmp.RotateFlip(RotateFlipType.RotateNoneFlipY);
            DateTime helpX = maxX;
            while (helpX > minX)
            {
                int x = (int)IntervalScale(minX.Ticks, maxX.Ticks, helpX.Ticks, 0, width - 1);
                g.DrawString(helpX.ToString("HH:mm"), new Font(fontFamily, fontSize), new SolidBrush(LabelColor), x - (int)(32 * fontScale), height - (int)(16 * fontScale), StringFormat.GenericDefault);
                helpX = helpX.AddMinutes(-stepX);
            }

            int helpY = 0;
            while (helpY < maxY)
            {
                int y = (int)IntervalScale(minY, maxY, helpY, 0, height - 1);
                g.DrawString(helpY.ToString(), new Font(fontFamily, fontSize), new SolidBrush(LabelColor), (int)(2 * fontScale), height - y - (int)(16 * fontScale), StringFormat.GenericDefault);
                helpY += stepY;
            }
            bmp.RotateFlip(RotateFlipType.RotateNoneFlipY);

            g.TextRenderingHint = TextRenderingHint.SystemDefault;
        }

        protected abstract void DrawEntries(Graphics g, int width, int height, IList<Entry> entries, int low, int high, DateTime minX, DateTime maxX, int minY, int maxY);

        protected virtual void DrawOuterBorder(Graphics g, int width, int height)
        {
            g.DrawRectangle(new Pen(GridLinesColor), 0, 0, width - 1, height - 1);
        }

        protected virtual Color GetGraphColor(int low, int high, int value)
        {
            return GetColor(low, high, value);
        }

        public Color GetColor(int low, int high, int value)
        {
            Color result;
            if (value >= high)
                result = HighColor;
            else if (value <= low)
                result = LowColor;
            else
                result = NormalColor;
            return result;
        }

        public void SetSymbolColor(Bitmap symbols, bool useColor, int low, int high, int value)
        {
            Color color = !useColor || value == 0 ? Color.White : GetColor(low, high, value);

            for (int x = 0; x < symbols.Width; x++)
            {
                for (int y = 0; y < symbols.Height; y++)
                {
                    if (symbols.GetPixel(x, y).A == 255)
                    {
                        symbols.SetPixel(x, y, color);
                    }
                }
            }
        }

        protected double IntervalScale(double minA, double maxA, double valueA, double minB, double maxB)
        {
            double rangeA = maxA - minA;
            double rangeB = maxB - minB;
            double relVAlue = (valueA - minA) / rangeA;
            double result = minB + (rangeB * relVAlue);
            return result;
        }
    }
}
