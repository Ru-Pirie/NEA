using BackendLib.Datatypes;
using System.Collections.Generic;

namespace BackendLib.Data
{
    public class Traversal<T>
    {
        private Graph<T> _graph;

        public Traversal(Graph<T> graph)
        {
            _graph = graph;
        }

        public T[] DFS(T start)
        {
            List<T> path = new List<T>();
            Datatypes.Stack<T> stack = new Datatypes.Stack<T>();
            Dictionary<T, bool> visited = new Dictionary<T, bool>();
            foreach (T s in _graph.GetAllNodes()) visited.Add(s, false);

            // Kick Start
            stack.Push(start);

            while (!stack.IsEmpty())
            {
                T node = stack.Pop();
                path.Add(node);
                visited[node] = true;

                List<T> connections = _graph.GetNode(node);

                connections.Reverse();

                foreach (T s in connections)
                {
                    if (visited[s] == false && !stack.Contains(s))
                    {
                        stack.Push(s);
                    }
                }
            }


            return path.ToArray();
        }

        public T[] BFS(T start)
        {
            List<T> path = new List<T>();
            Datatypes.Queue<T> queue = new Datatypes.Queue<T>();
            Dictionary<T, bool> visited = new Dictionary<T, bool>();
            foreach (T s in _graph.GetAllNodes()) visited.Add(s, false);

            // Kick Start
            queue.Enqueue(start);

            while (!queue.IsEmpty())
            {

                T node = queue.Dequeue();
                path.Add(node);
                visited[node] = true;

                List<T> connections = _graph.GetNode(node);

                connections.Reverse();

                foreach (T s in connections)
                {
                    if (visited[s] == false && !queue.Contains(s))
                    {
                        queue.Enqueue(s);
                    }
                }
            }

            return path.ToArray();
        }

        public T[] AStar(T start, T goal)
        {
            Datatypes.Queue<T> openSet = new Datatypes.Queue<T>(new[] { start });
            Datatypes.Queue<T> cameFrom = new Datatypes.Queue<T>();

            int gScore = 0;
            int fScore = 1;


            while (!openSet.IsEmpty())
            {
                T current = openSet.Dequeue();

                if (current.Equals(goal))
                {

                }

            }

            return new T[1];
        }

        public T[] Dijkstra(T start, T goal)
        {




            return new T[1];
        }
    }
}