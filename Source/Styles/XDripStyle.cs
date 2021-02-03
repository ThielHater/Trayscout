using System.Drawing;

namespace Trayscout
{
    public class XDripStyle : DotStyle
    {
        public XDripStyle()
        {
            LowColor = Color.Crimson;
            HighColor = Color.DarkOrange;
            NormalColor = Color.DodgerBlue;
            BackgroundColor = Color.FromArgb(33, 33, 33);
            HelpLinesColor = Color.Gray;
            AnnotationColor = Color.LightGray;
            Radius = 3;
        }

        protected override void DrawAreas(Graphics g, int width, int height, int lowY, int highY, int alpha)
        {
            g.DrawLine(new Pen(HighColor), 0, highY, width - 1, highY);
            g.DrawLine(new Pen(LowColor), 0, lowY, width - 1, lowY);
            g.FillRectangle(new SolidBrush(Color.FromArgb(alpha, LowColor)), 0, 0, width - 1, lowY);
        }
    }
}
