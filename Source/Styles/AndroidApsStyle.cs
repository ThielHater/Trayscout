using System.Drawing;

namespace Trayscout
{
    public class AndroidApsStyle : DotStyle
    {
        public AndroidApsStyle()
        {
            LowColor = Color.Red;
            HighColor = Color.Yellow;
            NormalColor = Color.Green;
            BackgroundColor = Color.FromArgb(33, 33, 33);
            HelpLinesColor = Color.Gray;
            AnnotationColor = Color.LightGray;
            Radius = 4;
        }

        protected override void DrawAreas(Graphics g, int width, int height, int lowY, int highY, int alpha)
        {
            g.FillRectangle(new SolidBrush(Color.FromArgb(alpha, NormalColor)), 0, lowY, width - 1, highY - lowY);
            g.DrawLine(new Pen(NormalColor), 0, highY, width - 1, highY);
            g.DrawLine(new Pen(NormalColor), 0, lowY, width - 1, lowY);
        }
    }
}
