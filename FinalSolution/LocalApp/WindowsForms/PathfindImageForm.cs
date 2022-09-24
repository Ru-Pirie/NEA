using System;
using System.Drawing;
using System.Windows.Forms;

namespace LocalApp.WindowsForms
{
    public partial class PathfindImageForm : Form
    {
        private readonly Bitmap _image;
        private int _width;
        private int _height;
        public PathfindImageForm(Bitmap image)
        {
            this._image = image;
            InitializeComponent();
        }

        private void ViewImageForm_Load(object sender, System.EventArgs e)
        {
            // Define size
            _width = Console.WindowWidth * 3 / 4 * 8;
            _height = Console.WindowHeight * 5 / 6 * 16;

            // Styling
            ControlBox = false;
            FormBorderStyle = FormBorderStyle.None;

            // set window to size of user area
            MinimumSize = new Size(_width, _height);
            MaximumSize = new Size(_width, _height);

            // account for window bar
            Location = new Point(0, 25);

            // Always on top
            TopMost = true;

            // set picture frame
            imageBox.Width = _width - 24;
            imageBox.Height = _height - 24;
            imageBox.SizeMode = PictureBoxSizeMode.Zoom;
            imageBox.Image = _image;

        }

        private void nextButton_Click(object sender, System.EventArgs e)
        {
            Close();
        }
    }
}
