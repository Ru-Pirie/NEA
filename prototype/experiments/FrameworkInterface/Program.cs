using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FrameworkInterface
{
    internal class Program
    {
        static void Main(string[] args)
        {
            ImageInterface thing = new ImageInterface();
            thing.ShowDialog();
        }
    }
}
