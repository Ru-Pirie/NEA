using BackendLib;
using BackendLib.Data;
using BackendLib.Datatypes;
using LocalApp.WindowsForms;
using System.Drawing;

namespace LocalApp.Processes
{
    public class Pathfinder
    {
        private readonly double[,] _input;
        private readonly Bitmap _originalBitmap;

        private Graph<Structures.Coord> _graph;
        private Traversal<Structures.Coord> _traversal;

        public Pathfinder(Bitmap originalImage, double[,] input)
        {
            _originalBitmap = originalImage;
            _input = input;
        }

        public void Start()
        {
            InstanceClasses();

            PathfindImageForm pathfindForm = new PathfindImageForm(_originalBitmap, _traversal, _graph);
            pathfindForm.ShowDialog();
        }

        private void InstanceClasses()
        {
            _graph = _input.ToGraph();
            _traversal = new Traversal<Structures.Coord>(_graph);
        }



    }
}
