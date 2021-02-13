using System.Drawing;

namespace Trayscout
{
    public class LightStyle : LineStyle
    {
        public LightStyle()
        {
            LowColor = Color.DodgerBlue;
            HighColor = Color.Crimson;
            NormalColor = Color.LimeGreen;
            BackgroundColor = Color.White;
            GridLinesColor = Color.Black;
            LabelColor = Color.Black;
            Radius = 2;
        }

        protected override Color GetGraphColor(int low, int high, int value)
        {
            return Color.Black;
        }

        protected override void DrawAreas(Graphics g, int width, int height, int lowY, int highY, int alpha)
        {
            g.FillRectangle(new SolidBrush(Color.FromArgb(alpha, HighColor)), 0, highY, width - 1, height - highY);
            g.FillRectangle(new SolidBrush(Color.FromArgb(alpha, NormalColor)), 0, lowY, width - 1, highY - lowY);
            g.FillRectangle(new SolidBrush(Color.FromArgb(alpha, LowColor)), 0, 0, width - 1, lowY);
            g.DrawLine(new Pen(HighColor), 0, highY, width - 1, highY);
            g.DrawLine(new Pen(LowColor), 0, lowY, width - 1, lowY);
        }
    }
}
