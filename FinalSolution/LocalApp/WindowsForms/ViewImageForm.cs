using System;
using System.Drawing;
using System.Windows.Forms;

namespace LocalApp.WindowsForms
{
    public partial class ViewImageForm : Form
    {
        private readonly Bitmap image;
        private int width;
        private int height;
        public ViewImageForm(Bitmap image)
        {
            this.image = image;
            InitializeComponent();
        }

        private void ViewImageForm_Load(object sender, System.EventArgs e)
        {
            // Define size
            width = Console.WindowWidth * 3 / 4 * 8;
            height = Console.WindowHeight * 5 / 6 * 16;

            // Styling
            ControlBox = false;
            FormBorderStyle = FormBorderStyle.None;

            // set window to size of user area
            MinimumSize = new Size(width, height);
            MaximumSize = new Size(width, height);

            // account for window bar
            Location = new Point(0, 25);

            // Always on top
            TopMost = true;

            // set picture frame
            imageBox.Width = width * 2 / 3 - 10;
            imageBox.Height = height - 20;
            imageBox.SizeMode = PictureBoxSizeMode.StretchImage;
            imageBox.Image = image;

            nextButton.Width = width / 3 - 20;
            nextButton.Height = height - 20;
            nextButton.Left = width * 2 / 3 + 10;

        }

        private void nextButton_Click(object sender, System.EventArgs e)
        {
            Close();
        }
    }
}
