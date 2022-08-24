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
    public partial class PreviewImage : Form
    {
        private Bitmap _image;
        private string _current;
        private string _next;
        private string[] _parameters;

        public PreviewImage(Bitmap image, string current, string next, string[] parameters)
        {
            this.ControlBox = false;

            _image = image;
            _current = current;
            _next = next;
            _parameters = parameters;

            InitializeComponent();
        }
        
        private void PreviewImage_Load(object sender, EventArgs e)
        {
            imageBox.SizeMode = PictureBoxSizeMode.StretchImage;
            imageBox.Image = _image;

            currentStage.Text = _current;
            nextStage.Text = _next;

            parameters.Text = string.Join("\n", _parameters);
        }

        private void back_Click(object sender, EventArgs e)
        {

        }

        private void next_Click(object sender, EventArgs e)
        {

        }
    }
}
