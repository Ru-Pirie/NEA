using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace FrameworkInterface
{
    public partial class ImageInterface : Form
    {
        public ImageInterface()
        {
            InitializeComponent();
            Bitmap tmp = new Bitmap(Box.Width, Box.Height);

            for (int i = 0; i < tmp.Width; i++)
            {
                for (int j = 0; j < tmp.Height; j++)
                {
                    tmp.SetPixel(i, j, Color.Black);
                }
            }

            Box.Image = tmp;
        }

        private void Box_MouseDown(object sender, MouseEventArgs e)
        {
            Bitmap image = (Bitmap)Box.Image;
            Fill(image, (e.X / 10) * 10, (e.Y / 10) * 10);
            Box.Image = image;
        }

        private void Fill(Bitmap img, int x, int y)
        {
            for (int i = 0; i < 10; i++)
            {
                for (int j = 0; j < 10; j++)
                {
                    img.SetPixel(x + i, y + j, Color.Red);
                }
            }
        }
    }
}
