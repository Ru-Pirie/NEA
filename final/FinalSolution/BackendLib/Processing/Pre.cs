using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BackendLib.Processing
{
    internal class Pre
    {
        private string _imagePath;
        private Bitmap _imageBitmap;
        private Structures.RGB[,] _imageRGB;

        public Pre(string imagePath)
        {
            _imagePath = imagePath;
        }

        public void Start()
        {

        }

    }
}
