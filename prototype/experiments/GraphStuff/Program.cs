using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace GraphStuff
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Dictionary<string, List<string>> temp = new Dictionary<string, List<string>>();
            temp.Add("A", new List<string>
            {
                "D"
            });
            temp.Add("B", new List<string>
            {
                "C", "F"
            });
            temp.Add("C", new List<string>
            {
                "B"
            });
            temp.Add("D", new List<string>
            {
                "A", "E", "G"
            });
            temp.Add("E", new List<string>
            {
                "D", "H"
            });
            temp.Add("F", new List<string>
            {
                "B", "G"
            });
            temp.Add("G", new List<string>
            {
                "D", "F"
            });
            temp.Add("H", new List<string>
            {
                "E"
            });

            Graph myGraph = new Graph(temp);
            Console.WriteLine(string.Join(", ", DFS("A", myGraph)));
            Console.WriteLine(string.Join(", ", BFS("A", myGraph)));
            Console.ReadLine();
        }

        public static string[] DFS(string start, Graph graph)
        {
            List<string> path = new List<string>();
            Stack<string> stack = new Stack<string>();
            Dictionary<string, bool> visited = new Dictionary<string, bool>();
            foreach (string s in graph.GetAllNodes()) visited.Add(s, false);

            // Kick Start
            stack.Push(start);

            while (!stack.IsEmpty())
            {
                
                string node = stack.Pop();
                path.Add(node);
                visited[node] = true;

                List<string> connections = graph.GetNode(node);

                connections.Reverse();

                foreach (string s in connections)
                {
                    if (visited[s] == false)
                    {
                        stack.Push(s);
                    }
                }
            }


            return path.ToArray();
        }

        public static string[] BFS(string start, Graph graph)
        {
            List<string> path = new List<string>();
            Queue<string> stack = new Queue<string>();
            Dictionary<string, bool> visited = new Dictionary<string, bool>();
            foreach (string s in graph.GetAllNodes()) visited.Add(s, false);

            // Kick Start
            stack.Enqueue(start);

            while (!stack.IsEmpty())
            {

                string node = stack.Dequeue();
                path.Add(node);
                visited[node] = true;

                List<string> connections = graph.GetNode(node);

                connections.Reverse();

                foreach (string s in connections)
                {
                    if (visited[s] == false)
                    {
                        stack.Enqueue(s);
                    }
                }
            }

            return path.ToArray();
        }
    }

    public class Queue<T>
    {
        public List<T> _data = new List<T>();

        public T Dequeue()
        {
            T val = _data[0];
            _data.RemoveAt(0);
            return val;
        }

        public void Enqueue(T val) => _data.Add(val);

        public bool IsEmpty() => _data.Count == 0;
    }

    public class Stack<T>
    {
        public List<T> _data = new List<T>();

        public T Pop()
        {
            T val = _data[_data.Count - 1];
            _data.RemoveAt(_data.Count - 1);
            return val;
        }

        public void Push(T val) => _data.Add(val);

        public bool IsEmpty() => _data.Count == 0;

    }


    public class Graph
    {
        public Dictionary<string, List<string>> _data = new Dictionary<string, List<string>>();

        public Graph(Dictionary<string, List<string>> graph)
        {
            _data = graph;
        }

        public void AddNode(string name)
        {
            if (_data.ContainsKey(name)) throw new GraphException($"Cannot add {name}, node already exists.");
            _data.Add(name, new List<string>());
        }

        public void RemoveNode(string name)
        {
            if (!_data.ContainsKey(name)) throw new GraphException($"Cannot remove {name}, node does not exist.");
            _data.Remove(name);
        }

        public void AddConnection(string node, string name)
        {
            if (!_data.ContainsKey(node)) throw new GraphException($"Cannot add connection {name} to {node} original node does not exist.");
            if (_data[node].Contains(name)) throw new GraphException($"Cannot add connection {name} to {node} connection already exists.");
            _data[node].Add(name);
        }

        public List<string> GetNode(string node)
        {
            if (!_data.ContainsKey(node)) throw new GraphException($"Node {node} does not exist.");
            return _data[node];
        }

        public string[] GetAllNodes() => _data.Keys.ToArray();

        public void Clear() => _data.Clear();
    }

    public class GraphException : Exception
    {
        public GraphException(string message) : base(message)
        {
        }
    }
}
