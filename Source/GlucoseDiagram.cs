using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace Trayscout
{
    public partial class GlucoseDiagram : Form
    {
        public GlucoseDiagram(Configuration config, IList<Entry> entries)
        {
            InitializeComponent();

            Width = config.Width;
            Height = config.Height;
            PictureBox.Width = config.Width;
            PictureBox.Height = config.Height;

            Size screenSize = Screen.PrimaryScreen.WorkingArea.Size;
            Location = new Point(screenSize.Width - Width - 16, screenSize.Height - Height - 16);

            PictureBox.Image = config.Style.DrawDiagram(config, entries);
        }
    }
}
