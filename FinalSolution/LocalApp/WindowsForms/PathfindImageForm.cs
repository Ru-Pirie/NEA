using BackendLib;
using BackendLib.Data;
using BackendLib.Datatypes;
using LocalApp.CLI;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace LocalApp.WindowsForms
{
    public partial class PathfindImageForm : Form
    {
        private readonly Structures.Coord invalidCord = new Structures.Coord { X = -1, Y = -1 };

        private Bitmap _image;
        private readonly Bitmap _originalImage;
        private int _width;
        private int _height;

        private readonly Graph<Structures.Coord> _graph;

        private readonly Traversal<Structures.Coord> _traversalObject;

        private Structures.Coord prevStartNode;
        private Structures.Coord startNode = new Structures.Coord { X = -1, Y = -1 };
        private Structures.Coord endNode = new Structures.Coord { X = -1, Y = -1 };

        private Dictionary<Structures.Coord, Structures.Coord> _preCalculatedTree;

        public PathfindImageForm(Bitmap image, Traversal<Structures.Coord> traversal, Graph<Structures.Coord> graph)
        {
            _image = image;
            _originalImage = image;
            _traversalObject = traversal;
            _graph = graph;

            InitializeComponent();
        }

        private void ViewImageForm_Load(object sender, EventArgs e)
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
            if (bool.Parse(Settings.UserSettings["forceFormsFront"].Item1)) TopMost = true;

            // set picture frame
            imageBox.Width = _width * 2 / 3 - 12;
            imageBox.Height = _height - 24;
            imageBox.SizeMode = PictureBoxSizeMode.StretchImage;
            imageBox.Image = _image;

            // Set Pathfind Button
            goButton.Width = _width / 3 - 24;
            goButton.Height = (_height / 4 - 24) / 2;
            goButton.Left = _width * 2 / 3 + 12;
            goButton.Top = _height * 3 / 4;

            // Set Exit Button
            exitButton.Width = _width / 3 - 24;
            exitButton.Height = (_height / 4 - 24) / 2;
            exitButton.Left = _width * 2 / 3 + 12;
            exitButton.Top = (_height * 3 / 4 + ((_height / 4 - 24) / 2)) + 12;
            //exitButton.Top = _height * 9 / 10 - 12;

            // Set instruction box
            textBox.Width = _width / 3 - 24;
            textBox.Height = _height * 3 / 4 - 24;
            textBox.Left = _width * 2 / 3 + 12;

            // Set running box
            runningBox.Width = _width / 3 - 24;
            runningBox.Height = _height * 2 / 4 - 24;
            runningBox.Left = _width * 2 / 3 + 12;
            runningBox.Visible = false;
            SetRunningBox();

            // Set working button
            workingButton.Width = _width / 3 - 24;
            workingButton.Height = _height / 2 - 12;
            workingButton.Left = _width * 2 / 3 + 12;
            workingButton.Top = _height / 2;
            workingButton.Visible = false;
        }

        private Structures.Coord ConvertImageBoxToBitmapCord(Point location)
        {
            int x = (int)(((double)_image.Width / imageBox.Width) * location.X);
            int y = (int)(((double)_image.Height / imageBox.Height) * location.Y);

            return new Structures.Coord { X = x, Y = y };
        }

        private void RedrawImage()
        {
            _image = new Bitmap(_originalImage);
            if (startNode != invalidCord)
            {
                if (_graph.GetNode(startNode).Count == 0 && bool.Parse(Settings.UserSettings["snapToGrid"].Item1))
                {
                    double value = Double.MaxValue;
                    Structures.Coord smallest = new Structures.Coord { X = int.MaxValue, Y = int.MaxValue };
                    foreach (Structures.Coord node in _graph.GetAllNodes())
                    {
                        double compare = Math.Sqrt(Math.Pow(startNode.X - node.X, 2) + Math.Pow(startNode.Y - node.Y, 2));
                        if (compare < value && _graph.GetNode(node).Count != 0)
                        {
                            smallest = node;
                            value = compare;
                        }
                    }

                    startNode = smallest;
                }

                DrawCross(startNode, Color.Green);
            }

            if (endNode != invalidCord)
            {
                if (_graph.GetNode(endNode).Count == 0 && bool.Parse(Settings.UserSettings["snapToGrid"].Item1))
                {
                    double value = Double.MaxValue;
                    Structures.Coord smallest = new Structures.Coord { X = int.MaxValue, Y = int.MaxValue };
                    foreach (Structures.Coord node in _graph.GetAllNodes())
                    {
                        double compare = Math.Sqrt(Math.Pow(endNode.X - node.X, 2) + Math.Pow(endNode.Y - node.Y, 2));
                        if (compare < value && _graph.GetNode(node).Count != 0)
                        {
                            smallest = node;
                            value = compare;
                        }
                    }

                    endNode = smallest;
                }
                DrawCross(endNode, Color.Red);
            }

            imageBox.Image = _image;
        }

        private void DrawCross(Structures.Coord center, Color colour)
        {
            double xRatio = (double)_image.Width / imageBox.Width;
            double yRatio = (double)_image.Height / imageBox.Height;

            for (int x = center.X - (int)(2 * xRatio); x <= center.X + (int)(2 * xRatio); x++)
            {
                for (int y = center.Y - (int)(10 * yRatio); y <= center.Y + (int)(10 * yRatio); y++)
                {
                    if (y >= 0 && y < _image.Height && x >= 0 && x < _image.Width)
                    {
                        _image.SetPixel(x, y, colour);
                    }
                }
            }

            for (int y = center.Y - (int)(2 * yRatio); y <= center.Y + (int)(2 * yRatio); y++)
            {
                for (int x = center.X - (int)(10 * xRatio); x <= center.X + (int)(10 * xRatio); x++)
                {
                    if (x >= 0 && x < _image.Width && y >= 0 && y < _image.Height)
                    {
                        _image.SetPixel(x, y, colour);
                    }
                }
            }
        }

        private void imageBox_Click(object sender, EventArgs eventArgs)
        {
            MouseEventArgs mouseEvent = (MouseEventArgs)eventArgs;
            Structures.Coord clickCord = ConvertImageBoxToBitmapCord(mouseEvent.Location);

            if (mouseEvent.Button == MouseButtons.Left) if (startNode != clickCord) startNode = clickCord;
            if (mouseEvent.Button == MouseButtons.Right) if (endNode != clickCord) endNode = clickCord;

            RedrawImage();
        }

        private void exitButton_Click(object sender, EventArgs e)
        {
            Close();
        }



        private void SetRunningBox()
        {
            string snapWarning = String.Empty;
            if (!bool.Parse(Settings.UserSettings["snapToGrid"].Item1))
                snapWarning = "(Warning can cause broken routes. To change goto settings -> pathfinding -> snapToGrid)\n";

            string mstWarning = String.Empty;
            if (bool.Parse(Settings.UserSettings["convertToMST"].Item1))
                mstWarning = "(Warning can cause non-optimal routes. To change goto settings -> pathfinding -> convertToMST)\n";

            string endWarning = String.Empty;
            if (bool.Parse(Settings.UserSettings["endOnFind"].Item1))
                endWarning = "(Warning causes longer times from different start nodes. To change goto settings -> pathfinding -> endOnFind)\n";


            runningBox.Text = "Current Pathfinding Settings\n\n" +
                              $"\nAlgorithm: {Settings.UserSettings["pathfindingAlgorithm"].Item1}" +
                              $"\n\nUsing Minimum Spanning Tree: {(Settings.UserSettings["convertToMST"].Item1 == "true" ? "Yes" : "No")}" +
                              $"\n{mstWarning}" +
                              $"\nSnapping to grid: {(Settings.UserSettings["snapToGrid"].Item1 == "true" ? "Yes" : "No")}" +
                              $"\n{snapWarning}" +
                              $"\nEnd pathfinding on Finding End (Dijkstra Only): {(Settings.UserSettings["endOnFind"].Item1 == "true" ? "Yes" : "No")}" +
                              $"\n{endWarning}";
        }

        private int GetDistanceBetweenNodes(Structures.Coord start, Structures.Coord goal) => (int)Utility.GetDistanceBetweenNodes(start, goal);

        private void goButton_Click(object sender, EventArgs e)
        {
            workingButton.Visible = !workingButton.Visible;
            textBox.Visible = false;
            runningBox.Visible = true;
            Update();

            if (startNode != invalidCord && endNode != invalidCord)
            {
                if (Settings.UserSettings["pathfindingAlgorithm"].Item1.ToLower() == "dijkstra")
                {
                    if (prevStartNode != startNode && startNode != endNode || bool.Parse(Settings.UserSettings["endOnFind"].Item1) == true)
                    {
                        Dictionary<Structures.Coord, Structures.Coord> tree = _traversalObject.Dijkstra(startNode, endNode, bool.Parse(Settings.UserSettings["endOnFind"].Item1));
                        Structures.Coord[] path = Utility.RebuildPath(tree, endNode);
                        foreach (Structures.Coord node in path)
                        {
                            _image.SetPixel(node.X, node.Y, Color.BlueViolet);
                            imageBox.Image = _image;
                        }

                        _preCalculatedTree = tree;
                    }
                    else if (prevStartNode == startNode && startNode != endNode)
                    {
                        Structures.Coord[] path = Utility.RebuildPath(_preCalculatedTree, endNode);
                        foreach (Structures.Coord node in path)
                        {
                            _image.SetPixel(node.X, node.Y, Color.BlueViolet);
                            imageBox.Image = _image;

                        }
                    }
                }
                else if (Settings.UserSettings["pathfindingAlgorithm"].Item1.ToLower() == "astar")
                {
                    Dictionary<Structures.Coord, Structures.Coord> tree = _traversalObject.AStar(startNode, endNode, GetDistanceBetweenNodes);
                    Structures.Coord[] path = Utility.RebuildPath(tree, endNode);
                    foreach (Structures.Coord node in path)
                    {
                        _image.SetPixel(node.X, node.Y, Color.BlueViolet);
                        imageBox.Image = _image;
                    }

                    _preCalculatedTree = tree;

                }

                prevStartNode = startNode;
                workingButton.Visible = !workingButton.Visible;
                textBox.Visible = true;
                runningBox.Visible = false;
            }
        }
    }
}
