using System.Drawing;

namespace Trayscout
{
    public class DexcomStyle : DotStyle
    {
        public DexcomStyle()
        {
            LowColor = Color.Crimson;
            HighColor = Color.FromArgb(255, 210, 10);
            NormalColor = Color.Gray;
            BackgroundColor = Color.White;
            HelpLinesColor = Color.Black;
            AnnotationColor = Color.Black;
            Radius = 3;
        }

        protected override Color GetGraphColor(int low, int high, int value)
        {
            return Color.Black;
        }

        protected override void DrawAreas(Graphics g, int width, int height, int lowY, int highY, int alpha)
        {
            g.FillRectangle(new SolidBrush(Color.FromArgb(alpha / 2, HighColor)), 0, highY, width - 1, height - highY);
            g.FillRectangle(new SolidBrush(Color.FromArgb(alpha * 2, NormalColor)), 0, lowY, width - 1, highY - lowY);
            g.FillRectangle(new SolidBrush(Color.FromArgb(alpha, LowColor)), 0, 0, width - 1, lowY);
            g.DrawLine(new Pen(Color.White), 0, highY, width - 1, highY);
            g.DrawLine(new Pen(Color.White), 0, lowY, width - 1, lowY);
        }
    }
}
