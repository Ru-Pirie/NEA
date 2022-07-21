using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace FinalSolution.src.local.forms
{
    public partial class ShowImage : Form
    {
        private Bitmap _image;
        private string _content;

        public ShowImage(Bitmap image, string content)
        {
            this.ControlBox = false;

            _image = image;
            _content = content;

            InitializeComponent();
        }

        private void ShowImage_Load(object sender, EventArgs e)
        {
            imageBox.SizeMode = PictureBoxSizeMode.StretchImage;
            imageBox.Image = _image;
            content.Text = _content;
        }

        private void next_Click(object sender, EventArgs e)
        {
            Close();
        }
    }
}
